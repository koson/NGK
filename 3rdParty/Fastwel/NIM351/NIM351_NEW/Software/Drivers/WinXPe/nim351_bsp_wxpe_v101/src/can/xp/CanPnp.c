#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

#include <eal/el.h>
#include <can/xp/CanDrv.h>

//-----------------------------------------------------------------------------
static const PHYSICAL_ADDRESS CanPhysicalZero = {0};

#ifdef ALLOC_PRAGMA
#pragma alloc_text(PAGE, CanCreateDevObj)
#pragma alloc_text(PAGE, CanAddDevice)
#pragma alloc_text(PAGE, CanDispatchPnp)
#pragma alloc_text(PAGE, CanStartDevice)
#pragma alloc_text(PAGE, CanFinishStartDevice)
#pragma alloc_text(PAGE, CanFinishRestartDevice)
#pragma alloc_text(PAGE, CanGetPortInfo)
#pragma alloc_text(PAGE, CanDoExternalNaming)
#pragma alloc_text(PAGE, CanUndoExternalNaming)
#pragma alloc_text(PAGE, CanFinishRestartDevice)
#endif // ALLOC_PRAGMA

////
//// Instantiate the GUID
////
//#include <initguid.h>
//
//DEFINE_GUID(GUID_CLASS_COMPORT, 0x86e0d1e0L, 0x8089, 0x11d0, 0x9c, 0xe4, 0x08,
//            0x00, 0x3e, 0x30, 0x1f, 0x73);

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
NTSTATUS CanSyncCompletion(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp, IN PKEVENT SyncEvent)
{
  KeSetEvent(SyncEvent, IO_NO_INCREMENT, FALSE);
  return STATUS_MORE_PROCESSING_REQUIRED;
}
//-----------------------------------------------------------------------------
NTSTATUS CanCreateDevObj(IN PDRIVER_OBJECT DriverObject, OUT PDEVICE_OBJECT *NewDeviceObject)
{
  UNICODE_STRING deviceObjName;
  UNICODE_STRING instanceStr;
  WCHAR instanceNumberBuffer[20];
  static ULONG currentInstance = 0;
  PDEVICE_OBJECT deviceObject = NULL;
  PCAN_DEVICE_EXTENSION pDevExt;
  NTSTATUS status = STATUS_SUCCESS;

  PAGED_CODE();

  F_DBG("CanCreateDevObj entry\n");
  do
  {
    // Create nt device name
    RtlZeroMemory(&deviceObjName, sizeof(UNICODE_STRING));
    deviceObjName.MaximumLength = DEVICE_OBJECT_NAME_LENGTH * sizeof(WCHAR);
    deviceObjName.Buffer = ExAllocatePool(PagedPool, deviceObjName.MaximumLength + sizeof(WCHAR));
    if(deviceObjName.Buffer == NULL)
    {
      status = STATUS_INSUFFICIENT_RESOURCES;
      break;
    }

    RtlZeroMemory(deviceObjName.Buffer, deviceObjName.MaximumLength + sizeof(WCHAR));
    RtlAppendUnicodeToString(&deviceObjName, DEVICE_NAME_U);

    RtlInitUnicodeString(&instanceStr, NULL);
    instanceStr.MaximumLength = sizeof(instanceNumberBuffer);
    instanceStr.Buffer = instanceNumberBuffer;
    RtlIntegerToUnicodeString(currentInstance++, 10, &instanceStr);
    RtlAppendUnicodeStringToString(&deviceObjName, &instanceStr);

    // Create the device object
    status = IoCreateDevice(
      DriverObject, 
      sizeof(CAN_DEVICE_EXTENSION),
      &deviceObjName, 
      FILE_DEVICE_UNKNOWN,
      FILE_DEVICE_SECURE_OPEN, 
      TRUE, 
      &deviceObject);
    if(!NT_SUCCESS(status))
      break;

    ASSERT(deviceObject != NULL);

    // Set up the device extension.
    pDevExt = deviceObject->DeviceExtension;
    RtlZeroMemory(pDevExt, sizeof(CAN_DEVICE_EXTENSION));

    // Allocate Buffer for CAN resourses config
    pDevExt->pConfig = ExAllocatePool(PagedPool, sizeof(CONFIG_DATA));
    if(pDevExt->pConfig == NULL)
    {
      F_DBG("Couldn't allocate memory for pConfig\n");
      status = STATUS_INSUFFICIENT_RESOURCES;
      break;
    }
    RtlZeroMemory(pDevExt->pConfig, sizeof(CONFIG_DATA));

    // Allocate Pool and save the nt device name in the device extension.
    pDevExt->DeviceName.Buffer = ExAllocatePool(PagedPool, deviceObjName.Length + sizeof(WCHAR));
    if(!pDevExt->DeviceName.Buffer)
    {
      F_DBG("Couldn't allocate memory for DeviceName\n");
      status = STATUS_INSUFFICIENT_RESOURCES;
      break;
    }
    pDevExt->DeviceName.MaximumLength = deviceObjName.Length + sizeof(WCHAR);
    RtlZeroMemory(pDevExt->DeviceName.Buffer, pDevExt->DeviceName.MaximumLength);
    RtlAppendUnicodeStringToString(&pDevExt->DeviceName, &deviceObjName);

    // Initialize the count of IRP's pending
    pDevExt->PendingIRPCnt = 1;
    // Initialize the count of DPC's pending
    pDevExt->DpcCount = 1;
    pDevExt->DeviceIsOpened = FALSE;
    pDevExt->DeviceObject   = deviceObject;
    pDevExt->DriverObject   = DriverObject;
    pDevExt->PowerState     = PowerDeviceD0;
    pDevExt->TxFifoAmount   = driverDefaults.TxFIFODefault;
    pDevExt->CreatedSymbolicLink    = FALSE;

    InitializeListHead(&pDevExt->CommonInterruptObject);
    InitializeListHead(&pDevExt->TopLevelSharers);
    InitializeListHead(&pDevExt->AllDevObjs);
    InitializeListHead(&pDevExt->ReadQueue);
    InitializeListHead(&pDevExt->WriteQueue);
    InitializeListHead(&pDevExt->PurgeQueue);
    InitializeListHead(&pDevExt->WaitQueue);
    InitializeListHead(&pDevExt->StalledIrpQueue);

    ExInitializeFastMutex(&pDevExt->OpenMutex);
    ExInitializeFastMutex(&pDevExt->CloseMutex);

    // Initialize the spinlock associated with fields read (& set)
    // by IO Control functions and the flags spinlock.
    KeInitializeSpinLock(&pDevExt->ControlLock);
    KeInitializeSpinLock(&pDevExt->FlagsLock);

    KeInitializeEvent(&pDevExt->PendingIRPEvent, SynchronizationEvent, FALSE);
    KeInitializeEvent(&pDevExt->PendingDpcEvent, SynchronizationEvent, FALSE);
    KeInitializeEvent(&pDevExt->PowerD0Event, SynchronizationEvent, FALSE);

    deviceObject->Flags &= ~DO_DEVICE_INITIALIZING;
    status = STATUS_SUCCESS;
  }
  while(0);

  if(!NT_SUCCESS(status))
  {
    if(deviceObject)
    {
      if(pDevExt->DeviceName.Buffer != NULL)
      {  
        ExFreePool(pDevExt->DeviceName.Buffer);
        pDevExt->DeviceName.Buffer = NULL;
      }

      if(pDevExt->pConfig != NULL)
      {
        ExFreePool(pDevExt->pConfig);
        pDevExt->pConfig = NULL;
      }

      IoDeleteDevice(deviceObject);
      deviceObject = NULL;
    }
  }

  // Free the allocated strings for the NT and symbolic names if they exist.
  if(deviceObjName.Buffer != NULL)
    ExFreePool(deviceObjName.Buffer);

  *NewDeviceObject = deviceObject;

  return status;
}
//-----------------------------------------------------------------------------
NTSTATUS CanAddDevice(PDRIVER_OBJECT DriverObject, PDEVICE_OBJECT PhysicalDeviceObject)
{
  PDEVICE_OBJECT pNewDevObj = NULL;
  PDEVICE_OBJECT pLowerDevObj = NULL;
  NTSTATUS status = STATUS_SUCCESS;
  PCAN_DEVICE_EXTENSION pDevExt;

  PAGED_CODE();

  F_DBG("CanAddDevice entry\n");
  
  do
  {
    if(PhysicalDeviceObject == NULL)
    {
      F_DBG("CanAddDevice(): STATUS_NO_MORE_ENTRIES\n");
      status = STATUS_NO_MORE_ENTRIES;
      break;
    }

    // create and initialize the new device object
    status = CanCreateDevObj(DriverObject, &pNewDevObj);
    if(!NT_SUCCESS(status))
      break;

    // Layer our DO on top of the lower device object
    // The return value is a pointer to the device object to which the
    // DO is actually attached.
    pLowerDevObj = IoAttachDeviceToDeviceStack(pNewDevObj, PhysicalDeviceObject);

    // No status. Do the best we can.
    ASSERT(pLowerDevObj != NULL);

    pDevExt = pNewDevObj->DeviceExtension;
    pDevExt->LowerDeviceObject = pLowerDevObj;
    pDevExt->PhysicalDeviceObject = PhysicalDeviceObject;

    // Specify that this driver only supports buffered IO.  This basically
    // means that the IO system copies the users data to and from
    // system supplied buffers.
    // Also specify that we are power pagable.
    pNewDevObj->Flags |= (DO_BUFFERED_IO | DO_POWER_PAGABLE);

    status = STATUS_SUCCESS;
  }
  while(0);
  
  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This is a dispatch routine for the IRPs that come to the driver with the
//    IRP_MJ_PNP major code (plug-and-play IRPs).
//
//Arguments:
//
//    pDevObj - Pointer to the device object for this device
//
//    PIrp - Pointer to the IRP for the current request
//
//Return Value:
//
//    The function value is the final status of the call
NTSTATUS CanDispatchPnp(IN PDEVICE_OBJECT pDevObj, IN PIRP PIrp)
{
  PCAN_DEVICE_EXTENSION pDevExt = pDevObj->DeviceExtension;
  PDEVICE_OBJECT pLowerDevObj = pDevExt->LowerDeviceObject;
  PIO_STACK_LOCATION pIrpStack = IoGetCurrentIrpStackLocation(PIrp);
  NTSTATUS status;

  PAGED_CODE();

  if((status = CanIRPPrologue(PIrp, pDevExt)) != STATUS_SUCCESS)
  {
    CanCompleteRequest(pDevExt, PIrp, IO_NO_INCREMENT);
    return status;
  }

  switch (pIrpStack->MinorFunction) 
  {
  case IRP_MN_START_DEVICE: 
    {
      F_DBG("Got IRP_MN_START_DEVICE Irp\n");

      // CanStartDevice will pass this Irp to the next driver,
      // and process it as completion so just complete it here.
      MmLockPagableSectionByHandle(CanGlobals.PAGEFCAN_Handle);
      status = CanStartDevice(pDevObj, PIrp);
      MmUnlockPagableImageSection(CanGlobals.PAGEFCAN_Handle);

      PIrp->IoStatus.Status = status;
      CanCompleteRequest(pDevExt, PIrp, IO_NO_INCREMENT);
      return status;
    }

  case IRP_MN_STOP_DEVICE:
    {
      ULONG pendingIRPs;

      F_DBG("Got IRP_MN_STOP_DEVICE Irp\n");

      CanSetFlags(pDevExt, CAN_FLAGS_STOPPED);
      CanSetAccept(pDevExt, CAN_PNPACCEPT_STOPPED);
      CanClearAccept(pDevExt, CAN_PNPACCEPT_STOPPING);

      pDevExt->PNPState = CAN_PNP_STOPPING;
      // From this point on all non-PNP IRP's will be queued
      //
      // Decrement for entry here
      InterlockedDecrement(&pDevExt->PendingIRPCnt);
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

      // Pass the irp down
      PIrp->IoStatus.Status = STATUS_SUCCESS;
      IoSkipCurrentIrpStackLocation(PIrp);
      return IoCallDriver(pLowerDevObj, PIrp);
    }
  
  case IRP_MN_REMOVE_DEVICE:
    // If we get this, we have to remove
    {
      ULONG pendingIRPs;

      F_DBG("Got IRP_MN_REMOVE_DEVICE Irp\n");

      // Mark as not accepting requests
      CanSetAccept(pDevExt, CAN_PNPACCEPT_REMOVING);
      // Complete all pending requests
      CanKillPendingIrps(pDevObj);
      // Decrement for this Irp itself
      InterlockedDecrement(&pDevExt->PendingIRPCnt);
      // Wait for any pending requests we raced on -- this decrement
      // is for our "placeholder".
      pendingIRPs = InterlockedDecrement(&pDevExt->PendingIRPCnt);
      if(pendingIRPs)
      {
        F_DBG("Wait for pending requests...\n");
        KeWaitForSingleObject(&pDevExt->PendingIRPEvent, Executive, KernelMode, FALSE, NULL);
      }
      // Remove us
      CanRemoveDevObj(pDevObj);

      // Pass the irp down
      PIrp->IoStatus.Status = STATUS_SUCCESS;
      //IoCopyCurrentIrpStackLocationToNext(PIrp);
      IoSkipCurrentIrpStackLocation(PIrp);
      return IoCallDriver(pLowerDevObj, PIrp);
    }

  default:
    break;
  }

  // Pass to driver beneath us
  IoSkipCurrentIrpStackLocation(PIrp);
  status = CanIoCallDriver(pDevExt, pLowerDevObj, PIrp);
  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine will get the configuration information and put
//    it and the translated values into CONFIG_DATA structures.
//    It first sets up with  defaults and then queries the registry
//    to see if the user has overridden these defaults; if this is a legacy
//    multiport card, it uses the info in PUserData instead of groping the
//    registry again.
//
//Arguments:
//
//    pDevObj - Pointer to the device object.
//
//    PResList - Pointer to the untranslated resources requested.
//
//    PTrResList - Pointer to the translated resources requested.
//
//    PConfig - Pointer to configuration info
//
//    PUserData - Pointer to data discovered in the registry for
//    legacy devices.
//
//Return Value:
//
//    STATUS_SUCCESS if consistant configuration was found.
NTSTATUS CanGetPortInfo(IN PDEVICE_OBJECT pDevObj, IN PCM_RESOURCE_LIST PResList, IN PCM_RESOURCE_LIST PTrResList, OUT PCONFIG_DATA PConfig)
{
  PCAN_DEVICE_EXTENSION pDevExt = pDevObj->DeviceExtension;
  PDEVICE_OBJECT pLowerDevObj = pDevExt->LowerDeviceObject;
  NTSTATUS status = STATUS_NOT_IMPLEMENTED;
  CONFIGURATION_TYPE pointer = PointerPeripheral;
  CONFIGURATION_TYPE controllerType  = OtherController;

  HANDLE keyHandle;
  ULONG count;
  ULONG i;

  PCM_PARTIAL_RESOURCE_LIST pPartialResourceList, pPartialTrResourceList;
  PCM_PARTIAL_RESOURCE_DESCRIPTOR pPartialResourceDesc, pPartialTrResourceDesc;

  PCM_FULL_RESOURCE_DESCRIPTOR pFullResourceDesc = NULL, pFullTrResourceDesc = NULL;

  ULONG zero = 0;
  ULONG gotInt = 0;
  ULONG gotIO = 0;
  ULONG ioResIndex = 0;
  ULONG curIoIndex = 0;
  ULONG gotMem = 0;

  PAGED_CODE();

  if((PResList == NULL) || (PTrResList == NULL))
  {
    // This shouldn't happen in theory
    ASSERT(PResList != NULL);
    ASSERT(PTrResList != NULL);
    // This status is as appropriate as I can think of
    return STATUS_INSUFFICIENT_RESOURCES;
  }

  // Each resource list should have only one set of resources
  ASSERT(PResList->Count == 1);
  ASSERT(PTrResList->Count == 1);

  pFullResourceDesc   = &PResList->List[0];
  pFullTrResourceDesc = &PTrResList->List[0];

  // Ok, if we have a full resource descriptor.  Let's take it apart.
  if(pFullResourceDesc) 
  {
    pPartialResourceList    = &pFullResourceDesc->PartialResourceList;
    pPartialResourceDesc    = pPartialResourceList->PartialDescriptors;
    count                   = pPartialResourceList->Count;

    // Pull out the stuff that is in the full descriptor.
    PConfig->InterfaceType  = pFullResourceDesc->InterfaceType;
    PConfig->BusNumber      = pFullResourceDesc->BusNumber;

    // Now run through the partial resource descriptors looking for the port and interrupt.
    for(i = 0; i < count; i++, pPartialResourceDesc++)
    {
      switch(pPartialResourceDesc->Type)
      {
      case CmResourceTypePort:
        if(gotIO==0)
        {
          F_DBG2("IO_Port Resource: Start(%X), Length(%lu)\n", pPartialResourceDesc->u.Port.Start, pPartialResourceDesc->u.Port.Length);
          gotIO = 1;
#ifdef FWCAN_USE_SWITCH_DEVICE
          PConfig->switchPort.start = pPartialResourceDesc->u.Port.Start;
          PConfig->switchPort.length = pPartialResourceDesc->u.Port.Length;
          PConfig->switchPort.addressSpace = pPartialResourceDesc->Flags;
#endif
        }
        break;

      case CmResourceTypeMemory:
        F_DBG2("Memory Resource: Start(%X), Length(%lu)\n", pPartialResourceDesc->u.Memory.Start, pPartialResourceDesc->u.Memory.Length);
        if((gotMem==0)/*&&(pPartialResourceDesc->u.Memory.Length==CAN_REGISTER_SPAN)*/)
        {
          gotMem = 1;
          PConfig->Controller = pPartialResourceDesc->u.Memory.Start;
          PConfig->SpanOfController = CAN_REGISTER_SPAN;
          PConfig->AddressSpace = CM_RESOURCE_PORT_MEMORY;
        }
        break;

      case CmResourceTypeInterrupt: 
        if(gotInt == 0)
        {
          gotInt = 1;
          PConfig->OriginalIrql = pPartialResourceDesc->u.Interrupt.Level;
          PConfig->OriginalVector = pPartialResourceDesc->u.Interrupt.Vector;
          PConfig->Affinity = pPartialResourceDesc->u.Interrupt.Affinity;
          PConfig->InterruptMode = (pPartialResourceDesc->Flags & CM_RESOURCE_INTERRUPT_LATCHED)? Latched: LevelSensitive;
        }
        break;

      case CmResourceTypeDeviceSpecific:
        break;

      default:
        break;
      }
    }
  }

  // Do the same for the translated resources
  gotInt = gotIO = gotMem = 0;
  if(pFullTrResourceDesc) 
  {
    pPartialTrResourceList = &pFullTrResourceDesc->PartialResourceList;
    pPartialTrResourceDesc = pPartialTrResourceList->PartialDescriptors;
    count = pPartialTrResourceList->Count;

    // Reload PConfig with the translated values for later use
    PConfig->InterfaceType  = pFullTrResourceDesc->InterfaceType;
    PConfig->BusNumber      = pFullTrResourceDesc->BusNumber;

    for(i = 0; i < count; i++, pPartialTrResourceDesc++)
    {
      switch(pPartialTrResourceDesc->Type)
      {
      case CmResourceTypePort:
        if(gotIO == 0)
        {
          F_DBG2("IO_Port Resource translated: Start(%X), Length(%lu)\n", pPartialTrResourceDesc->u.Port.Start, pPartialTrResourceDesc->u.Port.Length);
          gotIO = 1;
#ifdef FWCAN_USE_SWITCH_DEVICE
          PConfig->switchPort.start = pPartialTrResourceDesc->u.Port.Start;
          PConfig->switchPort.length = pPartialTrResourceDesc->u.Port.Length;
          PConfig->switchPort.addressSpace = pPartialTrResourceDesc->Flags;
#endif
        }
        break;

      case CmResourceTypeMemory:
        F_DBG2("Memory Resource Translated: Start(%X), Length(%lu)\n", pPartialTrResourceDesc->u.Memory.Start, pPartialTrResourceDesc->u.Memory.Length);
        if((gotMem==0)/*&&(pPartialTrResourceDesc->u.Memory.Length==CAN_REGISTER_SPAN)*/)
        {
          gotMem = 1;
          PConfig->TrController = pPartialTrResourceDesc->u.Memory.Start;
          PConfig->SpanOfController = CAN_REGISTER_SPAN;
          PConfig->AddressSpace = CM_RESOURCE_PORT_MEMORY;
        }
        break;

      case CmResourceTypeInterrupt:
        if(gotInt == 0)
        {
          gotInt = 1;
          PConfig->TrVector = pPartialTrResourceDesc->u.Interrupt.Vector;
          PConfig->TrIrql = pPartialTrResourceDesc->u.Interrupt.Level;
          PConfig->Affinity = pPartialTrResourceDesc->u.Interrupt.Affinity;
        }
        break;

      default:
        break;
      }
    }
  }

  // Initialize a config data structure with default values for those that
  // may not already be initialized.
  PConfig->DisablePort = 0;
  PConfig->RxFIFO = driverDefaults.RxFIFODefault;
  PConfig->TxFIFO = driverDefaults.TxFIFODefault;
  PConfig->ControllerSettings = driverDefaults.ControllerSettingsDefault;

  // Get any user data associated with the port now and override the
  // values passed in if applicable.

  // Open the "Device Parameters" section of registry for this device object.
  status = IoOpenDeviceRegistryKey(pDevExt->PhysicalDeviceObject, PLUGPLAY_REGKEY_DEVICE, STANDARD_RIGHTS_READ, &keyHandle);
  if(NT_SUCCESS(status))
  {
    status = CanGetRegistryKeyValue(keyHandle, L"DisablePort", sizeof(L"DisablePort"), &PConfig->DisablePort, sizeof (ULONG));
    status = CanGetRegistryKeyValue(keyHandle, L"RxFIFO", sizeof(L"RxFIFO"), &PConfig->RxFIFO, sizeof (ULONG));
    status = CanGetRegistryKeyValue(keyHandle, L"TxFIFO", sizeof(L"TxFIFO"), &PConfig->TxFIFO, sizeof (ULONG));
    ZwClose (keyHandle);
  }

  // Do some error checking on the configuration info we have.
  //
  // Make sure that the interrupt is non zero (which we defaulted it to).
  //
  // Make sure that the portaddress is non zero (which we defaulted it to).
  //
  // Make sure that the DosDevices is not NULL (which we defaulted it to).
  //
  // We need to make sure that if an interrupt status
  // was specified, that a port index was also specfied,
  // and if so that the port index is <= maximum ports
  // on a board.
  //
  // We should also validate that the bus type and number are correct.
  //
  // We will also validate that the interrupt mode makes sense for the bus.
  do
  {
    if(!PConfig->Controller.LowPart)
    {
      // Ehhhh! Lose Game.
      // CanLogError(...);
      F_DBG1("Bogus controller address %x\n", PConfig->Controller.LowPart);
      status = STATUS_INVALID_PARAMETER;
      break;
    }

#ifdef FWCAN_USE_SWITCH_DEVICE
    if(!PConfig->switchPort.start.LowPart)
    {
      // Ehhhh! Lose Game.
      // CanLogError(...);
      F_DBG1("Bogus switch address %x\n", PConfig->switchPort.start.LowPart);
      status = STATUS_INVALID_PARAMETER;
      break;
    }
#endif

    if(!PConfig->OriginalVector)
    {
      // Ehhhh! Lose Game.
      //LogError(...);
      F_DBG1("Bogus vector %x\n", PConfig->OriginalVector);
      status = STATUS_INVALID_PARAMETER;
      break;
    }

    status = STATUS_SUCCESS;

    // Dump out the port configuration.
    F_DBG1("Can Port address: %x\n", PConfig->Controller.LowPart);
    F_DBG1("Can Port BusNumber: %x\n", PConfig->BusNumber);
    F_DBG1("Can AddressSpace: %x\n", PConfig->AddressSpace);
    F_DBG1("Can InterruptMode: %x\n", PConfig->InterruptMode);
    F_DBG1("Can InterfaceType: %x\n", PConfig->InterfaceType);
    F_DBG1("Can OriginalVector: %x\n", PConfig->OriginalVector);
    F_DBG1("Can OriginalIrql: %x\n", PConfig->OriginalIrql);
  }
  while(0);

  return status;
}
//-----------------------------------------------------------------------------
VOID CanAddToAllDevs(PLIST_ENTRY PListEntry)
{
  KIRQL oldIrql;
  
  KeAcquireSpinLock(&CanGlobals.GlobalsSpinLock, &oldIrql);
  InsertTailList(&CanGlobals.AllDevObjs, PListEntry);
  KeReleaseSpinLock(&CanGlobals.GlobalsSpinLock, oldIrql);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine does specific procedures to start a device.
//
//
//Arguments:
//
//   pDevObj    -  Pointer to the devobj that is starting
//
//   PResList   -  Pointer to the untranslated resources needed by this device
//
//   PTrResList -  Pointer to the translated resources needed by this device
//
//   PUserData  -  Pointer to the user-specified resources/attributes
//
//
//  Return Value:
//
//    STATUS_SUCCESS on success, something else appropriate on failure
//-----------------------------------------------------------------------------
NTSTATUS CanFinishStartDevice(IN PDEVICE_OBJECT pDevObj, IN PCM_RESOURCE_LIST PResList, IN PCM_RESOURCE_LIST PTrResList)
{
  PCAN_DEVICE_EXTENSION pDevExt = pDevObj->DeviceExtension;
  NTSTATUS status;
  PCONFIG_DATA pConfig = NULL;
  ULONG one = 1;

  PAGED_CODE();

  // See if this is a restart, and if so don't reallocate the world
  if((pDevExt->Flags & CAN_FLAGS_STOPPED) && (pDevExt->Flags & CAN_FLAGS_STARTED)) 
  {
    CanClearFlags(pDevExt, CAN_FLAGS_STOPPED);
    pDevExt->PNPState = CAN_PNP_RESTARTING;

    // Re-init resource-related things in the extension
    pDevExt->TopLevelOurIsr = NULL;
    pDevExt->TopLevelOurIsrContext = NULL;

    pDevExt->OriginalController = CanPhysicalZero;
    pDevExt->OriginalInterruptStatus = CanPhysicalZero;

    pDevExt->OurIsr = NULL;
    pDevExt->OurIsrContext = NULL;

    pDevExt->Controller.reg_base = NULL;
    pDevExt->Interrupt = NULL;

    pDevExt->SpanOfController = 0;

    pDevExt->Vector = 0;
    pDevExt->Irql = 0;
    pDevExt->OriginalVector = 0;
    pDevExt->OriginalIrql = 0;
    pDevExt->AddressSpace = 0;
    pDevExt->BusNumber = 0;
    pDevExt->InterfaceType = 0;
    pDevExt->CIsrSw = NULL;
  }

  do
  {
    // Allocate the config record.
    pConfig = pDevExt->pConfig;
    RtlZeroMemory(pConfig, sizeof(CONFIG_DATA));

    // Get the configuration info for the device.
    status = CanGetPortInfo(pDevObj, PResList, PTrResList, pConfig);
    if(!NT_SUCCESS(status))
      break;

    // See if we are in the proper power state.
    if(pDevExt->PowerState != PowerDeviceD0)
    {
      //status = CanGotoPowerState(pDevExt->Pdo, pDevExt, PowerDeviceD0);
      //if(!NT_SUCCESS(status))
      //  break;
    }

    // Find and initialize the controller
    status = CanFindInitController(pDevObj, pConfig);
    if(!NT_SUCCESS(status)) 
      break;
   
    // The hardware that is set up to NOT interrupt, connect an interrupt.
    //
    // If a device doesn't already have an interrupt and it has an isr then
    // we attempt to connect to the interrupt if it is not shareing with other
    // devices.  If we fail to connect to an  interrupt we will delete this device.
    if((!pDevExt->Interrupt) && (pDevExt->OurIsr))
    {
      // Do a just in time construction of the ISR switch.
      pDevExt->CIsrSw->IsrFunc = pDevExt->OurIsr;
      pDevExt->CIsrSw->Context = pDevExt->OurIsrContext;
      status = IoConnectInterrupt(
        &pDevExt->Interrupt, CanCIsrSw, 
        pDevExt->CIsrSw, NULL, 
        pDevExt->Vector, pDevExt->Irql,
        pDevExt->Irql,
        pConfig->InterruptMode,
        pDevExt->InterruptShareable,
        pConfig->Affinity, FALSE);

      if(!NT_SUCCESS(status))
      {
        // Hmmm, how'd that happen?  Somebody either
        // didn't report their resources, or they
        // sneaked in since the last time I looked.
        //
        // Oh well,  delete this device.
        break;
      }
    }
    F_DBG1("Connected interrupt %08X\n", pDevExt->Interrupt);

    // Add the pDevObj to the master list
    CanAddToAllDevs(&pDevExt->AllDevObjs);

    // While the device isn't open, set device to reset mode.
    KeSynchronizeExecution(pDevExt->Interrupt, CanSetResetMode, pDevExt);

    if(pDevExt->PNPState == CAN_PNP_ADDED)
    {
      // Do the external naming now that the device is accessible.
      status = CanDoExternalNaming(pDevExt, pConfig);
      if(!NT_SUCCESS(status))
      {
        F_DBG1("External Naming Failed - Status %x\n", status);
#if 0
        CanLogError(pDevExt->DriverObject,  NULL, 0, 0, 0, 21, 
          status, FWCAN_NO_SYMLINK_CREATED, 
          pDevExt->DeviceName.Length + sizeof(WCHAR), pDevExt->DeviceName.Buffer, 
          0, NULL);  
#endif
        // Allow the device to start anyhow
        status = STATUS_SUCCESS;
      }
    }
    else
    {
      F_DBG1("Not doing external naming -- state is %x\n", pDevExt->PNPState);
    }
  }
  while(0);

  if(!NT_SUCCESS (status))
  {
    F_DBG("Cleaning up failed start\n");

    // Resources created by this routine will be cleaned up by the remove
    if(pDevExt->PNPState == CAN_PNP_RESTARTING)
    {
      // Kill all that lives and breathes -- we'll clean up the
      // rest on the impending remove
      CanKillPendingIrps(pDevObj);

      // In fact, pretend we're removing so we don't take any
      // more irps
      CanSetAccept(pDevExt, CAN_PNPACCEPT_REMOVING);
      CanClearFlags(pDevExt, CAN_FLAGS_STARTED);
    }
  }
  else
  {
    // SUCCESS
    if(pDevExt->PNPState == CAN_PNP_ADDED)
    {
      //IoWMIRegistrationControl(pDevObj, WMIREG_ACTION_REGISTER);
    }

    if(pDevExt->PNPState == CAN_PNP_RESTARTING)
    {
      // Release the stalled IRP's
      ////CanUnstallIrps(pDevExt);
    }

    pDevExt->PNPState = CAN_PNP_STARTED;
    CanClearAccept(pDevExt, ~CAN_PNPACCEPT_OK);
    CanSetFlags(pDevExt, CAN_FLAGS_STARTED);
  }

  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine does specific procedures to restart a device.
//
//
//Arguments:
//
//   pDevObj    -  Pointer to the devobj that is starting
//
//  Return Value:
//
//    STATUS_SUCCESS on success, something else appropriate on failure
//-----------------------------------------------------------------------------
NTSTATUS CanFinishRestartDevice(IN PDEVICE_OBJECT pDevObj)
{
  PCAN_DEVICE_EXTENSION pDevExt = pDevObj->DeviceExtension;
  NTSTATUS status;
  PCONFIG_DATA pConfig = NULL;
  ULONG one = 1;

  PAGED_CODE();

  // See if this is a restart, and if so don't reallocate the world
  if((pDevExt->Flags & CAN_FLAGS_STOPPED) && (pDevExt->Flags & CAN_FLAGS_STARTED)) 
  {
    CanClearFlags(pDevExt, CAN_FLAGS_STARTED);
    CanClearFlags(pDevExt, CAN_FLAGS_STOPPED);
    pDevExt->PNPState = CAN_PNP_RESTARTING;

    // Re-init resource-related things in the extension
    pDevExt->TopLevelOurIsr = NULL;
    pDevExt->TopLevelOurIsrContext = NULL;

    pDevExt->OriginalController = CanPhysicalZero;
    pDevExt->OriginalInterruptStatus = CanPhysicalZero;

    pDevExt->OurIsr = NULL;
    pDevExt->OurIsrContext = NULL;

    pDevExt->Controller.reg_base = NULL;
    pDevExt->Interrupt = NULL;

    pDevExt->SpanOfController = 0;

    pDevExt->Vector = 0;
    pDevExt->Irql = 0;
    pDevExt->OriginalVector = 0;
    pDevExt->OriginalIrql = 0;
    pDevExt->AddressSpace = 0;
    pDevExt->BusNumber = 0;
    pDevExt->InterfaceType = 0;
    pDevExt->CIsrSw = NULL;
  }

  do
  {
    pConfig = pDevExt->pConfig;

    // See if we are in the proper power state.
    if(pDevExt->PowerState != PowerDeviceD0)
    {
      //status = CanGotoPowerState(pDevExt->Pdo, pDevExt, PowerDeviceD0);
      //if(!NT_SUCCESS(status))
      //  break;
    }

    // Find and initialize the controller
    status = CanFindInitController(pDevObj, pConfig);
    if(!NT_SUCCESS(status)) 
      break;
   
    // The hardware that is set up to NOT interrupt, connect an interrupt.
    //
    // If a device doesn't already have an interrupt and it has an isr then
    // we attempt to connect to the interrupt if it is not shareing with other
    // devices.  If we fail to connect to an  interrupt we will delete this device.
    if((!pDevExt->Interrupt) && (pDevExt->OurIsr))
    {
      // Do a just in time construction of the ISR switch.
      pDevExt->CIsrSw->IsrFunc = pDevExt->OurIsr;
      pDevExt->CIsrSw->Context = pDevExt->OurIsrContext;
      status = IoConnectInterrupt(
        &pDevExt->Interrupt, CanCIsrSw, 
        pDevExt->CIsrSw, NULL, 
        pDevExt->Vector, pDevExt->Irql,
        pDevExt->Irql,
        pConfig->InterruptMode,
        pDevExt->InterruptShareable,
        pConfig->Affinity, FALSE);

      if(!NT_SUCCESS(status))
      {
        // Hmmm, how'd that happen?  Somebody either
        // didn't report their resources, or they
        // sneaked in since the last time I looked.
        //
        // Oh well,  delete this device.
        break;
      }
    }
    F_DBG1("Connected interrupt %08X\n", pDevExt->Interrupt);

    // Add the pDevObj to the master list
    CanAddToAllDevs(&pDevExt->AllDevObjs);

    // While the device isn't open, set device to reset mode.
    KeSynchronizeExecution(pDevExt->Interrupt, CanSetResetMode, pDevExt);
  }
  while(0);

  if(!NT_SUCCESS (status))
  {
    F_DBG("Cleaning up failed start\n");

    // Resources created by this routine will be cleaned up by the remove
    if(pDevExt->PNPState == CAN_PNP_RESTARTING)
    {
      // Kill all that lives and breathes -- we'll clean up the
      // rest on the impending remove
      CanKillPendingIrps(pDevObj);

      // In fact, pretend we're removing so we don't take any
      // more irps
      CanSetAccept(pDevExt, CAN_PNPACCEPT_REMOVING);
      CanClearFlags(pDevExt, CAN_FLAGS_STARTED);
    }
  }
  else
  {
    // SUCCESS
    if(pDevExt->PNPState == CAN_PNP_ADDED)
    {
      //IoWMIRegistrationControl(pDevObj, WMIREG_ACTION_REGISTER);
    }

    if(pDevExt->PNPState == CAN_PNP_RESTARTING)
    {
      // Release the stalled IRP's
      ////CanUnstallIrps(pDevExt);
    }

    pDevExt->PNPState = CAN_PNP_STARTED;
    CanClearAccept(pDevExt, ~CAN_PNPACCEPT_OK);
    CanSetFlags(pDevExt, CAN_FLAGS_STARTED);
  }
  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine first passes the start device Irp down the stack then
//    it picks up the resources for the device, ititializes, puts it on any
//    appropriate lists (i.e shared interrupt or interrupt status) and
//    connects the interrupt.
//
//Arguments:
//
//    pDevObj - Pointer to the device object for this device
//
//    PIrp - Pointer to the IRP for the current request
//
//Return Value:
//
//    Return status
//-----------------------------------------------------------------------------
NTSTATUS CanStartDevice(IN PDEVICE_OBJECT pDevObj, IN PIRP PIrp)
{
  PIO_STACK_LOCATION pIrpStack = IoGetCurrentIrpStackLocation(PIrp);
  NTSTATUS status = STATUS_NOT_IMPLEMENTED;
  PCAN_DEVICE_EXTENSION pDevExt = pDevObj->DeviceExtension;
  PDEVICE_OBJECT pLowerDevObj = pDevExt->LowerDeviceObject;

  PAGED_CODE();

  F_DBG("CanStartDevice entry \n");

  // Pass this down to the next device object
  KeInitializeEvent(&pDevExt->CanStartEvent, SynchronizationEvent, FALSE);

  IoCopyCurrentIrpStackLocationToNext(PIrp);
  IoSetCompletionRoutine(PIrp, CanSyncCompletion, &pDevExt->CanStartEvent, TRUE, TRUE, TRUE);
  status = IoCallDriver(pLowerDevObj, PIrp);

  // Wait for lower drivers to be done with the Irp
  if(status == STATUS_PENDING) 
  {
    KeWaitForSingleObject(&pDevExt->CanStartEvent, Executive, KernelMode, FALSE, NULL);
    status = PIrp->IoStatus.Status;
  }

  if(NT_SUCCESS(status))
  {
    // Do the specific items to start the device
    status = CanFinishStartDevice(pDevObj, 
      pIrpStack->Parameters.StartDevice.AllocatedResources, 
      pIrpStack->Parameters.StartDevice.AllocatedResourcesTranslated
      );
  }
  
  return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine will be used to create a symbolic link to the device
//
//Arguments:
//
//    Extension - Pointer to the device extension.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
NTSTATUS CanDoExternalNaming(IN PCAN_DEVICE_EXTENSION PDevExt, PCONFIG_DATA pConfig)
{
  NTSTATUS status = STATUS_SUCCESS;
  UNICODE_STRING linkName;
  UNICODE_STRING instanceStr;
  WCHAR instanceNumberBuffer[20];
  ULONG devNumber = CAN_UNINITIALIZED_DEFAULT;

  PAGED_CODE();

  do
  {
    RtlZeroMemory(&linkName, sizeof(UNICODE_STRING));
    linkName.MaximumLength = SYMBOLIC_NAME_LENGTH * sizeof(WCHAR);
    linkName.Buffer = ExAllocatePool(PagedPool, linkName.MaximumLength + sizeof(WCHAR));
    if(linkName.Buffer == NULL)
    {
      status = STATUS_INSUFFICIENT_RESOURCES;
      break;
    }
    RtlZeroMemory(linkName.Buffer, linkName.MaximumLength + sizeof(WCHAR));
    RtlAppendUnicodeToString(&linkName, SYMBOLIC_LINK_NAME_U);

    if(pConfig->AddressSpace == CM_RESOURCE_PORT_MEMORY)
    {
      ULONG FwBaseAddress[8] = {0xDF000, 0xDF200, 0xDE000, 0xDE200, 0xCF000, 0xCF200, 0xCE000, 0xCE200};
      ULONG i;
      for(i=0; i<8; i++)
      {
        if(pConfig->TrController.LowPart == FwBaseAddress[i])
        {
          devNumber = i + 1;
          break;
        }
      }
    }
    if(devNumber == CAN_UNINITIALIZED_DEFAULT)
    {
      status = STATUS_NO_SUCH_DEVICE;
      break;
    }

    RtlInitUnicodeString(&instanceStr, NULL);
    instanceStr.MaximumLength = sizeof(instanceNumberBuffer);
    instanceStr.Buffer = instanceNumberBuffer;
    RtlIntegerToUnicodeString(devNumber, 10, &instanceStr);
    RtlAppendUnicodeStringToString(&linkName, &instanceStr);

    PDevExt->SymbolicLinkName.MaximumLength = linkName.Length + sizeof(WCHAR);
    PDevExt->SymbolicLinkName.Buffer = ExAllocatePool(PagedPool | POOL_COLD_ALLOCATION, PDevExt->SymbolicLinkName.MaximumLength);
    if(PDevExt->SymbolicLinkName.Buffer == NULL)
    {
      status = STATUS_INSUFFICIENT_RESOURCES;
      break;
    }
    RtlZeroMemory(PDevExt->SymbolicLinkName.Buffer, PDevExt->SymbolicLinkName.MaximumLength);
    RtlAppendUnicodeStringToString(&PDevExt->SymbolicLinkName, &linkName);

    status = IoCreateSymbolicLink(&PDevExt->SymbolicLinkName, &PDevExt->DeviceName);
    if(!NT_SUCCESS(status))
    {
      // Oh well, couldn't create the symbolic link.  No point
      // in trying to create the device map entry.
      break;
   }

   PDevExt->CreatedSymbolicLink = TRUE;

   // TODO:
  }
  while(0);

  // Clean up error conditions
  if(!NT_SUCCESS(status))
  {
    if(PDevExt->CreatedSymbolicLink ==  TRUE)
    {
      IoDeleteSymbolicLink(&PDevExt->SymbolicLinkName);
      PDevExt->CreatedSymbolicLink = FALSE;
    }

    if(PDevExt->SymbolicLinkName.Buffer != NULL)
    {
      ExFreePool(PDevExt->SymbolicLinkName.Buffer);
      PDevExt->SymbolicLinkName.Buffer = NULL;
    }
  }

  // Always clean up our temp buffers.
  if(linkName.Buffer != NULL)
    ExFreePool(linkName.Buffer);

   return status;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine will be used to delete a symbolic link
//    to the driver name
//
//Arguments:
//
//    Extension - Pointer to the device extension.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanUndoExternalNaming(IN PCAN_DEVICE_EXTENSION PDevExt)
{
  PAGED_CODE();

  // We're cleaning up here.  One reason we're cleaning up
  // is that we couldn't allocate space for the directory
  // name or the symbolic link.
  if(PDevExt->CreatedSymbolicLink ==  TRUE)
  {
    IoDeleteSymbolicLink(&PDevExt->SymbolicLinkName);
    PDevExt->CreatedSymbolicLink = FALSE;
  }

  if(PDevExt->SymbolicLinkName.Buffer != NULL)
  {
    ExFreePool(PDevExt->SymbolicLinkName.Buffer);
    PDevExt->SymbolicLinkName.Buffer = NULL;
  }
}
//-----------------------------------------------------------------------------
#endif