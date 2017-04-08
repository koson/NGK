#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

#include <eal/el.h>
#include <can/xp/CanDrv.h>

CAN_GLOBALS CanGlobals;
static const PHYSICAL_ADDRESS CanPhysicalZero = {0};
CAN_FIRMWARE_DATA    driverDefaults;

NTSTATUS DriverEntry(IN PDRIVER_OBJECT pDriverObject,	IN PUNICODE_STRING pRegistryPath);
NTSTATUS CanInitController(IN PDEVICE_OBJECT pDevObj, IN PCONFIG_DATA PConfigData);
NTSTATUS doCanInitController(IN PDEVICE_OBJECT pDevObj, IN PCONFIG_DATA PConfigData);

#ifdef ALLOC_PRAGMA
//#pragma alloc_text(INIT, DriverEntry)
#pragma alloc_text(PAGE, DriverEntry)
#pragma alloc_text(PAGE, CanAddDevice)
#pragma alloc_text(PAGE, CanDispatchPnp)
#pragma alloc_text(PAGE, CanStartDevice)
#pragma alloc_text(PAGE, CanFinishStartDevice)
#pragma alloc_text(PAGE, CanGetPortInfo)
#pragma alloc_text(PAGE, CanDoExternalNaming)
#pragma alloc_text(PAGE, CanUnload)
#pragma alloc_text(PAGE, CanInitController)
#pragma alloc_text(PAGE, doCanInitController)
#pragma alloc_text(PAGE, CanRemoveDevObj)
#pragma alloc_text(PAGE, CanPowerDispatch)
#pragma alloc_text(PAGE, CanFinishRestartDevice)
#endif // ALLOC_PRAGMA

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine is defunct since all device objects are removed before
//    the driver is unloaded.
//
//Arguments:
//
//    DriverObject - Pointer to the driver object controling all of the
//                   devices.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanUnload(IN PDRIVER_OBJECT DriverObject)
{
  PVOID lockPtr;

  PAGED_CODE();

  lockPtr = MmLockPagableCodeSection(CanUnload);

  // Unnecessary since our BSS is going away, but do it anyhow to be safe
  CanGlobals.PAGEFCAN_Handle = NULL;

  if(CanGlobals.RegistryPath.Buffer != NULL)
  {
    ExFreePool(CanGlobals.RegistryPath.Buffer);
    CanGlobals.RegistryPath.Buffer = NULL;
  }

  F_DBG("In CanUnload\n");

  MmUnlockPagableImageSection(lockPtr);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    The entry point that the system point calls to initialize
//    any driver.
//
//    This routine will gather the configuration information,
//    report resource usage, attempt to initialize all can
//    devices, connect to interrupts for ports.  If the above
//    goes reasonably well it will fill in the dispatch points,
//    reset the can devices and then return to the system.
//
//Arguments:
//
//    DriverObject - Just what it says,  really of little use
//    to the driver itself, it is something that the IO system
//    cares more about.
//
//    PathToRegistry - points to the entry for this driver
//    in the current control set of the registry.
//
//Return Value:
//
//    Always STATUS_SUCCESS
//-----------------------------------------------------------------------------
NTSTATUS DriverEntry(IN PDRIVER_OBJECT pDriverObject,	IN PUNICODE_STRING pRegistryPath)
{
  // Lock the paged code in their frames
  PVOID lockPtr = MmLockPagableCodeSection(CanSetResetMode);

  PAGED_CODE();

  // Initialize all our globals
  ASSERT(CanGlobals.PAGEFCAN_Handle == NULL);

  CanGlobals.PAGEFCAN_Handle = lockPtr;
  F_DBG1("Pagable code lock: %X\n", CanGlobals.PAGEFCAN_Handle);

  CanGlobals.RegistryPath.MaximumLength = pRegistryPath->MaximumLength;
  CanGlobals.RegistryPath.Length = pRegistryPath->Length;
  CanGlobals.RegistryPath.Buffer = ExAllocatePool(PagedPool, CanGlobals.RegistryPath.MaximumLength);

  if(CanGlobals.RegistryPath.Buffer == NULL)
  {
    MmUnlockPagableImageSection(lockPtr);
    return STATUS_INSUFFICIENT_RESOURCES;
  }
  RtlZeroMemory(CanGlobals.RegistryPath.Buffer, CanGlobals.RegistryPath.MaximumLength);
  RtlMoveMemory(CanGlobals.RegistryPath.Buffer, pRegistryPath->Buffer, pRegistryPath->Length);

  KeInitializeSpinLock(&CanGlobals.GlobalsSpinLock);
  InitializeListHead(&CanGlobals.AllDevObjs);

  // Call to find out default values to use for all the devices that the
  // driver controls, including whether or not to break on entry.
  CanGetConfigDefaults(&driverDefaults, pRegistryPath);

  // Just dump out how big the extension is.
  F_DBG1("The number of bytes in the extension is: %d\n", sizeof(CAN_DEVICE_EXTENSION));

  // Initialize the Driver Object with driver's entry points
  pDriverObject->DriverUnload                          = CanUnload;
  pDriverObject->DriverExtension->AddDevice            = CanAddDevice;

  //pDriverObject->MajorFunction[IRP_MJ_FLUSH_BUFFERS]   = CanFlush;
  pDriverObject->MajorFunction[IRP_MJ_WRITE]           = CanWrite;
  pDriverObject->MajorFunction[IRP_MJ_READ]            = CanRead;
  pDriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL]  = CanIoControl;
  //pDriverObject->MajorFunction[IRP_MJ_INTERNAL_DEVICE_CONTROL] = CanInternalIoControl;
  pDriverObject->MajorFunction[IRP_MJ_CREATE]          = CanCreateOpen;
  pDriverObject->MajorFunction[IRP_MJ_CLOSE]           = CanClose;
  pDriverObject->MajorFunction[IRP_MJ_PNP]             = CanDispatchPnp;
  pDriverObject->MajorFunction[IRP_MJ_POWER]           = CanPowerDispatch;
  //pDriverObject->MajorFunction[IRP_MJ_SYSTEM_CONTROL]  = CanSystemControlDispatch;

  // Unlock pageable text
  MmUnlockPagableImageSection(lockPtr);

  return STATUS_SUCCESS;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Logs the power state.
//
//Arguments:
//
//    stack - pointer to an I/O stack location.
//
//Return Value:
//
//    NT status code.
//-----------------------------------------------------------------------------
VOID LogPowerState( PIO_STACK_LOCATION stack )
{
  static char* sysState[] = { 
                              "PowerSystemUnspecified", 
                              "PowerSystemWorking", 
                              "PowerSystemSleeping1", 
                              "PowerSystemSleeping2", 
                              "PowerSystemSleeping3", 
                              "PowerSystemHibernate", 
                              "PowerSystemShutdown", 
                              "PowerSystemMaximum" 
                            };
  static char* devState[] = { 
                              "PowerDeviceUnspecified", 
                              "PowerDeviceD0", 
	                            "PowerDeviceD1", 
                              "PowerDeviceD2", 
                              "PowerDeviceD3", 
                              "PowerDeviceMaximum"
                            };

  if ( stack->Parameters.Power.Type == SystemPowerState )
    F_DBG1("SystemPowerState = %s\n", sysState[stack->Parameters.Power.State.SystemState]);
  else
    F_DBG1("DevicePowerState = %s\n", devState[stack->Parameters.Power.State.DeviceState]);
}

//-----------------------------------------------------------------------------
//Routine Description:
//
//    The power dispatch routine.
//
//Arguments:
//
//    DeviceObject - pointer to a device object..
//
//    Irp - pointer to an I/O Request Packet.
//
//
//Return Value:
//
//    NT status code.
//-----------------------------------------------------------------------------
NTSTATUS CanPowerDispatch( IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp )
{
  PCAN_DEVICE_EXTENSION pDevExt = DeviceObject->DeviceExtension;
  PIO_STACK_LOCATION stack;

  PAGED_CODE();

  stack = IoGetCurrentIrpStackLocation(Irp);

  switch ( stack->MinorFunction ) 
  {
	  case IRP_MN_SET_POWER: 
	  {
		  F_DBG("Got IRP_MN_SET_POWER Irp\n");
                  LogPowerState( stack );

		  if( stack->Parameters.Power.Type == SystemPowerState )
		  {
			  if( stack->Parameters.Power.State.SystemState == PowerSystemWorking )
			  {
				  F_DBG("Wake up device\n");
				  CanFinishRestartDevice( DeviceObject );
				  if( pDevExt->DeviceState.Reopen == TRUE ) 
                                  {
                                    CAN_IOCTL_SYNC ioctl_sync;
                                    F_DBG( "Device was opened before, reinitialize device\n" ); 
                                    pDevExt->DeviceState.Reopen = FALSE;  
                                    KeSynchronizeExecution(pDevExt->Interrupt, CanMarkOpen, pDevExt);
                                    ioctl_sync.pExtension = pDevExt;
                                    KeSynchronizeExecution(pDevExt->Interrupt, CanStartController, &ioctl_sync);
                                  }
			  }
			  else if( !(pDevExt->Flags & CAN_FLAGS_STOPPED) && stack->Parameters.Power.State.SystemState!=PowerSystemShutdown )
			  {
				  ULONG pendingIRPs;
				  F_DBG1( "Turn device into the sleep mode, pDevExt->PendingIRPCnt = %d\n", pDevExt->PendingIRPCnt );

				  CanSetFlags(pDevExt, CAN_FLAGS_STOPPED);
				  CanSetAccept(pDevExt, CAN_PNPACCEPT_STOPPED);
				  CanClearAccept(pDevExt, CAN_PNPACCEPT_STOPPING);

				  pDevExt->PNPState = CAN_PNP_STOPPING;
				  CanKillPendingIrps( DeviceObject );
				  
				  // Decrement for stopping
				  pendingIRPs = InterlockedDecrement(&pDevExt->PendingIRPCnt);

				  if(pendingIRPs)
					  KeWaitForSingleObject(&pDevExt->PendingIRPEvent, Executive, KernelMode, FALSE, NULL);

				  // Re-increment the count for later
				  InterlockedIncrement(&pDevExt->PendingIRPCnt);

				  // We need to free resources...basically this is a remove
				  // without the detach from the stack.
				  if(pDevExt->Flags & CAN_FLAGS_STARTED)
				  {
					  CanReleaseResources(pDevExt);
				  }
			  }
		  }			
		  break;
	  }
	  case IRP_MN_QUERY_POWER: 
	  {
		  F_DBG("Got IRP_MN_QUERY_POWER Irp\n");
      LogPowerState( stack );
		  break;
	  }
	  case IRP_MN_WAIT_WAKE: 
	  {
		  F_DBG("Got IRP_MN_WAIT_WAKE Irp\n");
		  break;
	  }
	  case IRP_MN_POWER_SEQUENCE: 
	  {
		  F_DBG("Got IRP_MN_POWER_SEQUENCE Irp\n");
		  break;
	  }
	  default:
	  {
		  F_DBG("Got Unknown Irp in CanPowerDispatch\n");
		  break;
	  }
  }

  //
  // Inform the power manager that we are able to accept another power IRP.
  //
  PoStartNextPowerIrp(Irp);

  //
  // Call the next driver.
  //
  return CanPassIrpToLowerDriver( pDevExt, Irp );
}

//-----------------------------------------------------------------------------
//Routine Description:
//
//    Compare two phsical address.
//
//Arguments:
//
//    A - One half of the comparison.
//
//    SpanOfA - In units of bytes, the span of A.
//
//    B - One half of the comparison.
//
//    SpanOfB - In units of bytes, the span of B.
//
//
//Return Value:
//
//    The result of the comparison.
//-----------------------------------------------------------------------------
MEM_COMPARES MemCompare(IN PHYSICAL_ADDRESS A, IN ULONG SpanOfA, IN PHYSICAL_ADDRESS B, IN ULONG SpanOfB)
{
  LARGE_INTEGER a;
  LARGE_INTEGER b;

  LARGE_INTEGER lower;
  ULONG lowerSpan;
  LARGE_INTEGER higher;

  PAGED_CODE();

  a = A;
  b = B;
  if(a.QuadPart == b.QuadPart)
    return AddressesAreEqual;

  if(a.QuadPart > b.QuadPart)
  {
    higher = a;
    lower = b;
    lowerSpan = SpanOfB;
  } 
  else
  {
    higher = b;
    lower = a;
    lowerSpan = SpanOfA;
  }

  if((higher.QuadPart - lower.QuadPart) >= lowerSpan)
    return AddressesAreDisjoint;

  return AddressesOverlap;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine maps an IO address to system address space.
//
//Arguments:
//
//    IoAddress - base device address to be mapped.
//    NumberOfBytes - number of bytes for which address is valid.
//    AddressSpace - Denotes whether the address is in io space or memory.
//    MappedAddress - indicates whether the address was mapped.
//                    This only has meaning if the address returned
//                    is non-null.
//
//Return Value:
//
//    Mapped address
//-----------------------------------------------------------------------------
PVOID GetMappedAddress(PHYSICAL_ADDRESS IoAddress, ULONG NumberOfBytes, ULONG AddressSpace, PBOOLEAN MappedAddress)
{
  PVOID address;

  PAGED_CODE();

  // Map the device base address into the virtual address space
  // if the address is in memory space.
  if(!AddressSpace)
  {
    address = MmMapIoSpace(IoAddress, NumberOfBytes, FALSE);
    *MappedAddress = (BOOLEAN)((address)?(TRUE):(FALSE));
  } 
  else
  {
    address = ULongToPtr(IoAddress.LowPart);
    *MappedAddress = FALSE;
  }

  return address;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine examines several of what might be the can device
//    registers.  It ensures that the bits that should be zero are zero.
//
//    NOTE: If there is indeed a can port at the address specified
//          it will absolutely have interrupts inhibited upon return
//          from this routine.
//
//    NOTE: Since this routine should be called fairly early in
//          the device driver initialization, the only element
//          that needs to be filled in is the base register address.
//
//    NOTE: These tests all assume that this code is the only
//          code that is looking at these ports or this memory.
//
//          This is a not to unreasonable assumption even on
//          multiprocessor systems.
//
//Arguments:
//
//    Extension - A pointer to a can device extension.
//    InsertString - String to place in an error log entry.
//
//Return Value:
//
//    Will return true if the port really exists, otherwise it
//    will return false.
//-----------------------------------------------------------------------------
BOOLEAN CanDoInitPort(IN PCAN_DEVICE_EXTENSION Extension)
{
  BOOLEAN returnValue = FALSE;

  do
  {
    F_DBG("Probe chip\n");
    if(!sja1000_probe_chip(&Extension->Controller))
    {
      F_DBG("Probe chip failed\n");
      break;
    }

    // Well, we think it's a can device. 
    F_DBG("Init chip\n");
    if(!sja1000_init_chip(&Extension->Controller))
    {
      F_DBG("Failed to init chip\n");
      break;
    }

    // Absolutely positively, prevent interrupts from occuring.
    HW_CAN_DISABLE_ALL_INTERRUPTS(&Extension->Controller);

    returnValue = TRUE;  
  }
  while(0);

  return returnValue;
}
//-----------------------------------------------------------------------------
VOID SetDeviceIsOpened(IN PCAN_DEVICE_EXTENSION pDevExt, IN BOOLEAN DeviceIsOpened, IN BOOLEAN Reopen)
{
  KIRQL oldIrql;
    
  KeAcquireSpinLock(&pDevExt->ControlLock, &oldIrql);

  pDevExt->DeviceIsOpened     = DeviceIsOpened;
  pDevExt->DeviceState.Reopen = Reopen;

  KeReleaseSpinLock(&pDevExt->ControlLock, oldIrql);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Really too many things to mention here.  In general initializes
//    kernel synchronization structures, allocates the typeahead buffer,
//    sets up defaults, etc.
//
//Arguments:
//
//    pDevObj       - Device object for the device to be started
//
//    PConfigData   - Pointer to a record for a single port.
//
//Return Value:
//
//    STATUS_SUCCCESS if everything went ok.  A !NT_SUCCESS status
//    otherwise.
//-----------------------------------------------------------------------------
NTSTATUS doCanInitController(IN PDEVICE_OBJECT pDevObj, IN PCONFIG_DATA PConfigData)
{
  PCAN_DEVICE_EXTENSION pDevExt = pDevObj->DeviceExtension;
  // Holds the NT Status that is returned from each call to the
  // kernel and executive.
  NTSTATUS status = STATUS_SUCCESS;
  // Indicates that a conflict was detected for resources
  // used by this device.
  BOOLEAN conflictDetected = FALSE;
  // Indicates if we allocated an ISR switch
  BOOLEAN allocedISRSw = FALSE;

  PAGED_CODE();

  if(pDevExt->CIsrSw == NULL)
  {
    if((pDevExt->CIsrSw = ExAllocatePool(NonPagedPool, sizeof(CAN_CISR_SW))) == NULL)
    {
      return STATUS_INSUFFICIENT_RESOURCES;
    }
    InitializeListHead(&pDevExt->CIsrSw->SharerList);
    allocedISRSw = TRUE;
  }

  // Initialize the timers used to timeout operations.
  KeInitializeTimer(&pDevExt->RestartControllerTimer);
  KeInitializeTimer(&pDevExt->ReadRequestTotalTimer);
  KeInitializeTimer(&pDevExt->WriteRequestTotalTimer);
  KeInitializeTimer(&pDevExt->WaitOnMaskTimer);

  // Intialialize the dpcs that will be used to complete various IO operations.
  KeInitializeDpc(&pDevExt->CompleteWriteDpc, CanCompleteWrite, pDevExt);
  KeInitializeDpc(&pDevExt->CompleteReadDpc, CanCompleteRead, pDevExt);
  KeInitializeDpc(&pDevExt->TotalReadTimeoutDpc, CanReadTimeout, pDevExt);
  KeInitializeDpc(&pDevExt->TotalWriteTimeoutDpc, CanWriteTimeout, pDevExt);
  KeInitializeDpc(&pDevExt->CanRecoverDpc, CanRecover, pDevExt);
  KeInitializeDpc(&pDevExt->RestartControllerDpc, CanInvokeRestartController, pDevExt);
  KeInitializeDpc(&pDevExt->CanWaitDpc, CanCompleteWait, pDevExt);
  KeInitializeDpc(&pDevExt->IsrUnlockPagesDpc, CanUnlockPages, pDevExt);

  do
  {
    // Map the memory for the control registers for the can device into virtual memory.
    pDevExt->Controller.reg_base = GetMappedAddress(
      PConfigData->TrController,
      PConfigData->SpanOfController, 
      (BOOLEAN)PConfigData->AddressSpace, 
      &pDevExt->UnMapRegisters);

    if(!pDevExt->Controller.reg_base)
    {
      F_DBG1("Could not map memory for device registers for %wZ\n", &pDevExt->DeviceName);
      pDevExt->UnMapRegisters = FALSE;
      status = STATUS_NONE_MAPPED;
      break;
    }
    pDevExt->AddressSpace          = PConfigData->AddressSpace;
    pDevExt->OriginalController    = PConfigData->Controller;
    pDevExt->SpanOfController      = PConfigData->SpanOfController;

#ifdef FWCAN_USE_SWITCH_DEVICE
    // Map the memory for the switch device into virtual memory.
    pDevExt->switchPortBase = GetMappedAddress(
      PConfigData->switchPort.start, 
      PConfigData->switchPort.length, 
      (BOOLEAN)PConfigData->switchPort.addressSpace, 
      &pDevExt->UnMapSwitchRegisters);

    if(!pDevExt->switchPortBase)
    {
      F_DBG1("Could not map memory for switch device registers for %wZ\n", &pDevExt->DeviceName);
      pDevExt->UnMapSwitchRegisters = FALSE;
      status = STATUS_NONE_MAPPED;
      break;
    }
    pDevExt->switchPortLength = PConfigData->switchPort.length;
#endif

    // Shareable interrupt?
    pDevExt->InterruptShareable = TRUE;
    // Save off the interface type and the bus number.
    pDevExt->InterfaceType = PConfigData->InterfaceType;
    pDevExt->BusNumber     = PConfigData->BusNumber;

    // Get the interrupt vector, level, and affinity
    pDevExt->OriginalIrql      = PConfigData->OriginalIrql;
    pDevExt->OriginalVector    = PConfigData->OriginalVector;
    // PnP uses the passed translated values rather than calling
    // HalGetInterruptVector()
    pDevExt->Vector = PConfigData->TrVector;
    pDevExt->Irql = (UCHAR)PConfigData->TrIrql;

    // Set up the Isr.
    pDevExt->OurIsr        = CanISR;
    pDevExt->OurIsrContext = pDevExt;

    pDevExt->TxFifoAmount = (PConfigData->TxFIFO<1)? 1: PConfigData->TxFIFO;

    // Set up the default device control fields. Note that if the values are changed after
    // the file is open, they do NOT revert back to the old value at file close.
    pDevExt->Controller.settings   = PConfigData->ControllerSettings;

    if(!CanDoInitPort(pDevExt))
    {
      // We couldn't verify that there was actually a port.
      F_DBG1("DoesPortExist test failed for %wZ\n", &pDevExt->DeviceName);
      status = STATUS_NO_SUCH_DEVICE;
      break;
    }

    // If the user requested that we disable the port, then
    // do it now.  Log the fact that the port has been disabled.
    if(PConfigData->DisablePort)
    {
      F_DBG1("disabled port %wZ as requested in configuration\n", &pDevExt->DeviceName);
      status = STATUS_NO_SUCH_DEVICE;
#if 0
      CanLogError(pDevExt->DriverObject, pDevObj, 0, 0, 0, 22, status, FWCAN_DISABLED_PORT,
        pDevExt->DeviceName.Length + sizeof(WCHAR),
        pDevExt->DeviceName.Buffer, 
        0, NULL);
#endif
      break;
    }

    // Mark this device as not being opened by anyone.  We keep a
    // variable around so that spurious interrupts are easily
    // dismissed by the ISR.
    if(pDevExt->DeviceIsOpened) 
      SetDeviceIsOpened(pDevExt, FALSE, TRUE);
    else     
      SetDeviceIsOpened(pDevExt, FALSE, FALSE);
  }
  while(0);

  if(!NT_SUCCESS(status))
  {
    if(allocedISRSw)
    {
      ExFreePool(pDevExt->CIsrSw);
      pDevExt->CIsrSw = NULL;
    }
    if(pDevExt->UnMapRegisters)
    {
      MmUnmapIoSpace(pDevExt->Controller.reg_base, pDevExt->SpanOfController);
      pDevExt->UnMapRegisters = false;
    }
#ifdef FWCAN_USE_SWITCH_DEVICE
    if(pDevExt->UnMapSwitchRegisters)
    {
      MmUnmapIoSpace(pDevExt->switchPortBase, pDevExt->switchPortLength);
      pDevExt->UnMapSwitchRegisters = false;
    }
#endif
  }

  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine will call the real port initialization code.  It sets
//    up the ISR and Context correctly for a one port device.
//
//Arguments:
//
//    All args are simply passed along.
//
//Return Value:
//
//    Status returned from the controller initialization routine.
//-----------------------------------------------------------------------------
NTSTATUS CanInitController(IN PDEVICE_OBJECT pDevObj, IN PCONFIG_DATA PConfigData)
{
  NTSTATUS status;
  PCAN_DEVICE_EXTENSION pDevExt;

  PAGED_CODE();

  status = doCanInitController(pDevObj, PConfigData);

  if(NT_SUCCESS(status))
  {
    pDevExt = pDevObj->DeviceExtension;

    // We successfully initialized the single controller.
    // Stick the isr routine and the parameter for it
    // back into the extension.
    pDevExt->OurIsr = CanISR;
    pDevExt->OurIsrContext = pDevExt;
    pDevExt->TopLevelOurIsr = CanISR;
    pDevExt->TopLevelOurIsrContext = pDevExt;
  }

  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine will take a device extension for a can port and
//    allow it to share interrupts with other can ports.
//
//Arguments:
//
//    Context - The device extension of the port who is to start sharing
//    interrupts.
//
//Return Value:
//
//    Always TRUE.
//-----------------------------------------------------------------------------
BOOLEAN CanBecomeSharer(PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)Context;
  PCAN_DEVICE_EXTENSION pNewExt = (PCAN_DEVICE_EXTENSION)pDevExt->NewExtension;
  PCAN_CISR_SW pCIsrSw = pDevExt->CIsrSw;

  // See if we need to configure the pre-existing node to become a sharer.
  if(IsListEmpty(&pCIsrSw->SharerList))
  {
    pCIsrSw->IsrFunc = CanSharerIsr;
    pCIsrSw->Context = &pCIsrSw->SharerList;
    InsertTailList(&pCIsrSw->SharerList, &pDevExt->TopLevelSharers);
  }

  // They share an interrupt object and a context
  pNewExt->Interrupt = pDevExt->Interrupt;
  pNewExt->CIsrSw = pDevExt->CIsrSw;
  // Add to list of sharers
  InsertTailList(&pCIsrSw->SharerList, &pNewExt->TopLevelSharers);
  // Add to list of those who share this interrupt object --
  // we may already be on if this port is part of a multiport board
  if(IsListEmpty(&pNewExt->CommonInterruptObject))
  {
    InsertTailList(&pDevExt->CommonInterruptObject, &pNewExt->CommonInterruptObject);
  }

  return TRUE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This function discovers what type of controller is responsible for
//    the given port and initializes the controller and port.
//
//Arguments:
//
//    pDevObj - Pointer to the devobj for the port we are about to init.
//
//    PConfig - Pointer to configuration data for the port we are about to init.
//
//Return Value:
//
//    STATUS_SUCCESS on success, appropriate error value on failure.
//-----------------------------------------------------------------------------
NTSTATUS CanFindInitController(IN PDEVICE_OBJECT pDevObj, IN PCONFIG_DATA PConfig)
{
  PCAN_DEVICE_EXTENSION pDevExt = pDevObj->DeviceExtension;
  PCAN_DEVICE_EXTENSION pExtension;
  PHYSICAL_ADDRESS canPhysicalMax;
  //CAN_LIST_DATA listAddition;
  BOOLEAN didInit = FALSE;
  PLIST_ENTRY pCurDevObj;
  NTSTATUS status;
  KIRQL oldIrql;

  canPhysicalMax.LowPart = (ULONG)~0;
  canPhysicalMax.HighPart = ~0;

  F_DBG6("Attempting to init %wZ\n"
    "------- PortAddress is %x\n"
    "------- BusNumber is %d\n"
    "------- BusType is %d\n"
    "------- AddressSpace is %d\n"
    "------- Interrupt Mode is %d\n",
    &pDevExt->DeviceName, PConfig->Controller.LowPart, PConfig->BusNumber,
    PConfig->InterfaceType, PConfig->AddressSpace, PConfig->InterruptMode);

  // We don't support any boards whose memory wraps around
  // the physical address space.
  if(MemCompare(PConfig->Controller, PConfig->SpanOfController, canPhysicalMax, (ULONG)0) != AddressesAreDisjoint)
  {
    F_DBG1("Error in config record for %wZ: registers wrap around physical memory\n", &pDevExt->DeviceName);
    return STATUS_NO_SUCH_DEVICE;
  }

  // Loop through all of the driver's device objects making
  // sure that this new record doesn't overlap with any of them.
  KeAcquireSpinLock(&CanGlobals.GlobalsSpinLock, &oldIrql);
  if(!IsListEmpty(&CanGlobals.AllDevObjs))
  {
    pCurDevObj = CanGlobals.AllDevObjs.Flink;
    pExtension = CONTAINING_RECORD(pCurDevObj, CAN_DEVICE_EXTENSION, AllDevObjs);
  } 
  else
  {
    pCurDevObj = NULL;
    pExtension = NULL;
  }
  KeReleaseSpinLock(&CanGlobals.GlobalsSpinLock, oldIrql);

  while(pCurDevObj!=NULL && pCurDevObj!=&CanGlobals.AllDevObjs)
  {
    // We only care about this list if the elements are on the
    // same bus as this new entry.
    if((pExtension->InterfaceType==PConfig->InterfaceType)
      &&(pExtension->AddressSpace==PConfig->AddressSpace)
      &&(pExtension->BusNumber==PConfig->BusNumber))
    {
      // Check to see if the controller addresses are not equal.
      if(MemCompare(PConfig->Controller, PConfig->SpanOfController, pExtension->OriginalController, pExtension->SpanOfController) != AddressesAreDisjoint)
      {
        F_DBG1("Error in config record for %wZ\n"
          "------- Register address overlaps with\n"
          "------- previous device\n",
          &pDevExt->DeviceName);
        return STATUS_NO_SUCH_DEVICE;
      }
    }

    KeAcquireSpinLock(&CanGlobals.GlobalsSpinLock, &oldIrql);
    pCurDevObj = pCurDevObj->Flink;
    KeReleaseSpinLock(&CanGlobals.GlobalsSpinLock, oldIrql);

    if(pCurDevObj != NULL)
    {
      pExtension = CONTAINING_RECORD(pCurDevObj, CAN_DEVICE_EXTENSION, AllDevObjs);
    }
  }

  status = CanInitController(pDevObj, PConfig);
  if(!NT_SUCCESS(status))
  {
    return status;
  }

  // The device is initialized.  Now we need to check if
  // this device shares an interrupt with anyone.
  // Loop through all previously attached devices
  KeAcquireSpinLock(&CanGlobals.GlobalsSpinLock, &oldIrql);
  if(!IsListEmpty(&CanGlobals.AllDevObjs))
  {
    pCurDevObj = CanGlobals.AllDevObjs.Flink;
    pExtension = CONTAINING_RECORD(pCurDevObj, CAN_DEVICE_EXTENSION, AllDevObjs);
   }
  else
  {
    pCurDevObj = NULL;
    pExtension = NULL;
  }
  KeReleaseSpinLock(&CanGlobals.GlobalsSpinLock, oldIrql);

  // Go through the list again looking for previous devices
  // with the same interrupt.  The first one found will either be a root
  // or standalone.  Order of insertion is important here!
  if(pCurDevObj != NULL)
  {
    do
    {
      // We only care about interrupts that are on the same bus.
      if((pExtension->Irql == PConfig->TrIrql) && (pExtension->Vector == PConfig->TrVector))
      {
        pExtension->NewExtension = pDevExt;
        // We will share another's CIsrSw so we can free the one
        // allocated for us during init
        ExFreePool(pDevExt->CIsrSw);
        F_DBG3("FwCAN: Becoming sharer: %08X %08X %08X\n", pExtension, pExtension->OriginalIrql, &pExtension->CIsrSw->SharerList);
        KeSynchronizeExecution(pExtension->Interrupt, CanBecomeSharer, pExtension);
        return STATUS_SUCCESS;
      }

      // No match, check some more
      KeAcquireSpinLock(&CanGlobals.GlobalsSpinLock, &oldIrql);
      pCurDevObj = pCurDevObj->Flink;
      if(pCurDevObj != NULL)
      {
        pExtension = CONTAINING_RECORD(pCurDevObj, CAN_DEVICE_EXTENSION, AllDevObjs);
      }
      KeReleaseSpinLock(&CanGlobals.GlobalsSpinLock, oldIrql);
    }
    while(pCurDevObj!=NULL && pCurDevObj!=&CanGlobals.AllDevObjs);
  }

  return STATUS_SUCCESS;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Removes a device object from any of the lists it may appear on.
//
//Arguments:
//
//    Context - Actually a PCAN_DEVICE_EXTENSION (for the devobj being
//              removed).
//
//Return Value:
//
//    Always TRUE
//-----------------------------------------------------------------------------
BOOLEAN CanCleanLists(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)Context;

  if(!IsListEmpty(&pDevExt->TopLevelSharers))
  {
    // Remove ourselves
    RemoveEntryList(&pDevExt->TopLevelSharers);
    InitializeListHead(&pDevExt->TopLevelSharers);

    // Now check the master list to see if anyone is left...
    if(!IsListEmpty(&pDevExt->CIsrSw->SharerList))
    {
      // Others are chained on this interrupt, so we don't want to
      // disconnect it.
      pDevExt->Interrupt = NULL;
    }
  }

  if(!IsListEmpty(&pDevExt->CommonInterruptObject))
  {
    RemoveEntryList(&pDevExt->CommonInterruptObject);
    InitializeListHead(&pDevExt->CommonInterruptObject);
    // Others are sharing this interrupt object so we detach ourselves
    // from it this way instead of disconnecting.
    pDevExt->Interrupt = NULL;
  }

  return TRUE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Releases resources (not pool) stored in the device extension.
//
//Arguments:
//
//    pDevExt - Pointer to the device extension to release resources from.
//
//Return Value:
//
//    VOID
//-----------------------------------------------------------------------------
VOID CanReleaseResources(IN PCAN_DEVICE_EXTENSION pDevExt)
{
  KIRQL oldIrql;

  F_DBG("Release Resources\n");

  // Remove us from any lists we may be on
  if(pDevExt->Interrupt != NULL)
  {
    F_DBG("Remove us from any lists\n");
    KeSynchronizeExecution(pDevExt->Interrupt, CanCleanLists, pDevExt);

    // AllDevObjs should never be empty since we have a sentinal
    KeAcquireSpinLock(&CanGlobals.GlobalsSpinLock, &oldIrql);

    ASSERT(!IsListEmpty(&pDevExt->AllDevObjs));
    RemoveEntryList(&pDevExt->AllDevObjs);

    KeReleaseSpinLock(&CanGlobals.GlobalsSpinLock, oldIrql);

    InitializeListHead(&pDevExt->AllDevObjs);
  }

  // CanCleanLists can remove our interrupt from us...
  if(pDevExt->Interrupt != NULL)
  {
    // Stop servicing interrupts if we are the owner
    F_DBG("Stop servicing interrupts\n");

    IoDisconnectInterrupt(pDevExt->Interrupt);
    pDevExt->Interrupt = NULL;

    if(pDevExt->CIsrSw != NULL)
    {
      ExFreePool(pDevExt->CIsrSw);
      pDevExt->CIsrSw = NULL;
    }
  }

  // Stop handling timers
  F_DBG("Stop handling timers\n");

  CanCancelTimer(&pDevExt->ReadRequestTotalTimer, pDevExt);
  CanCancelTimer(&pDevExt->WriteRequestTotalTimer, pDevExt);
  CanCancelTimer(&pDevExt->RestartControllerTimer, pDevExt);
  CanCancelTimer(&pDevExt->WaitOnMaskTimer, pDevExt);

  // Stop servicing DPC's
  F_DBG("Stop servicing DPC's\n");

  CanRemoveQueueDpc(&pDevExt->CompleteWriteDpc, pDevExt);
  CanRemoveQueueDpc(&pDevExt->CompleteReadDpc, pDevExt);
  CanRemoveQueueDpc(&pDevExt->TotalReadTimeoutDpc, pDevExt);
  CanRemoveQueueDpc(&pDevExt->TotalWriteTimeoutDpc, pDevExt);
  CanRemoveQueueDpc(&pDevExt->CanRecoverDpc, pDevExt);
  CanRemoveQueueDpc(&pDevExt->CanWaitDpc, pDevExt);
  CanRemoveQueueDpc(&pDevExt->RestartControllerDpc, pDevExt);

  // If necessary, unmap the device registers.
  F_DBG("Unmap the device registers\n");
  if(pDevExt->UnMapRegisters)
  {
    MmUnmapIoSpace(pDevExt->Controller.reg_base, pDevExt->SpanOfController);
    pDevExt->UnMapRegisters = false;
  }
#ifdef FWCAN_USE_SWITCH_DEVICE
  if(pDevExt->UnMapSwitchRegisters)
  {
    MmUnmapIoSpace(pDevExt->switchPortBase, pDevExt->switchPortLength);
    pDevExt->UnMapSwitchRegisters = false;
  }
#endif
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//   This routine kills any irps pending for the passed device object.
//
//Arguments:
//
//    pDevObj - Pointer to the device object whose irps must die.
//
//Return Value:
//
//    VOID
//-----------------------------------------------------------------------------
VOID CanKillPendingIrps(PDEVICE_OBJECT pDevObj)
{
  PCAN_DEVICE_EXTENSION pDevExt = pDevObj->DeviceExtension;

  // First kill all the reads and writes.
  CanKillAllReadsOrWrites(pDevObj, &pDevExt->WriteQueue, &pDevExt->CurrentWriteIrp);
  CanKillAllReadsOrWrites(pDevObj, &pDevExt->ReadQueue, &pDevExt->CurrentReadIrp);
  // Next get rid of purges.
  CanKillAllReadsOrWrites(pDevObj, &pDevExt->PurgeQueue, &pDevExt->CurrentPurgeIrp);
  // Finally, dump any stalled IRPS
  CanKillAllStalled(pDevObj);
}
//-----------------------------------------------------------------------------
VOID CanDisableInterfacesResources(IN PDEVICE_OBJECT pDevObj)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)pDevObj->DeviceExtension;

  PAGED_CODE();

  // Only do these many things if the device has started and still
  // has resources allocated

  if(pDevExt->Flags & CAN_FLAGS_STARTED)
  {
    if(!(pDevExt->Flags & CAN_FLAGS_STOPPED))
    {
      HW_CAN_DISABLE_ALL_INTERRUPTS(&pDevExt->Controller);
    }

    CanReleaseResources(pDevExt);

    // Remove us from WMI consideration
    //IoWMIRegistrationControl(pDevObj, WMIREG_ACTION_DEREGISTER);
  }

  // Undo external names
  CanUndoExternalNaming(pDevExt);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    Removes a can device object from the system.
//
//Arguments:
//
//    pDevObj - A pointer to the Device Object we want removed.
//
//Return Value:
//
//    Always TRUE
//-----------------------------------------------------------------------------
NTSTATUS CanRemoveDevObj(IN PDEVICE_OBJECT pDevObj)
{
  PCAN_DEVICE_EXTENSION pDevExt = (PCAN_DEVICE_EXTENSION)pDevObj->DeviceExtension;

  PAGED_CODE();

  F_DBG("Remove us\n");

  if(!(pDevExt->DevicePNPAccept & CAN_PNPACCEPT_SURPRISE_REMOVING))
  {
    // Disable all external interfaces and release resources
    CanDisableInterfacesResources(pDevObj);
  }

  IoDetachDevice(pDevExt->LowerDeviceObject);

  // Free memory allocated in the extension
  if(pDevExt->DeviceName.Buffer != NULL)
  {
    ExFreePool(pDevExt->DeviceName.Buffer);
    pDevExt->DeviceName.Buffer = NULL;
  }
  if(pDevExt->SymbolicLinkName.Buffer != NULL)
  {
    ExFreePool(pDevExt->SymbolicLinkName.Buffer);
    pDevExt->SymbolicLinkName.Buffer = NULL;
  }
  if(pDevExt->pConfig != NULL)
  {
    ExFreePool(pDevExt->pConfig);
    pDevExt->pConfig = NULL;
  }

  // Delete the devobj
  IoDeleteDevice(pDevObj);

  return STATUS_SUCCESS;
}

#endif


