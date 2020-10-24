namespace ubltr
{
    partial class frmInvoice
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInvoice));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtPinKodu = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnZarfOlustur = new System.Windows.Forms.Button();
            this.btnIngKayitliKullaniciListesi = new System.Windows.Forms.Button();
            this.pgImzalama = new System.Windows.Forms.ProgressBar();
            this.btnPrint = new System.Windows.Forms.Button();
            this.txtImzalananSql = new System.Windows.Forms.TextBox();
            this.btnListSigned = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.txtImzalanacakSql = new System.Windows.Forms.TextBox();
            this.btnSignSelected = new System.Windows.Forms.Button();
            this.dpBitTarih = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dpBasTarih = new System.Windows.Forms.DateTimePicker();
            this.btnListUnSigned = new System.Windows.Forms.Button();
            this.dgView = new System.Windows.Forms.DataGridView();
            this.pmImzalanacaklar = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemFaturanoDegistir = new System.Windows.Forms.ToolStripMenuItem();
            this.pmImzalanlar = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.faturaDurumuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemGonderildi = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemKabulEdildi = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemRedEdildi = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemImzalamayiGeriAl = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemFaturaYazdir = new System.Windows.Forms.ToolStripMenuItem();
            this.miINGGonder = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCariUPD = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            this.pmImzalanacaklar.SuspendLayout();
            this.pmImzalanlar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCariUPD);
            this.panel1.Controls.Add(this.txtPinKodu);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnZarfOlustur);
            this.panel1.Controls.Add(this.btnIngKayitliKullaniciListesi);
            this.panel1.Controls.Add(this.pgImzalama);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.txtImzalananSql);
            this.panel1.Controls.Add(this.btnListSigned);
            this.panel1.Controls.Add(this.btnSelectAll);
            this.panel1.Controls.Add(this.txtImzalanacakSql);
            this.panel1.Controls.Add(this.btnSignSelected);
            this.panel1.Controls.Add(this.dpBitTarih);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dpBasTarih);
            this.panel1.Controls.Add(this.btnListUnSigned);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(752, 81);
            this.panel1.TabIndex = 1;
            // 
            // txtPinKodu
            // 
            this.txtPinKodu.Location = new System.Drawing.Point(615, 6);
            this.txtPinKodu.Name = "txtPinKodu";
            this.txtPinKodu.PasswordChar = '*';
            this.txtPinKodu.Size = new System.Drawing.Size(100, 20);
            this.txtPinKodu.TabIndex = 15;
            this.txtPinKodu.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(553, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Pin Kodu :";
            // 
            // btnZarfOlustur
            // 
            this.btnZarfOlustur.Location = new System.Drawing.Point(391, 33);
            this.btnZarfOlustur.Name = "btnZarfOlustur";
            this.btnZarfOlustur.Size = new System.Drawing.Size(75, 23);
            this.btnZarfOlustur.TabIndex = 13;
            this.btnZarfOlustur.Text = "Zarf Oluştur";
            this.btnZarfOlustur.UseVisualStyleBackColor = true;
            this.btnZarfOlustur.Click += new System.EventHandler(this.btnZarfOlustur_Click);
            // 
            // btnIngKayitliKullaniciListesi
            // 
            this.btnIngKayitliKullaniciListesi.Location = new System.Drawing.Point(391, 4);
            this.btnIngKayitliKullaniciListesi.Name = "btnIngKayitliKullaniciListesi";
            this.btnIngKayitliKullaniciListesi.Size = new System.Drawing.Size(156, 23);
            this.btnIngKayitliKullaniciListesi.TabIndex = 12;
            this.btnIngKayitliKullaniciListesi.Text = "İNG Kayıtlı Kullanıcı Listesi";
            this.btnIngKayitliKullaniciListesi.UseVisualStyleBackColor = true;
            this.btnIngKayitliKullaniciListesi.Click += new System.EventHandler(this.btnIngKayitliKullaniciListesi_Click);
            // 
            // pgImzalama
            // 
            this.pgImzalama.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pgImzalama.Location = new System.Drawing.Point(0, 58);
            this.pgImzalama.Name = "pgImzalama";
            this.pgImzalama.Size = new System.Drawing.Size(752, 23);
            this.pgImzalama.TabIndex = 11;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(584, 33);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 10;
            this.btnPrint.Text = "Yazdır";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // txtImzalananSql
            // 
            this.txtImzalananSql.Location = new System.Drawing.Point(649, 31);
            this.txtImzalananSql.Multiline = true;
            this.txtImzalananSql.Name = "txtImzalananSql";
            this.txtImzalananSql.Size = new System.Drawing.Size(100, 20);
            this.txtImzalananSql.TabIndex = 9;
            this.txtImzalananSql.Text = resources.GetString("txtImzalananSql.Text");
            this.txtImzalananSql.Visible = false;
            // 
            // btnListSigned
            // 
            this.btnListSigned.Location = new System.Drawing.Point(288, 4);
            this.btnListSigned.Name = "btnListSigned";
            this.btnListSigned.Size = new System.Drawing.Size(97, 23);
            this.btnListSigned.TabIndex = 8;
            this.btnListSigned.Text = "İmzalananlar";
            this.btnListSigned.UseVisualStyleBackColor = true;
            this.btnListSigned.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(185, 33);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(97, 23);
            this.btnSelectAll.TabIndex = 7;
            this.btnSelectAll.Text = "Tümünü Seç";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // txtImzalanacakSql
            // 
            this.txtImzalanacakSql.Location = new System.Drawing.Point(649, 6);
            this.txtImzalanacakSql.Multiline = true;
            this.txtImzalanacakSql.Name = "txtImzalanacakSql";
            this.txtImzalanacakSql.Size = new System.Drawing.Size(100, 20);
            this.txtImzalanacakSql.TabIndex = 3;
            this.txtImzalanacakSql.Text = resources.GetString("txtImzalanacakSql.Text");
            this.txtImzalanacakSql.Visible = false;
            // 
            // btnSignSelected
            // 
            this.btnSignSelected.Location = new System.Drawing.Point(288, 33);
            this.btnSignSelected.Name = "btnSignSelected";
            this.btnSignSelected.Size = new System.Drawing.Size(97, 23);
            this.btnSignSelected.TabIndex = 6;
            this.btnSignSelected.Text = "İmzala";
            this.btnSignSelected.UseVisualStyleBackColor = true;
            this.btnSignSelected.Click += new System.EventHandler(this.button2_Click);
            // 
            // dpBitTarih
            // 
            this.dpBitTarih.Checked = false;
            this.dpBitTarih.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dpBitTarih.Location = new System.Drawing.Point(70, 29);
            this.dpBitTarih.Name = "dpBitTarih";
            this.dpBitTarih.Size = new System.Drawing.Size(93, 20);
            this.dpBitTarih.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Bit. Tarih :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Baş. Tarih :";
            // 
            // dpBasTarih
            // 
            this.dpBasTarih.Checked = false;
            this.dpBasTarih.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dpBasTarih.Location = new System.Drawing.Point(70, 3);
            this.dpBasTarih.Name = "dpBasTarih";
            this.dpBasTarih.Size = new System.Drawing.Size(93, 20);
            this.dpBasTarih.TabIndex = 2;
            // 
            // btnListUnSigned
            // 
            this.btnListUnSigned.Location = new System.Drawing.Point(185, 4);
            this.btnListUnSigned.Name = "btnListUnSigned";
            this.btnListUnSigned.Size = new System.Drawing.Size(97, 23);
            this.btnListUnSigned.TabIndex = 1;
            this.btnListUnSigned.Text = "İmzalanacaklar";
            this.btnListUnSigned.UseVisualStyleBackColor = true;
            this.btnListUnSigned.Click += new System.EventHandler(this.button1_Click);
            // 
            // dgView
            // 
            this.dgView.AllowUserToAddRows = false;
            this.dgView.AllowUserToDeleteRows = false;
            this.dgView.AllowUserToOrderColumns = true;
            this.dgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgView.Location = new System.Drawing.Point(0, 81);
            this.dgView.Name = "dgView";
            this.dgView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.dgView.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.dgView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgView.Size = new System.Drawing.Size(752, 321);
            this.dgView.TabIndex = 2;
            this.dgView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellContentClick);
            // 
            // pmImzalanacaklar
            // 
            this.pmImzalanacaklar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemFaturanoDegistir});
            this.pmImzalanacaklar.Name = "pmImzalanacaklar";
            this.pmImzalanacaklar.Size = new System.Drawing.Size(170, 26);
            // 
            // MenuItemFaturanoDegistir
            // 
            this.MenuItemFaturanoDegistir.Name = "MenuItemFaturanoDegistir";
            this.MenuItemFaturanoDegistir.Size = new System.Drawing.Size(169, 22);
            this.MenuItemFaturanoDegistir.Text = "Fatura No Değiştir";
            this.MenuItemFaturanoDegistir.Click += new System.EventHandler(this.faturaNoDeğiştirToolStripMenuItem_Click);
            // 
            // pmImzalanlar
            // 
            this.pmImzalanlar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.faturaDurumuToolStripMenuItem,
            this.MenuItemImzalamayiGeriAl,
            this.MenuItemFaturaYazdir,
            this.miINGGonder});
            this.pmImzalanlar.Name = "pmImzalanlar";
            this.pmImzalanlar.Size = new System.Drawing.Size(173, 92);
            this.pmImzalanlar.Opening += new System.ComponentModel.CancelEventHandler(this.pmImzalanlar_Opening);
            // 
            // faturaDurumuToolStripMenuItem
            // 
            this.faturaDurumuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemGonderildi,
            this.MenuItemKabulEdildi,
            this.MenuItemRedEdildi});
            this.faturaDurumuToolStripMenuItem.Name = "faturaDurumuToolStripMenuItem";
            this.faturaDurumuToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.faturaDurumuToolStripMenuItem.Text = "Fatura Durumu";
            // 
            // MenuItemGonderildi
            // 
            this.MenuItemGonderildi.Name = "MenuItemGonderildi";
            this.MenuItemGonderildi.Size = new System.Drawing.Size(136, 22);
            this.MenuItemGonderildi.Text = "Gönderildi";
            this.MenuItemGonderildi.Click += new System.EventHandler(this.gönderildiToolStripMenuItem_Click);
            // 
            // MenuItemKabulEdildi
            // 
            this.MenuItemKabulEdildi.Name = "MenuItemKabulEdildi";
            this.MenuItemKabulEdildi.Size = new System.Drawing.Size(136, 22);
            this.MenuItemKabulEdildi.Text = "Kabul Edildi";
            this.MenuItemKabulEdildi.Click += new System.EventHandler(this.kabulEdildiToolStripMenuItem_Click);
            // 
            // MenuItemRedEdildi
            // 
            this.MenuItemRedEdildi.Name = "MenuItemRedEdildi";
            this.MenuItemRedEdildi.Size = new System.Drawing.Size(136, 22);
            this.MenuItemRedEdildi.Text = "Red Edildi";
            this.MenuItemRedEdildi.Click += new System.EventHandler(this.redEdildiToolStripMenuItem_Click);
            // 
            // MenuItemImzalamayiGeriAl
            // 
            this.MenuItemImzalamayiGeriAl.Name = "MenuItemImzalamayiGeriAl";
            this.MenuItemImzalamayiGeriAl.Size = new System.Drawing.Size(172, 22);
            this.MenuItemImzalamayiGeriAl.Text = "İmzalamayı Geri Al";
            this.MenuItemImzalamayiGeriAl.Click += new System.EventHandler(this.imzalamayıGeriAlToolStripMenuItem_Click);
            // 
            // MenuItemFaturaYazdir
            // 
            this.MenuItemFaturaYazdir.Name = "MenuItemFaturaYazdir";
            this.MenuItemFaturaYazdir.Size = new System.Drawing.Size(172, 22);
            this.MenuItemFaturaYazdir.Text = "Yazdır";
            this.MenuItemFaturaYazdir.Click += new System.EventHandler(this.yazdırToolStripMenuItem_Click);
            // 
            // miINGGonder
            // 
            this.miINGGonder.Name = "miINGGonder";
            this.miINGGonder.Size = new System.Drawing.Size(172, 22);
            this.miINGGonder.Text = "ING Gonder";
            this.miINGGonder.Click += new System.EventHandler(this.miINGGonder_Click);
            // 
            // btnCariUPD
            // 
            this.btnCariUPD.Location = new System.Drawing.Point(472, 33);
            this.btnCariUPD.Name = "btnCariUPD";
            this.btnCariUPD.Size = new System.Drawing.Size(75, 23);
            this.btnCariUPD.TabIndex = 16;
            this.btnCariUPD.Text = "Cari UPD";
            this.btnCariUPD.UseVisualStyleBackColor = true;
            this.btnCariUPD.Click += new System.EventHandler(this.btnCariUPD_Click);
            // 
            // frmInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 402);
            this.Controls.Add(this.dgView);
            this.Controls.Add(this.panel1);
            this.Name = "frmInvoice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fatura İmzalama İşlemleri";
            this.Load += new System.EventHandler(this.frmInvoice_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).EndInit();
            this.pmImzalanacaklar.ResumeLayout(false);
            this.pmImzalanlar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnListUnSigned;
        private System.Windows.Forms.DateTimePicker dpBitTarih;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dpBasTarih;
        private System.Windows.Forms.Button btnSignSelected;
        private System.Windows.Forms.DataGridView dgView;
        private System.Windows.Forms.TextBox txtImzalanacakSql;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnListSigned;
        private System.Windows.Forms.TextBox txtImzalananSql;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.ContextMenuStrip pmImzalanacaklar;
        private System.Windows.Forms.ContextMenuStrip pmImzalanlar;
        private System.Windows.Forms.ToolStripMenuItem MenuItemFaturanoDegistir;
        private System.Windows.Forms.ToolStripMenuItem faturaDurumuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuItemGonderildi;
        private System.Windows.Forms.ToolStripMenuItem MenuItemKabulEdildi;
        private System.Windows.Forms.ToolStripMenuItem MenuItemRedEdildi;
        private System.Windows.Forms.ToolStripMenuItem MenuItemImzalamayiGeriAl;
        private System.Windows.Forms.ToolStripMenuItem MenuItemFaturaYazdir;
        private System.Windows.Forms.ProgressBar pgImzalama;
        private System.Windows.Forms.ToolStripMenuItem miINGGonder;
        private System.Windows.Forms.Button btnIngKayitliKullaniciListesi;
        private System.Windows.Forms.Button btnZarfOlustur;
        private System.Windows.Forms.TextBox txtPinKodu;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCariUPD;
    }
}

