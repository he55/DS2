#include <Windows.h>
#include <ShlObj.h>

#define __HW_DLLEXPORT extern "C" _declspec(dllexport)


__HW_DLLEXPORT
ULONGLONG __stdcall GetLastInputTickCount(void) {
    LASTINPUTINFO lii = { sizeof(LASTINPUTINFO),0 };
    GetLastInputInfo(&lii);
    ULONGLONG tick = GetTickCount64();
    return tick - lii.dwTime;
}


__HW_DLLEXPORT
int __stdcall TestScreen(RECT rect) {
    static HWND g_hWnd=NULL;
    if (!g_hWnd) {
        EnumWindows([](HWND hWnd, LPARAM) {
            HWND hWnd1 = FindWindowEx(hWnd, NULL, "SHELLDLL_DefView", NULL);
            if (hWnd1) {
                g_hWnd = FindWindowEx(hWnd1, NULL, "SysListView32", NULL);
                return FALSE;
            }
            return TRUE;
        }, NULL);
    }

    const int offset = 4;
    int ic = 0;
    int x = rect.left+offset;
    int y = rect.top+offset;
    int w = rect.right-rect.left-offset*2;
    int h = rect.bottom-rect.top-offset*2;

    POINT ps[9] = {
        {x,y},      {x+(w/2),y},      {x+w,y},
        {x,y+(h/2)},{x+(w/2),y+(h/2)},{x+w,y+(h/2)},
        {x,y+h},    {x+(w/2),y+h},    {x+w,y+h}
    };

    for (size_t i = 0; i < 9; i++)
    {
        HWND hWnd2 = WindowFromPoint(ps[i]);
        if (hWnd2 == g_hWnd) {
            ++ic;
        }
    }
    return ic;
}


__HW_DLLEXPORT
HWND __stdcall GetDesktopWindowHandle(void) {
    static HWND g_hWnd=NULL;
    HWND hWnd1 = FindWindow("Progman", NULL);

    SendMessageTimeout(hWnd1,
        0x052c,
        NULL,
        NULL,
        SMTO_NORMAL,
        1000,
        NULL);

    EnumWindows([](HWND hWnd, LPARAM) {
        HWND hWnd2 = FindWindowEx(hWnd, NULL, "SHELLDLL_DefView", NULL);
        if (hWnd2) {
            g_hWnd = FindWindowEx(NULL, hWnd, "WorkerW", NULL);
            return FALSE;
        }
        return TRUE;
    }, NULL);

    return g_hWnd;
}


__HW_DLLEXPORT
void __stdcall RefreshDesktop() {
    char str[MAX_PATH + 1] = { 0 };
    SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, &str, 0);
    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, str, 0);
}


typedef struct MyStruct
{
    HWND hWnd;
    HWND hWndParent;
    RECT rect;
    LONG st;
} MyStruct;

MyStruct mys;


__HW_DLLEXPORT
void __stdcall SetWindowPosition(HWND hWnd, RECT rect) {
    ShowWindow(hWnd, SW_RESTORE);

    RECT orect;
    GetWindowRect(hWnd, &orect);
    LONG st = GetWindowLong(hWnd, GWL_STYLE);
    HWND hWndParent = GetParent(hWnd);
    mys = { hWnd,hWndParent,orect,st };

    SetWindowLong(hWnd, GWL_STYLE, st & (~WS_CAPTION) & (~WS_SYSMENU) & (~WS_THICKFRAME));
    SetWindowPos(hWnd,
        HWND_TOP,
        rect.left,
        rect.top,
        rect.right-rect.left,
        rect.bottom-rect.top,
        SWP_SHOWWINDOW);
}


__HW_DLLEXPORT
void __stdcall RestoreLastWindowPosition() {
    if (mys.hWnd) {
        SetParent(mys.hWnd, mys.hWndParent);
        SetWindowLong(mys.hWnd, GWL_STYLE, mys.st);
        SetWindowPos(mys.hWnd,
            HWND_TOP,
            mys.rect.left,
            mys.rect.top,
            mys.rect.right-mys.rect.left,
            mys.rect.bottom-mys.rect.top,
            SWP_SHOWWINDOW);
    }
    mys = { 0 };
}


__HW_DLLEXPORT
void __stdcall getD(void) {
    HRESULT nRet = CoInitialize(NULL);
    if (SUCCEEDED(nRet)) {
        IDesktopWallpaper* p = NULL;
        nRet = CoCreateInstance(CLSID_DesktopWallpaper, 0, CLSCTX_LOCAL_SERVER, IID_IDesktopWallpaper, (void**)&p);
        if (SUCCEEDED(nRet)) {
            LPWSTR str = NULL;
            p->GetWallpaper(NULL, &str);
            if (str && wcslen(str)) {
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
