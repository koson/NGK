using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using NLog;
using NGK.CorrosionMonitoringSystem.BL;
using NGK.CorrosionMonitoringSystem.Forms;
using NGK.CAN.ApplicationLayer.Network.Master;

namespace NGK.CorrosionMonitoringSystem
{
    static class Program
    {
        private static NetworksManager _NetworkManager;
        private static Logger _Logger;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.CurrentCulture = new System.Globalization.CultureInfo("ru-Ru");
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(EventHandler_CurrentDomain_UnhandledException);
            Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler(EventHandler_Application_ThreadException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Создаём объект для ведения логов приложения
            //_Logger = LogManager.GetCurrentClassLogger();
            _Logger = LogManager.GetLogger("CorrosionMonitoringSystemLogger");
            _Logger.Info("Приложение запущено");

            // Data base layer 
            _NetworkManager = NetworksManager.Instance;
            try
            {
                _NetworkManager.LoadConfig(Application.StartupPath + 
                    @"\newtorkconfig.bin.nwc");
            }
            catch
            {
                MessageBox.Show("Ошибка при конфигурировании системы. " + 
                    "Приложение будет закрыто", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            // Presentation layer
            CorrosionMonitoringSystemForm _PresentationForm = new CorrosionMonitoringSystemForm();

            // Business layer
            BLController controller = new BLController(_NetworkManager, _PresentationForm);

            Application.Run(_PresentationForm);
            
            _Logger.Info("Приложение остановлено");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void EventHandler_Application_ThreadException(object sender, 
            System.Threading.ThreadExceptionEventArgs e)
        {
            return;
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
    } //End of Class
}// End Of Namespace