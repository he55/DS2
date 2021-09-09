using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DyDesktopWinForms
{
    public static class DSPInvoke
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("ProjectBr.dll")]
        public static extern ulong getA();

        [DllImport("ProjectBr.dll")]
        public static extern int getB();

        [DllImport("ProjectBr.dll")]
        public static extern IntPtr getC();

        [DllImport("ProjectBr.dll")]
        public static extern ulong getD();

        [DllImport("ProjectBr.dll")]
        public static extern int setPos(IntPtr ptr, Rectangle r);

        [DllImport("ProjectBr.dll")]
        public static extern int reLastPos();
    }
}
