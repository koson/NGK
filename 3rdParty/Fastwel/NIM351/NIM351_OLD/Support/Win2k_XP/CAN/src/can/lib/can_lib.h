/**
 *
 * NAME:    can/can_lib.h
 *
 * DESCRIPTION: Библиотека поддержки драйвера CAN-адаптера
 *
 *
 * AUTHOR:  Валерий Наконечный
 *
 *
 *******************************************************
 *      Copyright (C) 2000-2010 Fastwel Group Co Ltd.
 *      All rights reserved.
 ********************************************************/
#ifndef ___CAN_LIB_H___
#define ___CAN_LIB_H___

#include <can/can.h>

//----------------------------------------------------------------------------------------------------
// Результат выполнения функций библиотеки
typedef enum _F_CAN_RESULT
{
  // Успех
  CAN_RES_OK = 0,
  // Аппаратная ошибка
  CAN_RES_HARDWARE,
  // Недействительный (неправильный) хэндл адаптера
  CAN_RES_INVALID_HANDLE,
  // Некорректный указатель, переданный в качестве параметра
  CAN_RES_INVALID_POINTER,
  // Неправильный параметр, переданный в функцию (один или несколько)
  CAN_RES_INVALID_PARAMETER,
  // Не хватило системных ресурсов
  CAN_RES_INSUFFICIENT_RESOURCES,
  // Не удалось открыть устройство
  CAN_RES_OPEN_DEVICE,
  // Внутренняя ошибка в драйвере устройства
  CAN_RES_UNEXPECTED,
  // Ошибка обращения к драйверу или к устройству
  CAN_RES_FAILURE,
  // Буфер приема пуст
  CAN_RES_RXQUEUE_EMPTY,
  // Операция не поддерживается
  CAN_RES_NOT_SUPPORTED,
  // Таймаут
  CAN_RES_TIMEOUT

} F_CAN_RESULT;

/**
 *  Проверка успешности завершения последней операции
 */
#define F_CAN_SUCCESS(res) (CAN_RES_OK == (res))
//----------------------------------------------------------------------------------------------------


// Идентификатор (индекс) CAN-адаптера. Под адаптером подразумевается
// одно физическое устройство, оканчивающееся соединителем интерфейса CAN.
typedef size_t F_CAN_DEVID;

// Хэндл адаптера
typedef FILE_HANDLE F_CAN_HANDLE, *PF_CAN_HANDLE;

// Структура, определяющая объект для ожидания статуса CAN-адаптера функцией fw_can_wait()
typedef struct _F_CAN_WAIT
{
  // Маска ожидаемого статуса CAN-адаптера (по ИЛИ), включая:
  // CAN_WAIT_RX -- в приемном буфере есть непрочитанное сообщение
  // CAN_WAIT_TX -- в буфер передачи нет неотправленных сообщений 
  // CAN_WAIT_ERR -- с момента последнего вызова fw_can_get_clear_errors() произошла ошибка(и)
  F_CAN_STATUS  waitMask;

  // Маска текущего статуса CAN-адаптера
  F_CAN_STATUS  status;

} F_CAN_WAIT, *PF_CAN_WAIT;

