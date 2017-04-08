#ifndef __CAN_DRV_H__
#define __CAN_DRV_H__

#include <can/canhw.h>
#include <can/SJA1000.h>
#include <can/xp/canlog.h>

//-----------------------------------------------------------------------------
// Some default driver values.  We will check the registry for them first.
#define CAN_UNINITIALIZED_DEFAULT       1234567
#define CAN_RX_FIFO_DEFAULT             32
#define CAN_TX_FIFO_DEFAULT             8
#define CAN_BAUDRATE_DEFAULT            CANBR_250kBaud


#define DEVICE_OBJECT_NAME_LENGTH       128
#define SYMBOLIC_NAME_LENGTH            128
#define DEVICE_NAME_U	L"\\Device\\nim351_"
#define SYMBOLIC_LINK_NAME_U	L"\\??\\nim351_"

//#define FWCAN_USE_SWITCH_DEVICE 

//-----------------------------------------------------------------------------
typedef struct _CAN_GLOBALS 
{
  LIST_ENTRY AllDevObjs;
  PVOID PAGEFCAN_Handle;
  UNICODE_STRING RegistryPath;
  KSPIN_LOCK GlobalsSpinLock;
#if DBG
  ULONG PAGEFCAN_Count;
#endif // DBG
} CAN_GLOBALS, *PCAN_GLOBALS;
extern CAN_GLOBALS CanGlobals;
//-----------------------------------------------------------------------------
// This structure contains configuration data, 
// much of which is read from the registry.
typedef struct _CAN_FIRMWARE_DATA
{
  ULONG           RxFIFODefault;
  ULONG           TxFIFODefault;
  F_CAN_SETTINGS  ControllerSettingsDefault;

} CAN_FIRMWARE_DATA, *PCAN_FIRMWARE_DATA;
//-----------------------------------------------------------------------------
typedef struct _CAN_USER_DATA
{
   PHYSICAL_ADDRESS UserPort;
   ULONG UserVector;
   UNICODE_STRING UserSymbolicLink;
   ULONG UserPortIndex;
   ULONG UserBaudRate;
   ULONG UserIndexed;
   ULONG UserInterruptMode;
   ULONG UserAddressSpace;
   ULONG UserLevel;
   ULONG DisablePort;
   ULONG RxFIFO;
   ULONG RxFIFODefault;
   ULONG TxFIFO;
   ULONG TxFIFODefault;
} CAN_USER_DATA, *PCAN_USER_DATA;
//-----------------------------------------------------------------------------
#if DBG
#define CanLockPagableSectionByHandle(_secHandle) \
{ \
    MmLockPagableSectionByHandle((_secHandle)); \
    InterlockedIncrement(&CanGlobals.PAGEFCAN_Count); \
}

#define CanUnlockPagableImageSection(_secHandle) \
{ \
   InterlockedDecrement(&CanGlobals.PAGEFCAN_Count); \
   MmUnlockPagableImageSection(_secHandle); \
}


#define CAN_LOCKED_PAGED_CODE() \
    if ((KeGetCurrentIrql() > APC_LEVEL)  \
    && (CanGlobals.PAGEFCAN_Count == 0)) { \
    KdPrint(("CAN: Pageable code called at IRQL %d without lock \n", \
             KeGetCurrentIrql())); \
        ASSERT(FALSE); \
        }

#else
#define CanLockPagableSectionByHandle(_secHandle) \
{ \
    MmLockPagableSectionByHandle((_secHandle)); \
}

#define CanUnlockPagableImageSection(_secHandle) \
{ \
   MmUnlockPagableImageSection(_secHandle); \
}

