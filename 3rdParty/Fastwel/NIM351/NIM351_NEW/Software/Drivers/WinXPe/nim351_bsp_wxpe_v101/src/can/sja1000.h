#ifndef __SJA1000_H__
#define __SJA1000_H__

#include <can/can.h>

typedef enum F_SJA1000_OPERATION_MODE
{
  BASIC_CAN,
  PELI_CAN
} F_SJA1000_OPERATION_MODE;

// This value - which could be redefined at compile
// time, define the stride between registers
#if !defined(CAN_REGISTER_STRIDE)
  #define CAN_REGISTER_STRIDE 1
#endif

#if !defined(F_SJA1000_read)
  #define F_SJA1000_read(reg) el_read8(reg)
  #define F_SJA1000_write(reg, v) el_write8(reg, v)
#endif

// Offsets from the base register address of the
// various registers for the CJA1000.

//----------------- BasicCAN mode -----------------------
#define BC_ctrl_REG               ((ULONG)((0)*CAN_REGISTER_STRIDE))
#define BC_cmnd_REG               ((ULONG)((1)*CAN_REGISTER_STRIDE))
#define BC_sts_REG                ((ULONG)((2)*CAN_REGISTER_STRIDE))
#define BC_inter_REG              ((ULONG)((3)*CAN_REGISTER_STRIDE))
#define BC_accc_REG               ((ULONG)((4)*CAN_REGISTER_STRIDE))
#define BC_accm_REG               ((ULONG)((5)*CAN_REGISTER_STRIDE))
#define BC_bt0_REG                ((ULONG)((6)*CAN_REGISTER_STRIDE))
#define BC_bt1_REG                ((ULONG)((7)*CAN_REGISTER_STRIDE))
#define BC_out_ctrl_REG           ((ULONG)((8)*CAN_REGISTER_STRIDE))
#define BC_test_REG               ((ULONG)((9)*CAN_REGISTER_STRIDE))
#define BC_tx_frame_start_REG     ((ULONG)((10)*CAN_REGISTER_STRIDE))
#define BC_rx_frame_start_REG     ((ULONG)((20)*CAN_REGISTER_STRIDE))
#define BC_clock_div_REG          ((ULONG)((31)*CAN_REGISTER_STRIDE))
// определения битов регистров
#define BC_CR_RR      0x01        /* Reset Request */
#define BC_CR_RIE     0x02        /* Receive Interrupt Enable */
#define BC_CR_TIE     0x04        /* Transmit Interrupt Enable */
#define BC_CR_EIE     0x08        /* Error Interrupt Enable */
#define BC_CR_OIE     0x10        /* Overrun Interrupt Enable */
#define BC_CR_S       0x20        /* SYNCH */

#define BC_CMR_TR     0x01        /*  Transmission Request  */
#define BC_CMR_AT     0x02        /*  Abort Transmission  */
#define BC_CMR_RRB    0x04        /*  Release Receive Buffer  */
#define BC_CMR_COS    0x08        /*  Clear Overrun Status  */
#define BC_CMR_GTS    0x10        /*  Sleep- und Wake-up-Bit */

#define BC_SR_RBS     0x01        /*  Receive Buffer Status  */
#define BC_SR_DO      0x02        /*  Data Overrun  */
#define BC_SR_TBA     0x04        /*  Transmit Buffer Access  */
#define BC_SR_TCS     0x08        /*  Transmit Complete Status  */
#define BC_SR_RS      0x10        /*  Receive Status  */
#define BC_SR_TS      0x20        /*  Transmit Status  */
#define BC_SR_ES      0x40        /*  Error Status  */
#define BC_SR_BS      0x80        /*  Bus Status  */

#define BC_IR_RI      0x01        /*  Receive Interrupt  */
#define BC_IR_TI      0x02        /*  Transmit Interrupt  */
#define BC_IR_EI      0x04        /*  Error Interrupt  */
#define BC_IR_OI      0x08        /*  Overrun Interrupt  */
#define BC_IR_WUI     0x10        /*  Wake-Up Interrupt  */
#define BC_IR_ALL     0x1f

