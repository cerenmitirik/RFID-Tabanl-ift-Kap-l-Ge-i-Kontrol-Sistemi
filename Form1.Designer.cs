namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvIceridekiler = new DataGridView();
            dgvLoglar = new DataGridView();
            label1 = new Label();
            label2 = new Label();
            dtpBaslangic = new DateTimePicker();
            dtpBitis = new DateTimePicker();
            txtFiltreKisi = new TextBox();
            btnFiltrele = new Button();
            btnFiltreTemizle = new Button();
            dgvTanimliKartlar = new DataGridView();
            chkTarih = new CheckBox();
            chkSaat = new CheckBox();
            dtpSaatBaslangic = new DateTimePicker();
            dtpSaatBitis = new DateTimePicker();
            cmbPortlar = new ComboBox();
            textBox1 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvIceridekiler).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvLoglar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTanimliKartlar).BeginInit();
            SuspendLayout();
            // 
            // dgvIceridekiler
            // 
            dgvIceridekiler.AllowUserToAddRows = false;
            dgvIceridekiler.AllowUserToDeleteRows = false;
            dgvIceridekiler.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvIceridekiler.Location = new Point(3, 32);
            dgvIceridekiler.Name = "dgvIceridekiler";
            dgvIceridekiler.ReadOnly = true;
            dgvIceridekiler.RowHeadersWidth = 51;
            dgvIceridekiler.Size = new Size(286, 172);
            dgvIceridekiler.TabIndex = 0;
            // 
            // dgvLoglar
            // 
            dgvLoglar.AllowUserToAddRows = false;
            dgvLoglar.AllowUserToDeleteRows = false;
            dgvLoglar.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLoglar.Location = new Point(314, 32);
            dgvLoglar.Name = "dgvLoglar";
            dgvLoglar.ReadOnly = true;
            dgvLoglar.RowHeadersWidth = 51;
            dgvLoglar.Size = new Size(818, 267);
            dgvLoglar.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 9);
            label1.Name = "label1";
            label1.Size = new Size(148, 20);
            label1.TabIndex = 2;
            label1.Text = "Şu An İçeride Olanlar";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(314, 9);
            label2.Name = "label2";
            label2.Size = new Size(139, 20);
            label2.TabIndex = 3;
            label2.Text = "Canlı Geçiş Kayıtları";
            // 
            // dtpBaslangic
            // 
            dtpBaslangic.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpBaslangic.Format = DateTimePickerFormat.Custom;
            dtpBaslangic.Location = new Point(1313, 29);
            dtpBaslangic.Name = "dtpBaslangic";
            dtpBaslangic.Size = new Size(160, 27);
            dtpBaslangic.TabIndex = 8;
            // 
            // dtpBitis
            // 
            dtpBitis.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpBitis.Format = DateTimePickerFormat.Custom;
            dtpBitis.Location = new Point(1512, 28);
            dtpBitis.Name = "dtpBitis";
            dtpBitis.Size = new Size(165, 27);
            dtpBitis.TabIndex = 9;
            // 
            // txtFiltreKisi
            // 
            txtFiltreKisi.Location = new Point(1245, 118);
            txtFiltreKisi.Name = "txtFiltreKisi";
            txtFiltreKisi.Size = new Size(125, 27);
            txtFiltreKisi.TabIndex = 10;
            txtFiltreKisi.Text = "İsme Göre Filtrele";
            // 
            // btnFiltrele
            // 
            btnFiltrele.Location = new Point(1408, 118);
            btnFiltrele.Name = "btnFiltrele";
            btnFiltrele.Size = new Size(70, 29);
            btnFiltrele.TabIndex = 11;
            btnFiltrele.Text = "Filtrele";
            btnFiltrele.UseVisualStyleBackColor = true;
            btnFiltrele.Click += btnFiltrele_Click;
            // 
            // btnFiltreTemizle
            // 
            btnFiltreTemizle.Location = new Point(1522, 118);
            btnFiltreTemizle.Name = "btnFiltreTemizle";
            btnFiltreTemizle.Size = new Size(68, 29);
            btnFiltreTemizle.TabIndex = 12;
            btnFiltreTemizle.Text = "Sıfırla";
            btnFiltreTemizle.UseVisualStyleBackColor = true;
            btnFiltreTemizle.Click += btnFiltreTemizle_Click;
            // 
            // dgvTanimliKartlar
            // 
            dgvTanimliKartlar.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTanimliKartlar.Location = new Point(12, 325);
            dgvTanimliKartlar.Name = "dgvTanimliKartlar";
            dgvTanimliKartlar.RowHeadersWidth = 51;
            dgvTanimliKartlar.Size = new Size(924, 228);
            dgvTanimliKartlar.TabIndex = 13;
            // 
            // chkTarih
            // 
            chkTarih.AutoSize = true;
            chkTarih.Location = new Point(1182, 32);
            chkTarih.Name = "chkTarih";
            chkTarih.Size = new Size(108, 24);
            chkTarih.TabIndex = 15;
            chkTarih.Text = "Tarih aralığı";
            chkTarih.UseVisualStyleBackColor = true;
            // 
            // chkSaat
            // 
            chkSaat.AutoSize = true;
            chkSaat.Location = new Point(1184, 70);
            chkSaat.Name = "chkSaat";
            chkSaat.Size = new Size(106, 24);
            chkSaat.TabIndex = 16;
            chkSaat.Text = "Saat aralığı";
            chkSaat.UseVisualStyleBackColor = true;
            // 
            // dtpSaatBaslangic
            // 
            dtpSaatBaslangic.Location = new Point(1313, 70);
            dtpSaatBaslangic.Name = "dtpSaatBaslangic";
            dtpSaatBaslangic.Size = new Size(165, 27);
            dtpSaatBaslangic.TabIndex = 17;
            // 
            // dtpSaatBitis
            // 
            dtpSaatBitis.Location = new Point(1512, 70);
            dtpSaatBitis.Name = "dtpSaatBitis";
            dtpSaatBitis.Size = new Size(160, 27);
            dtpSaatBitis.TabIndex = 18;
            // 
            // cmbPortlar
            // 
            cmbPortlar.FormattingEnabled = true;
            cmbPortlar.Location = new Point(1262, 325);
            cmbPortlar.Name = "cmbPortlar";
            cmbPortlar.Size = new Size(151, 28);
            cmbPortlar.TabIndex = 19;
            cmbPortlar.SelectedIndexChanged += cmbPortlar_SelectedIndexChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 292);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 20;
            textBox1.Text = "Tanımlı Kartlar";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1748, 676);
            Controls.Add(textBox1);
            Controls.Add(cmbPortlar);
            Controls.Add(dtpSaatBitis);
            Controls.Add(dtpSaatBaslangic);
            Controls.Add(chkSaat);
            Controls.Add(chkTarih);
            Controls.Add(dgvTanimliKartlar);
            Controls.Add(btnFiltreTemizle);
            Controls.Add(btnFiltrele);
            Controls.Add(txtFiltreKisi);
            Controls.Add(dtpBitis);
            Controls.Add(dtpBaslangic);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dgvLoglar);
            Controls.Add(dgvIceridekiler);
            Name = "Form1";
            Text = "label";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvIceridekiler).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvLoglar).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvTanimliKartlar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvIceridekiler;
        private DataGridView dgvLoglar;
        private Label label1;
        private Label label2;
        private DateTimePicker dtpBaslangic;
        private DateTimePicker dtpBitis;
        private TextBox txtFiltreKisi;
        private Button btnFiltrele;
        private Button btnFiltreTemizle;
        private DataGridView dgvTanimliKartlar;
        private CheckBox chkTarih;
        private CheckBox chkSaat;
        private DateTimePicker dtpSaatBaslangic;
        private DateTimePicker dtpSaatBitis;
        private ComboBox cmbPortlar;
        private TextBox textBox1;
    }
}