#define CAN_LOCKED_PAGED_CODE()
#endif // DBG
//-----------------------------------------------------------------------------
typedef struct _CAN_DEVICE_STATE 
{
  // TRUE if we need to set the state to open
  BOOLEAN Reopen;
} CAN_DEVICE_STATE, *PCAN_DEVICE_STATE;
//-----------------------------------------------------------------------------
typedef struct _PORT_AREA
{
  ULONG addressSpace;
  PHYSICAL_ADDRESS start;
  ULONG length;
} PORT_AREA, *PPORT_AREA;
//-----------------------------------------------------------------------------
typedef struct _CONFIG_DATA 
{
  PHYSICAL_ADDRESS    Controller;
  PHYSICAL_ADDRESS    TrController;
  ULONG               SpanOfController;
#ifdef FWCAN_USE_SWITCH_DEVICE
  PORT_AREA           switchPort;
#endif
  ULONG               PortIndex;
  F_CAN_SETTINGS      ControllerSettings;
  ULONG               BusNumber;
  ULONG               AddressSpace;
  ULONG               DisablePort;
  ULONG               ForceFifoEnable;
  ULONG               RxFIFO;
  ULONG               TxFIFO;
  ULONG               LogFifo;
  ULONG               MaskInverted;
  KINTERRUPT_MODE     InterruptMode;
  INTERFACE_TYPE      InterfaceType;
  ULONG               OriginalVector;
  ULONG               OriginalIrql;
  ULONG               TrVector;
  ULONG               TrIrql;
  KAFFINITY           Affinity;
} CONFIG_DATA,*PCONFIG_DATA;
//-----------------------------------------------------------------------------
typedef enum _MEM_COMPARES
{
  AddressesAreEqual,
  AddressesOverlap,
  AddressesAreDisjoint
} MEM_COMPARES, *PMEM_COMPARES;
//-----------------------------------------------------------------------------
// ISR switch structure
typedef struct _CAN_CISR_SW 
{
  BOOLEAN (*IsrFunc)(PKINTERRUPT, PVOID);
  PVOID Context;
  LIST_ENTRY SharerList;
} CAN_CISR_SW, *PCAN_CISR_SW;
//-----------------------------------------------------------------------------
typedef struct _CAN_DEVICE_EXTENSION
{
  // We keep a pointer around to our device name for dumps
  // and for creating "external" symbolic links to this device.
  UNICODE_STRING DeviceName;

  // This points to the symbolic link name that will be
  // linked to the actual nt device name.
  UNICODE_STRING SymbolicLinkName;

  // ISR switch structure
  PCAN_CISR_SW CIsrSw;

  // This holds the isr that should be called from our own
  // dispatching isr for "cards" that are trying to share the same interrupt.
  PKSERVICE_ROUTINE TopLevelOurIsr;

  // This holds the context that should be used when we
  // call the above service routine.
  PVOID TopLevelOurIsrContext;

  // This links together all of the different "cards" that are
  // trying to share the same interrupt.
  LIST_ENTRY TopLevelSharers;

  // Needed to add new devices to sharer list
  PVOID NewExtension;

  // Says whether this device can share interrupts
  BOOLEAN InterruptShareable;

  // This circular doubly linked list links together all
  // devices that are using the same interrupt object.
  // NOTE: This does not mean that they are using the
  // same interrupt "dispatching" routine.
  LIST_ENTRY CommonInterruptObject;

  // For reporting resource usage, we keep around the physical
  // address we got from the registry.
  PHYSICAL_ADDRESS OriginalController;

  // For reporting resource usage, we keep around the physical
  // address we got from the registry.
  PHYSICAL_ADDRESS OriginalInterruptStatus;

  // After initialization of the driver is complete, this
  // will either be NULL or point to the routine that the
  // kernel will call when an interrupt occurs.
  //
  // If the pointer is null then this is part of a list
  // of ports that are sharing an interrupt and this isn't
  // the first port that we configured for this interrupt.
  //
  // If the pointer is non-null then this routine has some
  // kind of structure that will "eventually" get us into
  // the real isr with a pointer to this device extension.
  //
  PKSERVICE_ROUTINE OurIsr;

  // This will generally point right to this device extension.
  //
  // However, when the port that this device extension is
  // "managing" was the first port initialized on a chain
  // of ports that were trying to share an interrupt, this
  // will point to a structure that will enable dispatching
  // to any port on the chain of sharers of this interrupt.
  //
  PVOID OurIsrContext;

  // Points to the interrupt object for used by this device.
  PKINTERRUPT Interrupt;

  // We keep the following values around so that we can connect to the 
  // interrupt and report resources after the configuration record is gone.
  ULONG Vector;                 // Translated vector
  KIRQL Irql;                   // Translated Irql
  ULONG OriginalVector;         // Untranslated vector
  ULONG OriginalIrql;           // Untranslated irql
  ULONG AddressSpace;           // Address space
  ULONG BusNumber;              // Bus number
  INTERFACE_TYPE InterfaceType; // Interface type
  // Set at intialization to indicate that on the current architecture we need
  // to unmap the base register address when we unload the driver.
  BOOLEAN UnMapRegisters;

#ifdef FWCAN_USE_SWITCH_DEVICE
  PUCHAR switchPortBase;
  ULONG switchPortLength;
  BOOLEAN UnMapSwitchRegisters;
#endif

  // Pointer to the driver object
  PDRIVER_OBJECT  DriverObject;
  // Pointer to the device object
  PDEVICE_OBJECT  DeviceObject;
  // This is where keep track of the power state the device is in.
  DEVICE_POWER_STATE PowerState;
  
  // This links together all devobjs that this driver owns.
  // It is needed to search when starting a new device.
  LIST_ENTRY AllDevObjs;

  PDEVICE_OBJECT  LowerDeviceObject;
  PDEVICE_OBJECT  PhysicalDeviceObject;

  // Count of pending IRP's
  ULONG           PendingIRPCnt;

  // Accepting requests?
  ULONG           DevicePNPAccept;

  // PNP State
  ULONG PNPState;

  // Misc Flags
  ULONG Flags;

  // Pending DPC count
  ULONG           DpcCount;

  // This points to a DPC used to complete write requests.
  KDPC CompleteWriteDpc;
  // This points to a DPC used to complete read requests.
  KDPC CompleteReadDpc;
  // This dpc is fired off if the timer for the total timeout
  // for the read expires.  It will execute a dpc routine that
  // will cause the current read to complete.
  KDPC TotalReadTimeoutDpc;
  // This dpc is fired off if the timer for the total timeout
  // for the write expires.  It will execute a dpc routine that
  // will cause the current write to complete.
  KDPC TotalWriteTimeoutDpc;
  // This dpc is fired off if a can bus-off occurs.  It will
  // execute a dpc routine that will cancel all pending reads and writes. 
  // If enabled, a timer is started to trigger bus-off recovery.
  KDPC CanRecoverDpc;
  // This dpc is fired off if an event occurs and there was
  // a irp waiting on that event.  A dpc routine will execute
  // that completes the irp.
  KDPC CanWaitDpc;
  // This DPC is fired to set an event stating that all other
  // DPC's have been finish for this device extension so that
  // paged code may be unlocked.
  KDPC IsrUnlockPagesDpc;

  // We keep track of whether the somebody has the device currently
  // opened with a simple boolean.  We need to know this so that
  // spurious interrupts from the device (especially during initialization)
  // will be ignored.  This value is only accessed in the ISR and
  // is only set via synchronization routines.  We may be able
  // to get rid of this boolean when the code is more fleshed out.
  BOOLEAN DeviceIsOpened;
  LARGE_INTEGER DeviceOpenTime;

  // Records whether we actually created the symbolic link name
  // at driver load time.  If we didn't create it, we won't try
  // to destroy it when we unload.
  BOOLEAN CreatedSymbolicLink;

  // The number of msgs to push out.
  ULONG TxFifoAmount;

  // This list head is used to contain the time ordered list
  // of read requests.  Access to this list is protected by
  // the global cancel spinlock.
  LIST_ENTRY ReadQueue;

  // This list head is used to contain the time ordered list
  // of write requests.  Access to this list is protected by
  // the global cancel spinlock.
  LIST_ENTRY WriteQueue;

  // Holds the serialized list of purge requests.
  LIST_ENTRY PurgeQueue;

  // This list head is used to contain the list of "wait on mask" requests.
  LIST_ENTRY WaitQueue;

  // List of stalled IRP's
  LIST_ENTRY StalledIrpQueue;

  // This points to the irp that is currently being processed
  // for the read queue. This field is initialized by the open to NULL.
  // This value is only set at dispatch level.  It may be read at
  // interrupt level.
  PIRP CurrentReadIrp;

  // This points to the irp that is currently being processed
  // for the write queue. This value is only set at dispatch level.
  // It may be read at interrupt level.
  PIRP CurrentWriteIrp;

  // Points to the irp that is currently being processed to
  // purge the read/write queues and buffers.
  PIRP CurrentPurgeIrp;

  // This points to the irp that is currently being processed
  // for the wait queue.
  //PIRP CurrentWaitIrp;

  // Mutex on open status
  FAST_MUTEX OpenMutex;
  // Mutex on close
  FAST_MUTEX CloseMutex;

  // Open count
  LONG OpenCount;

  // This lock will be used to protect various fields in
  // the extension that are set (& read) in the extension
  // by the io controls.
  KSPIN_LOCK ControlLock;

  // This lock will be used to protect the accept / reject state
  // transitions and flags of the driver  It must be acquired
  // before a cancel lock
  KSPIN_LOCK FlagsLock;

  // No IRP's pending event
  KEVENT PendingIRPEvent;

  // Pending DPC event
  KEVENT PendingDpcEvent;

  // Current state during powerdown
  CAN_DEVICE_STATE DeviceState;

  // Device stack capabilites
  DEVICE_POWER_STATE DeviceStateMap[PowerSystemMaximum];

  // SystemWake from devcaps
  SYSTEM_POWER_STATE SystemWake;

  // DeviceWake from devcaps
  DEVICE_POWER_STATE DeviceWake;
  
  // Event to signal transition to D0 completion
  KEVENT PowerD0Event;
  // Start sync event
  KEVENT CanStartEvent;

  // This timer is used to invoke a dpc after the timer is set.
  // That dpc will be used to reset controller if bus-off occurs.
  KTIMER RestartControllerTimer;
  // Holds the number of ms for automatic bus-off recovery.
  ULONG RestartMs;
  // This dpc is fired off when a timer expires, so that code can be invoked 
  // that will restart hardware.
  KDPC RestartControllerDpc;
  // This ulong is incremented each time something trys to start
  // the execution path that tries to restart hardware. 
  // If it "bumps" into another path
  // (indicated by a false return value from queueing a dpc
  // and a TRUE return value tring to start a timer) it will
  // decrement the count.  These increments and decrements
  // are all done at device level.  Note that in the case
  // of a bump while trying to start the timer, we have to
  // go up to device level to do the decrement.
  ULONG CountOfTryingToRestartController;

  // Holds the timeout controls for the device.  This value
  // is set by the Ioctl processing.
  // It should only be accessed under protection of the control
  // lock since more than one request can be in the control dispatch
  // routine at one time.
  F_CAN_TIMEOUTS Timeouts;

  // This is the kernal timer structure used to handle
  // total read request timing.
  KTIMER ReadRequestTotalTimer;
  // This is the kernal timer structure used to handle
  // total time request timing.
  KTIMER WriteRequestTotalTimer;

  // This is the kernal timer structure used to handle
  // "wait on mask" total time request timing.
  KTIMER WaitOnMaskTimer;
  //LARGE_INTEGER WaitOnMaskEnd;
  F_CAN_STATUS_T  WaitStatus;

  // This is a buffer for the read processing. The buffer works as a ring.
  // When the frame is read from the device it will be place at the end
  // of the ring. Frames are only placed in this buffer at interrupt level
  // although frames may be read at any level. The pointers that manage
  // this buffer may not be updated except at interrupt level.
  PF_CAN_RX InterruptReadBuffer;

  F_CAN_MSG PostedMsg;

  // This is the size of the interrupt buffer for the read processing.
  ULONG szInterruptReadBuffer;

  // This is the *write position* in the in the interrupt read buffer.
  // This value is only *incremented* at interrupt level
  // so it is safe to read it at any level.
  ULONG wrInterruptReadBuffer;

  // This is the *read position* in the in the interrupt buffer.
  // When frames are copied out of the read buffer, this count
  // is incremented by a routine that synchronizes with the ISR.
  ULONG rdInterruptReadBuffer;

  // This is a pointer to the first frame of the buffer into
  // which the interrupt service routine is copying frames.
  PF_CAN_RX ReadBufferBase;

  // This keeps a total of the number of frames that
  // are in all of the "write" irps that the driver knows about. 
  // It is only accessed with the cancel spinlock held.
  ULONG TotalFramesQueued;

  // Holds the number of frames remaining in the current write irp.
  // This location is only accessed while at interrupt level.
  ULONG WriteLength;

  // Holds a pointer to the current frame to be sent in the current write.
  // This location is only accessed while at interrupt level.
  PF_CAN_MSG WriteCurrentFrame;

  // This mask holds all of the reason that transmission is not proceeding.
  // Normal transmission can not occur if this is non-zero.
  // This is only written from interrupt level.
  // This could be (but is not) read at any level.
  ULONG TXHolding;

  // This is a count of the number of frames read by the
  // isr routine.  It is *ONLY* written at isr level.  We can
  // read it at dispatch level.
  ULONG ReadByIsr;

  //// The base address for the set of device registers of the can port.
  //PUCHAR Controller;
  F_CAN_DEVICE Controller;
  // This value holds the span (in units of bytes) of the register
  // set controlling this port.  This is constant over the life of the port.
  ULONG SpanOfController;

  // CAN resources
  PCONFIG_DATA pConfig;

  //F_CAN_IO_STATS can_io_stats;
} CAN_DEVICE_EXTENSION, *PCAN_DEVICE_EXTENSION;
//-----------------------------------------------------------------------------

