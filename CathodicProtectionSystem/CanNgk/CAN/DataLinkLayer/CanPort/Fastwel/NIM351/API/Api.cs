using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Text.RegularExpressions;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataLinkLayer.CanPort;

//=================================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //=============================================================================================
    public static class Api
    {
        //-----------------------------------------------------------------------------------------
        // Function		 :  F_CAN_RESULT fw_can_init(void)
        // Parameters	 : 
        // Return value  :  Результат инициализации библиотеки поддержки
        //                  CAN_RES_OK    - успех
        //                  CAN_RES_XX    - значение, отличное от CAN_RES_OK, свидетельствует об ошибке
        // Description	 :  Инициализирует библиотеку поддержки для вызывающего процесса.
        // Данная функция должна быть вызвана в начале программы, в которой предполагается
        // взаимодействовать с адаптером.           
        //
        /// <summary>
        /// Инициализирует библиотеку поддержки для вызывающего процесса.
        /// Данная функция должна быть вызвана в начале программы, в которой предполагается
        /// взаимодействовать с адаптером.
        /// </summary>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_init")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_init();
        //-----------------------------------------------------------------------------------------
        // Function		 :  F_CAN_RESULT fw_can_open(F_CAN_DEVID id, PF_CAN_HANDLE phDev)
        // Parameters	 :  id - идентификатор (индекс) адаптера, начиная с 1
        //               :  phDev - указатель на переменную типа F_CAN_HANDLE, в которой, при успешном
        //                  открытии адаптера, окажется хэндл устройства. Корректность хэндла можно проверить
        //                  при помощи макроса fw_can_is_handle_valid:
        //                  PF_CAN_HANDLE handle;
        // if(CAN_RES_OK == fw_can_open(1, &handle, true) && fw_can_is_handle_valid(handle))
        // {
        //      //устройство с индексом 1 открыто успешно
        //      fw_can_close(handle);
        // }
        // Return value : 
        // CAN_RES_OK - успех;
        // значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description	 :  Функция открывает адаптер с заданным идентификатором (индексом).
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">идентификатор (индекс) адаптера, начиная с 1</param>
        /// <param name="phDev">указатель на переменную типа F_CAN_HANDLE, в которой, при успешном
        ///                  открытии адаптера, окажется хэндл устройства</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_open")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_open(
            Int32 id,
            out SafeFileHandle phDev);
        //-----------------------------------------------------------------------------------------
        // Function	    :  F_CAN_RESULT fw_can_close(F_CAN_HANDLE hDev)
        // Parameters	:  hDev - хэндл закрываемого адаптера
        // Return value : CAN_RES_OK - успех;
        //                значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description	 :  Закрывает хэндл адаптера, ранее открытый при помощи fw_can_open().
        // После данного вызова hDev не может использоваться для доступа к адаптеру.
        /// <summary>
        /// Закрывает хэндл адаптера, ранее открытый при помощи fw_can_open().
        /// После данного вызова hDev не может использоваться для доступа к адаптеру.
        /// </summary>
        /// <param name="hDev">хэндл закрываемого адаптера</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_close")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_close(SafeFileHandle hDev);
        //-----------------------------------------------------------------------------------------
        // Function     : F_CAN_RESULT fw_can_get_controller_config(F_CAN_HANDLE hDev, F_CAN_SETTINGS pDcb)
        // Parameters   : hDev - хэндл адаптера
        //              : pDcb - указатель на структуру параметров CAN-адаптера
        // Return value : CAN_RES_OK - успех;
        //                значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  : Возвращает текущие параметры CAN-адаптера.
        /// <summary>
        /// Возвращает текущие параметры CAN-адаптера
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pDcb">Структура с параметрами CAN-адаптера</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
           CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_controller_config")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_controller_config(
            SafeFileHandle hDev,
            out F_CAN_SETTINGS pDcb);
        //-----------------------------------------------------------------------------------------
        // Function     : F_CAN_RESULT fw_can_set_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb)
        // Parameters   : hDev - хэндл адаптера
        //              : pDcb - указатель на структуру параметров CAN-адаптера
        // Return value : CAN_RES_OK - успех;
        // значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Задает новые значения параметров CAN-адаптера. Установка параметров
        // возможна только, когда адаптер находится в состоянии CAN_STATE_INIT.
        /// <summary>
        /// Задает новые значения параметров CAN-адаптера. Установка параметров
        /// возможна только, когда адаптер находится в состоянии CAN_STATE_INIT.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pDcb">Структура с параметрами CAN-адаптера</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
          CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_set_controller_config")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_set_controller_config(
            SafeFileHandle hDev,
            ref F_CAN_SETTINGS pDcb);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_get_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
        // Parameters   :  hDev - хэндл адаптера
        //              :  pTimeouts - указатель на структуру таймаутов CAN-адаптера
        // Return value : CAN_RES_OK - успех;
        //                значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Возвращает текущие значения таймаутов операций чтения, записи и сброса заданного CAN-адаптера.
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_timeouts")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_timeouts(
            SafeFileHandle hDev,
            out F_CAN_TIMEOUTS pTimeouts);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_set_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
        // Parameters   :  hDev - хэндл адаптера
        //              :  pTimeouts - указатель на структуру таймаутов CAN-адаптера
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Устанавливает новые значения таймаутов операций чтения, записи и сброса заданного CAN-адаптера.
        /// <summary>
        /// Устанавливает новые значения таймаутов операций чтения, записи и сброса заданного CAN-адаптера.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pTimeouts">Структура с таймаутами</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_set_timeouts")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_set_timeouts(
            SafeFileHandle hDev,
            ref F_CAN_TIMEOUTS pTimeouts);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_get_controller_state(F_CAN_HANDLE hDev, PF_CAN_STATE pState)
        // Parameters   :  hDev - хэндл адаптера
        //              :  pState - указатель на переменную, в которой будет возвращено текущее состояние адаптера,
        // включая:
        //              CAN_STATE_INIT - начальное состояние
        //              CAN_STATE_ERROR_ACTIVE - кол-во ошибок приема/передачи не более 96
        //              CAN_STATE_ERROR_WARNING	- кол-во ошибок приема/передачи от 96 до 127
        //              CAN_STATE_ERROR_PASSIVE -	кол-во ошибок приема/передачи от 128 до 255
        //              CAN_STATE_BUS_OFF -	кол-во ошибок приема/передачи >= 256 (BUSOFF)
        //              CAN_STATE_STOPPED - адаптер остановлен
        //              CAN_STATE_SLEEPING - не используется в текущей версии
        // Return value : CAN_RES_OK - успех;
        //              значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Возвращает текущее состояние CAN-адаптера
        /// <summary>
        /// Возвращает текущее состояние CAN-адаптера
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pState"> Структура с состояниями CAN-адаптера:
        ///             CAN_STATE_INIT - начальное состояние
        ///             CAN_STATE_ERROR_ACTIVE - кол-во ошибок приема/передачи не более 96
        ///             CAN_STATE_ERROR_WARNING	- кол-во ошибок приема/передачи от 96 до 127
        ///             CAN_STATE_ERROR_PASSIVE -	кол-во ошибок приема/передачи от 128 до 255
        ///             CAN_STATE_BUS_OFF -	кол-во ошибок приема/передачи >= 256 (BUSOFF)
        ///             CAN_STATE_STOPPED - адаптер остановлен
        ///             CAN_STATE_SLEEPING - не используется в текущей версии
        ///</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
           CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_controller_state")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_controller_state(
            SafeFileHandle hDev,
            out F_CAN_STATE pState);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_get_stats(F_CAN_HANDLE hDev, PF_CAN_STATS pStats)
        // Parameters   :  hDev - хэндл адаптера
        //              :  pDcb - указатель на структуру статистической информации CAN-адаптера
        // Return value : 
        //              CAN_RES_OK - успех;
        //              значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Возвращает статистическую информацию заданного адаптера.
        /// <summary>
        /// Возвращает статистическую информацию заданного адаптера.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pStats">Структура со статической информацией CAN-адаптера:
        /// кол-во принятых кадров; кол-во принятых байт; кол-во переданных кадров;
        /// кол-во переданных байт; кол-во ошибок приема; кол-во переполнений буфера приема;
        /// кол-во ошибок передачи; кол-во переполнений буфера передачи;
        /// кол-во ошибок на шине (кадрирование, CRC, etc.);
        /// кол-во переходов в CAN_STATE_ERROR_WARNING;
        /// кол-во переходов в CAN_STATE_ERROR_PASSIVE;
        /// кол-во переходов в CAN_STATE_BUS_OFF;
        /// кол-во "проигрышей" арбитража;
        /// кол-во перезапусков контроллера
        /// </param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_stats")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_stats(
            SafeFileHandle hDev,
            out F_CAN_STATS pStats);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_clear_stats(F_CAN_HANDLE hDev)
        // Parameters   :  hDev - хэндл адаптера
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Сбрасывает в 0 статистическую информацию заданного адаптера.
        /// <summary>
        /// Сбрасывает в 0 статистическую информацию заданного адаптера.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_clear_stats")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_clear_stats(SafeFileHandle hDev);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_start(F_CAN_HANDLE hDev)
        // Parameters   :  hDev - хэндл запускаемого адаптера
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Запускает заданный адаптер. При успешном запуске адаптер оказывается
        //                 соединенным с шиной CAN и выходит из начального состояния CAN_STATE_INIT.
        /// <summary>
        /// Запускает заданный адаптер. При успешном запуске адаптер оказывается
        /// соединенным с шиной CAN и выходит из начального состояния CAN_STATE_INIT.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <returns>Результат выполнения функции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_start")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_start(SafeFileHandle hDev);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_stop(F_CAN_HANDLE hDev)
        // Parameters   :  hDev - хэндл останавливаемого адаптера
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Останавливает заданный адаптер. При успешном останове адаптер отсоединяется
        //                 от шины CAN. После завершения функции адаптер пребывает в состоянии CAN_STATE_INIT.
        /// <summary>
        /// Останавливает заданный адаптер. При успешном останове адаптер отсоединяется
        /// от шины CAN. После завершения функции адаптер пребывает в состоянии CAN_STATE_INIT.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_stop")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_stop(SafeFileHandle hDev);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_send(F_CAN_HANDLE hDev, PF_CAN_TX pTx, size_t nTx, size_t* nSend)
        // Parameters   :  hDev - хэндл адаптера
        //                 pTx - указатель на буфер передаваемых сообщений
        //                 nTx - количество сообщений в буфере pTx
        //                 nSend - указатель на переменную, в которую будет помещено количество реально переданных
        //                         сообщений при возврате из данной функции.
        // Return value : CAN_RES_OK - успех;
        //                значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Передает сообщения в шину CAN.
        //                 Функция блокируется до тех пор, пока сообщения не будут переданы, либо пока
        //                 не произойдет таймаут передачи. Таймаут передачи должен быть предварительно задан
        //                 при помощи fw_can_set_timeouts().
        /// <summary>
        /// Передает сообщения в шину CAN. Функция блокируется до тех пор, пока сообщения не будут переданы, 
        /// либо пока не произойдет таймаут передачи. Таймаут передачи должен быть предварительно задан
        /// при помощи fw_can_set_timeouts().
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pTx">указатель на буфер передаваемых сообщений</param>
        /// <param name="nTx">количество сообщений в буфере pTx</param>
        /// <param name="nSend">указатель на переменную, в которую будет помещено 
        /// количество реально переданных сообщений при возврате из данной функции.</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "fw_can_send")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_send(
            SafeFileHandle hDev,
            ref F_CAN_TX pTx,
            UInt32 nTx,
            ref UInt32 nSend);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* nRecv)
        // Parameters   :  hDev - хэндл адаптера
        //                 pRx - указатель на буфер приема в приложении
        //                 nRx - емкость буфера приема
        //                 nRecv - указатель на переменную, в которую будет помещено количество принятых сообщений,
        //                         находящихся в буфере pRx при возврате из данной функции.
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  При вызове данной функции происходит считывание имеющихся сообщений во внутреннем буфере
        // драйвера для данного адаптера. Если внутренний буфер пуст, функция блокируется и ожидает 
        // приема хотя бы одного сообщения или таймаута приема. Таймаут приема должен быть предварительно 
        // задан при помощи fw_can_set_timeouts().
        /// <summary>
        /// При вызове данной функции происходит считывание имеющихся сообщений во внутреннем буфере
        /// драйвера для данного адаптера. Если внутренний буфер пуст, функция блокируется и ожидает 
        /// приема хотя бы одного сообщения или таймаута приема. Таймаут приема должен быть предварительно 
        /// задан при помощи fw_can_set_timeouts().
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pRx">указатель на буфер приема в приложении</param>
        /// <param name="szRx">емкость буфера приема</param>
        /// <param name="nRecv">указатель на переменную, в которую будет помещено 
        /// количество принятых сообщений, находящихся в буфере pRx при возврате 
        /// из данной функции.</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_recv")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_recv(
            SafeFileHandle hDev,
            out F_CAN_RX pRx, // Здесь следует передавть массив F_CAN_RX[]. Но, так тоже можно если szRx = 1 
            UInt32 szRx,
            out UInt32 nRecv);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* nRecv)
        // Parameters   :  hDev - хэндл адаптера
        //                 pRx - указатель на буфер приема в приложении
        //                 nRx - емкость буфера приема
        //                 nRecv - указатель на переменную, в которую будет помещено количество принятых сообщений,
        //                         находящихся в буфере pRx при возврате из данной функции.
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  При вызове данной функции происходит считывание имеющихся сообщений во внутреннем буфере
        // драйвера для данного адаптера. Если внутренний буфер пуст, функция блокируется и ожидает 
        // приема хотя бы одного сообщения или таймаута приема. Таймаут приема должен быть предварительно 
        // задан при помощи fw_can_set_timeouts().
        /// <summary>
        /// При вызове данной функции происходит считывание имеющихся сообщений во внутреннем буфере
        /// драйвера для данного адаптера. Если внутренний буфер пуст, функция блокируется и ожидает 
        /// приема хотя бы одного сообщения или таймаута приема. Таймаут приема должен быть предварительно 
        /// задан при помощи fw_can_set_timeouts().
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pRx">указатель на буфер приема в приложении</param>
        /// <param name="szRx">емкость буфера приема</param>
        /// <param name="nRecv">указатель на переменную, в которую будет помещено 
        /// количество принятых сообщений, находящихся в буфере pRx при возврате 
        /// из данной функции.</param>
        /// <returns>Результат выполнения операции</returns>
        /// <remarks>
        //Пример применения:
        //static void Read1(SafeFileHandle hDevice)
        //{
        //    CAN_RESULT result;
        //    F_CAN_RX[] rx_buf;
        //    UInt32 nRx;

        //    int size = Marshal.SizeOf(typeof(F_CAN_RX));

        //    IntPtr ptr = Marshal.AllocHGlobal(size);

        //    result = Api.fw_can_recv(hDevice, ptr, 5, out nRx);

        //    if (result == CAN_RESULT.CAN_RES_OK)
        //    {
        //        rx_buf = new F_CAN_RX[5];
        //        Console.WriteLine("Frames to read: {0}", nRx);

        //        for (int i = 0; i < nRx; i++)
        //        {
        //            IntPtr ptr1 = new IntPtr(ptr.ToInt32() + i * size);
        //            rx_buf[i] = (F_CAN_RX)Marshal.PtrToStructure(ptr1, typeof(F_CAN_RX));

        //            // Читаем буфер адаптера
        //            Console.WriteLine("Frame {0}:", i);
        //            Console.WriteLine("Time Stamp: {0}", rx_buf[i].timestamp);
        //            Console.WriteLine("Id: {0}", rx_buf[i].msg.can_id);
        //            Console.WriteLine("Total Bytes: {0}", rx_buf[i].msg.can_dlc);
        //            Console.WriteLine("Total Massages: {0} ", nRx);
        //            for (int x = 0; x < rx_buf[i].msg.can_dlc; x++)
        //            {
        //                Console.WriteLine("Data: {0}", rx_buf[i].msg.data[x]);
        //            }
        //            Console.WriteLine(" ");

        //        }

        //        Marshal.FreeHGlobal(ptr);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Read. Error: {0}", Enum.GetName(typeof(CAN_RESULT), result));
        //        Console.WriteLine(" ");
        //    }
        //    return;
        //}
        /// </remarks>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_recv")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_recv(
            SafeFileHandle hDev,
            IntPtr buffer, //указатель на массив F_CAN_RX[] pRx, 
            UInt32 szRx,
            out UInt32 nRecv);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_peek_message(F_CAN_HANDLE hDev, PF_CAN_RX pRx)
        // Parameters   :  hDev - хэндл адаптера
        //                 pRx - указатель на переменную F_CAN_RX, в которую будет помещено сообщение
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        //                 CAN_RES_RXQUEUE_EMPTY - нет сообщений во внутреннем буфере приема драйвера для
        //                 данного адаптера.
        // Description  :  При вызове данной функции происходит считывание одного сообщения из внутреннего
        //                 буфера приема. Функция не блокируется и немедленно завершается при любом состоянии буфера.
        /// <summary>
        /// При вызове данной функции происходит считывание одного сообщения из внутреннего
        /// буфера приема. Функция не блокируется и немедленно завершается при любом состоянии буфера.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pRx">pRx - указатель на переменную F_CAN_RX, 
        /// в которую будет помещено сообщение</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_peek_message")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_peek_message(
            SafeFileHandle hDev,
            out F_CAN_RX pRx);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_post_message(F_CAN_HANDLE hDev, PF_CAN_TX pTx)
        // Parameters   :  hDev - хэндл адаптера
        //                 pTx - указатель на переменную-сообщение, подлежащее передаче
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        //                 CAN_RES_TXQUEUE_FULL - переполнение буфера передачи.
        // Description  :  Функция помещает сообщение во внутренний буфер передачи адаптера.
        // Функция не блокируется и не ожидает завершения передачи сообщения.
        /// <summary>
        /// Функция помещает сообщение во внутренний буфер передачи адаптера.
        /// Функция не блокируется и не ожидает завершения передачи сообщения.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pTx">Указатель на переменную-сообщение, подлежащее передаче</param>
        /// <returns>Результат  выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_post_message")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_post_message(
            SafeFileHandle hDev,
            ref F_CAN_TX pTx);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_purge(F_CAN_HANDLE hDev, F_CAN_PURGE_MASK flags)
        // Parameters   :  hDev - хэндл адаптера
        //                 flags - флаги сброса/прерывания текущей операции над адаптером
        //                 CAN_PURGE_TXABORT - прерывает отложенные обращения к адаптеру по записи.
        //                 CAN_PURGE_TXCLEAR - сбрасывает в адаптере текущий запрос на передачу.
        //                 CAN_PURGE_RXCLEAR - очищает внутренний буфер приема.
        //                 CAN_PURGE_RXABORT - прерывает отложенные обращения к адаптеру по чтению.
        //                 CAN_PURGE_HWRESET - выполняет аппаратный сброс адаптера. При этом сбрасываются
        //                 счетчики ошибок адаптера. Настройки адаптера сохраняются.
        // Return value :  CAN_RES_OK - успех;
        //                 значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Функция, в зависимости от значения flags, очищает буферы приема и/или передачи и/или
        // выполняет аппаратный сброс адаптера. 
        // Кроме того, позволяет прервать текущие/отложенные обращения к адаптеру по чтению/записи.
        /// <summary>
        /// Функция, в зависимости от значения flags, очищает буферы приема и/или передачи и/или
        /// выполняет аппаратный сброс адаптера. Кроме того, позволяет прервать текущие/отложенные 
        /// обращения к адаптеру по чтению/записи.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="flags">флаги сброса/прерывания текущей операции над адаптером:
        ///                 CAN_PURGE_TXABORT - прерывает отложенные обращения к адаптеру по записи.
        ///                 CAN_PURGE_TXCLEAR - сбрасывает в адаптере текущий запрос на передачу.
        ///                 CAN_PURGE_RXCLEAR - очищает внутренний буфер приема.
        ///                 CAN_PURGE_RXABORT - прерывает отложенные обращения к адаптеру по чтению.
        ///                 CAN_PURGE_HWRESET - выполняет аппаратный сброс адаптера. При этом сбрасываются
        ///                 счетчики ошибок адаптера. Настройки адаптера сохраняются.</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_purge")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_purge(
            SafeFileHandle hDev,
            [MarshalAs(UnmanagedType.U4)] F_CAN_PURGE_MASK flags);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_wait(F_CAN_HANDLE hDev, PF_CAN_WAIT pWait, size_t msTimeout)
        // Parameters   :  hDev - хэндл адаптера
        //                 pWait - указатель на объект, содержащий маску ожидаемого статуса CAN-адаптера pWait->waitMask
        //                 и приемник текущего значения статуса.
        //                 msTimeout - таймаут ожидания в миллисекундах.
        // Return value :  CAN_RES_OK - успех;
        //                 CAN_RES_TIMEOUT - таймаут, текущий статус не соответствует ожидаемому, 
        //                 В случаях CAN_RES_OK и CAN_RES_TIMEOUT в поле pWait->status будет записано значение текущего статуса.
        //                 Значение, отличное от CAN_RES_OK и CAN_RES_TIMEOUT, свидетельствует об ошибке вызова функции.
        //                 Description  :  Функция блокирует выполнение вызывающего потока до тех пор, пока статус CAN-адаптера не 
        //                 будет соответствовать одному из значений, определенных маской pWait->waitMask, или 
        //                 не истечет таймаут заданный параметром msTimeout. При завершении данной функции с кодом 
        //                 CAN_RES_OK или CAN_RES_TIMEOUT поле pWait->status будет содержать текущее значения статуса.
        /// <summary>
        /// Функция блокирует выполнение вызывающего потока до тех пор, пока статус CAN-адаптера не 
        /// будет соответствовать одному из значений, определенных маской pWait->waitMask, или 
        /// не истечет таймаут заданный параметром msTimeout. При завершении данной функции с кодом 
        /// CAN_RES_OK или CAN_RES_TIMEOUT поле pWait->status будет содержать текущее значения статуса.
        /// </summary>
        /// <param name="hDev">Дескриптор CAN-порта</param>
        /// <param name="pWait">pWait - указатель на объект, содержащий маску ожидаемого 
        /// статуса CAN-адаптера pWait->waitMask и приемник текущего значения статуса.</param>
        /// <param name="msTimeout">таймаут ожидания в миллисекундах.</param>
        /// <returns>Результат выполнения операции:
        ///                CAN_RES_OK - успех;
        ///                 CAN_RES_TIMEOUT - таймаут, текущий статус не соответствует ожидаемому, 
        ///                 В случаях CAN_RES_OK и CAN_RES_TIMEOUT в поле pWait->status будет записано значение текущего статуса.
        ///                 Значение, отличное от CAN_RES_OK и CAN_RES_TIMEOUT, свидетельствует об ошибке вызова функции.
        ///                 Description  :  Функция блокирует выполнение вызывающего потока до тех пор, пока статус CAN-адаптера не 
        ///                 будет соответствовать одному из значений, определенных маской pWait->waitMask, или 
        ///                 не истечет таймаут заданный параметром msTimeout. При завершении данной функции с кодом 
        ///                 CAN_RES_OK или CAN_RES_TIMEOUT поле pWait->status будет содержать текущее значения статуса.</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_wait")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_wait(
            SafeFileHandle hDev,
            ref F_CAN_WAIT pWait,
            UInt32 msTimeout);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_get_clear_errors(F_CAN_HANDLE hDev, PF_CAN_ERRORS pErrors)
        // Parameters   :  hDev - хэндл адаптера
        //                 pErrors - указатель на объект, в котором будут возвращены счетчики ошибок.
        //                 Если в качестве pErrors передать NULL, произойдет сброс счетчиков ошибок для
        //                 данного адаптера.
        // Return value :  CAN_RES_OK - успех;
        // значение, отличное от CAN_RES_OK, свидетельствует об ошибке.
        // Description  :  Функция помещает значения счетчиков ошибок данного адаптера в
        // объект pErrors и сбрасывает счетчики. Если в качестве pErrors передать NULL, 
        // будет произведен сброс счетчиков ошибок для данного адаптера.
        /// <summary>
        /// Функция помещает значения счетчиков ошибок данного адаптера в
        /// объект pErrors и сбрасывает счетчики. Если в качестве pErrors передать NULL, 
        /// будет произведен сброс счетчиков ошибок для данного адаптера.
        /// </summary>
        /// <param name="hDev">Дескриптор устройства</param>
        /// <param name="pErrors">Указатель на объект, в котором будут возвращены счетчики ошибок.
        /// Если в качестве pErrors передать NULL, произойдет сброс счетчиков ошибок для
        /// данного адаптера.</param>
        /// <returns>Результат выполнения операции</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_clear_errors")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_clear_errors(
            SafeFileHandle hDev,
            out F_CAN_ERRORS pErrors);

        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
    CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_clear_errors")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_clear_errors(
            SafeFileHandle hDev,
            IntPtr pErrors);
        //-----------------------------------------------------------------------------------------
        public static Boolean f_can_success(F_CAN_RESULT result)
        {
            if (result == F_CAN_RESULT.CAN_RES_OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Проверяет дескрипртор устройства на корректность
        /// (Реализация не соотвествует Fastwel)
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static Boolean fw_can_is_handle_valid(SafeFileHandle handle)
        {
            if ((handle == null) || (handle.IsInvalid))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Строит и возвращает структуру режима работы порта NIM351
        /// </summary>
        /// <param name="mode">Режим работы порта</param>
        /// <param name="frameFormat">Формат кадра</param>
        /// <param name="errorFrameEnable">Разрешает/запрещает передачу информационных сообщений</param>
        /// <returns></returns>
        public static CAN_OPMODE OpModeBuilder(PortMode mode,
            FrameFormat frameFormat, Boolean errorFrameEnable)
        {
            CAN_OPMODE result;
            String msg;

            result = CAN_OPMODE.CAN_OPMODE_INIT;

            switch (mode)
            {
                case PortMode.NORMAL:
                    { break; }
                case PortMode.LISTEN_ONLY:
                    { result |= CAN_OPMODE.CAN_OPMODE_LSTNONLY; break; }
                case PortMode.SELFTEST:
                    { result |= CAN_OPMODE.CAN_OPMODE_SELFTEST; break; }
                case PortMode.SELFRECV:
                    { result |= CAN_OPMODE.CAN_OPMODE_SELFRECV; break; }
                default:
                    {
                        msg = String.Format(Properties.ErrorMessages.NotSupportedValue, mode);
                        throw new InvalidCastException(msg);
                    }
            }

            switch (frameFormat)
            {
                case FrameFormat.StandardFrame:
                    { result |= CAN_OPMODE.CAN_OPMODE_STANDARD; break; }
                case FrameFormat.ExtendedFrame:
                    { result |= CAN_OPMODE.CAN_OPMODE_EXTENDED; break; }
                case FrameFormat.MixedFrame:
                    { result |= (CAN_OPMODE.CAN_OPMODE_STANDARD | CAN_OPMODE.CAN_OPMODE_EXTENDED); break; }
                default:
                    {
                        msg = String.Format(Properties.ErrorMessages.NotSupportedValue, frameFormat);
                        throw new InvalidCastException(msg);
                    }
            }

            if (errorFrameEnable)
            { result |= CAN_OPMODE.CAN_OPMODE_ERRFRAME; }

            return result;
        }
        /// <summary>
        /// Строит сообщение
        /// </summary>
        /// <param name="identifier">Идентификатор сообщения</param>
        /// <param name="frameType">Тип сообщения</param>
        /// <param name="frameFormat">Формат сообщения</param>
        /// <param name="data">Данные сообщения</param>
        /// <returns></returns>
        public static F_CAN_MSG MessageBuilder(uint identifier, FrameType frameType,
            FrameFormat frameFormat, byte[] data)
        {
            F_CAN_MSG result;
            String msg;

            // Разбираем сообщение и подготавливаем его для отправки
            result.can_dlc = (Byte)data.Length;
            result.data = new Byte[8];
            Array.Copy(data, result.data, data.Length);

            result.can_id = identifier;
            //buffer.msg.can_id &= (CAN_MSG_MASK.CAN_SFF_MASK | CAN_MSG_MASK.CAN_EFF_MASK);

            if (frameFormat == FrameFormat.ExtendedFrame)
            {
                result.can_id |= CAN_MSG_FLAG.CAN_EFF_FLAG;
            }

            if (frameType == FrameType.REMOTEFRAME)
            {
                if (data.Length != 0)
                {
                    // При отправке сообщения возникла ошибка, при 
                    // устанавленном бите RTR, длина DLC данных должна быть равна 0
                    msg = "Не удалось отправить сообщение. DLC должно быть равно 0 при установленном бите RTR";
                    throw new Exception(msg);
                }
                result.can_id |= CAN_MSG_FLAG.CAN_RTR_FLAG;
            }

            return result; 
        }
        /// <summary>
        /// Возвращает номер порта по указанному наименованию порта
        /// </summary>
        /// <param name="portName">Наименование порта в формате CANx где x номер порта 1...9</param>
        /// <returns></returns>
        public static int GetPortNumber(string portName)
        {
            if (CheckPortName(portName))
            {
                return int.Parse(portName.Substring(3, 1));
            }
            else
            {
                throw new ArgumentException(
                    "Невозможно получить номер порта. " +
                    "Наименование порта не соотвествует формату CANx, где x номер порта 1...9");
            }
        }
        /// <summary>
        /// Проверяет строку с наименование порта на соотвествие
        /// формату.
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public static Boolean CheckPortName(String portName)
        {
            String PortNameRegexPattern = @"^CAN[1-9]$";

            if (Regex.IsMatch(portName, PortNameRegexPattern))
            { return true; }
            else
            { return false; }
        }
    }
    //=============================================================================================
}
//=================================================================================================
// End Of File