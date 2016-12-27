#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

#include <eal/el.h>
#include <can/xp/CanDrv.h>

#ifdef ALLOC_PRAGMA
#pragma alloc_text(PAGEFCAN, CanCreateOpen)
#pragma alloc_text(PAGEFCAN, CanClose)
#endif

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine merely sets a boolean to true to mark the fact that
//    somebody opened the device and its worthwhile to pay attention
//    to interrupts.
//    NOTE: This routine assumes that it is called at interrupt level.
//
//Arguments:
//
//    Context - Really a pointer to the device pDevExt.
//
//Return Value:
//
//    This routine always returns FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanMarkOpen(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = Context;
#if 0  

  sja1000_start(&pDevExt->Controller);

  HW_CAN_DISABLE_ALL_INTERRUPTS(&pDevExt->Controller);
  pDevExt->DeviceIsOpened = F_CAN_SUCCESS(res);

#else

  // Absolutely positively, prevent interrupts from occuring.
  sja1000_init_chip(&pDevExt->Controller);
  KeQueryTickCount(&pDevExt->DeviceOpenTime);

  pDevExt->DeviceIsOpened = TRUE;
#endif

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine merely sets a boolean to false to mark the fact that
//    somebody closed the device and it's no longer worthwhile to pay attention
//    to interrupts. It also disables the controller.
//
//Arguments:
//
//    Context - Really a pointer to the device extension.
//
//Return Value:
//
//    This routine always returns FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanMarkClose(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = Context;

  sja1000_stop(&pDevExt->Controller);

  pDevExt->DeviceIsOpened       = FALSE;
  pDevExt->DeviceState.Reopen   = FALSE;

  return FALSE;
}

