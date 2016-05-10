using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public static class ServiceHelper
    {
        /// <summary>
        /// Наименования сервисов приложения
        /// </summary>
        public struct ServiceNames
        {
            /// <summary>
            /// Сервис управления сетями NGK CAN
            /// </summary>
            public static string NgkCanService = "NgkCanService";
            /// <summary>
            /// Сервис для поддержки систем верхнего уровня
            /// по протоколу Modbus (режим Slave)
            /// </summary>
            public static string SystemInformationModbusService = "SysInfoModbusService";
        }
    }
}
