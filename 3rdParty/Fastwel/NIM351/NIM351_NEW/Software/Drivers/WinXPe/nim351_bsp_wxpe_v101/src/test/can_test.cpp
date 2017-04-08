/*
*
* NAME:    can_test.cpp
*
* DESCRIPTION: CAN Controller Driver Test
*
*
* AUTHOR:  Valeriy Nakonechniy
*
*
*******************************************************
*      Copyright (C) 2000-2009 Fastwel Co Ltd.
*      All rights reserved.
********************************************************/

#ifdef __CAN_TEST__

#include <can/lib/can_lib.h>

int baude_rate = 250;

//----------------------------------------------------------------------------------------------------
// Возвращает текущюю установку частоты передачи
static u32 get_baud_rate()
{
  switch(baude_rate)
  {
    case 1000: return CANBR_1MBaud;    // 1 MBit/sec
    case 800:  return CANBR_800kBaud;  // 800 kBit/sec
    case 500:  return CANBR_500kBaud;  // 500 kBit/sec
    case 250:  return CANBR_250kBaud;  // 250 kBit/sec
    case 125:  return CANBR_125kBaud;  // 125 kBit/sec
    case 100:  return CANBR_100kBaud;  // 100 kBit/sec
    case 50:   return CANBR_50kBaud;   // 50 kBit/sec
    case 20:   return CANBR_20kBaud;   // 20 kBit/sec
    case 10:   return CANBR_10kBaud;   // 10 kBit/sec
  }
  return CANBR_250kBaud;
}

//----------------------------------------------------------------------------------------------------
// Некий генератор случайного CAN-ID. Используется для генерации сообщения в тестах.
static u32 rand_canid(void)
{
  // pseudorandom id: 0 <= id <= (RAND_MAX)
  u32 id = rand();

  if((id & 0xC00)==0xC00)
  {
    // Кадрам с такой маской судьба иметь расширенный идентификатор
    id = ((((u32)rand()) << 15) | (u32)(rand())) & CAN_EFF_MASK;
  }
  else
  {
    // Иначе стандартный
    id &= CAN_SFF_MASK;
  }
  return id;
}

//----------------------------------------------------------------------------------------------------
// Некий генератор сообщения по CAN-ID. Используется для генерации сообщения в тестах.
static void define_frame(PF_CAN_MSG pFrame, u32 id)
{
  int i;

  pFrame->can_id = id & CAN_EFF_MASK;
  pFrame->can_dlc = pFrame->can_id % 9;

  if(pFrame->can_id & (~CAN_SFF_MASK))
    pFrame->can_id |= CAN_EFF_FLAG;

  // RTR
  if((id & 0x111) == 0x111)
  {
    pFrame->can_id |= CAN_RTR_FLAG;
    pFrame->can_dlc = 0;
  }

  for(i = 0; i < pFrame->can_dlc; i++)
    pFrame->data[i] = (u8)(pFrame->can_id>>i);
}

//----------------------------------------------------------------------------------------------------
// Сравнивает сообщения.
static bool compare_frames(PF_CAN_MSG fr1, PF_CAN_MSG fr2)
{
  int j;

  if(fr1->can_id != fr2->can_id)
    return false;

  if(fr1->can_dlc != fr2->can_dlc)
    return false;

  for(j = 0; j < fr1->can_dlc && j < 8; j++)
  {
    if(fr1->data[j] != fr2->data[j])
      return false;
  }
  return true;
}

//----------------------------------------------------------------------------------------------------
// Получает и печатает статистику адаптера.
static void printf_io_stats(FILE_HANDLE h)
{
  F_CAN_STATS stats;
  F_CAN_RESULT fwcRes;

  fwcRes = fw_can_get_stats(h, &stats);
  if(F_CAN_SUCCESS(fwcRes))
  {
    F_INFO("\n--IO STATISTICS--\n");
    F_INFO4("RX: Packets(%lu), Bytes(%lu), Overrun(%lu), Errors(%lu)\n", 
      stats.rx_packets, stats.rx_bytes, stats.rx_over_errors, stats.rx_errors);
    F_INFO4("TX: Packets(%lu), Bytes(%lu), Overrun(%lu), Errors(%lu)\n", 
      stats.tx_packets, stats.tx_bytes, stats.tx_over_errors, stats.tx_errors);
    F_INFO5("ERR: Bus(%lu), Warning(%lu), Passive(%lu), Off(%lu), Arbitration(%lu)\n", 
      stats.bus_error, stats.error_warning, stats.error_passive, stats.bus_off, stats.arbitration_lost);
    F_INFO1("Restarts(%lu)\n", stats.restarts);
  }
  else
  {
    F_INFO("Failed to retrive IO statistics\n");
  }
}

