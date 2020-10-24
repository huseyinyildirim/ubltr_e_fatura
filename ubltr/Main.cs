using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ubltr
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmInvoice f= new frmInvoice();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Text = "İbem Entegre (" + Conn.aliasName + ")";
        }

        private void btnEDefter_Click(object sender, EventArgs e)
        {

        }
    }
}
