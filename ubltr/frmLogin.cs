using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using log4net;
using log4net.Config;
using tr.gov.tubitak.uekae.esya.api.common.util;
using System.IO;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.config;

namespace ubltr
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kadi, sifre, db = "";
            kadi = Conn.convert2Latin(txtUserName.Text);
            sifre = Conn.convert2Latin(txtPassword.Text);
            string checkSql="select {0} as deger from yetkimuhasebe where durum='Aktif' and  kullaniciadi='" + kadi +
                            "' and pwd='" + Conn.CalculateMD5Hash(sifre) + "'";
            string sql = string.Format(checkSql,"count(*)");
            Conn.OpenConn();
            int ok = Int32.Parse(Conn.ReadSingleField(sql, "0"));
            if (ok > 0)
            {
                string userId = Conn.ReadSingleField(string.Format(checkSql, "id"), "0");
                sql = string.Format("select cast(accessright as char) from yetkimuhasebe_yetkiler where userid={0} and zone='acEFaturaIslemleri'", userId);
                int yetki = 0;
                try
                {
                    yetki = Int32.Parse(Conn.ReadSingleField(sql, "0"));
                }
                catch (Exception exx)
                {
                    yetki = 0;
                }
                if (yetki>0)
                {
                    frmMain f = new frmMain();
                    this.Hide();
                    Conn.CloseConn();
                    Properties.Settings s = new Properties.Settings();
                    s.MysqlDB = db;
                    f.ShowDialog();
                    txtPassword.Clear();
                    this.Show();
                    txtPassword.Focus();
                }
            }
            else
            {
                Conn.CloseConn();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            //todo-:Connection Settings choose
            //cmbSirket.Items.Clear();
            string sql = "select * from efatura.baglantilar order by alias";

            Conn.OpenConn();
            MySqlDataAdapter daBaglantilar = Conn.OpenAdapter(sql);
            DataTable dtBaglantilar = new DataTable();
            daBaglantilar.Fill(dtBaglantilar);
            foreach (DataRow row in dtBaglantilar.Rows)
            {
                cbConnection.Items.Add(new ComboboxItem(row["alias"].ToString(),
                                                new Baglanti(row["alias"].ToString(),
                                                             row["hostip"].ToString(),
                                                             row["hostport"].ToString(),
                                                             row["hostdb"].ToString(),
                                                             row["SaveXmlPath"].ToString(),
                                                             row["INGEntVar"].ToString()=="True"?true:false,
                                                             row["INGKullanici"].ToString(),
                                                             row["INGSifre"].ToString(),
                                                             row["INGVKN"].ToString(),
                                                             row["INGAlias"].ToString()
                                                             )
                                                       )
                                      );
            }
            if (dtBaglantilar.Rows.Count > 0) BaglantiYukle(0);
            dtBaglantilar = null;
            daBaglantilar = null;
            Conn.CloseConn();
            if (cbConnection.Items.Count > 0) cbConnection.SelectedIndex = 0;
            
            lblVersion.Text = Application.ProductVersion.ToString() ;
            Properties.Settings s = new Properties.Settings();
            if (s.IbemSifre == "18")
            {
                txtUserName.Text = "iadmin";
                txtPassword.Text = "183m8745";
            }
        }

        private void frmLogin_Shown(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter && txtUserName.Text!="") {
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtPassword.Text != "")
            {
                button1.PerformClick();
            }
        }

        private void BaglantiYukle(int ix)
        {
            Conn.hostIP = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).hostIP;
            Conn.hostPort = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).hostPort;
            Conn.hostDB = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).hostDB;
            Conn.SaveXmlPath = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).SaveXmlPath;
            Conn.aliasName = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).aliasName;
            Conn.INGEntVar = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).INGEntVar;
            Conn.INGUserName = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).INGKullanici;
            Conn.INGPassword = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).INGSifre;
            Conn.INGVKN = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).INGVKN;
            Conn.INGAlias = ((cbConnection.Items[ix] as ComboboxItem).Value as Baglanti).INGAlias;
            lblSavePath.Text = Conn.SaveXmlPath;
            lblSavePath.Links.Clear();
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "file://" + lblSavePath.Text.Replace("\\", "/");
            lblSavePath.Links.Add(link);
        }
        private void cbConnection_TextChanged(object sender, EventArgs e)
        {
            int ix = cbConnection.SelectedIndex;
            if (ix >= 0)
                BaglantiYukle(ix);
        }

        private void lblSavePath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

    }
}
