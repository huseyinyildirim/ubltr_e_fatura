namespace ubltr
{
    partial class frmTester
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
            this.btnSign = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtConfigXmlPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLicensePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPinNumber = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbNiteliksiz = new System.Windows.Forms.RadioButton();
            this.rbNitelikli = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSignXML = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtOutputXML = new System.Windows.Forms.TextBox();
            this.btnLoadSignXML = new System.Windows.Forms.Button();
            this.btnSetSignXML = new System.Windows.Forms.Button();
            this.btnSetConfigPath = new System.Windows.Forms.Button();
            this.btnSetLicencePath = new System.Windows.Forms.Button();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveFile = new System.Windows.Forms.SaveFileDialog();
            this.dlgChooseFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.cbInTest = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSign
            // 
            this.btnSign.Location = new System.Drawing.Point(171, 184);
            this.btnSign.Name = "btnSign";
            this.btnSign.Size = new System.Drawing.Size(75, 23);
            this.btnSign.TabIndex = 0;
            this.btnSign.Text = "İmzala";
            this.btnSign.UseVisualStyleBackColor = true;
            this.btnSign.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "smlsignature-config.xml yolu";
            // 
            // txtConfigXmlPath
            // 
            this.txtConfigXmlPath.Location = new System.Drawing.Point(155, 64);
            this.txtConfigXmlPath.Name = "txtConfigXmlPath";
            this.txtConfigXmlPath.Size = new System.Drawing.Size(376, 20);
            this.txtConfigXmlPath.TabIndex = 2;
            this.txtConfigXmlPath.Text = "c:\\users\\test\\";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "lisans dosya yolu";
            // 
            // txtLicensePath
            // 
            this.txtLicensePath.Location = new System.Drawing.Point(155, 90);
            this.txtLicensePath.Name = "txtLicensePath";
            this.txtLicensePath.Size = new System.Drawing.Size(376, 20);
            this.txtLicensePath.TabIndex = 4;
            this.txtLicensePath.Text = "c:\\users\\test\\lisans\\BES_lisans.xml";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "kartın pin numarası";
            // 
            // txtPinNumber
            // 
            this.txtPinNumber.Location = new System.Drawing.Point(155, 120);
            this.txtPinNumber.Name = "txtPinNumber";
            this.txtPinNumber.Size = new System.Drawing.Size(376, 20);
            this.txtPinNumber.TabIndex = 6;
            this.txtPinNumber.Text = "2626";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbNiteliksiz);
            this.panel1.Controls.Add(this.rbNitelikli);
            this.panel1.Location = new System.Drawing.Point(15, 142);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 36);
            this.panel1.TabIndex = 7;
            // 
            // rbNiteliksiz
            // 
            this.rbNiteliksiz.AutoSize = true;
            this.rbNiteliksiz.Location = new System.Drawing.Point(140, 12);
            this.rbNiteliksiz.Name = "rbNiteliksiz";
            this.rbNiteliksiz.Size = new System.Drawing.Size(91, 17);
            this.rbNiteliksiz.TabIndex = 1;
            this.rbNiteliksiz.Text = "Niteliksiz İmza";
            this.rbNiteliksiz.UseVisualStyleBackColor = true;
            // 
            // rbNitelikli
            // 
            this.rbNitelikli.AutoSize = true;
            this.rbNitelikli.Checked = true;
            this.rbNitelikli.Location = new System.Drawing.Point(7, 12);
            this.rbNitelikli.Name = "rbNitelikli";
            this.rbNitelikli.Size = new System.Drawing.Size(83, 17);
            this.rbNitelikli.TabIndex = 0;
            this.rbNitelikli.TabStop = true;
            this.rbNitelikli.Text = "Nitelikli İmza";
            this.rbNitelikli.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "İmzalanacak XML";
            // 
            // txtSignXML
            // 
            this.txtSignXML.Location = new System.Drawing.Point(155, 12);
            this.txtSignXML.Name = "txtSignXML";
            this.txtSignXML.Size = new System.Drawing.Size(376, 20);
            this.txtSignXML.TabIndex = 9;
            this.txtSignXML.Text = "D:\\desktop\\gdrive\\ibem\\efatura\\ibemtest.xml";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Çıktı XML";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // txtOutputXML
            // 
            this.txtOutputXML.Location = new System.Drawing.Point(155, 38);
            this.txtOutputXML.Name = "txtOutputXML";
            this.txtOutputXML.Size = new System.Drawing.Size(376, 20);
            this.txtOutputXML.TabIndex = 11;
            this.txtOutputXML.Text = "D:\\desktop\\gdrive\\ibem\\efatura\\1.xml";
            // 
            // btnLoadSignXML
            // 
            this.btnLoadSignXML.Location = new System.Drawing.Point(537, 9);
            this.btnLoadSignXML.Name = "btnLoadSignXML";
            this.btnLoadSignXML.Size = new System.Drawing.Size(25, 23);
            this.btnLoadSignXML.TabIndex = 12;
            this.btnLoadSignXML.Text = "...";
            this.btnLoadSignXML.UseVisualStyleBackColor = true;
            this.btnLoadSignXML.Click += new System.EventHandler(this.btnLoadSignXML_Click);
            // 
            // btnSetSignXML
            // 
            this.btnSetSignXML.Location = new System.Drawing.Point(537, 35);
            this.btnSetSignXML.Name = "btnSetSignXML";
            this.btnSetSignXML.Size = new System.Drawing.Size(25, 23);
            this.btnSetSignXML.TabIndex = 13;
            this.btnSetSignXML.Text = "...";
            this.btnSetSignXML.UseVisualStyleBackColor = true;
            this.btnSetSignXML.Click += new System.EventHandler(this.btnSetSignXML_Click);
            // 
            // btnSetConfigPath
            // 
            this.btnSetConfigPath.Location = new System.Drawing.Point(537, 61);
            this.btnSetConfigPath.Name = "btnSetConfigPath";
            this.btnSetConfigPath.Size = new System.Drawing.Size(25, 23);
            this.btnSetConfigPath.TabIndex = 14;
            this.btnSetConfigPath.Text = "...";
            this.btnSetConfigPath.UseVisualStyleBackColor = true;
            this.btnSetConfigPath.Click += new System.EventHandler(this.btnSetConfigPath_Click);
            // 
            // btnSetLicencePath
            // 
            this.btnSetLicencePath.Location = new System.Drawing.Point(537, 87);
            this.btnSetLicencePath.Name = "btnSetLicencePath";
            this.btnSetLicencePath.Size = new System.Drawing.Size(25, 23);
            this.btnSetLicencePath.TabIndex = 15;
            this.btnSetLicencePath.Text = "...";
            this.btnSetLicencePath.UseVisualStyleBackColor = true;
            this.btnSetLicencePath.Click += new System.EventHandler(this.btnSetLicencePath_Click);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.FileName = "openFileDialog1";
            // 
            // cbInTest
            // 
            this.cbInTest.AutoSize = true;
            this.cbInTest.Checked = true;
            this.cbInTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbInTest.Location = new System.Drawing.Point(425, 146);
            this.cbInTest.Name = "cbInTest";
            this.cbInTest.Size = new System.Drawing.Size(59, 17);
            this.cbInTest.TabIndex = 16;
            this.cbInTest.Text = "In Test";
            this.cbInTest.UseVisualStyleBackColor = true;
            // 
            // frmTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 213);
            this.Controls.Add(this.cbInTest);
            this.Controls.Add(this.btnSetLicencePath);
            this.Controls.Add(this.btnSetConfigPath);
            this.Controls.Add(this.btnSetSignXML);
            this.Controls.Add(this.btnLoadSignXML);
            this.Controls.Add(this.txtOutputXML);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtSignXML);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtPinNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLicensePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtConfigXmlPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSign);
            this.Name = "frmTester";
            this.Text = "Xades Sign Test";
            this.Load += new System.EventHandler(this.frmTester_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSign;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtConfigXmlPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLicensePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPinNumber;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbNiteliksiz;
        private System.Windows.Forms.RadioButton rbNitelikli;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSignXML;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOutputXML;
        private System.Windows.Forms.Button btnLoadSignXML;
        private System.Windows.Forms.Button btnSetSignXML;
        private System.Windows.Forms.Button btnSetConfigPath;
        private System.Windows.Forms.Button btnSetLicencePath;
        private System.Windows.Forms.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.FolderBrowserDialog dlgChooseFolder;
        private System.Windows.Forms.CheckBox cbInTest;
    }
}

