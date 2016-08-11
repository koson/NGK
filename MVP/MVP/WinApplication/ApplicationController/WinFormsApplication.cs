using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using Mvp.Presenter;
using Mvp.View;
using System.Threading;
using System.Reflection;
using Mvp.WinApplication.Collections.ObjectModel;
using Mvp.Plugin;
using System.IO;
using WinForms = System.Windows.Forms;

namespace Mvp.WinApplication
{
    public class WinFormsApplication : IApplicationController
    {
        #region Constructors
        
        private WinFormsApplication()
        {
            WinForms::Application.EnableVisualStyles();
            WinForms::Application.SetCompatibleTextRenderingDefault(false);
            WinForms::Application.ApplicationExit += 
                new EventHandler(EventHandler_Application_ApplicationExit);

            _AppContext = new ApplicationContext();
            // О SynchronizationContext смотри здесь оригинал и перевод
            // http://www.codeproject.com/Articles/31971/Understanding-SynchronizationContext-Part-I
            // https://habrahabr.ru/post/232169/
            // Здесь SynchronizationContext создаётся для приложений не имеющих UI 
            // (т.е консольных) 
            SynchronizationContext.SetSynchronizationContext(
                new SynchronizationContext());
            _SyncContext = SynchronizationContext.Current;
            
            _Version = Assembly.GetCallingAssembly().GetName().Version;
            _AppServices = new ApplicationServiceCollection();
        }
        
        #endregion

        #region Fields And Properties

        private static Object SyncRoot = new Object();
        private static volatile WinFormsApplication _Application;

        public static WinFormsApplication Application
        {
            get
            {
                if (_Application == null)
                {
                    lock (SyncRoot)
                    {
                        if (_Application == null)
                            _Application = new WinFormsApplication();
                    }
                }
                return _Application;
            }
        }

        private ApplicationContext _AppContext;
        private IPresenter _CurrentPresenter;
        private SynchronizationContext _SyncContext;

        public ApplicationContext AppContext 
        { 
            get 
            {
                return _AppContext;
            } 
        } 

        public SynchronizationContext SyncContext
        {
            get { return _SyncContext; }
        }

        /// <summary>
        /// Возвращает или устанавливаем текущий экран
        /// </summary>
        public IPresenter CurrentPresenter 
        {
            get { return _CurrentPresenter; }
        }

        public Form CurrentForm
        {
            get { return _AppContext.MainForm; }
        }

        public CultureInfo CurrentCulture 
        {
            get { return WinForms::Application.CurrentCulture; }
            set { WinForms::Application.CurrentCulture = value; }
        }

        Version _Version;
        public Version Version { get { return _Version; } }

        ApplicationServiceCollection _AppServices;

        public IApplicationService[] AppServices
        {
            get 
            {
                List<IApplicationService> list;
                lock (SyncRoot)
                {
                    list = new List<IApplicationService>(_AppServices.Count);
                    foreach (IApplicationService service in _AppServices)
                    {
                        list.Add(service);
                    }
                }
                return list.ToArray();
            }
        }

        #endregion

        #region EventHadler

        void EventHandler_Application_ApplicationExit(object sender, EventArgs e)
        {
            foreach (ApplicationServiceBase service in _AppServices)
            {
                service.Stop();
            }

            foreach (ApplicationServiceBase service in _AppServices)
            {
                service.Dispose();
            }

            OnApplicationClosing();
        }

        #endregion

        #region Methods

        public void ShowWindow(IPresenter presenter)
        {
            IView lastForm = null;
            
            if (_CurrentPresenter != presenter)
            {
                _CurrentPresenter = presenter;

                if (_AppContext.MainForm != null)
                {
                    lastForm = (IView)_AppContext.MainForm;
                }
                
                _AppContext.MainForm = (Form)_CurrentPresenter.View;
                
                if (lastForm != null)
                {
                    lastForm.Close();
                }
                ((IView)_AppContext.MainForm).Show();

                // После добавления формы или любого контрола в приложении
                // создаётся UI-поток и теперь привязываем контекс к UI-потоку. 
                _SyncContext = SynchronizationContext.Current;
            }
        }

        public DialogResult ShowDialog(IPresenter presenter)
        {
            Form form = (Form)presenter.View;
            return form.ShowDialog();
        }

        /// <summary>
        /// Запускает приложение на выполение
        /// </summary>
        public void Run() 
        {
            OnApplicationStarting(null);
            WinForms::Application.Run(_AppContext); 
        }

        public void Run(IPresenter mainFormPresenter)
        {
            OnApplicationStarting(null);
            ShowWindow(mainFormPresenter);
            WinForms::Application.Run(_AppContext);
        }

        public void Run(IPresenter mainFormPresenter, IPresenter splashScreenPresenter)
        {
            OnApplicationStarting(splashScreenPresenter);
            //ShowWindow(mainFormPresenter);
            mainFormPresenter.Show();
            WinForms::Application.Run(_AppContext);
        }

        /// <summary>
        /// Завершает работу приложения
        /// </summary>
        public void Exit()
        {
            OnApplicationClosing();
            Application.Exit();
        }
        
        private void OnApplicationStarting(IPresenter splashScreenPresenter)
        {
            if (splashScreenPresenter != null)
            {
                //ShowWindow(splashScreenPresenter);
                splashScreenPresenter.Show();
            }

            if (ApplicationStarting != null)
            {
                ApplicationStarting(this, new ApplicationStartingEventArgs(splashScreenPresenter));
            }

            string pluginsFolder = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "Plugins");

            PluginsService service = new PluginsService(pluginsFolder, true);
            service.Register(this);
            service.Initialize(null);
            service.Start();
        }

        private void OnApplicationClosing()
        {
            if (ApplicationClosing != null)
            {
                ApplicationClosing(this, new EventArgs());
            }
        }

        public void RegisterApplicationService(ApplicationServiceBase service)
        {
            service.Register(this);
            _AppServices.Add(service);
        }

        #endregion

        #region Events

        /// <summary>
        /// Событие происходит сразу после вызова метода Run,
        /// но до запуска приложения
        /// </summary>
        public event EventHandler<ApplicationStartingEventArgs> ApplicationStarting;
        /// <summary>
        /// Событие происходит перед закрытием приложения
        /// </summary>
        public event EventHandler ApplicationClosing;
        /// <summary>
        /// Событие возникает при возникновении необработанного исключения
        /// </summary>
        public event UnhandledExceptionEventHandler UnhandledException
        {
            add
            {
                lock (SyncRoot)
                {
                    AppDomain.CurrentDomain.UnhandledException += value;
                }
            }
            remove
            {
                lock (SyncRoot)
                {
                    AppDomain.CurrentDomain.UnhandledException -= value;
                }
            }
        }

        #endregion
    }
}
