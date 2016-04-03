using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Common.Controlling;
using NGK.CorrosionMonitoringSystem.Models;
using System.Data;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface ICanNetworkService: IManageable
    {
        BindingList<NgkCanDevice> Devices { get; }
        /// <summary>
        /// Количество неисправных устройтсв в системе.
        /// </summary>
        int FaultyDevices { get; }
        event EventHandler FaultyDevicesChanged;

    }
}
