using HarborFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await RunApplicationLogic(host.Services);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<HarborFlowDbContext>(options => options.UseNpgsql(connectionString));
                services.AddScoped<HarborService>();
            });

    private static async Task RunApplicationLogic(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var harborService = scope.ServiceProvider.GetRequiredService<HarborService>();
        var context = scope.ServiceProvider.GetRequiredService<HarborFlowDbContext>();

        AnsiConsole.MarkupLine("[bold green]Ensuring database is created and seeding users...[/]");
        await context.Database.EnsureCreatedAsync();
        await SeedInitialUsersAsync(context);
        AnsiConsole.MarkupLine("[bold green]Database is ready.[/]");
        AnsiConsole.WriteLine();

        var currentUser = await LoginUI(harborService);

        if (currentUser != null)
        {
            await ShowMainMenuUI(currentUser, harborService, context);
        }

        AnsiConsole.MarkupLine("[bold blue]Exiting application. Goodbye![/]");
    }

    private static async Task<User?> LoginUI(HarborService harborService)
    {
        AnsiConsole.MarkupLine("[bold yellow]Welcome to HarborFlow. Please log in.[/]");
        while (true)
        {
            var username = AnsiConsole.Ask<string>("Username:");
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Password:")
                    .Secret());

            var user = await harborService.LoginAsync(username, password);
            if (user != null)
            {
                AnsiConsole.MarkupLine($"[bold green]Login successful! Welcome, {user.Username} ({user.Role}).[/]");
                Console.ReadKey();
                return user;
            }
            
            AnsiConsole.MarkupLine("[bold red]Invalid username or password. Please try again.[/]");
            if (!AnsiConsole.Confirm("Try again?"))
            {
                return null;
            }
        }
    }

    private static async Task ShowMainMenuUI(User currentUser, HarborService harborService, HarborFlowDbContext context)
    {
        var keepRunning = true;
        while (keepRunning)
        {
            AnsiConsole.Clear();
            var menuChoices = new List<string>();
            
            // Role-based menu construction
            menuChoices.Add("Show All Vessels");
            menuChoices.Add("View All Schedules");

            if (currentUser.Role == UserRole.ShippingAgent)
            {
                menuChoices.Add("Create Service Request");
                menuChoices.Add("Add Document to Request");
                menuChoices.Add("Manage Cargo");
            }

            if (currentUser.Role == UserRole.PortManager)
            {
                menuChoices.Add("Add New Vessel");
                menuChoices.Add("Create Schedule");
                menuChoices.Add("Verify Document");
            }

            if (currentUser.Role == UserRole.FinanceAdmin)
            {
                menuChoices.Add("Generate Invoice");
                menuChoices.Add("Record Payment");
            }
            
            menuChoices.Add("Exit");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[bold yellow]HarborFlow Menu[/] (Logged in as [blue]{currentUser.Username}[/])")
                    .PageSize(10)
                    .AddChoices(menuChoices));

            switch (choice)
            {
                case "Add New Vessel":
                    await AddNewVesselUI(harborService);
                    break;
                case "Show All Vessels":
                    await ShowAllVesselsUI(harborService);
                    break;
                case "Create Service Request":
                    await CreateServiceRequestUI(harborService, currentUser);
                    break;
                case "Add Document to Request":
                    await AddDocumentUI(harborService, context, currentUser);
                    break;
                case "Manage Cargo":
                    await ManageCargoUI(harborService, context, currentUser);
                    break;
                case "Create Schedule":
                    await CreateScheduleUI(harborService, context);
                    break;
                case "Verify Document":
                    await VerifyDocumentUI(harborService, context);
                    break;
                case "View All Schedules":
                    await ViewAllSchedulesUI(harborService);
                    break;
                case "Generate Invoice":
                    await GenerateInvoiceUI(harborService, context);
                    break;
                case "Record Payment":
                    await RecordPaymentUI(harborService, context);
                    break;
                case "Exit":
                    keepRunning = false;
                    break;
            }

            if (keepRunning)
            {
                AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
                Console.ReadKey();
            }
        }
    }

    private static async Task SeedInitialUsersAsync(HarborFlowDbContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            var manager = new PortManager { Username = "manager", PasswordHash = "pass", Role = UserRole.PortManager };
            var agent = new ShippingAgent { Username = "agent", PasswordHash = "pass", Role = UserRole.ShippingAgent };
            var finance = new FinanceAdmin { Username = "finance", PasswordHash = "pass", Role = UserRole.FinanceAdmin };
            context.Users.AddRange(manager, agent, finance);
            await context.SaveChangesAsync();
            AnsiConsole.MarkupLine("[grey]Seeded initial users (manager/pass, agent/pass, finance/pass).[/]");
        }
    }

    // --- UI Helper Methods ---

    private static async Task GenerateInvoiceUI(HarborService harborService, HarborFlowDbContext context)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]Generate Invoice[/]");
        var completableRequests = await context.ServiceRequests
            .Where(sr => sr.Status == RequestStatus.Approved && sr.Invoice == null)
            .ToListAsync();

        if (!completableRequests.Any())
        {
            AnsiConsole.MarkupLine("[red]No approved service requests are available to invoice.[/]");
            return;
        }

        var requestChoices = completableRequests.Select(r => $"{r.RequestID}: {r.ServiceType} for Vessel ID {r.VesselID}").ToArray();
        var selectedRequestStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a service request to invoice:")
                .AddChoices(requestChoices));
        var requestId = int.Parse(selectedRequestStr.Split(':')[0]);

        var amount = AnsiConsole.Ask<double>("Enter total amount for the invoice:");

        var invoice = await harborService.GenerateInvoiceAsync(requestId, amount);
        if (invoice != null)
        {
            AnsiConsole.MarkupLine($"[green]Successfully generated invoice {invoice.InvoiceNumber}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to generate invoice. The request might already have one.[/]");
        }
    }

    private static async Task RecordPaymentUI(HarborService harborService, HarborFlowDbContext context)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]Record Payment[/]");
        var unpaidInvoices = await context.Invoices
            .Where(i => i.Status != InvoiceStatus.Paid)
            .ToListAsync();

        if (!unpaidInvoices.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No unpaid invoices found.[/]");
            return;
        }

        var invoiceChoices = unpaidInvoices.Select(i => $"{i.InvoiceID}: {i.InvoiceNumber} (Amount: {i.TotalAmount})").ToArray();
        var selectedInvoiceStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select an invoice to record payment for:")
                .AddChoices(invoiceChoices));
        var invoiceId = int.Parse(selectedInvoiceStr.Split(':')[0]);

        var paymentMethod = AnsiConsole.Ask<string>("Enter payment method (e.g., Bank Transfer):");
        var invoice = unpaidInvoices.First(i => i.InvoiceID == invoiceId);

        var payment = await harborService.RecordPaymentAsync(invoiceId, invoice.TotalAmount, paymentMethod);
        if (payment != null)
        {
            AnsiConsole.MarkupLine("[green]Payment recorded successfully.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to record payment. The invoice may already be paid.[/]");
        }
    }

    private static async Task ManageCargoUI(HarborService harborService, HarborFlowDbContext context, User currentUser)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]Manage Cargo[/]");
        var requests = await context.ServiceRequests
            .Where(sr => sr.CreatedByUserID == currentUser.UserID)
            .ToListAsync();

        if (!requests.Any())
        {
            AnsiConsole.MarkupLine("[red]You have no service requests to manage cargo for.[/]");
            return;
        }

        var requestChoices = requests.Select(r => $"{r.RequestID}: {r.ServiceType} (Status: {r.Status})").ToArray();
        var selectedRequestStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a service request:")
                .AddChoices(requestChoices));
        var requestId = int.Parse(selectedRequestStr.Split(':')[0]);

        var cargoAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .AddChoices("Add Cargo", "View Cargo"));

        if (cargoAction == "Add Cargo")
        {
            var description = AnsiConsole.Ask<string>("Enter cargo description:");
            var weight = AnsiConsole.Ask<double>("Enter weight (in tons):");
            var isHazardous = AnsiConsole.Confirm("Is the cargo hazardous?");

            var cargo = await harborService.AddCargoToRequestAsync(requestId, description, weight, isHazardous);
            if (cargo != null)
            {
                AnsiConsole.MarkupLine("[green]Successfully added cargo.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Failed to add cargo.[/]");
            }
        }
        else // View Cargo
        {
            var cargos = await harborService.GetCargoForRequestAsync(requestId);
            if (!cargos.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No cargo found for this service request.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Description");
            table.AddColumn("Weight (Tons)");
            table.AddColumn("Hazardous");
            table.AddColumn("Status");

            foreach (var c in cargos)
            {
                table.AddRow(c.CargoID.ToString(), c.Description, c.Weight.ToString(), c.IsHazardous ? "Yes" : "No", c.Status.ToString());
            }
            AnsiConsole.Write(table);
        }
    }

    private static async Task AddNewVesselUI(HarborService harborService)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]Add a New Vessel[/]");
        var name = AnsiConsole.Ask<string>("Enter vessel name:");
        var imo = AnsiConsole.Ask<string>("Enter IMO number:");
        var type = AnsiConsole.Ask<string>("Enter vessel type (e.g., Bulk Carrier):");
        var capacity = AnsiConsole.Ask<double>("Enter capacity (DWT):");

        var newVessel = await harborService.AddVesselAsync(name, imo, type, capacity);
        AnsiConsole.MarkupLine($"[bold green]Successfully added vessel:[/] {newVessel.VesselName} (ID: {newVessel.VesselID})");
    }

    private static async Task ShowAllVesselsUI(HarborService harborService)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]List of All Vessels[/]");
        var vessels = await harborService.GetAllVesselsAsync();

        if (vessels.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No vessels found in the database.[/]");
            return;
        }

        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("IMO Number");
        table.AddColumn("Type");
        table.AddColumn("Capacity (DWT)");

        foreach (var vessel in vessels)
        {
            table.AddRow(vessel.VesselID.ToString(), vessel.VesselName, vessel.IMONumber, vessel.Type, vessel.Capacity.ToString());
        }

        AnsiConsole.Write(table);
    }

    private static async Task CreateServiceRequestUI(HarborService harborService, User currentUser)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]Create a New Service Request[/]");
        
        var vessels = await harborService.GetAllVesselsAsync();
        if (!vessels.Any())
        {
            AnsiConsole.MarkupLine("[bold red]No vessels available. Please add a vessel first.[/]");
            return;
        }

        var vesselChoices = vessels.Select(v => $"{v.VesselID}: {v.VesselName}").ToArray();
        var selectedVesselStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a vessel for the service request:")
                .AddChoices(vesselChoices));
        
        var vesselId = int.Parse(selectedVesselStr.Split(':')[0]);
        var serviceType = AnsiConsole.Ask<string>("Enter service type (e.g., Port Entry):");

        var request = await harborService.CreateServiceRequestAsync(vesselId, currentUser.UserID, serviceType);
        if (request != null)
        {
            AnsiConsole.MarkupLine($"[bold green]Successfully created service request (ID: {request.RequestID}) for vessel ID {vesselId}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Failed to create service request. Vessel or user not found.[/]");
        }
    }

    private static async Task AddDocumentUI(HarborService harborService, HarborFlowDbContext context, User currentUser)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]Add Document to Service Request[/]");
        var requests = await context.ServiceRequests
            .Where(sr => sr.CreatedByUserID == currentUser.UserID)
            .ToListAsync();

        if (!requests.Any())
        {
            AnsiConsole.MarkupLine("[red]You have no service requests to add documents to.[/]");
            return;
        }

        var requestChoices = requests.Select(r => $"{r.RequestID}: {r.ServiceType} (Status: {r.Status})").ToArray();
        var selectedRequestStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a service request:")
                .AddChoices(requestChoices));
        
        var requestId = int.Parse(selectedRequestStr.Split(':')[0]);
        var docName = AnsiConsole.Ask<string>("Enter document name (e.g., Cargo Manifest):");
        var filePath = AnsiConsole.Ask<string>("Enter file path (e.g., /docs/manifest.pdf):");

        var document = await harborService.AddDocumentToServiceRequestAsync(requestId, docName, filePath);
        if (document != null)
        {
            AnsiConsole.MarkupLine("[green]Successfully added document.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to add document.[/]");
        }
    }

    private static async Task VerifyDocumentUI(HarborService harborService, HarborFlowDbContext context)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]Verify a Document[/]");
        var unverifiedDocs = await context.Documents
            .Where(d => !d.IsVerified)
            .Include(d => d.ServiceRequest)
            .ToListAsync();

        if (!unverifiedDocs.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No documents are pending verification.[/]");
            return;
        }

        var docChoices = unverifiedDocs.Select(d => $"{d.DocumentID}: {d.DocumentName} for Request {d.ServiceRequestID}").ToArray();
        var selectedDocStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a document to verify:")
                .AddChoices(docChoices));
        
        var docId = int.Parse(selectedDocStr.Split(':')[0]);
        var document = await harborService.VerifyDocumentAsync(docId);
        if (document != null)
        {
            AnsiConsole.MarkupLine("[green]Document has been successfully verified.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to verify document.[/]");
        }
    }

    private static async Task CreateScheduleUI(HarborService harborService, HarborFlowDbContext context)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]Create a New Schedule[/]");

        var pendingRequests = await context.ServiceRequests
            .Where(sr => sr.Status == RequestStatus.Pending)
            .Include(sr => sr.Documents) // Eager load documents
            .ToListAsync();

        var schedulableRequests = pendingRequests.Where(sr => sr.Documents.Any() && sr.Documents.All(d => d.IsVerified)).ToList();

        if (!schedulableRequests.Any())
        {
            AnsiConsole.MarkupLine("[bold red]No service requests with all documents verified are available to schedule.[/]");
            return;
        }

        var requestChoices = schedulableRequests.Select(r => $"{r.RequestID}: {r.ServiceType} for Vessel ID {r.VesselID}").ToArray();
        var selectedRequestStr = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select an approved service request to schedule:")
                .AddChoices(requestChoices));
        
        var requestId = int.Parse(selectedRequestStr.Split(':')[0]);
        
        var berth = AnsiConsole.Ask<int>("Enter berth number:");
        var arrival = AnsiConsole.Ask<DateTime>("Enter arrival date and time (YYYY-MM-DD HH:MM):");
        var departure = AnsiConsole.Ask<DateTime>("Enter departure date and time (YYYY-MM-DD HH:MM):");

        var schedule = await harborService.CreateScheduleAsync(requestId, berth, arrival, departure);
        if (schedule != null)
        {
            AnsiConsole.MarkupLine($"[bold green]Successfully created schedule (ID: {schedule.ScheduleID}) and approved service request {requestId}.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Failed to create schedule.[/]");
        }
    }

    private static async Task ViewAllSchedulesUI(HarborService harborService)
    {
        AnsiConsole.MarkupLine("\n[bold underline blue]All Vessel Schedules[/]");
        var schedules = await harborService.GetSchedulesAsync();

        if (!schedules.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No schedules found.[/]");
            return;
        }

        var table = new Table().Centered();
        table.AddColumn("Schedule ID");
        table.AddColumn("Vessel Name");
        table.AddColumn("Service Type");
        table.AddColumn("Berth");
        table.AddColumn("Arrival");
        table.AddColumn("Departure");

        foreach (var s in schedules)
        {
            table.AddRow(
                s.ScheduleID.ToString(), 
                s.ServiceRequest?.Vessel?.VesselName ?? "N/A", 
                s.ServiceRequest?.ServiceType ?? "N/A",
                s.BerthNumber.ToString(),
                s.ActualArrival.ToString("yyyy-MM-dd HH:mm"),
                s.ActualDeparture.ToString("yyyy-MM-dd HH:mm")
            );
        }

        AnsiConsole.Write(table);
    }
}