#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

#include <eal/el.h>
#include <can/xp/CanDrv.h>

NTSTATUS CanStartWrite(IN PCAN_DEVICE_EXTENSION pDevExt);
BOOLEAN CanRequestTransmission(IN PVOID Context);
VOID CanCancelCurrentWrite(PDEVICE_OBJECT DeviceObject, PIRP Irp);
VOID CanGetNextWrite(
                     IN PIRP *CurrentOpIrp,
                     IN PLIST_ENTRY QueueToProcess,
                     IN PIRP *NewIrp,
                     IN BOOLEAN CompleteCurrent,
                     PCAN_DEVICE_EXTENSION pDevExt
                     );
BOOLEAN CanGrabWriteFromIsr(IN PVOID Context);
VOID CanFinishWrite(IN PCAN_DEVICE_EXTENSION pDevExt);
BOOLEAN CanFinishTransmition(IN PVOID Context);

#ifdef ALLOC_PRAGMA
#pragma alloc_text(PAGEFCAN,CanWrite)
#pragma alloc_text(PAGEFCAN,CanStartWrite)
#pragma alloc_text(PAGEFCAN,CanGetNextWrite)
#pragma alloc_text(PAGEFCAN,CanRequestTransmission)
#pragma alloc_text(PAGEFCAN,CanCancelCurrentWrite)
#pragma alloc_text(PAGEFCAN,CanGrabWriteFromIsr)
#pragma alloc_text(PAGEFCAN,CanSetResetMode)
#pragma alloc_text(PAGEFCAN,CanFinishWrite)
#pragma alloc_text(PAGEFCAN,CanFinishTransmition)
#pragma alloc_text(PAGEFCAN,CanStartQueuedWrite)
#endif

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
//Routine Description:
//
//    This places the hardware in a reset mode.
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
BOOLEAN CanSetResetMode(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)Context;

  sja1000_set_reset_mode(&pDevExt->Controller);
  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This replaces the hardware in a normal mode in bus-off recovery.
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
BOOLEAN CanRestart(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)Context;
  F_CAN_STATE state = pDevExt->Controller.state;

  // We first need to test if we are actually still doing
  // restart controller.  If we aren't then
  // we have no reason to try be here.

  if(state==CAN_STATE_BUS_OFF || state==CAN_STATE_STOPPED)
  {
    sja1000_start(&pDevExt->Controller);

    if(pDevExt->WriteLength)
    {
      // We not finished transmitting current frame
      sja1000_start_xmit(&pDevExt->Controller, pDevExt->WriteCurrentFrame);
    }

    if((pDevExt->Controller.settings.opmode & CAN_OPMODE_ERRFRAME) && (pDevExt->Controller.settings.error_mask & CAN_ERR_RESTARTED))
    {
      F_CAN_RX ef;

      ef.timestamp = CanGetTimeStamp(pDevExt);
      ef.msg.can_id = CAN_ERR_FLAG | CAN_ERR_RESTARTED;
      ef.msg.can_dlc = CAN_ERR_DLC;
      RtlZeroMemory(ef.msg.data, 8);

      CanPutRx(pDevExt, &ef);
    }

    pDevExt->Controller.stats.restarts++;
    state = pDevExt->Controller.state;
    if((state==CAN_STATE_BUS_OFF || state==CAN_STATE_STOPPED) && pDevExt->RestartMs)
    {
      CanInsertQueueDpc(&pDevExt->CanRecoverDpc, NULL, NULL, pDevExt)? pDevExt->CountOfTryingToRestartController++: 0;
    }
  }

  // We decement the counter to indicate that we've reached
  // the end of the execution path that is trying to restart controller
  pDevExt->CountOfTryingToRestartController--;

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine completes the old write as well as getting
//    a pointer to the next write.
//
//    The reason that we have have pointers to the current write
//    queue as well as the current write irp is so that this
//    routine may be used in the common completion code for
//    read and write.
//
//Arguments:
//
//    CurrentOpIrp - Pointer to the pointer that points to the
//                   current write irp.
//
//    QueueToProcess - Pointer to the write queue.
//
//    NewIrp - A pointer to a pointer to the irp that will be the
//             current irp.  Note that this could end up pointing
//             to a null pointer.  This does NOT necessaryly mean
//             that there is no current write.  What could occur
//             is that while the cancel lock is held the write
//             queue ended up being empty, but as soon as we release
//             the cancel spin lock a new irp came in from
//             CanStartWrite.
//
//    CompleteCurrent - Flag indicates whether the CurrentOpIrp should
//                      be completed.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanGetNextWrite(
                     IN PIRP *CurrentOpIrp,
                     IN PLIST_ENTRY QueueToProcess,
                     IN PIRP *NewIrp,
                     IN BOOLEAN CompleteCurrent,
                     PCAN_DEVICE_EXTENSION pDevExt
                     )
{
  CAN_LOCKED_PAGED_CODE();

  do
  {
    if(*CurrentOpIrp)
    {
      PIO_STACK_LOCATION irpSp = IoGetCurrentIrpStackLocation(*CurrentOpIrp);
      
      // We could be completing a flush.
      if(irpSp->MajorFunction == IRP_MJ_WRITE)
      {
        ULONG wr_frames = irpSp->Parameters.Write.Length / sizeof(F_CAN_MSG);
        KIRQL OldIrql;

        RTL_SOFT_ASSERT(pDevExt->TotalFramesQueued >=wr_frames);
        
        IoAcquireCancelSpinLock(&OldIrql);
        pDevExt->TotalFramesQueued -= wr_frames;
        IoReleaseCancelSpinLock(OldIrql);
      }
    }

    // Note that the following call will (probably) also cause
    // the current irp to be completed.
    CanGetNextIrp(CurrentOpIrp, QueueToProcess, NewIrp, CompleteCurrent, pDevExt);
    if(!*NewIrp)
    {
      break;
    }
#if 0   // Not supported
    else if(IoGetCurrentIrpStackLocation(*NewIrp)->MajorFunction == IRP_MJ_FLUSH_BUFFERS)
    {
      // If we encounter a flush request we just want to get
      // the next irp and complete the flush.
      // Note that if NewIrp is non-null then it is also
      // equal to CurrentWriteIrp.
      ASSERT((*NewIrp) == (*CurrentOpIrp));
      (*NewIrp)->IoStatus.Status = STATUS_SUCCESS;
    }
#endif
    else
    {
      break;
    }
  }
  while(TRUE);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Put transmission request to the can device.
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
BOOLEAN CanRequestTransmission(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = Context;
  PIO_STACK_LOCATION IrpSp;

  CAN_LOCKED_PAGED_CODE();

  IrpSp = IoGetCurrentIrpStackLocation(pDevExt->CurrentWriteIrp);

  pDevExt->WriteLength = IrpSp->Parameters.Write.Length / sizeof(F_CAN_MSG);
  pDevExt->WriteCurrentFrame = (PF_CAN_MSG)(pDevExt->CurrentWriteIrp->AssociatedIrp.SystemBuffer);
  // The isr now has a reference to the irp.
  CAN_SET_REFERENCE(pDevExt->CurrentWriteIrp, CAN_REF_ISR);
  
  sja1000_start_xmit(&pDevExt->Controller, pDevExt->WriteCurrentFrame);

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//
//    This routine is used to grab the current irp, which could be timing
//    out or canceling, from the ISR
//
//    NOTE: This routine is being called from KeSynchronizeExecution.
//
//    NOTE: This routine assumes that the cancel spin lock is held
//          when this routine is called.
//
//Arguments:
//
//    Context - Really a pointer to the device pDevExt.
//
//Return Value:
//
//    Always false.
//-----------------------------------------------------------------------------
BOOLEAN CanGrabWriteFromIsr(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = Context;
    
  CAN_LOCKED_PAGED_CODE();

  // Check if the write length is non-zero.  If it is non-zero then 
  // the ISR still owns the irp. We calculate the number of bytes written 
  // and update the information field of the irp with the bytes written.
  // We then clear the write length the isr sees.
  if(pDevExt->WriteLength)
  {
    if(pDevExt->CurrentWriteIrp)
    {
      ULONG wr_length = IoGetCurrentIrpStackLocation(pDevExt->CurrentWriteIrp)->Parameters.Write.Length;
      ULONG wr_rest = pDevExt->WriteLength*sizeof(F_CAN_MSG);

      RTL_SOFT_ASSERT(wr_length >= wr_rest);

      pDevExt->CurrentWriteIrp->IoStatus.Information = wr_length - wr_rest;

      // Since the isr no longer references this irp, we can
      // decrement it's reference count.
      CAN_CLEAR_REFERENCE(pDevExt->CurrentWriteIrp, CAN_REF_ISR);
    }

    sja1000_abort_transmission(&pDevExt->Controller);

    if((pDevExt->Controller.settings.opmode & CAN_OPMODE_ERRFRAME) && (pDevExt->Controller.settings.error_mask & CAN_ERR_TX_TIMEOUT))
    {
      F_CAN_RX ef;

      ef.timestamp = CanGetTimeStamp(pDevExt);
      ef.msg.can_id = CAN_ERR_FLAG | CAN_ERR_TX_TIMEOUT;
      ef.msg.can_dlc = CAN_ERR_DLC;
      RtlZeroMemory(&ef.msg.data[0], 8);
      (*(u32*)(ef.msg.data)) = pDevExt->WriteCurrentFrame->can_id;

      CanPutRx(pDevExt, &ef);
    }

    pDevExt->Controller.errors.tx_timeout++;
    F_CAN_SetStatus(pDevExt->Controller.status, CAN_STATUS_ERR);

    pDevExt->WriteLength = 0;
  }
  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This is the dispatch routine for write.  It validates the parameters
//    for the write request and if all is ok then it places the request
//    on the work queue.
//
//Arguments:
//
//    DeviceObject - Pointer to the device object for this device
//
//    Irp - Pointer to the IRP for the current request
//
//Return Value:
//
//    If the io is zero length then it will return STATUS_SUCCESS,
//    otherwise this routine will return STATUS_PENDING.
//-----------------------------------------------------------------------------
NTSTATUS CanWrite(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
  ULONG wr_length;
  NTSTATUS status;

  F_DBG1(">CanWrite() for %wZ\n", &pDevExt->SymbolicLinkName);
  
  if( pDevExt->Flags & CAN_FLAGS_STOPPED )
  {
    F_DBG("In CanWrite() CAN_FLAGS_STOPPED set, cancel operation\n" ); 
    Irp->IoStatus.Status = STATUS_CANCELLED;
    Irp->IoStatus.Information = 0L;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_CANCELLED;
  }

  if( !pDevExt->Flags & CAN_FLAGS_STARTED )
  {
    F_DBG("In CanWrite() CAN_FLAGS_STARTED isn't set, cancel operation\n" ); 
    Irp->IoStatus.Status = STATUS_CANCELLED;
    Irp->IoStatus.Information = 0L;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_CANCELLED;
  }
  CAN_LOCKED_PAGED_CODE();

  if((status = CanIRPPrologue(Irp, pDevExt)) != STATUS_SUCCESS)
  {
    if(status != STATUS_PENDING)
      CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
    return status;
  }

  if(CanCompleteIfError(DeviceObject, Irp) != STATUS_SUCCESS)
    return STATUS_CANCELLED;

  Irp->IoStatus.Information = 0L;

  // Quick check for a zero length write.  If it is zero length
  // then we are already done!
  wr_length = IoGetCurrentIrpStackLocation(Irp)->Parameters.Write.Length;
  if(wr_length == 0)
  {
    Irp->IoStatus.Status = STATUS_SUCCESS;
    Irp->IoStatus.Information = 0;
    CanCompleteRequest(pDevExt, Irp, 0);
    return STATUS_SUCCESS;
  }
  // If it is not multiple length of the F_CAN_MSG size,
  // then cancel the irp.
  if(wr_length % sizeof(F_CAN_MSG))
  {
    Irp->IoStatus.Status = STATUS_CANCELLED;
    Irp->IoStatus.Information = 0;
    CanCompleteRequest(pDevExt, Irp, 0);
    return STATUS_CANCELLED;
  }

#ifdef FMSG_ENABLE_DEBUG
  {
    PIO_STACK_LOCATION IrpSp;
    IrpSp = IoGetCurrentIrpStackLocation(Irp);
    F_DBG2("Write Length(%d), Write Buffer(%08X)\n", IrpSp->Parameters.Write.Length, Irp->AssociatedIrp.SystemBuffer);
  }
#endif

  // Well it looks like we actually have to do some
  // work.  Put the write on the queue so that we can
  // process it when our previous writes are done.
  status = CanStartOrQueue(pDevExt, Irp, &pDevExt->WriteQueue, &pDevExt->CurrentWriteIrp, CanStartWrite);
  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to start off any write.  It initializes
//    the Iostatus fields of the irp.  It will set up any timers
//    that are used to control the write.
//
//Arguments:
//
//    pDevExt - Points to the can device pDevExt
//
//Return Value:
//
//    This routine will return STATUS_PENDING for all writes
//    other than those that we find are cancelled.
//-----------------------------------------------------------------------------
NTSTATUS CanStartWrite(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  PIRP NewIrp = NULL;
  KIRQL OldIrql;
  LARGE_INTEGER TotalTime;
  BOOLEAN UseATimer;
  F_CAN_TIMEOUTS Timeouts;
  ULONG n_wr_frames;
  BOOLEAN SetFirstStatus = FALSE;
  NTSTATUS FirstStatus;

  CAN_LOCKED_PAGED_CODE();

  F_DBG1(">CanStartWrite(%wZ)\n", &pDevExt->SymbolicLinkName);

  do
  {
    UseATimer = FALSE;

    // Calculate the timeout value needed for the request.  
    // Note that the values stored in the timeout record are in milliseconds.  
    // Note that if the timeout values are zero then we won't start the timer.
    KeAcquireSpinLock(&pDevExt->ControlLock, &OldIrql);
    Timeouts = pDevExt->Timeouts;
    KeReleaseSpinLock(&pDevExt->ControlLock, OldIrql);

    if(Timeouts.WriteTotalTimeoutConstant || Timeouts.WriteTotalTimeoutMultiplier)
    {
      PIO_STACK_LOCATION IrpSp = IoGetCurrentIrpStackLocation(pDevExt->CurrentWriteIrp);
      n_wr_frames = IrpSp->Parameters.Write.Length / sizeof(F_CAN_MSG);
      UseATimer = TRUE;

      // We have some timer values to calculate.
      TotalTime.QuadPart = ((LONGLONG)(UInt32x32To64(n_wr_frames, Timeouts.WriteTotalTimeoutMultiplier)
        + Timeouts.WriteTotalTimeoutConstant)) * -10000;
    }

    // The irp may be going to the isr shortly.  Now
    // is a good time to initialize its reference counts.
    CAN_INIT_REFERENCE(pDevExt->CurrentWriteIrp);

    // We need to see if this irp should be canceled.
    IoAcquireCancelSpinLock(&OldIrql);
    if(pDevExt->CurrentWriteIrp->Cancel)
    {
      IoReleaseCancelSpinLock(OldIrql);
      pDevExt->CurrentWriteIrp->IoStatus.Status = STATUS_CANCELLED;
      if(!SetFirstStatus)
      {
        FirstStatus = STATUS_CANCELLED;
        SetFirstStatus = TRUE;
      }
    }
    else
    {
      //if(UseATimer)
      {
        if(!SetFirstStatus)
        {
          // If we haven't set our first status, then
          // this is the only irp that could have possibly
          // not been on the queue.  (It could have been
          // on the queue if this routine is being invoked
          // from the completion routine.)  Since this
          // irp might never have been on the queue we
          // should mark it as pending.
          IoMarkIrpPending(pDevExt->CurrentWriteIrp);
          SetFirstStatus = TRUE;
          FirstStatus = STATUS_PENDING;
        }

        // We give the irp to the isr to write out.
        // We set a cancel routine that knows how to
        // grab the current write away from the isr.
        //
        // Since the cancel routine has an implicit reference
        // to this irp up the reference count.
        IoSetCancelRoutine(pDevExt->CurrentWriteIrp, CanCancelCurrentWrite);
        CAN_SET_REFERENCE(pDevExt->CurrentWriteIrp, CAN_REF_CANCEL);

        if(UseATimer)
        {
        CanSetTimer(&pDevExt->WriteRequestTotalTimer, TotalTime, &pDevExt->TotalWriteTimeoutDpc, pDevExt);
        // This timer now has a reference to the irp.
        CAN_SET_REFERENCE(pDevExt->CurrentWriteIrp, CAN_REF_TOTAL_TIMER);
        }

        F_DBG("Request transmission\n");
        KeSynchronizeExecution(pDevExt->Interrupt, CanRequestTransmission, pDevExt);
        IoReleaseCancelSpinLock(OldIrql);
        break;
      }
      //else
      //{
      //  PIRP postIrp = pDevExt->CurrentWriteIrp;
      //  CAN_UPDATE_INPUT can_update_input;

      //  can_update_input.framesCopied = 0;
      //  can_update_input.pExtension = pDevExt;

      //  KeSynchronizeExecution(pDevExt->Interrupt, CanPostTransmission, &can_update_input);
      //  IoReleaseCancelSpinLock(OldIrql);

      //  // We got all we needed for this write. 
      //  F_DBG1("Post msg(%d)\n", can_update_input.framesCopied);
      //  postIrp->IoStatus.Status = STATUS_SUCCESS;
      //  if(!SetFirstStatus)
      //  {
      //    FirstStatus = STATUS_SUCCESS;
      //    SetFirstStatus = TRUE;
      //  }
      //  CanCompleteRequest(pDevExt, postIrp, IO_NETWORK_INCREMENT);
      //  break;
      //}
    }

    // Well the write was canceled before we could start it up.
    // Try to get another.
    CanGetNextWrite(&pDevExt->CurrentWriteIrp, &pDevExt->WriteQueue, &NewIrp, TRUE, pDevExt);
  }
  while(NewIrp);

  return FirstStatus;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine will try to timeout the current write.
//
//Arguments:
//
//    Dpc - Not Used.
//
//    DeferredContext - Really points to the device pDevExt.
//
//    SystemContext1 - Not Used.
//
//    SystemContext2 - Not Used.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanWriteTimeout(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeferredContext;
  KIRQL OldIrql;

  UNREFERENCED_PARAMETER(SystemContext1);
  UNREFERENCED_PARAMETER(SystemContext2);

  F_DBG1(">CanWriteTimeout(%X)\n", pDevExt);
  IoAcquireCancelSpinLock(&OldIrql);

  CanTryToCompleteCurrent(
    pDevExt, 
    CanGrabWriteFromIsr,
    OldIrql,
    STATUS_TIMEOUT,
    &pDevExt->CurrentWriteIrp,
    &pDevExt->WriteQueue,
    &pDevExt->WriteRequestTotalTimer,
    CanStartWrite, 
    CanGetNextWrite,
    CAN_REF_TOTAL_TIMER,
    CanFinishWrite
    );

  CanDpcEpilogue(pDevExt, Dpc);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to cancel the current write.
//
//Arguments:
//
//    DeviceObject - Pointer to the device object for this device
//
//    Irp - Pointer to the IRP to be canceled.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanCancelCurrentWrite(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
    
  CAN_LOCKED_PAGED_CODE();

  CanTryToCompleteCurrent(
    pDevExt,
    CanGrabWriteFromIsr,
    Irp->CancelIrql,
    STATUS_CANCELLED,
    &pDevExt->CurrentWriteIrp,
    &pDevExt->WriteQueue,
    &pDevExt->WriteRequestTotalTimer,
    CanStartWrite,
    CanGetNextWrite,
    CAN_REF_CANCEL,
    CanFinishWrite
    );
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is merely used to complete any write.  It
//    assumes that the status and the information fields of
//    the irp are already correctly filled in.
//
//Arguments:
//
//    Dpc - Not Used.
//
//    DeferredContext - Really points to the device pDevExt.
//
//    SystemContext1 - Not Used.
//
//    SystemContext2 - Not Used.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanCompleteWrite(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeferredContext;
  KIRQL OldIrql;

  UNREFERENCED_PARAMETER(SystemContext1);
  UNREFERENCED_PARAMETER(SystemContext2);

  F_DBG1(">CanCompleteWrite(%X)\n", pDevExt);

  IoAcquireCancelSpinLock(&OldIrql);

  if(pDevExt->CurrentWriteIrp)
  {
    CanTryToCompleteCurrent(
      pDevExt, 
      NULL, 
      OldIrql, 
      STATUS_SUCCESS,
      &pDevExt->CurrentWriteIrp,
      &pDevExt->WriteQueue,
      &pDevExt->WriteRequestTotalTimer,
      CanStartWrite, 
      CanGetNextWrite,
      CAN_REF_ISR,
      CanFinishWrite
      );
  }
  else
  {
    // Start queued write on the can device if it's there.
    CanStartQueuedWrite(pDevExt, OldIrql);
  }

  CanDpcEpilogue(pDevExt, Dpc);
}
//-----------------------------------------------------------------------------
VOID CanFinishWrite(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  KIRQL OldIrql;

  CAN_LOCKED_PAGED_CODE();

  F_DBG1(">CanFinishWrite(%wZ)\n", &pDevExt->SymbolicLinkName);

  IoAcquireCancelSpinLock(&OldIrql);

  if((IsListEmpty(&pDevExt->WriteQueue)) && !(pDevExt->CurrentWriteIrp))
    KeSynchronizeExecution(pDevExt->Interrupt, CanFinishTransmition, pDevExt);
  
  IoReleaseCancelSpinLock(OldIrql);
}
//-----------------------------------------------------------------------------
BOOLEAN CanFinishTransmition(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = Context;

  CAN_LOCKED_PAGED_CODE();

  if(!pDevExt->WriteLength)
  {
    F_CAN_SetStatus(pDevExt->Controller.status, CAN_STATUS_TXBUF);
    pDevExt->Controller.transmitterBusy = FALSE;
    CanSignalEvent(pDevExt);
  }

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Start queued write on the can device if it's there.
//
//    NOTE: This routine assumes that the caller holds the cancel spinlock 
//          and we should release it when we're done.
//
//Arguments:
//
//    pDevExt - Really a pointer to the device pDevExt.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanStartQueuedWrite(IN PCAN_DEVICE_EXTENSION pDevExt, IN KIRQL OldIrql)
{
  PIRP newIrp;

  CAN_LOCKED_PAGED_CODE();

  CanGetNextWrite(&pDevExt->CurrentWriteIrp, &pDevExt->WriteQueue, &newIrp, FALSE, pDevExt);

  // Check to see if there is a new irp to start up.
  if(!IsListEmpty(&pDevExt->WriteQueue))
  {
    PLIST_ENTRY headOfList = RemoveHeadList(&pDevExt->WriteQueue);

    pDevExt->CurrentWriteIrp = CONTAINING_RECORD(headOfList, IRP, Tail.Overlay.ListEntry);
    IoSetCancelRoutine(pDevExt->CurrentWriteIrp, NULL);
  }

  if(pDevExt->CurrentWriteIrp)
  {
    IoReleaseCancelSpinLock(OldIrql);
    CanStartWrite(pDevExt);
  }
  else
  {
    KeSynchronizeExecution(pDevExt->Interrupt, CanFinishTransmition, pDevExt);
    IoReleaseCancelSpinLock(OldIrql);
  }
}
//-----------------------------------------------------------------------------

#endif