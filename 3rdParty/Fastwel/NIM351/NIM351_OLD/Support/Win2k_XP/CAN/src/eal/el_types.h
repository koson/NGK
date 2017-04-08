/**
 *
 * NAME:    el_types.h
 *
 * DESCRIPTION: Platform independent type declarations
 *              and bit manipulation macroses.
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

#ifndef ___EL_TYPES_H___
#define ___EL_TYPES_H___

// Type declarations

#ifndef __cplusplus

typedef enum _bool
{
	false,
	true
} bool;

#endif


typedef unsigned char u8, BYTE;
typedef short int i16;
typedef unsigned short int u16, WORD;
typedef unsigned int u32, F_ATOMIC, F_ADDR, F_COUNT, F_INDEX, F_SIZE, F_TIMEOUT;
typedef int i32;

#ifdef __WINDOWS_CE__
	#include <eal/ce/ce_types.h>
#elif defined __QNX_6__
	#include <eal/qnx/qnx_types.h>
#elif defined _WIN32_WINNT
	#include <eal/xp/xp_types.h>
#endif


// Useful macros

#define FF_GET_BIT(b, off) ____GET_BIT(b, off)
#define FF_SET_BIT(b, off, val) ____SET_BIT(b, off, val)

#define ____GET_BIT(b, off)  ( ((b) >> (off)) & 1 )

#define ____SET_BIT(b, off, val)\
{\
  b &= ~(1 << (off));\
  b |= (((val) != 0) << (off));\
}

#define FF_GET_BITS_VALUE(b, off, len) ____GET_BITS_VALUE(b, off, len)

#define FF_SET_BITS_VALUE(b, off, len, val) ____SET_BITS_VALUE(b, off, len, val)

#define ____GET_BITS_VALUE(b, off, len)  ( ((b) >> (off)) & ((1<<(len)) - 1) )
#define ____SET_BITS_VALUE(b, off, len, val)\
{\
  (b) &= ~(((1 << (len)) - 1) << (off));\
  (b) |= ((val) << (off));\
}

// Text

#if defined(_UNICODE) || defined(UNICODE)

typedef wchar_t F_CHAR, *F_STR;
#define ____EAL_T____(csttr) L##csttr
#define EAL_T(csttr) ____EAL_T____(csttr)



#else

typedef char  F_CHAR, *F_STR;
#define ____EAL_T____(csttr) csttr
#define EAL_T(csttr) ____EAL_T____(csttr)

#endif




#endif

