/*++ BUILD Version: 0001    // Increment this if a change has global effects

Copyright (c) 1992, 1993  Microsoft Corporation

Module Name:

    ntiologc.h

Abstract:

    Constant definitions for the I/O error code log values.

Author:

    Tony Ercolano (Tonye) 12-23-1992

Revision History:

--*/

#ifndef _CANLOG_
#define _CANLOG_

//
//  Status values are 32 bit values layed out as follows:
//
//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
//  +---+-+-------------------------+-------------------------------+
//  |Sev|C|       Facility          |               Code            |
//  +---+-+-------------------------+-------------------------------+
//
//  where
//
//      Sev - is the severity code
//
//          00 - Success
//          01 - Informational
//          10 - Warning
//          11 - Error
//
//      C - is the Customer code flag
//
//      Facility - is the facility code
//
//      Code - is the facility's status code
//

//
//  Values are 32 bit values layed out as follows:
//
//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
//  +---+-+-+-----------------------+-------------------------------+
//  |Sev|C|R|     Facility          |               Code            |
//  +---+-+-+-----------------------+-------------------------------+
//
//  where
//
//      Sev - is the severity code
//
//          00 - Success
//          01 - Informational
//          10 - Warning
//          11 - Error
//
//      C - is the Customer code flag
//
//      R - is a reserved bit
//
//      Facility - is the facility code
//
//      Code - is the facility's status code
//
//
// Define the facility codes
//
#define FACILITY_RPC_STUBS               0x3
#define FACILITY_RPC_RUNTIME             0x2
#define FACILITY_IO_ERROR_CODE           0x4
#define FACILITY_FWCAN_ERROR_CODE        0x6


//
// Define the severity codes
//
#define STATUS_SEVERITY_WARNING          0x2
#define STATUS_SEVERITY_SUCCESS          0x0
#define STATUS_SEVERITY_INFORMATIONAL    0x1
#define STATUS_SEVERITY_ERROR            0x3


//
// MessageId: FWCAN_KERNEL_DEBUGGER_ACTIVE
//
// MessageText:
//
//  %2.
//
#define FWCAN_KERNEL_DEBUGGER_ACTIVE     ((NTSTATUS)0x40060001L)

//
// MessageId: FWCAN_NO_SYMLINK_CREATED
//
// MessageText:
//
//  Unable to create the symbolic link for %2.
//
#define FWCAN_NO_SYMLINK_CREATED         ((NTSTATUS)0x80060002L)

//
// MessageId: FWCAN_NO_DEVICE_MAP_CREATED
//
// MessageText:
//
//  Unable to create the device map entry for %2.
//
#define FWCAN_NO_DEVICE_MAP_CREATED      ((NTSTATUS)0x80060003L)

//
// MessageId: FWCAN_NO_DEVICE_MAP_DELETED
//
// MessageText:
//
//  Unable to delete the device map entry for %2.
//
#define FWCAN_NO_DEVICE_MAP_DELETED      ((NTSTATUS)0x80060004L)

//
// MessageId: FWCAN_INSUFFICIENT_RESOURCES
//
// MessageText:
//
//  Not enough resources were available for the driver.
//
#define FWCAN_INSUFFICIENT_RESOURCES     ((NTSTATUS)0xC0060005L)

//
// MessageId: FWCAN_REGISTERS_NOT_MAPPED
//
// MessageText:
//
//  The hardware locations for %2 could not be translated to something the memory management system could understand.
//
#define FWCAN_REGISTERS_NOT_MAPPED       ((NTSTATUS)0xC0060006L)

//
// MessageId: FWCAN_NO_BUFFER_ALLOCATED
//
// MessageText:
//
//  No memory could be allocated in which to place new data for %2.
//
#define FWCAN_NO_BUFFER_ALLOCATED        ((NTSTATUS)0xC0060007L)

//
// MessageId: FWCAN_DEVICE_NOT_FOUND
//
// MessageText:
//
//  While validating that %2 was really a fwCAN port, probe chip failed.
//  The device is assumed not to be a fwCAN port and will be deleted.
//
#define FWCAN_DEVICE_NOT_FOUND           ((NTSTATUS)0xC0060008L)

//
// MessageId: FWCAN_INVALID_USER_CONFIG
//
// MessageText:
//
//  User configuration for parameter %2 must have %3.
//
#define FWCAN_INVALID_USER_CONFIG        ((NTSTATUS)0xC0060009L)

//
// MessageId: FWCAN_DISABLED_PORT
//
// MessageText:
//
//  Disabling %2 as requested by the configuration data.
//
#define FWCAN_DISABLED_PORT              ((NTSTATUS)0x4006000AL)

//
// MessageId: FWCAN_NO_TRANSLATE_PORT
//
// MessageText:
//
//  Could not translate the user reported I/O port for %2.
//
#define FWCAN_NO_TRANSLATE_PORT          ((NTSTATUS)0xC006000BL)

//
// MessageId: FWCAN_NO_GET_INTERRUPT
//
// MessageText:
//
//  Could not get the user reported interrupt for %2 from the HAL.
//
#define FWCAN_NO_GET_INTERRUPT           ((NTSTATUS)0xC006000CL)

//
// MessageId: FWCAN_NO_DEVICE_REPORT
//
// MessageText:
//
//  Could not report the discovered legacy device %2 to the IO subsystem.
//
#define FWCAN_NO_DEVICE_REPORT           ((NTSTATUS)0xC006000DL)

//
// MessageId: FWCAN_REGISTRY_WRITE_FAILED
//
// MessageText:
//
//  Error writing to the registry.
//
#define FWCAN_REGISTRY_WRITE_FAILED      ((NTSTATUS)0xC006000EL)

//
// MessageId: FWCAN_NO_DEVICE_REPORT_RES
//
// MessageText:
//
//  Could not report device %2 to IO subsystem due to a resource conflict.
//
#define FWCAN_NO_DEVICE_REPORT_RES       ((NTSTATUS)0xC006000FL)

//
// MessageId: FWCAN_HARDWARE_FAILURE
//
// MessageText:
//
//  The fwCAN driver detected a hardware failure on device %2 and will disable this device.
//
#define FWCAN_HARDWARE_FAILURE           ((NTSTATUS)0xC0060010L)

#endif /* _NTIOLOGC_ */
