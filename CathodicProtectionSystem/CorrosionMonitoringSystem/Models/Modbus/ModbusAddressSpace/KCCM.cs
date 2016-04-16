using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.Models.Modbus
{
    /// <summary>
    /// Адреса регистров блока КССМУ
    /// </summary>
    public class KCCM
    {
        public class InputRegister
        {
            public const UInt16 DeviceType = 0x0000;
            public const UInt16 SoftwareVersion = 0x0001;
            public const UInt16 HardwareVersion = 0x0002;
            public const UInt16 SerialNumberHigh = 0x0003;
            public const UInt16 SerialNumberMiddle = 0x0004;
            public const UInt16 SerialNumberLow = 0x0005;
            public const UInt16 CRC16 = 0x0006;
            public const UInt16 VendorCode = 0x0007;
            public const UInt16 TotalDevices = 0x0008;
        }
        public class HoldingRegisters
        {
            public const UInt16 DateTimeUnixHigh = 0x0000;
            public const UInt16 DateTimeUnixLow = 0x0001;
        }
    }
}
