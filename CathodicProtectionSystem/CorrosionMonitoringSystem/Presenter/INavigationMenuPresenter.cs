using System;
using Mvp.Presenter;
using NGK.CorrosionMonitoringSystem.View;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public interface INavigationMenuPresenter : IPresenter
    {
        NavigationMenuItems SelectedWindow { get; set; }

        //void SetDeviceDetailScreen();
        //void SetDeviceListScreen();
        //void SetPivotTableScreen();
        //void TimeDiagramScreen();
    }
}
