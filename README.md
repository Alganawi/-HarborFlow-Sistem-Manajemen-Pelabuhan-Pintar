# HarborFlow - Sistem Manajemen Pelabuhan

![HarborFlow Screenshot](https://via.placeholder.com/800x400.png?text=HarborFlow+Web+UI)
*(Contoh placeholder untuk screenshot aplikasi web yang sedang berjalan)*

## 1. Ringkasan Proyek

HarborFlow adalah sistem manajemen pelabuhan full-stack yang dibangun dengan .NET 9 (ASP.NET Core) untuk backend API dan React untuk frontend. Aplikasi ini menyediakan platform terpusat untuk mengelola alur kerja operasional pelabuhan, mulai dari permintaan layanan kapal hingga pembuatan faktur, dengan otentikasi berbasis peran untuk memastikan alur kerja yang aman dan efisien.

## 2. Tim Pengembang (Kelompok 4)

| Nama                   | Role                        | Tanggung Jawab Utama                                                         |
| ---------------------- | --------------------------- | ---------------------------------------------------------------------------- |
| Mirsad Alganawi Azma   | Lead Developer & Software Architect | Merancang arsitektur sistem, desain data, dan memimpin pengembangan teknis. |
| Marseillo R. B. Satrian| Full-Stack Developer        | Mengimplementasikan logika backend, manajemen data, dan membangun antarmuka pengguna. |

---

## 3. Permasalahan yang Dipecahkan

Operasional pelabuhan melibatkan banyak pihak dengan tanggung jawab berbeda, seperti agen pengiriman dan manajer pelabuhan. Tanpa sistem yang terintegrasi, proses-proses berikut menjadi tidak efisien:

-   **Koordinasi Manual:** Permintaan layanan, pengiriman dokumen, dan penjadwalan seringkali dilakukan secara manual (email, telepon), yang rentan terhadap kesalahan dan keterlambatan.
-   **Kurangnya Visibilitas:** Sulit bagi agen untuk melacak status permintaan mereka, dan bagi manajer untuk melihat semua permintaan yang perlu ditinjau dalam satu tempat.
-   **Alur Persetujuan yang Tidak Jelas:** Proses verifikasi dokumen dan persetujuan jadwal tidak terstruktur, memperlambat proses sandar kapal.

HarborFlow mengatasi masalah ini dengan menyediakan alur kerja digital yang jelas dan terpusat melalui antarmuka web modern.

## 4. Solusi & Fitur Utama

HarborFlow menyediakan platform berbasis peran di mana setiap pengguna memiliki akses ke fitur yang relevan dengan tanggung jawab mereka melalui antarmuka web yang responsif:

-   **Otentikasi & Otorisasi Aman:** Sistem login menggunakan JSON Web Tokens (JWT) untuk mengamankan endpoint API. Akses ke fitur-fitur spesifik dibatasi berdasarkan peran pengguna (`PortManager`, `ShippingAgent`, `FinanceAdmin`).
-   **Dasbor Interaktif:** Setelah login, pengguna disajikan dengan dasbor yang menampilkan daftar permintaan layanan dan daftar kapal.
-   **Manajemen Alur Kerja Lengkap:**
    -   **Agen:** Dapat membuat permintaan layanan baru dan menambahkan dokumen langsung dari halaman detail permintaan.
    -   **Manajer:** Dapat meninjau permintaan dan memverifikasi dokumen (fungsionalitas UI akan datang).
-   **Manajemen Keuangan:** Pengguna dengan peran `Finance` dapat membuat invoice dan mencatat pembayaran (fungsionalitas UI akan datang).
-   **Backend API yang Kuat:** Dibangun dengan ASP.NET Core, menyediakan endpoint yang aman dan efisien untuk semua operasi, dengan validasi input untuk memastikan integritas data.
-   **Frontend React Modern:** Antarmuka pengguna yang interaktif dan mudah digunakan dibangun dengan React, dengan manajemen sesi pengguna yang persisten.

## 5. Tampilan Aplikasi

Aplikasi ini terdiri dari backend API dan frontend React yang berjalan secara terpisah namun terintegrasi.

**Backend API:**
Menyediakan endpoint RESTful untuk mengelola semua data dan logika bisnis, diamankan dengan otentikasi JWT.

**Frontend Web (React):**
Memberikan pengalaman pengguna yang modern untuk berinteraksi dengan sistem, termasuk halaman login, dasbor utama, formulir pembuatan data, dan halaman detail.

## 6. Arsitektur & Teknologi

-   **Backend:**
    -   **Bahasa & Framework:** C# & ASP.NET Core (.NET 9)
    -   **Database:** PostgreSQL
    -   **ORM:** Entity Framework Core
    -   **Keamanan:** Otentikasi JWT (JSON Web Token)
-   **Frontend:**
    -   **Library:** React
    -   **HTTP Client:** Axios
-   **Struktur Proyek:**
    -   `HarborFlow.WebAPI`: Proyek ASP.NET Core yang menjadi backend.
    -   `HarborFlow.Core`: Class library yang berisi model data dan logika bisnis.
    -   `client-app`: Proyek React yang menjadi frontend.
    -   `HarborFlow.Tests`: Proyek xUnit untuk pengujian.

## 7. Aplikasi Sejenis (Competitor Landscape)

HarborFlow adalah prototipe sederhana yang terinspirasi dari *Vessel Traffic Management Systems (VTMS)* komersial. Beberapa pemain utama di industri ini meliputi:

-   **Wärtsilä Navi-Harbour & MarineTraffic:** Platform canggih yang menawarkan pemantauan lalu lintas global, analisis prediktif, dan integrasi mendalam dengan sistem pelabuhan.
-   **Kongsberg Gruppen & Indra:** Menyediakan solusi VTMS terintegrasi yang digunakan oleh pelabuhan besar di seluruh dunia untuk keamanan dan efisiensi navigasi.
-   **VesselFinder & FleetMon:** Layanan yang lebih fokus pada pelacakan kapal berbasis AIS (Automatic Identification System) untuk publik dan komersial.

HarborFlow mengambil konsep dasar dari sistem-sistem ini dan menyajikannya dalam skala yang lebih kecil dan dapat diakses.

## 8. Rencana Pengembangan (Roadmap)

-   **[✔] Fase 1 (Selesai):** Fondasi aplikasi dengan CRUD, sistem login, alur kerja dokumen, dan manajemen kargo.
-   **[✔] Fase 2 (Selesai):** Modul Keuangan untuk membuat invoice dan mencatat pembayaran.
-   **[✔] Fase 3 (Selesai):** Backend API dan Frontend Web dasar dengan otentikasi JWT.
-   **[✔] Fase 4 (Selesai):** Peningkatan Kualitas Kode Backend (mengatasi peringatan nullable dan menambah validasi input).
-   **[✔] Fase 5 (Sebagian Selesai):** Penulisan Unit Test untuk `HarborService` (mencakup service request, dokumen, kargo, dan invoice).
-   **[✔] Fase 6 (Sebagian Selesai):** Pengembangan UI Lanjutan (detail permintaan layanan dan tambah dokumen).
-   **[ ] Fase 7: Notifikasi:** Menambahkan sistem notifikasi real-time.
-   **[ ] Fase 8: Pengujian End-to-End:** Membangun pengujian untuk memvalidasi alur kerja dari UI hingga database.

## 9. Cara Menjalankan Aplikasi

### Prasyarat
-   **.NET 9 SDK**
-   **Node.js** (versi 14 atau lebih baru)
-   **PostgreSQL**

### Backend (ASP.NET Core API)
1.  Buat database baru di PostgreSQL (misal: `HarborFlowDb`).
2.  Buka file `HarborFlow.WebAPI/appsettings.json` dan perbarui `Password` di dalam `DefaultConnection` agar sesuai dengan konfigurasi PostgreSQL Anda.
3.  Buka terminal di direktori `HarborFlow.WebAPI` dan jalankan perintah:
    ```sh
    dotnet run
    ```
    API akan berjalan di `https://localhost:7246` (atau port lain yang dikonfigurasi).

### Frontend (React App)
1.  Buka terminal baru di direktori `client-app`.
2.  Install dependensi dengan menjalankan:
    ```sh
    npm install
    ```
3.  Mulai aplikasi React:
    ```sh
    npm start
    ```
    Aplikasi akan terbuka di browser pada alamat `http://localhost:3000`.

### Pengujian (Testing)

Proyek ini dilengkapi dengan unit test untuk memastikan kualitas dan stabilitas logika bisnis di `HarborService`. Tes ini mencakup pembuatan kapal, permintaan layanan, dokumen, kargo, dan invoice. Untuk menjalankan semua tes, gunakan perintah berikut dari direktori utama:

```sh
dotnet test
```

### Pengguna Demo

-   **Port Manager**
    -   **Username:** `manager`
    -   **Password:** `pass`
-   **Shipping Agent**
    -   **Username:** `agent`
    -   **Password:** `pass`
-   **Finance Admin**
    -   **Username:** `finance`
    -   **Password:** `pass`


 1. Buka Terminal Anda.

   2. Gunakan perintah `psql` untuk terhubung ke database. Berdasarkan 
      informasi dari proyek Anda, perintahnya adalah sebagai berikut:

   1     psql -h localhost -U postgres -d db_project

       * -h localhost: Menentukan localhost sebagai host database.
       * -U postgres: Menggunakan postgres sebagai nama pengguna.
       * -d db_project: Terhubung ke database bernama db_project.

   3. Masukkan Kata Sandi. Setelah menjalankan perintah di atas, Anda akan 
      diminta memasukkan kata sandi untuk pengguna postgres. Berdasarkan 
      file konfigurasi Anda, kata sandinya adalah Bizero_11.

   4. Tampilkan Tabel. Setelah berhasil terhubung, Anda akan masuk ke dalam
       shell psql. Untuk melihat daftar semua tabel di database, gunakan 
      perintah \dt:

   1     \dt

   5. Keluar dari `psql`. Jika sudah selesai, Anda bisa keluar dari shell 
      psql dengan mengetik:

   1     \q

  Itu saja! Anda akan melihat daftar tabel yang sama dengan yang saya 
  tunjukkan sebelumnya.

                List of relations
 Schema |      Name       | Type  |  Owner   
--------+-----------------+-------+----------
 public | Cargos          | table | postgres
 public | Documents       | table | postgres
 public | Invoices        | table | postgres
 public | Payments        | table | postgres
 public | Schedules       | table | postgres
 public | ServiceRequests | table | postgres
 public | Users           | table | postgres
 public | Vessels         | table | postgres
(8 rows)


                                                    List of databases
    Name    |     Owner      | Encoding | Locale Provider | Collate | Ctype | Locale | ICU Rules |   Access privileges   
------------+----------------+----------+-----------------+---------+-------+--------+-----------+-----------------------
 db_project | nama_user_baru | UTF8     | libc            | C       | C     |        |           | 
 postgres   | postgres       | UTF8     | libc            | C       | C     |        |           | 
 template0  | postgres       | UTF8     | libc            | C       | C     |        |           | =c/postgres          +
            |                |          |                 |         |       |        |           | postgres=CTc/postgres
 template1  | postgres       | UTF8     | libc            | C       | C     |        |           | =c/postgres          +
            |                |          |                 |         |       |        |           | postgres=CTc/postgres
(4 rows)

~
~
~
~
