/**
 *
 * NAME:    can/can.h
 *
 * DESCRIPTION: ���� ������ � ����� ��� ������ � CAN-����������
 *
 *
 * AUTHOR:  ������� ����������
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group Co Ltd.
 *      All rights reserved.
 ********************************************************/

#ifndef __CAN_H__
#define __CAN_H__

#include <eal/el.h>

// -------------------------------------------------------------------------
/**
 *  ���� ������ � �����, ����������� � CAN-���������
 */

// ��������� ������������/������������ ������� �����
#define CAN_EFF_FLAG 0x80000000U  // EFF/SFF is set in the MSB
// ��������� RTR-�����
#define CAN_RTR_FLAG 0x40000000U  // remote transmission request

// ������� ����� ��� �������� ��� � ������ ������������
// � ������������ ��������
#define CAN_SFF_MASK 0x000007FFU  // standard frame format (SFF)
#define CAN_EFF_MASK 0x1FFFFFFFU  // extended frame format (EFF)

// CAN-���������
typedef struct _F_CAN_MSG
{
  // CAN-ID, ������� ����� EFF/RTR/ERR
  u32 can_id;
  // ���-�� ������ � ����� (�� 0 �� 8)
  u8  can_dlc;
  // ���� ������ �����
  u8  data[8];

} F_CAN_MSG, *PF_CAN_MSG;


// �������� (�����������) ���������
typedef struct _F_CAN_RX
{
  // ����� ������� ��������� � �������������.
  // ����� ���������� � ������� �������� �������� � ������������� 
  // ����� �������� ������ �������� 71 ������.
  u32         timestamp;
  // "������� �����" (���� ���������)
  F_CAN_MSG   msg;

} F_CAN_RX, *PF_CAN_RX;

// ��������� (������������) ���������
typedef struct _F_CAN_TX
{
  // "������� �����" (���� ���������)
  F_CAN_MSG   msg;

} F_CAN_TX, *PF_CAN_TX;

// -------------------------------------------------------------------------
/**
 *  ���� ��������� CAN-��������
 */
typedef enum _F_CAN_STATE
{
  // ��������� ��������� ��������
	CAN_STATE_INIT = 0,
  // ���-�� ������ ������/�������� �� ����� 96
  CAN_STATE_ERROR_ACTIVE,
  // ���-�� ������ ������/�������� �� 96 �� 127
	CAN_STATE_ERROR_WARNING,
  // ���-�� ������ ������/�������� �� 128 �� 255
	CAN_STATE_ERROR_PASSIVE,
  // ���-�� ������ �������� >=256 (BUSOFF)
	CAN_STATE_BUS_OFF,
  // ������� ����������
	CAN_STATE_STOPPED,
  // ��������� ���, �� ������������ � ������� ������
	CAN_STATE_SLEEPING

} F_CAN_STATE, *PF_CAN_STATE;

// -------------------------------------------------------------------------
/**
 *  �������� ������ ������� CAN-��������
 */

#define CAN_STATUS_RXBUF  0x01  // 1 - ���� � ����� �������� ���������
                                // �������� ��� ������
                                // 0 - �����; ��� �������� ���������
                                  
#define CAN_STATUS_TXBUF  0x02  // 1 - ���������� ����� �������� CAN-�������� ��������;
                                // �������� ������ � ����� ��������
                                // 0 - ���������� ����� �������� CAN-�������� �����;
                                // ��������� ��������� ������� ��������
                                // ��� ��������� � �������� ��������
                                
#define CAN_STATUS_ERR    0x04  // 1 - ��������� ������ � ������� ����������
                                // ������ �������� ������
                                // 0 - � ������� ���������� ������ ���������
                                // ������ ����� ������ �� ���������.

// ������ CAN-��������
typedef size_t F_CAN_STATUS;

// ����� ��������� �������
#define F_CAN_SetStatus(st, flags)   (st) |= (flags)
#define F_CAN_ClearStatus(st, flags)   (st) &= ~(flags)

// -------------------------------------------------------------------------
/**
 *  �������� ������ ����� ���������� ������� CAN-��������: fw_can_wait()
 */

// �������� ������� CAN_STATUS_RXBUF
// �������� ������� � ������ ������ ���� �� ������ ���������
#define CAN_WAIT_RX       CAN_STATUS_RXBUF

// �������� ������� CAN_STATUS_TXBUF
// �������� ������������ ����������� ������ �������� CAN-��������
#define CAN_WAIT_TX       CAN_STATUS_TXBUF

// �������� ������� CAN_STATUS_ERR
// �������� ����� ������
#define CAN_WAIT_ERR      CAN_STATUS_ERR

// -------------------------------------------------------------------------
/**
 *  �������� ������ ������/���������� �������� ��� fw_can_purge()
 */

