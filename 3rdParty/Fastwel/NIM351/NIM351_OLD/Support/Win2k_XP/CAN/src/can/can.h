/**
 *
 * NAME:    can/can.h
 *
 * DESCRIPTION: Типы данных и флаги для работы с CAN-адаптерами
 *
 *
 * AUTHOR:  Валерий Наконечный
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group Co Ltd.
 *      All rights reserved.
 ********************************************************/

#ifndef __CAN_H__
#define __CAN_H__

#include <eal/el.h>

// -------------------------------------------------------------------------
/**
 *  Типы данных и флаги, относящиеся к CAN-сообщению
 */

// Индикатор расширенного/стандартного формата кадра
#define CAN_EFF_FLAG 0x80000000U  // EFF/SFF is set in the MSB
// Индикатор RTR-кадра
#define CAN_RTR_FLAG 0x40000000U  // remote transmission request

// Битовые маски для значащих бит в кадрах стандартного
// и расширенного форматов
#define CAN_SFF_MASK 0x000007FFU  // standard frame format (SFF)
#define CAN_EFF_MASK 0x1FFFFFFFU  // extended frame format (EFF)

// CAN-сообщение
typedef struct _F_CAN_MSG
{
  // CAN-ID, включая флаги EFF/RTR/ERR
  u32 can_id;
  // Кол-во данных в кадре (от 0 до 8)
  u8  can_dlc;
  // Поле данных кадра
  u8  data[8];

} F_CAN_MSG, *PF_CAN_MSG;


// Входящее (принимаемое) сообщение
typedef struct _F_CAN_RX
{
  // Метка времени сообщения в микросекундах.
  // Время измеряется с момента открытия адаптера и переполняется 
  // через интервал равный примерно 71 минуте.
  u32         timestamp;
  // "Базовый класс" (само сообщение)
  F_CAN_MSG   msg;

} F_CAN_RX, *PF_CAN_RX;

// Исходящее (передаваемое) сообщение
typedef struct _F_CAN_TX
{
  // "Базовый класс" (само сообщение)
  F_CAN_MSG   msg;

} F_CAN_TX, *PF_CAN_TX;

// -------------------------------------------------------------------------
/**
 *  Коды состояний CAN-адаптера
 */
typedef enum _F_CAN_STATE
{
  // начальное состояние адаптера
	CAN_STATE_INIT = 0,
  // кол-во ошибок приема/передачи не более 96
  CAN_STATE_ERROR_ACTIVE,
  // кол-во ошибок приема/передачи от 96 до 127
	CAN_STATE_ERROR_WARNING,
  // кол-во ошибок приема/передачи от 128 до 255
	CAN_STATE_ERROR_PASSIVE,
  // кол-во ошибок передачи >=256 (BUSOFF)
	CAN_STATE_BUS_OFF,
  // адаптер остановлен
	CAN_STATE_STOPPED,
  // состояние сна, не используется в текущей версии
	CAN_STATE_SLEEPING

} F_CAN_STATE, *PF_CAN_STATE;

// -------------------------------------------------------------------------
/**
 *  Значения флагов статуса CAN-адаптера
 */

#define CAN_STATUS_RXBUF  0x01  // 1 - одно и более принятых сообщений
                                // доступны для чтения
                                // 0 - пусто; нет принятых сообщений
                                  
#define CAN_STATUS_TXBUF  0x02  // 1 - внутренний буфер передачи CAN-адаптера свободен;
                                // возможна запись в буфер передачи
                                // 0 - внутренний буфер передачи CAN-адаптера занят;
                                // некоторое сообщение ожидает отправки
                                // или находится в процессе отправки
                                
#define CAN_STATUS_ERR    0x04  // 1 - произошла ошибка с момента последнего
                                // сброса счетчика ошибок
                                // 0 - с момента последнего сброса счетчиков
                                // ошибок новых ошибок не произошло.

// Статус CAN-адаптера
typedef size_t F_CAN_STATUS;

