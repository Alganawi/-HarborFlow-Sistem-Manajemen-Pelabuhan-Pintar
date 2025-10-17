
# Prompt Produk Baru untuk Blitzy: Sistem Manajemen Pelabuhan "HarborFlow 2.0"

## 1. Visi Produk (Product Vision)

Bangun sebuah aplikasi web full-stack modern bernama **HarborFlow 2.0**, sebuah Sistem Manajemen Pelabuhan yang terintegrasi. Aplikasi ini bertujuan untuk mendigitalisasi dan mengotomatisasi alur kerja operasional di pelabuhan, mulai dari pengajuan permintaan layanan kapal, verifikasi dokumen, penjadwalan, hingga proses penagihan (invoice) dan pembayaran.

Sistem ini harus dirancang dengan arsitektur berbasis peran (role-based) yang aman, efisien, dan menyediakan visibilitas penuh bagi semua pemangku kepentingan, seperti Agen Pengiriman, Manajer Pelabuhan, dan Staf Keuangan.

**Bahasa Target untuk Spesifikasi Teknis: Tolong hasilkan semua dokumen spesifikasi teknis, deskripsi arsitektur, dan komentar kode dalam Bahasa Indonesia.**

---

## 2. Audiens & Peran Pengguna (Audience & User Roles)

Aplikasi ini akan digunakan oleh tiga peran utama dengan hak akses yang berbeda:

1.  **Agen Pengiriman (Shipping Agent):**
    *   Bertanggung jawab untuk mengelola kapal dan kebutuhannya.
    *   Dapat membuat dan melacak status permintaan layanan untuk kapalnya.
    *   Dapat mengunggah dokumen yang diperlukan untuk verifikasi.

2.  **Manajer Pelabuhan (Port Manager):**
    *   Bertanggung jawab untuk mengelola operasional pelabuhan.
    *   Dapat meninjau, menyetujui, atau menolak permintaan layanan yang masuk.
    *   Dapat memverifikasi dokumen yang diunggah oleh agen.
    *   Membuat dan mengelola jadwal sandar kapal.

3.  **Admin Keuangan (Finance Admin):**
    *   Bertanggung jawab untuk alur kerja keuangan.
    *   Dapat membuat invoice berdasarkan layanan yang telah disetujui.
    *   Dapat mencatat dan memverifikasi pembayaran yang masuk.

---

## 3. Spesifikasi Teknologi (Technology Stack)

Gunakan tumpukan teknologi yang modern, teruji, dan skalabel:

*   **Backend:**
    *   **Framework:** ASP.NET Core 9 (atau versi stabil terbaru)
    *   **Bahasa:** C#
    *   **Database:** PostgreSQL
    *   **ORM:** Entity Framework Core
    *   **Arsitektur:** API RESTful dengan struktur proyek yang bersih (misalnya, memisahkan Core, WebAPI, dan Tests).
    *   **Keamanan:** Otentikasi berbasis JSON Web Token (JWT) untuk mengamankan endpoint API.

*   **Frontend:**
    *   **Library:** React (dengan Hooks)
    *   **Styling:** Gunakan framework CSS modern seperti Bootstrap 5 atau Tailwind CSS untuk desain yang responsif dan profesional.
    *   **State Management:** Implementasikan state management yang efisien (misalnya, React Context atau Redux Toolkit).
    *   **HTTP Client:** Axios untuk komunikasi dengan backend API.

---

## 4. Fitur Utama & Alur Kerja (Key Features & Workflows)

### a. Otentikasi dan Manajemen Pengguna
*   Halaman login yang aman untuk semua pengguna.
*   Sistem pendaftaran (registrasi) untuk pengguna baru.
*   Setiap pengguna memiliki satu peran (Agent, Manager, atau Finance).
*   Endpoint API harus dilindungi berdasarkan peran pengguna yang sesuai.

### b. Dasbor Utama (Dashboard)
*   Setelah login, pengguna disambut dengan dasbor yang menampilkan ringkasan informasi yang relevan dengan perannya.
*   **Untuk Agen:** Daftar permintaan layanan yang pernah dibuat beserta statusnya.
*   **Untuk Manajer:** Daftar permintaan layanan yang menunggu persetujuan.
*   **Untuk Keuangan:** Daftar invoice yang perlu diproses atau yang sudah lunas.

