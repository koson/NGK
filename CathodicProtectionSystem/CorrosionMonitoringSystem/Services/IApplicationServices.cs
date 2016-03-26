using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface IApplicationServices
    {
        IWindowsService WindowsService { get; }
    }
}
