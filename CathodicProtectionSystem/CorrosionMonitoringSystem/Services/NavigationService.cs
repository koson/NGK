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

        public void GoToWindow(ViewMode window)
        {
            ViewMode current;
            IPresenter presenter;

            try
            {
                current = (ViewMode)Enum.Parse(
                    typeof(ViewMode), _Application.CurrentPresenter.Name, true);
            }
            catch
            {
                current = ViewMode.NoSelection;
            }

            if (current == window)
            {
                return;
            }

            switch (window)
            {
                case ViewMode.NoSelection:
                    { return; }
                case ViewMode.PivoteTable:
                    {
                        if (_Application.CurrentPresenter is MainWindowPresenter)
                        {
                            MainWindowPresenter windowPresenter =
                                _Application.CurrentPresenter as MainWindowPresenter;
                            presenter = _Managers.PresentersFactory
                                .Create(ViewMode.PivoteTable);
                        }
                        break;
                    }
                case ViewMode.DeviceList:
                    {
                        if (_Application.CurrentPresenter is MainWindowPresenter)
                        {
                            MainWindowPresenter windowPresenter =
                                _Application.CurrentPresenter as MainWindowPresenter;
                            presenter = _Managers.PresentersFactory
                                .Create(ViewMode.DeviceList);
                        }
                        break;
                    }
                case ViewMode.DeviceDetail:
                    {
                        if (_Application.CurrentPresenter is MainWindowPresenter)
                        {
                            MainWindowPresenter windowPresenter =
                                _Application.CurrentPresenter as MainWindowPresenter;
                            presenter = _Managers.PresentersFactory
                                .Create(ViewMode.DeviceDetail);
                        }
                        break; 
                    }
                case ViewMode.LogViewer:
                    {
                        if (_Application.CurrentPresenter is MainWindowPresenter)
                        {
                            MainWindowPresenter windowPresenter =
                                _Application.CurrentPresenter as MainWindowPresenter;
                            presenter = _Managers.PresentersFactory
                                .Create(ViewMode.LogViewer);
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
