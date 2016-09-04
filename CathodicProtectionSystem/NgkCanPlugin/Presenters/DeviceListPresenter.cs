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
using Infrastructure.API.Models.CAN;

namespace NGK.Plugins.Presenters
{
    public class DevicesListPresenter : PartialViewPresenter<DevicesListView>
    {
        #region Constructors

        public DevicesListPresenter(NgkCanPlugin plugin)
        {
            _Plugin = plugin;

            _DeviceDetailCommand = new Command("Подробно", 
                new CommandAction(OnDeviceDetail), new Condition(CanDeviceDetail));
            _Commands.Add(_DeviceDetailCommand);

            _Devices = new BindingList<IDeviceInfo>();

            if (Plugin.CanNetworkService != null)
            {
                foreach (NgkCanDevice device in Plugin.CanNetworkService.Devices)
                {
                    Devices.Add(device);
                }
            }

            View.Devices = Devices;

            _ButtonDeviceDetail = new Button();
            _ButtonDeviceDetail.Name = "_ButtonDeviceDetail";
            _ButtonDeviceDetail.Text = "Подробно";
            _ButtonDeviceDetail.Click += 
                new EventHandler(EventHandler_ButtonDeviceDetail_Click);
            FunctionalButtons.Add(_ButtonDeviceDetail);
        }

        #endregion

        #region Fields And Properties

        private readonly BindingList<IDeviceInfo> _Devices;
        private readonly NgkCanPlugin _Plugin;
        private readonly Button _ButtonDeviceDetail;

        public override string Title
        {
            get { return "Устройства"; }
        }

        public BindingList<IDeviceInfo> Devices
        {
            get { return _Devices; }
        }

        public NgkCanDevice SelectedDevice
        {
            get { return View.SelectedDevice; }
        }

        private NgkCanPlugin Plugin 
        { 
            get { return _Plugin; } 
        }

        #endregion

        #region Commands

        Command _DeviceDetailCommand;

        void OnDeviceDetail()
        {
            //DeviceDetailPresenter presenter = 
            //    (DeviceDetailPresenter)_Managers.PresentersFactory.Create(
            //    ViewMode.DeviceDetail);
            //presenter.Device = SelectedDevice;
            //((MainWindowPresenter)_Application.CurrentPresenter)
            //    .WorkingRegionPresenter = presenter;
            //presenter.Show();
            MessageBox.Show("", "");
        }

        bool CanDeviceDetail()
        {
            return SelectedDevice != null;
        }

        #endregion

        #region Methods

        public override void Show()
        {
            Plugin.HostWindow.Show(this);
            base.Show();
        }

        public override void Close()
        {
            View.Close();
        }

        private void EventHandler_ButtonDeviceDetail_Click(object sender, EventArgs e)
        {
            _DeviceDetailCommand.Execute();
        }

        #endregion
    }
}
