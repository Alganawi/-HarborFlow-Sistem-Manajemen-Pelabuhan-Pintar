using HarborFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// HarborFlow Services Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HarborFlowDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<HarborService>();

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new ArgumentNullException("Jwt:Secret", "JWT Secret not configured in appsettings.json");
}
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // In production, this should be true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // In production, set and validate the issuer
        ValidateAudience = false // In production, set and validate the audience
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

SeedDatabase(app);

app.Run();

void SeedDatabase(IHost app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HarborFlowDbContext>();
        context.Database.EnsureCreated();
        SeedInitialUsersAsync(context).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the DB.");
    }
}

async Task SeedInitialUsersAsync(HarborFlowDbContext context)
{
    if (!await context.Users.AnyAsync())
    {
        var manager = new PortManager { Username = "manager", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"), Role = UserRole.PortManager };
        var agent = new ShippingAgent { Username = "agent", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"), Role = UserRole.ShippingAgent };
        var finance = new FinanceAdmin { Username = "finance", PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"), Role = UserRole.FinanceAdmin };
        context.Users.AddRange(manager, agent, finance);
        await context.SaveChangesAsync();
        Console.WriteLine("Seeded initial users.");
    }
}
