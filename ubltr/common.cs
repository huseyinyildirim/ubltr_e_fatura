using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using tr.gov.tubitak.uekae.esya.api.xmlsignature.resolver;
using tr.gov.tubitak.uekae.esya.api.common.util;
using log4net;
using MD5 = System.Security.Cryptography.MD5;
using System.Diagnostics;

namespace ubltr
{

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
        public ComboboxItem(string _Text, object _Value) {
            Text = _Text;
            Value=_Value;
        }
    }

    public class Baglanti
    {
        public string aliasName { get; set; }
        public string hostIP { get; set; }
        public string hostPort { get; set; }
        public string hostDB { get; set; }
        public string SaveXmlPath { get; set; }
        public bool INGEntVar { get; set; }
        public string INGKullanici { get; set; }
        public string INGSifre { get; set; }
        public string INGVKN { get; set; }
        public string INGAlias { get; set; }

        public override string ToString()
        {
            return aliasName;
        }

        public Baglanti(string _alias, string _ip, string _port, string _db, string _savePath, bool _ingent, string _inguser, string _ingpass, string _ingvkn, string _ingalias)
        {
            aliasName = _alias;
            hostIP = _ip;
            hostPort = _port;
            hostDB = _db;
            SaveXmlPath = _savePath;
            INGEntVar = _ingent;
            INGKullanici = _inguser;
            INGSifre = _ingpass;
            INGVKN = _ingvkn;
            INGAlias = _ingalias;
        }
    }

    public class KDV
    {
        public double kdvOrani { get; set; }
        public double kdvTutari { get; set; }
        public double kdvMatrahi { get; set; }
        public double kdvliTutar { get; set; }
    }

    public class KDVOranlari
    {
        public List <KDV> kdvler=new List<KDV>();

        public int KdvEkle(double kdvorani, double kdvtutari, double kdvmatrahi, double kdvlitutar)
        {
            int res = -1;
            for (int i = 0; i < kdvler.Count; i++)
            {
                KDV k=kdvler[i];
                if (k.kdvOrani == kdvorani)
                {
                    k.kdvliTutar += kdvlitutar;
                    k.kdvMatrahi += kdvmatrahi;
                    k.kdvTutari += kdvtutari;
                    res = i;
                    break;
                }
            }
            if (res == -1)
            {
                KDV k = new KDV();
                k.kdvOrani = kdvorani;
                k.kdvliTutar = kdvlitutar;
                k.kdvMatrahi = kdvmatrahi;
                k.kdvTutari = kdvtutari;
                kdvler.Add(k);
                res = kdvler.Count - 1;
            }
            return res;
        }

        public int find(double kdvorani)
        {
            int res = -1;
            
            return res;
        }

    }

    public class SampleBase
    {
        //private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void loadLicense()
        {
            ublSigner.LOGGER.Debug("License is being loaded from: " + Conn.ROOT_DIR + "efatura\\lisans\\BES_lisans.xml");
            LicenseUtil.setLicenseXml(new FileStream(Conn.ROOT_DIR + "efatura\\lisans/BES_lisans.xml", FileMode.Open, FileAccess.Read));
        }

        static protected OfflineResolver getPolicyResolver()
        {
            OfflineResolver POLICY_RESOLVER;
            POLICY_RESOLVER = new OfflineResolver();
            POLICY_RESOLVER.register("2.16.792.1.61.0.1.5070.3.1.1", Conn.ROOT_DIR + "efatura\\config\\profiller/Elektronik_Imza_Kullanim_Profilleri_Rehberi.pdf", "text/plain");
            POLICY_RESOLVER.register("2.16.792.1.61.0.1.5070.3.2.1", Conn.ROOT_DIR + "efatura\\config\\profiller/Elektronik_Imza_Kullanim_Profilleri_Rehberi.pdf", "text/plain");
            POLICY_RESOLVER.register("2.16.792.1.61.0.1.5070.3.3.1", Conn.ROOT_DIR + "efatura\\config\\profiller/Elektronik_Imza_Kullanim_Profilleri_Rehberi.pdf", "text/plain");
            return POLICY_RESOLVER;
        }
    }

    public static class Conn
    {
        public static MySqlConnection myConn;
        public static string ROOT_DIR;
        public static string aliasName = "";
        public static string hostIP = "";
        public static string hostDB="";
        public static string hostPort = "";
        public static string SaveXmlPath = "";
        public static bool INGEntVar = false;
        public static string INGVKN = "";
        public static string INGAlias = "";
        public static string INGUserName = "";
        public static string INGPassword = "";

        public static DialogResult InputBox(string title, string promptText, bool PasswordInput, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            if (PasswordInput == true)
            {
                textBox.PasswordChar = '*';
                textBox.UseSystemPasswordChar = true;
            }
            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        public static void RunShell(string cmdArgument, bool hidden=true)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = hidden ? System.Diagnostics.ProcessWindowStyle.Hidden : System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd";
            startInfo.Arguments = "/c " + cmdArgument;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.StandardOutputEncoding = Encoding.UTF8;
            startInfo.StandardErrorEncoding = Encoding.UTF8;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        private static void OutputHandler(object theProcess, System.Diagnostics.DataReceivedEventArgs evtdata)
        {
            //evtdata.Data has the output data
            //use it, display it, or discard it
        }

        public static string getOnlyNumber(string alpha)
        {
            string res = "";
            char[] mychar = alpha.ToCharArray();

            foreach (char ch in mychar)
            {
                if (char.IsDigit(ch))
                {

                    res = res + ch.ToString();
                }
            }

            //res = (from t in alpha where char.IsDigit(t) select t).ToString();
            return res;
        }

        public static string XmlEscape(string unescaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }

        public static string XmlUnescape(string escaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            md5 = null;
            return sb.ToString();
        }

        public static string CalculateMD5HashBytes(byte[] input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            //byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(input);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            md5 = null;
            return sb.ToString();
        }

        public static string convert2UTF(string orig, bool escape=false)
        {
            string r=orig;
            //char c1 = (char)221;
            r = r.Replace('Þ', 'Ş');
            r = r.Replace('Ý', 'İ');
            r = r.Replace('Ð', 'Ğ');
            r = r.Replace('þ', 'ş');
            r = r.Replace('ý', 'ı');
            r = r.Replace('ð', 'ğ');
            if (escape) r = XmlEscape(r);
            return r;
        }

        public static string convert2Latin(string orig, bool escape=false)
        {
            string res = orig;
            res = res.Replace('Ş', 'Þ');
            res = res.Replace('İ', 'Ý');
            res = res.Replace('Ğ', 'Ð');
            res = res.Replace('ş', 'þ');
            res = res.Replace('ı', 'ý');
            res = res.Replace('ğ', 'ð');
            if (escape) res = XmlEscape(res);
            return res;
        }

        public static string Left(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            string result = "";
            try
            {
                int len = param.Length;
                if (length > len) length = len;
                result = param.Substring(0, length);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Conn.Left hata oluştu\r\n" + param + " " + length.ToString() + "\r\n" + ex.Message);
            }
            //return the result of the operation
            return result;
        }

        public static string Right(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        public static string RightFromLeft(string param, int start)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = "";
            try
            {
                if (param.Length > start)
                {
                    int len = param.Length - start;
                    result = param.Substring(start, len);
                    //return the result of the operation
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Conn.RightFromLeft hata oluştu\r\n" + param + " " + start.ToString() + "\r\n" + ex.Message);
            }
            return result;
        }

        public static XmlDocument newEnvelope(String XMLPath)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(XMLPath);
                MemoryStream ms = new MemoryStream(bytes);

                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                XmlReader reader = XmlReader.Create(ms);
                doc.Load(reader);

                return doc;
            }
            catch (System.Exception x)
            {
                Debug.WriteLine(x.StackTrace);
            }
            throw new Exception("Cant construct envelope xml ");
        }

        public static string Mid(string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            string result = param.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            string result = param.Substring(startIndex);
            //return the result of the operation
            return result;
        }

        public static void OpenConn()
        {
            Properties.Settings mySettings = new Properties.Settings();
            try
            {
                myConn = new MySqlConnection();
                if (hostIP.Trim() == "") hostIP = mySettings.MysqlHost;
                if (hostDB.Trim() == "") hostDB = mySettings.MysqlDB;
                if (hostPort.Trim() == "") hostPort = mySettings.MysqlPort;
                if (SaveXmlPath.Trim() == "") SaveXmlPath = mySettings.SaveXmlPath;
                myConn.ConnectionString = String.Format("server={0}; user id={1}; password={2}; port={3}; " +
                                                        "database=mysql; pooling=false; Allow User Variables=True; "+
                                                        "Allow Zero Datetime=True; Character Set=utf8; "+
                                                        "Convert Zero Datetime=True; Connect Timeout=30; "+
                                                        "Default Command Timeout=60; Use Compression=True; "+
                                                        "Keep Alive=15; Charset=utf8; database={4}; respect binary flags=false;",
                                                        hostIP, "ibem", "mosgatim1", hostPort, hostDB); 

                myConn.Open();
                MySqlCommand c = new MySqlCommand();
                c.Connection = myConn;

                c.CommandText = "create database if not exists efatura collate=utf8_general_ci";
                c.ExecuteNonQuery();

                c.CommandText = "create table if not exists efatura.efatura ( "+
                                " id double unsigned not null auto_increment, " +
                                " faturaid double unsigned not null, "+
                                " faturatipi varchar(20), " +
                                " faturadb varchar(20), "+
                                " faturatarihi date, "+
                                " row_time timestamp default CURRENT_TIMESTAMP(),"+
                                " faturaxml longblob, " +
                                " faturaxslt longblob, " +
                                " faturazip longblob, "+
                                " faturalog longblob, "+
                                " primary key (id), "+
                                " key fid(faturaid), "+
                                " key ftip(faturatipi), "+
                                " key fdb(faturadb) "+
                                ") engine=myisam collate=utf8_general_ci";
                c.ExecuteNonQuery();


                c.CommandText = "create table if not exists efatura.efaturalog ( " +
                                " id double unsigned not null , " +
                                " faturaid double unsigned not null, " +
                                " faturatipi varchar(20), " +
                                " faturadb varchar(20), "+
                                " faturatarihi date, " +
                                " row_time datetime," +
                                " faturaxml longblob, " +
                                " faturaxslt longblob, " +
                                " faturazip longblob, " +
                                " faturalog longblob, " +
                                " delete_time timestamp default CURRENT_TIMESTAMP(), "+
                                " delid double unsigned not null auto_increment, "+
                                " primary key (delid), " +
                                " key fid(faturaid), "+
                                " key ftip(faturatipi), "+
                                " key fdb(faturadb)"+
                                ") engine=myisam collate=utf8_general_ci";
                c.ExecuteNonQuery();

                c.CommandText = "create table if not exists efatura.baglantilar ( "+
                                " id double unsigned not null auto_increment, "+
                                " alias varchar(255), "+
                                " hostip varchar(30), "+
                                " hostport varchar(8), "+
                                " hostdb varchar(255), "+
                                " SaveXmlPath varchar(255), "+
                                " primary key(id)," +
                                " unique key alho(alias,hostip) "+
                                ") engine=myisam collate=utf8_general_ci";
                c.ExecuteNonQuery();

                CheckField("efatura.baglantilar", "INGEntVar", "tinyint(1)", " default 0");
                CheckField("efatura.baglantilar", "INGKullanici", "varchar(255)", " default ''");
                CheckField("efatura.baglantilar", "INGSifre", "varchar(255)", " default ''");
                CheckField("efatura.baglantilar", "INGVKN", "varchar(255)", " default ''");
                CheckField("efatura.baglantilar", "INGAlias", "varchar(255)", " default ''");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error connecting to the server: " + ex.Message);
            }
        }

        public static void CloseConn()
        {
            myConn.Close();
        }

        public static MySqlDataReader OpenReader(string sql)
        {
            try
            {
                MySqlCommand c = new MySqlCommand();
                c.Connection = myConn;
                c.CommandText = sql;
                MySqlDataReader dr = c.ExecuteReader();
                return dr;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error connecting to the server: " + ex.Message);
                return null;
            }
        }

        public static bool CheckField(string fTable, string fName, string fType, string fNull)
        {
            bool res = false;
            string arama="show fields from {0} like \"{1}\"";
            string addCol = "alter table {0} add column `{1}` {2} {3}";
            string chCol = "alter table {0} modify `{1}` {2} {3}";
            string sql = string.Format(arama, fTable, fName);
            string dmp = ReadSingleField(sql, "Field").ToLower();
            if (dmp != fName.ToLower())
            {
                sql = string.Format(addCol, fTable, fName, fType, fNull);
                ExecuteSql(sql);
                res = true;
            }
            else
            {
                dmp = ReadSingleField(sql, "Type").ToLower();
                if (dmp != fType.ToLower())
                {
                    sql = string.Format(chCol, fTable, fName, fType, fNull);
                    ExecuteSql(sql);
                    res = true;
                }
            }

            return res;
        }

        public static MySqlDataAdapter OpenAdapter(string sql)
        {
            return new MySqlDataAdapter(sql, myConn);
            //MySqlDataAdapter da=c
        }

        public static string ReadSingleField(string sql, string fieldName)
        {
            string res = "";
            try
            {
                MySqlCommand c = new MySqlCommand();
                c.Connection = myConn;
                c.CommandText = sql;
                MySqlDataReader dr = c.ExecuteReader();
                int t = -1;
                try
                {
                    t = Int32.Parse(fieldName);
                }
                catch (Exception ex)
                {
                    t = -1;
                }
                if (dr.Read())
                {
                    if (t > -1)
                    {
                        res = dr[t].ToString();
                    }
                    else
                    {
                        res = dr[fieldName].ToString();
                    }
                }
                dr.Close();
                dr.Dispose();
                dr = null;
                c = null;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error connecting to the server: " + ex.Message);
            }
            return convert2UTF(res);
        }

        public static string SabitOku(string SabitAdi)
        {
            string sql = String.Format("select deger from sabitler where degeradi='{0}'", SabitAdi);
            return ReadSingleField(sql, "deger");
        }

        public static bool ExecuteSql(string sql)
        {
            bool res = false;
            try
            {
                MySqlCommand c = new MySqlCommand(sql, myConn);
                c.ExecuteNonQuery();
                res = true;
                c = null;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Sorgu Calistirilirken hata olustu.\r\n" + ex.Message.ToString());
            }

            return res;
        }

    }

    class common
    {

    }

}
