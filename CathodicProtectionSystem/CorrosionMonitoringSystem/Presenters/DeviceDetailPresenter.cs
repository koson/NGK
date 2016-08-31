using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Managers;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Models;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class DeviceDetailPresenter: RegionPresenter<DeviceDetailView>, 
        IViewMode, ISystemButtons
    {
        #region Constructors

        public DeviceDetailPresenter(IApplicationController application,
            IDeviceDetailView view, IViewRegion region, object model, 
            IManagers managers, NgkCanDevice device)
            :
            base(view, region, application)
        {
            _Name = ViewMode.DeviceDetail.ToString();
            _Managers = managers;

            _Parameters = new BindingList<Parameter>();
            Device = device;
            ViewConcrete.Parameters = _Parameters;
        }

        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public IDeviceDetailView ViewConcrete
        {
            get { return (IDeviceDetailView)base.View; }
        }

        NgkCanDevice _Device;
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

        BindingList<Parameter> _Parameters;

        public BindingList<Parameter> Parameters
        {
            get { return _Parameters; }
        }

        public ViewMode ViewMode { get { return ViewMode.DeviceDetail; } }

        MainWindowPresenter HostWindowPresenter
        {
            get { return (MainWindowPresenter)HostPresenter; }
        }

        public Command[] ButtonCommands
        {
            get { return null; }
        }

        #endregion

        #region Methods

        public override void Show()
        {
            base.Show();
            HostWindowPresenter.Title = @"Информация об устройстве";
        }

        #endregion

        #region Event Handlers

        #endregion

        #region Commands
        #endregion
    }
}
