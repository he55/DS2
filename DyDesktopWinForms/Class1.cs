using System.Drawing;

namespace DyDesktopWinForms
{
    public static class Class1
    {
        public static RECT mett(this Rectangle rc)
        {
            RECT rect;
            rect.left = rc.Left;
            rect.top = rc.Top;
            rect.right = rc.Right;
            rect.bottom = rc.Bottom;
            return rect;
        }
    }
}
