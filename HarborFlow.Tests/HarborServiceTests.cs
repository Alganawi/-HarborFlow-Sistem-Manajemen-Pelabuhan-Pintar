using HarborFlow;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

public class HarborServiceTests
{
    private HarborFlowDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<HarborFlowDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;
        var dbContext = new HarborFlowDbContext(options);
        return dbContext;
    }

    [Fact]
    public async Task AddVesselAsync_ShouldAddVesselSuccessfully()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var harborService = new HarborService(dbContext);
        var initialCount = await dbContext.Vessels.CountAsync();

        // Act
        await harborService.AddVesselAsync("Test Vessel", "IMO12345", "Container Ship", 50000);
        var newCount = await dbContext.Vessels.CountAsync();

        // Assert
        Assert.Equal(0, initialCount);
        Assert.Equal(1, newCount);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUser_WhenCredentialsAreCorrect()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var testUser = new PortManager { Username = "manager", PasswordHash = "pass" };
        dbContext.Users.Add(testUser);
        await dbContext.SaveChangesAsync();
        
        var harborService = new HarborService(dbContext);

        // Act
        var result = await harborService.LoginAsync("manager", "pass");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("manager", result.Username);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenCredentialsAreIncorrect()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var testUser = new PortManager { Username = "manager", PasswordHash = "pass" };
        dbContext.Users.Add(testUser);
        await dbContext.SaveChangesAsync();
        
        var harborService = new HarborService(dbContext);

        // Act
        var result = await harborService.LoginAsync("manager", "wrong_password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateServiceRequestAsync_ShouldCreateRequestSuccessfully()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var harborService = new HarborService(dbContext);
        
        var testUser = new ShippingAgent { Username = "agent", PasswordHash = "pass" };
        var testVessel = new Vessel { VesselName = "Test Vessel", IMONumber = "IMO12345", Type = "Container Ship", Capacity = 50000 };
        dbContext.Users.Add(testUser);
        dbContext.Vessels.Add(testVessel);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await harborService.CreateServiceRequestAsync(testVessel.VesselID, testUser.UserID, "Docking");
        var requestCount = await dbContext.ServiceRequests.CountAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, requestCount);
        Assert.Equal(testVessel.VesselID, result.VesselID);
        Assert.Equal(testUser.UserID, result.CreatedByUserID);
    }

    [Fact]
    public async Task AddDocumentToServiceRequestAsync_ShouldAddDocumentSuccessfully()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var harborService = new HarborService(dbContext);
        
        var testUser = new ShippingAgent { Username = "agent", PasswordHash = "pass" };
        var testVessel = new Vessel { VesselName = "Test Vessel", IMONumber = "IMO12345", Type = "Container Ship", Capacity = 50000 };
        dbContext.Users.Add(testUser);
        dbContext.Vessels.Add(testVessel);
        await dbContext.SaveChangesAsync();
        var serviceRequest = await harborService.CreateServiceRequestAsync(testVessel.VesselID, testUser.UserID, "Docking");
        Assert.NotNull(serviceRequest);

        // Act
        var result = await harborService.AddDocumentToServiceRequestAsync(serviceRequest.RequestID, "Test Document", "/path/to/doc");
        var documentCount = await dbContext.Documents.CountAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, documentCount);
        Assert.Equal(serviceRequest.RequestID, result.ServiceRequestID);
        Assert.Equal("Test Document", result.DocumentName);
    }

    [Fact]
    public async Task AddCargoToRequestAsync_ShouldAddCargoSuccessfully()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var harborService = new HarborService(dbContext);
        
        var testUser = new ShippingAgent { Username = "agent", PasswordHash = "pass" };
        var testVessel = new Vessel { VesselName = "Test Vessel", IMONumber = "IMO12345", Type = "Container Ship", Capacity = 50000 };
        dbContext.Users.Add(testUser);
        dbContext.Vessels.Add(testVessel);
        await dbContext.SaveChangesAsync();
        var serviceRequest = await harborService.CreateServiceRequestAsync(testVessel.VesselID, testUser.UserID, "Docking");
        Assert.NotNull(serviceRequest);

        // Act
        var result = await harborService.AddCargoToRequestAsync(serviceRequest.RequestID, "Test Cargo", 100.5, false);
        var cargoCount = await dbContext.Cargos.CountAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, cargoCount);
        Assert.Equal(serviceRequest.RequestID, result.ServiceRequestID);
        Assert.Equal("Test Cargo", result.Description);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_ShouldGenerateInvoiceSuccessfully()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var harborService = new HarborService(dbContext);
        
        var testUser = new ShippingAgent { Username = "agent", PasswordHash = "pass" };
        var testVessel = new Vessel { VesselName = "Test Vessel", IMONumber = "IMO12345", Type = "Container Ship", Capacity = 50000 };
        dbContext.Users.Add(testUser);
        dbContext.Vessels.Add(testVessel);
        await dbContext.SaveChangesAsync();
        var serviceRequest = await harborService.CreateServiceRequestAsync(testVessel.VesselID, testUser.UserID, "Docking");
        Assert.NotNull(serviceRequest);

        // Act
        var result = await harborService.GenerateInvoiceAsync(serviceRequest.RequestID, 1500.00);
        var invoiceCount = await dbContext.Invoices.CountAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, invoiceCount);
        Assert.Equal(serviceRequest.RequestID, result.ServiceRequestID);
        Assert.Equal(1500.00, result.TotalAmount);
    }
}
