/**
 *
 * NAME:    el.h
 *
 * DESCRIPTION: Common environment abstraction layer (EL)
 *
 *
 * AUTHOR:  Roman Filippov,
 *          Alexander Lokotkov
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group.
 *      All rights reserved.
 ********************************************************/

#ifndef ___EL_H___
#define ___EL_H___

#if (!defined __WINDOWS_CE__) && (!defined __QNX_6__) && (!defined _WIN32_WINNT)
	#error Specify target OS!
#endif

#include <eal/el_types.h>

// Include OS-specific header

#ifdef __WINDOWS_CE__
	#include <eal/ce/ce_el.h>
#elif defined __QNX_6__
	#include <eal/qnx/qnx_el.h>
#elif defined _WIN32_WINNT
	#include <eal/xp/xp_el.h>
#endif

#ifdef __cplusplus
extern "C" F_STR el_path_separator;
#else
extern F_STR el_path_separator;
#endif

#include <eal/declredf.h>

#ifdef __cplusplus
extern "C"
{
#endif


#ifndef KERNEL_DRIVER_CODE

  FDLL_EXPORT FILE_HANDLE el_load_driver(F_STR device_name_prefix, 
                                         F_STR device_namespace,
                                         F_STR driver_module_path,
                                         size_t device_index, 
                                         F_STR command_line);

  FDLL_EXPORT bool        el_unload_driver(FILE_HANDLE drv_handle);

  FDLL_EXPORT FILE_HANDLE el_open(const F_STR name); 

  FDLL_EXPORT bool        el_write(FILE_HANDLE handle, void* buffer, size_t count, size_t* count_written); 

  FDLL_EXPORT bool        el_read(FILE_HANDLE handle, void* buffer, size_t count, size_t* count_read); 

  FDLL_EXPORT bool        el_close(FILE_HANDLE handle);

  FDLL_EXPORT size_t      el_seek(FILE_HANDLE handle, size_t pos, EL_SEEK_METHOD method);


  void  app_startup();
  void  app_finish();

#endif

#ifdef __cplusplus
}
#endif

// Debug printing
#include <eal/debug/el_dbg.h>

// Common macro definitions
#define el_mem_alloc(sz) malloc(sz)
#define el_mem_free(ptr) free(ptr)
#define el_mem_clr(ptr, sz) memset((ptr), 0, (sz))
#define el_mem_set(ptr, v, sz) memset((ptr), (v), (sz))
#define el_mem_cpy(dst, src, sz) memcpy((dst), (src), (sz))



#define F_TIME_isExpired(cur_t, expire_t) ( ( (signed long int)((cur_t) - (expire_t)) ) >= 0 )



#endif