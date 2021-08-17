using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Text;
using HRESULT = System.Int32;
using COLORREF = System.UInt32;
using IShellItemArray = System.IntPtr;
using System.Runtime.InteropServices;

namespace DyDesktopWinForms
{
    public struct RECT
    {
        int left;
        int top;
        int right;
        int bottom;
    }

    public enum DESKTOP_SLIDESHOW_OPTIONS
    {
        DSO_SHUFFLEIMAGES = 0x1
    }

    [Flags]
    public enum DESKTOP_SLIDESHOW_STATE
    {
        DSS_ENABLED = 0x1,
        DSS_SLIDESHOW = 0x2,
        DSS_DISABLED_BY_REMOTE_SESSION = 0x4
    }

    [Flags]
    public enum DESKTOP_SLIDESHOW_DIRECTION
    {
        DSD_FORWARD = 0,
        DSD_BACKWARD = 1
    }

    public enum DESKTOP_WALLPAPER_POSITION
    {
        DWPOS_CENTER = 0,
        DWPOS_TILE = 1,
        DWPOS_STRETCH = 2,
        DWPOS_FIT = 3,
        DWPOS_FILL = 4,
        DWPOS_SPAN = 5
    }

    public static class Const
    {
        public const string IID_IDesktopWallpaper = "B92B56A9-8B55-4E14-9A89-0199BBB6F93B";
        public const string CLSID_DesktopWallpaper = "C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD";
    }

    [ComImport]
    [Guid(Const.IID_IDesktopWallpaper)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDesktopWallpaper
    {
        HRESULT SetWallpaper(
           /* [unique][in] */ string monitorID,
           /* [in] */ string wallpaper);

        HRESULT GetWallpaper(
           /* [unique][in] */ string monitorID,
           /* [string][out] */ out StringBuilder wallpaper);

        HRESULT GetMonitorDevicePathAt(
           /* [in] */ uint monitorIndex,
           /* [string][out] */ out StringBuilder monitorID);

        HRESULT GetMonitorDevicePathCount(
           /* [out] */ out uint count);

        HRESULT GetMonitorRECT(
           /* [in] */ string monitorID,
           /* [out] */ out RECT displayRect);

        HRESULT SetBackgroundColor(
           /* [in] */ COLORREF color);

        HRESULT GetBackgroundColor(
           /* [out] */ out COLORREF color);

        HRESULT SetPosition(
           /* [in] */ DESKTOP_WALLPAPER_POSITION position);

        HRESULT GetPosition(
           /* [out] */ out DESKTOP_WALLPAPER_POSITION position);

        HRESULT SetSlideshow(
           /* [in] */  IShellItemArray items);

        HRESULT GetSlideshow(
           /* [out] */ out IShellItemArray items);

        HRESULT SetSlideshowOptions(
           /* [in] */ DESKTOP_SLIDESHOW_OPTIONS options,
           /* [in] */ uint slideshowTick);

        HRESULT GetSlideshowOptions(
           /* [out] */ out DESKTOP_SLIDESHOW_OPTIONS options,
           /* [out] */ out uint slideshowTick);

        HRESULT AdvanceSlideshow(
           /* [unique][in] */ string monitorID,
           /* [in] */ DESKTOP_SLIDESHOW_DIRECTION direction);

        HRESULT GetStatus(
           /* [out] */ out DESKTOP_SLIDESHOW_STATE state);

        HRESULT Enable(
           /* [in] */ bool enable);
    }

    [ComImport]
    [Guid(Const.CLSID_DesktopWallpaper)]
    public class DesktopWallpaper
    {
    }

}
