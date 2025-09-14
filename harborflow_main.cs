using System;
using System.Collections.Generic;
using System.Linq;

namespace HarborFlow
{
    // Enumerasi untuk status dan role
    public enum UserRole
    {
        ShippingAgent,
        PortStaff,
        PortManager,
        FinanceAdmin
    }

    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected,
        InProgress,
        Completed,
        Cancelled
    }

    public enum InvoiceStatus
    {
        Draft,
        Sent,
        Paid,
        Overdue,
        Cancelled
    }

    // Class Program utama
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("  HarborFlow - Sistem Manajemen");
            Console.WriteLine("       Pelabuhan Pintar");
            Console.WriteLine("====================================\n");

            // Inisialisasi sistem
            HarborFlowSystem system = new HarborFlowSystem();
            system.InitializeDemo();
            
            // Menu interaktif
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== MENU UTAMA ===");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Lihat Demonstrasi Use Case");
                Console.WriteLine("3. Simulasi Activity Diagram");
                Console.WriteLine("4. Tampilkan Struktur Class");
                Console.WriteLine("5. Keluar");
                Console.Write("Pilih menu (1-5): ");

                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        system.LoginProcess();
                        break;
                    case "2":
                        system.DemonstrateUseCases();
                        break;
                    case "3":
                        system.SimulateActivityDiagram();
                        break;
                    case "4":
                        system.DisplayClassStructure();
                        break;
                    case "5":
                        running = false;
                        Console.WriteLine("\nTerima kasih telah menggunakan HarborFlow!");
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }
        }
    }

    // Sistem utama HarborFlow
    public class HarborFlowSystem
    {
        private List<User> users;
        private List<ServiceRequest> serviceRequests;
        private List<Vessel> vessels;
        private User currentUser;

        public HarborFlowSystem()
        {
            users = new List<User>();
            serviceRequests = new List<ServiceRequest>();
            vessels = new List<Vessel>();
        }

        public void InitializeDemo()
        {
            // Inisialisasi data demo
            Console.WriteLine("Menginisialisasi data demo...");
            
            // Tambah users
            users.Add(new ShippingAgent { 
                UserID = 1, 
                Username = "agent01", 
                PasswordHash = "pass123",
                Role = UserRole.ShippingAgent 
            });
            
            users.Add(new PortManager { 
                UserID = 2, 
                Username = "manager01", 
                PasswordHash = "admin123",
                Role = UserRole.PortManager 
            });
            
            // Tambah vessel
            vessels.Add(new Vessel {
                VesselID = 1,
                VesselName = "MV Jakarta Express",
                IMONumber = "IMO9517232",
                Type = "Container Ship",
                Capacity = 5000.0
            });
            
            Console.WriteLine($"✓ {users.Count} pengguna ditambahkan");
            Console.WriteLine($"✓ {vessels.Count} kapal terdaftar");
            Console.WriteLine("Inisialisasi selesai!\n");
        }

        public void LoginProcess()
        {
            Console.WriteLine("\n=== LOGIN SISTEM ===");
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            var user = users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
            
            if (user != null)
            {
                currentUser = user;
                user.Login();
                Console.WriteLine($"Login berhasil! Selamat datang, {username}");
                Console.WriteLine($"Role: {user.Role}");
                
                // Menu berdasarkan role
                ShowRoleBasedMenu();
            }
            else
            {
                Console.WriteLine("Login gagal! Username atau password salah.");
            }
        }

        private void ShowRoleBasedMenu()
        {
            if (currentUser == null) return;

            Console.WriteLine($"\n=== MENU {currentUser.Role.ToString().ToUpper()} ===");
            
            switch (currentUser.Role)
            {
                case UserRole.ShippingAgent:
                    Console.WriteLine("1. Buat Permohonan Layanan");
                    Console.WriteLine("2. Lihat Status Permohonan");
                    Console.WriteLine("3. Upload Dokumen");
                    break;
                case UserRole.PortManager:
                    Console.WriteLine("1. Lihat Dashboard");
                    Console.WriteLine("2. Generate Laporan");
                    Console.WriteLine("3. Kelola Lalu Lintas Kapal");
                    break;
                case UserRole.PortStaff:
                    Console.WriteLine("1. Verifikasi Permohonan");
                    Console.WriteLine("2. Jadwalkan Kedatangan");
                    Console.WriteLine("3. Update Status");
                    break;
                case UserRole.FinanceAdmin:
                    Console.WriteLine("1. Kelola Tagihan");
                    Console.WriteLine("2. Proses Pembayaran");
                    Console.WriteLine("3. Laporan Keuangan");
                    break;
            }
            
            Console.WriteLine("0. Logout");
        }

        public void DemonstrateUseCases()
        {
            Console.WriteLine("\n=== DEMONSTRASI USE CASE ===");
            Console.WriteLine("\nUse Case yang tersedia dalam sistem:");
            Console.WriteLine("├── Book Port Services (Agen Perkapalan)");
            Console.WriteLine("├── Manage Documents & Compliance (Agen Perkapalan, Staf)");
            Console.WriteLine("├── Manage Vessel Traffic (Staf Pelabuhan)");
            Console.WriteLine("├── View Operational Dashboard (Manajer)");
            Console.WriteLine("├── View Analytics & Reporting (Manajer)");
            Console.WriteLine("└── Manage Financials (Admin Keuangan)");
            
            Console.WriteLine("\n[Simulasi Use Case: Book Port Services]");
            Console.WriteLine("1. Agen login ke sistem");
            Console.WriteLine("2. Agen memilih 'Book Port Services'");
            Console.WriteLine("3. Sistem menampilkan form booking");
            Console.WriteLine("4. Agen mengisi detail kapal dan layanan");
            Console.WriteLine("5. Sistem validasi dan simpan booking");
            Console.WriteLine("6. Sistem kirim notifikasi ke staf pelabuhan");
        }

        public void SimulateActivityDiagram()
        {
            Console.WriteLine("\n=== SIMULASI ACTIVITY DIAGRAM ===");
            Console.WriteLine("Alur Proses Pelayanan Kapal:\n");
            
            // Simulasi langkah demi langkah
            string[] steps = {
                "[Start] Proses dimulai",
                "[Agen] Login ke sistem",
                "[Agen] Membuat permohonan layanan",
                "[Agen] Upload dokumen pendukung",
                "[Sistem] Kirim permohonan ke staf",
                "[Staf] Menerima & verifikasi permohonan",
                "[Decision] Dokumen valid?",
                "  ├─ Ya: Lanjut ke penjadwalan",
                "  └─ Tidak: Kirim notifikasi penolakan",
                "[Staf] Jadwalkan kedatangan kapal",
                "[Staf] Kelola operasi bongkar muat",
                "[Sistem] Generate tagihan otomatis",
                "[Admin] Verifikasi & kirim tagihan",
                "[Agen] Terima tagihan & lakukan pembayaran",
                "[Sistem] Catat pembayaran & konfirmasi",
                "[Sistem] Catat layanan selesai",
                "[End] Proses selesai"
            };
            
            foreach (var step in steps)
            {
                Console.WriteLine(step);
                System.Threading.Thread.Sleep(500); // Delay untuk efek simulasi
            }
        }

        public void DisplayClassStructure()
        {
            Console.WriteLine("\n=== STRUKTUR CLASS DIAGRAM ===");
            Console.WriteLine("\n┌─────────────────────────┐");
            Console.WriteLine("│         User            │");
            Console.WriteLine("├─────────────────────────┤");
            Console.WriteLine("│ - userID: int           │");
            Console.WriteLine("│ - username: string      │");
            Console.WriteLine("│ - passwordHash: string  │");
            Console.WriteLine("│ - role: UserRole        │");
            Console.WriteLine("├─────────────────────────┤");
            Console.WriteLine("│ + Login()               │");
            Console.WriteLine("│ + Logout()              │");
            Console.WriteLine("│ + UpdateProfile()       │");
            Console.WriteLine("└─────────────────────────┘");
            Console.WriteLine("           ▲");
            Console.WriteLine("           │ Inheritance");
            Console.WriteLine("    ┌──────┴──────┐");
            Console.WriteLine("    │             │");
            
            Console.WriteLine("\n┌─────────────────┐  ┌─────────────────┐");
            Console.WriteLine("│ ShippingAgent   │  │  PortManager    │");
            Console.WriteLine("├─────────────────┤  ├─────────────────┤");
            Console.WriteLine("│ + CreateRequest │  │ + ViewDashboard │");
            Console.WriteLine("└─────────────────┘  └─────────────────┘");
            
            Console.WriteLine("\nRelasi antar class:");
            Console.WriteLine("• User [1] creates [0..*] ServiceRequest");
            Console.WriteLine("• ServiceRequest [1] is for [1] Vessel");
            Console.WriteLine("• ServiceRequest [1] has [1..*] Document");
            Console.WriteLine("• ServiceRequest [1] results in [1] Schedule");
            Console.WriteLine("• ServiceRequest [1] generates [1] Invoice");
            Console.WriteLine("• Invoice [1] is paid by [0..1] Payment");
        }
    }
}