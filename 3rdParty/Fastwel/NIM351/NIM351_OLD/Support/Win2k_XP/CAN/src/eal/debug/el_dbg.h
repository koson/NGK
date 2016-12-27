/**
 *
 * NAME:    el_dbg.h
 *
 * DESCRIPTION: Debug and console output definitions
 *
 *
 * AUTHOR:  Alexander Lokotkov
 *          
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group.
 *      All rights reserved.
 ********************************************************/

#ifndef ___F_EL_LOGG_H___
#define ___F_EL_LOGG_H___

#define EL_CON_SPRINTFLIST1(a) a
#define EL_CON_SPRINTFLIST2(a,b) a,b
#define EL_CON_SPRINTFLIST3(a,b,c) a,b,c
#define EL_CON_SPRINTFLIST4(a,b,c,d) a,b,c,d
#define EL_CON_SPRINTFLIST5(a,b,c,d,e) a,b,c,d,e
#define EL_CON_SPRINTFLIST6(a,b,c,d,e,f) a,b,c,d,e,f
#define EL_CON_SPRINTFLIST7(a,b,c,d,e,f,g) a,b,c,d,e,f,g
#define EL_CON_SPRINTFLIST8(a,b,c,d,e,f,g,h) a,b,c,d,e,f,g,h
#define EL_CON_SPRINTFLIST9(a,b,c,d,e,f,g,h,i) a,b,c,d,e,f,g,h,i
#define EL_CON_SPRINTFLIST10(a,b,c,d,e,f,g,h,i,j) a,b,c,d,e,f,g,h,i,j
#define EL_CON_SPRINTFLIST11(a,b,c,d,e,f,g,h,i,j,k) a,b,c,d,e,f,g,h,i,j,k


#define _____EL_CON_MESSAGE_PARAMS(sprintflist) PRINTF((EL_CON_SPRINTF##sprintflist))

#define EL_CON_MSG_PARAMS(sprintflist) _____EL_CON_MESSAGE_PARAMS(sprintflist)

#define F_DBG(text) F_DBG_PARAMS(LIST1(EAL_T(text)))
#define F_DBG1(text,par1) F_DBG_PARAMS(LIST2(EAL_T(text),par1))
#define F_DBG2(text,par1,par2) F_DBG_PARAMS(LIST3(EAL_T(text),par1,par2))
#define F_DBG3(text,par1,par2,par3) F_DBG_PARAMS(LIST4(EAL_T(text),par1,par2,par3))
#define F_DBG4(text,par1,par2,par3,par4) F_DBG_PARAMS(LIST5(EAL_T(text),par1,par2,par3,par4))
#define F_DBG5(text,par1,par2,par3,par4,par5) F_DBG_PARAMS(LIST6(EAL_T(text), par1, par2, par3, par4, par5))
#define F_DBG6(text,par1,par2,par3,par4,par5,par6) F_DBG_PARAMS(LIST7(EAL_T(text), par1, par2, par3, par4, par5, par6))
#define F_DBG7(text,par1,par2,par3,par4,par5,par6,par7) F_DBG_PARAMS(LIST8(EAL_T(text), par1, par2, par3, par4, par5, par6, par7))
#define F_DBG8(text,par1,par2,par3,par4,par5,par6,par7,par8) F_DBG_PARAMS(LIST9(EAL_T(text), par1, par2, par3, par4, par5, par6, par7, par8))
#define F_DBG9(text,par1,par2,par3,par4,par5,par6,par7,par8,par9) F_DBG_PARAMS(LIST10(EAL_T(text), par1, par2, par3, par4, par5, par6, par7, par8, par9))
#define F_DBG10(text,par1,par2,par3,par4,par5,par6,par7,par8,par9,par10) F_DBG_PARAMS(LIST11(EAL_T(text), par1, par2, par3, par4, par5, par6, par7, par8, par9, par10))


#define F_INFO_PARAMS(sprintflist) EL_CON_MSG_PARAMS(sprintflist)

#define F_INFO(text) F_INFO_PARAMS(LIST1(EAL_T(text)))
#define F_INFO1(text,par1) F_INFO_PARAMS(LIST2(EAL_T(text), par1))
#define F_INFO2(text,par1,par2) F_INFO_PARAMS(LIST3(EAL_T(text), par1, par2))
#define F_INFO3(text,par1,par2,par3) F_INFO_PARAMS(LIST4(EAL_T(text), par1, par2, par3))
#define F_INFO4(text,par1,par2,par3,par4) F_INFO_PARAMS(LIST5(EAL_T(text), par1, par2, par3, par4))
#define F_INFO5(text,par1,par2,par3,par4,par5) F_INFO_PARAMS(LIST6(EAL_T(text), par1, par2, par3, par4, par5))
#define F_INFO6(text,par1,par2,par3,par4,par5,par6) F_INFO_PARAMS(LIST7(EAL_T(text), par1, par2, par3, par4, par5, par6))
#define F_INFO7(text,par1,par2,par3,par4,par5,par6,par7) F_INFO_PARAMS(LIST8(EAL_T(text), par1, par2, par3, par4, par5, par6, par7))
#define F_INFO8(text,par1,par2,par3,par4,par5,par6,par7,par8) F_INFO_PARAMS(LIST9(EAL_T(text), par1, par2, par3, par4, par5, par6, par7, par8))
#define F_INFO9(text,par1,par2,par3,par4,par5,par6,par7,par8,par9) F_INFO_PARAMS(LIST10(EAL_T(text), par1, par2, par3, par4, par5, par6, par7, par8, par9))
#define F_INFO10(text,par1,par2,par3,par4,par5,par6,par7,par8,par9,par10) F_INFO_PARAMS(LIST11(EAL_T(text), par1, par2, par3, par4, par5, par6, par7, par8, par9, par10))


#endif