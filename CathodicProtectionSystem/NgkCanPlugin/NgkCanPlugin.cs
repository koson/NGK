using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication.Infrastructure;
using NGK.Plugins.Services;
using NGK.CAN.ApplicationLayer.Network.Master;
using Mvp.WinApplication.ApplicationService;
using Mvp.Input;
using NGK.Plugins.Presenters;
using Infrastructure.Api.Managers;
using System.Windows.Forms;
using Mvp.Controls;
using Infrastructure.Api.Models.CAN;

namespace NGK.Plugins
{
    public class NgkCanPlugin: Plugin
    {
        #region Constructors

        public NgkCanPlugin()
        {
            Name = @"CAN НГК ЭХЗ";

            _ShowDevicesListCommand = new Command(OnShowDevicesList, CanShowDevicesList);
            _UpdateTotalDevicesCommand = new Command(OnUpdateTotalDevices, CanUpdateTotalDevices);
            _UpdateFaultyDevicesCommand = new Command(OnUpdateFaultyDevices, CanUpdateFaultyDevices);
            _ShowPivotTableCommand = new Command(OnShowPivotTable, CanShowPivotTable);

            NavigationMenu = new NavigationMenuItem(Name, null);
            NavigationMenu.SubMenuItems.Add(new NavigationMenuItem("Устройства", _ShowDevicesListCommand));
            NavigationMenu.SubMenuItems.Add(new NavigationMenuItem("Таблица параметров", _ShowPivotTableCommand));

            _BindableToolStripButtonTotalDevices = new BindableToolStripButton();
            _BindableToolStripButtonTotalDevices.Name = "_ToolStripButtonTotalDevices";
            _BindableToolStripButtonTotalDevices.Text = "Всего устройств: ";
            _BindableToolStripButtonTotalDevices.ToolTipText = "Всего устройств в системе";
            _BindableToolStripButtonTotalDevices.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _BindableToolStripButtonTotalDevices.Action = _ShowDevicesListCommand;
            StatusBarItems.Add(_BindableToolStripButtonTotalDevices);

            BindableToolStripButton _BindableToolStripButtonFaultyDevices = 
                new BindableToolStripButton();
            _BindableToolStripButtonFaultyDevices = new BindableToolStripButton();
            _BindableToolStripButtonFaultyDevices.Name = "_ToolStripButtonFaultyDevices";
            _BindableToolStripButtonFaultyDevices.Text = "Неисправных устройств: ";
            _BindableToolStripButtonFaultyDevices.ToolTipText = "Неисправных устройств в системе";
            _BindableToolStripButtonFaultyDevices.DisplayStyle = ToolStripItemDisplayStyle.Text;
            _BindableToolStripButtonFaultyDevices.DataBindings.Add(
                new Binding("Enabled", _UpdateFaultyDevicesCommand, "Status"));
            StatusBarItems.Add(_BindableToolStripButtonFaultyDevices);

            CanNetworkService = null;
        }

        #endregion

        #region Fields And Properties

        private CanNetworkService _CanNetworkService;
        private readonly BindableToolStripButton _BindableToolStripButtonTotalDevices;
        private readonly BindableToolStripButton _BindableToolStripButtonFaultyDevices;

        private ToolStripButton ToolStripButtonTotalDevices
        {
            get { return _BindableToolStripButtonTotalDevices; }
        }

        internal CanNetworkService CanNetworkService
        {
            get { return _CanNetworkService; }
            set 
            { 
                _CanNetworkService = value;
                _ShowDevicesListCommand.CanExecute();

                if (_UpdateTotalDevicesCommand.CanExecute())
                    _UpdateTotalDevicesCommand.Execute();

                _UpdateFaultyDevicesCommand.CanExecute();
            }
        }
        
        #endregion 

        #region Methods

        public override void Initialize(IManagers managers, object state)
        {
            base.Initialize(managers, state);

            // Создаём сервисы приложения
            try
            {
                // Загружаем конфигурацию из файла
                NgkCanNetworksManager.Instance.LoadConfig(Managers.ConfigManager.PathToAppDirectory +
                    @"\newtorkconfig.bin.nwc");

                //Создаём сетевой сервис и регистрируем его
                CanNetworkService = new CanNetworkService(
                    "NgkCanService", NgkCanNetworksManager.Instance, 300, Managers);
                CanNetworkService.Initialize(null);
                base.ApplicationServices.Add(CanNetworkService);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    String.Format("Ошибка при инициализации плагина {0}", Name), ex);
            }
        }

        public override void OnHostWindowSelectedPartivalViewPresenterChanged()
        {
            _ShowDevicesListCommand.CanExecute();
        }

        #endregion 

        #region Commands

        private Command _ShowDevicesListCommand;
        private void OnShowDevicesList()
        {
            DevicesListPresenter presenter = new DevicesListPresenter(this);
            presenter.Show();
        }
        private bool CanShowDevicesList()
        {
            return (CanNetworkService != null &&
                Managers.PartialViewService.Host.SelectedPartivalViewPresenter == null) ||
                (CanNetworkService != null &&
                Managers.PartialViewService.Host.SelectedPartivalViewPresenter != null &&
                !(Managers.PartialViewService.Host.SelectedPartivalViewPresenter is DevicesListPresenter));
        }

        private Command _UpdateTotalDevicesCommand;
        private void OnUpdateTotalDevices()
        {
            _BindableToolStripButtonTotalDevices.Text = 
                String.Format("Всего устройств: {0}", CanNetworkService.Devices.Count);
        }
        private bool CanUpdateTotalDevices()
        {
            return CanNetworkService != null;
        }

        private Command _UpdateFaultyDevicesCommand;
        private void OnUpdateFaultyDevices()
        {
            _BindableToolStripButtonFaultyDevices.Text =
                String.Format("Неисправных устройств: {0}", CanNetworkService.Devices.Count);
        }
        private bool CanUpdateFaultyDevices()
        {
            return CanNetworkService != null;
        }

        private Command _ShowPivotTableCommand;
        private void OnShowPivotTable()
        {
            PivoteTablePresenter presenter = new PivoteTablePresenter(this);
            presenter.Show();
        }
        private bool CanShowPivotTable()
        {
            return (CanNetworkService != null &&
                  Managers.PartialViewService.Host.SelectedPartivalViewPresenter == null) ||
                  (CanNetworkService != null &&
                  Managers.PartialViewService.Host.SelectedPartivalViewPresenter != null &&
                  !(Managers.PartialViewService.Host.SelectedPartivalViewPresenter is PivoteTablePresenter));
        }

        #endregion
    }
}