//static void printf_current_settings(FILE_HANDLE h)
//{
//  F_CAN_SETTINGS settings;
//  F_CAN_RESULT fwcRes;
//
//  fwcRes = fw_can_get_controller_config(h, &settings);
//  if(F_CAN_SUCCESS(fwcRes))
//  {
//    F_INFO("\n--CURRENT CONTROLLER SETTINGS--\n");
//    F_INFO2("Baudrate(%d), OperationMode(%d)\n", settings.baud_rate, settings.opmode);
//  }
//  else
//  {
//    F_INFO("Failed to retrive controller settings\n");
//  }
//}
//
//----------------------------------------------------------------------------------------------------
// Печатает значения структуры F_CAN_SETTINGS.
static void printf_controller_config(PF_CAN_SETTINGS pDcb)
{
  if(!F_CANBR_isUser(pDcb->baud_rate))
  {
    switch(pDcb->baud_rate)
    {
    case CANBR_1MBaud:
      F_INFO("1MBaud; ");
      break;
    case CANBR_800kBaud:
      F_INFO("800kBaud; ");
      break;
    case CANBR_500kBaud:
      F_INFO("500kBaud; ");
      break;
    case CANBR_250kBaud:
      F_INFO("250kBaud; ");
      break;
    case CANBR_125kBaud:
      F_INFO("125kBaud; ");
      break;
    case CANBR_100kBaud:
      F_INFO("100kBaud; ");
      break;
    case CANBR_50kBaud:
      F_INFO("50kBaud; ");
      break;
    case CANBR_20kBaud:
      F_INFO("20kBaud; ");
      break;
    case CANBR_10kBaud:
      F_INFO("10kBaud; ");
      break;
    default:
      F_INFO("???Baud; ");
    }
  }
  else
  {
    F_INFO2("User Baud (btr0=%02X, btr1=%02X): ", 
      F_CANBR_getUserBtr0(pDcb->baud_rate), F_CANBR_getUserBtr1(pDcb->baud_rate));
  }

  F_INFO6("%cStandard%cExtended%cError%cLstonly%cSelftest%cSelfrecv\n",
    (pDcb->opmode&CAN_OPMODE_STANDARD)?'+':'-',
    (pDcb->opmode&CAN_OPMODE_EXTENDED)?'+':'-',
    (pDcb->opmode&CAN_OPMODE_ERRFRAME)?'+':'-',
    (pDcb->opmode&CAN_OPMODE_LSTNONLY)?'+':'-',
    (pDcb->opmode&CAN_OPMODE_SELFTEST)?'+':'-',
    (pDcb->opmode&CAN_OPMODE_SELFRECV)?'+':'-'
    );
}

//----------------------------------------------------------------------------------------------------
// Печатает значения структуры F_CAN_TIMEOUTS.
static void printf_timeouts_config(PF_CAN_TIMEOUTS pTimeouts)
{
  F_INFO("\n--TIMEOUTS CONFIGURATION--\n");

  F_INFO4("Write(const=%lu, mult=%lu), Read(%lu), RecoverBusoff(%lu)",
    pTimeouts->WriteTotalTimeoutConstant, 
    pTimeouts->WriteTotalTimeoutMultiplier,
    pTimeouts->ReadTotalTimeout,
    pTimeouts->RestartBusoffTimeout);
}

//----------------------------------------------------------------------------------------------------
// Тест автономного тестирование адаптера (без подключения к сети).
// Используется режим в котором адаптер передает и одновременно принимает передаваемое сообщение.
static void io_test_self(FILE_HANDLE h)
{
  // результат
  bool res;
  // статус завершения операции
  F_CAN_RESULT fwcRes;
#define szArr 50
  // буфер передавемых сообщений
  F_CAN_TX trArr[szArr];
  // буфер принимаемых сообщений
  F_CAN_RX rcvArr[szArr];
  int i, try_count;

  F_INFO("\t--- SELF_MODE_IO TEST ---\n");

  // Определяем сообщения для передачи
  for(i = 0; i < szArr; i++)
  {
    // "Случайный" CAN-ID
    u32 id = rand_canid();
    // формируем сообщение генератором
    define_frame(&trArr[i].msg, id);
  }

  int i_tr = 0; // текущий индекс в буфере принимаемых сообщений
  for(try_count = 0; (i_tr < szArr) && (try_count < 10); try_count++)
  {
    // Определяем число сообщений для текущей операции отправки
    int n_tr = (i_tr == 0)? 1: 20;
    if((i_tr + n_tr) > szArr)
      n_tr = szArr - i_tr;

    // приемник числа отправленных сообщений
    size_t written = 0;

    F_INFO1("\tWrite(%d): ", n_tr);
    // Отправляем порцию сообщений
    fwcRes = fw_can_send(h, &trArr[i_tr], n_tr, &written);
    // Проверяем статус завершения операции
    if(F_CAN_SUCCESS(fwcRes))
    {
      i_tr += written;
      if(written == 0)
      {
        F_INFO("Timeout.\n");
      }
      else
      {
        F_INFO1("Written(%d)\n", written);
      }
    }
    else
    {
      F_INFO("FAILURE.\n");
    }
  }


  int i_rcv = 0;  // текущий индекс в буфере принимаемых сообщений
  for(try_count = 0; (i_rcv < i_tr) && (try_count < 10); try_count++)
  {
    // Определяем число сообщений для текущей операции чтения
    int n_rcv = (i_rcv == 0)? 5: 10;
    if((i_rcv + n_rcv) > i_tr)
      n_rcv = i_tr - i_rcv;

    // приемник числа считанных сообщений
    size_t received = 0;
    F_INFO1("\tRead(%d): ", n_rcv);
    // Читаем
    fwcRes = fw_can_recv(h, &rcvArr[i_rcv], n_rcv, &received);
    // Проверяем статус завершения операции
    if(F_CAN_SUCCESS(fwcRes))
    {
      i_rcv += received;
      if(received == 0)
      {
        F_INFO("Timeout.\n");
      }
      else
      {
        F_INFO1("Received(%d).\n", received);
      }
    }
    else
    {
      F_INFO("FAILURE.\n");
    }
  }

  // Проверяем на соответствие принятого отправленному
  res = true;
  for(i = 0; (i < i_tr) && (i < i_rcv) && res; i++)
    res = compare_frames(&trArr[i].msg, &rcvArr[i].msg);

  F_INFO3("\tIO_TEST result: Transmitted(%d) %s Received(%d)\n", i_tr, res? "==": "!=", i_rcv);
}
//----------------------------------------------------------------------------------------------------
// Получает хэндл и запускает адаптер
static bool io_test_open_adapter(int port, PF_CAN_HANDLE pHandle)
{
  // результат
  bool blRes = false;
  // статус завершения операции
  F_CAN_RESULT fwcRes;

  do
  {
    // Получаем хэндл адаптера. 
    // Устройство открывается для монопольного использования данным процессом.
    fwcRes = fw_can_open(port, pHandle);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO1("  fw_can_open() failed (%d)\n", fwcRes);
      break;
    }
    
    blRes = true;
  }
  while(0);

  if(!blRes)
  {
    // Ошибка. Освобождаем хэндл адаптера.
    if(fw_can_is_handle_valid(*pHandle))
    {
      fwcRes = fw_can_close(*pHandle);
      if(!F_CAN_SUCCESS(fwcRes))
      {
        F_INFO1("  fw_can_close() failed (%d)\n", fwcRes);
      }
    }
  }

  return blRes;
}