// �������� �������/���������� ��������� � �������� �� ������
#define CAN_PURGE_TXABORT       0x0001
// �������� �������/���������� ��������� � �������� �� ������
#define CAN_PURGE_RXABORT       0x0002
// �������� ���������� ����� �������� �������� ������� ��������
#define CAN_PURGE_TXCLEAR       0x0004
// �������� ���������� ����� ������ �������� ������� ��������
#define CAN_PURGE_RXCLEAR       0x0008
// ���������� ����� ��������
#define CAN_PURGE_HWRESET       0x0010

// ����� ��� fw_can_purge()
typedef size_t F_CAN_PURGE_MASK;

// -------------------------------------------------------------------------
/**
 *  ���������� ���������������� ��������
 */
typedef struct _F_CAN_STATS
{
  // ���-�� �������� ������
  u32 rx_packets;

  // ���-�� �������� ����
  u32 rx_bytes;

  // ���-�� ���������� ������
  u32 tx_packets;

  // ���-�� ���������� ����
  u32 tx_bytes;

  // ���-�� ������ ������
  u32 rx_errors;

  // ���-�� ������������ ������ ������
  u32 rx_over_errors;

  // ���-�� ������ ��������
  u32 tx_errors;

  // ���-�� ������������ ������ ��������
  u32 tx_over_errors;

  // ���-�� ������ �� ���� (������������, CRC, etc.)
  u32 bus_error;

  // ���-�� ��������� � CAN_STATE_ERROR_WARNING
	u32 error_warning;

  // ���-�� ��������� � CAN_STATE_ERROR_PASSIVE
	u32 error_passive;

  // ���-�� ��������� � CAN_STATE_BUS_OFF
	u32 bus_off;

  // ���-�� "����������" ���������
	u32 arbitration_lost;

  // ���-�� ������������ �����������
	u32 restarts;

} F_CAN_STATS, *PF_CAN_STATS;


// -------------------------------------------------------------------------
/**
 *  �������� ������ �������� ��� fw_can_get_clear_errors()
 */

typedef struct _F_CAN_ERRORS
{
  // ���-�� ��������� ��������
  size_t tx_timeout;

  // ���-�� ������������ ������ ������
  size_t data_overrun;

  // ���-�� ��������� � CAN_STATE_ERROR_PASSIVE
	size_t error_passive;

  // ���-�� ��������� � CAN_STATE_BUS_OFF
	size_t bus_off;

} F_CAN_ERRORS, *PF_CAN_ERRORS;

// -------------------------------------------------------------------------
/**
 *  �������� ������
 */
typedef enum _F_CAN_BAUDRATE
{
  CANBR_1MBaud    = 0,  // 1 MBit/sec
  CANBR_800kBaud  = 1,  // 800 kBit/sec
  CANBR_500kBaud  = 2,  // 500 kBit/sec
  CANBR_250kBaud  = 3,  // 250 kBit/sec
  CANBR_125kBaud  = 4,  // 125 kBit/sec
  CANBR_100kBaud  = 5,  // 100 kBit/sec
  CANBR_50kBaud   = 6,  // 50 kBit/sec
  CANBR_20kBaud   = 7,  // 20 kBit/sec
  CANBR_10kBaud   = 8   // 10 kBit/sec
} F_CAN_BAUDRATE;

// -------------------------------------------------------------------------
/**
 *  �������� �������� �� ������ � ��������, ������������ ���������:
 *  fw_can_get_timeouts()
 *  fw_can_set_timeouts()
 *  fw_can_recv()
 *  fw_can_send()
 *  
 *  �������� �������� �������� �������� fw_can_send() ������������ ��������:
 *  Tsend = N * WriteTotalTimeoutMultiplier + WriteTotalTimeoutConstant (��),
 *  ��� N -- ���-�� ���������, ������������ ���������� nTx.
 *  
 *  ���� WriteTotalTimeoutMultiplier � WriteTotalTimeoutConstant ����� 0,
 *  �������� fw_can_send() ����� �����������.
 *
 *  ���� ��� ������ fw_can_recv() � ������ ������ ������� ���� �� ���� ���������,
 *  fw_can_recv() ��������� �� � ���������� ����������.
 *  ���� ��� ������ fw_can_recv() � ������ ������ ��� �� ������ ���������, ��
 *  ��� ������ ������� �� ��������� fw_can_recv() ��������� ��� � ����������
 *  ����������.
 *  ���� ��� ������ fw_can_recv() � ������ ������ ��� �� ������ ���������, � �����
 *  ��� ����� �������� �� ���� ReadTotalTimeout, �� ��� ���������� �������� ���������
 *  � ������� ReadTotalTimeout (��) ������� fw_can_recv() ���������� ���������.
 */

