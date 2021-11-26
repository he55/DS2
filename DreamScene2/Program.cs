using System;
using System.IO;
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
            _ = new Mutex(true, he5.projectName, out bool isNew);
            if (!isNew)
            {
                return;
            }

            Directory.SetCurrentDirectory(Application.StartupPath);

            string extPath = Helper.ExtPath();
            if (!Directory.Exists(extPath))
            {
                Directory.CreateDirectory(extPath);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainDialog mainDialog = new MainDialog();
            mainDialog.Show();

            if (args.Length != 0 && args[0] == he5.cmd)
            {
                mainDialog.Hide();
            }

            Application.Run();
        }
    }
}
