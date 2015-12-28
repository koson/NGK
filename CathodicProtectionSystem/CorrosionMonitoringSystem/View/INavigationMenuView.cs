using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.View
{
    public interface INavigationMenuView: IView
    {
        void ShowPivotTableScreen();
        void ShowDeviceListScreen();
        void ShowDeviceDetailScreen();
        //void ShowTimeDiagramScreen();
        //void ShowLogViewerScreen();
    }
}
