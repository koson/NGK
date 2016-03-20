using System;
using Mvp.Presenter;
using NGK.CorrosionMonitoringSystem.Views;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public interface INavigationMenuPresenter : IPresenter
    {
        ViewMode CurrentViewMode { get; set; }
    }
}
