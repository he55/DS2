using System.Drawing;
using System.Windows;

namespace DyDesktopWinForms
{
    public static class DSWindowExtensions
    {
        public static void FullScreen(this Window @this)
        {
            @this.WindowStyle = WindowStyle.None;
            @this.ResizeMode = ResizeMode.NoResize;
            @this.Left = 0;
            @this.Top = 0;
            @this.Width = SystemParameters.PrimaryScreenWidth;
            @this.Height = SystemParameters.PrimaryScreenHeight;
        }

        public static void SetPosition(this Window @this, Rectangle rect)
        {
            @this.WindowStyle = WindowStyle.None;
            @this.ResizeMode = ResizeMode.NoResize;
            @this.Left = rect.Left;
            @this.Top = rect.Top;
            @this.Width = rect.Width;
            @this.Height = rect.Height;
        }
    }
}
