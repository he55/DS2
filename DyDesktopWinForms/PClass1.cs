using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DyDesktopWinForms
{
    public static class PClass1
    {

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("ProjectBr.dll")]
      public  static extern ulong getA();

        [DllImport("ProjectBr.dll")]
        public static extern int getB();

        [DllImport("ProjectBr.dll")]
        public static extern IntPtr getC();

        [DllImport("ProjectBr.dll")]
        public static extern ulong getD();

    }
}
