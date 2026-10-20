namespace WinFormsApp1.Models
{
    public class KartBilgisi
    {
        public string KartUID { get; set; }
        public string KartSahibi { get; set; }
        public bool IcerideMi { get; set; }       // Bina içinde mi
        public bool ArgeOdasindaMi { get; set; }
        public bool AktifMi { get; set; }         // Bina giriş yetkisi
        public bool ArgeYetkisiVar { get; set; }  // AR-GE giriş yetkisi
        public bool IsAdmin { get; set; }
    }
}