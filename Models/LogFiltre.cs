using System;

namespace WinFormsApp1.Models
{
    public class LogFiltre
    {
        public DateTime? BaslangicTarih { get; set; }   // null = tarih filtresi kapalı
        public DateTime? BitisTarih { get; set; }
        public TimeSpan? SaatBaslangic { get; set; }    // null = saat filtresi kapalı
        public TimeSpan? SaatBitis { get; set; }
        public string KisiAdi { get; set; }             // null/boş = isim filtresi kapalı

        public bool Bos =>
            !BaslangicTarih.HasValue && !BitisTarih.HasValue &&
            !SaatBaslangic.HasValue && !SaatBitis.HasValue &&
            string.IsNullOrWhiteSpace(KisiAdi);
    }
}