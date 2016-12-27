#ifndef ___XP_TYPES_H___
#define ___XP_TYPES_H___

#ifdef KERNEL_DRIVER_CODE
#define FILE_HANDLE
#else

#include <windows.h>


// Types definitions
//----------------------------------------------------------------------------------------------------

typedef HANDLE FILE_HANDLE;

#endif

typedef unsigned __int64 u64;

//#define ____MACRO_NUM_TO_STRING____(x) #x
//#define MACRO_NUM_TO_STRING(x) ____MACRO_NUM_TO_STRING____(x)

#endif