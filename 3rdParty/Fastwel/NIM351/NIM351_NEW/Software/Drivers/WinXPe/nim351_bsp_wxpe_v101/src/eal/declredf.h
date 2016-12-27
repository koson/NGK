/**
 *
 * NAME:    declredf.h
 *
 * DESCRIPTION: DLL import/export redefinitions helper
 *
 *
 * AUTHOR:  Alexander Lokotkov
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group.
 *      All rights reserved.
 ********************************************************/

#undef FDLL_CLASS
#undef FDLL_PROC
#undef FDLL_DATA
#undef FDLL_EXTERN


#undef FDLL_EXPORT

#if defined(FDLL_IMPORTS) && defined(FDLL_EXPORTS)
# error Only one DLL import/export definition should be used!
#endif

#if defined(_MSC_VER)

# if defined(FDLL_EXPORTS)
#   define FDLL_EXPORT __declspec(dllexport)
# elif defined(FDLL_IMPORTS)
#   define FDLL_EXPORT __declspec(dllimport)
# else
#   define FDLL_EXPORT
#endif


# if defined (FDLL_EXPORTS)
#   define FDLL_CLASS __declspec(dllexport)
#   define FDLL_PROC  __declspec(dllexport)
#   define FDLL_DATA  __declspec(dllexport)
#   define FDLL_EXTERN
# elif defined(FDLL_IMPORTS)
#   define FDLL_CLASS __declspec(dllimport)
#   define FDLL_PROC  __declspec(dllimport)
#   define FDLL_DATA  __declspec(dllimport)
#   define FDLL_EXTERN extern
# else
#   define FDLL_CLASS
#   define FDLL_PROC
#   define FDLL_DATA
#   define FDLL_EXTERN
# endif

#else

# define FDLL_EXPORT
# define FDLL_CLASS
# define FDLL_PROC
# define FDLL_DATA
# define FDLL_EXTERN

#endif

