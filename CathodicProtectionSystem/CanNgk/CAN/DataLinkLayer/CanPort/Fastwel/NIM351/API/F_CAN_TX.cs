using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    /// ��������� (������������) ��������� 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_TX
    {
        /// <summary>
        /// "������� �����" (���� ���������)  
        /// </summary>
        public F_CAN_MSG msg;
    }
    //===========================================================================================

}