#define CAN_PNPACCEPT_OK                 0x0L
#define CAN_PNPACCEPT_REMOVING           0x1L
#define CAN_PNPACCEPT_STOPPING           0x2L
#define CAN_PNPACCEPT_STOPPED            0x4L
#define CAN_PNPACCEPT_SURPRISE_REMOVING  0x8L
#define CAN_PNPACCEPT_POWER_DOWN         0X10L

#define CAN_PNP_ADDED                    0x0L
#define CAN_PNP_STARTED                  0x1L
#define CAN_PNP_QSTOP                    0x2L
#define CAN_PNP_STOPPING                 0x3L
#define CAN_PNP_QREMOVE                  0x4L
#define CAN_PNP_REMOVING                 0x5L
#define CAN_PNP_RESTARTING               0x6L

#define CAN_FLAGS_CLEAR                  0x0L
#define CAN_FLAGS_STARTED                0x1L
#define CAN_FLAGS_STOPPED                0x2L
#define CAN_FLAGS_BROKENHW               0x4L
#define CAN_FLAGS_LEGACY_ENUMED          0x8L
//-----------------------------------------------------------------------------

typedef struct _CAN_UPDATE_INPUT
{
  PCAN_DEVICE_EXTENSION pExtension;
  ULONG framesCopied;
} CAN_UPDATE_INPUT,*PCAN_UPDATE_INPUT;

