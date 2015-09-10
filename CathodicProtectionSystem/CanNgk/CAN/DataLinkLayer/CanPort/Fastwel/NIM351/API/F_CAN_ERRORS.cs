using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    //�������� ������ �������� ��� fw_can_get_clear_errors()
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_ERRORS
    {
        // ���-�� ��������� ��������
        public UInt32 tx_timeout;
        // ���-�� ������������ ������ ������
        public UInt32 data_overrun;
        // ���-�� ��������� � CAN_STATE_ERROR_PASSIVE
        public UInt32 error_passive;
        // ���-�� ��������� � CAN_STATE_BUS_OFF
        public UInt32 bus_off;
    }
    //===========================================================================================
}