typedef struct _F_CAN_TIMEOUTS
{
  // ��������� ��� ���������� �������� ��������
  u32 WriteTotalTimeoutMultiplier;

  // ��������� ��� ���������� �������� ��������
  u32 WriteTotalTimeoutConstant;

  // ������� ������ (� ��) �������� fw_can_recv()
  u32 ReadTotalTimeout;

  // ������� ��������������� �������������� �� ��������� bus-off (CAN_STATE_BUS_OFF)
  u32 RestartBusoffTimeout;

} F_CAN_TIMEOUTS, *PF_CAN_TIMEOUTS;


// -------------------------------------------------------------------------
/**
 *  ��������� ��������
 */

// ����� ������ ������:
// ���������� ��������� ������������ ������� (11-������� CAN-ID)
#define CAN_OPMODE_STANDARD 0x0001
// ����������� ��������� ������������ ������� (29-������� CAN-ID)
#define CAN_OPMODE_EXTENDED 0x0002

// ��������� ������ ����������� ������������ CAN-���������
#define CAN_OPMODE_ERRFRAME 0x0004

// ���������������� � ������ "Listen Only" (���������� ����), ��� �������
// ������� �� �������� ������� �������������, ���� ���� ��������� ������� �������.
#define CAN_OPMODE_LSTNONLY 0x0008

// ������� ����� ��������� ��������, ���� ��� ���������� ������������� �� ����
#define CAN_OPMODE_SELFTEST 0x0010

// ���������, ��������������� ��������� acceptance-�������, ����� ������������
// ������������ � ����������� ���������.
#define CAN_OPMODE_SELFRECV 0x0020

// ��� �������� CAN
typedef enum _F_CAN_CONTROLLER
{
  UNKNOWN_CAN_DEVICE,
  PHILIPS_SJA_1000,
} F_CAN_CONTROLLER;

// ���������, ������������ � fw_can_get_controller_config() �
// fw_can_set_controller_config().
typedef struct _F_CAN_SETTINGS
{
  // ��� �������� (������ ������)
  F_CAN_CONTROLLER  controller_type;

  // �������� ������
  F_CAN_BAUDRATE    baud_rate;
  
  // ����� ������ ������
  u16               opmode;

  // ������ ����������� CAN-ID-�� �������� (�����������) ���������.
  // ������������ ������ acceptance_code � acceptance_mask.

  // ������� ������� ��������������� ���������, ���������� ������
  u32               acceptance_code;
  // �����, ����������� ������� "����������" ������� ������� ��� �������� 
  // ������������ �������� ��������
  u32               acceptance_mask;

  // �����, ����������� ����������� ��������� ������, ������������ ������������
  // ����������� �� ������� ��� ������ � ������ CAN_OPMODE_ERRFRAME.
  u32               error_mask;

} F_CAN_SETTINGS, *PF_CAN_SETTINGS;


// -------------------------------------------------------------------------
/**
 *  ����������� ��������� �� ������� � ������ CAN_OPMODE_ERRFRAME
 */
//-----------------------------------------------------------------------------

// ����� ���� ������ ��������� �� ������
#define CAN_ERR_DLC 8 /* dlc for error frames */

// ������ ������������ ������

// ����������� ������ :)
#define CAN_ERR_UNSPEC       0x00000000U /* unspecified */
// ������� ��������
#define CAN_ERR_TX_TIMEOUT   0x00000001U /* TX timeout / data[0..3] */
// �������� ��������� (data[0])
#define CAN_ERR_LOSTARB      0x00000002U /* lost arbitration    / data[0]    */
// ���������� ���� �������� (data[1])
#define CAN_ERR_CRTL         0x00000004U /* controller problems / data[1]    */
// ��������� ��������� (data[2..3])
// ��������! ����� ��������� ����� ����� � �����!
#define CAN_ERR_PROT         0x00000008U /* protocol violations (may flood!) / data[2..3] */
// ������ ���������� (data[4]) ???
#define CAN_ERR_TRX          0x00000010U /* transceiver status  / data[4]    */
// �� ������� ACK �� ���������� ���������
#define CAN_ERR_ACK          0x00000020U /* received no ACK on transmission */
// Bus Off
#define CAN_ERR_BUSOFF       0x00000040U /* bus off */
// ������ �� ���� 
// ��������! ����� ��������� ����� ����� � �����!
#define CAN_ERR_BUSERROR     0x00000080U /* bus error (may flood!) */
// ������� �����������
#define CAN_ERR_RESTARTED    0x00000100U /* controller restarted */

/* TX timeout (by netdevice driver) / data[0..3] */
#define CAN_ERR_TX_TIMEOUT_UNSPEC 0x00000000U /* unspecified */
				      /* else can_id */

/* arbitration lost in bit ... / data[0] */
#define CAN_ERR_LOSTARB_UNSPEC   0x00 /* unspecified */
				      /* else bit number in bitstream */

