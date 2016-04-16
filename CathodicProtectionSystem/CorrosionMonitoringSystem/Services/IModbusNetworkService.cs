using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface IModbusSystemInformationNetworkService : IManageable, IDisposable
    {
        /// <summary>
        /// Инициализирует сервис
        /// </summary>
        void Initialize();
    }
}
