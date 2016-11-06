using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infrastructure.Dal.DbEntity;
using Infrastructure.Dal.DbContext.NgkDbDataSetTableAdapters;

namespace Infrastructure.Dal.DbContext
{
    public class DbContextSystemEventsLog : ISystemEventsRepository
    {
        #region Constructors

        public DbContextSystemEventsLog(NgkDbDataSet.SystemEnentsLogDataTable table)
        {
            _SystemEnentsLogDataTable = table;
            _SystemEventsLogCashTableAdapter = new SystemEnentsLogTableAdapter();
        }

        #endregion

        #region Fields And Properties

        private readonly NgkDbDataSet.SystemEnentsLogDataTable _SystemEnentsLogDataTable;
        private SystemEnentsLogTableAdapter _SystemEventsLogCashTableAdapter;

        public DataTable SystemEventsCashTable
        {
            get { return _SystemEnentsLogDataTable; }
        }

        #endregion

        #region Methods

        public void AddEvent(ISystemEventMessage eventMessage)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void UdateCash()
        {
            _SystemEventsLogCashTableAdapter.Fill(_SystemEnentsLogDataTable);
        }

        #endregion
    }
}
