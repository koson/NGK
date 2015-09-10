using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    //��������� ��������
    ///<sumary>
    /// ����� ������ ������:
    ///</sumary> 
    [Flags]
    [Serializable]
    public enum CAN_OPMODE : ushort
    {
        /// <summary>
        /// ��� ����� ��������. ��������� ���� ��� �������� (����������� � ������������ �� NIM351).
        /// </summary>
        CAN_OPMODE_INIT = 0x0000,
        ///<sumary>
        /// ���������� ��������� ������������ ������� (11-������� CAN-ID)
        ///</sumary> 
        CAN_OPMODE_STANDARD = 0x0001,
        ///<summary>
        ///  ����������� ��������� ������������ ������� (29-������� CAN-ID)
        /// </summary>
        CAN_OPMODE_EXTENDED = 0x0002,
        ///<summary>
        /// ��������� ������ ����������� ������������ CAN-���������
        ///</summary>
        CAN_OPMODE_ERRFRAME = 0x0004,
        ///<summary>
        /// ���������������� � ������ "Listen Only" (���������� ����), ��� �������
        /// ������� �� �������� ������� �������������, ���� ���� ��������� ������� �������.
        ///</summary>
        CAN_OPMODE_LSTNONLY = 0x0008,
        ///<summary>
        /// ������� ����� ��������� ��������, ���� ��� ���������� ������������� �� ����
        ///</summary>
        CAN_OPMODE_SELFTEST = 0x0010,
        ///<summary>
        /// ������� ��������� ����������� ������������ ��������� (��� �������, ��� ���
        /// ������������� ��������� acceptance-�������)
        ///</summary>
        CAN_OPMODE_SELFRECV = 0x0020
    }
    //===========================================================================================
}
