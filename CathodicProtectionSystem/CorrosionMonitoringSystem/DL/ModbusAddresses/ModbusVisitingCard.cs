using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.DL.ModbusAddresses
{
    /// <summary>
    /// ������ "�������� ��������" ������� ����� ���������� 
    /// </summary>
    public class ModbusVisitingCard
    {
        /// <summary>
        /// ������ �������� ����� ����������
        /// </summary>
        public class VisitingCard
        {
            public const UInt16 DeviceType = 0x0000;
            public const UInt16 SoftwareVersion = 0x0001;
            public const UInt16 HardwareVersion = 0x0002;
            public const UInt16 SerialNumberHigh = 0x0003;
            public const UInt16 SerialNumberMiddle = 0x0004;
            public const UInt16 SerialNumberLow = 0x0005;
            public const UInt16 CRC16 = 0x0006;
            public const UInt16 VendorCode = 0x0007;
        }
        /// <summary>
        /// ��������� ���������� �� ����������
        /// </summary>
        public class ServiceInformation
        {
            /// <summary>
            /// ��� ����������� ������ ����, � ������� �������� ������ ����������
            /// </summary>
            public const UInt16 NetworkType = 0x0008;
            /// <summary>
            /// ����� ���� (�����) ������-������ � ����� ����(�)
            /// </summary>
            public const UInt16 NetworkNumber = 0x0009;
            /// <summary>
            /// ������� ����� ���������� ��� ������� ������������� (CAN Node Id)
            /// </summary>
            public const UInt16 NetwrokAddress = 0x000A;
            /// <summary>
            /// ������� ����� � �����������.
            /// </summary>
            public const UInt16 ConectionStatus = 0x000B;
        }
    }
}