/* error status of CAN-controller / data[1] */
#define CAN_ERR_CRTL_UNSPEC      0x00 /* unspecified */
#define CAN_ERR_CRTL_RX_OVERFLOW 0x01 /* RX buffer overflow */
#define CAN_ERR_CRTL_TX_OVERFLOW 0x02 /* TX buffer overflow */
#define CAN_ERR_CRTL_RX_WARNING  0x04 /* reached warning level for RX errors */
#define CAN_ERR_CRTL_TX_WARNING  0x08 /* reached warning level for TX errors */
#define CAN_ERR_CRTL_RX_PASSIVE  0x10 /* reached error passive status RX */
#define CAN_ERR_CRTL_TX_PASSIVE  0x20 /* reached error passive status TX */
				      /* (at least one error counter exceeds */
				      /* the protocol-defined level of 127)  */

/* error in CAN protocol (type) / data[2] */
#define CAN_ERR_PROT_UNSPEC      0x00 /* unspecified */
#define CAN_ERR_PROT_BIT         0x01 /* single bit error */
#define CAN_ERR_PROT_FORM        0x02 /* frame format error */
#define CAN_ERR_PROT_STUFF       0x04 /* bit stuffing error */
#define CAN_ERR_PROT_BIT0        0x08 /* unable to send dominant bit */
#define CAN_ERR_PROT_BIT1        0x10 /* unable to send recessive bit */
#define CAN_ERR_PROT_OVERLOAD    0x20 /* bus overload */
#define CAN_ERR_PROT_ACTIVE      0x40 /* active error announcement */
#define CAN_ERR_PROT_TX          0x80 /* error occured on transmission */

/* error in CAN protocol (location) / data[3] */
#define CAN_ERR_PROT_LOC_UNSPEC  0x00 /* unspecified */
#define CAN_ERR_PROT_LOC_SOF     0x03 /* start of frame */
#define CAN_ERR_PROT_LOC_ID28_21 0x02 /* ID bits 28 - 21 (SFF: 10 - 3) */
#define CAN_ERR_PROT_LOC_ID20_18 0x06 /* ID bits 20 - 18 (SFF: 2 - 0 )*/
#define CAN_ERR_PROT_LOC_SRTR    0x04 /* substitute RTR (SFF: RTR) */
#define CAN_ERR_PROT_LOC_IDE     0x05 /* identifier extension */
#define CAN_ERR_PROT_LOC_ID17_13 0x07 /* ID bits 17-13 */
#define CAN_ERR_PROT_LOC_ID12_05 0x0F /* ID bits 12-5 */
#define CAN_ERR_PROT_LOC_ID04_00 0x0E /* ID bits 4-0 */
#define CAN_ERR_PROT_LOC_RTR     0x0C /* RTR */
#define CAN_ERR_PROT_LOC_RES1    0x0D /* reserved bit 1 */
#define CAN_ERR_PROT_LOC_RES0    0x09 /* reserved bit 0 */
#define CAN_ERR_PROT_LOC_DLC     0x0B /* data length code */
#define CAN_ERR_PROT_LOC_DATA    0x0A /* data section */
#define CAN_ERR_PROT_LOC_CRC_SEQ 0x08 /* CRC sequence */
#define CAN_ERR_PROT_LOC_CRC_DEL 0x18 /* CRC delimiter */
#define CAN_ERR_PROT_LOC_ACK     0x19 /* ACK slot */
#define CAN_ERR_PROT_LOC_ACK_DEL 0x1B /* ACK delimiter */
#define CAN_ERR_PROT_LOC_EOF     0x1A /* end of frame */
#define CAN_ERR_PROT_LOC_INTERM  0x12 /* intermission */

/* error status of CAN-transceiver / data[4] */
/*                                             CANH CANL */
#define CAN_ERR_TRX_UNSPEC             0x00 /* 0000 0000 */
#define CAN_ERR_TRX_CANH_NO_WIRE       0x04 /* 0000 0100 */
#define CAN_ERR_TRX_CANH_SHORT_TO_BAT  0x05 /* 0000 0101 */
#define CAN_ERR_TRX_CANH_SHORT_TO_VCC  0x06 /* 0000 0110 */
#define CAN_ERR_TRX_CANH_SHORT_TO_GND  0x07 /* 0000 0111 */
#define CAN_ERR_TRX_CANL_NO_WIRE       0x40 /* 0100 0000 */
#define CAN_ERR_TRX_CANL_SHORT_TO_BAT  0x50 /* 0101 0000 */
#define CAN_ERR_TRX_CANL_SHORT_TO_VCC  0x60 /* 0110 0000 */
#define CAN_ERR_TRX_CANL_SHORT_TO_GND  0x70 /* 0111 0000 */
#define CAN_ERR_TRX_CANL_SHORT_TO_CANH 0x80 /* 1000 0000 */

//-----------------------------------------------------------------------------

#endif
