using System;
using System.Collections.Generic;
using WinFormsApp1.Models;
using WinFormsApp1.Repositories;

namespace WinFormsApp1.Services
{
    public enum GecisSonucTipi
    {
        GirisYapildi,
        CikisYapildi,
        YetkisizKart,
        YoksayildiDebounce,
        AdminOturumuBasladi,
        AdminOturumuBitti,
        AdminModundaHedefSecildi,
        UyariOnceBina,
        UyariArgedenCikilmadi,
        KlonReddedildi,
        KlonOnaylandi
    }

    public class GecisYanit
    {
        public GecisSonucTipi Sonuc { get; set; }
        public string BuzzerKomutu { get; set; }
        public string Mesaj { get; set; }
        public string SeciliKartUid { get; set; }   // admin modunda hedef kart okutulunca dolar
        public string KlonYazilacakKimlik { get; set; } // YENİ
    }

    public class GecisKontrolServisi
    {
        private readonly IKartRepository _kartRepo;
        private string _sonOkunanUid = "";
        private DateTime _sonOkumaZamani = DateTime.MinValue;
        private const int DEBOUNCE_SANIYE = 3;

        // Admin oturumu - sadece bellekte, her program açılışında kapalı başlar
        private bool _adminAktif = false;
        private string _adminAdi = null;

        // Klonlama durum alanları:
        private bool _klonDogrulamaBekleniyor = false;
        private string _klonKaynakKimlik = null;

        public bool AdminOturumuAktif => _adminAktif;
        public string AktifAdminAdi => _adminAdi;

        public GecisKontrolServisi(IKartRepository kartRepo)
        {
            _kartRepo = kartRepo;
        }

        // 1. Klonlama başlatma (Admin kartı kopyalanamaz kuralı dahil)
        public string KlonBaslat(string kaynakKimlik)
        {
            if (!_adminAktif) return "Önce admin oturumu açılmalıdır.";

            var kaynakKart = _kartRepo.KartGetir(kaynakKimlik);
            if (kaynakKart != null && kaynakKart.IsAdmin)
            {
                return "GÜVENLİK UYARISI: Admin kartı kopyalanamaz!";
            }

            _klonDogrulamaBekleniyor = true;
            _klonKaynakKimlik = kaynakKimlik;
            return "Lütfen BOŞ kartı okutun (önce doğrulanacak)...";
        }

        // 2. Klonlama iptali (Form kapandığında çağrılır)
        public void KlonIptalEt()
        {
            _klonDogrulamaBekleniyor = false;
            _klonKaynakKimlik = null;
        }

        public GecisYanit KartiIsle(string uid, KapiTipi kapi)
        {
            // KLONLAMA DOĞRULAMA KONTROLÜ (Okunan kart gerçekten boş mu?)
            if (_klonDogrulamaBekleniyor)
            {
                _klonDogrulamaBekleniyor = false;
                var hedefKart = _kartRepo.KartGetir(uid);

                if (hedefKart != null)
                {
                    string uyari = hedefKart.IsAdmin
                        ? "Bu kart ADMIN kartı! Üzerine yazılamaz."
                        : $"Bu kart zaten '{hedefKart.KartSahibi}' kişisine kayıtlı! Tanımlı kartlar üzerine yazılamaz. Lütfen BOŞ bir kart kullanın.";

                    return new GecisYanit
                    {
                        Sonuc = GecisSonucTipi.KlonReddedildi,
                        Mesaj = uyari
                    };
                }

                // Kart sistemde kayıtlı değil, güvenli:
                return new GecisYanit
                {
                    Sonuc = GecisSonucTipi.KlonOnaylandi,
                    KlonYazilacakKimlik = _klonKaynakKimlik
                };
            }

            // DEBOUNCE
            if (uid == _sonOkunanUid && (DateTime.Now - _sonOkumaZamani).TotalSeconds < DEBOUNCE_SANIYE)
                return new GecisYanit { Sonuc = GecisSonucTipi.YoksayildiDebounce };

            _sonOkunanUid = uid;
            _sonOkumaZamani = DateTime.Now;

            var kart = _kartRepo.KartGetir(uid);

            // ADMIN KARTI MI?
            if (kart != null && kart.IsAdmin)
            {
                if (!_adminAktif)
                {
                    _adminAktif = true;
                    _adminAdi = kart.KartSahibi;
                    _kartRepo.LogEkle(kart.KartUID, "ADMIN_AC", kapi.ToString());
                    return new GecisYanit
                    {
                        Sonuc = GecisSonucTipi.AdminOturumuBasladi,
                        BuzzerKomutu = "CMD:ADMIN",
                        Mesaj = $"{kart.KartSahibi} - Yetkilendirme modu AÇILDI."
                    };
                }
                else
                {
                    _adminAktif = false;
                    _adminAdi = null;
                    _kartRepo.LogEkle(kart.KartUID, "ADMIN_KAPA", kapi.ToString());
                    return new GecisYanit
                    {
                        Sonuc = GecisSonucTipi.AdminOturumuBitti,
                        BuzzerKomutu = "CMD:ADMIN",
                        Mesaj = "Yetkilendirme modu KAPANDI."
                    };
                }
            }

            // 3. ADMİN OTURUMU AÇIKKEN OKUTULAN HER KART (Kayıtlı veya Tanımsız!) -> HEDEF SEÇİMİDİR
            if (_adminAktif)
            {
                return new GecisYanit
                {
                    Sonuc = GecisSonucTipi.AdminModundaHedefSecildi,
                    Mesaj = kart != null ? kart.KartSahibi : "Yeni Kart",
                    SeciliKartUid = uid
                };
            }

            // 4. ADMİN MODU KAPALIYKEN VE KART SİSTEMDE YOKSA -> YETKİSİZ GEÇİŞ
            if (kart == null)
            {
                _kartRepo.LogEkle(uid, "YETKISIZ", kapi.ToString());
                return new GecisYanit
                {
                    Sonuc = GecisSonucTipi.YetkisizKart,
                    BuzzerKomutu = "CMD:YETKISIZ",
                    Mesaj = "Tanımsız kart!"
                };
            }

            // TANIMSIZ KART UYARISI
            if (kart == null)
            {
                _kartRepo.LogEkle(uid, "YETKISIZ", kapi.ToString());
                return new GecisYanit { Sonuc = GecisSonucTipi.YetkisizKart, BuzzerKomutu = "CMD:YETKISIZ", Mesaj = "Tanımsız kart!" };
            }

            // 5. Normal kapı geçiş kontrolü
            return kapi == KapiTipi.BinaKapisi ? BinaKapisiIsle(kart) : ArgeKapisiIsle(kart);
        }

