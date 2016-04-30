using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface ISystemInformationModbusNetworkService : IManageable, IDisposable
    {
        #region Properties
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        byte Address { get; set; }

        #endregion
        
        #region Methods
        /// <summary>
        /// Инициализирует сервис
        /// </summary>
        void Initialize();

        #endregion
    }
}
