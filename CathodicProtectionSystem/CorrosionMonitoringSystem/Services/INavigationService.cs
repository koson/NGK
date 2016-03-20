using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Views;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface INavigationService
    {
        ViewMode ShowNavigationMenu(ViewMode currentViewMode);
        void GoToWindow(ViewMode window);
    }
}
