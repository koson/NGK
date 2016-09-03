using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using NGK.Log;
using Infrastructure.API.Services;

namespace Infrastructure.API.Managers
{
    public class AppManagers: IManagers
    {
        #region Constructor

        public AppManagers(IApplicationController application)
        {
            _Application = application;
            _ConfigManager = Infrastructure.API.Managers.ConfigManager.Instance;
            //_SystemLogger = null; //TODO: Не реализовано
            //_WindowsFactory = new PresentersFactory(application, this);
        }

        #endregion

        #region Fields And Properties

        ILogManager _Logger;
        ConfigManager _ConfigManager;
        //ISysLogManager _SystemLogger;
        ////PresentersFactory _WindowsFactory;
        //SystemInformationModbusNetworkService _ModbusSystemInfoNetworkService;

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
            set { _Logger = value; }
        }

        //public ISysLogManager SystemLogger
        //{
        //    get { return _SystemLogger; }
        //}

        public IConfigManager ConfigManager
        {
            get { return _ConfigManager; }
        }

        private ICanNetworkService _CanNetworkService;

        public ICanNetworkService CanNetworkService
        {
            get 
            {
                if (_CanNetworkService == null)
                {
                    foreach (ApplicationServiceBase service in _Application.AppServices)
                    {
                        if (service is ICanNetworkService)
                            _CanNetworkService = service as ICanNetworkService;
                    }
                }
                return _CanNetworkService; 
            }
        }

        //public ISystemInformationModbusNetworkService ModbusSystemInfoNetworkService
        //{
        //    get 
        //    {
        //        foreach (ApplicationServiceBase service in _Application.AppServices)
        //        {
        //            if (service.ServiceName == ServiceHelper.ServiceNames.SystemInformationModbusService)
        //                return service as ISystemInformationModbusNetworkService;
        //        }
        //        return null;
        //    }
        //}

        #endregion
    }
}