// Сброс установка статуса
#define F_CAN_SetStatus(st, flags)   (st) |= (flags)
#define F_CAN_ClearStatus(st, flags)   (st) &= ~(flags)

// -------------------------------------------------------------------------
/**
 *  Значения флагов маски ожидаемого статуса CAN-адаптера: fw_can_wait()
 */

// Ожидание статуса CAN_STATUS_RXBUF
// Ожидание наличия в буфере приема хотя бы одного сообщения
#define CAN_WAIT_RX       CAN_STATUS_RXBUF

// Ожидание статуса CAN_STATUS_TXBUF
// Ожидание освобождения внутреннего буфера передачи CAN-адаптера
#define CAN_WAIT_TX       CAN_STATUS_TXBUF

// Ожидание статуса CAN_STATUS_ERR
// Ожидание любой ошибки
#define CAN_WAIT_ERR      CAN_STATUS_ERR

// -------------------------------------------------------------------------
/**
 *  Значения флагов сброса/прерывания операции для fw_can_purge()
 */

// прервать текущие/отложенные обращения к адаптеру по записи
#define CAN_PURGE_TXABORT       0x0001
// прервать текущие/отложенные обращения к адаптеру по чтению
#define CAN_PURGE_RXABORT       0x0002
// очистить внутренний буфер передачи драйвера данного адаптера
#define CAN_PURGE_TXCLEAR       0x0004
// очистить внутренний буфер приема драйвера данного адаптера
#define CAN_PURGE_RXCLEAR       0x0008
// аппаратный сброс адаптера
#define CAN_PURGE_HWRESET       0x0010

// Маска для fw_can_purge()
typedef size_t F_CAN_PURGE_MASK;

// -------------------------------------------------------------------------
/**
 *  Статистика функционирования адаптера
 */
typedef struct _F_CAN_STATS
{
  // кол-во принятых кадров
  u32 rx_packets;

  // кол-во принятых байт
  u32 rx_bytes;

  // кол-во переданных кадров
  u32 tx_packets;

  // кол-во переданных байт
  u32 tx_bytes;

  // кол-во ошибок приема
  u32 rx_errors;

  // кол-во переполнений буфера приема
  u32 rx_over_errors;

  // кол-во ошибок передачи
  u32 tx_errors;

  // кол-во переполнений буфера передачи
  u32 tx_over_errors;

  // кол-во ошибок на шине (кадрирование, CRC, etc.)
  u32 bus_error;

  // кол-во переходов в CAN_STATE_ERROR_WARNING
	u32 error_warning;

  // кол-во переходов в CAN_STATE_ERROR_PASSIVE
	u32 error_passive;

  // кол-во переходов в CAN_STATE_BUS_OFF
	u32 bus_off;

  // кол-во "проигрышей" арбитража
	u32 arbitration_lost;

  // кол-во перезапусков контроллера
	u32 restarts;

} F_CAN_STATS, *PF_CAN_STATS;


// -------------------------------------------------------------------------
/**
 *  Счетчики ошибок адаптера для fw_can_get_clear_errors()
 */

typedef struct _F_CAN_ERRORS
{
  // кол-во таймаутов передачи
  size_t tx_timeout;

  // кол-во переполнений буфера приема
  size_t data_overrun;

  // кол-во переходов в CAN_STATE_ERROR_PASSIVE
	size_t error_passive;

  // кол-во переходов в CAN_STATE_BUS_OFF
	size_t bus_off;

} F_CAN_ERRORS, *PF_CAN_ERRORS;

// -------------------------------------------------------------------------
/**
 *  Скорости обмена
 */
