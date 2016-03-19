using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class SplashScreenPresenter: Presenter<ISplashScreenView>
    {
        #region Constructors

        public SplashScreenPresenter(IApplicationController application,
            ISplashScreenView view, object model, IManagers managers):
            base(view, application)
        {
            _Name = "SplashScreen";
            _Managers = managers;
            _View = view;
            _View.ViewShown += 
                new EventHandler(EventHandler_View_ViewShown);
        }

        #endregion

        #region Fields And Properties
        
        BackgroundWorker _Worker;
        IManagers _Managers;

        public ISplashScreenView ViewConcrete
        {
            get { return _View; }
        }

        #endregion

        #region EventHandler

        void EventHandler_View_ViewShown(object sender, EventArgs e)
        {
            _Worker = new BackgroundWorker();
            _Worker.WorkerReportsProgress = true;
            _Worker.WorkerSupportsCancellation = true;
            _Worker.DoWork += 
                new DoWorkEventHandler(EventHandler_Worker_DoWork);
            _Worker.RunWorkerCompleted += 
                new RunWorkerCompletedEventHandler(EventHandler_Worker_RunWorkerCompleted);
            _Worker.RunWorkerAsync();
        }

        void EventHandler_Worker_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            IPresenter presenter = 
                _Managers.PresentersFactory.Create(NavigationMenuItems.NoSelection);
            presenter.Show();
        }

        void EventHandler_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            OnInitSystemRunning();
        }

        #endregion

        #region Methods

        /// <summary>
        /// ¬ыводит текстовую строку в представлении 
        /// </summary>
        /// <param name="text"></param>
        public void WtriteText(string text)
        {
            _View.WriteLine(text);
        }

        void OnInitSystemRunning()
        {
            if (SystemInitializationRunning != null)
            {
                SystemInitializationRunning(this, new EventArgs());
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// —обытие возникает при запуске выполени€
        /// задачи инициализации системы в фоновом потоке.
        /// ¬ обработчике можно разместить код инициализации
        /// системы
        /// </summary>
        public event EventHandler SystemInitializationRunning;

        #endregion
    }
}
