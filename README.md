# HarborFlow - Sistem Manajemen Pelabuhan Pintar

![HarborFlow Screenshot](https://i.imgur.com/rGk5y6X.png)
*(Contoh screenshot aplikasi sedang berjalan)*

## 1. Ringkasan Proyek

HarborFlow adalah prototipe sistem manajemen pelabuhan berbasis konsol. Aplikasi ini dirancang sebagai *Command Center* mini untuk memonitor aktivitas maritim secara real-time, mengintegrasikan data cuaca dari API publik, dan mensimulasikan pergerakan kapal di sekitar area pelabuhan. Proyek ini dibangun menggunakan C# dan .NET 6 dengan antarmuka yang modern dan informatif berkat library Spectre.Console.

## 2. Tim Pengembang (Kelompok 4)

| Nama | Role | Tanggung Jawab Utama |
| --- | --- | --- |
| Mirsad Alganawi Azma | Lead Developer & Software Architect | Merancang arsitektur sistem, desain data, dan memimpin pengembangan teknis. |
| Marseillo R. B. Satrian | Full-Stack Developer | Mengimplementasikan logika backend, manajemen data, dan membangun antarmuka pengguna. |

---

## 3. Permasalahan yang Dipecahkan

Manajemen pelabuhan modern menghadapi tantangan dalam memonitor lalu lintas kapal dan kondisi lingkungan secara efisien. Tanpa sistem terpusat, operator pelabuhan seringkali kesulitan untuk:

-   **Mendapatkan Gambaran Real-Time:** Kurangnya visibilitas terpadu terhadap kapal-kapal yang mendekat, status mereka, dan perkiraan waktu tiba (ETA).
-   **Mengantisipasi Kondisi Cuaca:** Keterlambatan dalam merespons perubahan cuaca buruk dapat membahayakan navigasi dan operasional bongkar muat.
-   **Koordinasi Operasional:** Proses pemantauan yang manual dan terfragmentasi mengurangi efisiensi dan meningkatkan risiko kesalahan operasional.

HarborFlow dirancang untuk mengatasi masalah ini dengan menyediakan dashboard terpusat yang menyajikan informasi kunci secara *real-time*.

## 4. Solusi & Fitur Utama

Sebagai solusi, HarborFlow bertindak sebagai *single source of truth* bagi operator pelabuhan melalui fitur-fitur berikut:

-   **Dashboard Live:** Tampilan utama yang diperbarui secara otomatis menyajikan data cuaca, status pelabuhan, dan lalu lintas kapal dalam satu layar.
-   **Integrasi Data Cuaca:** Mengambil data cuaca (suhu dan kecepatan angin) dari Open-Meteo API untuk memberikan peringatan dini terhadap kondisi buruk.
-   **Pelacakan Kapal (Simulasi):** Mensimulasikan pergerakan kapal yang mendekat, lengkap dengan status dinamis (`En Route`, `Approaching`, `Arrived`) berdasarkan jarak ke pelabuhan.
-   **Sistem Peringatan Cerdas:** Memberikan notifikasi visual untuk kejadian penting, seperti cuaca buruk atau saat kapal memasuki zona krusial pelabuhan.
-   **Estimasi Waktu Tiba (ETA):** Menghitung perkiraan waktu tiba kapal secara dinamis berdasarkan kecepatan dan jarak, membantu perencanaan sandar.
-   **Pencatatan Kejadian (Logging):** Semua kejadian penting (peringatan, kedatangan kapal) dicatat ke dalam file log untuk audit dan analisis pasca-operasi.

## 5. Tampilan Aplikasi

Berikut adalah representasi data yang ditampilkan di dashboard:

| Vessel Traffic | Port Status |
| :--- | :--- |
| **Tabel Kapal Masuk**<br><table><thead><tr><th>Status</th><th>Nama Kapal</th><th>ETA</th><th>Posisi</th><th>Kecepatan</th></tr></thead><tbody><tr><td>🟡 Approaching</td><td>MSC Isabella</td><td>01j 22m</td><td>-6.12, 106.90</td><td>12 kts</td></tr><tr><td>🔵 En Route</td><td>Ever Ace</td><td>03j 45m</td><td>-6.25, 107.10</td><td>15 kts</td></tr><tr><td>🟢 Arrived</td><td>HMM Algeciras</td><td>-</td><td>-6.10, 106.88</td><td>0 kts</td></tr><tr><td>🔵 En Route</td><td>OOCL Hong Kong</td><td>05j 10m</td><td>-6.40, 107.25</td><td>14 kts</td></tr></tbody></table> | **Status Pelabuhan**<br>Temp: **29°C**, Angin: **15 km/h**<br><hr><strong>Ringkasan Aktivitas</strong><br>Dilacak: **4** \| Tiba: **1** \| Peringatan: **0** |

## 6. Arsitektur & Teknologi

-   **Bahasa Pemrograman:** C# & .NET 6
-   **Library Utama:** `Spectre.Console` untuk UI, `System.Net.Http` untuk API calls.
-   **API Eksternal:** Open-Meteo Weather API.
-   **Pola Desain:** Logika aplikasi dipisahkan ke dalam *Models*, *Services*, dan *UI (View)* untuk memastikan kode yang bersih dan mudah dikelola.

## 7. Aplikasi Sejenis (Competitor Landscape)

HarborFlow adalah prototipe sederhana yang terinspirasi dari *Vessel Traffic Management Systems (VTMS)* komersial. Beberapa pemain utama di industri ini meliputi:

-   **Wärtsilä Navi-Harbour & MarineTraffic:** Platform canggih yang menawarkan pemantauan lalu lintas global, analisis prediktif, dan integrasi mendalam dengan sistem pelabuhan.
-   **Kongsberg Gruppen & Indra:** Menyediakan solusi VTMS terintegrasi yang digunakan oleh pelabuhan besar di seluruh dunia untuk keamanan dan efisiensi navigasi.
-   **VesselFinder & FleetMon:** Layanan yang lebih fokus pada pelacakan kapal berbasis AIS (Automatic Identification System) untuk publik dan komersial.

HarborFlow mengambil konsep dasar dari sistem-sistem ini dan menyajikannya dalam skala yang lebih kecil dan dapat diakses.

## 8. Rencana Pengembangan (Roadmap)

Prototipe ini memiliki potensi besar untuk dikembangkan lebih lanjut. Berikut adalah beberapa fitur yang direncanakan untuk versi mendatang:

-   **[✔] Fase 1 (Current):** Dashboard konsol dengan data cuaca dan simulasi kapal.
-   **[ ] Fase 2: Integrasi Data AIS Real:** Mengganti data simulasi dengan data *real-time* dari sensor atau API AIS untuk pelacakan kapal yang akurat.
-   **[ ] Fase 3: Database & Analisis Historis:** Menyimpan data operasional ke dalam database untuk memungkinkan analisis tren, pelaporan, dan visualisasi data historis.
-   **[ ] Fase 4: Antarmuka Web/Desktop:** Mengembangkan antarmuka pengguna grafis (GUI) berbasis web atau desktop untuk visualisasi yang lebih kaya dan interaktif.
-   **[ ] Fase 5: Manajemen Bongkar Muat:** Menambahkan fitur untuk mengelola jadwal sandar kapal, alokasi dermaga, dan status bongkar muat.

## 9. Cara Menjalankan Aplikasi

1.  Pastikan Anda memiliki **.NET 6 SDK** terinstal.
2.  Buka terminal di direktori proyek.
3.  Jalankan perintah `dotnet run`.
4.  Dashboard akan muncul dan mulai memperbarui data secara otomatis. Tekan `Ctrl + C` untuk keluar.