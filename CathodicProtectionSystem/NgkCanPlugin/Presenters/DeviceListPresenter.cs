using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using System.Windows.Forms;
using Mvp.View;
using System.ComponentModel;
using NGK.Plugins.Views;
using Infrastructure.Api.Plugins;
using NGK.Plugins.Models;

namespace NGK.Plugins.Presenters
{
    public class DevicesListPresenter : PartialViewPresenter<DevicesListView>
    {
        #region Constructors

        public DevicesListPresenter()
        {
            _DeviceDetailCommand = new Command("Подробно", 
                new CommandAction(OnDeviceDetail), new Condition(CanDeviceDetail));
            _Commands.Add(_DeviceDetailCommand);

            Devices = new BindingList<IDeviceInfo>();

            if (NgkCanPlugin.CanNetworkService != null)
            {
                foreach (NgkCanDevice device in NgkCanPlugin.CanNetworkService.Devices)
                {
                    Devices.Add(device);
                }
            }
            
            //view.SelectedDeviceChanged += 
            //    new EventHandler(EventHandler_view_SelectedDeviceChanged);
        }

        #endregion

        #region Fields And Properties

        private NgkCanDevice _SelectedDevice;
        private BindingList<IDeviceInfo> _Devices;

        public BindingList<IDeviceInfo> Devices
        {
            get { return _Devices; }
            private set { _Devices = value; }
        }

        public NgkCanDevice SelectedDevice
        {
            get { return _Devices ; }
            set 
            { 
                _Devices = value;
                _DeviceDetailCommand.CanExecute(); 
            }
        }

        public Command[] ButtonCommands
        {
            get { return new Command[] { _DeviceDetailCommand }; }
        }

        public ICommand[] MenuItems
        {
            get { return new Command[] { _DeviceDetailCommand }; }
        }

        #endregion

        #region Commands

        Command _DeviceDetailCommand;

        void OnDeviceDetail()
        {
            DeviceDetailPresenter presenter = 
                (DeviceDetailPresenter)_Managers.PresentersFactory.Create(
                ViewMode.DeviceDetail);
            presenter.Device = SelectedDevice;
            ((MainWindowPresenter)_Application.CurrentPresenter)
                .WorkingRegionPresenter = presenter;
            presenter.Show();
        }

        bool CanDeviceDetail()
        {
            return SelectedDevice != null;
        }

        #endregion

        #region Methods

        public override void Show()
        {
            base.Show();
            HostWindowPresenter.Title = @"Устройства";
        }

        #endregion
    }
}