#ifdef __cplusplus
extern "C"
{
#endif

/**
 *  Проверка корректности (правильности) хэндла устройства
 */
  #define fw_can_is_handle_valid(handle) el_handle_valid(handle)

//----------------------------------------------------------------------------------------------------
/** 
 *  Function		 :  F_CAN_RESULT fw_can_init(void)
 *
 *  Parameters	 : 
 *
 *  Return value :  Результат инициализации библиотеки поддержки
 *									CAN_RES_OK    - успех
 *                  CAN_RES_XX    - значение, отличное от CAN_RES_OK, свидетельствует об ошибке
 *
 *  Description	 :  Инициализирует библиотеку поддержки для вызывающего процесса.
 *                  Данная функция должна быть вызвана в начале программы, в которой предполагается
 *                  взаимодействовать с адаптером.
 *                 
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_init(void);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function		 :  F_CAN_RESULT fw_can_open(F_CAN_DEVID id, PF_CAN_HANDLE phDev)
 *
 *  Parameters	 :  id - идентификатор (индекс) адаптера, начиная с 1
 *
 *                  phDev - указатель на переменную типа F_CAN_HANDLE, в которой, при успешном
 *                  открытии адаптера, окажется хэндл устройства. Корректность хэндла можно проверить
 *                  при помощи макроса fw_can_is_handle_valid:
 *                  PF_CAN_HANDLE handle;
 *                  if(CAN_RES_OK == fw_can_open(1, &handle, true) && fw_can_is_handle_valid(handle))
 *                  {
 *                    //устройство с индексом 1 открыто успешно
 *                    fw_can_close(handle);
 *                  }
 *
 *  Return value : 
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description	 :  Функция открывает адаптер с заданным идентификатором (индексом).
 *                 
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_open(F_CAN_DEVID id, PF_CAN_HANDLE phDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function	   :  F_CAN_RESULT fw_can_close(F_CAN_HANDLE hDev)
 *
 *  Parameters	 :  hDev - хэндл закрываемого адаптера
 *
 *  Return value : 
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description	 :  Закрывает хэндл адаптера, ранее открытый при помощи fw_can_open().
 *                  После данного вызова hDev не может использоваться для доступа к адаптеру.
 **/
  FDLL_EXPORT F_CAN_RESULT  fw_can_close(F_CAN_HANDLE hDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_controller_config(F_CAN_HANDLE hDev, F_CAN_SETTINGS pDcb)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *
 *               :  pDcb - указатель на структуру параметров CAN-адаптера
 *
 *  Return value : 
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Возвращает текущие параметры CAN-адаптера.
 **/
   FDLL_EXPORT F_CAN_RESULT fw_can_get_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_set_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *
 *               :  pDcb - указатель на структуру параметров CAN-адаптера
 *
 *  Return value : 
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Задает новые значения параметров CAN-адаптера. Установка параметров
 *                  возможна только, когда адаптер находится в состоянии CAN_STATE_INIT.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_set_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *
 *               :  pTimeouts - указатель на структуру таймаутов CAN-адаптера
 *
 *  Return value : 
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Возвращает текущие значения таймаутов операций чтения, записи и сброса заданного CAN-адаптера.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_get_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_set_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *
 *               :  pTimeouts - указатель на структуру таймаутов CAN-адаптера
 *
 *  Return value : 
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Устанавливает новые значения таймаутов операций чтения, записи и сброса заданного CAN-адаптера.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_set_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_controller_state(F_CAN_HANDLE hDev, PF_CAN_STATE pState)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *
 *               :  pState - указатель на переменную, в которой будет возвращено текущее состояние адаптера,
 *                  включая:
 *                  CAN_STATE_INIT - начальное состояние
 *                  CAN_STATE_ERROR_ACTIVE - кол-во ошибок приема/передачи не более 96
 *                  CAN_STATE_ERROR_WARNING	- кол-во ошибок приема/передачи от 96 до 127
 *                  CAN_STATE_ERROR_PASSIVE -	кол-во ошибок приема/передачи от 128 до 255
 *                  CAN_STATE_BUS_OFF -	кол-во ошибок приема/передачи >= 256 (BUSOFF)
 *                  CAN_STATE_STOPPED - адаптер остановлен
 *                  CAN_STATE_SLEEPING - не используется в текущей версии
 *
 *  Return value : 
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Возвращает текущее состояние CAN-адаптера
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_get_controller_state(F_CAN_HANDLE hDev, PF_CAN_STATE pState);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_stats(F_CAN_HANDLE hDev, PF_CAN_STATS pStats)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *               :  pDcb - указатель на структуру статистической информации CAN-адаптера
 *
 *  Return value : 
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Возвращает статистическую информацию заданного адаптера.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_get_stats(F_CAN_HANDLE hDev, PF_CAN_STATS pStats);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_clear_stats(F_CAN_HANDLE hDev)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Сбрасывает в 0 статистическую информацию заданного адаптера.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_clear_stats(F_CAN_HANDLE hDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_start(F_CAN_HANDLE hDev)
 *
 *  Parameters   :  hDev - хэндл запускаемого адаптера
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Запускает заданный адаптер. При успешном запуске адаптер оказывается
 *                  соединенным с шиной CAN и выходит из нчального состояния CAN_STATE_INIT.
 *                 
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_start(F_CAN_HANDLE hDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_stop(F_CAN_HANDLE hDev)
 *
 *  Parameters   :  hDev - хэндл останавливаемого адаптера
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Останавливает заданный адаптер. При успешном останове адаптер отсоединяется
 *                  от шины CAN. После завершения функции адаптер пребывает в состоянии CAN_STATE_INIT.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_stop(F_CAN_HANDLE hDev);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_send(F_CAN_HANDLE hDev, PF_CAN_TX pTx, size_t nTx, size_t* nSend)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *                  pTx - указатель на буфер передаваемых сообщений
 *                  nTx - количество сообщений в буфере pTx
 *                  nSend - указатель на переменную, в которую будет помещено количество реально переданных
 *                  сообщений при возврате из данной функции.
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Передает сообщения в шину CAN.
 *                  Функция блокируется до тех пор, пока сообщения не будут переданы, либо пока
 *                  не произойдет таймаут передачи. Таймаут передачи должен быть предварительно задан
 *                  при помощи fw_can_set_timeouts().
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_send(F_CAN_HANDLE hDev, PF_CAN_TX pTx, size_t nTx, size_t* nSend);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* nRecv)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *                  pRx - указатель на буфер приема в приложении
 *                  nRx - емкость буфера приема
 *                  nRecv - указатель на переменную, в которую будет помещено количество принятых сообщений,
 *                  находящихся в буфере pRx при возврате из данной функции.
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  При вызове данной функции происходит считывание имеющихся сообщений во внутреннем буфере
 *                  драйвера для данного адаптера. Если внутренний буфер пуст, функция блокируется и ожидает 
 *                  приема хотя бы одного сообщения или таймаута приема. Таймаут приема должен быть предварительно 
 *                  задан при помощи fw_can_set_timeouts().
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* nRecv);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_peek_message(F_CAN_HANDLE hDev, PF_CAN_RX pRx)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *                  pRx - указатель на переменную F_CAN_RX, в которую будет помещено сообщение
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *                  CAN_RES_RXQUEUE_EMPTY - нет сообщений во внутреннем буфере приема драйвера для
 *                  данного адаптера.
 *
 *  Description  :  При вызове данной функции происходит считывание одного сообщения из внутреннего
 *                  буфера приема. Функция не блокируется и немедленно завершается при любом состоянии буфера.
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_peek_message(F_CAN_HANDLE hDev, PF_CAN_RX pRx);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_post_message(F_CAN_HANDLE hDev, PF_CAN_TX pTx)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *                  pTx - указатель на переменную-сообщение, подлежащее передаче
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *                  CAN_RES_TXQUEUE_FULL - переполнение буфера передачи.
 *
 *  Description  :  Функция помещает сообщение во внутренний буфер передачи адаптера.
 *                  Функция не блокируется и не ожидает завершения передачи сообщения.
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_post_message(F_CAN_HANDLE hDev, PF_CAN_TX pTx);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_purge(F_CAN_HANDLE hDev, F_CAN_PURGE_MASK flags)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *                  flags - флаги сброса/прерывания текущей операции над адаптером
 *                  CAN_PURGE_TXABORT - прерывает отложенные обращения к адаптеру по записи.
 *                  CAN_PURGE_TXCLEAR - сбрасывает в адаптере текущий запрос на передачу.                                      и 
 *                  CAN_PURGE_RXCLEAR - очищает внутренний буфер приема.
 *                  CAN_PURGE_RXABORT - прерывает отложенные обращения к адаптеру по чтению.
 *                  CAN_PURGE_HWRESET - выполняет аппаратный сброс адаптера. При этом сбрасываются
 *                                      счетчики ошибок адаптера. Настройки адаптера сохраняются.
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Функция, в зависимости от значения flags, очищает буферы приема и/или передачи и/или
 *                  выполняет аппаратный сброс адаптера. 
 *                  Кроме того, позволяет прервать текущие/отложенные обращения к адаптеру по чтению/записи.
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_purge(F_CAN_HANDLE hDev, F_CAN_PURGE_MASK flags);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_wait(F_CAN_HANDLE hDev, PF_CAN_WAIT pWait, size_t msTimeout)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *                  pWait - указатель на объект, содержащий маску ожидаемого статуса CAN-адаптера pWait->waitMask
 *                  и приемник текущего значения статуса.
 *                  msTimeout - таймаут ожидания в миллисекундах.
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *								  CAN_RES_TIMEOUT - таймаут, текущий статус не соответствует ожидаемому, 
 *                  В случаях CAN_RES_OK и CAN_RES_TIMEOUT в поле pWait->status будет записано значение текущего статуса.
 *                  Значение, отличное от CAN_RES_OK и CAN_RES_TIMEOUT, свидетельствует об ошибке вызова функции.
 *
 *  Description  :  Функция блокирует выполнение вызывающего потока до тех пор, пока статус CAN-адаптера не 
 *                  будет соответствовать одному из значений, определенных маской pWait->waitMask, или 
 *                  не истечет таймаут заданный параметром msTimeout. При завершении данной функции с кодом 
 *                  CAN_RES_OK или CAN_RES_TIMEOUT поле pWait->status будет содержать текущее значения статуса.
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_wait(F_CAN_HANDLE hDev, PF_CAN_WAIT pWait, size_t msTimeout);

//----------------------------------------------------------------------------------------------------
/** 
 *  Function     :  F_CAN_RESULT fw_can_get_clear_errors(F_CAN_HANDLE hDev, PF_CAN_ERRORS pErrors)
 *
 *  Parameters   :  hDev - хэндл адаптера
 *                  pErrors - указатель на объект, в котором будут возвращены счетчики ошибок.
 *                  Если в качестве pErrors передать NULL, произойдет сброс счетчиков ошибок для
 *                  данного адаптера.
 *
 *  Return value :
 *								  CAN_RES_OK - успех;
 *                  значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
 *
 *  Description  :  Функция помещает значения счетчиков ошибок данного адаптера в
 *                  объект pErrors и сбрасывает счетчики. Если в качестве pErrors передать NULL, 
 *                  будет произведен сброс счетчиков ошибок для данного адаптера.
 *
 **/
  FDLL_EXPORT F_CAN_RESULT fw_can_get_clear_errors(F_CAN_HANDLE hDev, PF_CAN_ERRORS pErrors);

#ifdef __cplusplus
}
#endif

#endif