//-----------------------------------------------------------------------------
//Routine Description:
//
//    We set up hardware to the operating mode for the create/open and initialize
//    the structures needed to maintain an open for a device.
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
NTSTATUS CanCreateOpen(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
  NTSTATUS localStatus;

  PAGED_CODE();

  F_DBG1("In CanCreateOpen() for %wZ\n", &pDevExt->SymbolicLinkName);

  do
  {
    if(pDevExt->PNPState != CAN_PNP_STARTED)
    {
      Irp->IoStatus.Status = STATUS_INSUFFICIENT_RESOURCES;
      IoCompleteRequest(Irp, IO_NO_INCREMENT);
      localStatus = STATUS_INSUFFICIENT_RESOURCES;
      break;
    }

    // Lock out changes to PnP state until we have our open state decided
    ExAcquireFastMutex(&pDevExt->OpenMutex);

    if((localStatus = CanIRPPrologue(Irp, pDevExt)) != STATUS_SUCCESS)
    {
      ExReleaseFastMutex(&pDevExt->OpenMutex);
      if(localStatus != STATUS_PENDING)
      {
        CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
      }
      break;
    }

    if(InterlockedIncrement(&pDevExt->OpenCount) != 1)
    {
      ExReleaseFastMutex(&pDevExt->OpenMutex);
      InterlockedDecrement(&pDevExt->OpenCount);
      Irp->IoStatus.Status = STATUS_ACCESS_DENIED;
      CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
      localStatus = STATUS_ACCESS_DENIED;
      F_DBG1("Device %wZ\n is busy", &pDevExt->SymbolicLinkName);
      break;
    }

    // Before we do anything, let's make sure they aren't trying
    // to create a directory. what's a driver to do!?
    if(IoGetCurrentIrpStackLocation(Irp)->Parameters.Create.Options & FILE_DIRECTORY_FILE)
    {
      ExReleaseFastMutex(&pDevExt->OpenMutex);
      Irp->IoStatus.Status = STATUS_NOT_A_DIRECTORY;
      Irp->IoStatus.Information = 0;
      InterlockedDecrement(&pDevExt->OpenCount);
      CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
      localStatus = STATUS_NOT_A_DIRECTORY;
      break;
    }
    
    // Create a buffer for the RX data when no reads are outstanding.
    pDevExt->InterruptReadBuffer = NULL;
    //pDevExt->szInterruptReadBuffer = driverDefaults.RxFIFODefault;

    switch(MmQuerySystemSize())
    {
    case MmLargeSystem:
      pDevExt->szInterruptReadBuffer = 128;
      pDevExt->InterruptReadBuffer = ExAllocatePool(NonPagedPool, pDevExt->szInterruptReadBuffer*sizeof(F_CAN_RX));
      if(pDevExt->InterruptReadBuffer)
        break;

    case MmMediumSystem:
      pDevExt->szInterruptReadBuffer = 64;
      pDevExt->InterruptReadBuffer = ExAllocatePool(NonPagedPool, pDevExt->szInterruptReadBuffer*sizeof(F_CAN_RX));
      if(pDevExt->InterruptReadBuffer)
        break;

    case MmSmallSystem:
      pDevExt->szInterruptReadBuffer = 32;
      pDevExt->InterruptReadBuffer = ExAllocatePool(NonPagedPool, pDevExt->szInterruptReadBuffer*sizeof(F_CAN_RX));
      if(pDevExt->InterruptReadBuffer)
        break;
    }

    if(!pDevExt->InterruptReadBuffer)
    {
      ExReleaseFastMutex(&pDevExt->OpenMutex);
      pDevExt->szInterruptReadBuffer = 0;
      Irp->IoStatus.Status = STATUS_INSUFFICIENT_RESOURCES;
      Irp->IoStatus.Information = 0;

      InterlockedDecrement(&pDevExt->OpenCount);
      CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
      localStatus = STATUS_INSUFFICIENT_RESOURCES;
      F_DBG1("Failed to allocate read buffer for %wZ\n", &pDevExt->SymbolicLinkName);
      break;
    }

    // Ok, it looks like we really are going to open.  Lock down the driver.
    CanLockPagableSectionByHandle(CanGlobals.PAGEFCAN_Handle);

    // Power up the stack
    // (void)CanGotoPowerState(DeviceObject, pDevExt, PowerDeviceD0);

    // On a new open we "flush" the read queue
    pDevExt->ReadBufferBase = pDevExt->InterruptReadBuffer;
    pDevExt->wrInterruptReadBuffer = 0;
    pDevExt->rdInterruptReadBuffer = 0;
    pDevExt->TotalFramesQueued = 0;
    pDevExt->WriteLength = 0;

    F_CAN_ClearStatus(pDevExt->Controller.status, CAN_STATUS_RXBUF);
    F_CAN_ClearStatus(pDevExt->Controller.status, CAN_STATUS_TXBUF);
    F_CAN_ClearStatus(pDevExt->Controller.status, CAN_STATUS_ERR);

    // On a new open we set defaults settings
    pDevExt->Timeouts.ReadTotalTimeout = 0;
    pDevExt->Timeouts.WriteTotalTimeoutConstant = 0;
    pDevExt->Timeouts.WriteTotalTimeoutMultiplier = 0;
    pDevExt->Timeouts.RestartBusoffTimeout = 0;
    pDevExt->Controller.settings = driverDefaults.ControllerSettingsDefault;

    // Mark the device as busy for WMI
    //pDevExt->WmiCommData.IsBusy = TRUE;
    
    // Synchronize with the ISR and let it know that the device has been successfully opened.
    KeSynchronizeExecution(pDevExt->Interrupt, CanMarkOpen, pDevExt);

    // Clear out the statistics.
    KeSynchronizeExecution(pDevExt->Interrupt, CanClearStats, pDevExt);

    // We have been marked open, so now the PnP state can change
    ExReleaseFastMutex(&pDevExt->OpenMutex);

    Irp->IoStatus.Status = STATUS_SUCCESS;
    Irp->IoStatus.Information=0L;

    CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
    localStatus = STATUS_SUCCESS;
  }
  while(0);

  return localStatus;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    We set up hardware to the reset mode for the close and deinit
//    the structures needed to maintain an open for a device.
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
NTSTATUS CanClose(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp)
{
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
  NTSTATUS status;
  LONG openCount; // Number of opens still active
  ULONG pendingDPCs; // Number of DPC's still pending

  PAGED_CODE();

  F_DBG1("In CanCreateClose() for %wZ\n", &pDevExt->SymbolicLinkName);

  ExAcquireFastMutex(&pDevExt->CloseMutex);

  // We succeed a close on a removing device
  if((status = CanIRPPrologue(Irp, pDevExt)) != STATUS_SUCCESS)
  {
    F_DBG1("Close prologue failed for: %x\n", Irp);
    if(status == STATUS_DELETE_PENDING)
    {
      if(pDevExt->InterruptReadBuffer)
      {
        ExFreePool(pDevExt->InterruptReadBuffer);
        pDevExt->InterruptReadBuffer = NULL;
      }
      pDevExt->szInterruptReadBuffer = 0;
      status = Irp->IoStatus.Status = STATUS_SUCCESS;
    }

    if(status != STATUS_PENDING)
    {
      CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
      openCount = InterlockedDecrement(&pDevExt->OpenCount);
      ASSERT(openCount == 0);
    }

    ExReleaseFastMutex(&pDevExt->CloseMutex);
    return status;
  }

  ASSERT(pDevExt->OpenCount >= 1);

  if(pDevExt->OpenCount < 1)
  {
    F_DBG2("Close open count(%d) bad for: 0x%x\n", pDevExt->OpenCount, Irp);
    ExReleaseFastMutex(&pDevExt->CloseMutex);
    Irp->IoStatus.Status = STATUS_INVALID_DEVICE_REQUEST;
    CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);
    return STATUS_INVALID_DEVICE_REQUEST;
  }

  // Synchronize with the ISR to stop controller.
  KeSynchronizeExecution( pDevExt->Interrupt, CanMarkClose, pDevExt);

  //// Mark device as not busy for WMI
  //pDevExt->WmiCommData.IsBusy = FALSE;


  // All is done. The port has been disabled from interrupting
  // so there is no point in keeping the memory around.
  if(pDevExt->InterruptReadBuffer != NULL)
  {
    ExFreePool(pDevExt->InterruptReadBuffer);
    pDevExt->InterruptReadBuffer = NULL;
  }
  pDevExt->szInterruptReadBuffer = 0;


  Irp->IoStatus.Status = STATUS_SUCCESS;
  Irp->IoStatus.Information=0L;

  CanCompleteRequest(pDevExt, Irp, IO_NO_INCREMENT);

  // Unlock the pages.  If this is the last reference to the section
  // then the driver code will be flushed out.
  //
  // First, we have to let the DPC's drain.  No more should be queued
  // since we aren't taking interrupts now....
  pendingDPCs = InterlockedDecrement(&pDevExt->DpcCount);
  if(pendingDPCs)
  {
    F_DBG1("Draining DPC's: %x\n", Irp);
    KeWaitForSingleObject(&pDevExt->PendingDpcEvent, Executive, KernelMode, FALSE, NULL);
  }
  F_DBG1("DPC's drained: %x\n", Irp);

  // Pages must be locked to release the mutex, so don't unlock
  // them until after we release the mutex
  ExReleaseFastMutex(&pDevExt->CloseMutex);

  // Reset for next open
  InterlockedIncrement(&pDevExt->DpcCount);
  InterlockedDecrement(&pDevExt->OpenCount);

  CanUnlockPagableImageSection(CanGlobals.PAGEFCAN_Handle);

  return STATUS_SUCCESS;
}
//-----------------------------------------------------------------------------
#endif