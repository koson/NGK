using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using TestPlugins.Services;
using TestPlugins.Presenters;
using TestPlugins.Views;
using Mvp.WinApplication;
using Mvp.Presenter;
using Mvp.Plugin;
using VisualBasic = Microsoft.VisualBasic.ApplicationServices;
using PluginsInfrastructure;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using Mvp.WinApplication.ApplicationService;

namespace TestPlugins
{
    static class Program
    {
        //public static WindowsFormsApplication Application;
        public static PluginsService<Plugin> AppPluginsService;


        [STAThread]
        static void Main(string[] args)
        {
            //Application = new WindowsFormsApplication(PresentersFactory.CreateForm<MainFormPresenter>(), true);
            WindowsFormsApplication.Initialize(PresentersFactory.CreateForm<MainFormPresenter>(), true);
            WindowsFormsApplication.Application.Startup += new VisualBasic::StartupEventHandler(EventHandler_Application_Startup);
            WindowsFormsApplication.Application.Shutdown += new VisualBasic::ShutdownEventHandler(EventHandler_Application_Shutdown);
            WindowsFormsApplication.Application.UnhandledException += new VisualBasic::UnhandledExceptionEventHandler(Application_UnhandledException);
            WindowsFormsApplication.Application.ChangeCulture("ru-Ru");
            WindowsFormsApplication.Application.MinimumSplashScreenDisplayTime = 1000;
            WindowsFormsApplication.Application.SplashScreen = (Form)PresentersFactory.CreateForm<SplashScreenPresenter>().View;
            WindowsFormsApplication.Application.Run(args);
        }

        static void Application_UnhandledException(object sender, VisualBasic::UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("It's handled for program");
            return;
        }

        static void EventHandler_Application_Shutdown(object sender, EventArgs e)
        {
            return;
        }

        static void EventHandler_Application_Startup(object sender, VisualBasic::StartupEventArgs e)
        {
            SplashScreenView splash = (SplashScreenView)WindowsFormsApplication.Application.SplashScreen;

            splash.Output("Загрузка плагинов...");

            // Инициализирует сервис плагинов приложения. Ищем и загружаем плагины
            PluginsService<Plugin> pluginsService = 
                new PluginsService<Plugin>(Directory.GetCurrentDirectory() + @"\Plugins\", true);
            WindowsFormsApplication.Application.RegisterApplicationService(pluginsService);
            pluginsService.Initialize(null);
            pluginsService.Start();
            pluginsService.LoadPlugins();

            AppPluginsService = pluginsService; 

            foreach(Plugin plugin in pluginsService.Plugins)
            {
                splash.Output(String.Format("Плагин {0} загружен", plugin.Name));
                NavigationService.Menu.AddRange(plugin.Menu);
                Thread.Sleep(1000);
            }
        }
    }
}