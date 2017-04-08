/**
 *
 * NAME:    dbgredef.h
 *
 * DESCRIPTION: Debug printing re-definition helper
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

#undef F_DBG_PARAMS

# ifdef FMSG_ENABLE_DEBUG
#   define F_DBG_PARAMS(sprintflist) EL_CON_MSG_PARAMS(sprintflist)
# else
#   define F_DBG_PARAMS(sprintflist)
# endif

