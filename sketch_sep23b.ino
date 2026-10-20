#include <SPI.h>
#include <MFRC522.h>

// --- Pinler ---
#define RST_PIN     5
#define SS_BINA     10 // sda pinleri yani rc522 seçtiği cihazlar
#define SS_ARGE     9 // sda 
#define BUZZER_PIN  4

MFRC522 rcBina(SS_BINA, RST_PIN);
MFRC522 rcArge(SS_ARGE, RST_PIN);

MFRC522::MIFARE_Key varsayilanAnahtar;
const byte KIMLIK_BLOK = 4;   // Sektör 1, Blok 4

// --- Yazma modu durumu ---
bool yazmaBekleniyor = false;
String yazilacakKimlik = "";

// ---------------- BUZZER ----------------
void buzzerGiris()   { digitalWrite(BUZZER_PIN, HIGH); delay(120); digitalWrite(BUZZER_PIN, LOW); }
void buzzerCikis()   { digitalWrite(BUZZER_PIN, HIGH); delay(650); digitalWrite(BUZZER_PIN, LOW); }
void buzzerYetkisiz(){ for (int i=0;i<3;i++){ digitalWrite(BUZZER_PIN,HIGH); delay(80); digitalWrite(BUZZER_PIN,LOW); delay(80);} }
void buzzerAdmin()   { for (int i=0;i<2;i++){ digitalWrite(BUZZER_PIN,HIGH); delay(300); digitalWrite(BUZZER_PIN,LOW); delay(150);} }
void buzzerUyari()   { for (int i=0;i<2;i++){ digitalWrite(BUZZER_PIN,HIGH); delay(200); digitalWrite(BUZZER_PIN,LOW); delay(300);} }

// ---------------- UID OKUMA ----------------
String hamUidOku(MFRC522 &rc) { // girdi: okuyucu nesnesi referansı
  String s = "";
  for (byte i = 0; i < rc.uid.size; i++) { // kartın rc.uid.uidByte dizisini gezer. byter değeri 0*10 yani 16 dan üçükse başına 0 ekler (padding) 
    if (rc.uid.uidByte[i] < 0x10) s += "0";
    s += String(rc.uid.uidByte[i], HEX); // boşuk yoapr 
    if (i < rc.uid.size - 1) s += " ";
  }
  s.toUpperCase(); // c# beklediği foramta normalize eder.
  return s; // kartın uid
}

// ---------------- MANTIKSAL KİMLİK OKUMA ----------------
// Blok 4 boşsa "" döner, doluysa hex-space formatında kimliği döner
String kimlikOku(MFRC522 &rc) {
  byte buffer[18];
  byte size = 18;

  MFRC522::StatusCode durum = rc.PCD_Authenticate(
      MFRC522::PICC_CMD_MF_AUTH_KEY_A, KIMLIK_BLOK, &varsayilanAnahtar, &(rc.uid));
  if (durum != MFRC522::STATUS_OK) return "";

  durum = rc.MIFARE_Read(KIMLIK_BLOK, buffer, &size);
  rc.PCD_StopCrypto1();
  if (durum != MFRC522::STATUS_OK) return "";

  byte uzunluk = buffer[0];

  // Geçerli bir mantıksal kimlik yazılmış mı kontrolü:
  if (uzunluk != 4 && uzunluk != 7) {
    return ""; // Boş veya tanımsız blok
  }

  // Belirtilen uzunluk kadar byte'ı Hex String'e çevir
  String s = "";
  for (byte i = 0; i < uzunluk; i++) {
    byte b = buffer[1 + i];
    if (b < 0x10) s += "0";
    s += String(b, HEX);
    if (i < uzunluk - 1) s += " ";
  }
  s.toUpperCase();
  return s;
}

