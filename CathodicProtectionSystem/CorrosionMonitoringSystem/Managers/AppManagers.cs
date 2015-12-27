using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Managers.AppConfigManager;
using NGK.CorrosionMonitoringSystem.Managers.LogManager;
using NGK.CorrosionMonitoringSystem.Managers.SysLogManager;

namespace NGK.CorrosionMonitoringSystem.Managers
{
    public class AppManagers: IManagers
    {
        #region Constructor

        public AppManagers()
        {
            _ConfigManager = new ConfigManager();
            _Logger = null; //TODO: �� �����������
            _SystemLogger = null; //TODO: �� �����������
        }

        #endregion

        #region Fields And Properties

        ConfigManager _ConfigManager;
        ISysLogManager _SystemLogger;
        ILogManager _Logger;

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

        #endregion
    }
}
