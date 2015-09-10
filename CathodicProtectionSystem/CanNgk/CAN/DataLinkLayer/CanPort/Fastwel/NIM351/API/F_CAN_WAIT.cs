using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    /// ���������, ������������ ������ ��� �������� ������� CAN-�������� �������� fw_can_wait()
    /// </summary>
    [Serializable]
    public struct F_CAN_WAIT
    {
        ///<summary>
        /// ����� ���������� ������� CAN-�������� (�� ���), �������:
        /// CAN_WAIT_RX -- � �������� ������ ���� ������������� ���������
        /// CAN_WAIT_TX -- � ����� �������� ��� �������������� ��������� 
        /// CAN_WAIT_ERR -- � ������� ���������� ������ fw_can_get_clear_errors() ��������� ������(�)
        ///</summary>
        public F_CAN_STATUS waitMask;
        ///<summary>
        /// ����� �������� ������� CAN-��������
        ///</summary>
        public F_CAN_STATUS status;
    }
    //===========================================================================================
}
