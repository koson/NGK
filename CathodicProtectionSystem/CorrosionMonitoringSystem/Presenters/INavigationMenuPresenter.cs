using System;
using Mvp.Presenter;
using NGK.CorrosionMonitoringSystem.Views;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public interface INavigationMenuPresenter : IPresenter
    {
        ViewMode CurrentViewMode { get; set; }
        ICommand[] MenuItems { set; }
    }
}
