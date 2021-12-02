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
            _ = new Mutex(true, Constant.projectName, out bool isNew);
            if (!isNew)
            {
                return;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] vs = assembly.GetManifestResourceNames();
            foreach (string vsName in vs)
            {
                if (vsName.EndsWith(".dll"))
                {
                    string filename = vsName.Substring(Constant.projectName.Length + 1);
                    string pa = Path.Combine(Application.StartupPath, filename);
                    using (FileStream fileStream = File.Create(pa))
                    {
                        Stream stream = assembly.GetManifestResourceStream(vsName);
                        stream.CopyTo(fileStream);
                    }
                }
            }

            //Directory.SetCurrentDirectory(Application.StartupPath);

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
}
