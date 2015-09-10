using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    /// <summary>
    /// CAN-���������
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_MSG
    {
        /// <summary>
        /// CAN-ID, ������� ����� EFF/RTR/ERR
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 can_id;
        /// <summary>
        /// ���-�� ������ � ����� (�� 0 �� 8)
        /// </summary> 
        [MarshalAs(UnmanagedType.U1)]
        public Byte can_dlc;
        ///<summary>
        ////���� ������ ����� (����� ������ ���� 8),
        /// !!!��� ����������� ����� ���� ����� ������� Init();
        ///</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] data;
        ///// <summary>
        ///// ������������� ���� data;
        ///// </summary>
        //public void Init()
        //{
        //    can_id = 0;
        //    can_dlc = 0;
        //    this.data = new Byte[8];
        //}
    }
}
