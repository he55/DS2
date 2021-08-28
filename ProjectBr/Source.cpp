#include <Windows.h>
#include <ShlObj.h>

extern "C"
_declspec(dllexport)
ULONGLONG getA(void) {
	LASTINPUTINFO lii = { sizeof(LASTINPUTINFO),0 };
	GetLastInputInfo(&lii);
	ULONGLONG v= GetTickCount64();
	return v - lii.dwTime;
}


extern "C"
_declspec(dllexport)
int getB(void) {
	static HWND gg;

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

	int ic = 0;
	int w=1919;
	int h=900;
	POINT ps[9] = { 
		{0,0},{w/2,0},{w,0},
		{0,h/2},{w/2,h/2},{w,h/2},
		{0,h},{w/2,h},{w,h}
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
HWND getC(void) {
	static HWND gc;

	HWND ph = FindWindow("Progman", NULL);
	SendMessageTimeout(ph,0x052c, NULL, NULL,SMTO_NORMAL, 1000, NULL);
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
void getD(void) {
	HRESULT nRet = CoInitialize(NULL);
	if (SUCCEEDED(nRet)) {
		IDesktopWallpaper* p;
		nRet = CoCreateInstance(CLSID_DesktopWallpaper, 0, CLSCTX_LOCAL_SERVER, IID_IDesktopWallpaper, (void**)&p);
		if (SUCCEEDED(nRet)) {
			LPWSTR str;
			p->GetWallpaper(NULL, &str);
			if (wcslen(str)) {
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