//----------------------------------------------------------------------------------------------------
// Конфигурирует адаптер для тестов чтения и записи сообщений.
static bool io_read_write_test_config(F_CAN_HANDLE h)
{
  // результат
  bool blRes = false;

  do
  {
    // объект структуры параметров CAN-адаптера
    F_CAN_SETTINGS settings;
    // объект структуры таймаутов CAN-адаптера
    F_CAN_TIMEOUTS timeouts;
    // статус завершения операции
    F_CAN_RESULT fwcRes;

    // Устанавливаем требуемый режим работы адаптера
    F_INFO("\n1. Configuration:\n");
    // 1. Получаем текущие настройки
    fwcRes = fw_can_get_controller_config(h, &settings);
    // Проверяем статус завершения операции
    if(F_CAN_SUCCESS(fwcRes))
    {
      F_INFO("\n--CURRENT CONTROLLER SETTINGS--\n");
      // Печатаем текущие настройки
      printf_controller_config(&settings);
    }
    else
    {
      F_INFO1("  fw_can_get_controller_config() failed (%d)\n", fwcRes);
      break;
    }

    F_INFO("\n--DESIRED CONTROLLER SETTINGS--\n");
    // скорость обмена
    settings.baud_rate = get_baud_rate();//CANBR_Btr0Btr1(0x00, 0x1c);
    settings.error_mask = 0;
    // принимаем кадры стандартного и расширенного формата
    settings.opmode = CAN_OPMODE_STANDARD|CAN_OPMODE_EXTENDED;
    // Печатаем желаемые настройки
    printf_controller_config(&settings);
    // 2. Устанавливаем новые настройки
    fwcRes = fw_can_set_controller_config(h, &settings);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO1("  fw_can_set_controller_config() failed (%d)\n", fwcRes);
      break;
    }

    // 3. Устанавливает таймауты операций приема и передачи сообщений
    timeouts.ReadTotalTimeout = 1000;             // прием - 1 с
    timeouts.WriteTotalTimeoutConstant = 0;
    timeouts.WriteTotalTimeoutMultiplier = 1000;  // передача - 1 с на сообщение
    timeouts.RestartBusoffTimeout = 500;          // авто рестарт из bus-off - 500 мс
    fwcRes = fw_can_set_timeouts(h, &timeouts);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO1("  fw_can_set_timeouts() failed (%d)\n", fwcRes);
      break;
    }

    // Запускаем адаптер.
    fwcRes = fw_can_start(h);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO1("  fw_can_start() failed (%d)\n", fwcRes);
      break;
    }

    blRes = true;
  }
  while(0);

  return blRes;
}

