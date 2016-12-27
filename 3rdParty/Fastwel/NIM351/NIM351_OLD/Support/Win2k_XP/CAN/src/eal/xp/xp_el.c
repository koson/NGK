#if (defined _WIN32_WINNT)

#include <eal/el.h>
#include <eal/common/cmdlnprs.h>

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

#if !defined(KERNEL_DRIVER_CODE) && !defined(FDLL_IMPORTS)


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
