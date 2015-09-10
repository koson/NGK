using System;
using System.Runtime.InteropServices;
using System.Text;


namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    ///  Значения флагов статуса CAN-адаптера 
    /// </summary>
    [Flags]
    [Serializable]
    public enum F_CAN_STATUS : int
    {
        /// <summary>
        /// Все флаги сброшены, можно использовать как начальное значение
        /// или маску "И" для сброса всех флагов.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        CAN_STATUS_EMPTY = 0x00,
        /// <summary>
        /// 1 - одно и более принятых сообщений
        /// доступны для чтения
        /// 0 - пусто; нет принятых сообщений
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        CAN_STATUS_RXBUF = 0x01,
        /// <summary>
        /// 1 - внутренний буфер передачи CAN-адаптера свободен;
        /// возможна запись в буфер передачи
        /// 0 - внутренний буфер передачи CAN-адаптера занят;
        /// некоторое сообщение ожидает отправки
        /// или находится в процессе отправки
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        CAN_STATUS_TXBUF = 0x02,
        /// <summary>
        /// 1 - произошла ошибка с момента последнего
        /// сброса счетчика ошибок
        /// 0 - с момента последнего сброса счетчиков
        /// ошибок новых ошибок не произошло.  
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        CAN_STATUS_ERR = 0x04
    }

}
