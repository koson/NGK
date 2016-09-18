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
using Infrastructure.Api.Controls;

namespace NGK.Plugins.Presenters
{
    public class DevicesListPresenter : PartialViewPresenter<DevicesListView>
    {
        #region Constructors

        public DevicesListPresenter(NgkCanPlugin plugin)
        {
            _Plugin = plugin;

            _ShowDeviceDetailCommand = new Command("Подробно", 
                new CommandAction(OnShowDeviceDetail), new Condition(CanShowDeviceDetail));
            _Commands.Add(_ShowDeviceDetailCommand);

            _Devices = new BindingList<IDeviceInfo>();

            if (Plugin.CanNetworkService != null)
            {
                foreach (NgkCanDevice device in Plugin.CanNetworkService.Devices)
                {
                    Devices.Add(device);
                }
            }

            View.Devices = Devices;

            _ButtonDeviceDetail = new FunctionalButton(_ShowDeviceDetailCommand, Keys.F4);
            _ButtonDeviceDetail.Name = "_FunctionalButtonDeviceDetail";
            _ButtonDeviceDetail.Text = "Подробно";

            FunctionalButtons.Add(_ButtonDeviceDetail);
        }

        #endregion

        #region Fields And Properties

        private readonly BindingList<IDeviceInfo> _Devices;
        private readonly NgkCanPlugin _Plugin;
        private readonly FunctionalButton _ButtonDeviceDetail;

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

        private Command _ShowDeviceDetailCommand;

        private void OnShowDeviceDetail()
        {
            DeviceDetailPresenter presenter = new DeviceDetailPresenter(Plugin);
            presenter.Device = SelectedDevice;
            Plugin.HostWindow.Show(presenter);
        }

        private bool CanShowDeviceDetail()
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
            base.Close();
        }

        private void EventHandler_ButtonDeviceDetail_Click(object sender, EventArgs e)
        {
            _ShowDeviceDetailCommand.Execute();
        }

        #endregion
    }
}
