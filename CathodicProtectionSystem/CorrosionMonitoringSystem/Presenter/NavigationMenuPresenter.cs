using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.View;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public class NavigationMenuPresenter : INavigationMenuPresenter
    {
        #region Constructors

        public NavigationMenuPresenter(IApplicationController application,
            INavigationMenuView view, object model, IManagers managers) 
        {
            _View = view;
        }
        
        #endregion

        #region Fields And Properties

        INavigationMenuView _View;
        
        public IView View
        {
            get { return _View; }
        }

        IPresenter _SelectedWindow;

        public IPresenter SelectedWindow
        {
            get { return _SelectedWindow; }
            set { _SelectedWindow = value; }
        }

        #endregion

        #region Methods

        public void SetPivotTableScreen()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetDeviceListScreen()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetDeviceDetailScreen()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void TimeDiagramScreen()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
