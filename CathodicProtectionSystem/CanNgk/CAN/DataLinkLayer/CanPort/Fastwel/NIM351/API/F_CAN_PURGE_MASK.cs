using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    /// <summary>
    /// ����� ������/���������� ������� �������� ��� ���������
    /// </summary>
    [Flags]
    public enum F_CAN_PURGE_MASK: uint
    {
        /// <summary>
        /// ���������� ��� �����
        /// </summary>
        CAN_PURGE_CLEAR_ALL = 0x0000,
        /// <summary>
        /// �������� ������� ������� �� �������� ���������.
        /// (�������� �������/���������� ��������� � �������� �� ������)
        /// </summary>
        CAN_PURGE_TXABORT = 0x0001,
        /// <summary>
        /// �������� ������� ������� �� ������ �������� ���������.
        /// (�������� �������/���������� ��������� � �������� �� ������)
        /// </summary>
        CAN_PURGE_RXABORT = 0x0002,
        /// <summary>
        /// �������� � �������� ������� ������ �� ��������.
        /// (�������� ���������� ����� �������� �������� ������� ��������)
        /// </summary>
        CAN_PURGE_TXCLEAR = 0x0004,
        /// <summary>
        /// �������� ���������� ����� ������.
        /// </summary>
        CAN_PURGE_RXCLEAR = 0x0008,
        /// <summary>
        /// ��������� ���������� ����� CAN-��������.
        /// </summary>
        CAN_PURGE_HWRESET = 0x0010
    }
}
