using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ubltr
{
    public partial class frmInvoiceViewer : Form
    {
        public frmInvoiceViewer()
        {
            InitializeComponent();
        }

        public static void ShowInvoice(string xmlPath) 
        {
            frmInvoiceViewer iw = new frmInvoiceViewer();
            iw.txtInvoiceFile.Text = xmlPath;
            iw.Show();
            iw.btnShow.PerformClick();
            
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            string path=txtInvoiceFile.Text;
            if (File.Exists(path))
            {
                webBrowser1.Url = new Uri(String.Format("file:///{0}", path));
                //webBrowser1.Navigate(path);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }
    }
}
