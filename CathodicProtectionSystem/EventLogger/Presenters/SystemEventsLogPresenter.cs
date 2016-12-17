using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using NGK.Plugins.Views;
using Infrastructure.Api.Managers;
using Infrastructure.Api.Models;
using System.ComponentModel;
using Infrastructure.Dal.DbEntity;
using Infrastructure.Dal.DbContext;
using Mvp.Input;
using Infrastructure.Api.Controls;
using System.Windows.Forms;

namespace NGK.Plugins.Presenters
{
    public class SystemEventsLogPresenter: PartialViewPresenter<SystemEventsLogView>
    {
        #region Constructors

        public SystemEventsLogPresenter(IManagers managers)
        {
            _Managers = managers;
            
            _ShowCurrentSessionLogCommand = new Command("События сеанса", OnShowCurrentSessionLog);
            _Commands.Add(_ShowCurrentSessionLogCommand);
            _ShowNextPageCommand = new Command("Вперёд", OnShowNextPage, CanShowNextPage);
            _Commands.Add(_ShowNextPageCommand);
            _ShowPreviousPageCommand = new Command("Назад", OnShowPreviousPage, CanShowPreviousPage);
            _Commands.Add(_ShowPreviousPageCommand);
            
            View.SystemEvents = _Managers.SystemEventLogService.SystemEvents;

            _ButtonShowCurrentSessionLog = new FunctionalButton(_ShowCurrentSessionLogCommand, Keys.F4);
            _ButtonShowCurrentSessionLog.Name = "_FunctionalButtonCurrentSessionLog";
            _ButtonShowCurrentSessionLog.Text = "Текущий сеанс";
            FunctionalButtons.Add(_ButtonShowCurrentSessionLog);
            _ButtonShowNextPage = new FunctionalButton(_ShowNextPageCommand, Keys.F5);
            _ButtonShowNextPage.Name = "_FunctionalButtonNextPage";
            _ButtonShowNextPage.Text = "Вперед";
            FunctionalButtons.Add(_ButtonShowNextPage);
            _ButtonShowPreviousPage = new FunctionalButton(_ShowPreviousPageCommand, Keys.F6);
            _ButtonShowPreviousPage.Name = "_FunctionalButtonShowPreviousPage";
            _ButtonShowPreviousPage.Text = "Назад";
            FunctionalButtons.Add(_ButtonShowPreviousPage);

            UpdateStatusCommands();
        }

        #endregion

        #region Fields And Properties

        private readonly IManagers _Managers;
        private int _CurrentPageNumber = 0;

        private int CurrentPageNumber
        {
            get { return _CurrentPageNumber; }
            set 
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(
                        "CurrentPageNumber", "Недопустимое значение");
                _CurrentPageNumber = value; 
            }
        }

        public override string Title
        {
            get { return @"Журнал событий системы"; }
        }

        private readonly FunctionalButton _ButtonShowCurrentSessionLog;
        private readonly FunctionalButton _ButtonShowNextPage;
        private readonly FunctionalButton _ButtonShowPreviousPage;

        #endregion

        #region Methods

        public override void Close()
        {
            View.Close();
            base.Close();
        }

        public override void Show()
        {
            _Managers.PartialViewService.Host.Show(this);
            base.Show();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Отобразить журнал событий текущего сеанса работы приложения
        /// </summary>
        private Command _ShowCurrentSessionLogCommand;
        private void OnShowCurrentSessionLog() 
        {
            View.SystemEvents = _Managers.SystemEventLogService.SystemEvents;
            UpdateStatusCommands();
        }
        /// <summary>
        /// Показать следующую страницу журнала событий
        /// </summary>
        private Command _ShowNextPageCommand;
        private void OnShowNextPage()
        {
            CurrentPageNumber++;

            IEnumerable<ISystemEventMessage> page = 
                _Managers.SystemEventLogService.GetPage(CurrentPageNumber);
            BindingList<ISystemEventMessage> list = new BindingList<ISystemEventMessage>();
            foreach (ISystemEventMessage msg in page)
            {
                list.Add(msg);
            }
            View.SystemEvents = list; 
            UpdateStatusCommands();
        }
        private bool CanShowNextPage()
        {
            return _Managers.SystemEventLogService.GetTotalPages() > CurrentPageNumber;
        }
        /// <summary>
        /// Показать предыдущую страницу журнала событий
        /// </summary>
        private Command _ShowPreviousPageCommand;
        private void OnShowPreviousPage()
        {
            CurrentPageNumber--;
            IEnumerable<ISystemEventMessage> page =
                _Managers.SystemEventLogService.GetPage(CurrentPageNumber);
            BindingList<ISystemEventMessage> list = new BindingList<ISystemEventMessage>();
            foreach (ISystemEventMessage msg in page)
            {
                list.Add(msg);
            }
            View.SystemEvents = list; 
            UpdateStatusCommands();
        }
        private bool CanShowPreviousPage()
        {
            int pages = _Managers.SystemEventLogService.GetTotalPages();
            return pages > 0 && CurrentPageNumber > 0;
        }

        #endregion
    }
}
