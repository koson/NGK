using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.View;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public class SplashScreenPresenter: IPresenter
    {
        #region Constructors

        public SplashScreenPresenter(IApplicationController application,
            ISplashScreenView view, object model, IManagers managers)
        {
            _Application = application;
            _Managers = managers;
            _View = view;
            _View.ViewShown += 
                new EventHandler(EventHandler_View_ViewShown);
        }

        #endregion

        #region Fields And Properties
        
        BackgroundWorker _Worker;
        IApplicationController _Application;
        ISplashScreenView _View;
        IManagers _Managers;
        
        public IView View
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
            // ���������� �������� ����� ����������
            MainScreenView mainScreenView = new MainScreenView();

            // ����������� ����
            mainScreenView.ShowInTaskbar = _Managers.ConfigManager.ShowInTaskbar;
            mainScreenView.FormBorderStyle =
                _Managers.ConfigManager.FormBorderEnable ?
                FormBorderStyle.Sizable : FormBorderStyle.None;

            MainScreenPresenter mainScreenPresenter =
                new MainScreenPresenter(_Application, mainScreenView, null, _Managers);
            _Application.CurrentScreen = mainScreenPresenter;
        }

        void EventHandler_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            OnInitSystemRunning();
        }

        #endregion

        #region Methods

        /// <summary>
        /// ������� ��������� ������ � ������������� 
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
        /// ������� ��������� ��� ������� ���������
        /// ������ ������������� ������� � ������� ������.
        /// � ����������� ����� ���������� ��� �������������
        /// �������
        /// </summary>
        public event EventHandler SystemInitializationRunning;

        #endregion
    }
}
