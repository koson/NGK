using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    /// �������� ������ ����� ���������� ������� CAN-��������: fw_can_wait()
    /// </summary>
    [Serializable]
    public enum CAN_WAIT : byte
    {
        ///<summary>
        /// �������� ������� CAN_STATUS_RXBUF
        /// �������� ������� � ������ ������ ���� �� ������ ���������
        ///</summary> 
        CAN_WAIT_RX = F_CAN_STATUS.CAN_STATUS_RXBUF,
        ///<summary>
        /// �������� ������� CAN_STATUS_TXBUF
        /// �������� ������������ ����������� ������ �������� CAN-��������
        ///</summary>
        CAN_WAIT_TX = F_CAN_STATUS.CAN_STATUS_TXBUF,
        ///<summary>
        /// �������� ������� CAN_STATUS_ERR
        /// �������� ����� ������
        /// </summary>
        CAN_WAIT_ERR = F_CAN_STATUS.CAN_STATUS_ERR
    }
    //===========================================================================================
}
