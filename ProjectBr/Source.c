#include <stdlib.h>
#include <Windows.h>

_declspec(dllexport)
ULONGLONG getA(void) {
	LASTINPUTINFO lii = { sizeof(LASTINPUTINFO),0 };
	GetLastInputInfo(&lii);
	ULONGLONG v= GetTickCount64();
	return v - lii.dwTime;
}