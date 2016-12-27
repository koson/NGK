#if (defined _WIN32_WINNT)

#include <eal/el.h>
#include <eal/common/cmdlnprs.h>

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

#if !defined(KERNEL_DRIVER_CODE) && !defined(FDLL_IMPORTS)


#ifndef USE_OVERLAPPED

bool el_init()
{
  return true;
}

void el_deinit()
{
}

FILE_HANDLE el_open(const F_STR name)
{
  return CreateFile(name, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, 
                    0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, 0);
}

bool el_write(FILE_HANDLE handle, void* buffer, size_t count, size_t* count_wrote)
{
  return (TRUE == WriteFile(handle, buffer, count, (LPDWORD) count_wrote, NULL));
}

bool el_read(FILE_HANDLE handle, void* buffer, size_t count, size_t* count_read)
{
  return (TRUE == ReadFile(handle, buffer, count, (LPDWORD) count_read, NULL));
}
#else

#define SIMULTANEOUS_OVERLAPPED_OPERATIONS_MAX_COUNT   32

HANDLE overlaped_events_arr[SIMULTANEOUS_OVERLAPPED_OPERATIONS_MAX_COUNT];
int overlaped_events_count = 0;

bool el_init()
{
}

void el_deinit()
{
}

FILE_HANDLE el_open(const F_STR name)
{
  return CreateFile(name, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, 
                    0, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, 0);
}

bool el_write(FILE_HANDLE handle, void* buffer, size_t count, size_t* count_wrote)
{
  bool res;
  OVERLAPPED ov;

  memset(&ov, 0, sizeof(OVERLAPPED));
  ov.hEvent = CreateEvent( NULL, true, false, NULL );
			
  res = WriteFile(handle, buffer, count, (LPDWORD) count_wrote, &ov);
  if(!res)
  {
    if(GetLastError() == ERROR_IO_PENDING)
      res = GetOverlappedResult(handle, &ov, count_wrote, TRUE);
  }
  CloseHandle(ov.hEvent);

  return res;
}

bool el_read(FILE_HANDLE handle, void* buffer, size_t count, size_t* count_read)
{
  bool res;
  OVERLAPPED ov;

  memset(&ov, 0, sizeof(OVERLAPPED));
  ov.hEvent = CreateEvent( NULL, true, false, NULL );
			
  res = ReadFile(handle, buffer, count, (LPDWORD) count_read, &ov);
  if(!res)
  {
    if(GetLastError() == ERROR_IO_PENDING)
      res = GetOverlappedResult(handle, &ov, count_read, TRUE);
  }
  CloseHandle(ov.hEvent);

  return res;
}

bool el_device_ctl(FILE_HANDLE handle, DWORD ctl, void* inb, DWORD insz, void* outb, DWORD outsz, LPDWORD retn)
{
  bool res;
  OVERLAPPED ov;

  memset(&ov, 0, sizeof(OVERLAPPED));
  ov.hEvent = CreateEvent( NULL, true, false, NULL );
			
  res = DeviceIoControl(handle, ctl, inb, insz, outb, outsz, retn, &ov);
  if(!res)
  {
    if(GetLastError() == ERROR_IO_PENDING)
      res = GetOverlappedResult(handle, &ov, retn, TRUE);
  }
  CloseHandle(ov.hEvent);

  return res;
}

#endif

bool el_close(FILE_HANDLE handle)
{
  return (TRUE == CloseHandle(handle));
}

size_t el_seek(FILE_HANDLE handle, size_t pos, EL_SEEK_METHOD method)
{
  return SetFilePointer(handle, pos, NULL, method);
}
#endif
#if !defined(KERNEL_DRIVER_CODE) && (defined(_USRDLL) || defined(_DLL) || defined(_USERDLL))

int WINAPI DllMain( HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved )
{
  if (ul_reason_for_call == DLL_PROCESS_ATTACH) 
  {
    app_startup();
  }

  if (ul_reason_for_call == DLL_PROCESS_DETACH) 
  {
    app_finish();
  }
  return TRUE;
}

#endif


#endif
