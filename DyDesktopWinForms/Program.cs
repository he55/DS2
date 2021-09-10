using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DyDesktopWinForms
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            _ = new Mutex(true, "DyDesktopWinForms", out bool isNew);
            if (!isNew)
            {
                return;
            }

            string v = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string v1 = Path.Combine(v, ".DyDesktopWinForms");
            if (!Directory.Exists(v1))
            {
                Directory.CreateDirectory(v1);
            }

            Directory.SetCurrentDirectory(Application.StartupPath);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DSMainForm mainForm = new DSMainForm();
            mainForm.Show();

            if (args.Length != 0 && args[0] == "-c")
            {
                mainForm.Hide();
            }

            Application.Run();
        }
    }
}
