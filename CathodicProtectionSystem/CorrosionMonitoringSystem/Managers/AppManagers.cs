using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Managers.AppConfigManager;
using NGK.CorrosionMonitoringSystem.Managers.LogManager;
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
            _Logger = null; //TODO: Не реализовано
            _SystemLogger = null; //TODO: Не реализовано
            _WindowsFactory = new PresentersFactory(application, this);
            _CanNetwrokService = new CanNetworkService(application, 
                NgkCanNetworksManager.Instance, 300);
            _ModbusSystemInfoNetworkService = 
                new SystemInformationModbusNetworkService(application, this, 
                _ConfigManager.ModbusAddress, 400);
        }

        #endregion

        #region Fields And Properties

        ConfigManager _ConfigManager;
        ISysLogManager _SystemLogger;
        ILogManager _Logger;
        PresentersFactory _WindowsFactory;
        CanNetworkService _CanNetwrokService;
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
            get { return _CanNetwrokService; }
        }

        public ISystemInformationModbusNetworkService ModbusSystemInfoNetworkService
        {
            get { return _ModbusSystemInfoNetworkService; }
        }

        #endregion
    }
}
