using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Presenters;
using Mvp.View;
using NGK.CorrosionMonitoringSystem.Views.LogViewerView;

namespace NGK.CorrosionMonitoringSystem.Managers.Factory
{
    public class PresentersFactory: IPresentersFactory
    {
        #region Constructors

        public PresentersFactory(IApplicationController application, 
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

        #region Methods

        public IPresenter Create(ViewMode window)
        {
            IPresenter presenter;

            switch (window)
            {
                case ViewMode.NoSelection:
                    {
                        MainWindowView mainWindow = new MainWindowView();

                        MainWindowPresenter concretePresenter =
                            new MainWindowPresenter(_Application, mainWindow, null, _Managers);
                        presenter = concretePresenter;
                        break;
                    }
                case ViewMode.PivoteTable:
                    {
                        PivotTableView ptView = new PivotTableView();

                        PivoteTablePresenter ptPresenter =
                            new PivoteTablePresenter(_Application, ptView, null, null, _Managers);
                        presenter = ptPresenter;
                        break;
                    }
                case ViewMode.DeviceList:
                    {
                        DevicesView dlView = new DevicesView();

                        DeviceListPresenter dlPresenter =
                            new DeviceListPresenter(_Application, dlView, null, null, _Managers);
                        presenter = dlPresenter;
                        break;
                    }
                case ViewMode.DeviceDetail:
                    {
                        throw new NotImplementedException();

                        //DeviceDetailView ddView = new DeviceDetailView();

                        //// Настраиваем окно
                        //ddView.ShowInTaskbar = _Managers.ConfigManager.ShowInTaskbar;
                        //ddView.FormBorderStyle =
                        //    _Managers.ConfigManager.FormBorderEnable ?
                        //    FormBorderStyle.Sizable : FormBorderStyle.None;

                        //DeviceDetailPresenter ddPresenter =
                        //    new DeviceDetailPresenter(_Application, ddView, null, _Managers, null);
                        //presenter = ddPresenter;
                        //break; 
                    }
                case ViewMode.LogViewer:
                    {
                        LogViewerView lvView = new LogViewerView();

                        LogViewerPresenter lvPresenter =
                            new LogViewerPresenter(_Application, lvView, null, null, _Managers);
                        presenter = lvPresenter;
                        break; 
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
            return presenter;
        }
        
        public INavigationMenuPresenter CreateNavigationMenu()
        {
            NavigationMenuView view = new NavigationMenuView();
            view.ShowInTaskbar = false;
            view.FormBorderStyle = FormBorderStyle.FixedDialog;
            view.StartPosition = FormStartPosition.CenterScreen;

            INavigationMenuPresenter presenter =
                new NavigationMenuPresenter(_Application, view, null, null);

            return presenter;
        }

        #endregion
    }
}
