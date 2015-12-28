using System;
using Mvp.Presenter;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public interface INavigationMenuPresenter : IPresenter
    {
        IPresenter SelectedWindow { get; set; }

        //void SetDeviceDetailScreen();
        //void SetDeviceListScreen();
        //void SetPivotTableScreen();
        //void TimeDiagramScreen();
    }
}
