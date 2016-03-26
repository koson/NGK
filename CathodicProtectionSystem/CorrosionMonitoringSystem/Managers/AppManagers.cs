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

namespace NGK.CorrosionMonitoringSystem.Managers
{
    public class AppManagers: IManagers
    {
        #region Constructor

        public AppManagers(IApplicationController application)
        {
            _ConfigManager = new ConfigManager();
            _Logger = null; //TODO: Не реализовано
            _SystemLogger = null; //TODO: Не реализовано
            _NavigationService = NGK.CorrosionMonitoringSystem.Services.NavigationService.Instance;
            _NavigationService.Managers = this;
            _WindowsFactory = new PresentersFactory(application, this);
            _CanNetwrokService = new CanNetworkService(application, 
                NetworksManager.Instance, 300);
        }

        #endregion

        #region Fields And Properties

        ConfigManager _ConfigManager;
        ISysLogManager _SystemLogger;
        ILogManager _Logger;
        NavigationService _NavigationService;
        PresentersFactory _WindowsFactory;
        CanNetworkService _CanNetwrokService;

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
        
        public INavigationService NavigationService 
        {
            get { return _NavigationService; } 
        }

        public IPresentersFactory PresentersFactory
        {
            get { return _WindowsFactory; }
        }

        public ICanNetworkService CanNetworkService
        {
            get { return _CanNetwrokService; }
        }

        #endregion
    }
}
