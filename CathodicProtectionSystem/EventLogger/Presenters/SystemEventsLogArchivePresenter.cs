using System;
using System.Collections.Generic;
using System.Text;
using NGK.Plugins.Views;
using Infrastructure.Api.Plugins;
using Infrastructure.Dal.DbEntity;
using System.ComponentModel;
using Infrastructure.Api.Managers;
using Infrastructure.Api.Controls;
using Mvp.Input;
using System.Windows.Forms;

namespace NGK.Plugins.Presenters
{
    public class SystemEventsLogArchivePresenter: PartialViewPresenter<SystemEventsLogArchiveView>
    {
        #region Constructors

        public SystemEventsLogArchivePresenter(IManagers managers)
        {
            _Managers = managers;

            _GoToBeginOfLogCommand = new Command("На начало", OnGoToBeginOfLog, CanOnGoToBeginOfLog);
            _Commands.Add(_GoToBeginOfLogCommand);
            _ShowNextPageCommand = new Command("Вперёд", OnShowNextPage, CanShowNextPage);
            _Commands.Add(_ShowNextPageCommand);
            _ShowPreviousPageCommand = new Command("Назад", OnShowPreviousPage, CanShowPreviousPage);
            _Commands.Add(_ShowPreviousPageCommand);

            _ButtonShowCurrentSessionLog = new FunctionalButton(_GoToBeginOfLogCommand, Keys.F4);
            _ButtonShowCurrentSessionLog.Name = "_FunctionalButtonGoToBeginOfLog";
            _ButtonShowCurrentSessionLog.Text = "На начало";
            FunctionalButtons.Add(_ButtonShowCurrentSessionLog);
            _ButtonShowNextPage = new FunctionalButton(_ShowNextPageCommand, Keys.F5);
            _ButtonShowNextPage.Name = "_FunctionalButtonNextPage";
            _ButtonShowNextPage.Text = "Вперед";
            FunctionalButtons.Add(_ButtonShowNextPage);
            _ButtonShowPreviousPage = new FunctionalButton(_ShowPreviousPageCommand, Keys.F6);
            _ButtonShowPreviousPage.Name = "_FunctionalButtonShowPreviousPage";
            _ButtonShowPreviousPage.Text = "Назад";
            FunctionalButtons.Add(_ButtonShowPreviousPage);

            if (_GoToBeginOfLogCommand.CanExecute())
                _GoToBeginOfLogCommand.Execute();
            else
                View.SystemEvents = new BindingList<ISystemEventMessage>();

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
                View.PageNumber = value;
            }
        }

        private int TotalPages 
        {
            get
            {
                int pages = _Managers.SystemEventLogService.GetTotalPages();
                View.TotalPages = pages;
                return pages;
            }
        }

        public override string Title
        {
            get { return @"Журнал событий системы: Архив"; }
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
        /// Перейти на начальнутю страницу
        /// </summary>
        private Command _GoToBeginOfLogCommand;
        private void OnGoToBeginOfLog() 
        {
            CurrentPageNumber = 0;           
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
        private bool CanOnGoToBeginOfLog()
        {
            return TotalPages > 0;
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
            return TotalPages > CurrentPageNumber + 1;
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
            return TotalPages > 0 && CurrentPageNumber > 0;
        }

        #endregion
    }
}
