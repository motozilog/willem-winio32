#include <windows.h>
#include <stdio.h>
#include <tchar.h>
#include <iostream>
#include <fstream>
#include <sstream>
#include <ctime>
#include <iomanip>
#include "TVicPort.h"

enum {
	NotInitialized,
	Initialized,
	InitializeError,
};

// Base address of the relocated parallel port.
short int RelocatedAddress = (short)0x378;
short int isInitialized = NotInitialized;
HMODULE	moduleHandle;

void InitRemap(void) {
	static std::ofstream g_log("out.log");  //指定输出到out.log文件
	std::cout.rdbuf(g_log.rdbuf());  //rdbuf()将终端内容输出到文件

	FILE* fp = NULL;
	TCHAR filename[MAX_PATH];
	TCHAR* filename_ptr;

	if (GetModuleFileName(moduleHandle, filename, MAX_PATH) && (filename_ptr = _tcsrchr(filename, TCHAR('\\'))) != NULL) {
		++filename_ptr;
		_tcsncpy_s(filename_ptr, MAX_PATH - ((filename_ptr - filename) / sizeof(TCHAR)), TEXT("io.ini"), 6);
		if (_tfopen_s(&fp, filename, TEXT("r")) == 0) {
			if (fscanf_s(fp, "%hi", &RelocatedAddress) != 1) {
				MessageBox(NULL, TEXT("Could not parse address in io.ini."), NULL, MB_OK);
				isInitialized = InitializeError;
			}
			fclose(fp);

			if (!OpenTVicPort())
			{
				MessageBox(NULL, TEXT("OpenTVicPort() failed -- have you rebooted since installing TVicPort?"), NULL, MB_OK);
				isInitialized = InitializeError;
			}
			else
			{
				if (!IsDriverOpened())
				{
					MessageBox(NULL, TEXT("IsDriverOpened() returned FALSE, but OpenTVicPort() succeeded. Is TVicPort properly installed?"), NULL, MB_OK);
					isInitialized = InitializeError;
				}
				else
				{
					// Soft access allows for higher performance
					// at the expense of not working if other drivers are already using it...
					SetHardAccess(FALSE);
					isInitialized = Initialized;
				}
			}
		} else {
			MessageBox(NULL, TEXT("Could not open io.ini."), NULL, MB_OK);
			isInitialized = InitializeError;
		}
	} else {
		MessageBox(NULL, TEXT("Could not get module filename."), NULL, MB_OK);
		isInitialized = InitializeError;
	}		
}

// Initialisation callback.
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call) {
		case DLL_PROCESS_ATTACH:
			moduleHandle = hModule;
			isInitialized = NotInitialized;
			break;
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
			break;
		case DLL_PROCESS_DETACH:
			if (IsDriverOpened())
				CloseTVicPort();
			break;
	}
	return TRUE;
}

// Modifies port accesses in the legacy LPT1 parallel port range (0x378..0x37F).
short int FixPortAddress(short int portAddress) {
	if (isInitialized == NotInitialized) {
		InitRemap();
	}

	if (portAddress >= 0x378 && portAddress <= 0x37F) {
		return portAddress - 0x378 + RelocatedAddress;
	} else {
		return portAddress;
	}
}

// Reimplimented versions of the functions in io.dll modified to use the address-fixing function above.
extern "C" __declspec(dllexport) void PortOut(short int Port, char Data) {
	//Out32(FixPortAddress(Port), Data);

	SYSTEMTIME st = {0};
	GetLocalTime(&st);
	std::stringstream ssNow;
    ssNow << std::setw(2) << std::setfill('0') << st.wMinute ;
    ssNow << std::setw(2) << std::setfill('0') << st.wSecond ;
	std::string strNow = ssNow.str();

	std::stringstream ssPort;
    ssPort << std::hex << Port;
    std::string hexPort = ssPort.str();

	std::stringstream ssData;
    ssData << std::setw(2) << std::setfill('0') << std::hex << static_cast<unsigned int>(static_cast<unsigned char>(Data)) ;
	std::string hexData = ssData.str();

	std::cout<<"Time:"<< strNow << " Write:"<<hexPort<<" Data:"<< hexData << std::endl;

	WritePort(FixPortAddress(Port), Data);
}
extern "C" __declspec(dllexport) char PortIn(short int Port) {
	//return (char)Inp32(FixPortAddress(Port));
	char Data = (char)ReadPort(FixPortAddress(Port));

	SYSTEMTIME st = {0};
	GetLocalTime(&st);
	std::stringstream ssNow;
    ssNow << std::setw(2) << std::setfill('0') << st.wMinute ;
    ssNow << std::setw(2) << std::setfill('0') << st.wSecond ;
	std::string strNow = ssNow.str();

	std::stringstream ssPort;
    ssPort << std::hex << Port;
    std::string hexPort = ssPort.str();

	std::stringstream ssData;
    ssData << std::setw(2) << std::setfill('0')  << std::hex << static_cast<unsigned int>(static_cast<unsigned char>(Data)) ;
	std::string hexData = ssData.str();

	std::cout<<"Time:"<< strNow << " Read:"<<hexPort<<" Data:"<< hexData << std::endl;

	return Data;
}

extern "C" __declspec(dllexport) short int IsDriverInstalled() {
	return -1;
}