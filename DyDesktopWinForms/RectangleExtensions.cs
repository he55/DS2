using System.Drawing;

namespace DyDesktopWinForms
{
    public static class RectangleExtensions
    {
        public static RECT ToRECT(this Rectangle @this)
        {
            RECT rect;
            rect.left = @this.Left;
            rect.top = @this.Top;
            rect.right = @this.Right;
            rect.bottom = @this.Bottom;
            return rect;
        }
    }
}
