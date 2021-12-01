#include <Windows.h>
#include <ShlObj.h>

extern "C"
_declspec(dllexport)
ULONGLONG __stdcall GetLastInputTickCount(void) {
    LASTINPUTINFO lii = { sizeof(LASTINPUTINFO),0 };
    GetLastInputInfo(&lii);
    ULONGLONG v = GetTickCount64();
    return v - lii.dwTime;
}


extern "C"
_declspec(dllexport)
int __stdcall TestScreen(RECT rc) {
    static HWND gg=NULL;

    if (!gg) {
        EnumWindows([](HWND h, LPARAM l) {
            HWND p = FindWindowEx(h, NULL, "SHELLDLL_DefView", NULL);
            if (p) {
                gg = FindWindowEx(p, NULL, "SysListView32", NULL);
                return FALSE;
            }
            return TRUE;
            }, NULL);
    }

    const int offset = 4;
    int ic = 0;
    int x = rc.left+offset;
    int y = rc.top+offset;
    int w = rc.right-rc.left-offset*2;
    int h = rc.bottom-rc.top-offset*2;
    POINT ps[9] = {
        {x,y},      {x+(w/2),y},      {x+w,y},
        {x,y+(h/2)},{x+(w/2),y+(h/2)},{x+w,y+(h/2)},
        {x,y+h},    {x+(w/2),y+h},    {x+w,y+h}
    };

    for (size_t i = 0; i < 9; i++)
    {
        HWND ww = WindowFromPoint(ps[i]);
        if (ww == gg) {
            ++ic;
        }
    }

    return ic;
}


extern "C"
_declspec(dllexport)
HWND __stdcall GetDesktopWindowHandle(void) {
    static HWND gc=NULL;

    HWND ph = FindWindow("Progman", NULL);
    SendMessageTimeout(ph, 0x052c, NULL, NULL, SMTO_NORMAL, 1000, NULL);
    EnumWindows([](HWND h, LPARAM l) {
        HWND p = FindWindowEx(h, NULL, "SHELLDLL_DefView", NULL);
        if (p) {
            gc = FindWindowEx(NULL, h, "WorkerW", NULL);
            return FALSE;
        }
        return TRUE;
        }, NULL);
    return gc;
}

extern "C"
_declspec(dllexport)
void __stdcall getD(void) {
    HRESULT nRet = CoInitialize(NULL);
    if (SUCCEEDED(nRet)) {
        IDesktopWallpaper* p=NULL;
        nRet = CoCreateInstance(CLSID_DesktopWallpaper, 0, CLSCTX_LOCAL_SERVER, IID_IDesktopWallpaper, (void**)&p);
        if (SUCCEEDED(nRet)) {
            LPWSTR str=NULL;
            p->GetWallpaper(NULL, &str);
            if (str&&wcslen(str)) {
                p->SetWallpaper(NULL, str);
            }
            else {
                COLORREF c;
                p->GetBackgroundColor(&c);
                p->SetBackgroundColor(c);
            }
            p->Release();
        }

        CoUninitialize();
    }
}


extern "C"
_declspec(dllexport)
void __stdcall RefreshDesktop() {
    char str[MAX_PATH + 1] = { 0 };
    SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, &str, 0);
    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, str, 0);
}


typedef struct MyStruct
{
    HWND hw;
    HWND pa;
    RECT rc;
    LONG st;
} MyStruct;

MyStruct mys;

extern "C"
_declspec(dllexport)
void __stdcall SetWindowPosition(HWND hw, RECT rc) {
    ShowWindow(hw, SW_RESTORE);

    RECT orc;
    GetWindowRect(hw, &orc);
    LONG st = GetWindowLong(hw, GWL_STYLE);
    HWND pa = GetParent(hw);
    mys = { hw,pa,orc,st };

    SetWindowLong(hw, GWL_STYLE, st & (~WS_CAPTION) & (~WS_SYSMENU) & (~WS_THICKFRAME));
    SetWindowPos(hw, HWND_TOP, rc.left, rc.top, rc.right-rc.left, rc.bottom-rc.top, SWP_SHOWWINDOW);
}


extern "C"
_declspec(dllexport)
void __stdcall RestoreLastWindowPosition() {
    if (mys.hw) {
        SetParent(mys.hw, mys.pa);
        SetWindowLong(mys.hw, GWL_STYLE, mys.st);
        SetWindowPos(mys.hw, HWND_TOP, mys.rc.left, mys.rc.top, mys.rc.right-mys.rc.left, mys.rc.bottom-mys.rc.top, SWP_SHOWWINDOW);
    }
    mys = { 0 };
}
