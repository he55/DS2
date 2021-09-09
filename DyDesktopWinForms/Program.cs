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

            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            Directory.SetCurrentDirectory(appPath);

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
