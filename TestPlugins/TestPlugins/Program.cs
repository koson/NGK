using System;
using System.Collections.Generic;
//using System.Windows.Forms;
using Mvp.WinApplication;
using TestPlugins.Services;
using TestPlugins.Presenters;
using System.Threading;

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
                new EventHandler<ApplicationStartingEventArgs>(EventHandler_Application_ApplicationStarting);
            Application.ApplicationClosing += 
                new EventHandler(EventHandler_Application_ApplicationClosing);
            Application.Run(PresentersFactory.Create<MainFormPresenter>(),
                PresentersFactory.Create<SplashScreenPresenter>());
        }

        static void EventHandler_Application_ApplicationStarting(object sender, ApplicationStartingEventArgs e)
        {
            SplashScreenPresenter splashScreen = (SplashScreenPresenter)e.SplashScreen;

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    splashScreen.WriteLine(i.ToString());
                }
                catch (Exception ex)
                { 
                }
                Thread.Sleep(2000); 
            }

            return;
        }

        static void EventHandler_Application_ApplicationClosing(object sender, EventArgs e)
        {
            return;
        }

        static void EventHandler_Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            return;
        }
    }
}