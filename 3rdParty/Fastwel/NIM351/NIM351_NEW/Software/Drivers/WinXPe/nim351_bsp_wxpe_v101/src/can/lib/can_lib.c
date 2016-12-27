/*
 *
 * NAME:    can_lib.c
 *
 * DESCRIPTION:
 *
 *
 * AUTHOR:
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2009 Fastwel Co Ltd.
 *      All rights reserved.
 ********************************************************/


#ifdef __CAN_LIBRARY__

#include <eal/el.h>

#ifdef __WINDOWS_CE__
#error Not supported 
#elif defined(__QNX_63__)
#error Not supported 
#elif (defined _WIN32_WINNT)

//----->> xp_el.h 
#define el_get_last_error()   GetLastError()
//<<----- xp_el.h 

#include <can/xp/canio.h>

#endif

#include <can/can_a.h>
#include <can/lib/can_lib.h>

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_init(void)
{
  return CAN_RES_OK;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_open(F_CAN_DEVID id, PF_CAN_HANDLE phDev)
{
  F_CHAR device_name[256];
  FILE_HANDLE  file_handle;

  if(!phDev)
    return CAN_RES_INVALID_POINTER;

  // В случае неудачи вернем инвалидный хэндл
  *phDev = EL_INVALID_HANDLE_VALUE;

  el_strcpy(device_name, CAN_DEVICE_NAME);
  device_name[CAN_INDEX_INDEX] = ((F_CHAR)'0') + id;
  device_name[CAN_INDEX_INDEX+1] = 0;

  file_handle = el_open(device_name);

  F_DBG1("el_open(%s) ...\n", device_name);
  if(!el_handle_valid(file_handle))
  {
    F_DBG2("el_open(%s) failed with error %d.\n", device_name, el_get_last_error());
    return CAN_RES_OPEN_DEVICE;
  }

  *phDev = (F_CAN_HANDLE)file_handle;
  return CAN_RES_OK;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_close(F_CAN_HANDLE hDev)
{
  if(!el_handle_valid(hDev))
    return CAN_RES_INVALID_HANDLE;

  el_close(hDev);

  return CAN_RES_OK;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_start(F_CAN_HANDLE hDev)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    bool bRes;
    size_t szret;

    bRes = el_device_ctl(hDev, IOCTL_CAN_START_DEVICE, NULL, 0, NULL, 0, &szret);

    if(bRes)
      fwcRes = CAN_RES_OK;
    else
    {
      F_DBG1("IOCTL_CAN_START_DEVICE failed with error %d.\n", el_get_last_error());
      fwcRes = CAN_RES_FAILURE;
    }
  }
  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_stop(F_CAN_HANDLE hDev)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    bool bRes;
    size_t szret;

    bRes = el_device_ctl(hDev, IOCTL_CAN_STOP_DEVICE, NULL, 0, NULL, 0, &szret);
    if(bRes)
      fwcRes = CAN_RES_OK;
    else
    {
      F_DBG1("IOCTL_CAN_STOP_DEVICE failed with error %d.\n", el_get_last_error());
      fwcRes = CAN_RES_FAILURE;
    }
  }
  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_get_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pDcb)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t szret;
      bool bRes;

      bRes = el_device_ctl(hDev, IOCTL_CAN_GET_DEVSETTINGS, NULL, 0, pDcb, sizeof(F_CAN_SETTINGS), &szret);
      if(bRes)
      {
        // Операция успешно завершена. Еще немного контроля на всякий случай.
        if(szret == sizeof(F_CAN_SETTINGS))
          fwcRes = CAN_RES_OK;
        else
        {
          // ?. Скорее всего "bug" в драйвере.
          F_DBG1("IOCTL_CAN_GET_DEVSETTINGS returns with invalid data size %d\n", szret);
          fwcRes = CAN_RES_UNEXPECTED;
        }
      }
      else
      {
        // Операция завершена с ошибкой
        F_DBG1("IOCTL_CAN_GET_DEVSETTINGS failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }

  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_set_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pDcb)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t szret;
      bool bRes;

      bRes = el_device_ctl(hDev, IOCTL_CAN_SET_DEVSETTINGS, pDcb, sizeof(F_CAN_SETTINGS), NULL, 0, &szret);
      if(bRes)
        fwcRes = CAN_RES_OK;
      else
      {
        F_DBG1("IOCTL_CAN_SET_DEVSETTINGS failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }

  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_get_stats(F_CAN_HANDLE hDev, PF_CAN_STATS pStats)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pStats)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t szret;
      bool bRes;

      bRes = el_device_ctl(hDev, IOCTL_CAN_GET_STATS, NULL, 0, pStats, sizeof(F_CAN_STATS), &szret);
      if(bRes)
      {
        // Операция успешно завершена. Еще немного контроля на всякий случай.
        if(szret == sizeof(F_CAN_STATS))
          fwcRes = CAN_RES_OK;
        else
        {
          // ?. Скорее всего "bug" в драйвере.
          F_DBG1("IOCTL_CAN_GIOCTL_CAN_GET_STATS returns with invalid data size %d\n", szret);
          fwcRes = CAN_RES_UNEXPECTED;
        }
      }
      else
      {
        // Операция завершена с ошибкой
        F_DBG1("IOCTL_CAN_GET_STATS failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }
  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_clear_stats(F_CAN_HANDLE hDev)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    bool bRes;
    size_t szret;

    bRes = el_device_ctl(hDev, IOCTL_CAN_CLEAR_STATS, NULL, 0, 0, 0, &szret);
    if(bRes)
      fwcRes = CAN_RES_OK;
    else
    {
      F_DBG1("IOCTL_CAN_CLEAR_STATS failed with error %d.\n", el_get_last_error());
      fwcRes = CAN_RES_FAILURE;
    }
  }
  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_get_controller_state(F_CAN_HANDLE hDev, PF_CAN_STATE pState)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pState)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t szret;
      bool bRes;

      bRes = el_device_ctl(hDev, IOCTL_CAN_GET_DEVSTATUS, NULL, 0, pState, sizeof(F_CAN_STATE), &szret);
      if(bRes)
      {
        // Операция успешно завершена. Еще немного контроля на всякий случай.
        fwcRes = CAN_RES_OK;
#if 0
        // DUCK: При обработке данного запроса драйвер не возвращает длину.
        // Отсутствует Irp->IoStatus.Information = sizeof(F_CAN_STATE);
        // Таким образом данная проверка невалидна. Она и не особо нужна.
        if(szret == sizeof(F_CAN_STATE))
          fwcRes = CAN_RES_OK;
        else
        {
          // ?. Скорее всего "bug" в драйвере.
          F_DBG1("IOCTL_CAN_GET_DEVSTATUS returns with invalid data size %d\n", szret);
          fwcRes = CAN_RES_UNEXPECTED;
        }
#endif
      }
      else
      {
        // Операция завершена с ошибкой
        F_DBG1("IOCTL_CAN_GET_DEVSTATUS failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }
  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
FDLL_EXPORT F_CAN_RESULT fw_can_get_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pTimeouts)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t szret;
      bool bRes;

      bRes = el_device_ctl(hDev, IOCTL_CAN_GET_TIMEOUTS, NULL, 0, pTimeouts, sizeof(F_CAN_TIMEOUTS), &szret);
      if(bRes)
      {
        // Операция успешно завершена. Еще немного контроля на всякий случай.
        if(szret == sizeof(F_CAN_TIMEOUTS))
          fwcRes = CAN_RES_OK;
        else
        {
          // ?. Скорее всего "bug" в драйвере.
          F_DBG1("IOCTL_CAN_GET_TIMEOUTS returns with invalid data size %d\n", szret);
          fwcRes = CAN_RES_UNEXPECTED;
        }
      }
      else
      {
        // Операция завершена с ошибкой
        F_DBG1("IOCTL_CAN_GET_TIMEOUTS failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }
  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
FDLL_EXPORT F_CAN_RESULT fw_can_set_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pTimeouts)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t szret;
      bool bRes;

      bRes = el_device_ctl(hDev, IOCTL_CAN_SET_TIMEOUTS, pTimeouts, sizeof(F_CAN_TIMEOUTS), NULL, 0, &szret);
      if(bRes)
        fwcRes = CAN_RES_OK;
      else
      {
        F_DBG1("IOCTL_CAN_SET_TIMEOUTS failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }

  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
FDLL_EXPORT F_CAN_RESULT fw_can_purge(F_CAN_HANDLE hDev, F_CAN_PURGE_MASK flags)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!(flags & (CAN_PURGE_TXABORT|CAN_PURGE_RXABORT|CAN_PURGE_TXCLEAR|CAN_PURGE_RXCLEAR|CAN_PURGE_HWRESET)))
      fwcRes = CAN_RES_INVALID_PARAMETER;
    else
    {
      size_t szret;
      bool bRes;

      bRes = el_device_ctl(hDev, IOCTL_CAN_PURGE, &flags, sizeof(F_CAN_PURGE_MASK), NULL, 0, &szret);
      if(bRes)
        fwcRes = CAN_RES_OK;
      else
      {
        F_DBG1("IOCTL_CAN_PURGE failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }

  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
FDLL_EXPORT F_CAN_RESULT fw_can_send(F_CAN_HANDLE hDev, PF_CAN_TX pTx, size_t nTx, size_t* pnSend)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pTx)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t written;
      bool bRes;

      if(pnSend)
        *pnSend = 0;

      bRes = el_write(hDev, pTx, nTx*sizeof(F_CAN_TX), &written);
      if(bRes)
      {
        if(pnSend)
          *pnSend = written/sizeof(F_CAN_TX);
        fwcRes = CAN_RES_OK;
      }
      else
      {
        F_DBG1("Write file failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }

  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
FDLL_EXPORT F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* pnRecv)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pRx)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t nbread;
      bool bRes;

      if(pnRecv)
        *pnRecv = 0;

      bRes = el_read(hDev, pRx, szRx*sizeof(F_CAN_RX), &nbread);
      if(bRes)
      {
        if(pnRecv)
          *pnRecv = nbread/sizeof(F_CAN_RX);
        fwcRes = CAN_RES_OK;
      }
      else
      {
        F_DBG1("Read file failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }

  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
FDLL_EXPORT F_CAN_RESULT fw_can_post_message(F_CAN_HANDLE hDev, PF_CAN_TX pTx)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pTx)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      bool bRes;
      size_t szret;

      bRes = el_device_ctl(hDev, IOCTL_CAN_POST_MSG, pTx, sizeof(F_CAN_TX), NULL, 0, &szret);
      if(bRes)
        fwcRes = CAN_RES_OK;
      else
      {
        F_DBG1("IOCTL_CAN_POST_MSG failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }

  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
FDLL_EXPORT F_CAN_RESULT fw_can_peek_message(F_CAN_HANDLE hDev, PF_CAN_RX pRx)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pRx)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      size_t szret;
      bool bRes;

      bRes = el_device_ctl(hDev, IOCTL_CAN_PEEK_MSG, NULL, 0, pRx, sizeof(F_CAN_RX), &szret);
      if(bRes)
        fwcRes = (szret==sizeof(F_CAN_RX))? CAN_RES_OK : CAN_RES_RXQUEUE_EMPTY;
      else
      {
        F_DBG1("IOCTL_CAN_PEEK_MSG failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }

  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_get_clear_errors(F_CAN_HANDLE hDev, PF_CAN_ERRORS pErrors)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    F_CAN_ERRORS errs;
    size_t szret;
    bool bRes;

    bRes = el_device_ctl(hDev, IOCTL_CAN_GET_CLEAR_ERR, NULL, 0, &errs, sizeof(F_CAN_ERRORS), &szret);
    if(bRes)
    {
      // Операция успешно завершена. Еще немного контроля на всякий случай.
      if(szret == sizeof(F_CAN_ERRORS))
      {
        fwcRes = CAN_RES_OK;
        if(pErrors)
        {
          *pErrors = errs;
        }
      }
      else
      {
        // ?. Скорее всего "bug" в драйвере.
        F_DBG1("IOCTL_CAN_GET_CLEAR_ERR returns with invalid data size %d\n", szret);
        fwcRes = CAN_RES_UNEXPECTED;
      }
    }
    else
    {
      // Операция завершена с ошибкой
      F_DBG1("IOCTL_CAN_GET_CLEAR_ERR failed with error %d.\n", el_get_last_error());
      fwcRes = CAN_RES_FAILURE;
    }
  }
  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
F_CAN_RESULT fw_can_wait(F_CAN_HANDLE hDev, PF_CAN_WAIT pWait, size_t msTimeout)
{
  F_CAN_RESULT fwcRes = CAN_RES_INVALID_HANDLE;

  if(el_handle_valid(hDev))
  {
    if(!pWait)
      fwcRes = CAN_RES_INVALID_POINTER;
    else
    {
      F_CAN_WAIT_PARAM wait_param;
      F_CAN_STATUS_T wait_status;
      size_t szret;
      bool bRes;

      wait_param.mask = pWait->waitMask;
      wait_param.msec = msTimeout;

      bRes = el_device_ctl(hDev, 
        IOCTL_CAN_WAIT_ON_MASK, 
        &wait_param, sizeof(F_CAN_WAIT_PARAM), 
        &wait_status, sizeof(F_CAN_STATUS_T), 
        &szret);
      
      if(bRes)
      {
        // Операция успешно завершена. Пороверяем по таймауту или по событии
        if(szret == sizeof(F_CAN_STATUS_T))
        {
          pWait->status = wait_status.status;
          if(pWait->waitMask & pWait->status)
            fwcRes = CAN_RES_OK;
          else
            fwcRes = CAN_RES_TIMEOUT;
        }
        else
        {
          // ?. Скорее всего "bug" в драйвере.
          F_DBG1("IOCTL_CAN_WAIT_ON_MASK returns with invalid data size %d\n", szret);
          fwcRes = CAN_RES_UNEXPECTED;
        }
      }
      else
      {
        // Операция завершена с ошибкой
        F_DBG1("IOCTL_CAN_WAIT_ON_MASK failed with error %d.\n", el_get_last_error());
        fwcRes = CAN_RES_FAILURE;
      }
    }
  }
  return fwcRes;
}
//----------------------------------------------------------------------------------------------------
void app_startup()
{
  F_DBG("Fastwel FwCAN Library loaded\r\n");
  el_init();
}

void app_finish()
{
  el_deinit();
}


#endif