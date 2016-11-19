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

namespace NGK.Plugins.Presenters
{
    public class SystemEventsLogPresenter: PartialViewPresenter<SystemEventsLogView>
    {
        #region Constructors

        public SystemEventsLogPresenter(IManagers managers)
        {
            _Managers = managers;

            //BindingList<SystemEvent> log = new BindingList<SystemEvent>();
            //log.Add(new SystemEvent(SystemEvent.SystemEventCodes.CommunicationError,
            //    SystemEvent.Category.Error, @"Ошибка 1", DateTime.Now));
            //log.Add(new SystemEvent(SystemEvent.SystemEventCodes.ConfigurationError,
            //    SystemEvent.Category.Warring, @"Ошибка 2", DateTime.Now));
            

            //View.SystemEvents = _Managers.SystemEventLogService
        }

        #endregion

        #region Fields And Properties

        private readonly IManagers _Managers;
        

        public override string Title
        {
            get { return @"Журнал событий системы"; }
        }

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
    }
}
