using System;
using System.Collections.Generic;
//using System.Windows.Forms;
using Mvp.WinApplication;

namespace TestPlugins
{
    static class Program
    {
        public static WinFormsApplication Application;

        [STAThread]
        static void Main()
        {
            Application = WinFormsApplication.Application;
            Application.UnhandledException += 
                new UnhandledExceptionEventHandler(EventHandler_Application_UnhandledException);
            Application.CurrentCulture = new System.Globalization.CultureInfo("ru-Ru");
            Application.ApplicationStarting += 
                new EventHandler(EventHandler_Application_ApplicationStarting);
            Application.ApplicationClosing += 
                new EventHandler(EventHandler_Application_ApplicationClosing);
            Application.Run();
        }

        static void EventHandler_Application_ApplicationClosing(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        static void EventHandler_Application_ApplicationStarting(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        static void EventHandler_Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}