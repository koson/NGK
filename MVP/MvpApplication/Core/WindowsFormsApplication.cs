using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace MvpApplication.Core
{
    public class WindowsFormsApplication: WindowsFormsApplicationBase
    {
        #region Fields And Properties
        
        private AppDomain _AppDomain;
        
        #endregion

        #region Constructors

        public WindowsFormsApplication(AuthenticationMode authenticationMode) :
            base(authenticationMode)
        {
            _AppDomain = AppDomain.CurrentDomain;
            _AppDomain.UnhandledException += 
                new System.UnhandledExceptionEventHandler(
                EventHandler_AppDomain_UnhandledException);
        }
        
        #endregion

        #region Event Hadlers

        private void EventHandler_AppDomain_UnhandledException(
            object sender, System.UnhandledExceptionEventArgs e)
        {
            AppDomain domain = (AppDomain)sender;
            Exception exception = (Exception)e.ExceptionObject;
            bool x = e.IsTerminating;
            //Application.Exit();
            MessageBox.Show(String.Format("Domain: {0}", exception.Message));
            Process proc = Process.GetCurrentProcess();
            int code = proc.ExitCode;
            proc.Close();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Завершает работу приложения
        /// </summary>
        public void Exit()
        {
            Process.GetCurrentProcess().CloseMainWindow();
        }

        #endregion
    }
}
