using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace NGK.Log
{
    public class NLogManager : ILogManager
    {
        #region Constructors
        
        static NLogManager()
        {
            if (NLog.LogManager.Configuration != null && NLog.LogManager.Configuration.AllTargets.Count > 0)
                _Logger = NLog.LogManager.GetLogger(NLog.LogManager.Configuration.AllTargets[0].Name);
            else
                _Logger = NLog.LogManager.GetCurrentClassLogger();
        }

        private NLogManager() {}

        #endregion

        #region Fields And Properties

        private static Logger _Logger;
        private static object _SyncRoot = new object();
        private static volatile NLogManager _Instance;
        
        public static ILogManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new NLogManager();
                    }
                }
                return _Instance;
            }
        }
        
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
