using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.View;
using NGK.CorrosionMonitoringSystem.Models;
using NGK.CorrosionMonitoringSystem.Managers;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public class DeviceListPresenter : Presenter<IDeviceListView>
    {
        #region Constructors

        public DeviceListPresenter(IApplicationController application,
            IDeviceListView view, object model, IManagers managers)
            : 
            base(view, application)
        {
            _Name = NavigationMenuItems.DeviceList.ToString();
            _Managers = managers;

            _ShowMenuCommand = new Command(
                new CommandAction(OnShowMenu), new Condition(CanShowMenu));
            _DeviceDetailCommand = new Command(new CommandAction(OnDeviceDetail),
                new Condition(CanDeviceDetail));

            _BindingSourceDevices = new BindingSource();
            _BindingSourceDevices.CurrentItemChanged += 
                new EventHandler(EventHandler_BindingSourceDevices_CurrentItemChanged);
            _BindingSourceDevices.ListChanged += new System.ComponentModel.ListChangedEventHandler(_BindingSourceDevices_ListChanged);
            _BindingSourceDevices.DataSource = _Managers.CanNetworkService.Devices;

            view.ButtonF3Text = "Подробно";
            view.ButtonF4Text = "Сбросить ошибку";
            view.ButtonF5Text = "Запуск системы";
            view.ButtonClick +=
                new EventHandler<ButtonClickEventArgs>(EventHandler_View_ButtonClick);
            view.Devices = _BindingSourceDevices;
        }

        void _BindingSourceDevices_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            _View.ButtonF3IsAccessible = _DeviceDetailCommand.CanExecute();
            return;
        }

        #endregion

        #region Fields And Properties

        IManagers _Managers;
        BindingSource _BindingSourceDevices;

        IDeviceListView ViewConcrete
        {
            get { return (IDeviceListView)_View; }
        }

        public NgkCanDevice SelectedDevice
        {
            get 
            {
                return _BindingSourceDevices.Current == null ? null : 
                    (NgkCanDevice)_BindingSourceDevices.Current; 
            }
        }

        #endregion

        #region Event Handlers

        void EventHandler_View_ButtonClick(object sender, ButtonClickEventArgs e)
        {
            switch (e.Button)
            {
                case TemplateView.Buttons.F2:
                    {
                        _ShowMenuCommand.Execute();
                        break;
                    }
                case TemplateView.Buttons.F3:
                    {
                        _DeviceDetailCommand.Execute();
                        break;
                    }
            }
        }

        void EventHandler_BindingSourceDevices_CurrentItemChanged(object sender, EventArgs e)
        {
            _View.ButtonF3IsAccessible = _DeviceDetailCommand.CanExecute();
        }

        #endregion

        #region Commands

        Command _ShowMenuCommand;

        void OnShowMenu()
        {
            _Managers.NavigationService.ShowNavigationMenu();
        }

        bool CanShowMenu()
        {
            return true;
        }

        Command _DeviceDetailCommand;

        void OnDeviceDetail()
        {
            DeviceDetailPresenter presenter = 
                (DeviceDetailPresenter)_Managers.PresentersFactory.Create(NavigationMenuItems.DeviceDetail);
            presenter.Device = SelectedDevice;
            presenter.Show();
        }

        bool CanDeviceDetail()
        {
            return SelectedDevice != null;
        }

        #endregion

    }
}
