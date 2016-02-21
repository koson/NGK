using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Common.Controlling;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface ICanNetworkService: IManageable
    {
        BindingList<NgkCanDevice> Devices { get; }
    }
}
