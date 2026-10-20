# RFID Tabanlı Çift Kapılı Geçiş Kontrol Sistemi

## Proje Hakkında

Bu proje, ESP32 mikrodenetleyici, MFRC522 RFID kart okuyucuları ve C# Windows Forms kullanılarak geliştirilmiş iki kapılı bir geçiş kontrol ve yetkilendirme sistemidir.

Sistem, Bina ve AR-GE olmak üzere iki farklı geçiş noktasında RFID kartların okunmasını, kullanıcıların erişim yetkilerinin kontrol edilmesini ve giriş-çıkış hareketlerinin kayıt altına alınmasını sağlar.

Windows Forms üzerinde geliştirilen yönetim paneli sayesinde yeni kartlar tanımlanabilir, kullanıcıların erişim yetkileri düzenlenebilir ve kart kimlik bilgileri başka kartlara kopyalanabilir.

## Kullanılan Teknolojiler

**Donanım:**
- ESP32 mikrodenetleyici
- 2 adet MFRC522 RFID kart okuyucu
- RFID kartlar
- Buzzer

**Yazılım:**
- C#
- .NET Windows Forms
- Arduino IDE (C/C++)
- MFRC522 kütüphanesi
- SPI haberleşme protokolü
- Seri port (Serial Communication)

## Sistem Özellikleri

### 1. RFID Kart ile Geçiş Kontrolü

- İki farklı kapı için bağımsız RFID okuyucu kullanılır.
- Okutulan kartların kimlik bilgileri ESP32 tarafından okunur.
- Kart bilgileri seri haberleşme üzerinden C# uygulamasına iletilir.
- Kartın sistemde kayıtlı olup olmadığı ve ilgili kapıya erişim yetkisi kontrol edilir.
- Yetkili ve yetkisiz geçişler için farklı buzzer uyarıları verilir.

### 2. Admin Yönetim Paneli

Windows Forms ile geliştirilen admin paneli üzerinden:

- Yeni RFID kartları sisteme kaydetme
- Kart sahiplerini tanımlama
- Mevcut kart bilgilerini güncelleme
- Bina giriş yetkisini düzenleme
- AR-GE giriş yetkisini düzenleme
- Kart kimlik bilgilerini başka karta kopyalama
- Kart kopyalama işlemini iptal etme

işlemleri gerçekleştirilebilir.

Admin kartlarının normal kart kopyalama listesinde gösterilmesi engellenmiştir.

### 3. RFID Kart Kopyalama

Sistemde kayıtlı bir kartın kimlik bilgisi başka bir uyumlu RFID karta aktarılabilir.

Bu işlem sırasında kaynak kartın kimlik bilgisi, hedef kartın MIFARE bellek alanındaki 4 numaralı bloğa yazılır.

Sistem, kartın fiziksel UID bilgisini değiştirmek yerine uygulama düzeyinde tanımlanan kimlik bilgisini kullanır.

### 4. Buzzer ile Sesli Bildirim

Sistem farklı olaylar için farklı sesli uyarılar oluşturur.

| Olay | Buzzer Davranışı |
|---|---|
| Giriş | Kısa tek bip |
| Çıkış | Uzun tek bip |
| Yetkisiz geçiş | Üç kısa bip |
| Admin işlemi | İki uzun bip |
| Uyarı | İki orta uzunlukta bip |

### 5. Giriş-Çıkış Kayıtları

Sistem üzerinden gerçekleştirilen giriş ve çıkış işlemleri kayıt altına alınır.

Bu kayıtlar sayesinde kartların geçiş hareketleri takip edilebilir.

## Sistem Çalışma Mantığı

1. Kullanıcı RFID kartını Bina veya AR-GE okuyucusuna yaklaştırır.
2. ESP32, MFRC522 aracılığıyla kartın kimlik bilgisini okur.
3. Okunan kimlik ve kapı bilgisi seri port üzerinden Windows Forms uygulamasına gönderilir.
4. Uygulama, kartın kayıt ve yetki durumunu kontrol eder.
5. Geçiş sonucuna göre ESP32'ye buzzer komutu gönderilir.
6. Gerçekleşen giriş-çıkış hareketleri kayıt altına alınır.

## Seri Haberleşme

ESP32 ile C# uygulaması arasında 115200 baud hızında seri haberleşme kullanılır.

Örnek kart okuma mesajları:

```text
UID:BINA:AA BB CC DD
UID:ARGE:11 22 33 44
```

C# uygulamasından ESP32'ye gönderilebilen komutlar:

```text
CMD:GIRIS
CMD:CIKIS
CMD:YETKISIZ
CMD:ADMIN
CMD:UYARI
CMD:WRITE:AA BB CC DD
CMD:CANCEL
```

## Proje Dosyaları

- `Models/` – Veri modelleri
- `Repositories/` – Veri erişim işlemleri
- `Services/` – Geçiş kontrolü ve iş mantığı
- `AdminForm.cs` – Yönetici paneli
- `Form1.cs` – Ana uygulama ekranı
- `Arduino/ESP32_RFID.ino` – ESP32 RFID okuyucu ve buzzer kodu

## Donanım Bağlantıları

Kodda kullanılan ESP32 pin tanımlamaları:

| Bağlantı | ESP32 GPIO |
|---|---|
| MFRC522 RST | 5 |
| Bina RFID SS/SDA | 10 |
| AR-GE RFID SS/SDA | 9 |
| Buzzer | 4 |
| SPI SCK | 12 |
| SPI MISO | 13 |
| SPI MOSI | 11 |

İki RFID okuyucu ortak SPI hattını kullanırken ayrı SS pinleri üzerinden seçilir.

*Not: Pin bağlantıları kullanılan ESP32 kart modeline göre doğrulanmalıdır.*

## Güvenlik Notu

Proje, RFID tabanlı erişim kontrolü ve yönetim işlemlerini uygulamalı olarak göstermek amacıyla geliştirilmiştir. MIFARE Classic kartlarda varsayılan anahtar ve kopyalanabilir kimlik bilgileri kullanılması, yüksek güvenlik gerektiren gerçek erişim sistemleri için yeterli değildir.

## Projenin Amacı

Bu projenin amacı, RFID teknolojisi ile fiziksel erişim kontrolünü masaüstü yazılımıyla birleştirerek merkezi olarak yönetilebilen, yetkilendirme ve geçiş takibi yapabilen bir sistem geliştirmektir.
