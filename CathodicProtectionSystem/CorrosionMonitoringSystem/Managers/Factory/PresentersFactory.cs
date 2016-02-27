using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.View;
using NGK.CorrosionMonitoringSystem.Presenter;

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

        public IPresenter Create(NavigationMenuItems window)
        {
            IPresenter presenter;

            switch (window)
            {
                case NavigationMenuItems.PivoteTable:
                    {
                        PivotTableView ptView = new PivotTableView();

                        // Настраиваем окно
                        ptView.ShowInTaskbar = _Managers.ConfigManager.ShowInTaskbar;
                        ptView.FormBorderStyle =
                            _Managers.ConfigManager.FormBorderEnable ?
                            FormBorderStyle.Sizable : FormBorderStyle.None;

                        PivoteTablePresenter ptPresenter =
                            new PivoteTablePresenter(_Application, ptView, null, _Managers);
                        presenter = ptPresenter;
                        break;
                    }
                case NavigationMenuItems.DeviceList:
                    {
                        DeviceListView dlView = new DeviceListView();

                        // Настраиваем окно
                        dlView.ShowInTaskbar = _Managers.ConfigManager.ShowInTaskbar;
                        dlView.FormBorderStyle =
                            _Managers.ConfigManager.FormBorderEnable ?
                            FormBorderStyle.Sizable : FormBorderStyle.None;

                        DeviceListPresenter dlPresenter =
                            new DeviceListPresenter(_Application, dlView, null, _Managers);
                        presenter = dlPresenter;
                        break;
                    }
                case NavigationMenuItems.DeviceDetail:
                    {
                        DeviceDetailView ddView = new DeviceDetailView();

                        // Настраиваем окно
                        ddView.ShowInTaskbar = _Managers.ConfigManager.ShowInTaskbar;
                        ddView.FormBorderStyle =
                            _Managers.ConfigManager.FormBorderEnable ?
                            FormBorderStyle.Sizable : FormBorderStyle.None;

                        DeviceDetailPresenter ddPresenter =
                            new DeviceDetailPresenter(_Application, ddView, null, _Managers, null);
                        presenter = ddPresenter;
                        break; 
                    }
                case NavigationMenuItems.LogViewer:
                    {
                        LogViewerView lvView = new LogViewerView();

                        // Настраиваем окно
                        lvView.ShowInTaskbar = _Managers.ConfigManager.ShowInTaskbar;
                        lvView.FormBorderStyle =
                            _Managers.ConfigManager.FormBorderEnable ?
                            FormBorderStyle.Sizable : FormBorderStyle.None;

                        LogViewerPresenter lvPresenter =
                            new LogViewerPresenter(_Application, lvView, null, _Managers);
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
