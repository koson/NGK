using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Presenters;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface INavigationService
    {
        ViewMode ShowNavigationMenu(ViewMode currentViewMode);
        ViewMode? CurrentViewMode { get; }
        MainWindowPresenter MainWindowPresenter { get; set; }
    }
}
