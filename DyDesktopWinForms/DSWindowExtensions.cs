using System.Windows;

namespace DyDesktopWinForms
{
    public static class DSWindowExtensions
    {
        public static void FullScreen(this Window t)
        {
            t.WindowStyle = WindowStyle.None;
            t.ResizeMode = ResizeMode.NoResize;
            t.Left = 0;
            t.Top = 0;
            t.Width = SystemParameters.PrimaryScreenWidth;
            t.Height = SystemParameters.PrimaryScreenHeight;
        }

        public static void SetScreen(this Window t, int x, int y, int w, int h)
        {
            t.WindowStyle = WindowStyle.None;
            t.ResizeMode = ResizeMode.NoResize;
            t.Left = x;
            t.Top = y;
            t.Width = w;
            t.Height = h;
        }
    }
}
