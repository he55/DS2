using System;
using System.Runtime.InteropServices;

namespace DreamScene2
{
    public static class PInvoke
    {
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("ProjectBr.dll")]
        public static extern ulong getA();

        [DllImport("ProjectBr.dll")]
        public static extern int getB2(RECT rc);

        [DllImport("ProjectBr.dll")]
        public static extern IntPtr getC();

        [DllImport("ProjectBr.dll")]
        public static extern void reWall();

        [DllImport("ProjectBr.dll")]
        public static extern void setPos(IntPtr ptr, RECT r);

        [DllImport("ProjectBr.dll")]
        public static extern void reLastPos();
    }

    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}
