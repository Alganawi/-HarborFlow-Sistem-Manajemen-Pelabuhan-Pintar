using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HarborFlow
{
    // Class untuk menjalankan simulasi lengkap sistem
    public class HarborFlowSimulation
    {
        private List<User> users;
        private List<Vessel> vessels;
        private List<ServiceRequest> requests;
        private ShippingAgent currentAgent;
        private PortStaff currentStaff;
        private PortManager currentManager;
        private FinanceAdmin currentFinance;

        public HarborFlowSimulation()
        {
            InitializeSystem();
        }

        private void InitializeSystem()
        {
            Console.WriteLine("🚢 INISIALISASI SISTEM HARBORFLOW 🚢");
            Console.WriteLine("=====================================\n");
            
            users = new List<User>();
            vessels = new List<Vessel>();
            requests = new List<ServiceRequest>();

            // Create users
            currentAgent = new ShippingAgent
            {
                UserID = 1,
                Username = "agent_jakarta",
                PasswordHash = "secure_hash_123",
                Role = UserRole.ShippingAgent
            };

            currentStaff = new PortStaff
            {
                UserID = 2,
                Username = "staff_port01",
                PasswordHash = "secure_hash_456",
                Role = UserRole.PortStaff
            };

            currentManager = new PortManager
            {
                UserID = 3,
                Username = "manager_harbor",
                PasswordHash = "secure_hash_789",
                Role = UserRole.PortManager
            };

            currentFinance = new FinanceAdmin
            {
                UserID = 4,
                Username = "finance_admin",
                PasswordHash = "secure_hash_012",
                Role = UserRole.FinanceAdmin
            };

            users.AddRange(new User[] { currentAgent, currentStaff, currentManager, currentFinance });

            // Create vessels
            var vessel1 = new Vessel
            {
                VesselID = 1,
                VesselName = "MV Jakarta Express",
                IMONumber = "IMO9517232",
                Type = "Container Ship",
                Capacity = 5000
            };

            var vessel2 = new Vessel
            {
                VesselID = 2,
                VesselName = "MV Surabaya Star",
                IMONumber = "IMO9517233",
                Type = "Bulk Carrier",
                Capacity = 8000
            };

            var vessel3 = new Vessel
            {
                VesselID = 3,
                VesselName = "MV Bali Ocean",
                IMONumber = "IMO9517234",
                Type = "Tanker",
                Capacity = 10000
            };

            vessels.AddRange(new[] { vessel1, vessel2, vessel3 });

            Console.WriteLine($"✓ {users.Count} pengguna berhasil dibuat");
            Console.WriteLine($"✓ {vessels.Count} kapal terdaftar dalam sistem");
            Console.WriteLine("\nSistem siap digunakan!\n");
        }

        public void RunCompleteSimulation()
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║   SIMULASI LENGKAP SISTEM HARBORFLOW   ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");

            // Step 1: Use Case Simulation
            Console.WriteLine("📋 TAHAP 1: SIMULASI USE CASE");
            Console.WriteLine("─────────────────────────────");
            SimulateUseCases();
            PauseSimulation();

            // Step 2: Activity Diagram Simulation
            Console.WriteLine("\n📊 TAHAP 2: SIMULASI ACTIVITY DIAGRAM");
            Console.WriteLine("──────────────────────────────────────");
            SimulateCompleteWorkflow();
            PauseSimulation();

            // Step 3: Class Interaction Demo
            Console.WriteLine("\n🔧 TAHAP 3: DEMONSTRASI INTERAKSI CLASS");
            Console.WriteLine("────────────────────────────────────────");
            DemonstrateClassInteractions();
            
            Console.WriteLine("\n✅ Simulasi Selesai!");
        }

        private void SimulateUseCases()
        {
            Console.WriteLine("\n1️⃣ Use Case: Book Port Services");
            Console.WriteLine("   Actor: Shipping Agent");
            Console.WriteLine("   → Agent membuat permohonan layanan pelabuhan");
            
            Thread.Sleep(1000);
            
            Console.WriteLine("\n2️⃣ Use Case: Manage Vessel Traffic");
            Console.WriteLine("   Actor: Port Staff");
            Console.WriteLine("   → Staff mengelola lalu lintas kapal dan penjadwalan");
            
            Thread.Sleep(1000);
            
            Console.WriteLine("\n3️⃣ Use Case: View Operational Dashboard");
            Console.WriteLine("   Actor: Port Manager");
            Console.WriteLine("   → Manager melihat dashboard operasional pelabuhan");
            
            Thread.Sleep(1000);
            
            Console.WriteLine("\n4️⃣ Use Case: Manage Financials");
            Console.WriteLine("   Actor: Finance Admin");
            Console.WriteLine("   → Admin mengelola tagihan dan pembayaran");
        }

        private void SimulateCompleteWorkflow()
        {
            Console.WriteLine("\n🔄 Menjalankan alur lengkap pelayanan kapal...\n");
            
            var vessel = vessels[0];
            
            // 1. Agent Login
            PrintStep("1", "AGEN PERKAPALAN", "Login ke sistem");
            currentAgent.Login();
            Thread.Sleep(1500);

            // 2. Create Service Request
            PrintStep("2", "AGEN PERKAPALAN", "Membuat permohonan layanan");
            var request = currentAgent.CreateServiceRequest(vessel, "Bongkar Muat Container");
            requests.Add(request);
            Thread.Sleep(1500);

            // 3. Upload Documents
            PrintStep("3", "AGEN PERKAPALAN", "Upload dokumen pendukung");
            currentAgent.UploadDocument(request, "Manifest_Cargo.pdf");
            currentAgent.UploadDocument(request, "Certificate_of_Origin.pdf");
            currentAgent.UploadDocument(request, "Bill_of_Lading.pdf");
            Thread.Sleep(1500);

            // 4. Staff receives and verifies
            PrintStep("4", "STAF PELABUHAN", "Menerima & verifikasi permohonan");
            currentStaff.VerifyRequest(request);
            Thread.Sleep(1500);

            // 5. Decision point
            if (request.Status == RequestStatus.Approved)
            {
                Console.WriteLine("   ✅ Dokumen valid - Lanjut ke penjadwalan");
                Thread.Sleep(1000);

                // 6. Schedule vessel
                PrintStep("5", "STAF PELABUHAN", "Menjadwalkan kedatangan kapal");
                var schedule = currentStaff.ScheduleVessel(request, 3);
                Thread.Sleep(1500);

                // 7. Port operations
                PrintStep("6", "STAF PELABUHAN", "Mengelola operasi bongkar muat");
                Console.WriteLine($"   ⚓ Kapal {vessel.VesselName} berlabuh di dermaga {schedule.BerthNumber}");
                Console.WriteLine($"   📦 Operasi bongkar muat dimulai...");
                Thread.Sleep(2000);
                Console.WriteLine($"   ✓ Operasi selesai");
                Thread.Sleep(1500);

                // 8. Generate invoice
                PrintStep("7", "SISTEM", "Generate tagihan otomatis");
                var invoice = currentFinance.CreateInvoice(request, 250000.00);
                Thread.Sleep(1500);

                // 9. Send invoice
                PrintStep("8", "ADMIN KEUANGAN", "Verifikasi & kirim tagihan");
                invoice.SendInvoice();
                Thread.Sleep(1500);

                // 10. Process payment
                PrintStep("9", "AGEN PERKAPALAN", "Menerima tagihan & melakukan pembayaran");
                currentFinance.ProcessPayment(invoice, "Bank Transfer");
                Thread.Sleep(1500);

                // 11. Complete service
                PrintStep("10", "SISTEM", "Mencatat layanan selesai");
                request.UpdateStatus(RequestStatus.Completed);
                Console.WriteLine("   ✅ Proses pelayanan kapal selesai!");
            }
            else
            {
                Console.WriteLine("   ❌ Dokumen tidak valid - Kirim notifikasi penolakan");
                Console.WriteLine("   📧 Notifikasi penolakan dikirim ke agen");
            }
        }

        private void DemonstrateClassInteractions()
        {
            Console.WriteLine("\n🔗 Demonstrasi Relasi Antar Class:\n");

            // Create new complete scenario
            var demoVessel = vessels[1];
            
            Console.WriteLine("1. User → ServiceRequest (1 to many)");
            Console.WriteLine($"   • {currentAgent.Username} membuat multiple service requests");
            var req1 = currentAgent.CreateServiceRequest(demoVessel, "Loading");
            var req2 = currentAgent.CreateServiceRequest(vessels[2], "Unloading");
            Thread.Sleep(1000);

            Console.WriteLine("\n2. ServiceRequest → Vessel (many to 1)");
            Console.WriteLine($"   • Request #{req1.RequestID} untuk kapal {req1.Vessel.VesselName}");
            Console.WriteLine($"   • Request #{req2.RequestID} untuk kapal {req2.Vessel.VesselName}");
            Thread.Sleep(1000);

            Console.WriteLine("\n3. ServiceRequest → Document (1 to many)");
            currentAgent.UploadDocument(req1, "Customs_Declaration.pdf");
            currentAgent.UploadDocument(req1, "Insurance_Certificate.pdf");
            Thread.Sleep(1000);

            Console.WriteLine("\n4. ServiceRequest → Schedule (1 to 1)");
            var schedule = currentStaff.ScheduleVessel(req1, 2);
            Console.WriteLine($"   • Request #{req1.RequestID} → Schedule #{schedule.ScheduleID}");
            Thread.Sleep(1000);

            Console.WriteLine("\n5. ServiceRequest → Invoice → Payment");
            var invoice = currentFinance.CreateInvoice(req1, 175000);
            currentFinance.ProcessPayment(invoice, "Credit Card");
            Thread.Sleep(1000);

            Console.WriteLine("\n6. PortManager → Dashboard & Report");
            currentManager.ViewDashboard();
            currentManager.GenerateReport("Monthly Performance");
        }

        private void PrintStep(string stepNo, string actor, string action)
        {
            Console.WriteLine($"[Step {stepNo}] {actor}: {action}");
        }

        private void PauseSimulation()
        {
            Console.WriteLine("\nTekan Enter untuk melanjutkan...");
            Console.ReadLine();
        }
    }

    // Extended demo application
    public class HarborFlowDemo
    {
        public static void RunDemo()
        {
            Console.Clear();
            PrintHeader();
            
            var simulation = new HarborFlowSimulation();
            
            bool running = true;
            while (running)
            {
                PrintMainMenu();
                var choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        simulation.RunCompleteSimulation();
                        break;
                    case "2":
                        RunInteractiveMode();
                        break;
                    case "3":
                        ShowSystemArchitecture();
                        break;
                    case "4":
                        ShowProjectInfo();
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
                
                if (running)
                {
                    Console.WriteLine("\nTekan Enter untuk kembali ke menu...");
                    Console.ReadLine();
                    Console.Clear();
                    PrintHeader();
                }
            }
            
            Console.WriteLine("\n╔═══════════════════════════════════╗");
            Console.WriteLine("║  Terima kasih telah menggunakan   ║");
            Console.WriteLine("║         HarborFlow System         ║");
            Console.WriteLine("╚═══════════════════════════════════╝");
        }

        private static void PrintHeader()
        {
            Console.WriteLine("╔═══════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                   ║");
            Console.WriteLine("║     🚢  HARBORFLOW - SMART PORT MANAGEMENT  🚢    ║");
            Console.WriteLine("║            Sistem Manajemen Pelabuhan Pintar      ║");
            Console.WriteLine("║                    Kelompok 4                     ║");
            Console.WriteLine("║                                                   ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════╝");
        }

        private static void PrintMainMenu()
        {
            Console.WriteLine("\n┌─── MENU UTAMA ────────────────────┐");
            Console.WriteLine("│                                   │");
            Console.WriteLine("│  1. Jalankan Simulasi Lengkap     │");
            Console.WriteLine("│  2. Mode Interaktif               │");
            Console.WriteLine("│  3. Lihat Arsitektur Sistem       │");
            Console.WriteLine("│  4. Informasi Project             │");
            Console.WriteLine("│  5. Keluar                        │");
            Console.WriteLine("│                                   │");
            Console.WriteLine("└───────────────────────────────────┘");
            Console.Write("\nPilihan Anda: ");
        }

        private static void RunInteractiveMode()
        {
            Console.WriteLine("\n=== MODE INTERAKTIF ===");
            Console.WriteLine("Pilih role yang ingin Anda simulasikan:");
            Console.WriteLine("1. Shipping Agent");
            Console.WriteLine("2. Port Staff");
            Console.WriteLine("3. Port Manager");
            Console.WriteLine("4. Finance Admin");
            Console.Write("Pilihan: ");
            
            var roleChoice = Console.ReadLine();
            
            switch (roleChoice)
            {
                case "1":
                    SimulateShippingAgent();
                    break;
                case "2":
                    SimulatePortStaff();
                    break;
                case "3":
                    SimulatePortManager();
                    break;
                case "4":
                    SimulateFinanceAdmin();
                    break;
                default:
                    Console.WriteLine("Role tidak valid!");
                    break;
            }
        }

        private static void SimulateShippingAgent()
        {
            Console.WriteLine("\n[SHIPPING AGENT MODE]");
            Console.WriteLine("Anda login sebagai: agent_jakarta\n");
            
            var agent = new ShippingAgent
            {
                UserID = 1,
                Username = "agent_jakarta",
                Role = UserRole.ShippingAgent
            };
            
            var vessel = new Vessel
            {
                VesselID = 10,
                VesselName = "MV Test Vessel",
                IMONumber = "IMO9999999",
                Type = "Container",
                Capacity = 3000
            };
            
            Console.WriteLine("Membuat permohonan layanan baru...");
            var request = agent.CreateServiceRequest(vessel, "Bongkar Muat");
            
            Console.WriteLine("\nUpload dokumen? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                agent.UploadDocument(request, "Test_Document.pdf");
            }
            
            Console.WriteLine("\nPermohonan berhasil dibuat!");
        }

        private static void SimulatePortStaff()
        {
            Console.WriteLine("\n[PORT STAFF MODE]");
            Console.WriteLine("Anda login sebagai: staff_port01\n");
            Console.WriteLine("Fungsi yang tersedia:");
            Console.WriteLine("• Verifikasi permohonan");
            Console.WriteLine("• Jadwalkan kapal");
            Console.WriteLine("• Update status operasi");
        }

        private static void SimulatePortManager()
        {
            Console.WriteLine("\n[PORT MANAGER MODE]");
            var manager = new PortManager
            {
                UserID = 3,
                Username = "manager_harbor",
                Role = UserRole.PortManager
            };
            
            manager.ViewDashboard();
            
            Console.WriteLine("\nGenerate report? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                manager.GenerateReport("Weekly Summary");
            }
        }

        private static void SimulateFinanceAdmin()
        {
            Console.WriteLine("\n[FINANCE ADMIN MODE]");
            Console.WriteLine("Anda login sebagai: finance_admin\n");
            Console.WriteLine("Fungsi yang tersedia:");
            Console.WriteLine("• Buat tagihan");
            Console.WriteLine("• Proses pembayaran");
            Console.WriteLine("• Laporan keuangan");
        }

        private static void ShowSystemArchitecture()
        {
            Console.WriteLine("\n╔═══════════════════════════════════════╗");
            Console.WriteLine("║      ARSITEKTUR SISTEM HARBORFLOW      ║");
            Console.WriteLine("╚═══════════════════════════════════════╝\n");
            
            Console.WriteLine("📁 PROJECT STRUCTURE:");
            Console.WriteLine("├── Program.cs          (Entry point & main menu)");
            Console.WriteLine("├── Models.cs           (Domain classes)");
            Console.WriteLine("├── Simulation.cs       (Business logic)");
            Console.WriteLine("└── Database/           (Data persistence)");
            
            Console.WriteLine("\n🔧 TEKNOLOGI:");
            Console.WriteLine("• Language: C# (.NET 5.0+)");
            Console.WriteLine("• UI: Console Application (upgradable to WPF)");
            Console.WriteLine("• Database: PostgreSQL (planned)");
            Console.WriteLine("• Architecture: Object-Oriented Design");
            
            Console.WriteLine("\n📊 DESIGN PATTERNS:");
            Console.WriteLine("• Inheritance (User hierarchy)");
            Console.WriteLine("• Composition (Request-Document relationship)");
            Console.WriteLine("• Factory Pattern (untuk object creation)");
            Console.WriteLine("• Repository Pattern (untuk data access)");
        }

        private static void ShowProjectInfo()
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         INFORMASI PROJECT              ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");
            
            Console.WriteLine("📌 NAMA PROJECT:");
            Console.WriteLine("   HarborFlow - Sistem Manajemen Pelabuhan Pintar\n");
            
            Console.WriteLine("👥 TIM PENGEMBANG KELOMPOK 4:");
            Console.WriteLine("   • Mirsad Alganawi Azma - Software Architect");
            Console.WriteLine("   • Marseillo R. B. Satrian - Backend Developer\n");
            
            Console.WriteLine("🎯 TUJUAN:");
            Console.WriteLine("   Mengotomatisasi dan mengintegrasikan seluruh");
            Console.WriteLine("   aspek operasional pelabuhan dalam satu platform\n");
            
            Console.WriteLine("✨ FITUR UTAMA:");
            Console.WriteLine("   • Vessel Traffic Management");
            Console.WriteLine("   • Financial Management Module");
            Console.WriteLine("   • Operational Dashboard");
            Console.WriteLine("   • Document & Compliance Management");
            Console.WriteLine("   • Analytics & Reporting\n");
            
            Console.WriteLine("📅 STATUS: Development Phase");
            Console.WriteLine("📊 VERSION: 1.0.0-beta");
        }
    }

    // Entry point for the complete demo
    class CompleteProgram
    {
        static void Main(string[] args)
        {
            HarborFlowDemo.RunDemo();
        }
    }
}