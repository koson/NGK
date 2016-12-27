/*
 *
 * NAME:    qnx_el.h
 *
 * DESCRIPTION: Implementation of environment layer for QNX 6.3
 *
 *
 * AUTHOR:  Roman Filippov
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2009 Fastwel Co Ltd.
 *      All rights reserved.
 ********************************************************/

// OS headers
//----------------------------------------------------------------------------------------------------

#ifndef ___QNX_EL_H___
#define ___QNX_EL_H___

#include <errno.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/iofunc.h>
#include <sys/dispatch.h>
#include <sys/neutrino.h>
#include <sys/resmgr.h>
#include <sys/types.h>
#include <devctl.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <hw/inout.h>
#include <fcntl.h>
#include <stdlib.h>
#include <semaphore.h>
#include <hw/pci.h>
#include <atomic.h>
#include <time.h>


//----------------------------------------------------------------------------------------------------
// Utility macroses
//----------------------------------------------------------------------------------------------------
#define PRINTF(arglist) printf arglist

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

// Hardware access functions
//----------------------------------------------------------------------------------------------------

#define el_out8(a, v) out8(a, v)
#define el_out16(a, v) out16(a, v)
#define el_out32(a, v) out32(a, v)

#define el_in8(a) (in8(a))
#define el_in16(a) (in16(a))
#define el_in32(a) (in32(a))

// Common OS interface for drivers
//----------------------------------------------------------------------------------------------------

#define el_sleep(t) delay(t)

#define el_device_ctl(hnd, ctl, inb, insz, outb, outsz, retn) \
		devctl((int) (hnd), (ctl), (inb), (insz), 0)

#define el_ioctl_success(rs) ((rs) == EOK)

#define el_open(name) open(name, O_RDWR)

#define el_close(handle) close(handle)

#define el_handle_valid(handle) (handle != -1)

unsigned int el_get_tick_count(void);

enum
{
	EL_SEEK_SET = SEEK_SET,
	EL_SEEK_CUR = SEEK_CUR,
	EL_SEEK_END = SEEK_END
};

#define el_read(id, buf, sz, rd)		read(id, buf, sz)
#define el_write(id, buf, sz, wr)		write(id, buf, sz)
#define el_seek(id, pos, method)	lseek(id, pos, method)

#endif