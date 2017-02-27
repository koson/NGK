using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Models;
using NGK.CorrosionMonitoringSystem.Managers;
using System.Windows.Forms;
using Mvp.View;
using System.ComponentModel;
using NGK.CorrosionMonitoringSystem.Services;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class DeviceListPresenter : Presenter<IDeviceListView>, IViewMode, 
        ISystemButtons, ISystemMenu
    {
        #region Constructors

        public DeviceListPresenter(IApplicationController application,
            IDeviceListView view, IViewRegion region, object model, 
            IManagers managers)
            : 
            base(view, region, application)
        {
            _Name = ViewMode.DeviceList.ToString();
            _Managers = managers;

            _DeviceDetailCommand = new Command("Подробно", 
                new CommandAction(OnDeviceDetail), new Condition(CanDeviceDetail));
            _Commands.Add(_DeviceDetailCommand);

            BindingList<IDeviceInfo> devices = new BindingList<IDeviceInfo>();

            foreach(NgkCanDevice device in _Managers.CanNetworkService.Devices)
            {
                devices.Add(device);
            }

            view.Devices = devices;
            
            view.SelectedDeviceChanged += 
                new EventHandler(EventHandler_view_SelectedDeviceChanged);

            _Managers.CanNetworkService.StatusWasChanged += 
                new EventHandler(EventHandler_CanNetworkService_StatusWasChanged);
        }

        private void EventHandler_CanNetworkService_StatusWasChanged(object sender, EventArgs e)
        {
            ICanNetworkService service = (ICanNetworkService)sender;
            if (service.Status == Common.Controlling.Status.Running)
            {
                BindingList<IDeviceInfo> devices = new BindingList<IDeviceInfo>();

                foreach (NgkCanDevice device in service.Devices)
                {
                    devices.Add(device);
                }

                ViewConcrete.Devices = null;
                ViewConcrete.Devices = devices;
            }
        }

        #endregion

        #region Fields And Properties

        IManagers _Managers;

        IDeviceListView ViewConcrete
        {
            get { return (IDeviceListView)_View; }
        }

        public NgkCanDevice SelectedDevice
        {
            get { return ViewConcrete.SelectedDevice; }
        }

        MainWindowPresenter HostWindowPresenter
        {
            get { return (MainWindowPresenter)HostPresenter; }
        }

        public ViewMode ViewMode { get { return ViewMode.DeviceList; } }

        public Command[] ButtonCommands
        {
            get { return new Command[] { _DeviceDetailCommand }; }
        }

        public ICommand[] MenuItems
        {
            get { return new Command[] { _DeviceDetailCommand }; }
        }

        #endregion

        #region Event Handlers

        void EventHandler_view_SelectedDeviceChanged(object sender, EventArgs e)
        {
            _DeviceDetailCommand.CanExecute();
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
