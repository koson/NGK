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
            NavigationMenuView view = new NavigationMenuView();
            view.ShowInTaskbar = false;
            view.FormBorderStyle = FormBorderStyle.FixedDialog;
            view.StartPosition = FormStartPosition.CenterScreen;

            INavigationMenuPresenter presenter = 
                new NavigationMenuPresenter(_Application, view, null, null);
            
            // Устанавливаем окно (кнопку привязанную к нему заблокируем)
            presenter.SelectedWindow = (NavigationMenuItems)Enum.Parse(
                typeof(NavigationMenuItems), _Application.CurrentWindow.Name, true);
            
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
                    typeof(NavigationMenuItems), _Application.CurrentWindow.Name, true);
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
                    { break; }
                case NavigationMenuItems.PivoteTable:
                    {
                        presenter = _Managers.WindowsFactory
                            .Create(NavigationMenuItems.PivoteTable);
                        _Application.ShowWindow(presenter);
                        break;
                    }
                case NavigationMenuItems.DeviceList:
                    {

                        presenter = _Managers.WindowsFactory
                            .Create(NavigationMenuItems.DeviceList);
                        _Application.ShowWindow(presenter);
                        break;
                    }
                case NavigationMenuItems.DeviceDetail:
                //{ break; }
                case NavigationMenuItems.LogViewer:
                //{ break; }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }

        #endregion
    }
}
