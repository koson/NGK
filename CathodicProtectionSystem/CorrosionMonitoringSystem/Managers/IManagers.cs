using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Managers.LogManager;
using NGK.CorrosionMonitoringSystem.Managers.SysLogManager;
using NGK.CorrosionMonitoringSystem.Managers.AppConfigManager;
using NGK.CorrosionMonitoringSystem.Managers.Factory;
using NGK.CorrosionMonitoringSystem.Services;

namespace NGK.CorrosionMonitoringSystem.Managers
{
    public interface IManagers
    {
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
        /// Навигация по приложению
        /// </summary>
        INavigationService NavigationService { get; }
        /// <summary>
        /// Фабрика по созданию окон приложения
        /// </summary>
        IPresentersFactory PresentersFactory { get; }
        /// <summary>
        /// Событие возникает при изменении состояния сетевого контроллера
        /// </summary>
        ICanNetworkService CanNetworkService { get; }
    }
}