// ----------------- PeliCAN mode -----------------------
#define PC_mode_REG               ((ULONG)((0)*CAN_REGISTER_STRIDE))
#define PC_cmnd_REG               ((ULONG)((1)*CAN_REGISTER_STRIDE))
#define PC_sts_REG                ((ULONG)((2)*CAN_REGISTER_STRIDE))
#define PC_inter_REG              ((ULONG)((3)*CAN_REGISTER_STRIDE))
#define PC_inter_enbl_REG         ((ULONG)((4)*CAN_REGISTER_STRIDE))
#define PC_bt0_REG                ((ULONG)((6)*CAN_REGISTER_STRIDE))
#define PC_bt1_REG                ((ULONG)((7)*CAN_REGISTER_STRIDE))
#define PC_out_ctrl_REG           ((ULONG)((8)*CAN_REGISTER_STRIDE))
#define PC_test_REG               ((ULONG)((9)*CAN_REGISTER_STRIDE))
#define PC_arb_lost_cpt_REG       ((ULONG)((11)*CAN_REGISTER_STRIDE))
#define PC_err_code_cpt_REG       ((ULONG)((12)*CAN_REGISTER_STRIDE))
#define PC_err_warn_lmt_REG       ((ULONG)((13)*CAN_REGISTER_STRIDE))
#define PC_err_rx_cntr_REG        ((ULONG)((14)*CAN_REGISTER_STRIDE))
#define PC_err_tx_cntr_REG        ((ULONG)((15)*CAN_REGISTER_STRIDE))
#define PC_frame_start_REG        ((ULONG)((16)*CAN_REGISTER_STRIDE))
#define PC_clock_div_REG          ((ULONG)((31)*CAN_REGISTER_STRIDE))
// определения битов регистров
#define PC_CMR_TR     0x01        /*  Transmission Request  */
#define PC_CMR_AT     0x02        /*  Abort Transmission  */
#define PC_CMR_RRB    0x04        /*  Release Receive Buffer  */
#define PC_CMR_COS    0x08        /*  Clear Overrun Status  */
#define PC_CMR_SRR    0x10        /*  Self Reception Request */

#define CAN_REGISTER_SPAN         ((ULONG)(128*CAN_REGISTER_STRIDE))


/* SJA1000 registers - manual section 6.4 (Pelican Mode) */
#define REG_MOD		0x00
#define REG_CMR		0x01
#define REG_SR		0x02
#define REG_IR		0x03
#define REG_IER		0x04
#define REG_ALC		0x0B
#define REG_ECC		0x0C
#define REG_EWL		0x0D
#define REG_RXERR	0x0E
#define REG_TXERR	0x0F
#define REG_ACCC0	0x10
#define REG_ACCC1	0x11
#define REG_ACCC2	0x12
#define REG_ACCC3	0x13
#define REG_ACCM0	0x14
#define REG_ACCM1	0x15
#define REG_ACCM2	0x16
#define REG_ACCM3	0x17
#define REG_RMC		0x1D
#define REG_RBSA	0x1E

/* Common registers - manual section 6.5 */
#define REG_BTR0	0x06
#define REG_BTR1	0x07
#define REG_OCR		0x08
#define REG_CDR		0x1F

#define REG_FI		0x10    // frame information
#define SFF_BUF		0x13    // standard frame buffer adress
#define EFF_BUF		0x15    // extendede frame buffer adress

#define FI_FF		0x80
#define FI_RTR		0x40

#define REG_ID1		0x11
#define REG_ID2		0x12
#define REG_ID3		0x13
#define REG_ID4		0x14

#define CAN_RAM		0x20

/* mode register */
#define MOD_RM		0x01
#define MOD_LOM		0x02
#define MOD_STM		0x04
#define MOD_AFM		0x08
#define MOD_SM		0x10

/* commands */
#define CMD_SRR		0x10
#define CMD_CDO		0x08    // Clear Data Overrun
#define CMD_RRB		0x04    // Release Receive Buffer
#define CMD_AT		0x02    // Abort Transmission
#define CMD_TR		0x01

/* interrupt sources */
#define IRQ_BEI		0x80
#define IRQ_ALI		0x40
#define IRQ_EPI		0x20
#define IRQ_WUI		0x10
#define IRQ_DOI		0x08
#define IRQ_EI		0x04
#define IRQ_TI		0x02
#define IRQ_RI		0x01
#define IRQ_ALL		0xFF
#define IRQ_OFF		0x00

/* status register content */
#define SR_BS		  0x80
#define SR_ES		  0x40
#define SR_TS		  0x20
#define SR_RS		  0x10
#define SR_TCS		0x08
#define SR_TBS		0x04
#define SR_DOS		0x02
#define SR_RBS		0x01

#define SR_CRIT (SR_BS|SR_ES)

/* ECC register */
#define ECC_SEG		0x1F
#define ECC_DIR		0x20
#define ECC_ERR		6
#define ECC_BIT		0x00
#define ECC_FORM	0x40
#define ECC_STUFF	0x80
#define ECC_MASK	0xc0

/* clock divider register */
#define CDR_CLKOUT_MASK 0x07
#define CDR_CLK_OFF	0x08 /* Clock off (CLKOUT pin) */
#define CDR_RXINPEN	0x20 /* TX1 output is RX irq output */
#define CDR_CBP		0x40 /* CAN input comparator bypass */
#define CDR_PELICAN	0x80 /* PeliCAN mode */

/* output control register */
#define OCR_MODE_BIPHASE  0x00
#define OCR_MODE_TEST     0x01
#define OCR_MODE_NORMAL   0x02
#define OCR_MODE_CLOCK    0x03
#define OCR_MODE_MASK     0x07
#define OCR_TX0_INVERT    0x04
#define OCR_TX0_PULLDOWN  0x08
#define OCR_TX0_PULLUP    0x10
#define OCR_TX0_PUSHPULL  0x18
#define OCR_TX1_INVERT    0x20
#define OCR_TX1_PULLDOWN  0x40
#define OCR_TX1_PULLUP    0x80
#define OCR_TX1_PUSHPULL  0xc0
#define OCR_TX_MASK       0xfc
#define OCR_TX_SHIFT      2

