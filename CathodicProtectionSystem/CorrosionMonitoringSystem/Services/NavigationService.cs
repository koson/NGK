using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Presenter;
using NGK.CorrosionMonitoringSystem.View;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public class NavigationService: INavigationService
    {
        #region Constructors

        public NavigationService(IApplicationController application,
            IApplicationServices services)
        {
            _Application = application;
            _Services = services;
        }

        #endregion

        #region Fields And Properties

        IApplicationController _Application;
        IApplicationServices _Services;
        
        #endregion

        #region INavigationService Members

        public void ShowNavigationMenu()
        {
            NavigationMenuForm view = new NavigationMenuForm();
            view.ShowInTaskbar = false;
            view.FormBorderStyle = FormBorderStyle.FixedDialog;
            view.StartPosition = FormStartPosition.CenterScreen;

            INavigationMenuPresenter presenter = 
                new NavigationMenuPresenter(_Application, view, null, null);
            // Устанавливаем окно (кнопку привязанную к нему заблокируем)
            presenter.SelectedWindow = _Application.CurrentWindow;
            
            DialogResult result = _Services.WindowsService.ShowDialog(presenter);

            if (result == DialogResult.OK)
            {
                _Application.ShowWindow(presenter.SelectedWindow);
            }
        }

        #endregion
    }
}
