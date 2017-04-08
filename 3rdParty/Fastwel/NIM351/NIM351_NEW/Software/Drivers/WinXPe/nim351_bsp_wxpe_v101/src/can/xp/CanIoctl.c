#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

//#include <ntddk.h>
#include <eal/el.h>
#include <can/can_a.h>
#include <can/xp/CanDrv.h>
#include <can/xp/canio.h>

BOOLEAN CanSetDevSettings(IN PVOID Context);
BOOLEAN CanGetStats(IN PVOID Context);
BOOLEAN CanClearStats(IN PVOID Context);
BOOLEAN CanResetController(IN PVOID Context);
NTSTATUS CanStartPurge(IN PCAN_DEVICE_EXTENSION pDevExt);
BOOLEAN CanPurgeInterruptBuff(IN PVOID Context);
BOOLEAN CanPeekMessage(IN PVOID Context);
VOID CanTryCompletePendingWaits(IN PCAN_DEVICE_EXTENSION pDevExt);
NTSTATUS CanStartWaitOrQueue(IN PCAN_DEVICE_EXTENSION pDevExt, IN PIRP pIrp);
BOOLEAN CanGetStatus(IN PVOID Context);
BOOLEAN CanRequestWait(IN PVOID Context);
BOOLEAN CanGetClearErrors(IN PVOID Context);
BOOLEAN CanStartController(IN PVOID Context);
BOOLEAN CanStopController(IN PVOID Context);
NTSTATUS CanPostMessage(IN PCAN_DEVICE_EXTENSION pDevExt, IN PIRP pIrp);
BOOLEAN CanPostTransmission(IN PVOID Context);
VOID CanKillCurrentWrite(IN PCAN_DEVICE_EXTENSION pDevExt);
BOOLEAN CanAbortTransmission(IN PVOID Context);

