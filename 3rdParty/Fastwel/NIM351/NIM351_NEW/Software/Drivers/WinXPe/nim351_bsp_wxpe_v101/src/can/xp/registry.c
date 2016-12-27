#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

#include <can/xp/CanDrv.h>

#ifdef ALLOC_PRAGMA
//#pragma alloc_text(INIT,CanGetConfigDefaults)
#pragma alloc_text(PAGE,CanGetConfigDefaults)
#pragma alloc_text(PAGE,CanGetRegistryKeyValue)
//#pragma alloc_text(PAGESRP0,CanPutRegistryKeyValue)
#endif // ALLOC_PRAGMA

//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine reads the default configuration data from the
//    registry for the can driver.
//
//    It also builds fields in the registry for several configuration
//    options if they don't exist.
//
//Arguments:
//
//    DriverDefaultsPtr - Pointer to a structure that will contain
//                        the default configuration values.
//
//    RegistryPath - points to the entry for this driver in the
//                   current control set of the registry.
//
//Return Value:
//
//    STATUS_SUCCESS if we got the defaults, otherwise we failed.
//    The only way to fail this call is if the  STATUS_INSUFFICIENT_RESOURCES.
//-----------------------------------------------------------------------------
NTSTATUS CanGetConfigDefaults(IN PCAN_FIRMWARE_DATA DriverDefaultsPtr, IN PUNICODE_STRING RegistryPath)
{
  NTSTATUS Status = STATUS_SUCCESS;
  // We use this to query into the registry for defaults
  RTL_QUERY_REGISTRY_TABLE paramTable[4];
  PWCHAR  path;
  ULONG   zero = 0;
  ULONG   notThereDefault = CAN_UNINITIALIZED_DEFAULT;

  PAGED_CODE();

  // Since the registry path parameter is a "counted" UNICODE string, it
  // might not be zero terminated.  For a very short time allocate memory
  // to hold the registry path zero terminated so that we can use it to
  // delve into the registry.
  //
  // NOTE NOTE!!!! This is not an architected way of breaking into
  // a driver.  It happens to work for this driver because the author
  // likes to do things this way.
  //

  path = ExAllocatePool(PagedPool, RegistryPath->Length+sizeof(WCHAR));
  if(!path)
  {
    Status = STATUS_INSUFFICIENT_RESOURCES;
    return (Status);
  }

  RtlZeroMemory(DriverDefaultsPtr, sizeof(CAN_FIRMWARE_DATA));
  RtlZeroMemory(&paramTable[0], sizeof(paramTable));
  RtlZeroMemory(path, RegistryPath->Length+sizeof(WCHAR));
  RtlMoveMemory(path, RegistryPath->Buffer, RegistryPath->Length);

  paramTable[0].Flags         = RTL_QUERY_REGISTRY_DIRECT;
  paramTable[0].Name          = L"RxFIFO";
  paramTable[0].EntryContext  = &DriverDefaultsPtr->RxFIFODefault;
  paramTable[0].DefaultType   = REG_DWORD;
  paramTable[0].DefaultData   = &notThereDefault;
  paramTable[0].DefaultLength = sizeof(ULONG);

  paramTable[1].Flags         = RTL_QUERY_REGISTRY_DIRECT;
  paramTable[1].Name          = L"TxFIFO";
  paramTable[1].EntryContext  = &DriverDefaultsPtr->TxFIFODefault;
  paramTable[1].DefaultType   = REG_DWORD;
  paramTable[1].DefaultData   = &notThereDefault;
  paramTable[1].DefaultLength = sizeof(ULONG);

  Status = RtlQueryRegistryValues(RTL_REGISTRY_ABSOLUTE | RTL_REGISTRY_OPTIONAL, path, &paramTable[0], NULL, NULL);

  // Check to see if there was a rxfifo or a txfifo size.
  // If there isn't then write out values so that they could be adjusted later.
  if(DriverDefaultsPtr->RxFIFODefault==0 || DriverDefaultsPtr->RxFIFODefault==notThereDefault)
  {
    DriverDefaultsPtr->RxFIFODefault = CAN_RX_FIFO_DEFAULT;
    RtlWriteRegistryValue(
      RTL_REGISTRY_ABSOLUTE,
      path,
      L"RxFIFO",
      REG_DWORD,
      &DriverDefaultsPtr->RxFIFODefault,
      sizeof(ULONG)
      );
  }

  if(DriverDefaultsPtr->TxFIFODefault == notThereDefault)
  {
    DriverDefaultsPtr->TxFIFODefault = CAN_TX_FIFO_DEFAULT;
    RtlWriteRegistryValue(
      RTL_REGISTRY_ABSOLUTE,
      path,
      L"TxFIFO",
      REG_DWORD,
      &DriverDefaultsPtr->TxFIFODefault,
      sizeof(ULONG)
      );
  }

  // TODO: нужно ли хранить в реестре?
  DriverDefaultsPtr->ControllerSettingsDefault.controller_type = PHILIPS_SJA_1000;
  DriverDefaultsPtr->ControllerSettingsDefault.baud_rate = CAN_BAUDRATE_DEFAULT;
  DriverDefaultsPtr->ControllerSettingsDefault.opmode = 
#ifdef CAN_SELF_TEST_MODE
    CAN_OPMODE_SELFTEST | CAN_OPMODE_SELFRECV |
#endif
    CAN_OPMODE_STANDARD | CAN_OPMODE_EXTENDED;

  // Accept all messages
  DriverDefaultsPtr->ControllerSettingsDefault.acceptance_code = 0x00000000UL;
  DriverDefaultsPtr->ControllerSettingsDefault.acceptance_mask = 0x00000000UL;//0xFFFFFFFFUL;

  // Accept Errors: controller problems | bus off
  DriverDefaultsPtr->ControllerSettingsDefault.error_mask = CAN_ERR_CRTL|CAN_ERR_BUSOFF;

  // We don't need that path anymore.
  if(path)
  {
    ExFreePool(path);
  }

  return (Status);
}