typedef enum _F_CAN_BAUDRATE
{
  CANBR_1MBaud    = 0,  // 1 MBit/sec
  CANBR_800kBaud  = 1,  // 800 kBit/sec
  CANBR_500kBaud  = 2,  // 500 kBit/sec
  CANBR_250kBaud  = 3,  // 250 kBit/sec
  CANBR_125kBaud  = 4,  // 125 kBit/sec
  CANBR_100kBaud  = 5,  // 100 kBit/sec
  CANBR_50kBaud   = 6,  // 50 kBit/sec
  CANBR_20kBaud   = 7,  // 20 kBit/sec
  CANBR_10kBaud   = 8   // 10 kBit/sec
} F_CAN_BAUDRATE;

// -------------------------------------------------------------------------
/**
 *  Таймауты адаптера по приему и передаче, используемые функциями:
 *  fw_can_get_timeouts()
 *  fw_can_set_timeouts()
 *  fw_can_recv()
 *  fw_can_send()
 *  
 *  Значение таймаута передачи функцией fw_can_send() определяется формулой:
 *  Tsend = N * WriteTotalTimeoutMultiplier + WriteTotalTimeoutConstant (мс),
 *  где N -- кол-во сообщений, определяемое параметром nTx.
 *  
 *  Если WriteTotalTimeoutMultiplier и WriteTotalTimeoutConstant равны 0,
 *  ожидание fw_can_send() будет бесконечным.
 *
 *  Если при вызове fw_can_recv() в буфере приема имеется хотя бы одно сообщение,
 *  fw_can_recv() прочитает их и немедленно завершится.
 *  Если при вызове fw_can_recv() в буфере приема нет ни одного сообщения, то
 *  при приеме первого же сообщения fw_can_recv() прочитает его и немедленно
 *  завершится.
 *  Если при вызове fw_can_recv() в буфере приема нет ни одного сообщения, и ранее
 *  был задан отличный от нуля ReadTotalTimeout, то при отсутствии принятых сообщений
 *  в течение ReadTotalTimeout (мс) функция fw_can_recv() заврешится таймаутом.
 */

typedef struct _F_CAN_TIMEOUTS
{
  // Множитель для вычисления таймаута передачи
  u32 WriteTotalTimeoutMultiplier;

  // Константа для вычисления таймаута передачи
  u32 WriteTotalTimeoutConstant;

  // Таймаут приема (в мс) функцией fw_can_recv()
  u32 ReadTotalTimeout;

  // Таймаут автоматического восстановления из состояния bus-off (CAN_STATE_BUS_OFF)
  u32 RestartBusoffTimeout;

} F_CAN_TIMEOUTS, *PF_CAN_TIMEOUTS;


// -------------------------------------------------------------------------
/**
 *  Параметры адаптера
 */

// Флаги режима работы:
// принимаютя сообщения стандартного формата (11-битовые CAN-ID)
#define CAN_OPMODE_STANDARD 0x0001
// принимаются сообщения расширенного формата (29-битовые CAN-ID)
#define CAN_OPMODE_EXTENDED 0x0002

// Индикация ошибок посредством специального CAN-сообщения
#define CAN_OPMODE_ERRFRAME 0x0004

// Функционирование в режиме "Listen Only" (мониторинг шины), при котором
// адаптер не передает никаких подтверждений, даже если сообщение принято успешно.
#define CAN_OPMODE_LSTNONLY 0x0008

// Адаптер будет выполнять передачу, даже при отсутствии подтверждения на шине
#define CAN_OPMODE_SELFTEST 0x0010

// Сообщения, удовлетворяющие заданному acceptance-фильтру, будут одновременно
// передаваться и приниматься адаптером.
#define CAN_OPMODE_SELFRECV 0x0020

// Тип адаптера CAN
typedef enum _F_CAN_CONTROLLER
{
  UNKNOWN_CAN_DEVICE,
  PHILIPS_SJA_1000,
} F_CAN_CONTROLLER;

