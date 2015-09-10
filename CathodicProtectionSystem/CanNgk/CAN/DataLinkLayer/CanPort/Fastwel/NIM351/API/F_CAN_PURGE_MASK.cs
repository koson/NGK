using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    /// <summary>
    /// флаги сброса/прерывания текущей операции над адаптером
    /// </summary>
    [Flags]
    public enum F_CAN_PURGE_MASK: uint
    {
        /// <summary>
        /// Сбрасывает все флаги
        /// </summary>
        CAN_PURGE_CLEAR_ALL = 0x0000,
        /// <summary>
        /// Очистить очередь заданий на отправку сообщений.
        /// (прервать текущие/отложенные обращения к адаптеру по записи)
        /// </summary>
        CAN_PURGE_TXABORT = 0x0001,
        /// <summary>
        /// очистить очередь заданий на чтение входящих сообщений.
        /// (прервать текущие/отложенные обращения к адаптеру по чтению)
        /// </summary>
        CAN_PURGE_RXABORT = 0x0002,
        /// <summary>
        /// сбросить в адаптере текущий запрос на передачу.
        /// (очистить внутренний буфер передачи драйвера данного адаптера)
        /// </summary>
        CAN_PURGE_TXCLEAR = 0x0004,
        /// <summary>
        /// очистить внутренний буфер приема.
        /// </summary>
        CAN_PURGE_RXCLEAR = 0x0008,
        /// <summary>
        /// выполнить аппаратный сброс CAN-адаптера.
        /// </summary>
        CAN_PURGE_HWRESET = 0x0010
    }
}
