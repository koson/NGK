using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using MvpApplication2.View;

namespace MvpApplication2.Presenter
{
    public class BootstrapperPresenter: IPresenter
    {
        #region Constructors

        public BootstrapperPresenter(IApplicationController application,
            IBootstrapperView view, object model)
        {
            _Application = application;
            _View = view;
            _View.ViewShown += 
                new EventHandler(EventHandler_View_ViewShown);
        }

        #endregion

        #region Fields And Properties
        
        BackgroundWorker _Worker;
        IApplicationController _Application;
        IBootstrapperView _View;    
        
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
            MainScreenPresenter mainScreenPresenter =
                new MainScreenPresenter(_Application, mainScreenView, null);
            _Application.ShowWindow(mainScreenPresenter);
        }

        void EventHandler_Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            OnInitSystemRunning();
            //StartSystemInitialization();
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

        /// <summary>
        /// ��������� �������� �������
        /// </summary>
        void StartSystemInitialization()
        {
            _View.WriteLine("�������� ������������...");
            System.Threading.Thread.Sleep(2000);
            _View.WriteLine("���������� ������������...");
            System.Threading.Thread.Sleep(2000);
            _View.WriteLine("�������� ��...");
            System.Threading.Thread.Sleep(2000);
            _View.WriteLine("�������� ������� �������...");
            System.Threading.Thread.Sleep(2000);
            _View.WriteLine("������ ������� �����������...");
            System.Threading.Thread.Sleep(2000);
        }

        void LoadingCompleted()
        {
            // ���������� �������� ����� ����������
            MainScreenView mainScreenView = new MainScreenView();
            MainScreenPresenter mainScreenPresenter =
                new MainScreenPresenter(_Application, mainScreenView, null);
            _Application.ShowWindow(mainScreenPresenter);
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
