#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

#include <eal/el.h>
#include <can/xp/CanDrv.h>
#include <can/xp/canio.h>

VOID CanRundownIrpRefs(IN PIRP *CurrentOpIrp, IN PKTIMER TotalTimer OPTIONAL, IN PCAN_DEVICE_EXTENSION PDevExt);
VOID CanGetNextIrpLocked(IN PIRP *CurrentOpIrp, IN PLIST_ENTRY QueueToProcess, OUT PIRP *NextIrp,
                         IN BOOLEAN CompleteCurrent, IN PCAN_DEVICE_EXTENSION extension, IN KIRQL OldIrql);
BOOLEAN CanGetState(IN PVOID Context);

#ifdef ALLOC_PRAGMA
#pragma alloc_text(PAGEFCAN, CanGetNextIrp)
#pragma alloc_text(PAGEFCAN, CanGetNextIrpLocked)
#pragma alloc_text(PAGEFCAN, CanTryToCompleteCurrent)
#pragma alloc_text(PAGEFCAN, CanStartOrQueue)
#pragma alloc_text(PAGEFCAN, CanCancelQueued)
#pragma alloc_text(PAGEFCAN, CanCompleteIfError)
#pragma alloc_text(PAGEFCAN, CanRundownIrpRefs)
#pragma alloc_text(PAGEFCAN, CanGetControllerStatus)
#pragma alloc_text(PAGEFCAN, CanGetState)
#pragma alloc_text(PAGEFCAN, CanSignalEvent)
#pragma alloc_text(PAGEFCAN, CanGetTimeStamp)
//#pragma alloc_text(PAGE, CanLogError)
#endif

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine allocates an error log entry, copies the supplied data
//    to it, and requests that it be written to the error log file.
//
//Arguments:
//
//    DriverObject - A pointer to the driver object for the device.
//
//    DeviceObject - A pointer to the device object associated with the
//    device that had the error, early in initialization, one may not
//    yet exist.
//
//    SequenceNumber - A ulong value that is unique to an IRP over the
//    life of the irp in this driver - 0 generally means an error not
//    associated with an irp.
//
//    MajorFunctionCode - If there is an error associated with the irp,
//    this is the major function code of that irp.
//
//    RetryCount - The number of times a particular operation has been
//    retried.
//
//    UniqueErrorValue - A unique long word that identifies the particular
//    call to this function.
//
//    FinalStatus - The final status given to the irp that was associated
//    with this error.  If this log entry is being made during one of
//    the retries this value will be STATUS_SUCCESS.
//
//    SpecificIOStatus - The IO status for a particular error.
//
//    LengthOfInsert - The length in bytes (including the terminating NULL)
//                      of the insertion string.
//
//    Insert - The insertion string.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
#if 0
VOID CanLogError(IN PDRIVER_OBJECT DriverObject, IN PDEVICE_OBJECT DeviceObject OPTIONAL,
  IN ULONG SequenceNumber, IN UCHAR MajorFunctionCode, IN UCHAR RetryCount, IN ULONG UniqueErrorValue,
  IN NTSTATUS FinalStatus, IN NTSTATUS SpecificIOStatus,
  IN ULONG LengthOfInsert1, IN PWCHAR Insert1, IN ULONG LengthOfInsert2, IN PWCHAR Insert2)
{
  PIO_ERROR_LOG_PACKET errorLogEntry;
  PVOID objectToUse;
  SHORT dumpToAllocate = 0;
  PUCHAR ptrToFirstInsert;
  PUCHAR ptrToSecondInsert;

  PAGED_CODE();

  if(Insert1 == NULL)
    LengthOfInsert1 = 0;
  if(Insert2 == NULL)
    LengthOfInsert2 = 0;


  if(ARGUMENT_PRESENT(DeviceObject))
    objectToUse = DeviceObject;
  else
    objectToUse = DriverObject;

  errorLogEntry = IoAllocateErrorLogEntry(objectToUse, (UCHAR)(sizeof(IO_ERROR_LOG_PACKET)+LengthOfInsert1+LengthOfInsert2));

  if( errorLogEntry != NULL )
  {
    errorLogEntry->ErrorCode = SpecificIOStatus;
    errorLogEntry->SequenceNumber = SequenceNumber;
    errorLogEntry->MajorFunctionCode = MajorFunctionCode;
    errorLogEntry->RetryCount = RetryCount;
    errorLogEntry->UniqueErrorValue = UniqueErrorValue;
    errorLogEntry->FinalStatus = FinalStatus;
    errorLogEntry->DumpDataSize = dumpToAllocate;

    ptrToFirstInsert = (PUCHAR)&errorLogEntry->DumpData[0];
    ptrToSecondInsert = ptrToFirstInsert + LengthOfInsert1;

    if(LengthOfInsert1)
    {
      errorLogEntry->NumberOfStrings = 1;
      errorLogEntry->StringOffset = (USHORT)(ptrToFirstInsert - (PUCHAR)errorLogEntry);
      RtlCopyMemory(ptrToFirstInsert, Insert1, LengthOfInsert1);

      if(LengthOfInsert2)
      {
        errorLogEntry->NumberOfStrings = 2;
        RtlCopyMemory(ptrToSecondInsert, Insert2, LengthOfInsert2);
      }
    }
    IoWriteErrorLogEntry(errorLogEntry);
  }
}
#endif
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine will be used cancel irps on the stalled queue.
//
//Arguments:
//
//    PDevObj - Pointer to the device object.
//
//    PIrp - Pointer to the Irp to cancel
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanFilterCancelQueued(IN PDEVICE_OBJECT PDevObj, IN PIRP PIrp)
{
  PCAN_DEVICE_EXTENSION pDevExt = PDevObj->DeviceExtension;
  PIO_STACK_LOCATION pIrpSp = IoGetCurrentIrpStackLocation(PIrp);

  PIrp->IoStatus.Status = STATUS_CANCELLED;
  PIrp->IoStatus.Information = 0;

  RemoveEntryList(&PIrp->Tail.Overlay.ListEntry);
  IoReleaseCancelSpinLock(PIrp->CancelIrql);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine will be used to approve irps for processing.
//    If an irp is approved, success will be returned.  If not,
//    the irp will be queued or rejected outright.  The IoStatus struct
//    and return value will appropriately reflect the actions taken.
//
//Arguments:
//
//    PIrp - Pointer to the Irp to cancel
//
//    PDevExt - Pointer to the device extension
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
NTSTATUS CanFilterIrps(IN PIRP PIrp, IN PCAN_DEVICE_EXTENSION PDevExt)
{
  PIO_STACK_LOCATION pIrpStack;
  KIRQL oldIrqlFlags;

  pIrpStack = IoGetCurrentIrpStackLocation(PIrp);

  KeAcquireSpinLock(&PDevExt->FlagsLock, &oldIrqlFlags);

  if((PDevExt->DevicePNPAccept == CAN_PNPACCEPT_OK) && ((PDevExt->Flags & CAN_FLAGS_BROKENHW) == 0))
  {
    KeReleaseSpinLock(&PDevExt->FlagsLock, oldIrqlFlags);
    return STATUS_SUCCESS;
  }

  if((PDevExt->DevicePNPAccept & CAN_PNPACCEPT_REMOVING) 
    || (PDevExt->Flags & CAN_FLAGS_BROKENHW)
    || (PDevExt->DevicePNPAccept & CAN_PNPACCEPT_SURPRISE_REMOVING))
  {
    KeReleaseSpinLock(&PDevExt->FlagsLock, oldIrqlFlags);
    // Accept all PNP IRP's -- we assume PNP can synchronize itself
    if (pIrpStack->MajorFunction == IRP_MJ_PNP)
      return STATUS_SUCCESS;

    PIrp->IoStatus.Status = STATUS_DELETE_PENDING;
    return STATUS_DELETE_PENDING;
  }

  if((PDevExt->DevicePNPAccept & CAN_PNPACCEPT_STOPPING) || (PDevExt->DevicePNPAccept & CAN_PNPACCEPT_POWER_DOWN)) 
  {
    KIRQL oldIrql;
    KeReleaseSpinLock(&PDevExt->FlagsLock, oldIrqlFlags);
    // Accept all PNP IRP's -- we assume PNP can synchronize itself
    if (pIrpStack->MajorFunction == IRP_MJ_PNP) 
      return STATUS_SUCCESS;

    if((pIrpStack->MajorFunction == IRP_MJ_POWER) && (PDevExt->DevicePNPAccept & CAN_PNPACCEPT_POWER_DOWN)) 
      return STATUS_SUCCESS;

    IoAcquireCancelSpinLock(&oldIrql);

    if(PIrp->Cancel)
    {
      IoReleaseCancelSpinLock(oldIrql);
      PIrp->IoStatus.Status = STATUS_CANCELLED;
      return STATUS_CANCELLED;
    } 
    else 
    {
      // Mark the Irp as pending
       PIrp->IoStatus.Status = STATUS_PENDING;
       IoMarkIrpPending(PIrp);

       // Queue up the IRP
       InsertTailList(&PDevExt->StalledIrpQueue, &PIrp->Tail.Overlay.ListEntry);

       IoSetCancelRoutine(PIrp, CanFilterCancelQueued);
       IoReleaseCancelSpinLock(oldIrql);
       return STATUS_PENDING;
    }
  }

  KeReleaseSpinLock(&PDevExt->FlagsLock, oldIrqlFlags);

  return STATUS_SUCCESS;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This function must be called at any IRP dispatch entry point.  It,
//   with CanIRPEpilogue(), keeps track of all pending IRP's for the given
//   PDevObj.
//
//Arguments:
//
//   PDevObj - Pointer to the device object we are tracking pending IRP's for.
//
//Return Value:
//
//   Tentative status of the Irp.
//-----------------------------------------------------------------------------
NTSTATUS CanIRPPrologue(IN PIRP PIrp, IN PCAN_DEVICE_EXTENSION PDevExt)
{
  InterlockedIncrement(&PDevExt->PendingIRPCnt);
  return CanFilterIrps(PIrp, PDevExt);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This function must be called at any IRP dispatch entry point.  It,
//   with CanIRPPrologue(), keeps track of all pending IRP's for the given
//   PDevObj.
//
//Arguments:
//
//   PDevObj - Pointer to the device object we are tracking pending IRP's for.
//
//Return Value:
//
//   None.
//-----------------------------------------------------------------------------
VOID CanIRPEpilogue(IN PCAN_DEVICE_EXTENSION PDevExt)
{
  LONG pendingCnt;

  pendingCnt = InterlockedDecrement(&PDevExt->PendingIRPCnt);

  ASSERT(pendingCnt >= 0);
  if(pendingCnt == 0) 
    KeSetEvent(&PDevExt->PendingIRPEvent, IO_NO_INCREMENT, FALSE);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   Sets flags in a value protected by the flags spinlock.  This is used
//   to set values that would stop IRP's from being accepted.
//
//Arguments:
//   PDevExt - Device extension attached to PDevObj
//
//   PFlags - Pointer to the flags variable that needs changing
//
//   Value - Value to modify flags variable with
//
//   Set - TRUE if |= , FALSE if &=
//
//Return Value:
//
//   None.
//-----------------------------------------------------------------------------
VOID CanSetDeviceFlags(IN PCAN_DEVICE_EXTENSION PDevExt, OUT PULONG PFlags, IN ULONG Value, IN BOOLEAN Set)
{
  KIRQL oldIrql;

  KeAcquireSpinLock(&PDevExt->FlagsLock, &oldIrql);

  if(Set)
  {
    *PFlags |= Value;
  }
  else
  {
    *PFlags &= ~Value;
  }

  KeReleaseSpinLock(&PDevExt->FlagsLock, oldIrql);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This function must be called to queue DPC's for the can driver.
//
//Arguments:
//
//   PDpc thru Sarg2  - Standard args to KeInsertQueueDpc()
//
//   PDevExt - Pointer to the device extension for the device that needs to
//             queue a DPC
//
//Return Value:
//
//   Kicks up return value from KeInsertQueueDpc()
//-----------------------------------------------------------------------------
BOOLEAN CanInsertQueueDpc(IN PRKDPC PDpc, IN PVOID Sarg1, IN PVOID Sarg2, IN PCAN_DEVICE_EXTENSION PDevExt)
{
  BOOLEAN queued;

  InterlockedIncrement(&PDevExt->DpcCount);

  queued = KeInsertQueueDpc(PDpc, Sarg1, Sarg2);
  if(!queued)
  {
    ULONG pendingCnt = InterlockedDecrement(&PDevExt->DpcCount);
    if(pendingCnt == 0)
      KeSetEvent(&PDevExt->PendingIRPEvent, IO_NO_INCREMENT, FALSE);
  } 

  return queued;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This function is a DPC routine queue from the ISR if he released the
//   last lock on pending DPC's.
//
//Arguments:
//
//   PDpdc, PSysContext1, PSysContext2 -- not used
//
//   PDeferredContext -- Really the device extension
//
//Return Value:
//
//   None.
//-----------------------------------------------------------------------------
VOID CanUnlockPages(IN PKDPC PDpc, IN PVOID PDeferredContext, IN PVOID PSysContext1, IN PVOID PSysContext2)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)PDeferredContext;

  UNREFERENCED_PARAMETER(PDpc);
  UNREFERENCED_PARAMETER(PSysContext1);
  UNREFERENCED_PARAMETER(PSysContext2);

  KeSetEvent(&pDevExt->PendingDpcEvent, IO_NO_INCREMENT, FALSE);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This function must be called at the end of every dpc function.
//
//Arguments:
//
//   PDevObj - Pointer to the device object we are tracking dpc's for.
//
//Return Value:
//
//   None.
//-----------------------------------------------------------------------------
VOID CanDpcEpilogue(IN PCAN_DEVICE_EXTENSION PDevExt, PKDPC PDpc)
{
  LONG pendingCnt;

  UNREFERENCED_PARAMETER(PDpc);

  pendingCnt = InterlockedDecrement(&PDevExt->DpcCount);

  ASSERT(pendingCnt >= 0);

  if(pendingCnt == 0)
  {
    KeSetEvent(&PDevExt->PendingDpcEvent, IO_NO_INCREMENT, FALSE);
  }
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This function must be called to set timers for the can driver.
//
//Arguments:
//
//   Timer - pointer to timer dispatcher object
//
//   DueTime - time at which the timer should expire
//
//   Dpc - option Dpc
//
//   PDevExt - Pointer to the device extension for the device that needs to
//             set a timer
//
//Return Value:
//
//   Kicks up return value from KeSetTimer()
//-----------------------------------------------------------------------------
BOOLEAN CanSetTimer(IN PKTIMER Timer, IN LARGE_INTEGER DueTime, IN PKDPC Dpc OPTIONAL, IN PCAN_DEVICE_EXTENSION PDevExt)
{
  BOOLEAN set;

  InterlockedIncrement(&PDevExt->DpcCount);

  set = KeSetTimer(Timer, DueTime, Dpc);

  if(set)
  {
    InterlockedDecrement(&PDevExt->DpcCount);
  }

  return set;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This function must be called to cancel timers for the can driver.
//
//Arguments:
//
//   Timer - pointer to timer dispatcher object
//
//   PDevExt - Pointer to the device extension for the device that needs to
//             cancel a timer
//
//Return Value:
//
//   True if timer was cancelled
//-----------------------------------------------------------------------------
BOOLEAN CanCancelTimer(IN PKTIMER Timer, IN PCAN_DEVICE_EXTENSION PDevExt)
{
  BOOLEAN cancelled = KeCancelTimer(Timer);
   
  if(cancelled)
  {
    CanDpcEpilogue(PDevExt, Timer->Dpc);
  }

  return cancelled;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This function is used to cancel all queued and the current irps
//    for reads or for writes.
//
//Arguments:
//
//    DeviceObject - A pointer to the can device object.
//
//    QueueToClean - A pointer to the queue which we're going to clean out.
//
//    CurrentOpIrp - Pointer to a pointer to the current irp.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanKillAllReadsOrWrites(IN PDEVICE_OBJECT DeviceObject, IN PLIST_ENTRY QueueToClean, IN PIRP *CurrentOpIrp)
{
  KIRQL cancelIrql;
  PDRIVER_CANCEL cancelRoutine;

  // We acquire the cancel spin lock.  This will prevent the
  // irps from moving around.
  IoAcquireCancelSpinLock(&cancelIrql);

  // Clean the list from back to front.
  while(!IsListEmpty(QueueToClean))
  {
    PIRP currentLastIrp = CONTAINING_RECORD(QueueToClean->Blink, IRP, Tail.Overlay.ListEntry);

    RemoveEntryList(QueueToClean->Blink);
    cancelRoutine = currentLastIrp->CancelRoutine;
    currentLastIrp->CancelIrql = cancelIrql;
    currentLastIrp->CancelRoutine = NULL;
    currentLastIrp->Cancel = TRUE;

    cancelRoutine(DeviceObject, currentLastIrp);
    IoAcquireCancelSpinLock(&cancelIrql);
  }

  // The queue is clean.  Now go after the current if it's there.
  if(*CurrentOpIrp)
  {
    cancelRoutine = (*CurrentOpIrp)->CancelRoutine;
    (*CurrentOpIrp)->Cancel = TRUE;

    // If the current irp is not in a cancelable state
    // then it *will* try to enter one and the above
    // assignment will kill it.  If it already is in
    // a cancelable state then the following will kill it.
    if(cancelRoutine)
    {
      (*CurrentOpIrp)->CancelRoutine = NULL;
      (*CurrentOpIrp)->CancelIrql = cancelIrql;

      // This irp is already in a cancelable state.  We simply
      // mark it as canceled and call the cancel routine for it.
      cancelRoutine(DeviceObject, *CurrentOpIrp);
    }
    else
    {
      IoReleaseCancelSpinLock(cancelIrql);
    }
  }
  else
  {
    IoReleaseCancelSpinLock(cancelIrql);
  }
}
//-----------------------------------------------------------------------------
VOID CanKillAllStalled(IN PDEVICE_OBJECT PDevObj)
{
  KIRQL cancelIrql;
  PDRIVER_CANCEL cancelRoutine;
  PCAN_DEVICE_EXTENSION pDevExt = PDevObj->DeviceExtension;

  IoAcquireCancelSpinLock(&cancelIrql);
  while(!IsListEmpty(&pDevExt->StalledIrpQueue))
  {
    PIRP currentLastIrp = CONTAINING_RECORD(pDevExt->StalledIrpQueue.Blink, IRP, Tail.Overlay.ListEntry);

    RemoveEntryList(pDevExt->StalledIrpQueue.Blink);
    cancelRoutine = currentLastIrp->CancelRoutine;
    currentLastIrp->CancelIrql = cancelIrql;
    currentLastIrp->CancelRoutine = NULL;
    currentLastIrp->Cancel = TRUE;

    cancelRoutine(PDevObj, currentLastIrp);

    IoAcquireCancelSpinLock(&cancelIrql);
  }
  IoReleaseCancelSpinLock(cancelIrql);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This function must be called instead of IoCallDriver.  It automatically
//   updates Irp tracking for PDevObj.
//
//Arguments:
//   PDevExt - Device extension attached to PDevObj
//
//   PDevObj - Pointer to the device object we are tracking pending IRP's for.
//
//   PIrp - Pointer to the Irp we are passing to the next driver.
//
//Return Value:
//
//   None.
//-----------------------------------------------------------------------------
NTSTATUS CanIoCallDriver(PCAN_DEVICE_EXTENSION PDevExt, PDEVICE_OBJECT PDevObj, PIRP PIrp)
{
  NTSTATUS status;

  status = IoCallDriver(PDevObj, PIrp);
  CanIRPEpilogue(PDevExt);
  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine runs through the various items that *could*
//    have a reference to the current read/write.  It try's to kill
//    the reason.  If it does succeed in killing the reason it
//    will decrement the reference count on the irp.
//
//    NOTE: This routine assumes that it is called with the cancel
//          spin lock held.
//
//Arguments:
//
//    CurrentOpIrp - Pointer to a pointer to current irp for the
//                   particular operation.
//
//    TotalTimer - Pointer to the total timer for the operation.
//                 NOTE: This could be null.
//
//    PDevExt - Pointer to device extension
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanRundownIrpRefs(IN PIRP *CurrentOpIrp, IN PKTIMER TotalTimer OPTIONAL, IN PCAN_DEVICE_EXTENSION PDevExt)
{
  CAN_LOCKED_PAGED_CODE();

  // This routine is called with the cancel spin lock held
  // so we know only one thread of execution can be in here at one time.

  // First we see if there is still a cancel routine.  If
  // so then we can decrement the count by one.
  if((*CurrentOpIrp)->CancelRoutine)
  {
    CAN_CLEAR_REFERENCE(*CurrentOpIrp, CAN_REF_CANCEL);
    IoSetCancelRoutine(*CurrentOpIrp, NULL);
  }

  if(TotalTimer)
  {
    // Try to cancel the operations total timer.  If the operation
    // returns true then the timer did have a reference to the
    // irp.  Since we've canceled this timer that reference is
    // no longer valid and we can decrement the reference count.
    //
    // If the cancel returns false then this means either of two things:
    //
    // a) The timer has already fired.
    //
    // b) There never was an total timer.
    //
    // In the case of "b" there is no need to decrement the reference
    // count since the "timer" never had a reference to it.
    //
    // In the case of "a", then the timer itself will be coming
    // along and decrement it's reference.  Note that the caller
    // of this routine might actually be the this timer, but it
    // has already decremented the reference.
    if(CanCancelTimer(TotalTimer, PDevExt))
    {
      CAN_CLEAR_REFERENCE(*CurrentOpIrp, CAN_REF_TOTAL_TIMER);
    }
  }
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This function is used to make the head of the particular
//    queue the current irp.  It also completes the what
//    was the old current irp if desired.
//
//Arguments:
//
//    CurrentOpIrp - Pointer to a pointer to the currently active
//                   irp for the particular work list.  Note that
//                   this item is not actually part of the list.
//
//    QueueToProcess - The list to pull the new item off of.
//
//    NextIrp - The next Irp to process.  Note that CurrentOpIrp
//              will be set to this value under protection of the
//              cancel spin lock.  However, if *NextIrp is NULL when
//              this routine returns, it is not necessaryly true the
//              what is pointed to by CurrentOpIrp will also be NULL.
//              The reason for this is that if the queue is empty
//              when we hold the cancel spin lock, a new irp may come
//              in immediately after we release the lock.
//
//    CompleteCurrent - If TRUE then this routine will complete the
//                      irp pointed to by the pointer argument
//                      CurrentOpIrp.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanGetNextIrp(
                   IN PIRP *CurrentOpIrp,
                   IN PLIST_ENTRY QueueToProcess,
                   OUT PIRP *NextIrp,
                   IN BOOLEAN CompleteCurrent,
                   IN PCAN_DEVICE_EXTENSION extension
                   )
{
  KIRQL oldIrql;
  
  CAN_LOCKED_PAGED_CODE();

  IoAcquireCancelSpinLock(&oldIrql);
  CanGetNextIrpLocked(CurrentOpIrp, QueueToProcess, NextIrp, CompleteCurrent, extension, oldIrql);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This function is used to make the head of the particular
//    queue the current irp.  It also completes the what
//    was the old current irp if desired.  The difference between
//    this and CanGetNextIrp() is that for this we assume the caller
//    holds the cancel spinlock and we should release it when we're done.
//
//Arguments:
//
//    CurrentOpIrp - Pointer to a pointer to the currently active
//                   irp for the particular work list.  Note that
//                   this item is not actually part of the list.
//
//    QueueToProcess - The list to pull the new item off of.
//
//    NextIrp - The next Irp to process.  Note that CurrentOpIrp
//              will be set to this value under protection of the
//              cancel spin lock.  However, if *NextIrp is NULL when
//              this routine returns, it is not necessaryly true the
//              what is pointed to by CurrentOpIrp will also be NULL.
//              The reason for this is that if the queue is empty
//              when we hold the cancel spin lock, a new irp may come
//              in immediately after we release the lock.
//
//    CompleteCurrent - If TRUE then this routine will complete the
//                      irp pointed to by the pointer argument
//                      CurrentOpIrp.
//
//    OldIrql - IRQL which the cancel spinlock was acquired at and what we
//              should restore it to.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanGetNextIrpLocked(
                         IN PIRP *CurrentOpIrp,
                         IN PLIST_ENTRY QueueToProcess,
                         OUT PIRP *NextIrp,
                         IN BOOLEAN CompleteCurrent,
                         IN PCAN_DEVICE_EXTENSION extension,
                         IN KIRQL OldIrql
                         )
{
  PIRP oldIrp;

  CAN_LOCKED_PAGED_CODE();

  oldIrp = *CurrentOpIrp;

  // Check to see if there is a new irp to start up.
  if(!IsListEmpty(QueueToProcess))
  {
    PLIST_ENTRY headOfList = RemoveHeadList(QueueToProcess);

    *CurrentOpIrp = CONTAINING_RECORD(headOfList, IRP, Tail.Overlay.ListEntry);
    IoSetCancelRoutine(*CurrentOpIrp, NULL);
  }
  else
  {
    *CurrentOpIrp = NULL;
  }

  *NextIrp = *CurrentOpIrp;
  IoReleaseCancelSpinLock(OldIrql);

  if(CompleteCurrent)
  {
    if(oldIrp)
      CanCompleteRequest(extension, oldIrp, IO_NETWORK_INCREMENT);
  }
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine attempts to kill all of the reasons there are
//    references on the current read/write.  If everything can be killed
//    it will complete this read/write and try to start another.
//
//    NOTE: This routine assumes that it is called with the cancel
//          spinlock held.
//
//Arguments:
//
//    pDevExt - Simply a pointer to the device extension.
//
//    SynchRoutine - A routine that will synchronize with the isr
//                   and attempt to remove the knowledge of the
//                   current irp from the isr.  NOTE: This pointer
//                   can be null.
//
//    IrqlForRelease - This routine is called with the cancel spinlock held.
//                     This is the irql that was current when the cancel
//                     spinlock was acquired.
//
//    StatusToUse - The irp's status field will be set to this value, if
//                  this routine can complete the irp.
//
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanTryToCompleteCurrent(
  IN PCAN_DEVICE_EXTENSION pDevExt,
  IN PKSYNCHRONIZE_ROUTINE SynchRoutine OPTIONAL,
  IN KIRQL IrqlForRelease,
  IN NTSTATUS StatusToUse,
  IN PIRP *CurrentOpIrp,
  IN PLIST_ENTRY QueueToProcess OPTIONAL,
  IN PKTIMER TotalTimer OPTIONAL,
  IN PCAN_START_ROUTINE Starter OPTIONAL,
  IN PCAN_GET_NEXT_ROUTINE GetNextIrp OPTIONAL,
  IN LONG RefType,
  IN PCAN_FINISH_ROUTINE Finisher OPTIONAL)
{
  CAN_LOCKED_PAGED_CODE();

  // We can decrement the reference to "remove" the fact
  // that the caller no longer will be accessing this irp.
  CAN_CLEAR_REFERENCE(*CurrentOpIrp, RefType);

  if(SynchRoutine)
    KeSynchronizeExecution(pDevExt->Interrupt, SynchRoutine, pDevExt);

  if(*CurrentOpIrp)
  {
    // Try to run down all other references to this irp.
    CanRundownIrpRefs(CurrentOpIrp, TotalTimer, pDevExt);
  }
  // See if the ref count is zero after trying to kill everybody else.
  if(!(*CurrentOpIrp) || !CAN_REFERENCE_COUNT(*CurrentOpIrp))
  {
    PIRP newIrp;

    if(*CurrentOpIrp)
    {
      // The ref count was zero so we should complete this request.
      // The following call will also cause the current irp to be completed.
      (*CurrentOpIrp)->IoStatus.Status = StatusToUse;
      if(StatusToUse == STATUS_CANCELLED)
      {
        (*CurrentOpIrp)->IoStatus.Information = 0;
      }
    }

    if(GetNextIrp)
    {
      IoReleaseCancelSpinLock(IrqlForRelease);
      GetNextIrp(CurrentOpIrp, QueueToProcess, &newIrp, TRUE, pDevExt);
      if(newIrp)
      {
        if(Starter)
          Starter(pDevExt);
      }
      else
      {
        if(Finisher)
          Finisher(pDevExt);
      }
    }
    else
    {
      PIRP oldIrp = *CurrentOpIrp;

      // There was no get next routine.  We will simply complete
      // the irp.  We should make sure that we null out the
      // pointer to the pointer to this irp.
      *CurrentOpIrp = NULL;

      IoReleaseCancelSpinLock(IrqlForRelease);
      if(oldIrp)
      {
        CanCompleteRequest(pDevExt, oldIrp, IO_NETWORK_INCREMENT);
      }
    }
  }
  else
  {
    IoReleaseCancelSpinLock(IrqlForRelease);
  }
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to cancel Irps that currently reside on
//    a queue.
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
VOID CanCancelQueued(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
  PCAN_DEVICE_EXTENSION extension = DeviceObject->DeviceExtension;
  PIO_STACK_LOCATION irpSp = IoGetCurrentIrpStackLocation(Irp);

  CAN_LOCKED_PAGED_CODE();

  Irp->IoStatus.Status = STATUS_CANCELLED;
  Irp->IoStatus.Information = 0;

  RemoveEntryList(&Irp->Tail.Overlay.ListEntry);

  // If this is a write irp then take the amount of frames
  // to write and subtract it from the count of frames to write.
  if(irpSp->MajorFunction == IRP_MJ_WRITE)
  {
    extension->TotalFramesQueued -= irpSp->Parameters.Write.Length / sizeof(F_CAN_MSG);
  }

  IoReleaseCancelSpinLock(Irp->CancelIrql);
  CanCompleteRequest(extension, Irp, IO_NETWORK_INCREMENT);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    If the current irp is not an IOCTL_XXX request and
//    there is an error and the application requested abort on errors,
//    then cancel the irp.
//
//Arguments:
//
//    DeviceObject - Pointer to the device object for this device
//
//    Irp - Pointer to the IRP to test.
//
//Return Value:
//
//    STATUS_SUCCESS or STATUS_CANCELLED.
//-----------------------------------------------------------------------------
NTSTATUS CanCompleteIfError(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
  NTSTATUS status = STATUS_SUCCESS;
  F_CAN_STATE controller_state;
  
  CAN_LOCKED_PAGED_CODE();

  controller_state = CanGetControllerStatus(pDevExt);
  if(
    controller_state==CAN_STATE_INIT || 
    controller_state==CAN_STATE_BUS_OFF || 
    controller_state==CAN_STATE_STOPPED)
  {
    PIO_STACK_LOCATION irpSp = IoGetCurrentIrpStackLocation(Irp);

    // There is a non operation state in the driver. 
    // No read, write, wait requests should come through.
    if((irpSp->MajorFunction != IRP_MJ_DEVICE_CONTROL) || 
       (irpSp->Parameters.DeviceIoControl.IoControlCode == IOCTL_CAN_POST_MSG) ||
       (irpSp->Parameters.DeviceIoControl.IoControlCode == IOCTL_CAN_PEEK_MSG) ||
       (irpSp->Parameters.DeviceIoControl.IoControlCode == IOCTL_CAN_WAIT_ON_MASK)
      )
    {
      F_DBG1("Invalid state %d for request\n", controller_state);
      status = STATUS_CANCELLED;
      Irp->IoStatus.Status = STATUS_CANCELLED;
      Irp->IoStatus.Information = 0;
      CanCompleteRequest(pDevExt, Irp, 0);
    }
  }

  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is used to either start or queue any requst
//    that can be queued in the driver.
//
//Arguments:
//
//    pDevExt - Points to the can device extension.
//
//    Irp - The irp to either queue or start.  In either
//          case the irp will be marked pending.
//
//    QueueToExamine - The queue the irp will be place on if there
//                     is already an operation in progress.
//
//    CurrentOpIrp - Pointer to a pointer to the irp the is current
//                   for the queue.  The pointer pointed to will be
//                   set with to Irp if what CurrentOpIrp points to
//                   is NULL.
//
//    Starter - The routine to call if the queue is empty.
//
//Return Value:
//
//    This routine will return STATUS_PENDING if the queue is
//    not empty.  Otherwise, it will return the status returned
//    from the starter routine (or cancel, if the cancel bit is
//    on in the irp).
//-----------------------------------------------------------------------------
NTSTATUS CanStartOrQueue(
                         IN PCAN_DEVICE_EXTENSION pDevExt,
                         IN PIRP Irp,
                         IN PLIST_ENTRY QueueToExamine,
                         IN PIRP *CurrentOpIrp,
                         IN PCAN_START_ROUTINE Starter
                         )
{
  KIRQL oldIrql;
  BOOLEAN not_busy;

  CAN_LOCKED_PAGED_CODE();

  IoAcquireCancelSpinLock(&oldIrql);

  // We don't know how long the irp will be in the
  // queue.  So we need to handle cancel.
  if(Irp->Cancel)
  {
    IoReleaseCancelSpinLock(oldIrql);
    Irp->IoStatus.Status = STATUS_CANCELLED;
    CanCompleteRequest(pDevExt, Irp, 0);
    return STATUS_CANCELLED;
  }

  not_busy = ((IsListEmpty(QueueToExamine)) && !(*CurrentOpIrp));

  // If this is a write irp then take the amount of frames
  // to write and add it to the count of frames to write.
  if(IoGetCurrentIrpStackLocation(Irp)->MajorFunction == IRP_MJ_WRITE)
  {
    ULONG wr_frames = IoGetCurrentIrpStackLocation(Irp)->Parameters.Write.Length / sizeof(F_CAN_MSG);
    pDevExt->TotalFramesQueued += wr_frames;
    if(pDevExt->Controller.transmitterBusy)
    {
      not_busy = FALSE;
    }
  }

  if(not_busy /*(IsListEmpty(QueueToExamine)) && !(*CurrentOpIrp)*/)
  {
    // There were no current operation.  Mark this one as
    // current and start it up.
    *CurrentOpIrp = Irp;
    IoReleaseCancelSpinLock(oldIrql);
    return Starter(pDevExt);
  }

  Irp->IoStatus.Status = STATUS_PENDING;
  IoMarkIrpPending(Irp);
  InsertTailList(QueueToExamine, &Irp->Tail.Overlay.ListEntry);
  IoSetCancelRoutine(Irp, CanCancelQueued);
  IoReleaseCancelSpinLock(oldIrql);
  
  return STATUS_PENDING;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    In sync with the interrpt service routine (which sets the status)
//    return the controller status.
//
//
//Arguments:
//
//    Context - Pointer to a structure that contains a pointers to
//              the device extension and the current controller status.
//
//Return Value:
//
//    This routine always returns FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanGetState(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = ((PCAN_IOCTL_SYNC)Context)->pExtension;

  CAN_LOCKED_PAGED_CODE();
  *((F_CAN_STATE*)(((PCAN_IOCTL_SYNC)Context)->pData)) = pDevExt->Controller.state; 
    
  return FALSE;
}
//-----------------------------------------------------------------------------
F_CAN_STATE CanGetControllerStatus(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  CAN_IOCTL_SYNC sync;
  F_CAN_STATE controller_state;
  
  CAN_LOCKED_PAGED_CODE();

  sync.pExtension = pDevExt;
  sync.pData = &controller_state;
  KeSynchronizeExecution(pDevExt->Interrupt, CanGetState, &sync);
  
  return controller_state;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This function is used to signal all queued irps
//    for wait can events.
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
BOOLEAN CanSignalEvent(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)Context;

  CAN_LOCKED_PAGED_CODE();

  if(pDevExt->WaitStatus.status & pDevExt->Controller.status)
  {
    LARGE_INTEGER Wait;
    LARGE_INTEGER   Current;

    F_CAN_ClearStatus(pDevExt->WaitStatus.status, pDevExt->Controller.status);

    KeQueryTickCount(&Current);
    pDevExt->WaitStatus.taking.QuadPart = Current.QuadPart;

    Wait.QuadPart = (-1) * KeQueryTimeIncrement();
    CanSetTimer(&pDevExt->WaitOnMaskTimer, Wait, &pDevExt->CanWaitDpc, pDevExt);
  }

  return FALSE;
}
//-----------------------------------------------------------------------------
u32 CanGetTimeStamp(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  LARGE_INTEGER   Current;

  CAN_LOCKED_PAGED_CODE();

  KeQueryTickCount(&Current);
  Current.QuadPart = (Current.QuadPart - pDevExt->DeviceOpenTime.QuadPart) * KeQueryTimeIncrement();

  return Current.LowPart;
}
//-----------------------------------------------------------------------------
NTSTATUS CanPassIrpToLowerDriver(PCAN_DEVICE_EXTENSION pDevExt, IN PIRP Irp)
{
  Irp->IoStatus.Status = STATUS_SUCCESS;
  IoSkipCurrentIrpStackLocation(Irp);
  return IoCallDriver(pDevExt->LowerDeviceObject, Irp);
}
//-----------------------------------------------------------------------------

#endif