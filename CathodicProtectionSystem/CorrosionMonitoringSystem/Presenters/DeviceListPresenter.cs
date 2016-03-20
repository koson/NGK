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

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class DeviceListPresenter : Presenter<IDeviceListView>, IViewMode
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

            _DeviceDetailCommand = new Command(new CommandAction(OnDeviceDetail),
                new Condition(CanDeviceDetail));
            _Commands.Add(_DeviceDetailCommand);

            _BindingSourceDevices = new BindingSource();
            _BindingSourceDevices.CurrentItemChanged += 
                new EventHandler(EventHandler_BindingSourceDevices_CurrentItemChanged);
            _BindingSourceDevices.ListChanged += new System.ComponentModel.ListChangedEventHandler(_BindingSourceDevices_ListChanged);
            _BindingSourceDevices.DataSource = _Managers.CanNetworkService.Devices;

            //view.ButtonF3Text = "Подробно";
            //view.ButtonF4Text = "Сбросить ошибку";
            //view.ButtonF5Text = "Запуск системы";
            //view.ButtonClick +=
            //    new EventHandler<ButtonClickEventArgs>(EventHandler_View_ButtonClick);
            view.Devices = _BindingSourceDevices;
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

        MainWindowPresenter HostWindowPresenter
        {
            get { return (MainWindowPresenter)HostPresenter; }
        }

        public ViewMode ViewMode { get { return ViewMode.DeviceList; } }

        #endregion

        #region Event Handlers

        void EventHandler_View_ButtonClick(object sender, ButtonClickEventArgs e)
        {
            switch (e.Button)
            {
                case SystemButtons.F2:
                    {
                        break;
                    }
                case SystemButtons.F3:
                    {
                        _DeviceDetailCommand.Execute();
                        break;
                    }
            }
        }

        void EventHandler_BindingSourceDevices_CurrentItemChanged(object sender, EventArgs e)
        {
            //_View.ButtonF3IsAccessible = _DeviceDetailCommand.CanExecute();
        }

        void _BindingSourceDevices_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            //_View.ButtonF3IsAccessible = _DeviceDetailCommand.CanExecute();
            return;
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
