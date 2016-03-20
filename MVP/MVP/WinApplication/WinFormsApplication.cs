using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using Mvp.Presenter;
using Mvp.View;

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
        }
        
        #endregion

        #region Fields And Properties

        private ApplicationContext _AppContext;
        private IPresenter _CurrentPresenter;

        public ApplicationContext AppContext 
        { 
            get 
            { 
                return _AppContext; 
            } 
        } 

        /// <summary>
        /// ���������� ��� ������������� ������� �����
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
            }
        }

        public DialogResult ShowDialog(IPresenter presenter)
        {
            Form form = (Form)presenter.View;
            return form.ShowDialog();
        }

        /// <summary>
        /// ��������� ���������� �� ���������
        /// </summary>
        public void Run() 
        {
            OnApplicationStarting();
            Application.Run(_AppContext); 
        }

        /// <summary>
        /// ��������� ������ ����������
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
        /// ������� ���������� ����� ����� ������ ������ Run,
        /// �� �� ������� ����������
        /// </summary>
        public event EventHandler ApplicationStarting;
        /// <summary>
        /// ������� ���������� ����� ��������� ����������
        /// </summary>
        public event EventHandler ApplicationClosing;

        #endregion

    }
}
