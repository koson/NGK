#ifndef __CAN_A_H__
#define __CAN_A_H__

#include <can/can.h>

typedef struct _F_CAN_WAIT_PARAM
{
  F_CAN_STATUS      mask;
  size_t            msec;
} F_CAN_WAIT_PARAM, *PF_CAN_WAIT_PARAM;

typedef struct _F_CAN_STATUS_T
{
  F_CAN_STATUS      status;
  F_CAN_STATE       state;
  F_TIME            taking;
} F_CAN_STATUS_T, *PF_CAN_STATUS_T;

#endif