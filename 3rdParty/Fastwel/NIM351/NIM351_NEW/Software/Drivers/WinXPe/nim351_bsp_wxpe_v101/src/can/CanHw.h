#ifndef __CANHW_H__
#define __CANHW_H__

#include <can/can_a.h>

// Self test
// In this mode a full node test is possible without any other active node 
// on the bus using the self reception request command; 
// the CAN controller will perform a successful transmission, 
// even if there is no acknowledge received

//#define CAN_SELF_TEST_MODE

//-----------------------------------------------------------------------------
//--- Controller specific
//-----------------------------------------------------------------------------
#define CAN_REGISTER_STRIDE   1

////// Controller type
////typedef struct F_CAN_SETTINGS
////{
////  F_CAN_BAUDRATE BaudRate;
////  
////  // 1 - self test; theCAN controller will perform a successful
////  // transmission, even if there is no acknowledge received
////  // 0 - normal; an acknowledge is required for successful transmission
////  u8  self_test_mode;
////
////  // 1 - Upon self reception request a message is transmitted and simultaneously
////  // received if the acceptance filter is set to the corresponding identifier.
////  // 0 - normal
////  u8  self_reception_request;
////  
////  // 1 - listen only; in this mode the CAN controller would give no acknowledge
////  // to the CAN-bus, even if a message is received successfully.
////  // 0 - normal
////  u8  listen_only_mode;
////  
////  // 1 - single; the single acceptance filter option is
////  // enabled (one filter with the length of 32 bit is active)
////  // 0 - dual; the dual acceptance filter option is enabled
////  // (two filters, each with the length of 16 bit area)
////  u8  acceptance_filter_mode;
////  
////  // The acceptance filter is defined by the Acceptance Code Registers (acceptance_code_n)
////  // and the Acceptance Mask Registers (acceptance_mask_n). 
////  // The bit patterns of messages to be received are
////  // defined within the acceptance code registers.
////  // The corresponding acceptance mask registers allow to
////  // define certain bit positions to be ‘don’t care’. (p. 44 on spec)
////  u8  acceptance_code_0;
////  u8  acceptance_code_1;
////  u8  acceptance_code_2;
////  u8  acceptance_code_3;
////  u8  acceptance_mask_0;
////  u8  acceptance_mask_1;
////  u8  acceptance_mask_2;
////  u8  acceptance_mask_3;
////
////} F_CAN_SETTINGS, *PF_CAN_SETTINGS;
//-----------------------------------------------------------------------------
//--- CAN device struct
//-----------------------------------------------------------------------------
typedef struct F_CAN_DEVICE
{
  F_CAN_STATE         state;
  u8*                 reg_base;
  F_CAN_SETTINGS      settings;
  F_CAN_STATS         stats;
  F_CAN_STATUS        status;
  F_CAN_ERRORS        errors;
  BOOLEAN             transmitterBusy;

} F_CAN_DEVICE, *PF_CAN_DEVICE;

#endif