using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    /// Структура, определяющая объект для ожидания статуса CAN-адаптера функцией fw_can_wait()
    /// </summary>
    [Serializable]
    public struct F_CAN_WAIT
    {
        ///<summary>
        /// Маска ожидаемого статуса CAN-адаптера (по ИЛИ), включая:
        /// CAN_WAIT_RX -- в приемном буфере есть непрочитанное сообщение
        /// CAN_WAIT_TX -- в буфер передачи нет неотправленных сообщений 
        /// CAN_WAIT_ERR -- с момента последнего вызова fw_can_get_clear_errors() произошла ошибка(и)
        ///</summary>
        public F_CAN_STATUS waitMask;
        ///<summary>
        /// Маска текущего статуса CAN-адаптера
        ///</summary>
        public F_CAN_STATUS status;
    }
    //===========================================================================================
}
