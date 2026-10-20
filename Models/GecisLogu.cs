using System;

namespace WinFormsApp1.Models
{
    public class GecisLogu
    {
        public int Id { get; set; }
        public string KartUID { get; set; }
        public string KartSahibi { get; set; }
        public string IslemTipi { get; set; }
        public string KapiTipi { get; set; }
        public DateTime TarihSaat { get; set; }
    }
}