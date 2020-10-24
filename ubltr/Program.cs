using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.IO;
using System.Xml;

namespace ubltr
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                bool editConfig = false;
                string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\efatura\\";
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

                /*string usersPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                string userPath = "";
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    userPath = Directory.GetParent(usersPath).FullName + "\\";
                    usersPath = Directory.GetParent(usersPath).Parent.FullName + "\\";                
                }*/
                Conn.ROOT_DIR = docsPath;
                if (!Directory.Exists(docsPath)) Directory.CreateDirectory(docsPath);

                if (!Directory.Exists(docsPath + "efatura\\"))
                {
                    Directory.CreateDirectory(docsPath + "efatura\\");
                    Directory.CreateDirectory(docsPath + "efatura\\config\\");
                    Directory.CreateDirectory(docsPath + "efatura\\config\\policy\\");
                    Directory.CreateDirectory(docsPath + "efatura\\config\\profiler\\");

                    Directory.CreateDirectory(docsPath + "efatura\\lisans\\");
                    string[] a = Directory.GetFiles(appPath + "config\\");
                    foreach (string fname in a)
                        File.Copy(fname, docsPath + "efatura\\config\\" + Path.GetFileName(fname));

                    Array.Clear(a, 0, a.Length);
                    a = Directory.GetFiles(appPath + "config\\policy\\");
                    foreach (string fname in a)
                        File.Copy(fname, docsPath + "efatura\\config\\policy\\" + Path.GetFileName(fname));

                    Array.Clear(a, 0, a.Length);
                    a = Directory.GetFiles(appPath + "config\\profiler\\");
                    foreach (string fname in a)
                        File.Copy(fname, docsPath + "efatura\\config\\profiler\\" + Path.GetFileName(fname));

                    Array.Clear(a, 0, a.Length);
                    a = Directory.GetFiles(appPath + "lisans\\");
                    foreach (string fname in a)
                        File.Copy(fname, docsPath + "efatura\\lisans\\" + Path.GetFileName(fname));
                    File.Copy(appPath + "efatura.xslt", docsPath + "efatura\\efatura.xslt");
                    if (!File.Exists(docsPath + "efatura\\ubltr.exe.config")) editConfig = true;
                    File.Copy(appPath + "ubltr.exe.config", docsPath + "efatura\\ubltr.exe.config", true);

                }

                string userPath = Directory.GetParent(docsPath).Parent.Parent.FullName + "\\";
                if (!Directory.Exists(userPath + ".sertifikadeposu\\"))
                {
                    Directory.CreateDirectory(userPath + ".sertifikadeposu\\");

                    string[] b = Directory.GetFiles(appPath + "certstore\\");
                    foreach (string fname in b)
                        File.Copy(fname, userPath + ".sertifikadeposu\\" + Path.GetFileName(fname));
                }

                if (editConfig)
                {
                    Conn.RunShell("notepad \"" + docsPath + "efatura\\ubltr.exe.config\"");
                }

                if (File.Exists(docsPath + "efatura\\ubltr.exe.config"))
                {
                    File.Delete(appPath + "ubltr.exe.config");
                    File.Copy(docsPath + "efatura\\ubltr.exe.config", appPath + "ubltr.exe.config", true);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Uygulama Yuklenirken Hata Oluştu\r\n" + ex.Message.ToString());
            }
            Properties.Settings.Default.Reload();
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLogin());
        }
    }
}
