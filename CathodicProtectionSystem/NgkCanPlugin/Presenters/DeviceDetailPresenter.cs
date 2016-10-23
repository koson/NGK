using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using System.Windows.Forms;
using Mvp.View;
using NGK.Plugins.Views;
using Infrastructure.Api.Plugins;
using Infrastructure.Api.Models.CAN;
using Infrastructure.Api.Controls;

namespace NGK.Plugins.Presenters
{
    public class DeviceDetailPresenter: PartialViewPresenter<DeviceDetailView>
    {
        #region Constructors

        public DeviceDetailPresenter(NgkCanPlugin plugin)
        {
            _ShowDevicesListCommand = new Command(OnShowDevicesList);
            base._Commands.Add(_ShowDevicesListCommand);

            _Plugin = plugin;
            _Parameters = new BindingList<Parameter>();
            View.Parameters = _Parameters;

            _ButtonDevicesList = new FunctionalButton(_ShowDevicesListCommand, Keys.F4);
            _ButtonDevicesList.Name = "_FunctionalButtonDevicesList";
            _ButtonDevicesList.Text = "Устройства";

            FunctionalButtons.Add(_ButtonDevicesList);
        }

        #endregion

        #region Fields And Properties

        private readonly NgkCanPlugin _Plugin;
        private NgkCanDevice _Device;
        private BindingList<Parameter> _Parameters;
        private readonly FunctionalButton _ButtonDevicesList;
        
        public NgkCanDevice Device
        {
            get { return _Device; }
            set 
            { 
                _Device = value;
                if (_Device != null)
                {
                    _Parameters.Clear();
                    foreach (Parameter prm in _Device.Parameters)
                    {
                        _Parameters.Add(prm);
                    }
                }
                else
                {
                    _Parameters.Clear();
                }
            }
        }
        
        public BindingList<Parameter> Parameters
        {
            get { return _Parameters; }
        }
        
        public override string Title
        {
            get { return @"Информация об устройстве"; }
        }

        private NgkCanPlugin Plugin
        {
            get { return _Plugin; }
        }

        #endregion

        #region Methods

        public override void Close()
        {
            View.Close();
            base.Close();
        }

        #endregion

        #region Event Handlers

        #endregion

        #region Commands

        private Command _ShowDevicesListCommand;

        private void OnShowDevicesList()
        {
            DevicesListPresenter presenter = new DevicesListPresenter(Plugin);
            Plugin.HostWindow.Show(presenter);
        }

        #endregion
    }
}
