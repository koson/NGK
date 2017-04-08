/*
 *
 * NAME:    xp_el.h
 *
 * DESCRIPTION: Environment layer for Windows XP
 *
 *
 * AUTHOR:
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group.
 *      All rights reserved.
 ********************************************************/

#ifndef ___XP_EL_H___
#define ___XP_EL_H___

// OS includes
//----------------------------------------------------------------------------------------------------


//----------------------------------------------------------------------------------------------------
// Utility macroses
//----------------------------------------------------------------------------------------------------
#ifdef KERNEL_DRIVER_CODE

#include <ntddk.h>

#define PRINTF(arglist) DbgPrint arglist

// Hardware access macro
//----------------------------------------------------------------------------------------------------
#define el_out8(a, v) WRITE_PORT_UCHAR((PUCHAR) (a), (v))
#define el_out16(a, v) WRITE_PORT_USHORT((PUSHORT) (a), (v))
#define el_out32(a, v) WRITE_PORT_ULONG((PULONG) (a), (v))

#define el_in8(a) (READ_PORT_UCHAR((PUCHAR) (a)))
#define el_in16(a) (READ_PORT_USHORT((PUSHORT) (a)))
#define el_in32(a) (READ_PORT_ULONG((PULONG) (a)))

#define el_read8(a) (READ_REGISTER_UCHAR((PUCHAR) (a))) 
#define el_read16(a) (READ_REGISTER_USHORT((PUSHORT) (a))) 
#define el_read32(a) (READ_REGISTER_ULONG((PULONG) (a))) 

#define el_write8(a, v) WRITE_REGISTER_UCHAR((PUCHAR) (a), (v))
#define el_write16(a, v) WRITE_REGISTER_USHORT((PUSHORT) (a), (v))
#define el_write32(a, v) WRITE_REGISTER_ULONG((PULONG) (a), (v))

#define el_usleep(t) KeStallExecutionProcessor(t)

#else

#include <windows.h>
#include <stdio.h>
#include <conio.h>
#include <winioctl.h>
#include <tchar.h>

#if defined(_UNICODE) || defined(UNICODE)

#define PRINTF(arglist) wprintf arglist
#define SSCANF(arglist) swscanf arglist

#define el_ascii_strncmpi(string1, string2, num)            _strnicmp((string1),(string2),(num))
#define el_isalpha(c)                                       iswalpha((c))
#define el_itoa(num, dest, rdx)                             _itow((num), (dest), (rdx))
#define el_strlen(string)                                   wcslen(string)
#define el_strcpy(strDestination, strSource)                wcscpy((strDestination), (strSource))
#define el_strncpy(strDestination, strSource, count)        wcsncpy((strDestination), (strSource), (count))
#define el_strcat(strDestination, strSource)                wcscat((strDestination), (strSource))
#define el_strncat(strDestination, strSource, count)        wcsncat((strDestination), (strSource), (count))
#define el_strcmp(string1, string2)                         wcscmp((string1), (string2))
#define el_strncmp(string1, string2, count)                 wcsncmp((string1), (string2), (count))
#define el_strchr(string, c)                                wcschr((string), (c))
#define el_strrchr(string, c)                               wcsrchr((string), (c))
#define el_strcmpi(string1, string2)                        _wcsicmp((string1), (string2))
#define el_strtok(str_token, str_delim)                     wcstok((str_token), (str_delim))
#define el_strtoul(string, pend, base)                      wcstoul((string), (pend), (base))
#define el_strncmpi(string1, string2, num)                  _wcsnicmp((string1),(string2),(num))

#else

#define PRINTF(arglist) printf arglist
#define SSCANF(arglist) sscanf arglist

#define el_ascii_strncmpi(string1, string2, num)            strncmpi((string1),(string2),(num))
#define el_isalpha(c)                                       isalpha((c))
#define el_itoa(num, dest, rdx)                             itoa((num), (dest), (rdx))
#define el_strlen(string)                                   strlen((string))
#define el_strcpy(strDestination, strSource)                strcpy((strDestination), (strSource))
#define el_strncpy(strDestination, strSource, count)        strncpy((strDestination), (strSource), (count))
#define el_strcat(strDestination, strSource)                strcat((strDestination), (strSource))
#define el_strncat(strDestination, strSource, count)        strncat((strDestination), (strSource), (count))
#define el_strcmp(string1, string2)                         strcmp((string1), (string2))
#define el_strncmp(string1, string2, count)                 strncmp((string1), (string2), (count))
#define el_strchr(string, c)                                strchr((string), (c))
#define el_strrchr(string, c)                               strchr((string), (c))
#define el_strcmpi(string1, string2)                        strcmpi((string1), (string2))
#define el_strtok(str_token, str_delim)                     strtok((str_token), (str_delim))
#define el_strtoul(string, pend, base)                      strtoul((string), (pend), (base))
#define el_strncmpi(string1, string2, num)                  _strnicmp((string1),(string2),(num))

#endif


// Unified OS interface for drivers
//----------------------------------------------------------------------------------------------------
#define el_sleep(t) Sleep(t)

#ifndef USE_OVERLAPPED
#define el_device_ctl(hnd, ctl, inb, insz, outb, outsz, retn) \
			DeviceIoControl((HANDLE)(hnd), (ctl), (inb), (insz), (outb), (outsz), (LPDWORD)(retn), 0)
#endif

#define el_ioctl_success(rs) ((rs) != 0)

#define EL_INVALID_HANDLE_VALUE   INVALID_HANDLE_VALUE
#define el_handle_valid(handle) ((handle) != INVALID_HANDLE_VALUE)

#define el_get_tick_count() GetTickCount()

typedef enum _EL_SEEK_METHOD
{
	EL_SEEK_SET = FILE_BEGIN,
	EL_SEEK_CUR = FILE_CURRENT,
	EL_SEEK_END = FILE_END
} EL_SEEK_METHOD;

typedef HANDLE F_EVENT;
#define el_create_event() CreateEvent(NULL, TRUE, FALSE, NULL)
#define el_event_valid(ev) ((ev) != NULL)
#define el_destroy_event(ev) CloseHandle(ev)
#define el_set_event(ev) SetEvent(ev)
#define el_reset_event(ev) ResetEvent(ev)
#define el_wait_event_success(ev, ms) (WaitForSingleObject((ev), (DWORD)(ms)) == WAIT_OBJECT_0)

#endif

typedef LARGE_INTEGER F_TIME;


#endif