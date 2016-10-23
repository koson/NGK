using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication.Infrastructure;
using Mvp.Input;
using NGK.Plugins.Presenters;

namespace NGK.Plugins
{
    public class EventLoggerPlugin: Plugin
    {
        #region Constructors

        public EventLoggerPlugin() 
        {
            Name = @"��������� �����������";

            _ShowSystemEventsLogCommand = new Command(OnShowSystemEventsLog, CanShowSystemEventsLog);


            NavigationMenu = new NavigationMenuItem(Name, null);
            NavigationMenu.SubMenuItems.Add(
                new NavigationMenuItem("������ �������", _ShowSystemEventsLogCommand));
            NavigationMenu.SubMenuItems.Add(
                new NavigationMenuItem("����������� ����������", null));
        }

        #endregion

        #region Commands

        private Command _ShowSystemEventsLogCommand;
        private void OnShowSystemEventsLog()
        {
            SystemEventsLogPresenter presenter = new SystemEventsLogPresenter(Managers);
            presenter.Show();
        }
        private bool CanShowSystemEventsLog()
        {
            return true;
        }

        #endregion
    }
}
