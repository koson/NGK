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

        public void ShowNavigationMenu()
        {
            INavigationMenuPresenter presenter = 
                _Managers.PresentersFactory.CreateNavigationMenu();
            
            // Устанавливаем окно (кнопку привязанную к нему заблокируем)
            presenter.SelectedWindow = (NavigationMenuItems)Enum.Parse(
                typeof(NavigationMenuItems), _Application.CurrentPresenter.Name, true);

            _Application.ShowDialog(presenter);

            GoToWindow(presenter.SelectedWindow);
        }

        public void GoToWindow(NavigationMenuItems window)
        {
            NavigationMenuItems current;
            IPresenter presenter;

            try
            {
                current = (NavigationMenuItems)Enum.Parse(
                    typeof(NavigationMenuItems), _Application.CurrentPresenter.Name, true);
            }
            catch
            {
                current = NavigationMenuItems.NoSelection;
            }

            if (current == window)
            {
                return;
            }

            switch (window)
            {
                case NavigationMenuItems.NoSelection:
                    { return; }
                case NavigationMenuItems.PivoteTable:
                    {
                        if (_Application.CurrentPresenter is MainWindowPresenter)
                        {
                            MainWindowPresenter windowPresenter =
                                _Application.CurrentPresenter as MainWindowPresenter;
                            presenter = _Managers.PresentersFactory
                                .Create(NavigationMenuItems.PivoteTable);
                        }
                        break;
                    }
                case NavigationMenuItems.DeviceList:
                    {
                        if (_Application.CurrentPresenter is MainWindowPresenter)
                        {
                            MainWindowPresenter windowPresenter =
                                _Application.CurrentPresenter as MainWindowPresenter;
                            presenter = _Managers.PresentersFactory
                                .Create(NavigationMenuItems.DeviceList);
                        }
                        break;
                    }
                case NavigationMenuItems.DeviceDetail:
                    {
                        if (_Application.CurrentPresenter is MainWindowPresenter)
                        {
                            MainWindowPresenter windowPresenter =
                                _Application.CurrentPresenter as MainWindowPresenter;
                            presenter = _Managers.PresentersFactory
                                .Create(NavigationMenuItems.DeviceDetail);
                        }
                        break; 
                    }
                case NavigationMenuItems.LogViewer:
                    {
                        if (_Application.CurrentPresenter is MainWindowPresenter)
                        {
                            MainWindowPresenter windowPresenter =
                                _Application.CurrentPresenter as MainWindowPresenter;
                            presenter = _Managers.PresentersFactory
                                .Create(NavigationMenuItems.LogViewer);
                        }
                        break; 
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
            //_Application.ShowWindow(presenter);
        }

        #endregion
    }
}
