using HarborFlow;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

public class HarborService
{
    private readonly HarborFlowDbContext _context;

    public HarborService(HarborFlowDbContext context)
    {
        _context = context;
    }

    public async Task<User?> RegisterAsync(string username, string password)
    {
        var newUser = new ShippingAgent // Default to creating a Shipping-Agent
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.ShippingAgent
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }

    // 1. Method to add a new Vessel
    public async Task<Vessel> AddVesselAsync(string name, string imoNumber, string type, double capacity)
    {
        var newVessel = new Vessel
        {
            VesselName = name,
            IMONumber = imoNumber,
            Type = type,
            Capacity = capacity
        };

        _context.Vessels.Add(newVessel);
        await _context.SaveChangesAsync();

        return newVessel;
    }

    // 2. Method to create a new Service Request
    public async Task<ServiceRequest?> CreateServiceRequestAsync(int vesselId, int createdByUserId, string serviceType)
    {
        var vessel = await _context.Vessels.FindAsync(vesselId);
        var user = await _context.Users.FindAsync(createdByUserId);

        if (vessel == null || user == null)
        {
            return null;
        }

        var newRequest = new ServiceRequest
        {
            RequestDate = DateTime.UtcNow,
            EstimatedArrival = DateTime.UtcNow.AddDays(5),
            ServiceType = serviceType,
            Status = RequestStatus.Pending,
            VesselID = vesselId,
            CreatedByUserID = createdByUserId
        };

        _context.ServiceRequests.Add(newRequest);
        await _context.SaveChangesAsync();

        return newRequest;
    }

    // 3. Method to get all Vessels
    public async Task<List<Vessel>> GetAllVesselsAsync()
    {
        return await _context.Vessels.ToListAsync();
    }

    // 4. Method to create a new Schedule for a Service Request
    public async Task<Schedule?> CreateScheduleAsync(int serviceRequestId, int berthNumber, DateTime arrival, DateTime departure)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(serviceRequestId);
        if (serviceRequest == null)
        {
            return null;
        }

        var newSchedule = new Schedule
        {
            ServiceRequestID = serviceRequestId,
            BerthNumber = berthNumber,
            ActualArrival = arrival,
            ActualDeparture = departure
        };

        serviceRequest.Status = RequestStatus.Approved;
        _context.Schedules.Add(newSchedule);
        await _context.SaveChangesAsync();

        return newSchedule;
    }

    // 5. Method to get all Schedules
    public async Task<List<Schedule>> GetSchedulesAsync()
    {
        return await _context.Schedules
            .Include(s => s.ServiceRequest)
            .ThenInclude(sr => sr!.Vessel)
            .OrderBy(s => s.ActualArrival)
            .ToListAsync();
    }

    // 6. Method for user login
    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return user;
        }
        return null;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    // 7. Method to add a document to a service request
    public async Task<Document?> AddDocumentToServiceRequestAsync(int serviceRequestId, string documentName, string filePath)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(serviceRequestId);
        if (serviceRequest == null) return null;

        var newDocument = new Document
        {
            ServiceRequestID = serviceRequestId,
            DocumentName = documentName,
            FilePath = filePath,
            IsVerified = false
        };

        _context.Documents.Add(newDocument);
        await _context.SaveChangesAsync();
        return newDocument;
    }

    // 8. Method to get all documents for a service request
    public async Task<List<Document>> GetDocumentsForServiceRequestAsync(int serviceRequestId)
    {
        return await _context.Documents
            .Where(d => d.ServiceRequestID == serviceRequestId)
            .ToListAsync();
    }

    // 9. Method to verify a document
    public async Task<Document?> VerifyDocumentAsync(int documentId)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null) return null;

        document.IsVerified = true;
        await _context.SaveChangesAsync();
        return document;
    }

    // 10. Method to add a cargo item to a service request
    public async Task<Cargo?> AddCargoToRequestAsync(int serviceRequestId, string description, double weight, bool isHazardous)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(serviceRequestId);
        if (serviceRequest == null) return null;

        var newCargo = new Cargo
        {
            ServiceRequestID = serviceRequestId,
            Description = description,
            Weight = weight,
            IsHazardous = isHazardous,
            Status = CargoStatus.Onboard
        };

        _context.Cargos.Add(newCargo);
        await _context.SaveChangesAsync();
        return newCargo;
    }

    // 11. Method to get all cargo for a service request
    public async Task<List<Cargo>> GetCargoForRequestAsync(int serviceRequestId)
    {
        return await _context.Cargos
            .Where(c => c.ServiceRequestID == serviceRequestId)
            .ToListAsync();
    }

    // 12. Method to generate an invoice for a service request
    public async Task<Invoice?> GenerateInvoiceAsync(int serviceRequestId, double totalAmount)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(serviceRequestId);
        if (serviceRequest == null || serviceRequest.Invoice != null) return null;

        var newInvoice = new Invoice
        {
            ServiceRequestID = serviceRequestId,
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{serviceRequestId}",
            IssueDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = InvoiceStatus.Draft
        };

        _context.Invoices.Add(newInvoice);
        await _context.SaveChangesAsync();
        return newInvoice;
    }

    // 13. Method to record a payment for an invoice
    public async Task<Payment?> RecordPaymentAsync(int invoiceId, double amount, string paymentMethod)
    {
        var invoice = await _context.Invoices.FindAsync(invoiceId);
        if (invoice == null || invoice.Status == InvoiceStatus.Paid) return null;

        var newPayment = new Payment
        {
            InvoiceID = invoiceId,
            PaymentDate = DateTime.UtcNow,
            Amount = amount,
            PaymentMethod = paymentMethod
        };

        invoice.Status = InvoiceStatus.Paid;
        _context.Payments.Add(newPayment);
        await _context.SaveChangesAsync();
        return newPayment;
    }
}
