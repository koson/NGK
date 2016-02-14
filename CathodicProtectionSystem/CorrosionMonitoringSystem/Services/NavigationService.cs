using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Presenter;
using NGK.CorrosionMonitoringSystem.View;
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
                _Managers.WindowsFactory.CreateNavigationMenu();
            
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
                        presenter = _Managers.WindowsFactory
                            .Create(NavigationMenuItems.PivoteTable);
                        break;
                    }
                case NavigationMenuItems.DeviceList:
                    {
                        presenter = _Managers.WindowsFactory
                            .Create(NavigationMenuItems.DeviceList);
                        break;
                    }
                case NavigationMenuItems.DeviceDetail:
                    {
                        presenter = _Managers.WindowsFactory
                            .Create(NavigationMenuItems.DeviceDetail);
                        break; 
                    }
                case NavigationMenuItems.LogViewer:
                    {
                        presenter = _Managers.WindowsFactory
                            .Create(NavigationMenuItems.LogViewer);
                        break; 
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
            _Application.ShowWindow(presenter);
        }

        #endregion
    }
}
