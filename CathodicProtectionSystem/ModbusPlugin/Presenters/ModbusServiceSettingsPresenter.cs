using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using NGK.Plugins.Views;
using Infrastructure.Api.Managers;
using NGK.Plugins.ApplicationServices;

namespace NGK.Plugins.Presenters
{
    public class ModbusServiceSettingsPresenter : PartialViewPresenter<ModbusServiceSettingsView>
    {
        #region Constructors

        public ModbusServiceSettingsPresenter(IManagers managers, ModbusService modbusService)
        {
            _Managers = managers;
            _ModbusService = modbusService;
            UpdateStatusCommands();
            
        }

        #endregion

        #region Fields And Properties

        private readonly IManagers _Managers;
        private readonly ModbusService _ModbusService;

        public override string Title
        {
            get { return @"Настройки сервиса Modbus"; }
        }

        #endregion

        #region Methods

        public override void Close()
        {
            View.Close();
            base.Close();
        }

        public override void Show()
        {
            _Managers.PartialViewService.Host.Show(this);
            base.Show();
        }

        #endregion

        #region Commands
        #endregion
    }
}