#ifdef ALLOC_PRAGMA
#pragma alloc_text(PAGEFCAN,CanIoControl)
#pragma alloc_text(PAGEFCAN,CanSetDevSettings)
#pragma alloc_text(PAGEFCAN,CanGetStats)
#pragma alloc_text(PAGEFCAN,CanClearStats)
#pragma alloc_text(PAGEFCAN,CanResetController)
#pragma alloc_text(PAGEFCAN,CanStartPurge)
#pragma alloc_text(PAGEFCAN,CanPurgeInterruptBuff)
#pragma alloc_text(PAGEFCAN,CanPeekMessage)
#pragma alloc_text(PAGEFCAN,CanStartWaitOrQueue)
#pragma alloc_text(PAGEFCAN,CanTryCompletePendingWaits)
#pragma alloc_text(PAGEFCAN,CanGetStatus)
#pragma alloc_text(PAGEFCAN,CanRequestWait)
#pragma alloc_text(PAGEFCAN,CanGetClearErrors)
#pragma alloc_text(PAGEFCAN,CanStartController)
#pragma alloc_text(PAGEFCAN,CanStopController)
#pragma alloc_text(PAGEFCAN,CanPostMessage)
#pragma alloc_text(PAGEFCAN,CanPostTransmission)
#pragma alloc_text(PAGEFCAN,CanKillCurrentWrite)
#pragma alloc_text(PAGEFCAN,CanAbortTransmission)
#endif

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine provides the initial processing for all of the
//    Ioctrls for the can device.
//
//Arguments:
//
//    DeviceObject - Pointer to the device object for this device
//
//    Irp - Pointer to the IRP for the current request
//
//Return Value:
//
//    The function value is the final status of the call
//-----------------------------------------------------------------------------
NTSTATUS CanIoControl(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp)
{
  // The status that gets returned to the caller and set in the Irp.
  NTSTATUS Status;
  // The current stack location.  This contains all of the
  // information we need to process this particular request.
  PIO_STACK_LOCATION IrpSp;
  // Just what it says.  This is the can specific device
  // extension of the device object create for the can driver.
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
  // A temporary to hold the old IRQL so that it can be
  // restored once we complete/validate this request.
  KIRQL OldIrql;
  // The following simple structure is used to send a pointer
  // the device extension and an ioctl specific pointer to data.
  CAN_IOCTL_SYNC ioctl_sync;

  if( pDevExt->Flags & CAN_FLAGS_STOPPED )
  {
    F_DBG("In CanIoControl() CAN_FLAGS_STOPPED set, cancel operation\n" ); 
    Irp->IoStatus.Status = STATUS_CANCELLED;
    Irp->IoStatus.Information = 0L;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_CANCELLED;
  }

  if( !pDevExt->Flags & CAN_FLAGS_STARTED )
  {
    F_DBG("In CanIoControl() CAN_FLAGS_STARTED isn't set, cancel operation\n" ); 
    Irp->IoStatus.Status = STATUS_CANCELLED;
    Irp->IoStatus.Information = 0L;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_CANCELLED;
  }

  CAN_LOCKED_PAGED_CODE();

  // We expect to be open so all our pages are locked down.  This is, after
  // all, an IO operation, so the device should be open first.
  KeAcquireSpinLock(&pDevExt->ControlLock, &OldIrql);
  if(!pDevExt->DeviceIsOpened)
  {
    KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);
    Irp->IoStatus.Status = STATUS_INVALID_DEVICE_REQUEST;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_INVALID_DEVICE_REQUEST;
  }
  KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);

  if((Status = CanIRPPrologue(Irp, pDevExt)) != STATUS_SUCCESS)
  {
    if(Status != STATUS_PENDING)
    {
      Irp->IoStatus.Status = Status;
      CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
    }
    return Status;
  }

  if(CanCompleteIfError(DeviceObject, Irp) != STATUS_SUCCESS)
  {
    F_DBG("Request aborted by error\n");
    return STATUS_CANCELLED;
  }

  IrpSp = IoGetCurrentIrpStackLocation(Irp);
  Irp->IoStatus.Information = 0L;
  Status = STATUS_SUCCESS;
  switch(IrpSp->Parameters.DeviceIoControl.IoControlCode)
  {
  case IOCTL_CAN_GET_DEVSETTINGS:
    {
      if(IrpSp->Parameters.DeviceIoControl.OutputBufferLength < sizeof(F_CAN_SETTINGS))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      KeAcquireSpinLock(&pDevExt->ControlLock, &OldIrql);
      *((F_CAN_SETTINGS*)Irp->AssociatedIrp.SystemBuffer) = pDevExt->Controller.settings;
      KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);
      Irp->IoStatus.Information = sizeof(F_CAN_SETTINGS);
      break;
    }

  case IOCTL_CAN_SET_DEVSETTINGS:
    {
      PF_CAN_SETTINGS pNewSettings = (PF_CAN_SETTINGS)Irp->AssociatedIrp.SystemBuffer;
      if(IrpSp->Parameters.DeviceIoControl.InputBufferLength < sizeof(F_CAN_SETTINGS))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      
      ioctl_sync.pExtension = pDevExt;
      ioctl_sync.pData = pNewSettings;
      KeAcquireSpinLock(&pDevExt->ControlLock, &OldIrql);
      KeSynchronizeExecution(pDevExt->Interrupt, CanSetDevSettings, &ioctl_sync);
      KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);
      Status = ioctl_sync.status;
      break;
    }

  case IOCTL_CAN_GET_TIMEOUTS:
    {
      if(IrpSp->Parameters.DeviceIoControl.OutputBufferLength < sizeof(F_CAN_TIMEOUTS))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      KeAcquireSpinLock(&pDevExt->ControlLock, &OldIrql);
      *((PF_CAN_TIMEOUTS)Irp->AssociatedIrp.SystemBuffer) = pDevExt->Timeouts;
      Irp->IoStatus.Information = sizeof(F_CAN_TIMEOUTS);
      KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);
      break;
    }

  case IOCTL_CAN_SET_TIMEOUTS:
    {
      PF_CAN_TIMEOUTS pNewTimeouts = (PF_CAN_TIMEOUTS)(Irp->AssociatedIrp.SystemBuffer);
      if(IrpSp->Parameters.DeviceIoControl.InputBufferLength < sizeof(F_CAN_TIMEOUTS))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      if(pNewTimeouts->WriteTotalTimeoutMultiplier==MAXULONG || pNewTimeouts->WriteTotalTimeoutConstant==MAXULONG)
      {
        Status = STATUS_INVALID_PARAMETER;
        break;
      }
      if(pNewTimeouts->RestartBusoffTimeout==MAXULONG)
      {
        Status = STATUS_INVALID_PARAMETER;
        break;
      }

      KeAcquireSpinLock(&pDevExt->ControlLock, &OldIrql);
      pDevExt->Timeouts = *pNewTimeouts;
      KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);
      break;
    }

  case IOCTL_CAN_GET_DEVSTATUS:
    {
      if(IrpSp->Parameters.DeviceIoControl.OutputBufferLength < sizeof(F_CAN_STATE))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      *((F_CAN_STATE*)Irp->AssociatedIrp.SystemBuffer) = CanGetControllerStatus(pDevExt);
      Irp->IoStatus.Information = sizeof(F_CAN_STATE);
    }
    break;

  case IOCTL_CAN_PURGE:
    {
      F_CAN_PURGE_MASK Mask;
      if(IrpSp->Parameters.DeviceIoControl.InputBufferLength < sizeof(F_CAN_PURGE_MASK))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }

      Mask = *((F_CAN_PURGE_MASK *)(Irp->AssociatedIrp.SystemBuffer));
      // Check to make sure that the mask has the appropriate values.
      if(!(Mask & (CAN_PURGE_TXABORT | CAN_PURGE_RXABORT | CAN_PURGE_TXCLEAR | CAN_PURGE_RXCLEAR | CAN_PURGE_HWRESET)))
      {
        Status = STATUS_INVALID_PARAMETER;
        break;
      }

      // Either start this irp or put it on the queue.
      return CanStartOrQueue(pDevExt, Irp, &pDevExt->PurgeQueue, &pDevExt->CurrentPurgeIrp, CanStartPurge);
    }
  
  case IOCTL_CAN_RESET_DEVICE:
    {
      KeSynchronizeExecution(pDevExt->Interrupt, CanResetController, pDevExt);
      break;
    }

  case IOCTL_CAN_START_DEVICE:
    {
      ioctl_sync.pExtension = pDevExt;
      KeSynchronizeExecution(pDevExt->Interrupt, CanStartController, &ioctl_sync);
      Status = ioctl_sync.status;
      break;
    }

  case IOCTL_CAN_STOP_DEVICE:
    {
      ioctl_sync.pExtension = pDevExt;
      KeSynchronizeExecution(pDevExt->Interrupt, CanStopController, &ioctl_sync);
      Status = ioctl_sync.status;
      break;
    }

  case IOCTL_CAN_GET_STATS:
    {
      if (IrpSp->Parameters.DeviceIoControl.OutputBufferLength < sizeof(F_CAN_STATS))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      Irp->IoStatus.Information = sizeof(F_CAN_STATS);
      Status = STATUS_SUCCESS;
      KeSynchronizeExecution(pDevExt->Interrupt, CanGetStats, Irp);
      break;
    }

  case IOCTL_CAN_CLEAR_STATS:
    {
      KeSynchronizeExecution(pDevExt->Interrupt, CanClearStats, pDevExt);
      break;
    }

  case IOCTL_CAN_PEEK_MSG:
    {
      if(IrpSp->Parameters.DeviceIoControl.OutputBufferLength < sizeof(F_CAN_RX))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      
      ioctl_sync.pExtension = pDevExt;
      ioctl_sync.pData = Irp->AssociatedIrp.SystemBuffer;
      KeAcquireSpinLock(&pDevExt->ControlLock, &OldIrql);
      KeSynchronizeExecution(pDevExt->Interrupt, CanPeekMessage, &ioctl_sync);
      KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);

      Status = STATUS_SUCCESS;
      Irp->IoStatus.Information = (NT_SUCCESS(ioctl_sync.status))? sizeof(F_CAN_RX) : 0;
      break;
    }

  case IOCTL_CAN_POST_MSG:
    {
      if(IrpSp->Parameters.DeviceIoControl.InputBufferLength < sizeof(F_CAN_TX))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      Status = CanPostMessage(pDevExt, Irp);
      break;
    }
  
  case IOCTL_CAN_WAIT_ON_MASK:
    {
      PF_CAN_WAIT_PARAM pWaitParam;

      if(IrpSp->Parameters.DeviceIoControl.OutputBufferLength < sizeof(F_CAN_STATUS_T))
      {
        F_DBG1("WaitOnMask: Output buffer invalid size %lu\n", IrpSp->Parameters.DeviceIoControl.OutputBufferLength);
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      if(IrpSp->Parameters.DeviceIoControl.InputBufferLength < sizeof(F_CAN_WAIT_PARAM))
      {
        F_DBG1("WaitOnMask: Input buffer invalid size %lu\n", IrpSp->Parameters.DeviceIoControl.InputBufferLength);
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }

      pWaitParam = (PF_CAN_WAIT_PARAM)(Irp->AssociatedIrp.SystemBuffer);
      // Check to make sure that the mask has the appropriate values.
      // If the app queues a wait on a invalid mask it can't
      // be statisfied so it makes no sense to start it.
      if(!(pWaitParam->mask & (CAN_WAIT_RX|CAN_WAIT_TX|CAN_WAIT_ERR)))
      {
        F_DBG2("WaitOnMask: Invalid input mask(0x%X), time(%u)\n", pWaitParam->mask, pWaitParam->msec);
        Status = STATUS_INVALID_PARAMETER;
        break;
      }

      F_DBG2("WaitOnMask: mask(0x%X), time(%u)\n", pWaitParam->mask, pWaitParam->msec);

      return CanStartWaitOrQueue(pDevExt, Irp);
    }

  case IOCTL_CAN_GET_CLEAR_ERR:
    {
      if (IrpSp->Parameters.DeviceIoControl.OutputBufferLength < sizeof(F_CAN_ERRORS))
      {
        Status = STATUS_BUFFER_TOO_SMALL;
        break;
      }
      Irp->IoStatus.Information = sizeof(F_CAN_ERRORS);
      Status = STATUS_SUCCESS;
      KeSynchronizeExecution(pDevExt->Interrupt, CanGetClearErrors, Irp);
      break;
    }
  }

  Irp->IoStatus.Status = Status;
  CanCompleteRequest(pDevExt, Irp, 0);

  return Status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to set the baud rate, operation modes and filters of the device.
