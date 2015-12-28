using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface IApplicationServices
    {
        INavigationService NavigationService { get; }
        IWindowsService WindowsService { get; }
    }
}
