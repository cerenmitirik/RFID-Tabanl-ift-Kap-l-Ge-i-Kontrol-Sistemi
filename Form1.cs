using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using WinFormsApp1.Models;
using WinFormsApp1.Repositories;
using WinFormsApp1.Services;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private SerialPort _serialPort;
        private readonly StringBuilder _seriTampon = new StringBuilder();
        private string _sonOkunanUID = "";

        private readonly IKartRepository _kartRepo;
        private readonly GecisKontrolServisi _gecisServisi;

        private LogFiltre _aktifFiltre = null;
        private AdminForm _aktifAdminForm = null;
        public Form1()
        {
            InitializeComponent();

            string connStr = "Server=DESKTOP-UCV1A8M;Database=AkilliGecisDb;Trusted_Connection=True;TrustServerCertificate=True;";
            _kartRepo = new KartRepository(connStr);
            _gecisServisi = new GecisKontrolServisi(_kartRepo);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgvLoglar.ReadOnly = true; dgvLoglar.AllowUserToAddRows = false;
            dgvIceridekiler.ReadOnly = true; dgvIceridekiler.AllowUserToAddRows = false;
            dgvTanimliKartlar.ReadOnly = true; dgvTanimliKartlar.AllowUserToAddRows = false;

            dtpBaslangic.Format = DateTimePickerFormat.Short;
            dtpBitis.Format = DateTimePickerFormat.Short;
            dtpSaatBaslangic.Format = DateTimePickerFormat.Custom; dtpSaatBaslangic.CustomFormat = "HH:mm"; dtpSaatBaslangic.ShowUpDown = true;
            dtpSaatBitis.Format = DateTimePickerFormat.Custom; dtpSaatBitis.CustomFormat = "HH:mm"; dtpSaatBitis.ShowUpDown = true;

            chkTarih.CheckedChanged += (s, ev) => FiltreKutulariniGuncelle();
            chkSaat.CheckedChanged += (s, ev) => FiltreKutulariniGuncelle();
            FiltreKutulariniSifirla();

            cmbPortlar.Items.Clear();
            cmbPortlar.Items.AddRange(SerialPort.GetPortNames());
            if (cmbPortlar.Items.Count > 0)
            {
                int com14 = cmbPortlar.Items.IndexOf("COM14");
                cmbPortlar.SelectedIndex = com14 != -1 ? com14 : 0;
            }
            else MessageBox.Show("COM portu bulunamadý!");

            EkraniYenile();
        }

        private void cmbPortlar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPortlar.SelectedItem == null) return;
            try
            {
                if (_serialPort == null)
                {
                    _serialPort = new SerialPort();
                    _serialPort.DataReceived += SerialPort_DataReceived;
                }
                if (_serialPort.IsOpen) _serialPort.Close();

                _serialPort.PortName = cmbPortlar.SelectedItem.ToString();
                _serialPort.BaudRate = 115200;
                _serialPort.DtrEnable = true;
                _serialPort.RtsEnable = true;
                _serialPort.Open();
            }
            catch (Exception ex) { MessageBox.Show("Port hatasý: " + ex.Message); }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var satirlar = new List<string>();
            try
            {
                string gelen = _serialPort.ReadExisting();
                lock (_seriTampon)
                {
                    _seriTampon.Append(gelen);
                    string tampon = _seriTampon.ToString();
                    int idx;
                    while ((idx = tampon.IndexOf('\n')) >= 0)
                    {
                        satirlar.Add(tampon.Substring(0, idx).Trim());
                        tampon = tampon.Substring(idx + 1);
                    }
                    _seriTampon.Clear();
                    _seriTampon.Append(tampon);
                }
            }
            catch (Exception ex) { Debug.WriteLine("Seri okuma hatasý: " + ex.Message); return; }

            foreach (string satir in satirlar) SatiriIsle(satir);
        }

        private void SatiriIsle(string satir)
        {
            try
            {
                if (satir.StartsWith("UID:"))
                {
                    // Format: UID:BINA:5A 62 30 40  veya  UID:ARGE:...
                    string[] parca = satir.Substring(4).Split(new[] { ':' }, 2);
                    if (parca.Length != 2) return;

                    KapiTipi kapi = parca[0] == "ARGE" ? KapiTipi.ArgeKapisi : KapiTipi.BinaKapisi;
                    string uid = parca[1].Trim();
                    _sonOkunanUID = uid;

                    var yanit = _gecisServisi.KartiIsle(uid, kapi);

                    if (!string.IsNullOrEmpty(yanit.BuzzerKomutu)) SeriKomutGonder(yanit.BuzzerKomutu);

                    if (IsHandleCreated && !IsDisposed)
                    {
                        BeginInvoke(new Action(() => YanitiIsle(yanit)));
                    }
                }
                else if (satir.StartsWith("WRITE_OK"))
                {
                    if (IsHandleCreated && !IsDisposed)
                        BeginInvoke(new Action(() => MessageBox.Show("Kart yazma / klonlama iþlemi BAÞARILI!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                }
                else if (satir.StartsWith("WRITE_FAIL"))
                {
                    if (IsHandleCreated && !IsDisposed)
                        BeginInvoke(new Action(() => MessageBox.Show("Kart yazma BAÞARISIZ oldu! Kartý okuyucuya yakýn tutarak tekrar deneyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
            catch (Exception ex) { Debug.WriteLine("Kart iþleme hatasý: " + ex.Message); }
        }

        // BeginInvoke içinden, UI thread'inde çalýþýr
        private void YanitiIsle(GecisYanit yanit)
        {
            switch (yanit.Sonuc)
            {
                case GecisSonucTipi.YoksayildiDebounce:
                    return;
                case GecisSonucTipi.KlonReddedildi:
                    MessageBox.Show(yanit.Mesaj, "Klonlama Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _aktifAdminForm?.KlonSonucunuGoster("Reddedildi: " + yanit.Mesaj);
                    return;
                case GecisSonucTipi.KlonOnaylandi:
                    SeriKomutGonder("CMD:WRITE:" + yanit.KlonYazilacakKimlik);
                    _aktifAdminForm?.KlonSonucunuGoster("Kart boþ, onaylandý! Kimliði yazmak için kartý okuyucuya YAKIN TUTUN.");
                    return;
                case GecisSonucTipi.AdminOturumuBasladi:
                    if (_aktifAdminForm == null || _aktifAdminForm.IsDisposed)
                    {
                        _aktifAdminForm = new AdminForm(_gecisServisi, _kartRepo, SeriKomutGonder);
                        _aktifAdminForm.FormClosed += (s, ev) =>
                        {
                            _aktifAdminForm = null;
                            _gecisServisi.AdminOturumunuZorlaKapat();
                            EkraniYenile();
                        };
                        _aktifAdminForm.Show();
                    }
                    EkraniYenile();
                    return;
                case GecisSonucTipi.AdminOturumuBitti:
                    if (_aktifAdminForm != null && !_aktifAdminForm.IsDisposed)
                    {
                        _aktifAdminForm.Close();
                        _aktifAdminForm = null;
                    }
                    EkraniYenile();
                    return;
                case GecisSonucTipi.AdminModundaHedefSecildi:
                    // Kart okutulduðunda hedef kiþi bilgisini açýk olan AdminForm'a gönderiyoruz
                    if (_aktifAdminForm != null && !_aktifAdminForm.IsDisposed)
                    {
                        _aktifAdminForm.HedefKartSecildi(yanit.SeciliKartUid, yanit.Mesaj);
                    }
                    return;

                case GecisSonucTipi.UyariArgedenCikilmadi:
                case GecisSonucTipi.UyariOnceBina:
                case GecisSonucTipi.YetkisizKart:
                    MessageBox.Show(yanit.Mesaj, "Geçiþ Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    EkraniYenile();
                    return;

                default:
                    EkraniYenile();
                    return;
            }
        }
        private void EkraniYenile()
        {
            try
            {
                dgvLoglar.DataSource = null;
                dgvLoglar.DataSource = _aktifFiltre != null
                    ? _gecisServisi.LoglariFiltrele(_aktifFiltre)
                    : _kartRepo.SonGecisLoglariniGetir(30);
                LogBasliklariniDuzenle();

                dgvIceridekiler.DataSource = null;
                dgvIceridekiler.DataSource = _kartRepo.AktifIceridekileriGetir();
                if (dgvIceridekiler.Columns["KartUID"] != null) dgvIceridekiler.Columns["KartUID"].HeaderText = "Kart UID";
                if (dgvIceridekiler.Columns["KartSahibi"] != null) dgvIceridekiler.Columns["KartSahibi"].HeaderText = "Ýçerideki Kiþi";
                foreach (var kolon in new[] { "IcerideMi", "ArgeOdasindaMi", "AktifMi", "ArgeYetkisiVar", "IsAdmin" })
                    if (dgvIceridekiler.Columns[kolon] != null) dgvIceridekiler.Columns[kolon].Visible = false;

                dgvTanimliKartlar.DataSource = null;
                dgvTanimliKartlar.DataSource = _gecisServisi.KayitliKartlariGetir();
                if (dgvTanimliKartlar.Columns["KartUID"] != null) dgvTanimliKartlar.Columns["KartUID"].HeaderText = "Kart UID";
                if (dgvTanimliKartlar.Columns["KartSahibi"] != null) dgvTanimliKartlar.Columns["KartSahibi"].HeaderText = "Kullanýcý";
                if (dgvTanimliKartlar.Columns["IcerideMi"] != null) dgvTanimliKartlar.Columns["IcerideMi"].HeaderText = "Binada Mý?";
                if (dgvTanimliKartlar.Columns["ArgeOdasindaMi"] != null) dgvTanimliKartlar.Columns["ArgeOdasindaMi"].HeaderText = "AR-GE'de Mi?"; // anlýk konum
                if (dgvTanimliKartlar.Columns["AktifMi"] != null) dgvTanimliKartlar.Columns["AktifMi"].HeaderText = "Bina Yetkili Mi?";
                if (dgvTanimliKartlar.Columns["ArgeYetkisiVar"] != null) dgvTanimliKartlar.Columns["ArgeYetkisiVar"].HeaderText = "AR-GE Yetkili Mi?"; // yetki 
                if (dgvTanimliKartlar.Columns["IsAdmin"] != null) dgvTanimliKartlar.Columns["IsAdmin"].HeaderText = "Admin Mi?";
            }
            catch (Exception ex) { Debug.WriteLine("Tablo yenileme hatasý: " + ex.Message); }
        }

        private void LogBasliklariniDuzenle()
        {
            if (dgvLoglar.Columns["KartUID"] != null) dgvLoglar.Columns["KartUID"].HeaderText = "Kart UID";
            if (dgvLoglar.Columns["KartSahibi"] != null) dgvLoglar.Columns["KartSahibi"].HeaderText = "Kart Sahibi";
            if (dgvLoglar.Columns["IslemTipi"] != null) dgvLoglar.Columns["IslemTipi"].HeaderText = "Ýþlem";
            if (dgvLoglar.Columns["KapiTipi"] != null) dgvLoglar.Columns["KapiTipi"].HeaderText = "Kapý";
            if (dgvLoglar.Columns["TarihSaat"] != null) dgvLoglar.Columns["TarihSaat"].HeaderText = "Tarih/Saat";
        }
        private void FiltreKutulariniGuncelle()
        {
            dtpBaslangic.Enabled = chkTarih.Checked;
            dtpBitis.Enabled = chkTarih.Checked;
            dtpSaatBaslangic.Enabled = chkSaat.Checked;
            dtpSaatBitis.Enabled = chkSaat.Checked;
        }

        private void FiltreKutulariniSifirla()
        {
            txtFiltreKisi.Clear();
            chkTarih.Checked = false;
            chkSaat.Checked = false;
            dtpBaslangic.Value = DateTime.Today;
            dtpBitis.Value = DateTime.Today;
            dtpSaatBaslangic.Value = DateTime.Today;
            dtpSaatBitis.Value = DateTime.Today.AddHours(23).AddMinutes(59);
            FiltreKutulariniGuncelle();
        }

        private void btnFiltrele_Click(object sender, EventArgs e)
        {
            var filtre = new LogFiltre();
            if (chkTarih.Checked)
            {
                filtre.BaslangicTarih = dtpBaslangic.Value.Date;
                filtre.BitisTarih = dtpBitis.Value.Date;
            }
            if (chkSaat.Checked)
            {
                DateTime b = dtpSaatBaslangic.Value, s = dtpSaatBitis.Value;
                filtre.SaatBaslangic = new TimeSpan(b.Hour, b.Minute, 0);
                filtre.SaatBitis = new TimeSpan(0, s.Hour, s.Minute, 59, 999);
            }
            if (!string.IsNullOrWhiteSpace(txtFiltreKisi.Text)) filtre.KisiAdi = txtFiltreKisi.Text.Trim();

            if (filtre.Bos) { MessageBox.Show("En az bir filtre seçin."); return; }

            try
            {
                var sonuc = _gecisServisi.LoglariFiltrele(filtre);
                _aktifFiltre = filtre;
                dgvLoglar.DataSource = null;
                dgvLoglar.DataSource = sonuc;
                LogBasliklariniDuzenle();
            }
            catch (ArgumentException ex) { MessageBox.Show(ex.Message); }
            catch (Exception ex) { MessageBox.Show("Filtreleme hatasý: " + ex.Message); }
        }

        private void btnFiltreTemizle_Click(object sender, EventArgs e)
        {
            _aktifFiltre = null;
            FiltreKutulariniSifirla();
            EkraniYenile();
        }

        private void SeriKomutGonder(string komut)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try { _serialPort.WriteLine(komut); } catch { }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                _serialPort.Close();
            }
        }

    
    }
}