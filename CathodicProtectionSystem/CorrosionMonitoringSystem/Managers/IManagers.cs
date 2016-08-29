using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.LogManager;
using NGK.CorrosionMonitoringSystem.Managers.SysLogManager;
using NGK.CorrosionMonitoringSystem.Managers.AppConfigManager;
using NGK.CorrosionMonitoringSystem.Managers.Factory;
using NGK.CorrosionMonitoringSystem.Services;

namespace NGK.CorrosionMonitoringSystem.Managers
{
    public interface IManagers
    {
        /// <summary>
        /// Версия ПО
        /// </summary>
        Version SoftwareVersion { get; }
        /// <summary>
        /// Версия аппаратуры
        /// </summary>
        Version HardwareVersion { get; }
        /// <summary>
        /// Менеждер для трассировки приложения
        /// </summary>
        ILogManager Logger { get; }
        /// <summary>
        /// Менеджер для записи сообщений приложения
        /// в журнал событий приложения
        /// </summary>
        ISysLogManager SystemLogger { get; }
        /// <summary>
        /// Менеджер для работы с файлом конфигурации
        /// приложения
        /// </summary>
        IConfigManager ConfigManager { get; }
        /// <summary>
        /// Сервис для работы с CAN-сетями
        /// </summary>
        ICanNetworkService CanNetworkService { get; }
        /// <summary>
        /// Сервис для работы с Modbus-сетью 
        /// (режим Slave: для поддержики систем верхнего уровня )
        /// </summary>
        ISystemInformationModbusNetworkService ModbusSystemInfoNetworkService { get; }
    }
}
