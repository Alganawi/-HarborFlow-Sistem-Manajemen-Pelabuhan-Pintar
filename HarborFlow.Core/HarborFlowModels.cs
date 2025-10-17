using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HarborFlow
{
    // ============ DATABASE CONTEXT ============
    public class HarborFlowDbContext : DbContext
    {
        public HarborFlowDbContext(DbContextOptions<HarborFlowDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ShippingAgent> ShippingAgents { get; set; }
        public DbSet<PortManager> PortManagers { get; set; }
        public DbSet<PortStaff> PortStaffs { get; set; }
        public DbSet<FinanceAdmin> FinanceAdmins { get; set; }
        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cargo> Cargos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the User hierarchy using TPH (Table-Per-Hierarchy)
            modelBuilder.Entity<User>()
                .HasDiscriminator<UserRole>("Role")
                .HasValue<ShippingAgent>(UserRole.ShippingAgent)
                .HasValue<PortManager>(UserRole.PortManager)
                .HasValue<PortStaff>(UserRole.PortStaff)
                .HasValue<FinanceAdmin>(UserRole.FinanceAdmin);
        }
    }

    // ============ BASE USER ENTITY ============
    public abstract class User
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
    
    // ============ USER SUBCLASSES (ENTITIES) ============
    public class ShippingAgent : User { }
    public class PortManager : User { }
    public class PortStaff : User { }
    public class FinanceAdmin : User { }
    
    // ============ DOMAIN ENTITIES ============
    public class ServiceRequest
    {
        [Key]
        public int RequestID { get; set; }
        public DateTime RequestDate { get; set; }
        [Required]
        public string ServiceType { get; set; } = string.Empty;
        public RequestStatus Status { get; set; }
        public DateTime EstimatedArrival { get; set; }
    
        public int VesselID { get; set; }
        public Vessel? Vessel { get; set; }
    
        public int CreatedByUserID { get; set; }
        public User? CreatedBy { get; set; }
    
        public List<Document> Documents { get; set; } = new();
        public List<Cargo> Cargos { get; set; } = new();
        public Schedule? Schedule { get; set; }
        public Invoice? Invoice { get; set; }
    }
    
    public class Vessel
    {
        [Key]
        public int VesselID { get; set; }
        [Required]
        public string VesselName { get; set; } = string.Empty;
        [Required]
        public string IMONumber { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty;
        public double Capacity { get; set; }
        [JsonIgnore]
        public List<ServiceRequest> ServiceRequests { get; set; } = new();
    }
    
    public class Document
    {
        [Key]
        public int DocumentID { get; set; }
        [Required]
        public string DocumentName { get; set; } = string.Empty;
        [Required]
        public string FilePath { get; set; } = string.Empty;
        public bool IsVerified { get; set; }

        public int ServiceRequestID { get; set; }
        [JsonIgnore]
        public ServiceRequest? ServiceRequest { get; set; }
    }
    
    public class Schedule
    {
        [Key]
        public int ScheduleID { get; set; }
        public int BerthNumber { get; set; }
        public DateTime ActualArrival { get; set; }
        public DateTime ActualDeparture { get; set; }
    
        public int ServiceRequestID { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
    }
    
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }
        [Required]
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public double TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
    
        public int ServiceRequestID { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
        public Payment? Payment { get; set; }
    }
    
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
    
        public int InvoiceID { get; set; }
        public Invoice? Invoice { get; set; }
    }

    public class Cargo
    {
        [Key]
        public int CargoID { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public double Weight { get; set; } // in tons
        public bool IsHazardous { get; set; }
        public CargoStatus Status { get; set; }

        public int ServiceRequestID { get; set; }
        [JsonIgnore]
        public ServiceRequest? ServiceRequest { get; set; }
    }

    // ============ ENUMS ============
    public enum UserRole { ShippingAgent, PortManager, PortStaff, FinanceAdmin }
    public enum RequestStatus { Pending, Approved, Rejected, Cancelled, Completed }
    public enum InvoiceStatus { Draft, Sent, Paid, Overdue }
    public enum CargoStatus { Onboard, Discharged, Loaded }
}
