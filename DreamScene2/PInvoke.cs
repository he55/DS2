﻿using System;
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
        public static extern ulong GetLastInputTickCount();

        [DllImport("ProjectBr.dll")]
        public static extern IntPtr GetDesktopWindowHandle();

        [DllImport("ProjectBr.dll")]
        public static extern int TestScreen(RECT rect);

        [DllImport("ProjectBr.dll")]
        public static extern void SetWindowPosition(IntPtr hWnd, RECT rect);

        [DllImport("ProjectBr.dll")]
        public static extern void RestoreLastWindowPosition();

        [DllImport("ProjectBr.dll")]
        public static extern void RefreshDesktop();
    }

    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}
