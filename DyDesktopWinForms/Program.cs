using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PInvoke.User32;
using System.Windows.Forms;
using System.IO;
using System.Threading;

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
            Mutex _singleInstanceMutex;
            bool isNew;
            _singleInstanceMutex = new Mutex(true, "DyDesktopWinForms", out isNew);
            if (!isNew)
                return;

            string path = Path.GetDirectoryName(Application.ExecutablePath);
            Directory.SetCurrentDirectory(path);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form1 = new Form1();
            if (args.Length!=0&&args[0]=="-c")
            {
                form1.Show();
                form1.Hide();
            }
            else
            {
                form1.Show();
            }

            Application.Run();
        }
    }
}
