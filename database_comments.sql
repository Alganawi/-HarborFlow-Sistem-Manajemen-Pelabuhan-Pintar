-- Skrip untuk menambahkan komentar ke kolom-kolom tabel di database harborflow_db

-- Tabel: Users
COMMENT ON COLUMN "Users"."UserID" IS 'ID unik untuk setiap pengguna.';
COMMENT ON COLUMN "Users"."Username" IS 'Nama pengguna untuk login.';
COMMENT ON COLUMN "Users"."PasswordHash" IS 'Hash dari password pengguna untuk keamanan.';
COMMENT ON COLUMN "Users"."Role" IS 'Peran pengguna dalam sistem (0: ShippingAgent, 1: PortManager, 2: PortStaff, 3: FinanceAdmin) untuk kontrol akses.';

-- Tabel: Vessels
COMMENT ON COLUMN "Vessels"."VesselID" IS 'ID unik untuk setiap kapal.';
COMMENT ON COLUMN "Vessels"."VesselName" IS 'Nama resmi dari kapal.';
COMMENT ON COLUMN "Vessels"."IMONumber" IS 'Nomor identifikasi unik dari International Maritime Organization.';
COMMENT ON COLUMN "Vessels"."Type" IS 'Jenis kapal (misal: Kargo, Tanker, Kontainer).';
COMMENT ON COLUMN "Vessels"."Capacity" IS 'Kapasitas angkut kapal (misalnya dalam ton).';

-- Tabel: ServiceRequests
COMMENT ON COLUMN "ServiceRequests"."RequestID" IS 'ID unik untuk setiap permintaan layanan.';
COMMENT ON COLUMN "ServiceRequests"."RequestDate" IS 'Tanggal saat permintaan layanan dibuat.';
COMMENT ON COLUMN "ServiceRequests"."ServiceType" IS 'Jenis layanan yang diminta (misal: Bongkar Muat, Pengisian Bahan Bakar).';
COMMENT ON COLUMN "ServiceRequests"."Status" IS 'Status terkini dari permintaan layanan (0: Pending, 1: Approved, 2: Rejected, 3: Cancelled, 4: Completed).';
COMMENT ON COLUMN "ServiceRequests"."EstimatedArrival" IS 'Perkiraan waktu kedatangan kapal.';
COMMENT ON COLUMN "ServiceRequests"."VesselID" IS 'Foreign key yang merujuk ke tabel Vessels.';
COMMENT ON COLUMN "ServiceRequests"."CreatedByUserID" IS 'Foreign key yang merujuk ke tabel Users, menandakan siapa yang membuat permintaan.';

-- Tabel: Documents
COMMENT ON COLUMN "Documents"."DocumentID" IS 'ID unik untuk setiap dokumen.';
COMMENT ON COLUMN "Documents"."DocumentName" IS 'Nama atau judul dari dokumen.';
COMMENT ON COLUMN "Documents"."FilePath" IS 'Lokasi penyimpanan file dokumen.';
COMMENT ON COLUMN "Documents"."IsVerified" IS 'Status yang menandakan apakah dokumen sudah diverifikasi atau belum.';
COMMENT ON COLUMN "Documents"."ServiceRequestID" IS 'Foreign key yang merujuk ke tabel ServiceRequests.';

-- Tabel: Schedules
COMMENT ON COLUMN "Schedules"."ScheduleID" IS 'ID unik untuk setiap jadwal.';
COMMENT ON COLUMN "Schedules"."BerthNumber" IS 'Nomor dermaga tempat kapal akan sandar.';
COMMENT ON COLUMN "Schedules"."ActualArrival" IS 'Waktu kedatangan aktual kapal di dermaga.';
COMMENT ON COLUMN "Schedules"."ActualDeparture" IS 'Waktu keberangkatan aktual kapal dari dermaga.';
COMMENT ON COLUMN "Schedules"."ServiceRequestID" IS 'Foreign key yang merujuk ke tabel ServiceRequests.';

-- Tabel: Invoices
COMMENT ON COLUMN "Invoices"."InvoiceID" IS 'ID unik untuk setiap faktur.';
COMMENT ON COLUMN "Invoices"."InvoiceNumber" IS 'Nomor unik yang tertera pada faktur.';
COMMENT ON COLUMN "Invoices"."IssueDate" IS 'Tanggal saat faktur diterbitkan.';
COMMENT ON COLUMN "Invoices"."TotalAmount" IS 'Jumlah total tagihan pada faktur.';
COMMENT ON COLUMN "Invoices"."Status" IS 'Status faktur (0: Draft, 1: Sent, 2: Paid, 3: Overdue).';
COMMENT ON COLUMN "Invoices"."ServiceRequestID" IS 'Foreign key yang merujuk ke tabel ServiceRequests.';

-- Tabel: Payments
COMMENT ON COLUMN "Payments"."PaymentID" IS 'ID unik untuk setiap transaksi pembayaran.';
COMMENT ON COLUMN "Payments"."PaymentDate" IS 'Tanggal saat pembayaran dilakukan.';
COMMENT ON COLUMN "Payments"."Amount" IS 'Jumlah uang yang dibayarkan.';
COMMENT ON COLUMN "Payments"."PaymentMethod" IS 'Metode yang digunakan untuk pembayaran (misal: Transfer Bank, Kartu Kredit).';
COMMENT ON COLUMN "Payments"."InvoiceID" IS 'Foreign key yang merujuk ke tabel Invoices.';

-- Tabel: Cargos
COMMENT ON COLUMN "Cargos"."CargoID" IS 'ID unik untuk setiap item kargo.';
COMMENT ON COLUMN "Cargos"."Description" IS 'Deskripsi dari muatan atau kargo.';
COMMENT ON COLUMN "Cargos"."Weight" IS 'Berat kargo dalam satuan ton.';
COMMENT ON COLUMN "Cargos"."IsHazardous" IS 'Menandakan apakah kargo tersebut termasuk barang berbahaya atau tidak.';
COMMENT ON COLUMN "Cargos"."Status" IS 'Status kargo (0: Onboard, 1: Discharged, 2: Loaded).';
COMMENT ON COLUMN "Cargos"."ServiceRequestID" IS 'Foreign key yang merujuk ke tabel ServiceRequests.';
