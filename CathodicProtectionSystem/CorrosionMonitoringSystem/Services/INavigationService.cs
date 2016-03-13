using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Views;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface INavigationService
    {
        void ShowNavigationMenu();
        void GoToWindow(NavigationMenuItems window);
    }
}
