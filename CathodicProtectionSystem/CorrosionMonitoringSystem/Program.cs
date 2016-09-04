using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using VisualBasic = Microsoft.VisualBasic.ApplicationServices;
using Mvp.WinApplication;
using Mvp.View;
using Mvp.Presenter;
using Mvp.Plugin;
using NGK.CorrosionMonitoringSystem.Presenters;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Devices;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using Modbus.OSIModel.ApplicationLayer.Slave;
using Modbus.OSIModel.ApplicationLayer;
using Common.Controlling;
using NGK.CorrosionMonitoringSystem.Services;
using NGK.Log;
using Infrastructure.API.Managers;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication.ApplicationService;

namespace NGK.CorrosionMonitoringSystem
{
    static class Program
    {
        public static ILogManager _Logger;
        public static IManagers Managers;
        public static PluginsService<Plugin> AppPluginsService;
        public static MainWindowPresenter MainWindowPresenter;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            MainWindowPresenter = new MainWindowPresenter();
            WindowsFormsApplication.Initialize(MainWindowPresenter, true);
            WindowsFormsApplication.Application.UnhandledException +=
                new VisualBasic::UnhandledExceptionEventHandler(EventHandler_Application_UnhandledException);
            WindowsFormsApplication.Application.ChangeCulture("ru-Ru");
            WindowsFormsApplication.Application.ChangeUICulture("ru-Ru");
            WindowsFormsApplication.Application.Startup +=
                new VisualBasic::StartupEventHandler(EventHandler_Application_Startup);
            WindowsFormsApplication.Application.Shutdown +=
                new VisualBasic::ShutdownEventHandler(EventHandler_Application_Shutdown);
#if(DEBUG)
            WindowsFormsApplication.Application.MinimumSplashScreenDisplayTime = 1000;
#endif
            WindowsFormsApplication.Application.SplashScreen = 
                (Form)new SplashScreenPresenter().View.Form;

            Managers = new AppManagers(WindowsFormsApplication.Application);
            _Logger = NLogManager.Instance;
            Managers.Logger = _Logger;

            WindowsFormsApplication.Application.Run(args);            
        }

