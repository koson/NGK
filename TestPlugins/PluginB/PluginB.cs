using System;
using System.Collections.Generic;
using System.Text;
using PluginsInfrastructure;
using Mvp.Input;
using Mvp.WinApplication.ApplicationService;
using Mvp.WinApplication.Infrastructure;

namespace PluginB
{
    public class PluginB: Plugin
    {
        #region Constructors

        public PluginB() 
        {
            base.Name = "Module B";

            _ActionA = new Command(OnActionA);

            Menu root = new Menu("Меню модуля B", null);
            base._Menu.Add(root);

            root.SubMenuItems.Add(new Menu("Действие B", _ActionA)); 
        }

        #endregion

        #region Fields And Properties
        #endregion

        #region Commands

        private Command _ActionA;
        private void OnActionA()
        {
            MessageBoxService.ShowInformation("Это тестовое сообщение", "Тест");
        }

        #endregion
    }
}
