using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using NGK.Log;
using Infrastructure.Api.Services;

namespace Infrastructure.Api.Managers
{
    public class AppManagers: IManagers
    {
        #region Constructor

        public AppManagers(IApplicationController application)
        {
            _Application = application;
            _ConfigManager = Infrastructure.Api.Managers.ConfigManager.Instance;
            _PartialVIewService = new PartialVIewService(application);
            //_SystemLogger = null; //TODO: Не реализовано
            //_WindowsFactory = new PresentersFactory(application, this);
        }

        #endregion

        #region Fields And Properties

        private ILogManager _Logger;
        private ConfigManager _ConfigManager;
        private PartialVIewService _PartialVIewService;
        private ICanNetworkService _CanNetworkService;
        private ISystemEventLogService _SystemEventLogService;
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

        public IPartialVIewService PartialViewService
        {
            get { return _PartialVIewService; }
        }

        public ISystemEventLogService SystemEventLogService
        {
            get 
            {
                if (_SystemEventLogService == null)
                {
                    foreach (ApplicationServiceBase service in _Application.AppServices)
                    {
                        if (service is ISystemEventLogService)
                        {
                            _SystemEventLogService = service as ISystemEventLogService;
                            break;
                        }
                    }
                }
                return _SystemEventLogService;
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
