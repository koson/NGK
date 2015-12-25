using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using MvpApplication.Core;
using Microsoft.VisualBasic.ApplicationServices;

namespace MvpApplication
{
    static class Program
    {
        public static WindowsFormsApplication ApplicationController;

        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            ApplicationController = 
                new WindowsFormsApplication(AuthenticationMode.Windows);
            ApplicationController.ChangeCulture((new CultureInfo("ru-Ru")).Name);
            ApplicationController.Startup += 
                new StartupEventHandler(EventHandler_Me_Startup);
            ApplicationController.Shutdown += 
                new ShutdownEventHandler(EventHandler_Me_Shutdown);
            ApplicationController.UnhandledException += 
                new Microsoft.VisualBasic.ApplicationServices
                .UnhandledExceptionEventHandler(EventHandler_Me_UnhandledException);
            //Application.Run(new Form1());
            ApplicationController.ApplicationContext.MainForm = new FormMain();
            ApplicationController.Run(Environment.GetCommandLineArgs());
        }

        private static void EventHandler_Me_Shutdown(object sender, EventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            MessageBox.Show("Event Shutdown");
        }

        private static void EventHandler_Me_UnhandledException(
            object sender, 
            Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            MessageBox.Show(e.Exception.Message);
            e.ExitApplication = true;
            
        }

        private static void EventHandler_Me_Startup(object sender, StartupEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }
    }
}