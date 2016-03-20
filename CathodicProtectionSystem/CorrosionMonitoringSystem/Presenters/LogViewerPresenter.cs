using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class LogViewerPresenter : Presenter<ILogViewerView>, IViewMode
    {
        #region Constructors

        public LogViewerPresenter(IApplicationController application,
            ILogViewerView view, object model, IManagers managers)
            :
            base(view, application)
        {
            _Name = ViewMode.LogViewer.ToString();
            _Managers = managers;

            view.ButtonClick += 
                new EventHandler<ButtonClickEventArgs>(EventHandler_View_ButtonClick);
        }


        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public ILogViewerView ViewConcrete
        {
            get { return (ILogViewerView)base.View; }
        }

        public ViewMode ViewMode { get { return ViewMode.DeviceDetail; } }

        #endregion

        #region Event Handlers

        void EventHandler_View_ButtonClick(object sender, ButtonClickEventArgs e)
        {
            switch (e.Button)
            {
                case SystemButtons.F2:
                    {
                        break;
                    }
            }
        }

        #endregion

        #region Commands
        #endregion
    }
}
