using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Common.Controlling;
using NGK.CorrosionMonitoringSystem.Models;
using System.Data;
using NGK.CAN.ApplicationLayer.Network.Devices;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface ICanNetworkService: IManageable
    {
        #region Properties
        /// <summary>
        /// Список устройств в системе
        /// </summary>
        BindingList<NgkCanDevice> Devices { get; }
        /// <summary>
        /// Количество неисправных устройтсв в системе.
        /// </summary>
        int FaultyDevices { get; }
        /// <summary>
        /// Сводная таблица параметров системы
        /// </summary>
        DataTable ParametersPivotTable { get; }

        #endregion

        #region Methods
        #endregion

        #region Events

        /// <summary>
        /// Событие происходит при изменении количества
        /// неисправных устройств в системе.
        /// </summary>
        event EventHandler FaultyDevicesChanged;

        #endregion
    }
}
