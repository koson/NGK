#if defined __CAN_DRIVER__

#include <can/canhw.h>
#include <can/sja1000.h>

#define SJA1000_BAUDINDEX_TABLE_SIZE  9
#define SJA1000_BAUDRATE_DEFAULT  CANBR_250kBaud

F_SJA1000_BUSTIMINGS  const SJA1000_BdiTable[SJA1000_BAUDINDEX_TABLE_SIZE] = 
{
#if !defined(SJA1000_CYCLE_FREQUANCY)
#error Undefined CAN controller frequency
#elif SJA1000_CYCLE_FREQUANCY == SJA1000_CYCLE_FREQUANCY_16MHZ
  { 0x00, 0x14 },         /* 1000 kb CIA settings */
  { 0x00, 0x16 },         /*  800 kb CIA settings */   
  { 0x00, 0x1c },         /*  500 kb CIA settings */
  { 0x01, 0x1c },         /*  250 kb CIA settings */
  { 0x03, 0x1c },         /*  125 kb CIA settings */
  { 0x04, 0x1c },         /*  100 kb CIA settings */
  { 0x09, 0x1c },         /*   50 kb CIA settings */
  { 0x18, 0x1c },         /*   20 kb CIA settings */
  { 0x31, 0x1c }          /*   10 kb CIA settings */
#elif SJA1000_CYCLE_FREQUANCY == SJA1000_CYCLE_FREQUANCY_20MHZ
  { 0x00, 0x16 },         /* 1000 kb CIA settings */
  { 0x00, 0x16 },         /*  dummy Value         */
  { 0x40, 0x2f },         /*  500 kb CIA settings */
  { 0x41, 0x2f },         /*  250 kb CIA settings */
  { 0x43, 0x2f },         /*  125 kb CIA settings */
  { 0x44, 0x2f },         /*  100 kb CIA settings */
  { 0x49, 0x2f },         /*   50 kb CIA settings */
  { 0x58, 0x2f },         /*   20 kb CIA settings */
  { 0x71, 0x2f }          /*   10 kb CIA settings */
#else
#error Unknown CAN controller cycle frequency
#endif
};

//#include <eal/debug/idbg.h>
#include <eal/debug/udbg.h>

//-----------------------------------------------------------------------------
#ifdef FMSG_ENABLE_DEBUG

