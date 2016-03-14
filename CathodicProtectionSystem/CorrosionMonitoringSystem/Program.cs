using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using NLog;
using Mvp.WinApplication;
using Mvp.View;
using Mvp.Presenter;
using NGK.CorrosionMonitoringSystem.Presenters;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem
{
    static class Program
    {
        public static Logger _Logger;
        public static WinFormsApplication _Application;
        public static IManagers Managers;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <remarks>
        /// https://exceptionalcode.wordpress.com/2010/03/25/splash-screen-for-windows-forms-application/
        /// </remarks>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(EventHandler_CurrentDomain_UnhandledException);
            
            _Application = new WinFormsApplication();
            _Application.CurrentCulture = new CultureInfo("ru-Ru");
            _Application.ApplicationStarting += 
                new EventHandler(EventHandler_Application_ApplicationRunning);
            _Application.ApplicationClosing += 
                new EventHandler(EventHandler_Application_ApplicationClosing);
            _Application.Run();
        }

        /// <summary>
        /// Инициализация приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void EventHandler_Application_ApplicationRunning(object sender, EventArgs e)
        {
            WinFormsApplication app = (WinFormsApplication)sender;

            Managers = new AppManagers(app);

            if (Managers.ConfigManager.CursorEnable)
            {
                Cursor.Show();
            }
            else
            {
                Cursor.Hide();
            }

            //Создаём presenter splash screen 
            SplashScreenView splashscreenView = new SplashScreenView();
            SplashScreenPresenter splashscreenPresenter =
                new SplashScreenPresenter(app, splashscreenView, null, Managers);
            //Подключем метод для выполнения инициализации приложения
            splashscreenPresenter.SystemInitializationRunning += new EventHandler(
                EventHandler_SplashscreenPresenter_SystemInitializationRunning);
            app.ShowWindow(splashscreenPresenter);
        }

        static void EventHandler_Application_ApplicationClosing(
            object sender, EventArgs e)
        {
            Managers.CanNetworkService.Stop();
            _Logger.Info("Приложение остановлено");
        }

        static void EventHandler_SplashscreenPresenter_SystemInitializationRunning(
            object sender, EventArgs e)
        {
            SplashScreenPresenter presenter = (SplashScreenPresenter)sender;

            // Создаём объект для ведения логов приложения
            presenter.WtriteText("Инициализация системы логирования...");

            _Logger = LogManager.GetLogger("CorrosionMonitoringSystemLogger");

            //Загружаем конфигурацию сети
            presenter.WtriteText("Загрузка конфигурации сети...");
            LoadNetworkConfig();

            System.Threading.Thread.Sleep(300);
            presenter.WtriteText("Загрузка БД...");
            System.Threading.Thread.Sleep(300);
            presenter.WtriteText("Загрузка журнала событий...");
            System.Threading.Thread.Sleep(300);
            
            presenter.WtriteText("Запуск системы мониторинга...");
            Managers.CanNetworkService.Start();

            _Logger.Info("Приложение запущено");
        }

        static void LoadNetworkConfig()
        {
            try
            {
                NetworksManager.Instance.LoadConfig(Application.StartupPath +
                    @"\newtorkconfig.bin.nwc");
            }
            catch
            {
                MessageBox.Show("Ошибка при конфигурировании системы. " +
                    "Приложение будет закрыто",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _Application.Exit();
            }
        }

        /// <summary>
        /// Обработчик события возникновения необработанного исключеия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void EventHandler_CurrentDomain_UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                // CLR - останавливается
            }
            Exception exception = e.ExceptionObject as Exception;

            _Logger.FatalException("Фатальная ошибка, приложение будет остановлено", exception);

            #if(DEBUG)
            MessageBox.Show(String.Format("Фатальная ошибка, приложение будет остановлено. Описание: {0} Стек: {1}", 
                exception.Message, exception.StackTrace), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
            #else
            Application.Exit();
            #endif
            return;
        }
    }
}