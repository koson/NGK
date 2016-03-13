using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;
using Mvp.Presenter;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public class MainWindowPresenter : Presenter<IMainWindowView>
    {
        #region Constructors

        public MainWindowPresenter(IApplicationController application,
            IMainWindowView view, object model, IManagers managers)
            : 
            base(view, application)
        {
            _Name = String.Empty;
            _Managers = managers;
            ViewConcrete.Title = String.Empty;
        }

        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public IMainWindowView ViewConcrete
        {
            get { return (IMainWindowView)base.View; }
        }

        #endregion

    }
}