// The following simple structure is used to send a pointer
// the device extension and an ioctl specific pointer to data.
typedef struct _CAN_IOCTL_SYNC
{
  PCAN_DEVICE_EXTENSION pExtension;
  PVOID pData;
  NTSTATUS status;
} CAN_IOCTL_SYNC,*PCAN_IOCTL_SYNC;

typedef NTSTATUS (*PCAN_START_ROUTINE)(IN PCAN_DEVICE_EXTENSION);
typedef VOID (*PCAN_FINISH_ROUTINE)(IN PCAN_DEVICE_EXTENSION);

typedef VOID (*PCAN_GET_NEXT_ROUTINE)(
  IN PIRP *CurrentOpIrp, 
  IN PLIST_ENTRY QueueToProcess, 
  OUT PIRP *NewIrp, 
  IN BOOLEAN CompleteCurrent,
  PCAN_DEVICE_EXTENSION Extension);

extern CAN_FIRMWARE_DATA  driverDefaults;

VOID      CanUnload (IN PDRIVER_OBJECT pDriverObject);
NTSTATUS  CanAddDevice( IN PDRIVER_OBJECT DriverObject, IN PDEVICE_OBJECT PhysicalDeviceObject);
NTSTATUS  CanCreateOpen(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
NTSTATUS  CanClose(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
NTSTATUS  CanDispatchDeviceControl(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
NTSTATUS  CanDispatchPnp( IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
NTSTATUS  CanPowerDispatch ( IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp );
NTSTATUS  CanStartDevice (IN PDEVICE_OBJECT DeviceObject,IN PIRP Irp);
NTSTATUS  CanGetPortInfo(IN PDEVICE_OBJECT PDevObj, IN PCM_RESOURCE_LIST PResList, IN PCM_RESOURCE_LIST PTrResList, OUT PCONFIG_DATA PConfig);
NTSTATUS  CanFinishStartDevice(IN PDEVICE_OBJECT PDevObj, IN PCM_RESOURCE_LIST PResList, IN PCM_RESOURCE_LIST PTrResList);
NTSTATUS  CanFinishRestartDevice(IN PDEVICE_OBJECT PDevObj);
NTSTATUS  CanIRPPrologue(IN PIRP PIrp, IN PCAN_DEVICE_EXTENSION PDevExt);
VOID      CanIRPEpilogue(IN PCAN_DEVICE_EXTENSION PDevExt);
NTSTATUS  CanIoCallDriver(PCAN_DEVICE_EXTENSION PDevExt, PDEVICE_OBJECT PDevObj, PIRP PIrp);
VOID      CanSetDeviceFlags(IN PCAN_DEVICE_EXTENSION PDevExt, OUT PULONG PFlags, IN ULONG Value, IN BOOLEAN Set);
BOOLEAN   CanInsertQueueDpc(IN PRKDPC PDpc, IN PVOID Sarg1, IN PVOID Sarg2, IN PCAN_DEVICE_EXTENSION PDevExt);
BOOLEAN   CanSetTimer(IN PKTIMER Timer, IN LARGE_INTEGER DueTime, IN PKDPC Dpc OPTIONAL, IN PCAN_DEVICE_EXTENSION PDevExt);
VOID      CanDpcEpilogue(IN PCAN_DEVICE_EXTENSION PDevExt, PKDPC PDpc);
NTSTATUS  CanGetRegistryKeyValue(IN HANDLE Handle, IN PWCHAR KeyNameString, IN ULONG KeyNameStringLength, IN PVOID Data, IN ULONG DataLength);
NTSTATUS  CanGetConfigDefaults(IN PCAN_FIRMWARE_DATA DriverDefaultsPtr, IN PUNICODE_STRING RegistryPath);
NTSTATUS  CanFindInitController(IN PDEVICE_OBJECT PDevObj, IN PCONFIG_DATA PConfig);
BOOLEAN   CanSetResetMode(IN PVOID Context);
BOOLEAN   CanRestart(IN PVOID Context);
F_CAN_STATE CanGetControllerStatus(IN PCAN_DEVICE_EXTENSION pDevExt);
NTSTATUS  CanCreateDevObj(IN PDRIVER_OBJECT DriverObject, OUT PDEVICE_OBJECT *NewDeviceObject);
NTSTATUS  CanDoExternalNaming(IN PCAN_DEVICE_EXTENSION PDevExt, PCONFIG_DATA pConfig);
VOID      CanUndoExternalNaming(IN PCAN_DEVICE_EXTENSION PDevExt);
NTSTATUS  CanRemoveDevObj(IN PDEVICE_OBJECT PDevObj);
NTSTATUS  CanCompleteIfError(PDEVICE_OBJECT DeviceObject, PIRP Irp);
NTSTATUS  CanStartOrQueue(
                          IN PCAN_DEVICE_EXTENSION Extension,
                          IN PIRP Irp,
                          IN PLIST_ENTRY QueueToExamine,
                          IN PIRP *CurrentOpIrp,
                          IN PCAN_START_ROUTINE Starter
                          );
VOID CanTryToCompleteCurrent(
                             IN PCAN_DEVICE_EXTENSION Extension,
                             IN PKSYNCHRONIZE_ROUTINE SynchRoutine OPTIONAL,
                             IN KIRQL IrqlForRelease,
                             IN NTSTATUS StatusToUse,
                             IN PIRP *CurrentOpIrp,
                             IN PLIST_ENTRY QueueToProcess OPTIONAL,
                             IN PKTIMER TotalTimer OPTIONAL,
                             IN PCAN_START_ROUTINE Starter OPTIONAL,
                             IN PCAN_GET_NEXT_ROUTINE GetNextIrp OPTIONAL,
                             IN LONG RefType,
                             IN PCAN_FINISH_ROUTINE Finisher OPTIONAL);
VOID CanGetNextIrp(
                   IN PIRP *CurrentOpIrp,
                   IN PLIST_ENTRY QueueToProcess,
                   OUT PIRP *NextIrp,
                   IN BOOLEAN CompleteCurrent,
                   IN PCAN_DEVICE_EXTENSION extension
                   );


BOOLEAN   CanISR(IN PKINTERRUPT InterruptObject, IN PVOID Context);
BOOLEAN   CanSharerIsr(IN PKINTERRUPT InterruptObject, IN PVOID Context);
BOOLEAN   CanCIsrSw(IN PKINTERRUPT InterruptObject, IN PVOID Context);
BOOLEAN   CanClearStats(IN PVOID Context);
BOOLEAN   CanSignalEvent(IN PVOID Context);

NTSTATUS  CanWrite(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
NTSTATUS  CanRead(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
NTSTATUS  CanIoControl(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp);
VOID      CanWriteTimeout(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanCompleteWrite(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanCompleteRead(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanReadTimeout(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanWriteTimeout(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanRecover(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanCompleteWait(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanUnlockPages(IN PKDPC PDpc, IN PVOID PDeferredContext, IN PVOID PSysContext1, IN PVOID PSysContext2);
VOID      CanStartTimerRestartController(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanInvokeRestartController(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2);
VOID      CanPutRx(IN PCAN_DEVICE_EXTENSION Extension, IN PF_CAN_RX pcf);

VOID      CanReleaseResources(IN PCAN_DEVICE_EXTENSION PDevExt);
VOID      CanKillPendingIrps(PDEVICE_OBJECT PDevObj);
VOID      CanKillAllReadsOrWrites(IN PDEVICE_OBJECT DeviceObject, IN PLIST_ENTRY QueueToClean, IN PIRP *CurrentOpIrp);
VOID      CanKillAllStalled(IN PDEVICE_OBJECT PDevObj);
VOID      CanCancelQueued(PDEVICE_OBJECT DeviceObject, PIRP Irp);
VOID      CanStartQueuedWrite(IN PCAN_DEVICE_EXTENSION pDevExt, IN KIRQL OldIrql);

BOOLEAN   CanCancelTimer(IN PKTIMER Timer, IN PCAN_DEVICE_EXTENSION PDevExt);
VOID      CanLogError(
                      IN PDRIVER_OBJECT DriverObject, 
                      IN PDEVICE_OBJECT DeviceObject OPTIONAL,
                      IN ULONG SequenceNumber, 
                      IN UCHAR MajorFunctionCode, 
                      IN UCHAR RetryCount, 
                      IN ULONG UniqueErrorValue,
                      IN NTSTATUS FinalStatus, 
                      IN NTSTATUS SpecificIOStatus,
                      IN ULONG LengthOfInsert1, 
                      IN PWCHAR Insert1, 
                      IN ULONG LengthOfInsert2, 
                      IN PWCHAR Insert2);

u32     CanGetTimeStamp(IN PCAN_DEVICE_EXTENSION pDevExt);
NTSTATUS CanPassIrpToLowerDriver(PCAN_DEVICE_EXTENSION pDevExt, IN PIRP Irp);
BOOLEAN CanMarkOpen(IN PVOID Context);
BOOLEAN CanStartController(IN PVOID Context);

#define CanCompleteRequest(PDevExt, PIrp, PriBoost) \
  { \
    IoCompleteRequest((PIrp), (PriBoost)); \
    CanIRPEpilogue((PDevExt)); \
  }

#define CanRemoveQueueDpc(_dpc, _pExt) \
{ \
  if(KeRemoveQueueDpc((_dpc))) { \
     InterlockedDecrement(&(_pExt)->DpcCount); \
  } \
}


#define CanSetFlags(PDevExt, Value) \
  CanSetDeviceFlags((PDevExt), &(PDevExt)->Flags, (Value), TRUE)
#define CanClearFlags(PDevExt, Value) \
  CanSetDeviceFlags((PDevExt), &(PDevExt)->Flags, (Value), FALSE)
#define CanSetAccept(PDevExt, Value) \
  CanSetDeviceFlags((PDevExt), &(PDevExt)->DevicePNPAccept, (Value), TRUE)
#define CanClearAccept(PDevExt, Value) \
  CanSetDeviceFlags((PDevExt), &(PDevExt)->DevicePNPAccept, (Value), FALSE)
//-----------------------------------------------------------------------------

// The following three macros are used to initialize, set
// and clear references in IRPs that are used by
// this driver.  The reference is stored in the fourth
// argument of the irp, which is never used by any operation
// accepted by this driver.
#define CAN_REF_ISR         (0x00000001)
#define CAN_REF_CANCEL      (0x00000002)
#define CAN_REF_TOTAL_TIMER (0x00000004)

#define CAN_INIT_REFERENCE(Irp) { \
    ASSERT(sizeof(ULONG_PTR) <= sizeof(PVOID)); \
    IoGetCurrentIrpStackLocation((Irp))->Parameters.Others.Argument4 = NULL; \
    }

#define CAN_SET_REFERENCE(Irp,RefType) \
   do { \
       LONG _refType = (RefType); \
       PULONG_PTR _arg4 = (PVOID)&IoGetCurrentIrpStackLocation((Irp))->Parameters.Others.Argument4; \
       ASSERT(!(*_arg4 & _refType)); \
       *_arg4 |= _refType; \
   } while (0)

#define CAN_CLEAR_REFERENCE(Irp,RefType) \
   do { \
       LONG _refType = (RefType); \
       PULONG_PTR _arg4 = (PVOID)&IoGetCurrentIrpStackLocation((Irp))->Parameters.Others.Argument4; \
       ASSERT(*_arg4 & _refType); \
       *_arg4 &= ~_refType; \
   } while (0)

#define CAN_REFERENCE_COUNT(Irp) \
    ((ULONG_PTR)((IoGetCurrentIrpStackLocation((Irp))->Parameters.Others.Argument4)))

#endif