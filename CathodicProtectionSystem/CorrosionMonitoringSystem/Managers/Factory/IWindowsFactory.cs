using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using NGK.CorrosionMonitoringSystem.Presenter;
using NGK.CorrosionMonitoringSystem.View;

namespace NGK.CorrosionMonitoringSystem.Managers.Factory
{
    public interface IWindowsFactory
    {
        IPresenter Create(NavigationMenuItems window);
        INavigationMenuPresenter CreateNavigationMenu();
    }
}
