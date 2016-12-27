#ifndef __CAN_IO_H___
#define __CAN_IO_H___

//-------------------------------------------------------------------------------------------------------------
#define	CAN_DRV_CTL_TYPE	0xA001
#define IOCTL_BASE 0xA10

// Retrieves the current Baudrate, filter and mode settings for the specified CAN controller.
#define IOCTL_CAN_GET_DEVSETTINGS CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 0, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Sets the current Baudrate, filter and mode settings for the specified CAN controller.
#define IOCTL_CAN_SET_DEVSETTINGS	CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 1, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Retrieves the time-out parameters for all read and write operations.
#define IOCTL_CAN_GET_TIMEOUTS		CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 2, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Sets the time-out parameters for all read and write operations.
#define IOCTL_CAN_SET_TIMEOUTS    CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 3, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Retrieves bus status of the controller.
#define IOCTL_CAN_GET_DEVSTATUS   CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 4, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Discards all frames from the output or input buffer.
// It can also terminate pending read or write operations on the resource.
#define IOCTL_CAN_PURGE           CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 5, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Reset/Restart controller.
#define IOCTL_CAN_RESET_DEVICE    CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 6, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Retrieves the statistic values.
#define IOCTL_CAN_GET_STATS       CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 7, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Clear all statistic values.
#define IOCTL_CAN_CLEAR_STATS     CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 8, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Waits CAN event.
#define IOCTL_CAN_WAIT_ON_MASK    CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 9, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Peek message from the typeahead buffer.
#define IOCTL_CAN_PEEK_MSG        CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 10, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Post message in the controller transmit port.
#define IOCTL_CAN_POST_MSG        CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 11, METHOD_BUFFERED, FILE_ANY_ACCESS)
// Get and clear error counters.
#define IOCTL_CAN_GET_CLEAR_ERR   CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 12, METHOD_BUFFERED, FILE_ANY_ACCESS)

#define IOCTL_CAN_START_DEVICE    CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 13, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_CAN_STOP_DEVICE     CTL_CODE(CAN_DRV_CTL_TYPE, IOCTL_BASE + 14, METHOD_BUFFERED, FILE_ANY_ACCESS)
//-------------------------------------------------------------------------------------------------------------

// Device name used by library
#define CAN_DEVICE_NAME EAL_T("\\\\.\\nim351_0")

#define CAN_INDEX_INDEX 11


#endif