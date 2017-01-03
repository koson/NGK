using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication.Infrastructure;
using Mvp.Input;
using Infrastructure.Api.Managers;
using NGK.Plugins.ApplicationServices;

namespace NGK.Plugins
{
    public class ModbusPlugin : Plugin
    {
        #region Constructors

        public ModbusPlugin() 
        {
            Name = @"Modbus";
        }

        #endregion

        #region Fields And Properties

        private ModbusService _ModbusService;

        #endregion

        #region Methods

        public override void Initialize(IManagers managers, object state)
        {
            base.Initialize(managers, state);
            
            // Создаём сервисы приложения и регистрируем их
            try
            {
                _ModbusService = new ModbusService(Managers);
                _ModbusService.Initialize(null);
                base.ApplicationServices.Add(_ModbusService);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    String.Format("Ошибка при инициализации плагина {0}", Name), ex);
            }
        }

        #endregion

        #region Commands
        #endregion
    }
}
