namespace WinFormsApp1
{
    partial class AdminForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblAdminDurum = new GroupBox();
            cmbKlonlanacaklar = new ComboBox();
            btnKartiKlonla = new Button();
            lblKlonDurum = new Label();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            lblSeciliKisi = new Label();
            btnYetkiUygula = new Button();
            chkArgeYetkisi = new CheckBox();
            chkBinaYetkisi = new CheckBox();
            label3 = new Label();
            txtKartSahibi = new TextBox();
            btnKaydet = new Button();
            lblAdminDurum.SuspendLayout();
            SuspendLayout();
            // 
            // lblAdminDurum
            // 
            lblAdminDurum.Controls.Add(cmbKlonlanacaklar);
            lblAdminDurum.Controls.Add(btnKartiKlonla);
            lblAdminDurum.Controls.Add(lblKlonDurum);
            lblAdminDurum.Controls.Add(textBox2);
            lblAdminDurum.Controls.Add(textBox1);
            lblAdminDurum.Controls.Add(lblSeciliKisi);
            lblAdminDurum.Controls.Add(btnYetkiUygula);
            lblAdminDurum.Controls.Add(chkArgeYetkisi);
            lblAdminDurum.Controls.Add(chkBinaYetkisi);
            lblAdminDurum.Controls.Add(label3);
            lblAdminDurum.Controls.Add(txtKartSahibi);
            lblAdminDurum.Controls.Add(btnKaydet);
            lblAdminDurum.Location = new Point(124, 97);
            lblAdminDurum.Name = "lblAdminDurum";
            lblAdminDurum.Size = new Size(980, 341);
            lblAdminDurum.TabIndex = 20;
            lblAdminDurum.TabStop = false;
            lblAdminDurum.Text = "groupBox1";
            // 
            // cmbKlonlanacaklar
            // 
            cmbKlonlanacaklar.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKlonlanacaklar.FormattingEnabled = true;
            cmbKlonlanacaklar.Location = new Point(152, 224);
            cmbKlonlanacaklar.Name = "cmbKlonlanacaklar";
            cmbKlonlanacaklar.Size = new Size(151, 28);
            cmbKlonlanacaklar.TabIndex = 28;
            // 
            // btnKartiKlonla
            // 
            btnKartiKlonla.Location = new Point(323, 224);
            btnKartiKlonla.Name = "btnKartiKlonla";
            btnKartiKlonla.Size = new Size(94, 29);
            btnKartiKlonla.TabIndex = 24;
            btnKartiKlonla.Text = "Kopyala";
            btnKartiKlonla.UseVisualStyleBackColor = true;
            btnKartiKlonla.Click += btnKartiKlonla_Click;
            // 
            // lblKlonDurum
            // 
            lblKlonDurum.AutoSize = true;
            lblKlonDurum.Location = new Point(6, 274);
            lblKlonDurum.Name = "lblKlonDurum";
            lblKlonDurum.Size = new Size(50, 20);
            lblKlonDurum.TabIndex = 25;
            lblKlonDurum.Text = "Bilgi : ";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(6, 224);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(125, 27);
            textBox2.TabIndex = 27;
            textBox2.Text = "Klonlanacak Kişi";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 113);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 26;
            textBox1.Text = "Hedef Kişi ";
            // 
            // lblSeciliKisi
            // 
            lblSeciliKisi.AutoSize = true;
            lblSeciliKisi.Location = new Point(147, 113);
            lblSeciliKisi.Name = "lblSeciliKisi";
            lblSeciliKisi.Size = new Size(50, 20);
            lblSeciliKisi.TabIndex = 20;
            lblSeciliKisi.Text = "label5";
            // 
            // btnYetkiUygula
            // 
            btnYetkiUygula.Location = new Point(323, 162);
            btnYetkiUygula.Name = "btnYetkiUygula";
            btnYetkiUygula.Size = new Size(139, 29);
            btnYetkiUygula.TabIndex = 23;
            btnYetkiUygula.Text = "Yetkileri Güncelle";
            btnYetkiUygula.UseVisualStyleBackColor = true;
            btnYetkiUygula.Click += btnYetkiUygula_Click;
            // 
            // chkArgeYetkisi
            // 
            chkArgeYetkisi.AutoSize = true;
            chkArgeYetkisi.Location = new Point(147, 167);
            chkArgeYetkisi.Name = "chkArgeYetkisi";
            chkArgeYetkisi.Size = new Size(108, 24);
            chkArgeYetkisi.TabIndex = 22;
            chkArgeYetkisi.Text = "Arge Yetkisi";
            chkArgeYetkisi.UseVisualStyleBackColor = true;
            // 
            // chkBinaYetkisi
            // 
            chkBinaYetkisi.AutoSize = true;
            chkBinaYetkisi.Location = new Point(6, 167);
            chkBinaYetkisi.Name = "chkBinaYetkisi";
            chkBinaYetkisi.Size = new Size(105, 24);
            chkBinaYetkisi.TabIndex = 21;
            chkBinaYetkisi.Text = "Bina Yetkisi";
            chkBinaYetkisi.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 70);
            label3.Name = "label3";
            label3.Size = new Size(81, 20);
            label3.TabIndex = 7;
            label3.Text = "Kart Sahibi";
            // 
            // txtKartSahibi
            // 
            txtKartSahibi.Location = new Point(147, 69);
            txtKartSahibi.Name = "txtKartSahibi";
            txtKartSahibi.Size = new Size(156, 27);
            txtKartSahibi.TabIndex = 4;
            // 
            // btnKaydet
            // 
            btnKaydet.Location = new Point(323, 68);
            btnKaydet.Name = "btnKaydet";
            btnKaydet.Size = new Size(151, 29);
            btnKaydet.TabIndex = 5;
            btnKaydet.Text = "Yeni Kartı Kaydet";
            btnKaydet.UseVisualStyleBackColor = true;
            btnKaydet.Click += btnKaydet_Click;
            // 
            // AdminForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1116, 450);
            Controls.Add(lblAdminDurum);
            Name = "AdminForm";
            Text = "AdminForm";
            Load += AdminForm_Load;
            lblAdminDurum.ResumeLayout(false);
            lblAdminDurum.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox lblAdminDurum;
        private Button btnKartiKlonla;
        private Label lblKlonDurum;
        private TextBox textBox2;
        private TextBox textBox1;
        private Label lblSeciliKisi;
        private Button btnYetkiUygula;
        private CheckBox chkArgeYetkisi;
        private CheckBox chkBinaYetkisi;
        private Label label3;
        private TextBox txtKartSahibi;
        private Button btnKaydet;
        private ComboBox cmbKlonlanacaklar;
    }
}