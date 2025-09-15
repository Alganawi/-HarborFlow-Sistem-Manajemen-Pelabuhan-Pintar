using System;

namespace HarborFlow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("HarborFlow System Simulation Started");
            Console.WriteLine("====================================");

            // 1. Initialize Users
            var agent = new ShippingAgent { UserID = 1, Username = "Agent01" };
            var staff = new PortStaff { UserID = 2, Username = "Staff01" };
            var finance = new FinanceAdmin { UserID = 3, Username = "Finance01" };
            var manager = new PortManager { UserID = 4, Username = "Manager01" };

            // 2. Agent creates a service request for a new vessel
            Console.WriteLine("\n--- Step 1: Shipping Agent Creates Request ---");
            var vessel = new Vessel 
            { 
                VesselID = 101, 
                VesselName = "MV Sumatra", 
                IMONumber = "IMO9876543", 
                Type = "Container Ship", 
                Capacity = 5000 
            };
            var serviceRequest = agent.CreateServiceRequest(vessel, "Standard Docking & Unloading");
            agent.UploadDocument(serviceRequest, "Bill_of_Lading.pdf");
            agent.UploadDocument(serviceRequest, "Cargo_Manifest.pdf");

            // 3. Port Staff verifies the request and schedules the vessel
            Console.WriteLine("\n--- Step 2: Port Staff Verifies and Schedules ---");
            staff.VerifyRequest(serviceRequest);

            // Check if request is approved before scheduling
            if (serviceRequest.Status == RequestStatus.Approved)
            {
                staff.ScheduleVessel(serviceRequest, 7); // Assign to Berth 7
            }

            // 4. Finance Admin creates an invoice and processes payment
            Console.WriteLine("\n--- Step 3: Finance Admin Manages Billing ---");
            if (serviceRequest.Status == RequestStatus.Approved)
            {
                var invoice = finance.CreateInvoice(serviceRequest, 12500.75);
                invoice.SendInvoice();
                finance.ProcessPayment(invoice, "Wire Transfer");
            }
            
            // 5. Port Manager views the dashboard
            Console.WriteLine("\n--- Step 4: Port Manager Views Dashboard ---");
            manager.ViewDashboard();


            Console.WriteLine("\n====================================");
            Console.WriteLine("HarborFlow System Simulation Finished");
        }
    }
}