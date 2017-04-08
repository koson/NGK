/**
 *
 * NAME:    can/can_lib.h
 *
 * DESCRIPTION: ���������� ��������� �������� CAN-��������
 *
 *
 * AUTHOR:  ������� ����������
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group Co Ltd.
 *      All rights reserved.
 ********************************************************/
#ifndef ___CAN_LIB_H___
#define ___CAN_LIB_H___

#include <can/can.h>

//----------------------------------------------------------------------------------------------------
// ��������� ���������� ������� ����������
typedef enum _F_CAN_RESULT
{
  // �����
  CAN_RES_OK = 0,
  // ���������� ������
  CAN_RES_HARDWARE,
  // ���������������� (������������) ����� ��������
  CAN_RES_INVALID_HANDLE,
  // ������������ ���������, ���������� � �������� ���������
  CAN_RES_INVALID_POINTER,
  // ������������ ��������, ���������� � ������� (���� ��� ���������)
  CAN_RES_INVALID_PARAMETER,
  // �� ������� ��������� ��������
  CAN_RES_INSUFFICIENT_RESOURCES,
  // �� ������� ������� ����������
  CAN_RES_OPEN_DEVICE,
  // ���������� ������ � �������� ����������
  CAN_RES_UNEXPECTED,
  // ������ ��������� � �������� ��� � ����������
  CAN_RES_FAILURE,
  // ����� ������ ����
  CAN_RES_RXQUEUE_EMPTY,
  // �������� �� ��������������
  CAN_RES_NOT_SUPPORTED,
  // �������
  CAN_RES_TIMEOUT

} F_CAN_RESULT;

/**
 *  �������� ���������� ���������� ��������� ��������
 */
#define F_CAN_SUCCESS(res) (CAN_RES_OK == (res))
//----------------------------------------------------------------------------------------------------


// ������������� (������) CAN-��������. ��� ��������� ���������������
// ���� ���������� ����������, �������������� ������������ ���������� CAN.
typedef size_t F_CAN_DEVID;

// ����� ��������
typedef FILE_HANDLE F_CAN_HANDLE, *PF_CAN_HANDLE;

// ���������, ������������ ������ ��� �������� ������� CAN-�������� �������� fw_can_wait()
typedef struct _F_CAN_WAIT
{
  // ����� ���������� ������� CAN-�������� (�� ���), �������:
  // CAN_WAIT_RX -- � �������� ������ ���� ������������� ���������
  // CAN_WAIT_TX -- � ����� �������� ��� �������������� ��������� 
  // CAN_WAIT_ERR -- � ������� ���������� ������ fw_can_get_clear_errors() ��������� ������(�)
  F_CAN_STATUS  waitMask;

  // ����� �������� ������� CAN-��������
  F_CAN_STATUS  status;

} F_CAN_WAIT, *PF_CAN_WAIT;