### c. Alur Kerja Permintaan Layanan (Service Request Workflow)
1.  **Pembuatan:** Agen Pengiriman membuat "Permintaan Layanan" baru untuk sebuah kapal, memilih jenis layanan yang dibutuhkan (misalnya, pandu, tunda, air bersih).
2.  **Unggah Dokumen:** Agen dapat masuk ke halaman detail permintaan dan mengunggah dokumen-dokumen terkait (misalnya, surat izin berlayar, daftar kru).
3.  **Peninjauan:** Manajer Pelabuhan menerima notifikasi (atau melihat di dasbor) tentang permintaan baru.
4.  **Verifikasi:** Manajer meninjau detail permintaan dan dokumen. Dia dapat menandai dokumen sebagai "Terverifikasi".
5.  **Persetujuan:** Setelah semua syarat terpenuhi, Manajer menyetujui permintaan layanan. Status permintaan berubah menjadi "Disetujui".
6.  **Penjadwalan:** Manajer membuat jadwal sandar untuk kapal berdasarkan permintaan yang telah disetujui.

### d. Alur Kerja Keuangan (Finance Workflow)
1.  **Pembuatan Invoice:** Admin Keuangan melihat daftar permintaan layanan yang telah "Disetujui".
2.  Berdasarkan layanan tersebut, Admin Keuangan dapat men-"generate" sebuah Invoice.
3.  **Pencatatan Pembayaran:** Setelah pembayaran diterima, Admin Keuangan dapat menandai invoice sebagai "Lunas".

### e. Manajemen Data Master
*   CRUD (Create, Read, Update, Delete) untuk data **Kapal (Vessels)**.
*   CRUD untuk data **Kargo (Cargo)** yang terkait dengan sebuah kapal.
*   CRUD untuk data **Dokumen (Documents)** yang terkait dengan permintaan layanan.
*   CRUD untuk data **Jadwal (Schedules)**.

---

## 5. Model Data (Data Model)

Berikut adalah entitas utama yang harus ada dalam database:

*   **User:** `Id`, `Username`, `PasswordHash`, `Role` (enum: `ShippingAgent`, `PortManager`, `FinanceAdmin`).
*   **Vessel:** `Id`, `Name`, `IMONumber`, `OwnerId` (foreign key ke User).
*   **ServiceRequest:** `Id`, `VesselId`, `RequestDate`, `ServiceType`, `Status` (enum: `Pending`, `Approved`, `Rejected`).
*   **Document:** `Id`, `ServiceRequestId`, `Name`, `Url`, `Status` (enum: `Uploaded`, `Verified`).
*   **Cargo:** `Id`, `VesselId`, `Description`, `Weight`.
*   **Schedule:** `Id`, `VesselId`, `ArrivalTime`, `DepartureTime`, `Berth`.
*   **Invoice:** `Id`, `ServiceRequestId`, `Amount`, `IssueDate`, `DueDate`, `Status` (enum: `Unpaid`, `Paid`).
*   **Payment:** `Id`, `InvoiceId`, `PaymentDate`, `Amount`.

---

## 6. Desain & Pengalaman Pengguna (Design & UX)

*   **Desain:** Modern, bersih, dan profesional. Gunakan layout yang intuitif dan mudah dinavigasi.
*   **Responsif:** Aplikasi harus dapat diakses dengan baik di berbagai ukuran layar, dari desktop hingga tablet.
*   **Interaktivitas:** Berikan feedback visual kepada pengguna untuk setiap aksi (misalnya, loading spinners, notifikasi sukses/gagal).
*   **Alur yang Jelas:** Pengguna harus dapat dengan mudah memahami langkah-langkah yang harus diambil sesuai dengan perannya.

---

## 7. Prioritas Pengembangan (Development Priority)

1.  **Fondasi Backend:** Bangun semua model data, koneksi database, dan sistem otentikasi JWT.
2.  **API Endpoints:** Implementasikan semua endpoint API untuk fitur-fitur yang telah dijelaskan.
3.  **Frontend Dasar:** Buat halaman login, registrasi, dan dasbor utama.
4.  **Implementasi Alur Kerja:** Bangun fungsionalitas penuh untuk alur kerja permintaan layanan dan keuangan.
5.  **Pengujian:** Buat unit test untuk logika bisnis di backend dan pertimbangkan pengujian end-to-end untuk alur kerja utama.
