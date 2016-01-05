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
    public class NavigationMenuPresenter : Presenter<INavigationMenuView>, 
        INavigationMenuPresenter
    {
        #region Constructors

        public NavigationMenuPresenter(IApplicationController application,
            INavigationMenuView view, object model, IManagers managers) :
            base (view)
        {
            _Managers = managers;

            ViewConcrete.MenuClosed += 
                new EventHandler(EventHandler_View_MenuClosed);
        }
        
        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public INavigationMenuView ViewConcrete
        {
            get { return (INavigationMenuView)base._View; }
        }

        NavigationMenuItems _SelectedWindow;

        public NavigationMenuItems SelectedWindow
        {
            get { return _SelectedWindow; }
            set 
            { 
                _SelectedWindow = value;
                if (ViewConcrete != null)
                {
                    ViewConcrete.SelectedMenuItem = _SelectedWindow;
                }
            }
        }

        #endregion

        #region Event Handlers
        
        void EventHandler_View_MenuClosed(object sender, EventArgs e)
        {
            INavigationMenuView view = (INavigationMenuView)sender;
            SelectedWindow = view.SelectedMenuItem;
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
