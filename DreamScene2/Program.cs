using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace DreamScene2
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew;
            Mutex obj = new Mutex(initiallyOwned: true, "Global\\{2EA411F1-BFE2-4EA9-8768-0CFCD6EED87B}", out createdNew);
            if (createdNew)
            {
                string extPath = Helper.ExtPath();
                if (!Directory.Exists(extPath))
                {
                    Directory.CreateDirectory(extPath);
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                MainDialog mainDialog = new MainDialog();
                mainDialog.Show();

                if (args.Length != 0 && args[0] == Constant.cmd)
                {
                    mainDialog.Hide();
                }

                Application.Run();
            }
        }

        static void extractRes()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] vs = assembly.GetManifestResourceNames();
            foreach (string vsName in vs)
            {
                if (vsName.EndsWith(".dll"))
                {
                    string filename = vsName.Substring(Constant.projectName.Length + 1);
                    string pa = Path.Combine(Application.StartupPath, filename);
                    if (!File.Exists(pa))
                    {
                        using (FileStream fileStream = File.Create(pa))
                        {
                            Stream stream = assembly.GetManifestResourceStream(vsName);
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
    }
}