// CAN bus oscillator frequency
#define SJA1000_CYCLE_FREQUANCY_16MHZ 16
#define SJA1000_CYCLE_FREQUANCY_20MHZ 20

#ifndef SJA1000_CYCLE_FREQUANCY
  #define SJA1000_CYCLE_FREQUANCY   SJA1000_CYCLE_FREQUANCY_16MHZ
#endif

// Структура данных элемента таблицы установок bus timing register1 и bus timing register2
typedef struct F_SJA1000_BUSTIMINGS
{
  u8  bt0;
  u8  bt1;
} F_SJA1000_BUSTIMINGS;

//// Структура специфичных для SJA1000 данных
//typedef struct F_SJA1000_HARDC
//{
//  F_SJA1000_OPERATION_MODE  op_mode;
//  F_SJA1000_BUSTIMINGS      bus_timigs;
//
//} F_SJA1000_HARDC, *PF_SJA1000_HARDC;

#define HW_CAN_READ_REG(pdev, reg)          (F_SJA1000_read((pdev)->reg_base+(reg)))
#define HW_CAN_WRITE_REG(pdev, reg, val)    (F_SJA1000_write((pdev)->reg_base+(reg), (val)))

#define HW_CAN_IS_INTERRUPT_ENABLE(pdev)    ((F_SJA1000_read((pdev)->reg_base+REG_IER))!=IRQ_OFF)

#define HW_CAN_DISABLE_ALL_INTERRUPTS(pdev) (F_SJA1000_write((pdev)->reg_base+REG_IER, IRQ_OFF))
#define HW_CAN_READ_INTERRUPT_ID(pdev)      (F_SJA1000_read((pdev)->reg_base+REG_IR))

#define HW_CAN_READ_STATUS_ID(pdev)         (F_SJA1000_read((pdev)->reg_base+REG_SR))

#define HW_CAN_READ_FRAME_INFO(pdev)        (F_SJA1000_read((pdev)->reg_base+REG_FI))

#define HW_CAN_READ_EXTENDED_FRAME_ID(pdev) \
  ((((u32)(F_SJA1000_read((pdev)->reg_base+REG_ID1)))<<(5+16)) | \
  (((u32)(F_SJA1000_read((pdev)->reg_base+REG_ID2)))<<(5+8)) | \
  (((u32)(F_SJA1000_read((pdev)->reg_base+REG_ID3)))<<5) | \
  (((u32)(F_SJA1000_read((pdev)->reg_base+REG_ID4)))>>3))

#define HW_CAN_READ_STANDARD_FRAME_ID(pdev) \
  (((u32)(F_SJA1000_read((pdev)->reg_base+REG_ID1)))<<3) | (((u32)(F_SJA1000_read((pdev)->reg_base+REG_ID2)))>>5)

#define HW_CAN_READ_FRAME_DATA(pdev, dreg)  (F_SJA1000_read((pdev)->reg_base+(dreg)))

//After reading the contents of the receive buffer, the CPU can release this memory space in the RXFIFO by setting
//the release receive buffer bit to logic 1. This may result in another message becoming immediately available within
//the receive buffer. If there is no other message available, the receive interrupt bit is reset.
//-- manual section 6.4.4
#define HW_CAN_RELEASE_RECEIVE_BUFFER(pdev) (F_SJA1000_write((pdev)->reg_base + REG_CMR, CMD_RRB))

bool sja1000_probe_chip(PF_CAN_DEVICE pdev);
bool sja1000_init_chip(PF_CAN_DEVICE pdev);
bool sja1000_set_reset_mode(PF_CAN_DEVICE pdev);
bool sja1000_set_normal_mode(PF_CAN_DEVICE pdev);
bool sja1000_start(PF_CAN_DEVICE pdev);
bool sja1000_stop(PF_CAN_DEVICE pdev);
bool sja1000_rx(PF_CAN_DEVICE pdev, PF_CAN_MSG pcf);
void sja1000_start_xmit(PF_CAN_DEVICE pdev, PF_CAN_MSG pcf);
void sja1000_abort_transmission(PF_CAN_DEVICE pdev);
void sja1000_err(PF_CAN_DEVICE pdev, u8 isrc, u8 status, PF_CAN_MSG pcf);
bool sja1000_set_settings(PF_CAN_DEVICE pdev, PF_CAN_SETTINGS pNewSettigs);
u8 get_switch_state(u8* psw);
void sja100_dump_regs(PF_CAN_DEVICE pdev, char* title);
////bool sja1000_get_status(PF_CAN_DEVICE pdev, F_CAN_STATUS* pstatus);

#endif