using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using System.Threading;

namespace TestApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            args = new string[] { "ARG1", "ARG2" };

            MyApplication application = new MyApplication(new FormMain(), true);
            application.Startup += new StartupEventHandler(EventHandler_Application_Startup);
            application.StartupNextInstance += new StartupNextInstanceEventHandler(EventHandler_Application_StartupNextInstance);
            application.MinimumSplashScreenDisplayTime = 5000;
            application.SplashScreen = new SplashScreenForm();
            application.Run(args);
        }

        static void EventHandler_Application_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            //string[] args = new string[e.CommandLine.Count];
            //e.CommandLine.CopyTo(args, 0);

            return;
        }

        static void EventHandler_Application_Startup(object sender, StartupEventArgs e)
        {
            WindowsFormsApplicationBase application = (WindowsFormsApplicationBase)sender;

            SplashScreenForm splash = (SplashScreenForm)application.SplashScreen;

            for (int i = 0; i < 10; i++)
            {
                splash.Output = i.ToString();
                Thread.Sleep(1000);
            }

        }
    }
}