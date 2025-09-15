using System;
using System.Collections.Generic;

namespace HarborFlow
{
    // ============ BASE USER CLASS ============ 
    public abstract class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        public virtual void Login()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] User {Username} logged in");
        }

        public virtual void Logout()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] User {Username} logged out");
        }

        public virtual void UpdateProfile()
        {
            Console.WriteLine($"Profile updated for user: {Username}");
        }
    }

    // ============ USER SUBCLASSES ============ 
    public class ShippingAgent : User
    {
        private List<ServiceRequest> myRequests;

        public ShippingAgent()
        {
            myRequests = new List<ServiceRequest>();
            Role = UserRole.ShippingAgent;
        }

        public ServiceRequest CreateServiceRequest(Vessel vessel, string serviceType)
        {
            var request = new ServiceRequest
            {
                RequestID = new Random().Next(1000, 9999),
                RequestDate = DateTime.Now,
                ServiceType = serviceType,
                Status = RequestStatus.Pending,
                EstimatedArrival = DateTime.Now.AddDays(7),
                Vessel = vessel,
                CreatedBy = this
            };
            myRequests.Add(request);
            Console.WriteLine($"Service request #{request.RequestID} created for vessel {vessel.VesselName}");
            return request;
        }

        public void UploadDocument(ServiceRequest request, string documentName)
        {
            var doc = new Document
            {
                DocumentID = new Random().Next(1000, 9999),
                DocumentName = documentName,
                FilePath = $"/documents/{documentName}",
                IsVerified = false,
                ServiceRequest = request
            };
            request.AddDocument(doc);
            Console.WriteLine($"Document '{documentName}' uploaded for request #{request.RequestID}");
        }
    }

    public class PortManager : User
    {
        private Dashboard dashboard;

        public PortManager()
        {
            Role = UserRole.PortManager;
            dashboard = new Dashboard();
        }

        public void ViewDashboard()
        {
            Console.WriteLine("\n=== OPERATIONAL DASHBOARD ===");
            Console.WriteLine($"Total Vessels: {dashboard.VesselCount}");
            Console.WriteLine($"Berth Occupancy: {dashboard.BerthOccupancy:P}");
            Console.WriteLine("Recent Activities:");
            foreach (var activity in dashboard.RecentActivities)
            {
                Console.WriteLine($" • {activity}");
            }
        }

        public Report GenerateReport(string reportType)
        {
            var report = new Report
            {
                ReportID = new Random().Next(1000, 9999),
                ReportType = reportType,
                GeneratedDate = DateTime.Now,
                Data = new Dictionary<string, object>()
            };
            // Simulasi data report
            report.Data.Add("total_vessels", 25);
            report.Data.Add("total_revenue", 1500000);
            report.Data.Add("avg_turnaround", 48);
            Console.WriteLine($"Report generated: {reportType} (ID: {report.ReportID})");
            return report;
        }
    }

    public class PortStaff : User
    {
        public PortStaff()
        {
            Role = UserRole.PortStaff;
        }

        public void VerifyRequest(ServiceRequest request)
        {
            Console.WriteLine($"Verifying request #{request.RequestID}...");
            // Simulasi verifikasi
            bool allDocumentsVerified = true;
            foreach (var doc in request.Documents)
            {
                doc.Verify();
                if (!doc.IsVerified)
                {
                    allDocumentsVerified = false;
                }
            }

            if (allDocumentsVerified)
            {
                request.UpdateStatus(RequestStatus.Approved);
                Console.WriteLine($"Request #{request.RequestID} approved");
            }
            else
            {
                request.UpdateStatus(RequestStatus.Rejected);
                Console.WriteLine($"Request #{request.RequestID} rejected - documents incomplete");
            }
        }

        public Schedule ScheduleVessel(ServiceRequest request, int berthNumber)
        {
            var schedule = new Schedule
            {
                ScheduleID = new Random().Next(1000, 9999),
                BerthNumber = berthNumber,
                ActualArrival = request.EstimatedArrival,
                ActualDeparture = request.EstimatedArrival.AddDays(2),
                ServiceRequest = request
            };
            request.Schedule = schedule;
            Console.WriteLine($"Vessel {request.Vessel.VesselName} scheduled at berth {berthNumber}");
            return schedule;
        }
    }

    public class FinanceAdmin : User
    {
        public FinanceAdmin()
        {
            Role = UserRole.FinanceAdmin;
        }

        public Invoice CreateInvoice(ServiceRequest request, double amount)
        {
            var invoice = new Invoice
            {
                InvoiceID = new Random().Next(1000, 9999),
                InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{request.RequestID}",
                IssueDate = DateTime.Now,
                TotalAmount = amount,
                Status = InvoiceStatus.Draft,
                ServiceRequest = request
            };
            request.Invoice = invoice;
            Console.WriteLine($"Invoice {invoice.InvoiceNumber} created for ${amount:N2}");
            return invoice;
        }

        public void ProcessPayment(Invoice invoice, string paymentMethod)
        {
            var payment = new Payment
            {
                PaymentID = new Random().Next(1000, 9999),
                PaymentDate = DateTime.Now,
                Amount = invoice.TotalAmount,
                PaymentMethod = paymentMethod,
                Invoice = invoice
            };
            payment.ProcessPayment();
            invoice.Payment = payment;
            invoice.Status = InvoiceStatus.Paid;
            Console.WriteLine($"Payment processed: ${payment.Amount:N2} via {paymentMethod}");
        }
    }

    // ============ DOMAIN CLASSES ============ 
    public class ServiceRequest
    {
        public int RequestID { get; set; }
        public DateTime RequestDate { get; set; }
        public string ServiceType { get; set; }
        public RequestStatus Status { get; set; }
        public DateTime EstimatedArrival { get; set; }
        public Vessel Vessel { get; set; }
        public User CreatedBy { get; set; }
        public List<Document> Documents { get; set; }
        public Schedule Schedule { get; set; }
        public Invoice Invoice { get; set; }

        public ServiceRequest()
        {
            Documents = new List<Document>();
        }

        public void CreateRequest()
        {
            Status = RequestStatus.Pending;
            RequestDate = DateTime.Now;
            Console.WriteLine($"Request #{RequestID} created");
        }

        public void CancelRequest()
        {
            Status = RequestStatus.Cancelled;
            Console.WriteLine($"Request #{RequestID} cancelled");
        }

        public void UpdateStatus(RequestStatus newStatus)
        {
            var oldStatus = Status;
            Status = newStatus;
            Console.WriteLine($"Request #{RequestID} status changed: {oldStatus} → {newStatus}");
        }

        public void AddDocument(Document document)
        {
            Documents.Add(document);
            document.ServiceRequest = this;
        }
    }

    public class Vessel
    {
        public int VesselID { get; set; }
        public string VesselName { get; set; }
        public string IMONumber { get; set; }
        public string Type { get; set; }
        public double Capacity { get; set; }

        public void GetDetails()
        {
            Console.WriteLine($"Vessel: {VesselName}");
            Console.WriteLine($"IMO: {IMONumber}");
            Console.WriteLine($"Type: {Type}");
            Console.WriteLine($"Capacity: {Capacity} TEU");
        }

        public void UpdateDetails(string name, string type, double capacity)
        {
            VesselName = name;
            Type = type;
            Capacity = capacity;
            Console.WriteLine("Vessel details updated");
        }
    }

    public class Document
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string FilePath { get; set; }
        public bool IsVerified { get; set; }
        public ServiceRequest ServiceRequest { get; set; }

        public void Upload()
        {
            Console.WriteLine($"Document '{DocumentName}' uploaded to {FilePath}");
        }

        public void Verify()
        {
            IsVerified = true;
            Console.WriteLine($"Document '{DocumentName}' verified");
        }
    }

    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int BerthNumber { get; set; }
        public DateTime ActualArrival { get; set; }
        public DateTime ActualDeparture { get; set; }
        public ServiceRequest ServiceRequest { get; set; }

        public void AssignBerth(int berth)
        {
            BerthNumber = berth;
            Console.WriteLine($"Berth {berth} assigned");
        }

        public void UpdateSchedule(DateTime arrival, DateTime departure)
        {
            ActualArrival = arrival;
            ActualDeparture = departure;
            Console.WriteLine($"Schedule updated: {arrival:dd/MM/yyyy} - {departure:dd/MM/yyyy}");
        }
    }

    public class Invoice
    {
        public int InvoiceID { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public double TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public ServiceRequest ServiceRequest { get; set; }
        public Payment Payment { get; set; }

        public void GenerateInvoice()
        {
            Status = InvoiceStatus.Draft;
            Console.WriteLine($"Invoice {InvoiceNumber} generated");
        }

        public void SendInvoice()
        {
            Status = InvoiceStatus.Sent;
            Console.WriteLine($"Invoice {InvoiceNumber} sent to customer");
        }
    }

    public class Payment
    {
        public int PaymentID { get; set; }
        public DateTime PaymentDate { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public Invoice Invoice { get; set; }

        public void ProcessPayment()
        {
            Console.WriteLine($"Processing payment of ${Amount:N2}...");
            Console.WriteLine($"Payment successful via {PaymentMethod}");
        }
    }

    public class Dashboard
    {
        public int VesselCount { get; set; }
        public float BerthOccupancy { get; set; }
        public List<string> RecentActivities { get; set; }

        public Dashboard()
        {
            // Data simulasi
            VesselCount = 15;
            BerthOccupancy = 0.75f;
            RecentActivities = new List<string>
            {
                "MV Jakarta Express arrived at Berth 3",
                "Service request #1234 approved",
                "Payment received for Invoice INV-20250108-5678",
                "New vessel registered: MV Surabaya Star",
                "Berth 5 maintenance completed"
            };
        }

        public void GetDashboardData()
        {
            Console.WriteLine("Fetching dashboard data...");
        }
    }

    public class Report
    {
        public int ReportID { get; set; }
        public string ReportType { get; set; }
        public DateTime GeneratedDate { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public Report()
        {
            Data = new Dictionary<string, object>();
        }

        public void GenerateReport()
        {
            Console.WriteLine($"Generating {ReportType} report...");
            Console.WriteLine($"Report ID: {ReportID}");
            Console.WriteLine($"Generated: {GeneratedDate:dd/MM/yyyy HH:mm}");
        }
    }
    
    // ============ ENUMS ============ 
    public enum UserRole { ShippingAgent, PortManager, PortStaff, FinanceAdmin } 
    public enum RequestStatus { Pending, Approved, Rejected, Cancelled, Completed } 
    public enum InvoiceStatus { Draft, Sent, Paid, Overdue } 
}