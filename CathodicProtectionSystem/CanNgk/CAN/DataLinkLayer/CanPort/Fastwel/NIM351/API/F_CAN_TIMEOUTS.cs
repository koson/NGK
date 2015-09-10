using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    ///<summary>
    /// �������� �������� �� ������ � ��������, ������������ ���������:
    /// fw_can_get_timeouts()
    /// fw_can_set_timeouts()
    /// fw_can_recv()
    /// fw_can_send()
    /// �������� �������� �������� �������� fw_can_send() ������������ ��������:
    /// Tsend = N * WriteTotalTimeoutMultiplier + WriteTotalTimeoutConstant (��),
    /// ��� N -- ���-�� ���������, ������������ ���������� nTx.
    /// ���� WriteTotalTimeoutMultiplier � WriteTotalTimeoutConstant ����� 0,
    /// �������� fw_can_send() ����� �����������.
    /// ���� ��� ������ fw_can_recv() � ������ ������ ������� ���� �� ���� ���������,
    /// fw_can_recv() ��������� �� � ���������� ����������.
    /// ���� ��� ������ fw_can_recv() � ������ ������ ��� �� ������ ���������, ��
    /// ��� ������ ������� �� ��������� fw_can_recv() ��������� ��� � ����������
    /// ����������.
    /// ���� ��� ������ fw_can_recv() � ������ ������ ��� �� ������ ���������, � �����
    /// ��� ����� �������� �� ���� ReadTotalTimeout, �� ��� ���������� �������� ���������
    /// � ������� ReadTotalTimeout (��) ������� fw_can_recv() ���������� ���������.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_TIMEOUTS
    {
        /// <summary>
        ///  ��������� ��� ���������� �������� ��������
        /// </summary>
        public UInt32 WriteTotalTimeoutMultiplier;
        /// <summary>
        /// ��������� ��� ���������� �������� ��������
        /// </summary>
        public UInt32 WriteTotalTimeoutConstant;
        /// <summary>
        ///  ������� ������ (� ��) �������� fw_can_recv()
        /// </summary>
        public UInt32 ReadTotalTimeout;
        /// <summary>
        ///  ������� ��������������� �������������� �� ��������� bus-off (CAN_STATE_BUS_OFF)
        /// </summary>
        public UInt32 RestartBusoffTimeout;
    }
    //===========================================================================================
}