        static void  EventHandler_Application_Startup(object sender, VisualBasic::StartupEventArgs e)
        {
            WindowsFormsApplication application = (WindowsFormsApplication)sender;
            SplashScreenForm splash = (SplashScreenForm)application.SplashScreen;


            Managers.Logger.Info("Запуск приложения");

            if (Managers.ConfigManager.CursorEnable)
            {
                Cursor.Show();
            }
            else
            {
                Cursor.Hide();
            }

            //Загружаем конфигурацию сети CAN НГК ЭХЗ
            splash.WriteLine("Загрузка конфигурации сети CAN НГК ЭХЗ...");
            //LoadCanNetworkConfig();

            //Загружаем конфигурацию для сети Modbus 
            //splash.WriteLine("Загрузка конфигурации сети Modbus...");
            //LoadModbusNetworkConfig();
            //System.Threading.Thread.Sleep(300);

            //TODO: сделать менеджер базы данных. Если база не найдена, предлагает создать её.
            splash.WriteLine("Загрузка БД...");
            System.Threading.Thread.Sleep(300);

            
            splash.WriteLine("Загрузка плагинов...");
            // Инициализирует сервис плагинов приложения. Ищем и загружаем плагины
            PluginsService<Plugin> pluginsService =
                new PluginsService<Plugin>(Directory.GetCurrentDirectory() + @"\Plugins\", true);
            WindowsFormsApplication.Application.RegisterApplicationService(pluginsService);
            pluginsService.Initialize(null);
            pluginsService.Start();
            pluginsService.LoadPlugins();

            AppPluginsService = pluginsService;
            
            foreach (Plugin plugin in pluginsService.Plugins)
            {
                splash.WriteLine(String.Format("Плагин {0} загружен", plugin.Name));
                NavigationService.Menu.Add(plugin.NavigationMenu);
                plugin.Initialize(MainWindowPresenter, Managers, null);

                Thread.Sleep(500);
            }

            splash.WriteLine("Запуск системы мониторинга...");
            //Managers.CanNetworkService.Start();
            //TODO: создать запись в журнал
            //splash.WriteLine("Запуск информационного Modbus-сервиса...");
            //Managers.ModbusSystemInfoNetworkService.Start();
            //TODO: создать запись в журнал

            // Запуск всех сервисов приложения
            foreach (IApplicationService service in WindowsFormsApplication.Application.AppServices)
            {
                if (service.Status != Status.Running)
                {
                    splash.WriteLine(String.Format("Запуск сервиса {0}...", service.ServiceName));
                    service.Start();
                }
            }

            _Logger.Info("Приложение запущено");
        }

        static void  EventHandler_Application_Shutdown(object sender, EventArgs e)
        {
            // Останавливаем все сервисы
            foreach (IApplicationService service in WindowsFormsApplication.Application.AppServices)
            {
                if (service.Status != Status.Stopped)
                {
                    service.Stop();
                }
            }

            _Logger.Info("Приложение остановлено");
        }

        //static void LoadCanNetworkConfig()
        //{
        //    try
        //    {
        //        NgkCanNetworksManager.Instance.LoadConfig(Application.StartupPath +
        //            @"\newtorkconfig.bin.nwc");

        //        //Создаём сетевой сервис и регистрируем его
        //        CanNetworkService canNetworkService = new CanNetworkService(
        //            ServiceHelper.ServiceNames.NgkCanService,
        //            NgkCanNetworksManager.Instance, 300, Managers);
        //        WindowsFormsApplication.Application.RegisterApplicationService(canNetworkService);
        //        canNetworkService.Initialize(null);
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Ошибка при конфигурировании системы. " +
        //            "Приложение будет закрыто",
        //            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        //_Application.Exit();
        //        throw;
        //    }
        //}

        //static void LoadModbusNetworkConfig()
        //{
        //    try
        //    {
        //        ComPortSlaveMode serialPort = new ComPortSlaveMode(
        //            Managers.ConfigManager.SerialPortName,
        //            Managers.ConfigManager.SerialPortBaudRate,
        //            Managers.ConfigManager.SerialPortParity,
        //            Managers.ConfigManager.SerialPortDataBits,
        //            Managers.ConfigManager.SerialPortStopBits);

        //        ModbusNetworkControllerSlave modbusNetwork = new ModbusNetworkControllerSlave(
        //            Managers.ConfigManager.ModbusSystemInfoNetworkName, serialPort);
        //        ModbusNetworksManager.Instance.Networks.Add(modbusNetwork);

        //        // Создаём сервис приложения
        //        SystemInformationModbusNetworkService modbusSystemInfoNetworkService = 
        //            new SystemInformationModbusNetworkService(ServiceHelper.ServiceNames.SystemInformationModbusService, 
        //            Managers, modbusNetwork, Managers.ConfigManager.ModbusAddress, 400);
        //        WindowsFormsApplication.Application.RegisterApplicationService(modbusSystemInfoNetworkService);
        //        modbusSystemInfoNetworkService.Initialize(null);
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Ошибка при конфигурировании системы. " +
        //            "Приложение будет закрыто",
        //            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Обработчик события возникновения необработанного исключеия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void  EventHandler_Application_UnhandledException(object sender, VisualBasic::UnhandledExceptionEventArgs e)
        {
            _Logger.FatalException("Фатальная ошибка, приложение будет остановлено", e.Exception);

#if(DEBUG)
            MessageBox.Show(String.Format("Фатальная ошибка, приложение будет остановлено. Описание: {0} Стек: {1}", 
                e.Exception.Message, e.Exception.StackTrace), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.ExitApplication = true;
#else
            e.ExitApplication = true;
#endif
        }
    }
}