//----------------------------------------------------------------------------------------------------
// Конфигурирует адаптер для автономного тестирования (без подключения к сети).
// Устанавливается режим в котором адаптер передает и одновременно принимает передаваемое сообщение.
static bool io_self_test_config(F_CAN_HANDLE h)
{
  // результат
  bool blRes = false;

  do
  {
    // объект структуры параметров CAN-адаптера
    F_CAN_SETTINGS settings;
    // объект структуры таймаутов CAN-адаптера
    F_CAN_TIMEOUTS timeouts;
    // статус завершения операции
    F_CAN_RESULT fwcRes;

    // Устанавливаем требуемый режим работы адаптера
    F_INFO("Set self test mode: ");
    // 1. Получаем текущие настройки
    fwcRes = fw_can_get_controller_config(h, &settings);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO("Failed to get settings\n");
      break;
    }
    // 2. Изменяем только режим работы адаптера. Остальные параметры оставляем без изменения.
    // принимаем кадры стандартного и расширенного формата
    // передача, считается выполненной и при отсутствии подтверждения на шине
    // адаптер принимет свои передаваемые сообщения (прием осуществляется одновременно с  передачей)
    settings.opmode = CAN_OPMODE_STANDARD|CAN_OPMODE_EXTENDED|CAN_OPMODE_SELFTEST|CAN_OPMODE_SELFRECV;
    fwcRes = fw_can_set_controller_config(h, &settings);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO("Failed to update settings\n");
      break;
    }
    F_INFO("success\n");

    // 3. Устанавливает таймауты операций приема и передачи сообщений
    timeouts.WriteTotalTimeoutConstant = 1000;  // передача - 1 с
    timeouts.WriteTotalTimeoutMultiplier = 0;
    timeouts.ReadTotalTimeout = 1000;           // прием - 1 с
    timeouts.RestartBusoffTimeout = 0;
    F_INFO2("Set timeouts for read(%d) and write(%d) operations: ", timeouts.ReadTotalTimeout, timeouts.WriteTotalTimeoutConstant);
    fwcRes = fw_can_set_timeouts(h, &timeouts);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO("Failed to update timeouts\n");
      break;
    }

    // Запускаем адаптер.
    fwcRes = fw_can_start(h);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO1("  fw_can_start() failed (%d)\n", fwcRes);
      break;
    }

    F_INFO("success\n");

    blRes = true;
  }
  while(0);

  return blRes;
}
//----------------------------------------------------------------------------------------------------
#define CLIENT_MSG_ID   0x200
#define SERVER_MSG_ID   0x300
#define TRANSACTION_TIMEOUT 1000  //ms
//----------------------------------------------------------------------------------------------------
// Конфигурирует адаптер для эхо-клиента и эхо-сервера.
static bool io_echo_config(F_CAN_HANDLE h, bool server)
{
  // результат
  bool blRes = false;

  do
  {
    // объект структуры параметров CAN-адаптера
    F_CAN_SETTINGS settings;
    // объект структуры таймаутов CAN-адаптера
    F_CAN_TIMEOUTS timeouts;
    // статус завершения операции
    F_CAN_RESULT fwcRes;

    // Устанавливаем требуемый режим работы адаптера
    F_INFO("\n1. Configuration:\n");
    // 1. Получаем текущие настройки
    fwcRes = fw_can_get_controller_config(h, &settings);
    // Проверяем статус завершения операции
    if(F_CAN_SUCCESS(fwcRes))
    {
      F_INFO("\n--CURRENT CONTROLLER SETTINGS--\n");
      // Печатаем текущие настройки
      printf_controller_config(&settings);
    }
    else
    {
      F_INFO1("  fw_can_get_controller_config() failed (%d)\n", fwcRes);
      break;
    }

    F_INFO("\n--DESIRED CONTROLLER SETTINGS--\n");
    // скорость обмена
    settings.baud_rate = get_baud_rate();
    // принимаем только кадры стандартного формата
    settings.opmode = CAN_OPMODE_STANDARD;
    // Настроим приемный фильтр только на "свои" сообщения
    settings.acceptance_code = (server)? CLIENT_MSG_ID << 1 : SERVER_MSG_ID << 1;
    settings.acceptance_mask = (CAN_SFF_MASK << 1) | 1; // все позиции релевантны
    // Печатаем желаемые настройки
    printf_controller_config(&settings);
    // 2. Устанавливаем новые настройки
    fwcRes = fw_can_set_controller_config(h, &settings);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO1("  fw_can_set_controller_config() failed (%d)\n", fwcRes);
      break;
    }

    // 3. Устанавливает таймауты операций приема и передачи сообщений
    timeouts.ReadTotalTimeout = 0;                // прием - таймаут не используется
    timeouts.WriteTotalTimeoutConstant = 0;
    timeouts.WriteTotalTimeoutMultiplier = 0;     // передача - таймаут не используется
    timeouts.RestartBusoffTimeout = 0;            // авто рестарт из bus-off - нет
    fwcRes = fw_can_set_timeouts(h, &timeouts);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO1("  fw_can_set_timeouts() failed (%d)\n", fwcRes);
      break;
    }

    // Запускаем адаптер.
    fwcRes = fw_can_start(h);
    // Проверяем статус завершения операции
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO1("  fw_can_start() failed (%d)\n", fwcRes);
      break;
    }

    blRes = true;
  }
  while(0);

  return blRes;
}
//----------------------------------------------------------------------------------------------------
// Отвечает на сообщения, посылаемые io_echo_client()
static void io_echo_server(int port)
{
  enum echo_server_state
  {
    SERVER_RX_STATE,
    SERVER_TX_STATE
  } srv_state;

  do
  {
    // хэндл адаптера
    F_CAN_HANDLE h;
    // статус завершения операции
    F_CAN_RESULT fwcRes;
    
    u32 n_wt = 0; 
    u32 n_rx = 0;
    u32 n_tx = 0;
    u32 n_time = 0; 
    u32 n_ovr = 0;
    u32 n_bpsv = 0;
    u32 n_boff = 0;

    const u32 client_msg_id = 0x200;
    const u32 server_msg_id = 0x300;

    // Получаем хендл адаптера и запускаем устройство
    if(!io_test_open_adapter(port, &h))
      break;
    // Конфигурируем адаптер.
    if(!io_echo_config(h, true))
      break;

    srv_state = SERVER_RX_STATE;

    // Пока не получена команда выхода (нажатие любой клавиши)
    while(1)
    {
      // Объект для принимаемого сообщения
      F_CAN_RX rx;
      // Объект для отправляемого сообщения
      F_CAN_TX tx;
      // Объект ожидания события
      F_CAN_WAIT wt;

      // Вывод на экран результатов теста
      F_INFO7("\r(%lu): Rx(%lu), Tx(%lu), Tx_tm(%u), Rx_ov(%u), Bus_psv(%u), Bus_off(%u)", 
        n_wt, n_rx, n_tx, n_time, n_ovr, n_bpsv, n_boff);

      if(_kbhit())
        break;

      // Определяем маску интересующего нас события (пришло сообщение + произошла ошибка)
      wt.waitMask = CAN_WAIT_RX | CAN_WAIT_ERR;
      // Становимся на ожидание события (таймаут 5000 мс)
      fwcRes = fw_can_wait(h, &wt, 5000);
      
      n_wt++;
      
      if(F_CAN_SUCCESS(fwcRes))
      {
        // Операция завершена успешно. Контроллер имеет ожидаемый статус.
        // Анализируем его.

        if(wt.status & CAN_STATUS_RXBUF)
        {
          // Пришло сообщение. Читаем его функцией fw_can_peek_message()
          if(F_CAN_SUCCESS(fw_can_peek_message(h, &rx)))
          {
            // Отвечаем клиенту
            if(rx.msg.can_id == client_msg_id)
            {
              n_rx++;
              // Отправляем сообщение с идентификатором сервера и теми же данными (эхо)
              tx.msg = rx.msg;
              tx.msg.can_id = server_msg_id;
              if(F_CAN_SUCCESS(fw_can_post_message(h, &tx)))
              {
                srv_state = SERVER_TX_STATE;
                n_tx++;
              }
            }
          }
        }
        
        if(wt.status & CAN_STATUS_ERR)
        {
          // Произошла ошибка
          F_CAN_ERRORS errs;
          if(F_CAN_SUCCESS(fw_can_get_clear_errors(h, &errs)))
          {
            n_time += errs.tx_timeout;
            n_ovr += errs.data_overrun;
            n_bpsv += errs.error_passive;
            n_boff += errs.bus_off;
          }
        }
      }
      else if(fwcRes == CAN_RES_TIMEOUT)
      {
        // Timeout
      }
      else
      {
        F_INFO1("\nfw_can_wait() failed %d\n", fwcRes);
        el_sleep(2000);
      }
    }
    fw_can_close(h);
  }
  while(0);
}
//----------------------------------------------------------------------------------------------------
// Посылает сообщения, которые будут приняты io_echo_server()
static void io_echo_client(int port)
{
  enum echo_server_state
  {
    CLIENT_TX_STATE,
    CLIENT_RX_STATE
  } client_state;

  do
  {
    // хэндл адаптера
    F_CAN_HANDLE h;
    // статус завершения операции
    F_CAN_RESULT fwcRes;
    // Счетчики
    u32 n_tr = 0;
    u32 n_tr_ok = 0;
    u32 n_err_time = 0;
    u32 n_err_ovr = 0;
    u32 n_err_bpsv = 0;
    u32 n_err_boff=0;
    // Метки времени для определения таймаутов ожидания
    u32 expire_time, cur_time;
    // Объект для принимаемого сообщения
    F_CAN_RX rx;
    // Объект для отправляемого сообщения
    F_CAN_TX tx;
    int j;

    const u32 client_msg_id = CLIENT_MSG_ID;
    const u32 server_msg_id = SERVER_MSG_ID;

    // Получаем хендл адаптера и запускаем устройство
    if(!io_test_open_adapter(port, &h))
      break;
    // Конфигурируем адаптер.
    if(!io_echo_config(h, false))
      break;

    client_state = CLIENT_TX_STATE;

    // Пока не получена команда выхода (нажатие любой клавиши)
    while(1)
    {
      // Объект ожидания события
      F_CAN_WAIT wt;
      // Флаги сброса адаптера
      F_CAN_PURGE_MASK purge_mask = 0;
      // Таймаут для операции ожидания
      size_t wt_tout;

      // Вывод на экран результатов теста
      F_INFO6("\rTr[%lu, Ok(%lu)], ERR[tm(%lu), ov(%lu), bpsv(%lu), boff(%lu)", 
        n_tr, n_tr_ok, n_err_time, n_err_ovr, n_err_bpsv, n_err_boff);

      if(_kbhit())
        break;

      // Определяем маску интересующего нас статуса и таймаут ожидания
      if(client_state == CLIENT_TX_STATE)
      {
        wt.waitMask = CAN_WAIT_TX | CAN_WAIT_ERR;
        wt_tout = TRANSACTION_TIMEOUT;
      }
      else
      {
        wt.waitMask = CAN_WAIT_RX | CAN_WAIT_ERR;
        cur_time = el_get_tick_count();
        
        if(F_TIME_isExpired(cur_time, expire_time))
          wt_tout = 0;
        else
          wt_tout = expire_time - cur_time; 
      }

      // Становимся на ожидание
      fwcRes = fw_can_wait(h, &wt, wt_tout);
      
      if(F_CAN_SUCCESS(fwcRes))
      {
        // Операция завершена успешно. Контроллер имеет ожидаемый статус.
        // Анализируем его.

        if(wt.status & CAN_STATUS_ERR)
        {
          // Зарегестрирована ошибка
          F_CAN_ERRORS errs;
          if(F_CAN_SUCCESS(fw_can_get_clear_errors(h, &errs)))
          {
            n_err_time += errs.tx_timeout;
            n_err_ovr += errs.data_overrun;
            n_err_bpsv += errs.error_passive;
            n_err_boff += errs.bus_off;
          }
        }

        if(wt.status & CAN_STATUS_RXBUF)
        {
          // Пришло сообщение. Читаем его функцией fw_can_peek_message()
          if(F_CAN_SUCCESS(fw_can_peek_message(h, &rx)))
          {
            // Если это ответ сервера, завершаем транзакцию
            if((client_state == CLIENT_RX_STATE) && (rx.msg.can_id == server_msg_id))
            {
              bool transaction_ok = true;

              if(rx.msg.can_dlc != tx.msg.can_dlc)
                transaction_ok = false;

              for(j = 0; transaction_ok && j < tx.msg.can_dlc; j++)
              {
                if(rx.msg.data[j] != tx.msg.data[j])
                  transaction_ok = false;
              }

              client_state = CLIENT_TX_STATE;
              if(transaction_ok)
                n_tr_ok++;
            }
          }
        }

        if(wt.status & CAN_STATUS_TXBUF)
        {
          // Можно передавать
          if(client_state == CLIENT_TX_STATE)
          {
            // Определяем идентификатор сообщения
            tx.msg.can_id = client_msg_id;
            // Случайное кол-во данных в кадре (от 0 до 8)
            tx.msg.can_dlc = (u8)(rand() % 9);
            // Случайные данные
            for(j = 0; j < tx.msg.can_dlc; j++)
              tx.msg.data[j] = (u8)rand();

            // Инициируем передачу сообщения
            if(F_CAN_SUCCESS(fw_can_post_message(h, &tx)))
            {
              // Увеличиваем счетчик инициированных транзакций
              n_tr++;
              // Определяеем предельное время выполнения транзакции
              expire_time = el_get_tick_count() + TRANSACTION_TIMEOUT;
              // Меняем состояние клиента
              client_state = CLIENT_RX_STATE;
            }
          }
        }
      }
      else if(fwcRes == CAN_RES_TIMEOUT)
      {
        // Произошел таймаут
        // Полученный статус CAN-адаптера действителен!
        if(!(wt.status & CAN_WAIT_TX))
        {
          // Запрос так и не отправлен. Очистим передатчик.
          purge_mask |= (CAN_PURGE_TXABORT | CAN_PURGE_TXCLEAR);
          n_err_time++;
        }

        client_state = CLIENT_TX_STATE;
      }
      else
      {
        F_CAN_STATE adapter_state;

        // Ошибка nfw_can_wait()
        F_INFO1("\nfw_can_wait() failed %d\n", fwcRes);

        // Получим текущее состояние CAN-адаптера
        fwcRes = fw_can_get_controller_state(h, &adapter_state);
        
        if(F_CAN_SUCCESS(fwcRes))
        {
          F_INFO1("Controller state %d\n", adapter_state);
          if(adapter_state == CAN_STATE_BUS_OFF || adapter_state == CAN_STATE_STOPPED)
          {
            // Перезапустим адаптер + сбросим текущую передачу
            F_INFO("Reset Controller\n");
            purge_mask |= (CAN_PURGE_HWRESET | CAN_PURGE_TXABORT | CAN_PURGE_TXCLEAR);
          }
        }
        else
        {
          // Тушим свет
          F_INFO1("\nfw_can_get_controller_state() failed %d. Abort test.\n", fwcRes);
          break;
        }

        client_state = CLIENT_TX_STATE;
      }

      // Сбросим адаптер (если требуется)
      if(purge_mask)
      {
        fwcRes = fw_can_purge(h, purge_mask);
        
        if(!F_CAN_SUCCESS(fwcRes))
        {
          // Тушим свет
          F_INFO2("\nfw_can_purge(0x%02X) failed %d. Abort test.\n", purge_mask, fwcRes);
          break;
        }

        if(purge_mask & CAN_PURGE_HWRESET)
        {
          el_sleep(1000);
        }
      }
    }
    fw_can_close(h);
  }
  while(0);
}
//----------------------------------------------------------------------------------------------------
#define SFTST_START_PORT  1
#define SFTST_END_PORT    8
// Запускает тест автономного тестирование адаптера (без подключения к сети).
// Если адаптер не определен (port==0), тестируются адаптеры с индексами от SFTST_START_PORT до SFTST_END_PORT
// Используется режим, в котором адаптер передает и одновременно принимает передаваемое сообщение.
static void io_test_self_start(int port)
{
  int start_port;
  int end_port;
  int i;

  if(port != 0)
  {
    start_port = end_port = port;
  }
  else
  {
    start_port = SFTST_START_PORT;
    end_port = SFTST_END_PORT;
  }

  for(i = start_port; i <= end_port; i++)
  {
    // хэндл адаптера
    FILE_HANDLE h;

    // Получаем хендл адаптера и запускаем устройство
    if(io_test_open_adapter(i, &h))
    {
      F_INFO1("open can adapter %d: success\n", i);
      do
      {
        // Конфигурируем адаптер.
        if(!io_self_test_config(h))
          break;
        // Тестируем адаптер
        io_test_self(h);
        // Выводим статистику
        printf_io_stats(h);
      }
      while(0);
      // Закрываем хэндл адаптера
      fw_can_close(h);
    }
    else
    {
      F_INFO1("open can adapter %d: FAILURE!\n", i);
    }
  }
}
//----------------------------------------------------------------------------------------------------
// Читает принятые из сети CAN сообщения.
// Проводит проверку принятых сообщений на соответствие с формируемыми генератором define_frame().
// Информация о работе выводится на экран в формате:
// Read[nRd: SF-nSf, EF-nEf, RF- nRtr, err-nErr], Fail(nFail), Time(nTm)
// nRd  - счетчик считанных сообщений
// nSf  - счетчик сообщений, имеющих стандартный формат кадра
// nEf  - счетчик сообщений, имеющих расширенный формат кадра
// nRtr - счетчик сообщений, имеющих формат RTR кадра
// nErr - счетчик соообщений, не соответствующих генератору
// nFail - счетчик операций, завершенных с ошибкой
// nTm  - счетчик операций, завершенных по таймауту
static void io_test_read(int port)
{
  do
  {
    // хэндл адаптера
    F_CAN_HANDLE h;
    // статус завершения операции
    F_CAN_RESULT fwcRes;
    // буфер для принимаемых сообщений
    F_CAN_RX rcv;
    // буфер для генерируемых по идентификатору сообщений
    F_CAN_MSG chk;
    // счетчики
    u32 n_rd = 0;
    u32 n_eff = 0;
    u32 n_sff = 0;
    u32 n_rtr = 0;
    u32 n_failure = 0;
    u32 n_timeout = 0;
    u32 n_err = 0;

    // Получаем хендл адаптера и запускаем устройство
    if(!io_test_open_adapter(port, &h))
      break;
    // Конфигурируем адаптер. (Одинаково для тестов io_test_read() и io_test_write().)
    if(!io_read_write_test_config(h))
      break;
      
    // Вывод на экран данных теста
    F_INFO7("\nRead[%lu: SF-%lu, EF-%lu, RF-%lu, err-%lu], Fail[%lu], Time[%lu]", 
      n_rd, n_sff, n_eff, n_rtr, n_err, n_failure, n_timeout);
      
    // Пока не получена команда выхода (нажатие любой клавиши)
    while(!_kbhit())
    {
      // приемник числа принятых сообщений
      size_t received = 0;
      // Читаем из приемного буфера устройства
      fwcRes = fw_can_recv(h, &rcv, 1, &received);
      // Проверяем статус завершения операции
      if(F_CAN_SUCCESS(fwcRes))
      {
        // Операция завершена успешно

        // Анализируем количество принятых сообщений
        if(received == 0)
        {
          // Принято 0 сообщений -> вышли по таймауту
          n_timeout++;
        }
        else if(received > 1)
        {
          // Ошибка драйвера
          F_INFO1("\nDriver error: invalid received value %d. Must be 1!\n", received);
        }
        else
        {
          // Модифицируем значения счетчиков теста
          n_rd++;
          if(rcv.msg.can_id & CAN_EFF_FLAG)
            n_eff++;
          else
            n_sff++;
    
          if(rcv.msg.can_id & CAN_RTR_FLAG)
            n_rtr++;
          
          // Формируем тестовое сообщение
          define_frame(&chk, rcv.msg.can_id);
          // Сравниваем с полученным
          if(false == compare_frames(&rcv.msg, &chk))
            n_err++;
        }
      }
      else
      {
        // Операция завершена с ошибкой
        n_failure++;
      }

      F_INFO7("\rRead[%lu: SF-%lu, EF-%lu, RF-%lu, err-%lu], Fail[%lu], Time[%lu]", 
        n_rd, n_sff, n_eff, n_rtr, n_err, n_failure, n_timeout);
    }
    // Работа теста завершена, закрываем хендл адаптера
    fw_can_close(h);
  }
  while(0);
}

