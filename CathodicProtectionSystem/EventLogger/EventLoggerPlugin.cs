using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication.Infrastructure;
using Mvp.Input;
using NGK.Plugins.Presenters;
using Infrastructure.Api.Managers;
using NGK.Plugins.ApplicationServices;

namespace NGK.Plugins
{
    public class EventLoggerPlugin: Plugin
    {
        #region Constructors

        public EventLoggerPlugin() 
        {
            Name = @"Журнал событий";

            _ShowSystemEventsLogCommand = 
                new Command(OnShowSystemEventsLog, CanShowSystemEventsLog);
            _ShowSystemEventsLogArchiveCommand = 
                new Command(OnShowSystemEventsLogArchive, CanShowSystemEventsLogArchiveCommand);

            NavigationMenu = new NavigationMenuItem(Name, null);
            NavigationMenu.SubMenuItems.Add(
                new NavigationMenuItem("Журнал событий: Текущий сеанс", _ShowSystemEventsLogCommand));
            NavigationMenu.SubMenuItems.Add(
                new NavigationMenuItem("Журнал событий: Архив", _ShowSystemEventsLogArchiveCommand));
            NavigationMenu.SubMenuItems.Add(
                new NavigationMenuItem("Регистратор параметров", null));
        }

        #endregion

        #region Fields And Properties
        
        private SystemEventLogService _SystemEventLogService;

        #endregion

        #region Methods

        public override void Initialize(IManagers managers, object state)
        {
            base.Initialize(managers, state);
            
            // Создаём сервисы приложения и регистрируем их
            try
            {
                _SystemEventLogService = new SystemEventLogService(Managers);
                _SystemEventLogService.Initialize(null);
                _SystemEventLogService.PageSize = 20;
                base.ApplicationServices.Add(_SystemEventLogService);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    String.Format("Ошибка при инициализации плагина {0}", Name), ex);
            }
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

        private Command _ShowSystemEventsLogArchiveCommand;
        private void OnShowSystemEventsLogArchive()
        {
            SystemEventsLogArchivePresenter presenter = new SystemEventsLogArchivePresenter(Managers);
            presenter.Show();
        }
        private bool CanShowSystemEventsLogArchiveCommand()
        {
            return true;
        }

        #endregion
    }
}
