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
using System.ComponentModel;
using Microsoft.VisualBasic.ApplicationServices;
using System.Collections.ObjectModel;

namespace Mvp.WinApplication
{
    [Serializable]
    public class WindowsFormsApplication : WindowsFormsApplicationBase, IApplicationController 
    {
        #region Constructors

        private WindowsFormsApplication(IWindowPresenter mainFormPresenter, bool isSingleInstance)
        {
            base.EnableVisualStyles = true;
            base.IsSingleInstance = isSingleInstance;
            base.ShutdownStyle = ShutdownMode.AfterMainFormCloses;

            _MainFormPresenter = mainFormPresenter;

            _Version = Assembly.GetCallingAssembly().GetName().Version;
            _AppServices = new ApplicationServiceCollection();
        }
        
        #endregion

        #region Fields And Properties

        private static volatile WindowsFormsApplication Instance;
        private static object SyncRoot = new object();
        private IWindowPresenter _MainFormPresenter;
        private Version _Version;
        private ApplicationServiceCollection _AppServices;

        public static WindowsFormsApplication Application
        {
            get 
            {
                if (Instance == null)
                    throw new InvalidOperationException(
                        "Невозможно получить конроллер приложения. Необходимо вызвать метод Initialize");
                return Instance;
            }
        }

        public IWindowPresenter MainFormPresenter { get { return _MainFormPresenter; } }
        /// <summary>
        /// Версия ПО
        /// </summary>
        public Version Version { get { return _Version; } }
        /// <summary>
        /// Сервисы приложения
        /// </summary>
        public ReadOnlyCollection<IApplicationService> AppServices
        {
            get 
            {
                lock (SyncRoot)
                {
                    List<IApplicationService> list = new List<IApplicationService>();
                    foreach (ApplicationServiceBase service in _AppServices)
                    {
                        list.Add(service);
                    }

                    return list.AsReadOnly();
                }
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
        }

        #endregion

        #region Methods
        /// <summary>
        /// Создаёт контроллер приложения. Метод должен вызываться 
        /// до запуска приложения и только один раз.
        /// </summary>
        /// <param name="mainFormPresenter">Контроллер главной формы приложения</param>
        /// <param name="isSingleInstance">Указывает создать единственный экземляр приложения в системе</param>
        /// <exception cref="InvalidOperationException">
        /// Возникает при более чем одном вызове инициализации контроллера приложения
        /// </exception>
        public static void Initialize(IWindowPresenter mainFormPresenter, bool isSingleInstance)
        {
            if (Instance == null)
            {
                lock (SyncRoot)
                {
                    if (Instance == null)
                        Instance = new WindowsFormsApplication(mainFormPresenter, isSingleInstance);
                }
            }
            else
                throw new InvalidOperationException("Попытка создать второй экземляр контроллера приложения");
        }

        public void ShowWindow(IWindowPresenter presenter)
        {
            presenter.View.Show();
        }

        //public void ShowWindow(IPresenter presenter)
        //{
        //    IView lastForm = null;
            
        //    if (_CurrentPresenter != presenter)
        //    {
        //        _CurrentPresenter = presenter;

        //        if (_AppContext.MainForm != null)
        //        {
        //            lastForm = (IView)_AppContext.MainForm;
        //        }
                
        //        _AppContext.MainForm = (Form)_CurrentPresenter.View;
                
        //        if (lastForm != null)
        //        {
        //            lastForm.Close();
        //        }
        //        ((IView)_AppContext.MainForm).Show();

        //        // После добавления формы или любого контрола в приложении
        //        // создаётся UI-поток и теперь привязываем контекс к UI-потоку. 
        //        _SyncContext = SynchronizationContext.Current;
        //    }
        //}

        //public DialogResult ShowDialog(IPresenter presenter)
        //{
        //    Form form = (Form)presenter.View;
        //    return form.ShowDialog();
        //}

        protected override void OnCreateMainForm()
        {
            base.OnCreateMainForm();
            base.MainForm = (Form)_MainFormPresenter.View.Form;
        }

        /// <summary>
        /// Завершает работу приложения
        /// </summary>
        public void Exit()
        {
            if (MainForm != null)
                base.MainForm.Close();
            else
                base.ApplicationContext.ExitThread();
        }
        
        public void RegisterApplicationService(ApplicationServiceBase service)
        {
            service.Register(this);
            _AppServices.Add(service);
        }

        #endregion

        #region Events
        #endregion

        #region IApplicationController Members

        public SynchronizationContext SyncContext
        {
            get 
            {
                if (SynchronizationContext.Current == null)
                    SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

                return SynchronizationContext.Current; 
            } 
        }

        public ApplicationContext AppContext
        {
            get { return base.ApplicationContext; }
        }

        public Form CurrentForm
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        IApplicationService[] IApplicationController.AppServices
        {
            get 
            {
                lock(SyncRoot)
                {
                    List<IApplicationService> list = new List<IApplicationService>();
                    foreach(ApplicationServiceBase service in _AppServices)
                        list.Add(service);
                    return list.ToArray();
                }
            }
        }

        #endregion
    }
}
