/**
 *
 * NAME:    idbg.h
 *
 * DESCRIPTION: Turns off F_DBG() printing
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

#ifdef FMSG_ENABLE_DEBUG
  # undef FMSG_ENABLE_DEBUG
#endif
# include <eal/debug/dbgredef.h>