// Параметры, используемые в fw_can_get_controller_config() и
// fw_can_set_controller_config().
typedef struct _F_CAN_SETTINGS
{
  // Тип адаптера (только чтение)
  F_CAN_CONTROLLER  controller_type;

  // Скорость обмена
  F_CAN_BAUDRATE    baud_rate;
  
  // Флаги режима работы
  u16               opmode;

  // Фильтр допускаемых CAN-ID-ов входящих (принимаемых) сообщений.
  // Определяется полями acceptance_code и acceptance_mask.

  // Битовый паттерн идентификаторов сообщений, подлежащих приему
  u32               acceptance_code;
  // Маска, позволяющая указать "значимость" битовых позиций при проверке 
  // соответствия битовому паттерну
  u32               acceptance_mask;

  // Маска, позволяющая фильтровать некоторые ошибки, передаваемые специальными
  // сообщениями об ошибках при работе в режиме CAN_OPMODE_ERRFRAME.
  u32               error_mask;

} F_CAN_SETTINGS, *PF_CAN_SETTINGS;


// -------------------------------------------------------------------------
/**
 *  Специальные сообщения об ошибках в режиме CAN_OPMODE_ERRFRAME
 */
//-----------------------------------------------------------------------------

// Длина поля данных сообщения об ошибке
#define CAN_ERR_DLC 8 /* dlc for error frames */

// Классы индицируемых ошибок

// Неизвестная ошибка :)
#define CAN_ERR_UNSPEC       0x00000000U /* unspecified */
// Таймаут передачи
#define CAN_ERR_TX_TIMEOUT   0x00000001U /* TX timeout / data[0..3] */
// Проигрыш арбитража (data[0])
#define CAN_ERR_LOSTARB      0x00000002U /* lost arbitration    / data[0]    */
// Внутренний сбой адаптера (data[1])
#define CAN_ERR_CRTL         0x00000004U /* controller problems / data[1]    */
// Нарушение протокола (data[2..3])
// ВНИМАНИЕ! Может возникать очень много и часто!
#define CAN_ERR_PROT         0x00000008U /* protocol violations (may flood!) / data[2..3] */
// Статус трансивера (data[4]) ???
#define CAN_ERR_TRX          0x00000010U /* transceiver status  / data[4]    */
// Не получен ACK на переданное сообщение
#define CAN_ERR_ACK          0x00000020U /* received no ACK on transmission */
// Bus Off
#define CAN_ERR_BUSOFF       0x00000040U /* bus off */
// Ошибка на шине 
// ВНИМАНИЕ! Может возникать очень много и часто!
#define CAN_ERR_BUSERROR     0x00000080U /* bus error (may flood!) */
// Адаптер перезапущен
#define CAN_ERR_RESTARTED    0x00000100U /* controller restarted */

/* TX timeout (by netdevice driver) / data[0..3] */
#define CAN_ERR_TX_TIMEOUT_UNSPEC 0x00000000U /* unspecified */
				      /* else can_id */

/* arbitration lost in bit ... / data[0] */
#define CAN_ERR_LOSTARB_UNSPEC   0x00 /* unspecified */
				      /* else bit number in bitstream */

/* error status of CAN-controller / data[1] */
#define CAN_ERR_CRTL_UNSPEC      0x00 /* unspecified */
#define CAN_ERR_CRTL_RX_OVERFLOW 0x01 /* RX buffer overflow */
#define CAN_ERR_CRTL_TX_OVERFLOW 0x02 /* TX buffer overflow */
#define CAN_ERR_CRTL_RX_WARNING  0x04 /* reached warning level for RX errors */
#define CAN_ERR_CRTL_TX_WARNING  0x08 /* reached warning level for TX errors */
#define CAN_ERR_CRTL_RX_PASSIVE  0x10 /* reached error passive status RX */
#define CAN_ERR_CRTL_TX_PASSIVE  0x20 /* reached error passive status TX */
				      /* (at least one error counter exceeds */
				      /* the protocol-defined level of 127)  */

