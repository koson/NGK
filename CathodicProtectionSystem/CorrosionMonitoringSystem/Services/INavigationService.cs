using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.View;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface INavigationService
    {
        void ShowNavigationMenu();
        void GoToWindow(NavigationMenuItems window);
    }
}
