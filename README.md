# HarborFlow - Sistem Manajemen Pelabuhan

![HarborFlow Screenshot](https://via.placeholder.com/800x400.png?text=HarborFlow+Console+UI)
*(Contoh placeholder untuk screenshot aplikasi sedang berjalan)*

## 1. Ringkasan Proyek

HarborFlow adalah prototipe sistem manajemen pelabuhan berbasis konsol yang dibangun dengan .NET 9 dan PostgreSQL. Aplikasi ini berfungsi sebagai platform terpusat untuk mengelola alur kerja operasional pelabuhan, mulai dari permintaan layanan kapal hingga penjadwalan sandar, dengan sistem otentikasi berbasis peran untuk memastikan keamanan dan alur kerja yang terdefinisi dengan baik.

## 2. Tim Pengembang

| Nama | Role | Tanggung Jawab Utama |
| :--- | :--- | :--- |
| Gemini | Lead Developer | Merancang arsitektur, mengimplementasikan logika backend & frontend, dan manajemen database. |

---

## 3. Permasalahan yang Dipecahkan

Operasional pelabuhan melibatkan banyak pihak dengan tanggung jawab berbeda, seperti agen pengiriman dan manajer pelabuhan. Tanpa sistem yang terintegrasi, proses-proses berikut menjadi tidak efisien:

-   **Koordinasi Manual:** Permintaan layanan, pengiriman dokumen, dan penjadwalan seringkali dilakukan secara manual (email, telepon), yang rentan terhadap kesalahan dan keterlambatan.
-   **Kurangnya Visibilitas:** Sulit bagi agen untuk melacak status permintaan mereka, dan bagi manajer untuk melihat semua permintaan yang perlu ditinjau dalam satu tempat.
-   **Alur Persetujuan yang Tidak Jelas:** Proses verifikasi dokumen dan persetujuan jadwal tidak terstruktur, memperlambat proses sandar kapal.

HarborFlow mengatasi masalah ini dengan menyediakan alur kerja digital yang jelas dan terpusat.

## 4. Solusi & Fitur Utama

HarborFlow menyediakan platform berbasis peran di mana setiap pengguna memiliki akses ke fitur yang relevan dengan tanggung jawab mereka:

-   **Otentikasi & Peran Pengguna:** Sistem login yang aman membedakan akses antara `PortManager` dan `ShippingAgent`.
-   **Manajemen Alur Kerja Lengkap:**
    -   **Agen:** Dapat membuat permintaan layanan, melampirkan dokumen, dan menambahkan detail kargo.
    -   **Manajer:** Dapat meninjau permintaan, memverifikasi dokumen, dan menyetujui jadwal sandar kapal.
-   **Manajemen Data Terpusat:** Semua data (kapal, permintaan, dokumen, kargo, jadwal) disimpan dalam database PostgreSQL, menyediakan satu sumber kebenaran.
-   **Logika Bisnis Cerdas:** Sistem secara otomatis memberlakukan aturan, seperti hanya mengizinkan penjadwalan untuk permintaan yang dokumennya telah diverifikasi sepenuhnya.
-   **Antarmuka Konsol Modern:** Dibangun menggunakan `Spectre.Console` untuk pengalaman pengguna yang bersih dan interaktif.

## 5. Tampilan Aplikasi

Berikut adalah representasi dari beberapa tampilan utama di konsol:

**Layar Login:**
```
Welcome to HarborFlow. Please log in.
Username: manager
Password: ****
```

**Menu Port Manager:**
```
HarborFlow Menu (Logged in as manager)
>   Show All Vessels
    View All Schedules
    Add New Vessel
    Create Schedule
    Verify Document
    Exit
```

**Menu Shipping Agent:**
```
HarborFlow Menu (Logged in as agent)
>   Show All Vessels
    View All Schedules
    Create Service Request
    Add Document to Request
    Manage Cargo
    Exit
```

## 6. Arsitektur & Teknologi

-   **Bahasa Pemrograman:** C# & .NET 9
-   **Database:** PostgreSQL
-   **ORM:** Entity Framework Core
-   **Library UI:** `Spectre.Console`
-   **Pola Desain:** Arsitektur N-Tier sederhana dengan pemisahan antara UI (di `Program.cs`), Layanan Bisnis (`HarborService.cs`), dan Model Data (`HarborFlowModels.cs`).

## 7. Rencana Pengembangan (Roadmap)

-   **[✔] Fase 1 (Selesai):** Fondasi aplikasi dengan CRUD, sistem login, alur kerja dokumen, dan manajemen kargo.
-   **[ ] Fase 2: Modul Keuangan:** Mengimplementasikan pembuatan `Invoice` dari `ServiceRequest` dan pencatatan `Payment`.
-   **[ ] Fase 3: Notifikasi:** Menambahkan sistem notifikasi sederhana (misalnya, agen mendapat notifikasi jika permintaan disetujui).
-   **[ ] Fase 4: Pengujian & Validasi:** Membangun proyek unit test untuk memvalidasi logika di `HarborService`.
-   **[ ] Fase 5: Antarmuka Web/Desktop:** Migrasi dari aplikasi konsol ke antarmuka grafis (GUI) untuk visualisasi yang lebih baik.

## 8. Cara Menjalankan Aplikasi

1.  Pastikan Anda memiliki **.NET 9 SDK** dan **PostgreSQL** terinstal.
2.  Buat database baru di PostgreSQL (misal: `HarborFlowDb`).
3.  Buka file `appsettings.json` dan perbarui `Password` di dalam `DefaultConnection` agar sesuai dengan konfigurasi PostgreSQL Anda.
4.  Buka terminal di direktori proyek dan jalankan `dotnet run`.
5.  Login menggunakan salah satu akun demo di bawah.

### Pengguna Demo

-   **Port Manager**
    -   **Username:** `manager`
    -   **Password:** `pass`
-   **Shipping Agent**
    -   **Username:** `agent`
    -   **Password:** `pass`
