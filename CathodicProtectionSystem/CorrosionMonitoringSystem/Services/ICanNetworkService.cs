using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface ICanNetworkService: IManageable
    {
        CanDevice[] Devices { get; }
    }
}
