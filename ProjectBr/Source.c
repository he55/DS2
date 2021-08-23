#include <stdlib.h>
#include <Windows.h>

_declspec(dllexport)
ULONGLONG getA(void) {
	LASTINPUTINFO lii = { sizeof(LASTINPUTINFO),0 };
	GetLastInputInfo(&lii);
	ULONGLONG v= GetTickCount64();
	return v - lii.dwTime;
}

HWND gg;
BOOL CALLBACK pbWNDENUMPROC(HWND h, LPARAM l) {
	HWND p = FindWindowEx(h, NULL, "SHELLDLL_DefView", NULL);
	if (p) {
		gg = FindWindowEx(p, NULL, "SysListView32", NULL);
		return FALSE;
	}
	return TRUE;
}

_declspec(dllexport)
int getB(void) {
	EnumWindows(pbWNDENUMPROC,NULL);

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