        private GecisYanit BinaKapisiIsle(KartBilgisi kart)
        {
            if (!kart.AktifMi)
            {
                _kartRepo.LogEkle(kart.KartUID, "YETKISIZ", "BinaKapisi");
                return new GecisYanit { Sonuc = GecisSonucTipi.YetkisizKart, BuzzerKomutu = "CMD:YETKISIZ", Mesaj = "Bina giriş yetkiniz yok!" };
            }

            bool girisYapiliyor = !kart.IcerideMi;

            // Kural: AR-GE'deyken bina dışına çıkamaz
            if (!girisYapiliyor && kart.ArgeOdasindaMi)
            {
                return new GecisYanit { Sonuc = GecisSonucTipi.UyariArgedenCikilmadi, BuzzerKomutu = "CMD:UYARI", Mesaj = "Önce AR-GE odasından çıkış yapmalısınız!" };
            }

            string islem = girisYapiliyor ? "GIRIS" : "CIKIS";
            _kartRepo.KartDurumGuncelle(kart.KartUID, KapiTipi.BinaKapisi, girisYapiliyor);
            _kartRepo.LogEkle(kart.KartUID, islem, "BinaKapisi");

            return new GecisYanit
            {
                Sonuc = girisYapiliyor ? GecisSonucTipi.GirisYapildi : GecisSonucTipi.CikisYapildi,
                BuzzerKomutu = girisYapiliyor ? "CMD:GIRIS" : "CMD:CIKIS",
                Mesaj = $"{kart.KartSahibi} - Bina {islem}"
            };
        }

        private GecisYanit ArgeKapisiIsle(KartBilgisi kart)
        {
            if (!kart.ArgeYetkisiVar)
            {
                _kartRepo.LogEkle(kart.KartUID, "YETKISIZ", "ArgeKapisi");
                return new GecisYanit { Sonuc = GecisSonucTipi.YetkisizKart, BuzzerKomutu = "CMD:YETKISIZ", Mesaj = "AR-GE erişim yetkiniz yok!" };
            }

            bool girisYapiliyor = !kart.ArgeOdasindaMi;

            // Kural: Önce binaya girmemişse AR-GE'ye giremez
            if (girisYapiliyor && !kart.IcerideMi)
            {
                return new GecisYanit { Sonuc = GecisSonucTipi.UyariOnceBina, BuzzerKomutu = "CMD:UYARI", Mesaj = "Önce ana binaya giriş yapmalısınız!" };
            }

            string islem = girisYapiliyor ? "GIRIS" : "CIKIS";
            _kartRepo.KartDurumGuncelle(kart.KartUID, KapiTipi.ArgeKapisi, girisYapiliyor);
            _kartRepo.LogEkle(kart.KartUID, islem, "ArgeKapisi");

            return new GecisYanit
            {
                Sonuc = girisYapiliyor ? GecisSonucTipi.GirisYapildi : GecisSonucTipi.CikisYapildi,
                BuzzerKomutu = girisYapiliyor ? "CMD:GIRIS" : "CMD:CIKIS",
                Mesaj = $"{kart.KartSahibi} - AR-GE {islem}"
            };
        }

        // Admin oturumu açıkken Form'dan çağrılır
        public string YetkiUygula(string hedefUid, bool binaYetkisi, bool argeYetkisi)
        {
            if (!_adminAktif) return "Önce admin kartını okutun.";
            if (string.IsNullOrEmpty(hedefUid)) return "Önce bir kart seçin.";

            _kartRepo.KartYetkiDurumuGuncelle(hedefUid, binaYetkisi);
            _kartRepo.ArgeYetkiDurumuGuncelle(hedefUid, argeYetkisi);
            return "Yetkiler güncellendi.";
        }

        public void KartiYetkilendir(string uid, string adSoyad)
        {
            _kartRepo.KartTanimlaVeyaGuncelle(uid, adSoyad);
        }

        // GecisKontrolServisi.cs içine ekle
        public void AdminOturumunuZorlaKapat()
        {
            _adminAktif = false;
            _adminAdi = null;
        }
        public List<GecisLogu> LoglariFiltrele(LogFiltre filtre)
        {
            if (filtre.BaslangicTarih.HasValue && filtre.BitisTarih.HasValue &&
                filtre.BaslangicTarih.Value.Date > filtre.BitisTarih.Value.Date)
                throw new ArgumentException("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

            return _kartRepo.LoglariFiltrele(filtre);
        }

        public List<KartBilgisi> KayitliKartlariGetir() => _kartRepo.TumTanimliKartlariGetir();
    }
}