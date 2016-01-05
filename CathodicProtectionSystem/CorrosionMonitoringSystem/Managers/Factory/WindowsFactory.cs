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
    public class WindowsFactory: IWindowsFactory
    {
        public WindowsFactory(IApplicationController application, 
            IManagers managers)
        {
            _Application = application;
            _Managers = managers;
        }

        IApplicationController _Application;
        IManagers _Managers;

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
                //{ break; }
                case NavigationMenuItems.LogViewer:
                //{ break; }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
            return presenter;
        }
    }
}
