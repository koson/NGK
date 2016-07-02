using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Managers.AppConfigManager;
using Infrastructure.LogManager;
using NGK.CorrosionMonitoringSystem.Managers.SysLogManager;
using NGK.CorrosionMonitoringSystem.Managers.Factory;
using NGK.CorrosionMonitoringSystem.Services;
using Mvp.WinApplication;
using NGK.CAN.ApplicationLayer.Network.Master;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using Modbus.OSIModel.ApplicationLayer.Slave;
using Modbus.OSIModel.ApplicationLayer;

namespace NGK.CorrosionMonitoringSystem.Managers
{
    public class AppManagers: IManagers
    {
        #region Constructor

        public AppManagers(IApplicationController application)
        {
            _Application = application;
            _ConfigManager = AppConfigManager.ConfigManager.Instance;
            _Logger = Program._Logger;
            _SystemLogger = null; //TODO: Не реализовано
            _WindowsFactory = new PresentersFactory(application, this);
        }

        #endregion

        #region Fields And Properties

        ConfigManager _ConfigManager;
        ISysLogManager _SystemLogger;
        ILogManager _Logger;
        PresentersFactory _WindowsFactory;
        SystemInformationModbusNetworkService _ModbusSystemInfoNetworkService;

        IApplicationController _Application;
        /// <summary>
        /// Версия ПО
        /// </summary>
        public Version SoftwareVersion
        {
            get { return _Application.Version; }
        }
        /// <summary>
        /// Версия аппаратуры
        /// </summary>
        public Version HardwareVersion
        {
            get { return new Version(1, 0, 0, 0); }
        }

        public ILogManager Logger
        {
            get { return _Logger; }
        }

        public ISysLogManager SystemLogger
        {
            get { return _SystemLogger; }
        }

        public IConfigManager ConfigManager
        {
            get { return _ConfigManager; }
        }
        
        public IPresentersFactory PresentersFactory
        {
            get { return _WindowsFactory; }
        }

        public ICanNetworkService CanNetworkService
        {
            get 
            { 
                foreach(ApplicationServiceBase service in _Application.AppServices)
                {
                    if (service.ServiceName == ServiceHelper.ServiceNames.NgkCanService)
                        return service as ICanNetworkService;
                }
                return null; 
            }
        }

        public ISystemInformationModbusNetworkService ModbusSystemInfoNetworkService
        {
            get 
            {
                foreach (ApplicationServiceBase service in _Application.AppServices)
                {
                    if (service.ServiceName == ServiceHelper.ServiceNames.SystemInformationModbusService)
                        return service as ISystemInformationModbusNetworkService;
                }
                return null;
            }
        }

        #endregion
    }
}
