using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Presenters;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public class NavigationService: INavigationService
    {
        #region Constructors

        public NavigationService(IApplicationController application, 
            IManagers managers)
        {
            _Application = application;
            _Managers = managers;
        }

        #endregion

        #region Fields And Properties

        IApplicationController _Application;
        IManagers _Managers;
        
        #endregion

        #region INavigationService Members

        public ViewMode ShowNavigationMenu(ViewMode currentViewMode)
        {
            INavigationMenuPresenter presenter = 
                _Managers.PresentersFactory.CreateNavigationMenu();
            
            presenter.CurrentViewMode = currentViewMode;
            presenter.Show();

            return presenter.CurrentViewMode;
        }

        #endregion
    }
}
