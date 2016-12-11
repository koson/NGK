using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infrastructure.Dal.DbEntity;
using Infrastructure.Dal.DbContext.NgkDbDataSetTableAdapters;
using System.ComponentModel;
using System.Data.SqlClient;

namespace Infrastructure.Dal.DbContext
{
    public class DbContextSystemEventsLog : ISystemEventsRepository
    {
        #region Constructors

        public DbContextSystemEventsLog()
        {
            _DbContext = new NgkDbDataSet();
            _SystemEnentsLog = new BindingList<ISystemEventMessage>();
            _DbContextAdapter = new SystemEventsLogTableAdapter();
            _DbContextQueriesAdapter = new QueriesTableAdapter();
        }

        #endregion

        #region Fields And Properties
        private NgkDbDataSet _DbContext;
        private SystemEventsLogTableAdapter _DbContextAdapter;
        private QueriesTableAdapter _DbContextQueriesAdapter;
        private readonly BindingList<ISystemEventMessage>  _SystemEnentsLog;

        public BindingList<ISystemEventMessage> SystemEnentsLog
        {
            get { return _SystemEnentsLog; }
        }
        #endregion

        #region Methods

        public void AddEvent(ISystemEventMessage eventMessage)
        {
            int? id = 0;
            _DbContextQueriesAdapter.AddSystemEvent((int?)eventMessage.SystemEventCode,
                eventMessage.Message, (DateTime?)eventMessage.Created, (byte?)eventMessage.Category,
                eventMessage.HasRead, ref id);
            if (id.HasValue && id.Value != 0)
                eventMessage.MessageId = id.Value;
 
            _SystemEnentsLog.Add(eventMessage);
        }
        
        public void Dispose()
        {
            _DbContext.Dispose();
            _DbContextAdapter.Dispose();
            _DbContextQueriesAdapter.Dispose();
            _DbContext = null;
            _DbContextAdapter = null;
            _DbContextQueriesAdapter = null;
        }

        public void ShowLog()
        {

        }

        public void NextPage()
        { 
        }

        #endregion
    }
}