//----------------------------------------------------------------------------------------------------
// Периодически посылает сообщения в сеть CAN.
// Идентификатор кадра следующего сообщения выбирается случайным образом.
// Сообщения формируются генератором define_frame() по идентификатору кадра.
// (Принимающая сторона может проводить проверку принятых сообщений.)
// Информация о работе выводится на экран в формате:
// Write[nWr: SF-nSf, EF-nEf, RF- nRtr], Fail(nFail), Abort(nTm)
// nWr  - счетчик успешно переданных сообщений
// nSf  - счетчик сообщений имеющих стандартный формат кадра
// nEf  - счетчик сообщений имеющих расширенный формат кадра
// nRtr - счетчик сообщений имеющих формат RTR кадра
// nFail - счетчик операций завершенных с ошибкой
// nTm  - счетчик операций прекращенных по таймауту
static void io_test_write(int port)
{
  do
  {
    // хэндл адаптера
    F_CAN_HANDLE h;
    // статус завершения операции
    F_CAN_RESULT fwcRes;
    // Счетчики
    u32 n_wr = 0;
    u32 n_eff = 0;
    u32 n_sff = 0;
    u32 n_rtr = 0;
    u32 n_failure = 0;
    u32 n_timeout = 0;

    // Получаем хендл адаптера и запускаем устройство
    if(!io_test_open_adapter(port, &h))
      break;
      
    // Конфигурируем адаптер (одинаково для тестов io_test_read() и io_test_write())
    if(!io_read_write_test_config(h))
      break;
    
    // Вывод на экран результатов теста
    F_INFO6("\nWrite[%lu: SF-%lu, EF-%lu, RF-%lu], Fail(%lu), Abort(%lu)", 
      n_wr, n_sff, n_eff, n_rtr, n_failure, n_timeout);
      
    // Пока не получена команда выхода (нажатие любой клавиши)
    while(!_kbhit())
    {
      // буфер для отправляемых сообщений
      F_CAN_TX wr;
      // приемник числа отправленных сообщений
      size_t written = 0;
      // "случайный" иденитификатор кадра сообщения
      u32 id = rand_canid();

      // формируем сообщение генератором
      define_frame(&wr.msg, id);
      // отправляем сообщение
      fwcRes = fw_can_send(h, &wr, 1, &written);
      // Проверяем статус завершения операции
      if(F_CAN_SUCCESS(fwcRes))
      {
        // Операция завершена успешно

        // Анализируем количество отправленных сообщений
        if(written == 0)
        {
          // Отправлено 0 сообщений -> вышли по таймауту
          n_timeout++;
        }
        else if(written > 1)
        {
          // Ошибка драйвера
          F_INFO1("\nDriver error: invalid written value %d. Must be 1!\n", written);
        }
        else
        {
          // Модифицируем значения счетчиков теста
          n_wr++;
          if(wr.msg.can_id & CAN_EFF_FLAG)
            n_eff++;
          else
            n_sff++;

          if(wr.msg.can_id & CAN_RTR_FLAG)
            n_rtr++;
        }
      }
      else
      {
        // Операция завершена с ошибкой
        n_failure++;
      }

      F_INFO6("\rWrite[%lu: SF-%lu, EF-%lu, RF-%lu], Fail(%lu), Abort(%lu)", 
        n_wr, n_sff, n_eff, n_rtr, n_failure, n_timeout);
        
      el_sleep(100);
    }
    fw_can_close(h);
  }
  while(0);
}