void sja100_dump_regs(PF_CAN_DEVICE pdev, char* title)
{
  if(title)
  {
    F_DBG1("%s", title);
  }
  F_DBG3("MOD(0x%02X)   SR(0x%02X)    IER(0x%02X)\n", HW_CAN_READ_REG(pdev, REG_MOD), HW_CAN_READ_REG(pdev, REG_SR), HW_CAN_READ_REG(pdev, REG_IER));
  F_DBG2("BTR0(0x%02X)  BTR1(0x%02X)\n", HW_CAN_READ_REG(pdev, REG_BTR0), HW_CAN_READ_REG(pdev, REG_BTR1));
  F_DBG2("OCR(0x%02X)   CDR(0x%02X)\n", HW_CAN_READ_REG(pdev, REG_OCR), HW_CAN_READ_REG(pdev, REG_CDR));
  F_DBG2("RXERR(0x%02X) TXERR(0x%02X)\n", HW_CAN_READ_REG(pdev, REG_RXERR), HW_CAN_READ_REG(pdev, REG_TXERR));
}
#endif
//-----------------------------------------------------------------------------
u8 get_switch_state(u8* psw)
{
  u8 mod = el_in8(psw);

	return mod;
}
//-----------------------------------------------------------------------------
// TODO: Сделать в соответствии с документацией
static void sja1000_set_acc_filter(PF_CAN_DEVICE pdev, PF_CAN_SETTINGS pSettings)
{
  u16 format = pSettings->opmode & (CAN_OPMODE_STANDARD|CAN_OPMODE_EXTENDED);
  u32 ac;
  u32 am;

  // Single filter mode!!!
  if(format == CAN_OPMODE_STANDARD)
  {
    ac = pSettings->acceptance_code<<4;
    am = ~(pSettings->acceptance_mask<<4);

    HW_CAN_WRITE_REG(pdev, REG_ACCC0, (u8)(ac>>8));
    HW_CAN_WRITE_REG(pdev, REG_ACCC1, (u8)(ac));
    HW_CAN_WRITE_REG(pdev, REG_ACCC2, 0x00);
    HW_CAN_WRITE_REG(pdev, REG_ACCC3, 0x00);

    HW_CAN_WRITE_REG(pdev, REG_ACCM0, (u8)(am>>8));
    HW_CAN_WRITE_REG(pdev, REG_ACCM1, (u8)(am));
    HW_CAN_WRITE_REG(pdev, REG_ACCM2, 0xFF);
    HW_CAN_WRITE_REG(pdev, REG_ACCM3, 0xFF);
  }
  else if(format == CAN_OPMODE_EXTENDED)
  {
    ac = pSettings->acceptance_code<<2;
    am = ~(pSettings->acceptance_mask<<2);
    
    HW_CAN_WRITE_REG(pdev, REG_ACCC0, (u8)(ac>>24));
    HW_CAN_WRITE_REG(pdev, REG_ACCC1, (u8)(ac>>16));
    HW_CAN_WRITE_REG(pdev, REG_ACCC2, (u8)(ac>>8));
    HW_CAN_WRITE_REG(pdev, REG_ACCC3, (u8)(ac));
    HW_CAN_WRITE_REG(pdev, REG_ACCM0, (u8)(am>>24));
    HW_CAN_WRITE_REG(pdev, REG_ACCM1, (u8)(am>>16));
    HW_CAN_WRITE_REG(pdev, REG_ACCM2, (u8)(am>>8));
    HW_CAN_WRITE_REG(pdev, REG_ACCM3, (u8)(am));
  }
  else
  {
    // accept all by hardware (use software filter)
    HW_CAN_WRITE_REG(pdev, REG_ACCC0, 0x00);
    HW_CAN_WRITE_REG(pdev, REG_ACCC1, 0x00);
    HW_CAN_WRITE_REG(pdev, REG_ACCC2, 0x00);
    HW_CAN_WRITE_REG(pdev, REG_ACCC3, 0x00);
    HW_CAN_WRITE_REG(pdev, REG_ACCM0, 0xFF);
    HW_CAN_WRITE_REG(pdev, REG_ACCM1, 0xFF);
    HW_CAN_WRITE_REG(pdev, REG_ACCM2, 0xFF);
    HW_CAN_WRITE_REG(pdev, REG_ACCM3, 0xFF);
  }
}
//-----------------------------------------------------------------------------
static bool accept_message(PF_CAN_DEVICE pdev, PF_CAN_MSG pcf)
{
  u16 format = pdev->settings.opmode & (CAN_OPMODE_STANDARD|CAN_OPMODE_EXTENDED);
  bool res = true;

  if(format == CAN_OPMODE_STANDARD)
  {
    if(pcf->can_id & CAN_EFF_FLAG)
      res = false;
  }
  else if(format == CAN_OPMODE_EXTENDED)
  {
    if((pcf->can_id & CAN_EFF_FLAG)==0)
      res = false;
  }
  else
  {
    // software filter
    u32 acode = pdev->settings.acceptance_code;
    u32 amask = pdev->settings.acceptance_mask;
    u32 fmask = (pcf->can_id & CAN_EFF_FLAG)? ((CAN_EFF_MASK<<1) | 0x01) : ((CAN_SFF_MASK<<1) | 0x01); 
    u32 id = (pcf->can_id<<1);
    
    if(pcf->can_id & CAN_RTR_FLAG)
      id |= 0x01;
    
    res = ((((acode ^ id) & amask) & fmask) == 0);
  }

  return res;
}
//-----------------------------------------------------------------------------
static bool sja1000_set_bitrate(PF_CAN_DEVICE pdev, F_CAN_BAUDRATE br)
{
  u8 btr0, btr1;
  bool res = true;

  if(F_CANBR_isUser(br))
  {
    btr0 = F_CANBR_getUserBtr0(br);
    btr1 = F_CANBR_getUserBtr1(br);
  }
  else
  {
    // index in the table
    if(br >= SJA1000_BAUDINDEX_TABLE_SIZE)
    {
      res = false;
    }
#if SJA1000_CYCLE_FREQUANCY == SJA1000_CYCLE_FREQUANCY_20MHZ
    else if(br == BDI_800kBaud)  // 800kb недопустимо для 20MHz
    {
      res = false;
    }
#endif
    else
    {
      btr0 = SJA1000_BdiTable[br].bt0;
      btr1 = SJA1000_BdiTable[br].bt1;
    }
  }

  if(res)
  {
	  HW_CAN_WRITE_REG(pdev, REG_BTR0, btr0);
	  HW_CAN_WRITE_REG(pdev, REG_BTR1, btr1);
  }

  return res;
}
//-----------------------------------------------------------------------------

