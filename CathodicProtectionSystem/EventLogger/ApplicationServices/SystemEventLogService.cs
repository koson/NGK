using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using Infrastructure.Api.Managers;
using Infrastructure.Api.Services;
using Infrastructure.Dal.DbEntity;
using Infrastructure.Dal.DbContext;
using System.ComponentModel;
using System.Data;

namespace NGK.Plugins.ApplicationServices
{
    public class SystemEventLogService : ApplicationServiceBase, ISystemEventLogService, IDisposable
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="networkManager"></param>
        /// <param name="pollingInterval">Интервал обновления CAN-устройства, мсек</param>
        public SystemEventLogService(IManagers managers):
            base("SystemEventLogService")
        {
            _Managers = managers;
            _Repository = DbContext.Create().SystemEventRepository;
        }

        #endregion

        #region Fields And Properties

        private readonly IManagers _Managers;
        private ISystemEventsRepository _Repository;

        public BindingList<ISystemEventMessage> SystemEvents
        {
            get { return _Repository.SystemEnentsLog; }
        }

        public byte PageSize
        {
            get { return _Repository.PageSize; }
            set { _Repository.PageSize = value; }
        }

        #endregion

        #region Methods

        public void AddEvent(ISystemEventMessage eventMessage)
        {
            if (_Repository != null)
                _Repository.AddEvent(eventMessage);
        }

        public void AddEvent(SystemEventCodes eventCode, Category category, 
            string message, DateTime created)
        {
            SystemEventMessage evt = new SystemEventMessage();
            evt.SystemEventCode = eventCode;
            evt.Category = category;
            evt.Message = message;
            evt.Created = created;
            evt.HasRead = false;
            AddEvent(evt);
        }

        public int GetTotalPages()
        {
            return _Repository.GetTotalPages();
        }

        public IEnumerable<ISystemEventMessage> GetPage(int pageNumber)
        {
            return _Repository.GetPage(pageNumber);
        }

        public override void Dispose()
        {
            _Repository.Dispose();
            _Repository = null;
            base.Dispose();
        }

        #endregion
    }
}
