using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Mvp.Presenter;
using Mvp.View;

namespace Mvp.WinApplication
{
    public interface IApplicationController
    {
        /// <summary>
        /// ���������� ��� ������������� ������� �����
        /// </summary>
        IPresenter CurrentScreen { get; set; }
    }

    public class WinFormsApplication : IApplicationController
    {
        #region Constructors
        
        public WinFormsApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _AppContext = new ApplicationContext();

            AppDomain.CurrentDomain.UnhandledException += 
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        
        #endregion

        #region Fields And Properties

        private ApplicationContext _AppContext;
        private IPresenter _CurrentScreen;

        /// <summary>
        /// ���������� ��� ������������� ������� �����
        /// </summary>
        public IPresenter CurrentScreen 
        {
            get { return _CurrentScreen; }
            set 
            {
                IView lastForm = null;

                if (_CurrentScreen != value)
                {
                    _CurrentScreen = value;

                    if (_AppContext.MainForm != null)
                    {
                        lastForm = (IView)_AppContext.MainForm;
                    }

                    _AppContext.MainForm = (Form)_CurrentScreen.View;

                    if (lastForm != null)
                    {
                        lastForm.Close();
                    }
                    ((IView)_AppContext.MainForm).Show();
                }
            }
        }

        #endregion

        #region EventHadler

        private void CurrentDomain_UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            OnUnhandledExceptionRasing();
            Application.Exit();
        }

        #endregion

        #region Methods

        /// <summary>
        /// ��������� ���������� �� ���������
        /// </summary>
        public void Run() 
        {
            OnApplicationRunning();
            Application.Run(_AppContext); 
        }

        /// <summary>
        /// ����� ����������� ��� ������������� ��������������� 
        /// ���������� ����� ��������� ����������.
        /// </summary>
        protected virtual void OnUnhandledExceptionRasing() { }
        
        /// <summary>
        /// ���������� ������� ApplicationRunning 
        /// </summary>
        private void OnApplicationRunning()
        {
            if (ApplicationRunning != null)
            {
                ApplicationRunning(this, new EventArgs());
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// ������� ���������� ����� ����� ������ ������ Run,
        /// �� �� ������� ����������
        /// </summary>
        public event EventHandler ApplicationRunning;

        #endregion
    }
}