//-----------------------------------------------------------------------------
//Routine Description:
//
//    Reads a registry key value from an already opened registry key.
//
//Arguments:
//
//    Handle              Handle to the opened registry key
//
//    KeyNameString       ANSI string to the desired key
//
//    KeyNameStringLength Length of the KeyNameString
//
//    Data                Buffer to place the key value in
//
//    DataLength          Length of the data buffer
//
//Return Value:
//
//    STATUS_SUCCESS if all works, otherwise status of system call that went wrong.
//-----------------------------------------------------------------------------
NTSTATUS CanGetRegistryKeyValue(IN HANDLE Handle, IN PWCHAR KeyNameString, IN ULONG KeyNameStringLength, IN PVOID Data, IN ULONG DataLength)
{
  UNICODE_STRING              keyName;
  ULONG                       length;
  PKEY_VALUE_FULL_INFORMATION fullInfo;

  NTSTATUS                    ntStatus = STATUS_INSUFFICIENT_RESOURCES;

  PAGED_CODE();

  RtlInitUnicodeString (&keyName, KeyNameString);

  length = sizeof(KEY_VALUE_FULL_INFORMATION) + KeyNameStringLength + DataLength;
  fullInfo = ExAllocatePool(PagedPool, length);

  if(fullInfo)
  {
    ntStatus = ZwQueryValueKey(Handle, &keyName, KeyValueFullInformation, fullInfo, length, &length);
    if(NT_SUCCESS(ntStatus))
    {
      // If there is enough room in the data buffer, copy the output
      if(DataLength >= fullInfo->DataLength)
      {
        RtlCopyMemory (Data, ((PUCHAR) fullInfo) + fullInfo->DataOffset, fullInfo->DataLength);
      }
    }

    ExFreePool(fullInfo);
  }

  return ntStatus;
}
//-----------------------------------------------------------------------------

#endif