bool sja1000_set_settings(PF_CAN_DEVICE pdev, PF_CAN_SETTINGS pNewSettigs)
{
  bool res = false;

  do
  {
    // BUS TIMING REGISTERs can be accessed (write) if the reset mode is active.
	  if((res = sja1000_set_reset_mode(pdev)) == false)
      break;

    // set baudrate
    if(!sja1000_set_bitrate(pdev, (F_CAN_BAUDRATE)pNewSettigs->baud_rate))
      break;
    // set acceptance filter
    sja1000_set_acc_filter(pdev, pNewSettigs);

    // save new values
    pdev->settings = *pNewSettigs;
    // read only
    pdev->settings.controller_type = PHILIPS_SJA_1000;

    res = true;
  }
  while(0);

#ifdef FMSG_ENABLE_DEBUG
  sja100_dump_regs(pdev, "sja1000_set_settings()>\n");
#endif
  return res;
}
//-----------------------------------------------------------------------------
bool sja1000_probe_chip(PF_CAN_DEVICE pdev)
{
	bool res = false;
  u8 mod;

  do
  {
    if(!pdev->reg_base)
      break;

    mod = HW_CAN_READ_REG(pdev, REG_CDR);

    if(mod == 0xFF)
      break;
		
    res = true;
	}
  while(0);

	return res;
}
//-----------------------------------------------------------------------------
bool sja1000_set_reset_mode(PF_CAN_DEVICE pdev)
{
	u8 status;
  bool res = false;
	int i;

  for(i=0; ;i++)
  {
		// check reset bit
    status = HW_CAN_READ_REG(pdev, REG_MOD);
		if(status & MOD_RM)
    {
      pdev->state = CAN_STATE_STOPPED;
			res = true;
      break;
		}

    // Failure
    if(i > 10)
      break;

    // set chip to reset mode
		HW_CAN_WRITE_REG(pdev, REG_MOD, MOD_RM);
    el_usleep(5);
  }

  //sja100_dump_regs(pdev, "sja1000_set_reset_mode()>\n");
  F_DBG1("sja1000_set_reset_mode()>res(%d)\n", res);
  return res;
}
//-----------------------------------------------------------------------------
bool sja1000_set_normal_mode(PF_CAN_DEVICE pdev)
{
  u8 status;
	bool res = false;
  int i;
  u8 modr = 0;

  if(pdev->settings.opmode & CAN_OPMODE_SELFTEST)
    modr |= MOD_STM;
  if(pdev->settings.opmode & CAN_OPMODE_LSTNONLY)
    modr |= MOD_LOM;
  // Single filter mode (bit AFM is logic 1).
  modr |= MOD_AFM;
  HW_CAN_WRITE_REG(pdev, REG_MOD, modr);

  for(i = 0; ;i++)
  {
		// check reset bit
    status = HW_CAN_READ_REG(pdev, REG_MOD);
		if((status & MOD_RM) == 0)
    {
			pdev->state = CAN_STATE_ERROR_ACTIVE;
			res = true;
      break;
		}
    // Failure
    if(i > 10)
      break;

    // leave reset mode
		HW_CAN_WRITE_REG(pdev, REG_MOD, modr);
		el_usleep(5);
	}
  
  //sja100_dump_regs(pdev, "sja1000_set_normal_mode()>\n");
  F_DBG1("sja1000_set_normal_mode()>res(%d)\n", res);
  return res;
}
//-----------------------------------------------------------------------------
bool sja1000_init_chip(PF_CAN_DEVICE pdev)
{
  bool res;
  
  res = sja1000_set_reset_mode(pdev);
  if(res)
  {
    u8 cdr;
    u8 ocr;

    // set clock divider register
    // go into Pelican mode, disable clkout, disable comparator
    cdr = CDR_PELICAN | CDR_CLK_OFF | CDR_CBP;
    HW_CAN_WRITE_REG(pdev, REG_CDR, cdr);

	  // disable all interrupts
	  HW_CAN_WRITE_REG(pdev, REG_IER, IRQ_OFF);

    // set acceptance filter
    sja1000_set_acc_filter(pdev, &pdev->settings);

    // set baudrate
    res = sja1000_set_bitrate(pdev, pdev->settings.baud_rate);

    // set output control register
    ocr = 0x1A; // connected to external transceiver
	  HW_CAN_WRITE_REG(pdev, REG_OCR, ocr);

    HW_CAN_WRITE_REG(pdev, REG_CMR, CMD_AT|CMD_RRB|CMD_CDO);
  }

  pdev->state = CAN_STATE_INIT;

  //sja100_dump_regs(pdev, "sja1000_init_chip()>\n");
  F_DBG1("sja1000_init_chip()>res(%d)\n", res);
	return res;
}
//-----------------------------------------------------------------------------
bool sja1000_start(PF_CAN_DEVICE pdev)
{
  bool res = false;

  do
  {
	  if((res = sja1000_set_reset_mode(pdev)) == false)
      break;
    // disable all interrupts
    HW_CAN_WRITE_REG(pdev, REG_IER, IRQ_OFF);

    // Clear error counters and error code capture
	  HW_CAN_WRITE_REG(pdev, REG_TXERR, 0x0);
	  HW_CAN_WRITE_REG(pdev, REG_RXERR, 0x0);
	  HW_CAN_READ_REG(pdev, REG_ECC);

    // leave reset mode
	  if((res = sja1000_set_normal_mode(pdev)) == false)
      break;

    pdev->transmitterBusy = false;
    F_CAN_SetStatus(pdev->status, CAN_STATUS_TXBUF);

    HW_CAN_WRITE_REG(pdev, REG_CMR, CMD_AT|CMD_RRB|CMD_CDO);
  
#ifdef FMSG_ENABLE_DEBUG
    sja100_dump_regs(pdev, "sja1000_start()...\n");
#endif

    // enable all interrupts
		HW_CAN_WRITE_REG(pdev, REG_IER, IRQ_ALL);
  }
  while(0);

  F_DBG1("sja1000_start()>res(%d)\n", res);
  return res;
}
//-----------------------------------------------------------------------------
bool sja1000_stop(PF_CAN_DEVICE pdev)
{
  bool res;

  // disable all interrupts
  HW_CAN_WRITE_REG(pdev, REG_IER, IRQ_OFF);
  // leave operating mode
  res = sja1000_set_reset_mode(pdev);

  F_DBG1("sja1000_stop()>res(%d)\n", res);
  return res;
}
//-----------------------------------------------------------------------------
bool sja1000_rx(PF_CAN_DEVICE pdev, PF_CAN_MSG pcf)
{
  u8  fi;
	u8  dreg;
	u32 id;
	u8  dlc;
	int i;
  bool res = true;

	fi = HW_CAN_READ_FRAME_INFO(pdev);
	dlc = fi & 0x0F;

  if(dlc <=8)
  {
    if(fi & FI_FF)
    {
		  // extended frame format (EFF)
		  dreg = EFF_BUF;
      id = HW_CAN_READ_EXTENDED_FRAME_ID(pdev);
		  id |= CAN_EFF_FLAG;
	  }
    else
    {
		  // standard frame format (SFF)
		  dreg = SFF_BUF;
		  id = HW_CAN_READ_STANDARD_FRAME_ID(pdev);
	  }

	  if(fi & FI_RTR)
		  id |= CAN_RTR_FLAG;

	  pcf->can_id = id;
	  pcf->can_dlc = dlc;
	  for (i = 0; i < dlc; i++, dreg++)
		  pcf->data[i] = HW_CAN_READ_FRAME_DATA(pdev, dreg);

	  while(i < 8)
		  pcf->data[i++] = 0;

    pdev->stats.rx_packets++;
	  pdev->stats.rx_bytes += dlc;

    F_DBG2("sja1000_rx(): id(0x%08X), dlc(%d)\n", id, dlc);
  }
  else
  {
    res = false;
    F_DBG1("sja1000_rx(): unexpected frame REG_FI(0x%02X)\n", fi);
  }

  // release receive buffer
	HW_CAN_RELEASE_RECEIVE_BUFFER(pdev);

  if(res)
    res = accept_message(pdev, pcf);

  return res;
}
//-----------------------------------------------------------------------------
void sja1000_start_xmit(PF_CAN_DEVICE pdev, PF_CAN_MSG pcf)
{
  u8  fi;
	u8  dlc;
	u32 id;
	u8  dreg;
	int i;

	id = pcf->can_id;
  fi = dlc = pcf->can_dlc;

  F_DBG2(">sja1000_start_xmit(): id(0x%08X), dlc(%d)\n", id, dlc);
#ifdef FMSG_ENABLE_DEBUG
  sja100_dump_regs(pdev, NULL);
#endif

  if(dlc > 8)
    dlc=8;

	if(id & CAN_RTR_FLAG)
  {
    if(dlc>0)
      return;
		fi |= FI_RTR;
  }

	if(id & CAN_EFF_FLAG)
  {
		fi |= FI_FF;
		dreg = EFF_BUF;
		HW_CAN_WRITE_REG(pdev, REG_FI, fi);
		HW_CAN_WRITE_REG(pdev, REG_ID1, (id & 0x1fe00000) >> (5 + 16));
		HW_CAN_WRITE_REG(pdev, REG_ID2, (id & 0x001fe000) >> (5 + 8));
		HW_CAN_WRITE_REG(pdev, REG_ID3, (id & 0x00001fe0) >> 5);
		HW_CAN_WRITE_REG(pdev, REG_ID4, (id & 0x0000001f) << 3);
	} 
  else 
  {
		dreg = SFF_BUF;
		HW_CAN_WRITE_REG(pdev, REG_FI, fi);
		HW_CAN_WRITE_REG(pdev, REG_ID1, (id & 0x000007f8) >> 3);
		HW_CAN_WRITE_REG(pdev, REG_ID2, (id & 0x00000007) << 5);
	}

	for(i = 0; i < dlc; i++, dreg++)
		HW_CAN_WRITE_REG(pdev, dreg, pcf->data[i]);

  if(!(pdev->settings.opmode & CAN_OPMODE_SELFRECV))
  {
	  HW_CAN_WRITE_REG(pdev, REG_CMR, CMD_TR);
  }
  else
  {
    HW_CAN_WRITE_REG(pdev, REG_CMR, CMD_SRR | CMD_AT);
  }
  pdev->transmitterBusy = true;
  F_CAN_ClearStatus(pdev->status, CAN_STATUS_TXBUF);
}
//-----------------------------------------------------------------------------
void sja1000_abort_transmission(PF_CAN_DEVICE pdev)
{
  F_DBG(">sja1000_abort_transmission()\n");
#ifdef FMSG_ENABLE_DEBUG
  sja100_dump_regs(pdev, NULL);
#endif
  HW_CAN_WRITE_REG(pdev, REG_CMR, CMD_AT);
  //pdev->transmitterBusy = false;
  //F_CAN_SetStatus(pdev->status, CAN_STATUS_TXBUF);
}
//-----------------------------------------------------------------------------
void sja1000_err(PF_CAN_DEVICE pdev, u8 isrc, u8 status, PF_CAN_MSG pcf)
{
  F_CAN_STATE state = pdev->state;
	u8 ecc, alc;

  pcf->can_id = CAN_ERR_FLAG | CAN_ERR_UNSPEC;
  pcf->can_dlc = CAN_ERR_DLC;
  pcf->data[0] = CAN_ERR_LOSTARB_UNSPEC;
  pcf->data[1] = CAN_ERR_CRTL_UNSPEC;
  pcf->data[2] = CAN_ERR_PROT_UNSPEC;
  pcf->data[3] = CAN_ERR_PROT_LOC_UNSPEC;
  pcf->data[4] = CAN_ERR_TRX_UNSPEC;

	if(isrc & IRQ_DOI)
  {
		// data overrun interrupt
		pcf->can_id |= CAN_ERR_CRTL;
		pcf->data[1] |= CAN_ERR_CRTL_RX_OVERFLOW;
    pdev->stats.rx_over_errors++;
		pdev->stats.rx_errors++;
    pdev->errors.data_overrun++;
    F_CAN_SetStatus(pdev->status, CAN_STATUS_ERR);
    // clear bit
		HW_CAN_WRITE_REG(pdev, REG_CMR, CMD_CDO);
	}

	if(isrc & IRQ_EI)
  {
		// error warning interrupt
		if(status & SR_BS)
    {
			state = CAN_STATE_BUS_OFF;
			pcf->can_id |= CAN_ERR_BUSOFF;
      pdev->stats.bus_off++;
      pdev->errors.bus_off++;
      F_CAN_SetStatus(pdev->status, CAN_STATUS_ERR);
		}
    else if(status & SR_ES)
    {
			state = CAN_STATE_ERROR_WARNING;
		}
    else
    {
		  state = CAN_STATE_ERROR_ACTIVE;
    }
	}

	if(isrc & IRQ_BEI)
  {
		// bus error interrupt
		pdev->stats.bus_error++;
		pdev->stats.rx_errors++;

		ecc = HW_CAN_READ_REG(pdev, REG_ECC);
		pcf->can_id |= CAN_ERR_PROT | CAN_ERR_BUSERROR;

		switch(ecc & ECC_MASK)
    {
    case ECC_BIT:
			pcf->data[2] |= CAN_ERR_PROT_BIT;
			break;
		case ECC_FORM:
			pcf->data[2] |= CAN_ERR_PROT_FORM;
			break;
		case ECC_STUFF:
			pcf->data[2] |= CAN_ERR_PROT_STUFF;
			break;
		default:
			pcf->data[2] |= CAN_ERR_PROT_UNSPEC;
			pcf->data[3] = (ecc & ECC_SEG);
			break;
		}
		// Error occured during transmission?
		if((ecc & ECC_DIR) == 0)
    {
			pcf->data[2] |= CAN_ERR_PROT_TX;
      if((ecc & ECC_MASK) == CAN_ERR_PROT_LOC_ACK)
        pcf->can_id |= CAN_ERR_ACK;
    }
	}

	if (isrc & IRQ_EPI)
  {
		// error passive interrupt
		if(status & SR_ES)
			state = CAN_STATE_ERROR_PASSIVE;
		else
			state = CAN_STATE_ERROR_ACTIVE;
	}

	if(isrc & IRQ_ALI)
  {
		// arbitration lost interrupt
		alc = HW_CAN_READ_REG(pdev, REG_ALC);
		pdev->stats.arbitration_lost++;
		pdev->stats.tx_errors++;
		pcf->can_id |= CAN_ERR_LOSTARB;
		pcf->data[0] = alc & 0x1f;
	}

	if(state!=pdev->state && (state==CAN_STATE_ERROR_WARNING || state==CAN_STATE_ERROR_PASSIVE))
  {
		u8 rxerr = HW_CAN_READ_REG(pdev, REG_RXERR);
		u8 txerr = HW_CAN_READ_REG(pdev, REG_TXERR);
		pcf->can_id |= CAN_ERR_CRTL;
		if(state == CAN_STATE_ERROR_WARNING)
    {
			pdev->stats.error_warning++;
			pcf->data[1] = (txerr > rxerr)?	CAN_ERR_CRTL_TX_WARNING :	CAN_ERR_CRTL_RX_WARNING;
		}
    else
    {
			pdev->stats.error_passive++;
      pdev->errors.error_passive++;
			pcf->data[1] = (txerr > rxerr)?	CAN_ERR_CRTL_TX_PASSIVE :	CAN_ERR_CRTL_RX_PASSIVE;
      F_CAN_SetStatus(pdev->status, CAN_STATUS_ERR);
		}
	}

	pdev->state = state;
}
//-----------------------------------------------------------------------------
////bool sja1000_get_status(PF_CAN_DEVICE pdev, F_CAN_STATUS* pstatus)
////{
////  *pstatus = 0;
////  u8 sr = HW_CAN_READ_STATUS_ID(pdev);
////
////  if(sr & SR_BS)
////    *pstatus |= CAN_STATUS_BUS;
////  if(sr & SR_ES)
////    *pstatus |= CAN_STATUS_ERR;
////  if(sr & SR_TBS)
////    *pstatus |= CAN_STATUS_TXBUF;
////
////  return true;
////}
#endif