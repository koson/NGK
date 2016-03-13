using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using NGK.CorrosionMonitoringSystem.Presenter;
using NGK.CorrosionMonitoringSystem.Views;

namespace NGK.CorrosionMonitoringSystem.Managers.Factory
{
    public interface IPresentersFactory
    {
        IPresenter Create(NavigationMenuItems window);
        INavigationMenuPresenter CreateNavigationMenu();
    }
}
