using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.DL.ModbusAddresses
{
    /// <summary>
    /// Адреса "визитной карточки" записей файла устройства 
    /// </summary>
    public class ModbusVisitingCard
    {
        /// <summary>
        /// Адреса визитной карты устройства
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
        /// Служебная информация об устройства
        /// </summary>
        public class ServiceInformation
        {
            /// <summary>
            /// Код физического уровня сети, в которой работает данное устройство
            /// </summary>
            public const UInt16 NetworkType = 0x0008;
            /// <summary>
            /// Номер сети (порта) подклю-чённой к блоку КССМ(У)
            /// </summary>
            public const UInt16 NetworkNumber = 0x0009;
            /// <summary>
            /// Сетевой адрес устройства или сетевой идентификатор (CAN Node Id)
            /// </summary>
            public const UInt16 NetwrokAddress = 0x000A;
            /// <summary>
            /// Наличие связи с устройством.
            /// </summary>
            public const UInt16 ConectionStatus = 0x000B;
        }
    }
}
