#if (defined _WIN32_WINNT) && (defined __CAN_DRIVER__) && (defined KERNEL_DRIVER_CODE)

#include <eal/el.h>
#include <can/xp/CanDrv.h>


#ifdef ALLOC_PRAGMA
#pragma alloc_text(PAGEFCAN,CanPutRx)
#endif

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine, which only runs at device level, takes care of
//    placing a frame into the receive buffer.
//
//Arguments:
//
//    Extension - The device extension.
//
//Return Value:
//
//    None.
//-----------------------------------------------------------------------------
VOID CanPutRx(IN PCAN_DEVICE_EXTENSION Extension, IN PF_CAN_RX pcf)
{
  CAN_LOCKED_PAGED_CODE();

  F_DBG10("Rx: id(0x%08X),dlc(%d),[%02X %02X %02X %02X %02X %02X %02X %02X]\n",
    pcf->msg.can_id, pcf->msg.can_dlc, 
    pcf->msg.data[0], pcf->msg.data[1], pcf->msg.data[2], pcf->msg.data[3],
    pcf->msg.data[4], pcf->msg.data[5], pcf->msg.data[6], pcf->msg.data[7]);

  // Check to see if we are copying into the users buffer or into the interrupt buffer.
  // If we are copying into the interrupt buffer then we will need to check 
  // if we have enough room.

  if(Extension->ReadBufferBase != Extension->InterruptReadBuffer)
  {
    // We are in the user buffer. Place the frame into the buffer. 
    // The read is complete. (CanRead waits for first frame only!!!).
    *Extension->ReadBufferBase = *pcf;
    // Switch back to the interrupt buffer.
    Extension->ReadBufferBase = Extension->InterruptReadBuffer;
    // Send off a DPC to Complete the read.
    Extension->CurrentReadIrp->IoStatus.Information = sizeof(F_CAN_RX);
    CanInsertQueueDpc(&Extension->CompleteReadDpc, NULL, NULL, Extension);
  }
  else
  {
    // We are in the interrupt buffer.
    if((ULONG)(Extension->wrInterruptReadBuffer - Extension->rdInterruptReadBuffer)  < Extension->szInterruptReadBuffer)
    {
      Extension->InterruptReadBuffer[Extension->wrInterruptReadBuffer++ % Extension->szInterruptReadBuffer] = *pcf;
      F_CAN_SetStatus(Extension->Controller.status, CAN_STATUS_RXBUF);
    }
    else
    {
      // We have a new frame but no room for it.
      Extension->Controller.stats.rx_over_errors++;
      Extension->Controller.stats.rx_errors++;
      Extension->Controller.errors.data_overrun++;
      F_CAN_SetStatus(Extension->Controller.status, CAN_STATUS_ERR);
    }
  }
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This is the interrupt service routine for the can port driver.
//    It will determine whether the can port is the source of this
//    interrupt.  If it is, then this routine will do the minimum of
//    processing to quiet the interrupt.  It will store any information
//    necessary for later processing.
//
//Arguments:
//
//    InterruptObject - Points to the interrupt object declared for this
//    device.  We *do not* use this parameter.
//
//    Context - This is really a pointer to the device extension for this
//    device.
//
//Return Value:
//
//    This function will return TRUE if the port is the source
//    of this interrupt, FALSE otherwise.
//-----------------------------------------------------------------------------
BOOLEAN CanISR(IN PKINTERRUPT InterruptObject, IN PVOID Context)
{
  // Holds the information specific to handling this device.
  PCAN_DEVICE_EXTENSION Extension = Context;
  // Holds the pointer to can controller structure
  PF_CAN_DEVICE pController = &Extension->Controller;
  // Holds the contents of the interrupt identification record.
  UCHAR InterruptIdReg;
  // Holds the contents of the status record.
  UCHAR status;
  // Will hold whether we've serviced any interrupt causes in this routine.
  BOOLEAN ServicedAnInterrupt = FALSE;

  UNREFERENCED_PARAMETER(InterruptObject);

  // Make sure we have an interrupt pending.  If we do then
  // we need to make sure that the device is open.  If the
  // device isn't open or powered down then quiet the device.  Note that
  // if the device isn't opened when we enter this routine
  // it can't open while we're in it.

  InterruptIdReg = HW_CAN_READ_INTERRUPT_ID(pController);

  // Apply lock so if close happens concurrently we don't miss the DPC queueing
  InterlockedIncrement(&Extension->DpcCount);

  // Shared interrupts and IRQ off?
  if(InterruptIdReg == IRQ_OFF)
  {
    ServicedAnInterrupt = FALSE;
  }
  else if(!Extension->DeviceIsOpened /*|| (Extension->PowerState != PowerDeviceD0)*/)
  {
    // We got an interrupt with the device being closed.  
    // This is not unlikely?  
    // We just quietly keep servicing the causes until it calms down.
    ServicedAnInterrupt = TRUE;
	  do
    {
      status = HW_CAN_READ_STATUS_ID(pController);

		  if(InterruptIdReg & IRQ_WUI)
      {
			  // wakeup interrupt
      }

		  if(InterruptIdReg & IRQ_TI)
      {
			  // transmission complete interrupt
		  }
		  if(InterruptIdReg & IRQ_RI)
      {
			  // receive interrupt
			  while(status & SR_RBS)
        {
				  HW_CAN_RELEASE_RECEIVE_BUFFER(pController);
				  status = HW_CAN_READ_STATUS_ID(pController);
			  }
		  }
		  if(InterruptIdReg & (IRQ_DOI | IRQ_EI | IRQ_BEI | IRQ_EPI | IRQ_ALI))
      {
			  // error interrupt
			  //if(sja1000_err(dev, isrc, status))
				 // break;
		  }
	  }
    while((InterruptIdReg = HW_CAN_READ_INTERRUPT_ID(pController)) != IRQ_OFF);
  } 
  else 
  {
    ServicedAnInterrupt = TRUE;
	  do
    {
      status = HW_CAN_READ_STATUS_ID(pController);

		  if(InterruptIdReg & IRQ_WUI)
      {
			  // wakeup interrupt
      }

		  if(InterruptIdReg & IRQ_TI)
      {
			  // transmission complete interrupt
        Extension->Controller.stats.tx_bytes += HW_CAN_READ_FRAME_INFO(pController) & 0xf;
        Extension->Controller.stats.tx_packets ++;
			  
        if(Extension->WriteLength)
        {
          Extension->WriteLength--;
          if(Extension->WriteLength)
          {
            // We not finished transmitting all frames
            Extension->WriteCurrentFrame++;
            sja1000_start_xmit(&Extension->Controller, Extension->WriteCurrentFrame);
          }
          else
          {
            // No More frames left.  This write is complete.
            if(Extension->CurrentWriteIrp)
            {
              PIO_STACK_LOCATION IrpSp;
              IrpSp = IoGetCurrentIrpStackLocation(Extension->CurrentWriteIrp);
              Extension->CurrentWriteIrp->IoStatus.Information = IrpSp->Parameters.Write.Length;
            }
            CanInsertQueueDpc(&Extension->CompleteWriteDpc, NULL, NULL, Extension);
            //F_CAN_SetStatus(Extension->Controller.status, CAN_STATUS_TXBUF);
          }
        }
		  }
		  if(InterruptIdReg & IRQ_RI)
      {
			  // receive interrupt
        F_CAN_RX cf;
			  while(status & SR_RBS)
        {
          cf.timestamp = CanGetTimeStamp(Extension);
          if(sja1000_rx(pController, &cf.msg))
            CanPutRx(Extension, &cf);
				  status = HW_CAN_READ_STATUS_ID(pController);
			  }
		  }
		  if(InterruptIdReg & (IRQ_DOI | IRQ_EI | IRQ_BEI | IRQ_EPI | IRQ_ALI))
      {
			  // error interrupt
        F_CAN_RX ef;
        ef.timestamp = CanGetTimeStamp(Extension);
        sja1000_err(pController, InterruptIdReg, status, &ef.msg);
        // check error frame is enabled & error type is accepted
        if((pController->settings.opmode & CAN_OPMODE_ERRFRAME) && (pController->settings.error_mask & ef.msg.can_id))
        {
          CanPutRx(Extension, &ef);
        }

        if((ef.msg.can_id & CAN_ERR_BUSOFF) && Extension->RestartMs)
        {
          // The device goes bus-off.
          if(Extension->RestartMs)
          {
            // If enabled, a timer is started to trigger bus-off recovery.
            CanInsertQueueDpc(&Extension->CanRecoverDpc, NULL, NULL, Extension)? Extension->CountOfTryingToRestartController++: 0;
          }
        }
		  }
	  }
    while((InterruptIdReg = HW_CAN_READ_INTERRUPT_ID(pController)) != IRQ_OFF);
  }

  if(ServicedAnInterrupt)
  {
    if(Extension->WaitStatus.status & Extension->Controller.status)
    {
      F_CAN_ClearStatus(Extension->WaitStatus.status, Extension->Controller.status);
      CanInsertQueueDpc(&Extension->CanWaitDpc, NULL, NULL, Extension);
    }
  }

  // This will "unlock" the close and cause the event
  // to fire if we didn't queue any DPC's
  while(1)
  {
    LONG pendingCnt;

    // Increment once more.  This is just a quick test to see if we
    // have a chance of causing the event to fire... we don't want
    // to run a DPC on every ISR if we don't have to....
    InterlockedIncrement(&Extension->DpcCount);
    // Decrement and see if the lock above looks like the only one left.
    pendingCnt = InterlockedDecrement(&Extension->DpcCount);
    if(pendingCnt == 1)
    {
      KeInsertQueueDpc(&Extension->IsrUnlockPagesDpc, NULL, NULL);
    }
    else
    {
      if(InterlockedDecrement(&Extension->DpcCount) != 0)
        break;

      // We missed it.  Retry...
      InterlockedIncrement(&Extension->DpcCount);
    }
  }

  return ServicedAnInterrupt;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This is the isr that the system will call if there are any
//    can devices sharing the same interrupt. This routine traverses
//    a linked list structure that contains a pointer to a more
//    refined isr and context that will indicate whether one of
//    the ports on this interrupt actually was interrupting.
//
//Arguments:
//
//    InterruptObject - Points to the interrupt object declared for this
//    device.  We *do not* use this parameter.
//
//    Context - Pointer to a linked list of contextes and isrs.
//    device.
//
//Return Value:
//
//    This function will return TRUE if a can port using this
//    interrupt was the source of this interrupt, FALSE otherwise.
//-----------------------------------------------------------------------------
BOOLEAN CanSharerIsr(IN PKINTERRUPT InterruptObject, IN PVOID Context)
{
  BOOLEAN servicedAnInterrupt = FALSE;
  BOOLEAN thisPassServiced;
  PLIST_ENTRY interruptEntry = ((PLIST_ENTRY)Context)->Flink;
  PLIST_ENTRY firstInterruptEntry = Context;

  if(IsListEmpty(firstInterruptEntry))
  {
    return FALSE;
  }

  do
  {
    thisPassServiced = FALSE;
    do
    {
      PCAN_DEVICE_EXTENSION extension = CONTAINING_RECORD(interruptEntry, CAN_DEVICE_EXTENSION, TopLevelSharers);
      thisPassServiced |= extension->TopLevelOurIsr(InterruptObject, extension->TopLevelOurIsrContext);
      servicedAnInterrupt |= thisPassServiced;
      interruptEntry = interruptEntry->Flink;
    }while(interruptEntry != firstInterruptEntry);
  } while (thisPassServiced);

  return servicedAnInterrupt;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    All interrupts get vectored through here and switched.
//
//Arguments:
//
//    InterruptObject - Points to the interrupt object declared for this
//    device.  We merely pass this parameter along.
//
//    Context - Actually a PCAN_CISR_SW; a switch structure for
//    can ISR's that contains the real function and context to use.
//
//Return Value:
//
//    This function will return TRUE if a can port using this
//    interrupt was the source of this interrupt, FALSE otherwise.
//-----------------------------------------------------------------------------
BOOLEAN CanCIsrSw(IN PKINTERRUPT InterruptObject, IN PVOID Context)
{
  PCAN_CISR_SW csw = (PCAN_CISR_SW)Context;

  return (*(csw->IsrFunc))(InterruptObject, csw->Context);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    NOTE: This routine assumes that it is called at interrupt
//          level.
//
//Arguments:
//
//    Context - Really a pointer to the device extension.
//
//Return Value:
//
//    Always FALSE.
//-----------------------------------------------------------------------------
BOOLEAN CanDecrementRestartCounter(IN PVOID Context)
{
  PCAN_DEVICE_EXTENSION Extension = Context;
  Extension->CountOfTryingToRestartController--;

  return FALSE;
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This dpc routine exists solely to call the code that
//    restart can controller for automatic bus-off recovery.
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
VOID CanInvokeRestartController(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2)
{
  PCAN_DEVICE_EXTENSION Extension = DeferredContext;

  UNREFERENCED_PARAMETER(Dpc);
  UNREFERENCED_PARAMETER(SystemContext1);
  UNREFERENCED_PARAMETER(SystemContext2);

  KeSynchronizeExecution(Extension->Interrupt, CanRestart, Extension);

  CanDpcEpilogue(Extension, Dpc);
}
//-----------------------------------------------------------------------------
//Routine Description:
//
//    This routine starts a timer that when it expires will start
//    a dpc that will restart the hardware.
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
VOID CanRecover(IN PKDPC Dpc, IN PVOID DeferredContext, IN PVOID SystemContext1, IN PVOID SystemContext2)
{
  PCAN_DEVICE_EXTENSION Extension = DeferredContext;
  KIRQL OldIrql;

  UNREFERENCED_PARAMETER(Dpc);
  UNREFERENCED_PARAMETER(SystemContext1);
  UNREFERENCED_PARAMETER(SystemContext2);

  // First we calculate the number of 100 nanosecond intervals.
  // Take out the lock to prevent the line control
  // from changing out from under us while we calculate the number.
  KeAcquireSpinLock(&Extension->ControlLock, &OldIrql);
  Extension->RestartMs = Extension->Timeouts.RestartBusoffTimeout;
  KeReleaseSpinLock(&Extension->ControlLock, OldIrql);

  if(Extension->RestartMs)
  {
    LARGE_INTEGER wait_time;
    wait_time.QuadPart = 10000 * Extension->RestartMs;
    wait_time.QuadPart = -wait_time.QuadPart;

    if(CanSetTimer(&Extension->RestartControllerTimer, wait_time, &Extension->RestartControllerDpc, Extension))
    {
      // The timer was already in the timer queue.  This implies
      // that one path of execution that was trying to restart hardware "died".
      // Synchronize with the ISR so that we can lower the count.
      KeSynchronizeExecution(Extension->Interrupt, CanDecrementRestartCounter, Extension);
    }
  }
  CanDpcEpilogue(Extension, Dpc);
}
//-----------------------------------------------------------------------------

#endif