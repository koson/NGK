using System;
using System.Runtime.InteropServices;
using System.Text;


namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    ///  �������� ������ ������� CAN-�������� 
    /// </summary>
    [Flags]
    [Serializable]
    public enum F_CAN_STATUS : int
    {
        /// <summary>
        /// ��� ����� ��������, ����� ������������ ��� ��������� ��������
        /// ��� ����� "�" ��� ������ ���� ������.
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        CAN_STATUS_EMPTY = 0x00,
        /// <summary>
        /// 1 - ���� � ����� �������� ���������
        /// �������� ��� ������
        /// 0 - �����; ��� �������� ���������
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        CAN_STATUS_RXBUF = 0x01,
        /// <summary>
        /// 1 - ���������� ����� �������� CAN-�������� ��������;
        /// �������� ������ � ����� ��������
        /// 0 - ���������� ����� �������� CAN-�������� �����;
        /// ��������� ��������� ������� ��������
        /// ��� ��������� � �������� ��������
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        CAN_STATUS_TXBUF = 0x02,
        /// <summary>
        /// 1 - ��������� ������ � ������� ����������
        /// ������ �������� ������
        /// 0 - � ������� ���������� ������ ���������
        /// ������ ����� ������ �� ���������.  
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        CAN_STATUS_ERR = 0x04
    }

}