// ---------------- MANTIKSAL KİMLİK YAZMA ----------------
bool kimlikYaz(MFRC522 &rc, String hedefHex) {
  byte veri[16];
  memset(veri, 0, 16);

  // String'i parçala ve byte dizisine çevir (Maksimum 10 byte UID destekler)
  byte uidBytes[10];
  byte byteSayisi = 0;
  int parcaBaslangic = 0;

  for (int i = 0; i <= hedefHex.length() && byteSayisi < 10; i++) {
    if (i == hedefHex.length() || hedefHex[i] == ' ') {
      String parca = hedefHex.substring(parcaBaslangic, i);
      if (parca.length() > 0) {
        uidBytes[byteSayisi++] = (byte)strtol(parca.c_str(), NULL, 16);
      }
      parcaBaslangic = i + 1;
    }
  }

  if (byteSayisi != 4 && byteSayisi != 7) {
    return false; // Standart dışı uzunluk
  }

  // Protokol Formatı:
  // Byte 0: Kimliğin uzunluğu (4 veya 7)
  // Byte 1..N: Kimlik byte'ları
  veri[0] = byteSayisi;
  for (byte i = 0; i < byteSayisi; i++) {
    veri[1 + i] = uidBytes[i];
  }

  // Kart Doğrulama (Sektör 1, Blok 4)
  MFRC522::StatusCode durum = rc.PCD_Authenticate(
      MFRC522::PICC_CMD_MF_AUTH_KEY_A, KIMLIK_BLOK, &varsayilanAnahtar, &(rc.uid));
  if (durum != MFRC522::STATUS_OK) { rc.PCD_StopCrypto1(); return false; }

  // 16 Byte'ı Blok 4'e Yaz
  durum = rc.MIFARE_Write(KIMLIK_BLOK, veri, 16);
  rc.PCD_StopCrypto1();
  return durum == MFRC522::STATUS_OK;
}

// ---------------- BİR OKUYUCUYU İŞLE ----------------
void okuyucuyuIsle(MFRC522 &rc, String kapiEtiketi) { // girdi : okuyuu nesnersi ve kapı kimliği : bina veya arge 
  if (!rc.PICC_IsNewCardPresent() || !rc.PICC_ReadCardSerial()) return; // kart yoksa veya seri okuma başlatılamazsa fonksiyondan çık

  if (yazmaBekleniyor) { // kontrol : yazmaBekleniyor == true ise kartın kimliğini okumaz; doğrudan kimlikYaz() çalıştırır.
    bool basarili = kimlikYaz(rc, yazilacakKimlik);
    Serial.println(basarili ? ("WRITE_OK:" + yazilacakKimlik) : "WRITE_FAIL");
    yazmaBekleniyor = false;
    yazilacakKimlik = "";
    rc.PICC_HaltA(); // kart uyku moduna al
    return;
  }

  String hamUid = hamUidOku(rc); // önce hamuid oku sonra kimlik oku çağır.
  String kimlik = kimlikOku(rc);
  String efektif = (kimlik.length() > 0) ? kimlik : hamUid;// eğer blok4 doluysa kimlikOku , blok4 boşsa HamUid oku 

  Serial.println("UID:" + kapiEtiketi + ":" + efektif); // sonucu seri porta UİD:KAPI:EFEKİTF KİMLİK 

  rc.PICC_HaltA();
  delay(300);
}

void setup() {
  Serial.begin(115200);
  delay(1000);

  pinMode(BUZZER_PIN, OUTPUT);
  digitalWrite(BUZZER_PIN, LOW);

  for (byte i = 0; i < 6; i++) varsayilanAnahtar.keyByte[i] = 0xFF;

  SPI.begin(12, 13, 11, 10); // SCK, MISO, MOSI (SS parametresi burada kullanılmıyor, ayrı ayrı veriliyor)
  rcBina.PCD_Init();
  rcArge.PCD_Init();

  Serial.println("Sistem hazir - 2 kapi aktif.");
}

void loop() {
  // C#'tan gelen komutları dinle
  if (Serial.available() > 0) {
    String komut = Serial.readStringUntil('\n');
    komut.trim();

    if (komut == "CMD:GIRIS") buzzerGiris();
    else if (komut == "CMD:CIKIS") buzzerCikis();
    else if (komut == "CMD:YETKISIZ") buzzerYetkisiz();
    else if (komut == "CMD:ADMIN") buzzerAdmin();
    else if (komut == "CMD:UYARI") buzzerUyari();
    else if (komut.startsWith("CMD:WRITE:")) {
      yazilacakKimlik = komut.substring(10);
      yazmaBekleniyor = true;
      Serial.println("Yazma bekleniyor, karti yaklastirin...");
    }
    else if (komut.startsWith("CMD:CANCEL")){
      yazmaBekleniyor = false ; 
      yazilacakKimlik = "";
      Serial.println("Yazma iptal edildi");
    }
  }

  okuyucuyuIsle(rcBina, "BINA");
  okuyucuyuIsle(rcArge, "ARGE");
}