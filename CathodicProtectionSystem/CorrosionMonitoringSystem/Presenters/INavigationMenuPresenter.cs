using System;
using Mvp.Presenter;
using NGK.CorrosionMonitoringSystem.Views;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public interface INavigationMenuPresenter : IPresenter
    {
        NavigationMenuItems SelectedWindow { get; set; }
    }
}
