using System;
using System.Collections.Generic;
using System.Text;
using PluginsInfrastructure;
using Mvp.WinApplication.Infrastructure;
using Mvp.Input;
using Mvp.WinApplication.ApplicationService;

namespace PluginA
{
    public class PluginA: Plugin
    {
        #region Constructors

        public PluginA() 
        {
            base.Name = "Module A";

            _ActionA = new Command(OnActionA);

            Menu root = new Menu("���� ������ �", null);
            base._Menu.Add(root);

            root.SubMenuItems.Add(new Menu("�������� �", _ActionA)); 
        }

        #endregion

        #region Fields And Properties
        #endregion

        #region Commands

        private Command _ActionA;
        private void OnActionA()
        {
            MessageBoxService.ShowInformation("��� �������� ���������", "����");
        }

        #endregion
    }
}
