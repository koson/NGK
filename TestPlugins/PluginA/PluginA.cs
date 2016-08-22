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

            _TestPartialViewPresenter = new TestPartialVIewPresenter();
            _TestPartialViewPresenter.View.Dock = System.Windows.Forms.DockStyle.Fill;
            base._PartialPresenters.Add(_TestPartialViewPresenter);

            NavigationMenuItem root = new NavigationMenuItem("Меню модуля А", null);
            base._Menu.Add(root);
            root.SubMenuItems.Add(new NavigationMenuItem("Действие А", _ActionACommand));
            root.SubMenuItems.Add(new NavigationMenuItem("Частичное предстваление 1", _ShowTestViewCommand));
        }

        #endregion

        #region Fields And Properties

        TestPartialVIewPresenter _TestPartialViewPresenter;

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
            //IRegionContainer regionContainer = 
            //    WindowsFormsApplication.Application.MainFormPresenter.View.Regions["WorkingRegion"];
            //regionContainer.Add(_TestPartialViewPresenter);
            _TestPartialViewPresenter.Show();
        }

        #endregion
    }
}
