using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PInvoke.User32;
using System.Windows.Forms;

namespace DyDesktopWinForms
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            IntPtr h = FindWindow(null, "DyDesktopWinForms");
            if (h!=IntPtr.Zero)
            {
                ShowWindow(h, WindowShowStyle.SW_RESTORE);
                SetForegroundWindow(h);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
