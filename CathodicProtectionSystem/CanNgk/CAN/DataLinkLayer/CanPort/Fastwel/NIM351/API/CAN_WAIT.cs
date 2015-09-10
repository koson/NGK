using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    /// «начени€ флагов маски ожидаемого статуса CAN-адаптера: fw_can_wait()
    /// </summary>
    [Serializable]
    public enum CAN_WAIT : byte
    {
        ///<summary>
        /// ќжидание статуса CAN_STATUS_RXBUF
        /// ќжидание наличи€ в буфере приема хот€ бы одного сообщени€
        ///</summary> 
        CAN_WAIT_RX = F_CAN_STATUS.CAN_STATUS_RXBUF,
        ///<summary>
        /// ќжидание статуса CAN_STATUS_TXBUF
        /// ќжидание освобождени€ внутреннего буфера передачи CAN-адаптера
        ///</summary>
        CAN_WAIT_TX = F_CAN_STATUS.CAN_STATUS_TXBUF,
        ///<summary>
        /// ќжидание статуса CAN_STATUS_ERR
        /// ќжидание любой ошибки
        /// </summary>
        CAN_WAIT_ERR = F_CAN_STATUS.CAN_STATUS_ERR
    }
    //===========================================================================================
}
