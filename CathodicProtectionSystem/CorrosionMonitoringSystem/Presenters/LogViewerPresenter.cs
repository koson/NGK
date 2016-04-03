using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class LogViewerPresenter : Presenter<ILogViewerView>, 
        IViewMode
    {
        #region Constructors

        public LogViewerPresenter(IApplicationController application,
            ILogViewerView view, IViewRegion region, object model, 
            IManagers managers)
            :
            base(view, region, application)
        {
            _Name = ViewMode.LogViewer.ToString();
            _Managers = managers;
        }

        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public ILogViewerView ViewConcrete
        {
            get { return (ILogViewerView)base.View; }
        }

        public ViewMode ViewMode { get { return ViewMode.DeviceDetail; } }

        MainWindowPresenter HostWindowPresenter
        {
            get { return (MainWindowPresenter)HostPresenter; }
        }

        #endregion

        #region Methods

        public override void Show()
        {
            base.Show();
            HostWindowPresenter.Title = @"Журнал событий";
        }

        #endregion

        #region Event Handlers
        #endregion

        #region Commands
        #endregion
    }
}
