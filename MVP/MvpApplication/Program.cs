using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using MvpApplication;
using Microsoft.VisualBasic.ApplicationServices;

namespace MvpApplication
{
    static class Program
    {
        public static WindowsFormsApplication ApplicationController;

        [STAThread]
        static void Main()
        {
            ApplicationController = 
                new WindowsFormsApplication(AuthenticationMode.Windows);
            ApplicationController.Startup += 
                new StartupEventHandler(EventHandler_Me_Startup);
            ApplicationController.Shutdown += 
                new ShutdownEventHandler(EventHandler_Me_Shutdown);
            ApplicationController.UnhandledException += 
                new Microsoft.VisualBasic.ApplicationServices
                .UnhandledExceptionEventHandler(
                EventHandler_Me_UnhandledException);
            ApplicationController.AppDomainUnhandledException += 
                new System.UnhandledExceptionEventHandler(
                ApplicationController_AppDomainUnhandledException);

            ApplicationController.ChangeCulture((new CultureInfo("ru-Ru")).Name);
            ApplicationController.SplashScreen = new SplashScreen();
            ApplicationController.MinimumSplashScreenDisplayTime = 3000;
            ApplicationController.StartScreen = new FormMain();
            ApplicationController.Run(Environment.GetCommandLineArgs());
        }


        private static void EventHandler_Me_Shutdown(object sender, EventArgs e)
        {
            MessageBox.Show("Event Shutdown");
        }

        private static void EventHandler_Me_UnhandledException(
            object sender, 
            Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            e.ExitApplication = true;            
        }

        static void ApplicationController_AppDomainUnhandledException(
            object sender, System.UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception)e.ExceptionObject).Message);
        }

        private static void EventHandler_Me_Startup(
            object sender, StartupEventArgs e)
        {
            WindowsFormsApplication application = 
                (WindowsFormsApplication)sender; 
            // Инициализируем приложение. Загружаем ресурсы
            SplashScreen splashScreen = (SplashScreen)application.SplashScreen;
            splashScreen.WriteLine("Загрузка конфигурации...");
            System.Threading.Thread.Sleep(2000);
            splashScreen.WriteLine("Применение конфигурации...");
            System.Threading.Thread.Sleep(2000);
            splashScreen.WriteLine("Загрузка БД...");
            System.Threading.Thread.Sleep(2000);
            splashScreen.WriteLine("Загрузка журнала событий...");
            System.Threading.Thread.Sleep(2000);
            splashScreen.WriteLine("Запуск системы мониторинга...");
            System.Threading.Thread.Sleep(2000);
        }
    }
}