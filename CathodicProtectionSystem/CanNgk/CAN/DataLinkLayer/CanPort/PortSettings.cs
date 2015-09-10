using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;

namespace NGK.CAN.DataLinkLayer.CanPort
{
    /// <summary>
    /// ��� �������� � �������� ����������� ��������� �����
    /// </summary>
    public struct PortSettings
    {
        /// <summary>
        /// �������� ������ �������
        /// </summary>
        public BaudRate BitRate;
        /// <summary>
        /// ������ ������ (���� ID: 11 ��� 29 ���) � �������� �������� ����
        /// ����� ����������� �� "���"
        /// </summary>
        public FrameFormat FrameFormat;
        /// <summary>
        /// ��������� ����� �������� ����: Tx passive
        /// </summary>
        public Boolean ListenOnlyMode;
        /// <summary>
        /// ��������� ���� ��������� Error Frame
        /// </summary>
        //public Boolean ErrorFrameEnable = true;
        /// <summary>
        /// ��������� ��������������� ����� ������ CAN-����������
        /// ????????
        /// </summary>
        //public Boolean LowSpeedModeEnable = false;
        public struct DefaultSettings
        { 
            public const BaudRate BitRateDefault = BaudRate.BR10;
            public const FrameFormat FrameFormatDefault = FrameFormat.MixedFrame;
            public const Boolean ListenOlnyModeDefault = false;
        }
    }
}
