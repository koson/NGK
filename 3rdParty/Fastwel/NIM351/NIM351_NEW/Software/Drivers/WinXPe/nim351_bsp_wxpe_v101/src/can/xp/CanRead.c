#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

#include <eal/el.h>
#include <can/xp/CanDrv.h>

NTSTATUS CanStartRead(IN PCAN_DEVICE_EXTENSION pDevExt);
BOOLEAN CanUpdateInterruptBuffer(IN PVOID Context);
BOOLEAN CanUpdateSwitchToUser(IN PVOID Context);
VOID CanCancelCurrentRead(PDEVICE_OBJECT DeviceObject, PIRP Irp);
BOOLEAN CanGrabReadFromIsr(IN PVOID Context);

#ifdef ALLOC_PRAGMA
#pragma alloc_text(PAGEFCAN,CanRead)
#pragma alloc_text(PAGEFCAN,CanStartRead)
#pragma alloc_text(PAGEFCAN,CanUpdateInterruptBuffer)
#pragma alloc_text(PAGEFCAN,CanCancelCurrentRead)
#pragma alloc_text(PAGEFCAN,CanGrabReadFromIsr)
#endif

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
//Routine Description:
//
//    This is the dispatch routine for reading.  It validates the parameters
//    for the read request and if all is ok then it places the request
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
//    otherwise this routine will return the status returned by
//    the actual start read routine.
//-----------------------------------------------------------------------------
NTSTATUS CanRead(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
  NTSTATUS status;
  ULONG read_length;

  CAN_LOCKED_PAGED_CODE();

  F_DBG2(">CanRead(%X, %X)\n", DeviceObject, Irp);

  if( pDevExt->Flags & CAN_FLAGS_STOPPED )
  {
    F_DBG("In CanRead() CAN_FLAGS_STOPPED set, cancel operation\n" ); 
    Irp->IoStatus.Status = STATUS_CANCELLED;
    Irp->IoStatus.Information = 0L;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_CANCELLED;
  }

  if( !pDevExt->Flags & CAN_FLAGS_STARTED )
  {
    F_DBG("In CanRead() CAN_FLAGS_STARTED isn't set, cancel operation\n" ); 
    Irp->IoStatus.Status = STATUS_CANCELLED;
    Irp->IoStatus.Information = 0L;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_CANCELLED;
  }

  do
  {
    if((status = CanIRPPrologue(Irp, pDevExt)) != STATUS_SUCCESS)
    {
      if(status != STATUS_PENDING)
      {
        CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
      }
      break;
    }

    if(CanCompleteIfError(DeviceObject, Irp) != STATUS_SUCCESS)
    {
      status = STATUS_CANCELLED;
      break;
    }

    Irp->IoStatus.Information = 0L;

    // Quick check for a zero length read.  If it is zero length
    // then we are already done!
    read_length = IoGetCurrentIrpStackLocation(Irp)->Parameters.Read.Length;
    if(read_length)
    {
      // If it is small user buffer, then cancel the irp.
      if(read_length < sizeof(F_CAN_RX))
      {
        Irp->IoStatus.Status = STATUS_CANCELLED;
        CanCompleteRequest(pDevExt, Irp, 0);
        status = STATUS_CANCELLED;
        break;
      }

      // Well it looks like we actually have to do some work.
      // Put the read on the queue so that we can
      // process it when our previous reads are done.
      status = CanStartOrQueue(pDevExt, Irp, &pDevExt->ReadQueue, &pDevExt->CurrentReadIrp, CanStartRead);
      break;
    }

    Irp->IoStatus.Status = STATUS_SUCCESS;
    CanCompleteRequest(pDevExt, Irp, 0);
    status = STATUS_SUCCESS;
  }
  while(0);

  F_DBG1("<CanRead: status(%X)\n", status);
  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to start off any read.  It initializes
//    the Iostatus fields of the irp.  It will set up timer
//    that is used to control the read.  It will attempt to complete
//    the read from data already in the interrupt buffer.  If the
//    read can be completed quickly it will start off another if
//    necessary.
//
//Arguments:
//
//    pDevExt - Simply a pointer to the can device extension.
//
//Return Value:
//
//    This routine will return the status of the first read
//    irp.  This is useful in that if we have a read that can
//    complete right away (AND there had been nothing in the
//    queue before it) the read could return SUCCESS and the
//    application won't have to do a wait.
//-----------------------------------------------------------------------------
NTSTATUS CanStartRead(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  PIRP newIrp;
  KIRQL oldIrql;
  KIRQL controlIrql;

  ULONG sz_read_buffer;
  CAN_UPDATE_INPUT can_update_input;

  F_CAN_TIMEOUTS timeouts;
  BOOLEAN useTotalTimer;
  LARGE_INTEGER totalTime;
  BOOLEAN returnWithWhatsPresent;

  BOOLEAN setFirstStatus = FALSE;
  NTSTATUS firstStatus;

  CAN_LOCKED_PAGED_CODE();

  F_DBG1(">CanStartRead(%X)\n", pDevExt);

  do
  {
    sz_read_buffer = IoGetCurrentIrpStackLocation(pDevExt->CurrentReadIrp)->Parameters.Read.Length;

    // Calculate the timeout value needed for the current request.
    // Note that the values stored in the timeout record are in milliseconds.
    useTotalTimer = FALSE;
    returnWithWhatsPresent = FALSE;

    // Always initialize the timer objects so that the
    // completion code can tell when it attempts to
    // cancel the timers whether the timers had ever been Set.
    //
    // What we want to do is just make sure the timers are
    // cancelled to the best of our ability and move on with life.
    CanCancelTimer(&pDevExt->ReadRequestTotalTimer, pDevExt);

    // We get the *current* timeout values to use for timing this read.
    KeAcquireSpinLock(&pDevExt->ControlLock, &controlIrql);
    timeouts = pDevExt->Timeouts;
    KeReleaseSpinLock(&pDevExt->ControlLock, controlIrql);

    // We need to do special return quickly stuff here.
    // 1) If ReadTotalTimeout is 0 then we return immediately 
    //    with whatever we've got, even if it was zero.
    // 2) If ReadTotalTimeout is not MAXULONG
    //    then return immediately if any frames are present,
    //    but if nothing is there, then use the timeout as specified.
    // 3) If ReadTotalTimeout is MAXULONG then do as in
    //    "2" but return when the first frame arrives.
    if(!timeouts.ReadTotalTimeout)
    {
      returnWithWhatsPresent = TRUE;
    }
    else if(timeouts.ReadTotalTimeout != MAXULONG)
    {
      useTotalTimer = TRUE;
      totalTime.QuadPart = ((LONGLONG)(timeouts.ReadTotalTimeout)) * -10000;
    }
    
    // We need to protect with a spinlock since we don't want a purge to hose us.
    KeAcquireSpinLock(&pDevExt->ControlLock, &controlIrql);

    can_update_input.framesCopied = 0;
    can_update_input.pExtension = pDevExt;

    // See if we have to return immediately.
    if(returnWithWhatsPresent)
    {
      // We got all we needed for this read. 
      // Copy data (if present) and update the number of frames in the interrupt read buffer.
      KeSynchronizeExecution(pDevExt->Interrupt, CanUpdateInterruptBuffer, &can_update_input);
      KeReleaseSpinLock(&pDevExt->ControlLock, controlIrql);
      pDevExt->CurrentReadIrp->IoStatus.Status = STATUS_SUCCESS;
      if(!setFirstStatus)
      {
        firstStatus = STATUS_SUCCESS;
        setFirstStatus = TRUE;
      }
    }
    else
    {
      // The irp might go under control of the isr. 
      // It won't hurt to initialize the reference count right now.
      CAN_INIT_REFERENCE(pDevExt->CurrentReadIrp);
      IoAcquireCancelSpinLock(&oldIrql);

      // We need to see if this irp should be canceled.
      if(pDevExt->CurrentReadIrp->Cancel)
      {
        IoReleaseCancelSpinLock(oldIrql);
        KeReleaseSpinLock(&pDevExt->ControlLock, controlIrql);
        pDevExt->CurrentReadIrp->IoStatus.Status = STATUS_CANCELLED;
        pDevExt->CurrentReadIrp->IoStatus.Information = 0;
        if(!setFirstStatus)
        {
          firstStatus = STATUS_CANCELLED;
          setFirstStatus = TRUE;
        }
      }
      else
      {
        // We need to get data for this read.
        // Synchronize with the isr so that we can update the inputs and if necessary
        // it will have the isr switch to copying into the users buffer.
        KeSynchronizeExecution(pDevExt->Interrupt, CanUpdateSwitchToUser, &can_update_input);
        if(!can_update_input.framesCopied)
        {
          // The irp still isn't complete. The completion routines will end up
          // reinvoking this routine. So we simply leave.
          // First thought we should start off the total
          // timer for the read and increment the reference
          // count that the total timer has on the current irp.
          // Note that this is safe, because even if
          // the io has been satisfied by the isr it can't
          // complete yet because we still own the cancel spinlock.
          if(useTotalTimer)
          {
            CAN_SET_REFERENCE(pDevExt->CurrentReadIrp, CAN_REF_TOTAL_TIMER);
            CanSetTimer(&pDevExt->ReadRequestTotalTimer, totalTime, &pDevExt->TotalReadTimeoutDpc, pDevExt);
          }
          IoMarkIrpPending(pDevExt->CurrentReadIrp);
          IoReleaseCancelSpinLock(oldIrql);
          KeReleaseSpinLock(&pDevExt->ControlLock, controlIrql);
          if(!setFirstStatus)
          {
            firstStatus = STATUS_PENDING;
          }
          return firstStatus;
        }
        else
        {
          IoReleaseCancelSpinLock(oldIrql);
          KeReleaseSpinLock(&pDevExt->ControlLock, controlIrql);
          pDevExt->CurrentReadIrp->IoStatus.Status = STATUS_SUCCESS;
          if(!setFirstStatus)
          {
            firstStatus = STATUS_SUCCESS;
            setFirstStatus = TRUE;
          }
        }
      }
    }

    // Well the operation is complete.
    CanGetNextIrp(&pDevExt->CurrentReadIrp, &pDevExt->ReadQueue, &newIrp, TRUE, pDevExt);
  }
  while(newIrp);

  F_DBG1("<CanStartRead %X\n", firstStatus);
  return firstStatus;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to copy data out of the interrupt
//    buffer into the users buffer and to update the number of frames that
//    remain in the interrupt buffer.
//    We need to use this routine since the count could be updated during the 
//    update by executionof the ISR.

//    NOTE: This is called by KeSynchronizeExecution.
//
//Arguments:
//
//    Context - Points to a structure that contains a pointer to the
//              device extension and count of the number of frames
//              that we copied into the users buffer.
//
//Return Value:
//
//    Always FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanUpdateInterruptBuffer(IN PVOID Context)
{
  PCAN_UPDATE_INPUT pUpdate = (PCAN_UPDATE_INPUT)Context;
  PCAN_DEVICE_EXTENSION pDevExt = pUpdate->pExtension;
  // This value will be the number of frames that this
  // routine copy.  It will be the minimum of the number
  // of frames currently in the buffer or the number of
  // frames required for the read.
  ULONG nFramesToGet;
  // This value will be the number of frames
  // required for the read.
  ULONG nFramesNeeded;

  CAN_LOCKED_PAGED_CODE();

  nFramesToGet = (ULONG)(pDevExt->wrInterruptReadBuffer - pDevExt->rdInterruptReadBuffer);

  // The minimum of the number of frames we need and
  // the number of frames available
  nFramesNeeded = (IoGetCurrentIrpStackLocation(pDevExt->CurrentReadIrp))->
    Parameters.Read.Length / sizeof(F_CAN_RX); 
  if(nFramesToGet > nFramesNeeded)
    nFramesToGet = nFramesNeeded;

  if(nFramesToGet)
  {
    // Read Index in the interrupt buffer
    ULONG iRead = pDevExt->rdInterruptReadBuffer % pDevExt->szInterruptReadBuffer;
    // This will hold the number of frames between the
    // first available frame and the end of the buffer.
    // Note that the buffer could wrap around.
    ULONG nFramesFirstCopy = pDevExt->szInterruptReadBuffer - iRead;
    ULONG nFramesLastCopy = 0;

    if(nFramesFirstCopy > nFramesToGet)
      nFramesFirstCopy = nFramesToGet;

    RtlMoveMemory(
      pDevExt->CurrentReadIrp->AssociatedIrp.SystemBuffer, 
      &pDevExt->InterruptReadBuffer[iRead], 
      nFramesFirstCopy * sizeof(F_CAN_RX));
    
    nFramesLastCopy = nFramesToGet - nFramesFirstCopy;
    if(nFramesLastCopy)
    {
      RtlMoveMemory(
        ((PF_CAN_RX)(pDevExt->CurrentReadIrp->AssociatedIrp.SystemBuffer)) + nFramesFirstCopy, 
        &pDevExt->InterruptReadBuffer[0], 
        nFramesLastCopy * sizeof(F_CAN_RX));
    }

    // Update read position in the interrupt read buffer
    pDevExt->rdInterruptReadBuffer += nFramesToGet;
  }

  if(pDevExt->rdInterruptReadBuffer == pDevExt->wrInterruptReadBuffer)
  {
    F_CAN_ClearStatus(pDevExt->Controller.status, CAN_STATUS_RXBUF);
  }

  pDevExt->CurrentReadIrp->IoStatus.Information = nFramesToGet * sizeof(F_CAN_RX);
  pUpdate->framesCopied = nFramesToGet;

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine tried to get frames from the interrupt buffer.
//    If the interrupt buffer is empty it will then we set things up 
//    so that the ISR uses the user buffer copy into.
//
//    This routine is also used to update a count that is maintained
//    by the ISR to keep track of the number of frames in its buffer.
//
//    NOTE: This is called by KeSynchronizeExecution.
//
//Arguments:
//
//    Context - Points to a structure that contains a pointer to the
//              device extension, a count of the number of frames
//              that we copied into the users buffer.
//
//Return Value:
//
//    Always FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanUpdateSwitchToUser(IN PVOID Context)
{
  PCAN_UPDATE_INPUT pUpdate = Context;
  PCAN_DEVICE_EXTENSION pDevExt = pUpdate->pExtension;

  CAN_LOCKED_PAGED_CODE();

  // Copy inputs that have arrived since we got the last batch.
  CanUpdateInterruptBuffer(Context);

  if(!pUpdate->framesCopied)
  {
    // We shouldn't be switching unless there are no frames left.
    RTL_SOFT_ASSERT(!((ULONG)(pDevExt->wrInterruptReadBuffer - pDevExt->rdInterruptReadBuffer)));

    // By compareing the read buffer base address to the
    // the base address of the interrupt buffer the ISR
    // can determine whether we are using the interrupt
    // buffer or the user buffer.
    pDevExt->ReadBufferBase = (PF_CAN_RX)(pDevExt->CurrentReadIrp->AssociatedIrp.SystemBuffer);

    // Mark the irp as being in a cancelable state.
    IoSetCancelRoutine(pDevExt->CurrentReadIrp, CanCancelCurrentRead);

    // Increment the reference count twice.
    // Once for the Isr owning the irp and once
    // because the cancel routine has a reference to it.
    CAN_SET_REFERENCE(pDevExt->CurrentReadIrp, CAN_REF_ISR);
    CAN_SET_REFERENCE(pDevExt->CurrentReadIrp, CAN_REF_CANCEL);
  }
    
  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to cancel the current read.
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
VOID CanCancelCurrentRead(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
    
  CAN_LOCKED_PAGED_CODE();

  CanTryToCompleteCurrent(
    pDevExt,
    CanGrabReadFromIsr,
    Irp->CancelIrql,
    STATUS_CANCELLED,
    &pDevExt->CurrentReadIrp,
    &pDevExt->ReadQueue,
    &pDevExt->ReadRequestTotalTimer,
    CanStartRead,
    CanGetNextIrp,
    CAN_REF_CANCEL,
    NULL
    );

}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to grab (if possible) the irp from the
//    isr.  If it finds that the isr still owns the irp it grabs
//    the ipr away (updating the number of characters copied into the
//    users buffer).  If it grabs it away it also decrements the
//    reference count on the irp since it no longer belongs to the
//    isr (and the dpc that would complete it).
//
//    NOTE: This routine assumes that if the current buffer that the
//          ISR is copying characters into is the interrupt buffer then
//          the dpc has already been queued.
//
//    NOTE: This routine is being called from KeSynchronizeExecution.
//
//    NOTE: This routine assumes that it is called with the cancel spin
//          lock held.
//
//Arguments:
//
//    Context - Really a pointer to the device extension.
//
//Return Value:
//
//    Always false.
//-----------------------------------------------------------------------------
BOOLEAN CanGrabReadFromIsr(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = Context;

  CAN_LOCKED_PAGED_CODE();

  // The current buffer is the "users" buffer. No inputs done.
  if(pDevExt->ReadBufferBase != pDevExt->InterruptReadBuffer)
  {
    pDevExt->CurrentReadIrp->IoStatus.Information = 0;

    // Switch back to the interrupt buffer.
    pDevExt->ReadBufferBase = pDevExt->InterruptReadBuffer;
    CAN_CLEAR_REFERENCE(pDevExt->CurrentReadIrp, CAN_REF_ISR);
  }
    
  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to complete a read because its total
//    timer has expired.
//
//Arguments:
//
//    Dpc - Not Used.
//
//    DeferredContext - Really points to the device extension.
//
//    SystemContext1 - Not Used.
//
//    SystemContext2 - Not Used.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanReadTimeout(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeferredContext;
  KIRQL oldIrql;

  UNREFERENCED_PARAMETER(SystemContext1);
  UNREFERENCED_PARAMETER(SystemContext2);


  F_DBG1(">CanReadTimeout(%X)\n", pDevExt);

  IoAcquireCancelSpinLock(&oldIrql);

  CanTryToCompleteCurrent(
    pDevExt,
    CanGrabReadFromIsr,
    oldIrql,
    STATUS_TIMEOUT,
    &pDevExt->CurrentReadIrp,
    &pDevExt->ReadQueue,
    &pDevExt->ReadRequestTotalTimer,
    CanStartRead,
    CanGetNextIrp,
    CAN_REF_TOTAL_TIMER,
    NULL
    );

  CanDpcEpilogue(pDevExt, Dpc);
  F_DBG("<CanReadTimeout\n");
}
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is merely used to complete any read that
//    ended up being used by the Isr.  It assumes that the
//    status and the information fields of the irp are already
//    correctly filled in.
//
//Arguments:
//
//    Dpc - Not Used.
//
//    DeferredContext - Really points to the device extension.
//
//    SystemContext1 - Not Used.
//
//    SystemContext2 - Not Used.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanCompleteRead(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeferredContext;
  KIRQL oldIrql;

  UNREFERENCED_PARAMETER(SystemContext1);
  UNREFERENCED_PARAMETER(SystemContext2);


  F_DBG1(">CanCompleteRead(%X)\n", pDevExt);

  IoAcquireCancelSpinLock(&oldIrql);

  CanTryToCompleteCurrent(
    pDevExt,
    NULL,
    oldIrql,
    STATUS_SUCCESS,
    &pDevExt->CurrentReadIrp,
    &pDevExt->ReadQueue,
    &pDevExt->ReadRequestTotalTimer,
    CanStartRead,
    CanGetNextIrp,
    CAN_REF_ISR,
    NULL
    );

  CanDpcEpilogue(pDevExt, Dpc);

  F_DBG1("<CanCompleteRead(%X)\n", pDevExt);
}
//-----------------------------------------------------------------------------

#endif