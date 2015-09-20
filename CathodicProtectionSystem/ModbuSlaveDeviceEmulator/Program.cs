using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ModbuSlaveDevicesNetwork
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //System.Diagnostics.Debugger.Launch();

            //AppDomain.CurrentDomain.UnhandledException 
            //    += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

//        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
//        {
//#if DEBUG 
//            if (!System.Diagnostics.Debugger.IsAttached)
//            {
//                System.Diagnostics.Debugger.Launch();
//                //System.Diagnostics.Debugger.Break();
//            }
//#endif
//        }
    }
}