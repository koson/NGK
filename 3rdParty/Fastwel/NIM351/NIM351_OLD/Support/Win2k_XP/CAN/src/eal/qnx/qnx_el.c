/*
 *
 * NAME:    qnx_el.c
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


#ifdef __QNX_63__

#include <eal/el.h>

F_STR el_path_separator = EAL_T("/");

unsigned int el_get_tick_count(void)
{
  struct timespec clocks;
  clock_gettime(CLOCK_REALTIME, &clocks);
  
  return (timespec2nsec(&clocks)) / 1000000;
}

FILE_HANDLE el_load_driver(F_STR device_name_prefix, 
                           F_STR device_namespace,
                           F_STR driver_module_path,
                           size_t device_index, 
                           F_STR command_line)
{
  return NULL;
}

bool  el_unload_driver(FILE_HANDLE drv_handle)
{
  return false;
}


#endif