//
//Arguments:
//
//    Context - Pointer to a structure that contains a pointer to
//              the device extension and what should be the current
//              settings and operation status.
//
//Return Value:
//
//    This routine always returns FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanSetDevSettings(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = ((PCAN_IOCTL_SYNC)Context)->pExtension;
  PF_CAN_SETTINGS pNewSettings = (PF_CAN_SETTINGS)((PCAN_IOCTL_SYNC)Context)->pData;

  CAN_LOCKED_PAGED_CODE();

  if(pDevExt->Controller.state == CAN_STATE_INIT)
  {
    if(sja1000_set_settings(&pDevExt->Controller, pNewSettings))
      ((PCAN_IOCTL_SYNC)Context)->status = STATUS_SUCCESS;
    else
      ((PCAN_IOCTL_SYNC)Context)->status = STATUS_ADAPTER_HARDWARE_ERROR;

    // Any way
    pDevExt->Controller.state = CAN_STATE_INIT;
  }
  else
    ((PCAN_IOCTL_SYNC)Context)->status = STATUS_CANCELLED;

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    In sync with the interrpt service routine (which sets the perf stats)
//    return the perf stats to the caller.
//
//
//Arguments:
//
//    Context - Pointer to a the irp.
//
//Return Value:
//
//    This routine always returns FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanGetStats(IN PVOID Context)
{
  PIO_STACK_LOCATION irpSp = IoGetCurrentIrpStackLocation((PIRP)Context);
  PCAN_DEVICE_EXTENSION pDevExt = irpSp->DeviceObject->DeviceExtension;
  PF_CAN_STATS pcs = (PF_CAN_STATS)(((PIRP)Context)->AssociatedIrp.SystemBuffer);

  CAN_LOCKED_PAGED_CODE();

  *pcs = pDevExt->Controller.stats;
    
  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    In sync with the interrpt service routine (which sets the perf stats)
//    clear the perf stats.
//
//
//Arguments:
//
//    Context - Pointer to a the extension.
//
//Return Value:
//
//    This routine always returns FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanClearStats(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = Context;

  CAN_LOCKED_PAGED_CODE();

  RtlZeroMemory(&pDevExt->Controller.stats, sizeof(F_CAN_STATS));
  //RtlZeroMemory(&pDevExt->WmiPerfData, sizeof(F_CAN_WMI_PERF_DATA));
    
  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This reset the hardware in a normal mode.
//
//    NOTE: This assumes that it is called at interrupt level.
//
//
//Arguments:
//
//    Context - The device pDevExt for can device
//    being managed.
//
//Return Value:
//
//    Always FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanResetController(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)Context;
  F_CAN_STATE curState = pDevExt->Controller.state;

  CAN_LOCKED_PAGED_CODE();

  sja1000_stop(&pDevExt->Controller);
  
  if(curState != CAN_STATE_INIT)
  {
    if(sja1000_start(&pDevExt->Controller))
    {
      if(pDevExt->WriteLength)
      {
        // We not finished transmitting current frame
        sja1000_start_xmit(&pDevExt->Controller, pDevExt->WriteCurrentFrame);
      }
    }
  }
  else
  {
    pDevExt->Controller.state = CAN_STATE_INIT;
  }

  return FALSE;
}
//-----------------------------------------------------------------------------
BOOLEAN CanStartController(IN PVOID Context)
{
  PCAN_IOCTL_SYNC pSync = (PCAN_IOCTL_SYNC)Context;
  PCAN_DEVICE_EXTENSION pDevExt = pSync->pExtension;

  CAN_LOCKED_PAGED_CODE();

  if(pDevExt->Controller.state == CAN_STATE_INIT)
  {
    if(sja1000_start(&pDevExt->Controller))
    {
      pSync->status = STATUS_SUCCESS;
    }
    else
    {
      pDevExt->Controller.state = CAN_STATE_INIT;
      pSync->status = STATUS_ADAPTER_HARDWARE_ERROR;
    }
  }
  else
    pSync->status = STATUS_SUCCESS;

  return FALSE;
}
//-----------------------------------------------------------------------------
BOOLEAN CanStopController(IN PVOID Context)
{
  PCAN_IOCTL_SYNC pSync = (PCAN_IOCTL_SYNC)Context;
  PCAN_DEVICE_EXTENSION pDevExt = pSync->pExtension;

  CAN_LOCKED_PAGED_CODE();

  if(sja1000_stop(&pDevExt->Controller))
    pSync->status = STATUS_SUCCESS;
  else
    pSync->status = STATUS_ADAPTER_HARDWARE_ERROR;
  
  // Anyway
  pDevExt->Controller.state = CAN_STATE_INIT;

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Depending on the mask in the current irp, purge the interrupt
//    buffer, the read queue, or the write queue, or all of the above.
//
//Arguments:
//
//    pDevExt - Pointer to the device extension.
//
//Return Value:
//
//    Will return STATUS_SUCCESS always.  This is reasonable
//    since the DPC completion code that calls this routine doesn't
//    care and the purge request always goes through to completion
//    once it's started.
//-----------------------------------------------------------------------------
NTSTATUS CanStartPurge(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  PIRP NewIrp;
  KIRQL OldIrql;

  CAN_LOCKED_PAGED_CODE();
  do
  {
    F_CAN_PURGE_MASK Mask = *((F_CAN_PURGE_MASK *)(pDevExt->CurrentPurgeIrp->AssociatedIrp.SystemBuffer));
        
    if(Mask & CAN_PURGE_TXABORT)
      CanKillAllReadsOrWrites(pDevExt->DeviceObject, &pDevExt->WriteQueue, &pDevExt->CurrentWriteIrp);
    
    if(Mask & CAN_PURGE_TXCLEAR)
    {
      CanKillCurrentWrite(pDevExt);
    }

    if(Mask & CAN_PURGE_RXABORT)
      CanKillAllReadsOrWrites(pDevExt->DeviceObject, &pDevExt->ReadQueue, &pDevExt->CurrentReadIrp);

    if(Mask & CAN_PURGE_RXCLEAR)
    {
      // Clean out the interrupt buffer.
      // Note that we do this under protection of the the drivers control lock 
      // so that we don't hose the pointers if there is currently a read out of the buffer.
      KeAcquireSpinLock(&pDevExt->ControlLock, &OldIrql);
      KeSynchronizeExecution(pDevExt->Interrupt, CanPurgeInterruptBuff, pDevExt);
      KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);
    }

    if(Mask & CAN_PURGE_HWRESET)
      KeSynchronizeExecution(pDevExt->Interrupt, CanResetController, pDevExt);

    pDevExt->CurrentPurgeIrp->IoStatus.Status = STATUS_SUCCESS;
    pDevExt->CurrentPurgeIrp->IoStatus.Information = 0;
    CanGetNextIrp(&pDevExt->CurrentPurgeIrp, &pDevExt->PurgeQueue, &NewIrp, TRUE, pDevExt);
  }
  while(NewIrp);

  return STATUS_SUCCESS;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine simply resets the interrupt (typeahead) buffer.
//
//    NOTE: This routine is being called from KeSynchronizeExecution.
//
//Arguments:
//
//    Context - Really a pointer to the device extension.
//
//Return Value:
//
//    Always false.
//-----------------------------------------------------------------------------
BOOLEAN CanPurgeInterruptBuff(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = Context;
    
  CAN_LOCKED_PAGED_CODE();

  // The typeahead buffer is by definition empty if there
  // currently is a read owned by the isr.
  if(pDevExt->ReadBufferBase == pDevExt->InterruptReadBuffer)
  {
    pDevExt->rdInterruptReadBuffer = pDevExt->wrInterruptReadBuffer;
    F_CAN_ClearStatus(pDevExt->Controller.status, CAN_STATUS_RXBUF);
  }

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine peek message from the interrupt (typeahead) buffer.
//
//    NOTE: This routine is being called from KeSynchronizeExecution.
//
//Arguments:
//
//    Context - Pointer to a structure that contains a pointer to
//              the device extension and a pointer to the message buffer.
//
//Return Value:
//
//    Always false.
//-----------------------------------------------------------------------------
BOOLEAN CanPeekMessage(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = ((PCAN_IOCTL_SYNC)Context)->pExtension;
  PF_CAN_RX pBuf = (PF_CAN_RX)((PCAN_IOCTL_SYNC)Context)->pData;
    
  CAN_LOCKED_PAGED_CODE();

  ((PCAN_IOCTL_SYNC)Context)->status = STATUS_CANCELLED;

  // The typeahead buffer is by definition not empty if there
  // currently is a read owned by the isr.
  if(pDevExt->ReadBufferBase == pDevExt->InterruptReadBuffer)
  {
    ULONG nFramesToGet = (ULONG)(pDevExt->wrInterruptReadBuffer - pDevExt->rdInterruptReadBuffer);

    F_DBG3("CanPeekMessage(): wr(%lu), rd(%lu), available(%lu)\n", 
      pDevExt->wrInterruptReadBuffer,
      pDevExt->rdInterruptReadBuffer,
      nFramesToGet);
    if(nFramesToGet)
    {
      // Read Index in the interrupt buffer
      ULONG iRead = pDevExt->rdInterruptReadBuffer % pDevExt->szInterruptReadBuffer;

      RtlMoveMemory(pBuf, &pDevExt->InterruptReadBuffer[iRead], sizeof(F_CAN_RX));

      // Update read position in the interrupt read buffer
      pDevExt->rdInterruptReadBuffer ++;
      ((PCAN_IOCTL_SYNC)Context)->status = STATUS_SUCCESS;
      nFramesToGet --;
    }
    if(!nFramesToGet)
    {
      F_CAN_ClearStatus(pDevExt->Controller.status, CAN_STATUS_RXBUF);
    }
  }

  return FALSE;
}
//-----------------------------------------------------------------------------
BOOLEAN CanGetStatus(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = ((PCAN_IOCTL_SYNC)Context)->pExtension;
  PF_CAN_STATUS_T pSt = (PF_CAN_STATUS_T)((PCAN_IOCTL_SYNC)Context)->pData;

    
  CAN_LOCKED_PAGED_CODE();

  KeQueryTickCount(&pSt->taking);
  pSt->status = pDevExt->Controller.status;
  pSt->state = pDevExt->Controller.state;
  return FALSE;
}
//-----------------------------------------------------------------------------
BOOLEAN CanRequestWait(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = ((PCAN_IOCTL_SYNC)Context)->pExtension;
  PF_CAN_STATUS_T pSt = (PF_CAN_STATUS_T)((PCAN_IOCTL_SYNC)Context)->pData;

    
  CAN_LOCKED_PAGED_CODE();

  pDevExt->WaitStatus.status |= pSt->status;
  return FALSE;
}
//-----------------------------------------------------------------------------
VOID CanTryCompletePendingWaits(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  PLIST_ENTRY pWaitQueue = &pDevExt->WaitQueue;
  PLIST_ENTRY pWaitEntry;
  F_CAN_STATUS_T curSt;
  LARGE_INTEGER ExpireTime;
  KIRQL OldIrql;

  CAN_LOCKED_PAGED_CODE();

  F_DBG(">CanTryCompletePendingWaits()\n");
  IoAcquireCancelSpinLock(&OldIrql);

  if(IsListEmpty(pWaitQueue))
  {
    IoReleaseCancelSpinLock(OldIrql);
    F_DBG("CanTryCompletePendingWaits(): waitQueue empty\n");
    return;
  }

  pWaitEntry = pWaitQueue->Flink;
  while(pWaitEntry != pWaitQueue)
  {
    PIRP pIrp = CONTAINING_RECORD(pWaitEntry, IRP, Tail.Overlay.ListEntry);
    PF_CAN_STATUS_T pWaitSt = (PF_CAN_STATUS_T)(pIrp->AssociatedIrp.SystemBuffer);
    BOOLEAN first = (pWaitEntry->Blink == pWaitQueue);

    // First element of the list
    if(first)
    {
      CAN_IOCTL_SYNC syncData;
      syncData.pExtension = pDevExt;
      syncData.pData = &curSt;
      KeSynchronizeExecution(pDevExt->Interrupt, CanGetStatus, &syncData);
    }

    F_DBG6("CanTryCompletePendingWaits(): cur_st(%u), wait_st(%u), cur_time(%X%X), exp_time(%X%X)\n", curSt.status, pWaitSt->status, curSt.taking.HighPart, curSt.taking.LowPart, pWaitSt->taking.HighPart, pWaitSt->taking.LowPart);
    // 1. Current status is according to wait mask => complete, return current status
    // 2. Wait time is expired => complete, return current status
    if((curSt.status & pWaitSt->status)||((curSt.taking.QuadPart - pWaitSt->taking.QuadPart) >= 0))
    {
      RemoveEntryList(pWaitEntry);

#if 0
      if(curSt.status & pWaitSt->status)
      {
        *pWaitSt = curSt;
        pIrp->IoStatus.Information = sizeof(F_CAN_STATUS_T);
        pIrp->IoStatus.Status = STATUS_SUCCESS;
      }
      else
      {
        pIrp->IoStatus.Status = STATUS_TIMEOUT;
      }
#else
      *pWaitSt = curSt;
      pIrp->IoStatus.Information = sizeof(F_CAN_STATUS_T);
      pIrp->IoStatus.Status = STATUS_SUCCESS;
#endif

      IoReleaseCancelSpinLock(OldIrql);
      F_DBG1("CanTryCompletePendingWaits(): complete request st(%d)\n", pIrp->IoStatus.Status);
      CanCompleteRequest(pDevExt, pIrp, IO_NETWORK_INCREMENT);
      
      IoAcquireCancelSpinLock(&OldIrql);
      pWaitEntry = pWaitQueue->Flink;
    }
    else
    {
      if(pWaitEntry->Blink == pWaitQueue)
      {
        ExpireTime.QuadPart = pWaitSt->taking.QuadPart;
      }
      else
      {
        if(ExpireTime.QuadPart > pWaitSt->taking.QuadPart)
          ExpireTime.QuadPart = pWaitSt->taking.QuadPart;
      }

      pWaitEntry = pWaitEntry->Flink;
    }
  }

  if(IsListEmpty(pWaitQueue) || pDevExt->WaitStatus.taking.QuadPart > ExpireTime.QuadPart)
  {
    CanCancelTimer(&pDevExt->WaitOnMaskTimer, pDevExt);
    
    if(!IsListEmpty(pWaitQueue))
    {
      LARGE_INTEGER   Current;
      LARGE_INTEGER   Wait;

      KeQueryTickCount(&Current);
    
      if((ExpireTime.QuadPart - Current.QuadPart) > 0)
        Wait.QuadPart = (ExpireTime.QuadPart - Current.QuadPart) * KeQueryTimeIncrement();
      else
        Wait.QuadPart = KeQueryTimeIncrement();

      F_DBG1("CanTryCompletePendingWaits(): Queue not empty. SetTimer on %lu\n", (ULONG)(Wait.QuadPart/10000));
      Wait.QuadPart *= (-1);
      pDevExt->WaitStatus.taking.QuadPart = ExpireTime.QuadPart;
      CanSetTimer(&pDevExt->WaitOnMaskTimer, Wait, &pDevExt->CanWaitDpc, pDevExt);
    }
    else
    {
      F_DBG("CanTryCompletePendingWaits(): Queue empty\n");
    }
  }

  IoReleaseCancelSpinLock(OldIrql);
}
//-----------------------------------------------------------------------------
NTSTATUS CanStartWaitOrQueue(IN PCAN_DEVICE_EXTENSION pDevExt, IN PIRP pIrp)
{
  PLIST_ENTRY pWaitQueue = &pDevExt->WaitQueue;
  F_CAN_STATUS_T currentStatus;
  PF_CAN_WAIT_PARAM pWaitParam;
  CAN_IOCTL_SYNC syncData;
  KIRQL oldIrql;

  CAN_LOCKED_PAGED_CODE();

  IoAcquireCancelSpinLock(&oldIrql);

  pWaitParam = (PF_CAN_WAIT_PARAM)(pIrp->AssociatedIrp.SystemBuffer);

  syncData.pExtension = pDevExt;
  syncData.pData = &currentStatus;
  KeSynchronizeExecution(pDevExt->Interrupt, CanGetStatus, &syncData);

  F_DBG2(">CanStartWaitOrQueue(): cur_status(%u), wait_status(%u)\n", currentStatus.status, pWaitParam->mask);

  if((currentStatus.status & pWaitParam->mask) || (pWaitParam->msec == 0))
  {
    *((PF_CAN_STATUS_T)(pIrp->AssociatedIrp.SystemBuffer)) = currentStatus;
    pIrp->IoStatus.Information = sizeof(F_CAN_STATUS_T);
    pIrp->IoStatus.Status = STATUS_SUCCESS;

    IoReleaseCancelSpinLock(oldIrql);
    CanCompleteRequest(pDevExt, pIrp, IO_NETWORK_INCREMENT);
    return STATUS_SUCCESS;
  }

  // We don't know how long the irp will be in the
  // queue.  So we need to handle cancel.
  if(pIrp->Cancel)
  {
    IoReleaseCancelSpinLock(oldIrql);
    pIrp->IoStatus.Status = STATUS_CANCELLED;
    CanCompleteRequest(pDevExt, pIrp, 0);
    return STATUS_CANCELLED;
  }
  else
  {
    LARGE_INTEGER   Wait;
    LARGE_INTEGER   Current;
    LARGE_INTEGER   ExpireTime;

    // Wait timeout
    Wait.QuadPart = pWaitParam->msec;
    Wait.QuadPart = Wait.QuadPart * 10000; // in 100 nsec intervals
    // Expire time
    KeQueryTickCount(&Current);
    ExpireTime.QuadPart = Current.QuadPart + Wait.QuadPart / KeQueryTimeIncrement();

    ((PF_CAN_STATUS_T)(pIrp->AssociatedIrp.SystemBuffer))->taking.QuadPart = ExpireTime.QuadPart;

    if(IsListEmpty(pWaitQueue) || ((pDevExt->WaitStatus.taking.QuadPart - ExpireTime.QuadPart) > 0))
    {
      CanCancelTimer(&pDevExt->WaitOnMaskTimer, pDevExt);
      pDevExt->WaitStatus.taking.QuadPart = ExpireTime.QuadPart;
      F_DBG5("CanStartWaitOrQueue(): cur_time(%X%X), exp_time(%X%X). SetTimer on %lu ms\n", Current.HighPart, Current.LowPart, ExpireTime.HighPart, ExpireTime.LowPart, (ULONG)(Wait.QuadPart/10000));

      Wait.QuadPart *= (-1);
      CanSetTimer(&pDevExt->WaitOnMaskTimer, Wait, &pDevExt->CanWaitDpc, pDevExt);
    }

    pIrp->IoStatus.Status = STATUS_PENDING;
    IoMarkIrpPending(pIrp);
    InsertTailList(pWaitQueue, &pIrp->Tail.Overlay.ListEntry);
    IoSetCancelRoutine(pIrp, CanCancelQueued);

    syncData.pExtension = pDevExt;
    syncData.pData = pIrp->AssociatedIrp.SystemBuffer;
    KeSynchronizeExecution(pDevExt->Interrupt, CanRequestWait, &syncData);

    IoReleaseCancelSpinLock(oldIrql);
  
    return STATUS_PENDING;
  }
}
//-----------------------------------------------------------------------------
VOID CanCompleteWait(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeferredContext;

  UNREFERENCED_PARAMETER(SystemContext1);
  UNREFERENCED_PARAMETER(SystemContext2);

  F_DBG1(">CanCompleteWait(%X)\n", pDevExt);

  CanTryCompletePendingWaits(pDevExt);
  CanDpcEpilogue(pDevExt, Dpc);
}
//-----------------------------------------------------------------------------
BOOLEAN CanGetClearErrors(IN PVOID Context)
{
  PIO_STACK_LOCATION irpSp = IoGetCurrentIrpStackLocation((PIRP)Context);
  PCAN_DEVICE_EXTENSION pDevExt = irpSp->DeviceObject->DeviceExtension;
  PF_CAN_ERRORS pce = (PF_CAN_ERRORS)(((PIRP)Context)->AssociatedIrp.SystemBuffer);

  CAN_LOCKED_PAGED_CODE();

  *pce = pDevExt->Controller.errors;
  RtlZeroMemory(&pDevExt->Controller.errors, sizeof(F_CAN_ERRORS));
  F_CAN_ClearStatus(pDevExt->Controller.status, CAN_STATUS_ERR);
    
  return FALSE;
}
//-----------------------------------------------------------------------------
NTSTATUS CanPostMessage(IN PCAN_DEVICE_EXTENSION pDevExt, IN PIRP pIrp)
{
  KIRQL oldIrql;
  BOOLEAN not_busy;
  CAN_IOCTL_SYNC ioctl_sync;

  CAN_LOCKED_PAGED_CODE();

  IoAcquireCancelSpinLock(&oldIrql);

  // We don't know how long the irp will be in the
  // queue.  So we need to handle cancel.
  if(pIrp->Cancel)
  {
    IoReleaseCancelSpinLock(oldIrql);
    return STATUS_CANCELLED;
  }

  not_busy = (IsListEmpty(&pDevExt->WriteQueue)) && !(pDevExt->CurrentWriteIrp) 
    && !(pDevExt->Controller.transmitterBusy);

  if(!not_busy)
  {
    IoReleaseCancelSpinLock(oldIrql);
    return STATUS_CANCELLED;
  }

  ioctl_sync.pExtension = pDevExt;
  ioctl_sync.pData = pIrp;
  KeSynchronizeExecution(pDevExt->Interrupt, CanPostTransmission, &ioctl_sync);
  IoReleaseCancelSpinLock(oldIrql);

  // We got all we needed for this request. 
  F_DBG1("CanPostMessage> res(%u)\n", ioctl_sync.status);

  return ioctl_sync.status;
}
//-----------------------------------------------------------------------------
BOOLEAN CanPostTransmission(IN PVOID Context)
{
  PCAN_IOCTL_SYNC pSync = (PCAN_IOCTL_SYNC)Context;
  PCAN_DEVICE_EXTENSION pDevExt = pSync->pExtension;
  PIRP pIrp = (PIRP)pSync->pData;

  CAN_LOCKED_PAGED_CODE();

  if(pDevExt->WriteLength == 0)
  {
    PF_CAN_MSG pMsg = &((PF_CAN_TX)pIrp->AssociatedIrp.SystemBuffer)->msg;
    RtlMoveMemory(&pDevExt->PostedMsg, pMsg, sizeof(F_CAN_MSG));

    pDevExt->WriteLength = 1;
    pDevExt->WriteCurrentFrame = &pDevExt->PostedMsg;

    sja1000_start_xmit(&pDevExt->Controller, pDevExt->WriteCurrentFrame);
    
    pSync->status = STATUS_SUCCESS;
  }
  else
  {
    pSync->status = STATUS_CANCELLED;
  }

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This function is used to cancel the current write
//    (the current irp for write or the posted transmition).
//
//Arguments:
//
//    pDevExt - A pointer to the can device extension.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanKillCurrentWrite(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  KIRQL cancelIrql;

  CAN_LOCKED_PAGED_CODE();

  // We acquire the cancel spin lock.  This will prevent the
  // irps from moving around.
  IoAcquireCancelSpinLock(&cancelIrql);

  // Now go after the current write IRP if it's there.
  if(pDevExt->CurrentWriteIrp)
  {
    PDRIVER_CANCEL cancelRoutine;
    
    cancelRoutine = pDevExt->CurrentWriteIrp->CancelRoutine;
    pDevExt->CurrentWriteIrp->Cancel = TRUE;

    // If the current irp is not in a cancelable state
    // then it *will* try to enter one and the above
    // assignment will kill it.  If it already is in
    // a cancelable state then the following will kill it.
    if(cancelRoutine)
    {
      pDevExt->CurrentWriteIrp->CancelRoutine = NULL;
      pDevExt->CurrentWriteIrp->CancelIrql = cancelIrql;

      // This irp is already in a cancelable state.  We simply
      // mark it as canceled and call the cancel routine for it.
      cancelRoutine(pDevExt->DeviceObject, pDevExt->CurrentWriteIrp);
    }
    else
    {
      IoReleaseCancelSpinLock(cancelIrql);
    }
  }
  else
  {
    // Now go after the posted transmittion if it's there.
    KeSynchronizeExecution(pDevExt->Interrupt, CanAbortTransmission, pDevExt);

    CanStartQueuedWrite(pDevExt, cancelIrql);
  }
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Abort posted transmission on the can device if it's there.
//
//    NOTE: This routine is called by KeSynchronizeExecution.
//
//    NOTE: This routine assumes that it is called with the
//          cancel spin lock held.
//
//Arguments:
//
//    Context - Really a pointer to the device pDevExt.
//
//Return Value:
//
//    This routine always returns FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanAbortTransmission(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)Context;

  CAN_LOCKED_PAGED_CODE();

  if(pDevExt->WriteLength)
  {
    sja1000_abort_transmission(&pDevExt->Controller);
    pDevExt->WriteLength = 0;
  }

  return FALSE;
}
//-----------------------------------------------------------------------------

#endif