#ifdef __cplusplus
extern "C"
{
#endif

/**
 *  �������� ������������ (������������) ������ ����������
 */
  #define fw_can_is_handle_valid(handle) el_handle_valid(handle)

//----------------------------------------------------------------------------------------------------
/** 
 *  Function		 :  F_CAN_RESULT fw_can_init(void)
 *
 *  Parameters	 : 
 *
 *  Return value :  ��������� ������������� ���������� ���������
 *									CAN_RES_OK    - �����
 *                  CAN_RES_XX    - ��������, �������� �� CAN_RES_OK, ��������������� �� ������
 *
 *  Description	 :  �������������� ���������� ��������� ��� ����������� ��������.
 *                  ������ ������� ������ ���� ������� � ������ ���������, � ������� ��������������
 *                  ����������������� � ���������.
 *                 
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_init(void);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function		 :  F_CAN_RESULT fw_can_open(F_CAN_DEVID id, PF_CAN_HANDLE phDev)
 *
 *  Parameters	 :  id - ������������� (������) ��������, ������� � 1
 *
 *                  phDev - ��������� �� ���������� ���� F_CAN_HANDLE, � �������, ��� ��������
 *                  �������� ��������, �������� ����� ����������. ������������ ������ ����� ���������
 *                  ��� ������ ������� fw_can_is_handle_valid:
 *                  PF_CAN_HANDLE handle;
 *                  if(CAN_RES_OK == fw_can_open(1, &handle, true) && fw_can_is_handle_valid(handle))
 *                  {
 *                    //���������� � �������� 1 ������� �������
 *                    fw_can_close(handle);
 *                  }
 *
 *  Return value : 
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description	 :  ������� ��������� ������� � �������� ��������������� (��������).
 *                 
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_open(F_CAN_DEVID id, PF_CAN_HANDLE phDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function	   :  F_CAN_RESULT fw_can_close(F_CAN_HANDLE hDev)
 *
 *  Parameters	 :  hDev - ����� ������������ ��������
 *
 *  Return value : 
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description	 :  ��������� ����� ��������, ����� �������� ��� ������ fw_can_open().
 *                  ����� ������� ������ hDev �� ����� �������������� ��� ������� � ��������.
 **/
  FDLL_EXPORT F_CAN_RESULT  fw_can_close(F_CAN_HANDLE hDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_controller_config(F_CAN_HANDLE hDev, F_CAN_SETTINGS pDcb)
 *
 *  Parameters   :  hDev - ����� ��������
 *
 *               :  pDcb - ��������� �� ��������� ���������� CAN-��������
 *
 *  Return value : 
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ���������� ������� ��������� CAN-��������.
 **/
   FDLL_EXPORT F_CAN_RESULT fw_can_get_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_set_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb)
 *
 *  Parameters   :  hDev - ����� ��������
 *
 *               :  pDcb - ��������� �� ��������� ���������� CAN-��������
 *
 *  Return value : 
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ������ ����� �������� ���������� CAN-��������. ��������� ����������
 *                  �������� ������, ����� ������� ��������� � ��������� CAN_STATE_INIT.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_set_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
 *
 *  Parameters   :  hDev - ����� ��������
 *
 *               :  pTimeouts - ��������� �� ��������� ��������� CAN-��������
 *
 *  Return value : 
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ���������� ������� �������� ��������� �������� ������, ������ � ������ ��������� CAN-��������.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_get_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_set_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
 *
 *  Parameters   :  hDev - ����� ��������
 *
 *               :  pTimeouts - ��������� �� ��������� ��������� CAN-��������
 *
 *  Return value : 
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ������������� ����� �������� ��������� �������� ������, ������ � ������ ��������� CAN-��������.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_set_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_controller_state(F_CAN_HANDLE hDev, PF_CAN_STATE pState)
 *
 *  Parameters   :  hDev - ����� ��������
 *
 *               :  pState - ��������� �� ����������, � ������� ����� ���������� ������� ��������� ��������,
 *                  �������:
 *                  CAN_STATE_INIT - ��������� ���������
 *                  CAN_STATE_ERROR_ACTIVE - ���-�� ������ ������/�������� �� ����� 96
 *                  CAN_STATE_ERROR_WARNING	- ���-�� ������ ������/�������� �� 96 �� 127
 *                  CAN_STATE_ERROR_PASSIVE -	���-�� ������ ������/�������� �� 128 �� 255
 *                  CAN_STATE_BUS_OFF -	���-�� ������ ������/�������� >= 256 (BUSOFF)
 *                  CAN_STATE_STOPPED - ������� ����������
 *                  CAN_STATE_SLEEPING - �� ������������ � ������� ������
 *
 *  Return value : 
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ���������� ������� ��������� CAN-��������
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_get_controller_state(F_CAN_HANDLE hDev, PF_CAN_STATE pState);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_stats(F_CAN_HANDLE hDev, PF_CAN_STATS pStats)
 *
 *  Parameters   :  hDev - ����� ��������
 *               :  pDcb - ��������� �� ��������� �������������� ���������� CAN-��������
 *
 *  Return value : 
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ���������� �������������� ���������� ��������� ��������.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_get_stats(F_CAN_HANDLE hDev, PF_CAN_STATS pStats);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_clear_stats(F_CAN_HANDLE hDev)
 *
 *  Parameters   :  hDev - ����� ��������
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ���������� � 0 �������������� ���������� ��������� ��������.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_clear_stats(F_CAN_HANDLE hDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_start(F_CAN_HANDLE hDev)
 *
 *  Parameters   :  hDev - ����� ������������ ��������
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ��������� �������� �������. ��� �������� ������� ������� �����������
 *                  ����������� � ����� CAN � ������� �� ��������� ��������� CAN_STATE_INIT.
 *                 
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_start(F_CAN_HANDLE hDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_stop(F_CAN_HANDLE hDev)
 *
 *  Parameters   :  hDev - ����� ���������������� ��������
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ������������� �������� �������. ��� �������� �������� ������� �������������
 *                  �� ���� CAN. ����� ���������� ������� ������� ��������� � ��������� CAN_STATE_INIT.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_stop(F_CAN_HANDLE hDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_send(F_CAN_HANDLE hDev, PF_CAN_TX pTx, size_t nTx, size_t* nSend)
 *
 *  Parameters   :  hDev - ����� ��������
 *                  pTx - ��������� �� ����� ������������ ���������
 *                  nTx - ���������� ��������� � ������ pTx
 *                  nSend - ��������� �� ����������, � ������� ����� �������� ���������� ������� ����������
 *                  ��������� ��� �������� �� ������ �������.
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  �������� ��������� � ���� CAN.
 *                  ������� ����������� �� ��� ���, ���� ��������� �� ����� ��������, ���� ����
 *                  �� ���������� ������� ��������. ������� �������� ������ ���� �������������� �����
 *                  ��� ������ fw_can_set_timeouts().
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_send(F_CAN_HANDLE hDev, PF_CAN_TX pTx, size_t nTx, size_t* nSend);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* nRecv)
 *
 *  Parameters   :  hDev - ����� ��������
 *                  pRx - ��������� �� ����� ������ � ����������
 *                  nRx - ������� ������ ������
 *                  nRecv - ��������� �� ����������, � ������� ����� �������� ���������� �������� ���������,
 *                  ����������� � ������ pRx ��� �������� �� ������ �������.
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ��� ������ ������ ������� ���������� ���������� ��������� ��������� �� ���������� ������
 *                  �������� ��� ������� ��������. ���� ���������� ����� ����, ������� ����������� � ������� 
 *                  ������ ���� �� ������ ��������� ��� �������� ������. ������� ������ ������ ���� �������������� 
 *                  ����� ��� ������ fw_can_set_timeouts().
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* nRecv);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_peek_message(F_CAN_HANDLE hDev, PF_CAN_RX pRx)
 *
 *  Parameters   :  hDev - ����� ��������
 *                  pRx - ��������� �� ���������� F_CAN_RX, � ������� ����� �������� ���������
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *                  CAN_RES_RXQUEUE_EMPTY - ��� ��������� �� ���������� ������ ������ �������� ���
 *                  ������� ��������.
 *
 *  Description  :  ��� ������ ������ ������� ���������� ���������� ������ ��������� �� �����������
 *                  ������ ������. ������� �� ����������� � ���������� ����������� ��� ����� ��������� ������.
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_peek_message(F_CAN_HANDLE hDev, PF_CAN_RX pRx);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_post_message(F_CAN_HANDLE hDev, PF_CAN_TX pTx)
 *
 *  Parameters   :  hDev - ����� ��������
 *                  pTx - ��������� �� ����������-���������, ���������� ��������
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *                  CAN_RES_TXQUEUE_FULL - ������������ ������ ��������.
 *
 *  Description  :  ������� �������� ��������� �� ���������� ����� �������� ��������.
 *                  ������� �� ����������� � �� ������� ���������� �������� ���������.
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_post_message(F_CAN_HANDLE hDev, PF_CAN_TX pTx);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_purge(F_CAN_HANDLE hDev, F_CAN_PURGE_MASK flags)
 *
 *  Parameters   :  hDev - ����� ��������
 *                  flags - ����� ������/���������� ������� �������� ��� ���������
 *                  CAN_PURGE_TXABORT - ��������� ���������� ��������� � �������� �� ������.
 *                  CAN_PURGE_TXCLEAR - ���������� � �������� ������� ������ �� ��������.                                      � 
 *                  CAN_PURGE_RXCLEAR - ������� ���������� ����� ������.
 *                  CAN_PURGE_RXABORT - ��������� ���������� ��������� � �������� �� ������.
 *                  CAN_PURGE_HWRESET - ��������� ���������� ����� ��������. ��� ���� ������������
 *                                      �������� ������ ��������. ��������� �������� �����������.
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  �������, � ����������� �� �������� flags, ������� ������ ������ �/��� �������� �/���
 *                  ��������� ���������� ����� ��������. 
 *                  ����� ����, ��������� �������� �������/���������� ��������� � �������� �� ������/������.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_purge(F_CAN_HANDLE hDev, F_CAN_PURGE_MASK flags);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_wait(F_CAN_HANDLE hDev, PF_CAN_WAIT pWait, size_t msTimeout)
 *
 *  Parameters   :  hDev - ����� ��������
 *                  pWait - ��������� �� ������, ���������� ����� ���������� ������� CAN-�������� pWait->waitMask
 *                  � �������� �������� �������� �������.
 *                  msTimeout - ������� �������� � �������������.
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *								  CAN_RES_TIMEOUT - �������, ������� ������ �� ������������� ����������, 
 *                  � ������� CAN_RES_OK � CAN_RES_TIMEOUT � ���� pWait->status ����� �������� �������� �������� �������.
 *                  ��������, �������� �� CAN_RES_OK � CAN_RES_TIMEOUT, ��������������� �� ������ ������ �������.
 *
 *  Description  :  ������� ��������� ���������� ����������� ������ �� ��� ���, ���� ������ CAN-�������� �� 
 *                  ����� ��������������� ������ �� ��������, ������������ ������ pWait->waitMask, ��� 
 *                  �� ������� ������� �������� ���������� msTimeout. ��� ���������� ������ ������� � ����� 
 *                  CAN_RES_OK ��� CAN_RES_TIMEOUT ���� pWait->status ����� ��������� ������� �������� �������.
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_wait(F_CAN_HANDLE hDev, PF_CAN_WAIT pWait, size_t msTimeout);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_clear_errors(F_CAN_HANDLE hDev, PF_CAN_ERRORS pErrors)
 *
 *  Parameters   :  hDev - ����� ��������
 *                  pErrors - ��������� �� ������, � ������� ����� ���������� �������� ������.
 *                  ���� � �������� pErrors �������� NULL, ���������� ����� ��������� ������ ���
 *                  ������� ��������.
 *
 *  Return value :
 *								  CAN_RES_OK - �����;
 *                  ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
 *
 *  Description  :  ������� �������� �������� ��������� ������ ������� �������� �
 *                  ������ pErrors � ���������� ��������. ���� � �������� pErrors �������� NULL, 
 *                  ����� ���������� ����� ��������� ������ ��� ������� ��������.
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_get_clear_errors(F_CAN_HANDLE hDev, PF_CAN_ERRORS pErrors);

#ifdef __cplusplus
}
#endif

#endif