//----------------------------------------------------------------------------------------------------

//===================================================================================================
struct wait_test_cntxt
{
  F_CAN_HANDLE  hCan;
  HANDLE        hSema;
  bool          terminated;
};

static int wait_err_task(struct wait_test_cntxt* cntxt)
{
  F_CAN_WAIT wt;
  F_CAN_RESULT fwcRes;

  wt.waitMask = CAN_WAIT_ERR;

  F_INFO1("->fw_can_wait() at %d\n", el_get_tick_count());
  fwcRes = fw_can_wait(cntxt->hCan, &wt, 15000);
  if(F_CAN_SUCCESS(fwcRes))
  {
    fw_can_purge(cntxt->hCan, CAN_PURGE_TXABORT | CAN_PURGE_TXCLEAR);
  }
  F_INFO2("fw_can_wait()-> at %d with result %d\n", el_get_tick_count(), fwcRes);

  return 0;
}

static void wait_err_test(int port)
{
  size_t n_passes;
  HANDLE hThread = NULL;
  struct wait_test_cntxt context;

  context.hCan = EL_INVALID_HANDLE_VALUE;
  context.hSema = NULL;
  context.terminated = false;

  do
  {
    F_CAN_TIMEOUTS timeouts;
    F_CAN_TX wr;
    F_CAN_STATE adapter_state;
    F_CAN_STATS stats;
    F_CAN_ERRORS errs;
    F_CAN_RESULT fwcRes;

    fwcRes = fw_can_open(port, &context.hCan);
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO("fw_can_open() failed");
      break;
    }

    timeouts.ReadTotalTimeout = 0;
    timeouts.WriteTotalTimeoutConstant = 0;
    timeouts.WriteTotalTimeoutMultiplier = 2000;  // передача - 2 с на сообщение
    timeouts.RestartBusoffTimeout = 0;
    fwcRes = fw_can_set_timeouts(context.hCan, &timeouts);
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO("fw_can_set_timeouts() failed");
      break;
    }

    fwcRes = fw_can_start(context.hCan);
    if(!F_CAN_SUCCESS(fwcRes))
    {
      F_INFO("fw_can_start() failed");
      break;
    }

    context.hSema = CreateSemaphore(NULL, 0, 1, NULL);
    if(context.hSema == NULL)
      break;

    context.terminated = false;
    hThread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)wait_err_task, &context, 0, NULL);
    if(hThread == NULL)
      break;

    wr.msg.can_id = 0x80;
    wr.msg.can_dlc = 8;
    size_t written = 0;

    el_sleep(1000);
    F_INFO1("->fw_can_send() at %d\n", el_get_tick_count());
    fw_can_send(context.hCan, &wr, 1, &written);
    F_INFO1("fw_can_send()-> at %d\n", el_get_tick_count());

    WaitForSingleObject(hThread, INFINITE);

    F_INFO1("fw_can_send(): written(%d)\n", written);

    fw_can_get_clear_errors(context.hCan, &errs);
  
    F_INFO4("tx_timeout(%d), data_overrun(%d), error_passive(%d), bus_off(%d)\n", 
      errs.tx_timeout, errs.data_overrun, errs.error_passive, errs.bus_off);

    fw_can_get_stats(context.hCan, &stats);
    F_INFO5("STATS: tx(%d), tx_err(%d), error_warning(%d), error_passive(%d), bus_off(%d)\n", 
      stats.tx_packets, stats.tx_errors, stats.error_warning, stats.error_passive, stats.bus_off);

  }
  while(0);

  if(fw_can_is_handle_valid(context.hCan))
    fw_can_close(context.hCan);

  if(hThread != NULL)
    CloseHandle(hThread);

}
//===================================================================================================

