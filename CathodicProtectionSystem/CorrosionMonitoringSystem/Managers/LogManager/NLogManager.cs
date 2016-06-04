using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace NGK.CorrosionMonitoringSystem.Managers.LogManager
{
    public class NLogManager : ILogManager
    {
        #region Constructors
        
        public NLogManager(string name)
        {
            _Logger = NLog.LogManager.GetLogger(name);
        }

        #endregion

        #region Fields And Properties

        private static Logger _Logger;
        
        #endregion

        #region Methods

        public void Info(string message)
        {
            _Logger.Info(message);
        }

        public void FatalException(string message, Exception exception)
        {
            _Logger.FatalException(message, exception);
        }

        public void Error(string message)
        {
            _Logger.Error(message);
        }

        #endregion
    }
}
