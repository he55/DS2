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
            _ = new Mutex(true, "DreamScene2", out bool isNew);
            if (!isNew)
            {
                return;
            }

            string extPath = Helper.ExtPath();
            if (!Directory.Exists(extPath))
            {
                Directory.CreateDirectory(extPath);
            }

            Directory.SetCurrentDirectory(Application.StartupPath);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainDialog mainDialog = new MainDialog();
            mainDialog.Show();

            if (args.Length != 0 && args[0] == "-c")
            {
                mainDialog.Hide();
            }

            Application.Run();
        }
    }
}
