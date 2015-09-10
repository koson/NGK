using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    ///<summary>
    /// ���������, ������������ � fw_can_get_controller_config() �
    /// fw_can_set_controller_config().
    ///</summary> 
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_SETTINGS
    {
        ///<summary>
        /// ��� �������� (������ ������)
        ///</summary>
        //[MarshalAs(UnmanagedType.I4)]
        public F_CAN_CONTROLLER controller_type;
        /// <summary>
        ///  �������� ������
        /// </summary>
        public F_CAN_BAUDRATE baud_rate;
        /// <summary>
        /// ����� ������ ������ (��. CAN_OPMODE)
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 opmode;
        ///<summary>
        /// ������ ����������� CAN-ID-�� �������� (�����������) ���������.
        /// ������������ ������ acceptance_code � acceptance_mask.
        /// ������� ������� ��������������� ���������, ���������� ������
        ///</summary>
        public UInt32 acceptance_code;
        ///<summary>
        /// �����, ����������� ������� "����������" ������� ������� ��� �������� 
        /// ������������ �������� ��������
        ///</summary> 
        public UInt32 acceptance_mask;
        ///<summary>
        /// �����, ����������� ����������� ��������� ������, ������������ ������������
        /// ����������� �� ������� ��� ������ � ������ CAN_OPMODE_ERRFRAME.
        ///</summary> 
        public UInt32 error_mask;
    }
    //===========================================================================================
}
