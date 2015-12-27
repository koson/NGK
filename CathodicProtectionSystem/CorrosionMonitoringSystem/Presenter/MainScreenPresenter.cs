using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.View;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public class MainScreenPresenter: IPresenter
    {
        #region Constructors
        
        public MainScreenPresenter(IApplicationController application,
            IMainScreenView view, object model, IManagers managers)
        {
            _Managers = managers;
            _Application = application;
            _View = view;
        }
        
        #endregion

        #region Fields And Properties

        IApplicationController _Application;
        IMainScreenView _View;
        IManagers _Managers;

        public IView View
        {
            get { return _View; }
        }

        #endregion
    }
}
