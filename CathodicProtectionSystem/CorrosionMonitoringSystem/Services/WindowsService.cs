using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.WinApplication;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public class WindowsService: IWindowsService
    {
        #region Constructors

        public WindowsService(IApplicationController application) 
        {
            _Application = application;
        }

        #endregion

        #region Fields And Properties

        IApplicationController _Application;

        #endregion

        #region IWindowsService Members

        public DialogResult ShowDialog(IPresenter presenter)
        {
            return ((Form)presenter.View).ShowDialog();
        }

        public void Show(IPresenter presenter)
        {
            _Application.ShowWindow(presenter);
        }

        public Form CurrentWindow
        {
            get { return _Application.CurrentForm; }
        }

        #endregion
    }
}
