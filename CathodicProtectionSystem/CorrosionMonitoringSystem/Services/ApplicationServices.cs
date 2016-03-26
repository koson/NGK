using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public class ApplicationServices: IApplicationServices
    {
        #region Constructors

        public ApplicationServices(IApplicationController application, 
            IManagers managers) 
        {
            _Application = application;
            _IWindowService = new WindowsService(application);
            _Managers = managers;
        }
        
        #endregion

        #region Fields And Properties

        IApplicationController _Application;
        IWindowsService _IWindowService;
        IManagers _Managers;

        public IWindowsService WindowsService
        {
            get { return _IWindowService; }
        }

        #endregion
    }
}
