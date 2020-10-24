namespace ubltr
{
    partial class frmMain
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
            this.btnEFatura = new System.Windows.Forms.Button();
            this.btnEDefter = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEFatura
            // 
            this.btnEFatura.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEFatura.Location = new System.Drawing.Point(105, 106);
            this.btnEFatura.Name = "btnEFatura";
            this.btnEFatura.Size = new System.Drawing.Size(150, 57);
            this.btnEFatura.TabIndex = 0;
            this.btnEFatura.Text = "E-Fatura İşlemleri";
            this.btnEFatura.UseVisualStyleBackColor = true;
            this.btnEFatura.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnEDefter
            // 
            this.btnEDefter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEDefter.Location = new System.Drawing.Point(105, 169);
            this.btnEDefter.Name = "btnEDefter";
            this.btnEDefter.Size = new System.Drawing.Size(150, 55);
            this.btnEDefter.TabIndex = 1;
            this.btnEDefter.Text = "E-Defter İşlemleri";
            this.btnEDefter.UseVisualStyleBackColor = true;
            this.btnEDefter.Click += new System.EventHandler(this.btnEDefter_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ubltr.Properties.Resources.ibem_logo_small;
            this.pictureBox1.Location = new System.Drawing.Point(105, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 81);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 236);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnEDefter);
            this.Controls.Add(this.btnEFatura);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "İbem Entegre";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEFatura;
        private System.Windows.Forms.Button btnEDefter;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}