/* error in CAN protocol (type) / data[2] */
#define CAN_ERR_PROT_UNSPEC      0x00 /* unspecified */
#define CAN_ERR_PROT_BIT         0x01 /* single bit error */
#define CAN_ERR_PROT_FORM        0x02 /* frame format error */
#define CAN_ERR_PROT_STUFF       0x04 /* bit stuffing error */
#define CAN_ERR_PROT_BIT0        0x08 /* unable to send dominant bit */
#define CAN_ERR_PROT_BIT1        0x10 /* unable to send recessive bit */
#define CAN_ERR_PROT_OVERLOAD    0x20 /* bus overload */
#define CAN_ERR_PROT_ACTIVE      0x40 /* active error announcement */
#define CAN_ERR_PROT_TX          0x80 /* error occured on transmission */

/* error in CAN protocol (location) / data[3] */
#define CAN_ERR_PROT_LOC_UNSPEC  0x00 /* unspecified */
#define CAN_ERR_PROT_LOC_SOF     0x03 /* start of frame */
#define CAN_ERR_PROT_LOC_ID28_21 0x02 /* ID bits 28 - 21 (SFF: 10 - 3) */
#define CAN_ERR_PROT_LOC_ID20_18 0x06 /* ID bits 20 - 18 (SFF: 2 - 0 )*/
#define CAN_ERR_PROT_LOC_SRTR    0x04 /* substitute RTR (SFF: RTR) */
#define CAN_ERR_PROT_LOC_IDE     0x05 /* identifier extension */
#define CAN_ERR_PROT_LOC_ID17_13 0x07 /* ID bits 17-13 */
#define CAN_ERR_PROT_LOC_ID12_05 0x0F /* ID bits 12-5 */
#define CAN_ERR_PROT_LOC_ID04_00 0x0E /* ID bits 4-0 */
#define CAN_ERR_PROT_LOC_RTR     0x0C /* RTR */
#define CAN_ERR_PROT_LOC_RES1    0x0D /* reserved bit 1 */
#define CAN_ERR_PROT_LOC_RES0    0x09 /* reserved bit 0 */
#define CAN_ERR_PROT_LOC_DLC     0x0B /* data length code */
#define CAN_ERR_PROT_LOC_DATA    0x0A /* data section */
#define CAN_ERR_PROT_LOC_CRC_SEQ 0x08 /* CRC sequence */
#define CAN_ERR_PROT_LOC_CRC_DEL 0x18 /* CRC delimiter */
#define CAN_ERR_PROT_LOC_ACK     0x19 /* ACK slot */
#define CAN_ERR_PROT_LOC_ACK_DEL 0x1B /* ACK delimiter */
#define CAN_ERR_PROT_LOC_EOF     0x1A /* end of frame */
#define CAN_ERR_PROT_LOC_INTERM  0x12 /* intermission */

/* error status of CAN-transceiver / data[4] */
/*                                             CANH CANL */
#define CAN_ERR_TRX_UNSPEC             0x00 /* 0000 0000 */
#define CAN_ERR_TRX_CANH_NO_WIRE       0x04 /* 0000 0100 */
#define CAN_ERR_TRX_CANH_SHORT_TO_BAT  0x05 /* 0000 0101 */
#define CAN_ERR_TRX_CANH_SHORT_TO_VCC  0x06 /* 0000 0110 */
#define CAN_ERR_TRX_CANH_SHORT_TO_GND  0x07 /* 0000 0111 */
#define CAN_ERR_TRX_CANL_NO_WIRE       0x40 /* 0100 0000 */
#define CAN_ERR_TRX_CANL_SHORT_TO_BAT  0x50 /* 0101 0000 */
#define CAN_ERR_TRX_CANL_SHORT_TO_VCC  0x60 /* 0110 0000 */
#define CAN_ERR_TRX_CANL_SHORT_TO_GND  0x70 /* 0111 0000 */
#define CAN_ERR_TRX_CANL_SHORT_TO_CANH 0x80 /* 1000 0000 */

//-----------------------------------------------------------------------------

#endif
