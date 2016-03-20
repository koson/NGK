using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class NavigationMenuPresenter : Presenter<INavigationMenuView>, 
        INavigationMenuPresenter
    {
        #region Constructors

        public NavigationMenuPresenter(IApplicationController application,
            INavigationMenuView view, object model, IManagers managers) :
            base (view, application)
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

        ViewMode _SelectedVIewMode;

        public ViewMode CurrentViewMode
        {
            get { return _SelectedVIewMode; }
            set 
            { 
                _SelectedVIewMode = value;
                
                if (ViewConcrete != null)
                {
                    ViewConcrete.SelectedMenuItem = _SelectedVIewMode;
                    // Блокируем элементы меню
                    switch (_SelectedVIewMode)
                    {
                        case ViewMode.DeviceList:
                            {
                                ViewConcrete.DeviceListMenuEnabled = false;
                                ViewConcrete.DeviceDetailMenuEnabled = true;
                                ViewConcrete.LogViewerMenuEnabled = true;
                                ViewConcrete.PivoteTableMenuEnabled = true;
                                break;
                            }
                        case ViewMode.DeviceDetail:
                            {
                                ViewConcrete.DeviceListMenuEnabled = true;
                                ViewConcrete.DeviceDetailMenuEnabled = false;
                                ViewConcrete.LogViewerMenuEnabled = false;
                                ViewConcrete.PivoteTableMenuEnabled = false;
                                break;
                            }
                        case ViewMode.PivoteTable:
                            {
                                ViewConcrete.DeviceListMenuEnabled = true;
                                ViewConcrete.DeviceDetailMenuEnabled = false;
                                ViewConcrete.LogViewerMenuEnabled = true;
                                ViewConcrete.PivoteTableMenuEnabled = false;
                                break;
                            }
                        case ViewMode.LogViewer:
                            {
                                ViewConcrete.DeviceListMenuEnabled = true;
                                ViewConcrete.DeviceDetailMenuEnabled = false;
                                ViewConcrete.LogViewerMenuEnabled = false;
                                ViewConcrete.PivoteTableMenuEnabled = true;
                                break;
                            }
                    }
                }
            }
        }

        #endregion

        #region Event Handlers
        
        void EventHandler_View_MenuClosed(object sender, EventArgs e)
        {
            INavigationMenuView view = (INavigationMenuView)sender;
            CurrentViewMode = view.SelectedMenuItem;
        }
        
        #endregion

        #region Methods

        #endregion
    }
}
