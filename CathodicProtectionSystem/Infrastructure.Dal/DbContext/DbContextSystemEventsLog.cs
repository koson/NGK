using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infrastructure.Dal.DbEntity;
using Infrastructure.Dal.DbContext.NgkDbDataSetTableAdapters;
using System.ComponentModel;

namespace Infrastructure.Dal.DbContext
{
    public class DbContextSystemEventsLog : ISystemEventsRepository
    {
        #region Constructors

        public DbContextSystemEventsLog()
        {
            _DbContext = new NgkDbDataSet();
            _SystemEnentsLog = new BindingList<ISystemEventMessage>();
            _SystemEventsLogCashTableAdapter = new SystemEventsLogTableAdapter();
        }

        #endregion

        #region Fields And Properties
        private NgkDbDataSet _DbContext;
        private SystemEventsLogTableAdapter _SystemEventsLogCashTableAdapter;
        private readonly BindingList<ISystemEventMessage>  _SystemEnentsLog;

        public BindingList<ISystemEventMessage> SystemEnentsLog
        {
            get { return _SystemEnentsLog; }
        }
        #endregion

        #region Methods

        public void AddEvent(ISystemEventMessage eventMessage)
        {
            _SystemEnentsLog.Add(eventMessage);
            _SystemEventsLogCashTableAdapter.Insert((int)eventMessage.SystemEventCode,
                eventMessage.Message, eventMessage.Created, (byte)eventMessage.Category, false);
            //throw new Exception("The method or operation is not implemented.");
        }

        //public void UdateCash()
        //{
        //    _SystemEventsLogCashTableAdapter.Fill(_SystemEnentsLogDataTable);
        //}
        
        public void Dispose()
        {
            _DbContext.Dispose();
            _SystemEventsLogCashTableAdapter.Dispose();
            _DbContext = null;
            _SystemEventsLogCashTableAdapter = null;
        }

        #endregion
    }
}
