using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    /// ���� ������ � �����, ����������� � CAN-���������
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct CAN_MSG_FLAG
    {
        /// <summary>
        ///  ��������� ������������/������������ ������� �����.
        /// EFF/SFF is set in the MSB
        /// </summary>
        public const UInt32 CAN_EFF_FLAG = 0x80000000U;
        /// <summary>
        /// ��������� RTR-�����. 
        /// remote transmission request
        /// </summary>
        public const UInt32 CAN_RTR_FLAG = 0x40000000U;
    }
    //===========================================================================================
}
