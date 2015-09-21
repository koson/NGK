using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Permissions;
using AppLog.AppDump;

//==============================================================================================================
namespace NGK.MeasuringDeviceTech
{
    //==========================================================================================================
    public enum EventID
    { }
    //==========================================================================================================
    static class Program
    {
        //------------------------------------------------------------------------------------------------------
        private static TraceSource _Trace = new TraceSource(Application.ProductName);
        private static XmlWriterTraceListener _XmlLog;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main()
        {
            DumpMaker.PathToDirectory = Application.StartupPath;

            // Получаем аргументы коммандной строки
            String[] args = Environment.GetCommandLineArgs();
            
            //_Trace.Switch.Level = SourceLevels.Error;
            String pathToExe = Application.StartupPath + @"\AppXmlLog.xml";
            _XmlLog = new XmlWriterTraceListener(pathToExe, "XmlLog");
            _XmlLog.TraceOutputOptions |= (TraceOptions.Callstack | TraceOptions.DateTime |
                TraceOptions.LogicalOperationStack | TraceOptions.ProcessId | 
                TraceOptions.ThreadId | TraceOptions.Timestamp);
            _XmlLog.Filter = new EventTypeFilter(SourceLevels.Error);
            _Trace.Listeners.Add(_XmlLog);

            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            //AppDomain.CurrentDomain.UnhandledException += DumpMaker.CurrentDomain_UnhandledException;
            
            
            // Событие необработанного исключения в других потоках
            //Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);
            
            // Событие необработанного исключения на уровне приложения
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(args)); 
        }
        //------------------------------------------------------------------------------------------------------
        //private static void Application_ThreadException(
        //    object sender, 
        //    System.Threading.ThreadExceptionEventArgs e)
        //{
        //    try
        //    {
        //        MessageBox.Show(
        //            String.Format("Произошел сбой работы приложения. Ошибка: {0}. Стек: {1}", 
        //            e.Exception.Message, e.Exception.StackTrace),
        //            "Ошибка", MessageBoxButtons.OK);
        //        Application.Exit();
        //    }
        //    catch
        //    {
        //        try
        //        {
        //            MessageBox.Show("Fatal Windows Forms Error",
        //                "Fatal Windows Forms Error", 
        //                MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
        //        }
        //        finally
        //        {
        //            Application.Exit();
        //        }
        //    }
        //}
        //------------------------------------------------------------------------------------------------------
        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only 
        // log the event, and inform the user about it. 
        private static void CurrentDomain_UnhandledException(
            object sender, 
            UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                String dumpName;

                DumpMaker.CreateMiniDump(Application.StartupPath, DUMP_TYPE.MiniDumpNormal, out dumpName);

                string errorMsg = "An application error occurred. Please contact the adminstrator " +
                    "with the following information:\n\n" + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace +
                    "This application made a dump in directory: " + Application.StartupPath + @"\" + dumpName;

                //// Since we can't prevent the app from terminating, log this to the event log.
                if (!EventLog.SourceExists(Application.ProductName))
                {
                    EventLog.CreateEventSource(Application.ProductName, "Application");
                }

                //// Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog("Application", System.Environment.MachineName);
                myLog.Source = Application.ProductName;
                myLog.WriteEntry(errorMsg, EventLogEntryType.Error);

                // Записываем в журнал событий
                //_Trace.TraceEvent(TraceEventType.Error, 1, ex.StackTrace);
                //_Trace.Flush(); 

                //MessageBox.Show(errorMsg + String.Format("Error: {0}. StackTrace: {1}",
                //    ex.Message, ex.StackTrace),
                //    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                Application.Exit();
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error",
                        "Fatal Non-UI Error. Could not write the error to the event log. Reason: "
                        + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                } 
            }
        }
        //------------------------------------------------------------------------------------------------------
    }
    //==========================================================================================================
}
//==============================================================================================================
// End of file