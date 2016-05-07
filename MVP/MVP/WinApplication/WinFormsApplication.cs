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

namespace Mvp.WinApplication
{
    public class WinFormsApplication : IApplicationController
    {
        #region Constructors
        
        public WinFormsApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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
        }
        
        #endregion

        #region Fields And Properties

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
            get { return Application.CurrentCulture; }
            set { Application.CurrentCulture = value; }
        }

        Version _Version;
        public Version Version { get { return _Version; } }

        #endregion

        #region EventHadler
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
            OnApplicationStarting();
            Application.Run(_AppContext); 
        }

        /// <summary>
        /// Завершает работу приложения
        /// </summary>
        public void Exit()
        {
            OnApplicationClosing();
            Application.Exit();
        }
        
        private void OnApplicationStarting()
        {
            if (ApplicationStarting != null)
            {
                ApplicationStarting(this, new EventArgs());
            }
        }

        private void OnApplicationClosing()
        {
            if (ApplicationClosing != null)
            {
                ApplicationClosing(this, new EventArgs());
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Событие происходит сразу после вызова метода Run,
        /// но до запуска приложения
        /// </summary>
        public event EventHandler ApplicationStarting;
        /// <summary>
        /// Событие происходит перед закрытием приложения
        /// </summary>
        public event EventHandler ApplicationClosing;

        #endregion

    }
}
