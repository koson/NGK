using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using NGK.CorrosionMonitoringSystem.Presenters;
using NGK.CorrosionMonitoringSystem.Views;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Managers.Factory
{
    public interface IPresentersFactory
    {
        IPresenter CreateMainWindow();
        IPresenter Create(ViewMode viewMode);
        INavigationMenuPresenter CreateNavigationMenu();
    }
}