// Структура описания теста
struct can_test_routine
{
  char* desc;             // Название
  void (*func)(int port); // Функция запуска
};

// Включенные в пример тесты
struct can_test_routine test[] =
{
  {"FwCAN: \"Self Mode\" Test (All ports)", io_test_self_start},
  {"FwCAN: \"Read\" Test", io_test_read},
  {"FwCAN: \"Write\" Test", io_test_write},
  {"FwCAN: \"Echo Server\"", io_echo_server},
  {"FwCAN: \"Echo Client\"", io_echo_client}, 
  {"FwCAN: \"Wait Test\"", wait_err_test} 
};

#define N_TEST_DEFINED  (sizeof(test)/sizeof(struct can_test_routine))

//----------------------------------------------------------------------------------------------------

int main (int argc, char* argv[])
{
#define TEST_UNINITIALIZED_DEFAULT       12345
  size_t test_num = TEST_UNINITIALIZED_DEFAULT;
  size_t port_num = TEST_UNINITIALIZED_DEFAULT;
  int i;

  // Параметры теста (номер + адаптер) берем из командной строки
  if(argc > 1)
    test_num = strtoul(argv[1], 0, 10);
  if(argc > 2)
    port_num = strtoul(argv[2], 0, 10);
  if(argc > 3)
    baude_rate = strtoul(argv[3], 0, 10);

  // Инициализирует библиотеку поддержки
  if(!F_CAN_SUCCESS(fw_can_init()))
  {
    F_INFO("fw_can_init() failed\n");
    return 1;
  }

  // Анализируем заданные пользователем параметры теста.
  // Запускаем тест или выводим информацию для запуска. 
  switch(test_num)
  {
  case 1:
    F_INFO1("%s\n", test[0].desc); 
    test[0].func(0);
    break;

  default:
    if(test_num <= N_TEST_DEFINED)
    {
      F_INFO1("%s ", test[test_num-1].desc);
      if(port_num == TEST_UNINITIALIZED_DEFAULT)
      {
        F_INFO("(please, specify CAN port)\n");
        break;
      }
      F_INFO1("PORT(%d)\n", (int)port_num);
      test[test_num-1].func((int)port_num);
    }
    else
    {
      F_INFO("Test\nSyntax is: can_test N [ Port = {1, 2, 3, 4, 5, 6, 7, 8} ] [Baud rate = { 10, 20, 50, 100, 125, 250, 500, 1000 }]\n");
      for(i = 0; i < N_TEST_DEFINED; i++)
      {
        F_INFO2("  %d  %s\n", i+1, test[i].desc);
      }
    }
  }

  return 0;
}
//----------------------------------------------------------------------------------------------------

#endif