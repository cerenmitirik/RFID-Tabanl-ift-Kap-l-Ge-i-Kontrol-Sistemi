using System.Collections.Generic;
using WinFormsApp1.Models;

namespace WinFormsApp1.Repositories
{
    public interface IKartRepository
    {
        KartBilgisi KartGetir(string uid);
        KartBilgisi AdminKartiGetir();
        void KartDurumGuncelle(string uid, KapiTipi kapi, bool yeniDurum);
        void KartTanimlaVeyaGuncelle(string uid, string kartSahibi);
        void KartYetkiDurumuGuncelle(string uid, bool aktifMi);
        void ArgeYetkiDurumuGuncelle(string uid, bool argeYetkisiVar);
        void LogEkle(string uid, string islemTipi, string kapiTipi);
        List<KartBilgisi> AktifIceridekileriGetir();
        List<KartBilgisi> TumTanimliKartlariGetir();
        List<GecisLogu> SonGecisLoglariniGetir(int limit = 30);
        List<GecisLogu> LoglariFiltrele(LogFiltre filtre);
    }
}