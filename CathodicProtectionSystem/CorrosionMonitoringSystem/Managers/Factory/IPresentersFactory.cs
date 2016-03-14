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
        IPresenter Create(NavigationMenuItems window, IViewRegion region);
        INavigationMenuPresenter CreateNavigationMenu();
    }
}
