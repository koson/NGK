using System;
using System.Collections.Generic;
using System.Text;
using PluginsInfrastructure;
using Mvp.WinApplication.Infrastructure;
using Mvp.Input;
using Mvp.WinApplication.ApplicationService;
using Mvp.WinApplication;
using PluginA.Presenters;
using Mvp.View;

namespace PluginA
{
    public class PluginA: Plugin
    {
        #region Constructors

        public PluginA() 
        {
            base.Name = "Module A";

            _ActionACommand = new Command(OnActionA);
            _ShowTestViewCommand = new Command(OnShowTestView);
            _ShowTestView2Command = new Command(OnShowTestView2);

            _TestPartialViewPresenter = new TestPartialViewPresenter();
            _TestPartialViewPresenter.View.Dock = System.Windows.Forms.DockStyle.Fill;
            base._PartialPresenters.Add(_TestPartialViewPresenter);

            _TestPartialView2Presenter = new TestPartialView2Presenter();
            _TestPartialView2Presenter.View.Dock = System.Windows.Forms.DockStyle.Fill;
            base._PartialPresenters.Add(_TestPartialView2Presenter);

            NavigationMenuItem root = new NavigationMenuItem("Меню модуля А", null);
            base._Menu.Add(root);
            root.SubMenuItems.Add(new NavigationMenuItem("Действие А", _ActionACommand));
            root.SubMenuItems.Add(new NavigationMenuItem("Частичное представление 1", _ShowTestViewCommand));
            root.SubMenuItems.Add(new NavigationMenuItem("Частичное представление 2", _ShowTestView2Command)); 
        }

        #endregion

        #region Fields And Properties

        private TestPartialViewPresenter _TestPartialViewPresenter;
        private TestPartialView2Presenter _TestPartialView2Presenter;
 
        #endregion

        #region Commands

        private Command _ActionACommand;
        private void OnActionA()
        {
            WindowsFormsApplication.Application.ShowWindow(new TestFormPresenter());
        }

        private Command _ShowTestViewCommand;
        private void OnShowTestView()
        {
            _TestPartialViewPresenter.Show();
        }

        private Command _ShowTestView2Command;
        private void OnShowTestView2()
        {
            _TestPartialView2Presenter.Show();
        }

        #endregion
    }
}
