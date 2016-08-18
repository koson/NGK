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

namespace TestPlugins
{
    static class Program
    {
        public static WindowsFormsApplication Application;

        [STAThread]
        static void Main(string[] args)
        {
            Application = new WindowsFormsApplication(PresentersFactory.CreateForm<MainFormPresenter>(), true);
            Application.Startup += new VisualBasic::StartupEventHandler(EventHandler_Application_Startup);
            Application.Shutdown += new VisualBasic::ShutdownEventHandler(EventHandler_Application_Shutdown);
            Application.UnhandledException += new VisualBasic::UnhandledExceptionEventHandler(Application_UnhandledException);
            Application.ChangeCulture("ru-Ru");
            Application.MinimumSplashScreenDisplayTime = 5000;
            Application.SplashScreen = (Form)PresentersFactory.CreateForm<SplashScreenPresenter>().View;
            Application.Run(args);
        }

        static void Application_UnhandledException(object sender, VisualBasic::UnhandledExceptionEventArgs e)
        {
            return;
        }

        static void EventHandler_Application_Shutdown(object sender, EventArgs e)
        {
            return;
        }

        static void EventHandler_Application_Startup(object sender, VisualBasic::StartupEventArgs e)
        {
            SplashScreenView splash = (SplashScreenView)Application.SplashScreen;

            splash.Output("Загрузка плагинов...");

            //string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = Directory.GetCurrentDirectory() + @"\Plugins\";
            Debug.WriteLine(path);

            PluginsService<Plugin> pluginsService = new PluginsService<Plugin>(path, true);

            Application.RegisterApplicationService(pluginsService);
            pluginsService.Initialize(null);
            pluginsService.Start();
            pluginsService.LoadPlugins();
            ReadOnlyCollection<Plugin> plugins = pluginsService.Plugins;



            for (int i = 0; i < 10; i++)
            {
                splash.Output(i.ToString());
                Thread.Sleep(1000);
            }
        }
    }
}