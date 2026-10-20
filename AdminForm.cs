using System;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1.Repositories;
using WinFormsApp1.Services;

namespace WinFormsApp1
{
    public partial class AdminForm : Form
    {
        private readonly GecisKontrolServisi _servis;
        private readonly IKartRepository _repo;
        private readonly Action<string> _seriGonder;
        private string _seciliUid = "";

        public AdminForm(GecisKontrolServisi servis, IKartRepository repo, Action<string> seriGonder)
        {
            InitializeComponent();
            _servis = servis;
            _repo = repo;
            _seriGonder = seriGonder;
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            KlonListesiniDoldur();

            // Açılışta adminin kendi bilgilerini yükle
            var adminKart = _repo.AdminKartiGetir();
            if (adminKart != null)
            {
                _seciliUid = adminKart.KartUID;
                lblSeciliKisi.Text = $"{adminKart.KartSahibi} (Admin - {adminKart.KartUID})";
                txtKartSahibi.Text = adminKart.KartSahibi;
                chkBinaYetkisi.Checked = adminKart.AktifMi;
                chkArgeYetkisi.Checked = adminKart.ArgeYetkisiVar;
            }
        }

        // Listeye sadece admin OLMAYAN normal kartları dolduruyoruz
        public void KlonListesiniDoldur()
        {
            cmbKlonlanacaklar.Items.Clear();
            var tumKartlar = _repo.TumTanimliKartlariGetir()
                                  .Where(k => !k.IsAdmin) // Admin kartının kopyalanmasını engeller
                                  .ToList();

            foreach (var kart in tumKartlar)
            {
                cmbKlonlanacaklar.Items.Add($"{kart.KartSahibi} ({kart.KartUID})");
            }

            if (cmbKlonlanacaklar.Items.Count > 0)
                cmbKlonlanacaklar.SelectedIndex = 0;
        }

        public void HedefKartSecildi(string uid, string kisiAdi)
        {
            _seciliUid = uid;
            lblSeciliKisi.Text = $"{kisiAdi} ({uid})";

            var kart = _repo.KartGetir(uid);
            if (kart != null)
            {
                txtKartSahibi.Text = kart.KartSahibi;
                chkBinaYetkisi.Checked = kart.AktifMi;
                chkArgeYetkisi.Checked = kart.ArgeYetkisiVar;

                btnYetkiUygula.Enabled = true; 
                chkBinaYetkisi.Enabled = true;
                chkArgeYetkisi.Enabled = true;
            }
            else
            {
                txtKartSahibi.Clear();
                txtKartSahibi.Focus();
                chkBinaYetkisi.Checked = true;
                chkArgeYetkisi.Checked = false;

                btnYetkiUygula.Enabled = false; // TANIMSIZ KARTA YETKİ VERİLEMEZ!
                chkBinaYetkisi.Enabled = false;
                chkArgeYetkisi.Enabled = false;

                lblKlonDurum.Text = "Yeni kart algılandı. Önce 'Yeni Kartı Kaydet' ile sisteme ekleyin.";
            }
        }

        // 1. Yeni Kart Kaydet Butonu
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_seciliUid))
            {
                MessageBox.Show("Önce bir kart okutun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtKartSahibi.Text))
            {
                MessageBox.Show("Lütfen kişi adı girin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKartSahibi.Focus();
                return;
            }

            var mevcutKart = _repo.KartGetir(_seciliUid);
            if (mevcutKart != null)
            {
                var onay = MessageBox.Show(
                    $"Bu kart zaten '{mevcutKart.KartSahibi}' adına kayıtlı!\nİsmi '{txtKartSahibi.Text.Trim()}' olarak güncellemek istiyor musunuz?",
                    "Mevcut Kartı Güncelle",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (onay == DialogResult.No) return;
            }

            try
            {
                _servis.KartiYetkilendir(_seciliUid, txtKartSahibi.Text.Trim());
                MessageBox.Show($"[{_seciliUid}] başarıyla kaydedildi.");
                txtKartSahibi.Clear();
                KlonListesiniDoldur(); // Yeni kart listeye de eklensin
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayıt hatası: " + ex.Message);
            }
        }

        // 2. Yetkileri Güncelle Butonu
        private void btnYetkiUygula_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_seciliUid))
            {
                MessageBox.Show("Önce bir kart okutun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sonuc = _servis.YetkiUygula(_seciliUid, chkBinaYetkisi.Checked, chkArgeYetkisi.Checked);
            MessageBox.Show(sonuc);
        }

        // 3. Kopyala Butonu (Klonlama Başlatma)
        private void btnKartiKlonla_Click(object sender, EventArgs e)
        {
            if (cmbKlonlanacaklar.SelectedItem == null)
            {
                MessageBox.Show("Lütfen kopyalanacak kişiyi seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string secilenMetin = cmbKlonlanacaklar.SelectedItem.ToString();
            int basla = secilenMetin.IndexOf('('), bitir = secilenMetin.IndexOf(')');
            if (basla == -1 || bitir == -1) return;

            string kaynakUid = secilenMetin.Substring(basla + 1, bitir - basla - 1);

            string sonuc = _servis.KlonBaslat(kaynakUid);
            lblKlonDurum.Text = sonuc;
        }

        // Form1'den mesaj iletmek için metot:
        public void KlonSonucunuGoster(string mesaj)
        {
            lblKlonDurum.Text = mesaj;
        }

        // 4. Çarpıdan kapatılınca ESP32'yi ve Servisi iptal et!
        private void AdminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _servis.KlonIptalEt();
            _seriGonder("CMD:CANCEL"); // ESP32 yazma modundan çıksın
        }
    }
}