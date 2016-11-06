using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infrastructure.Dal.DbEntity;
using Infrastructure.Dal.DbContext.NgkDbDataSetTableAdapters;

namespace Infrastructure.Dal.DbContext
{
    public class DbContext: IDataRepository
    {
        #region Constructors

        private DbContext()
        {
            _DbContext = new NgkDbDataSet();
            _SystemEventsRepository = 
                new DbContextSystemEventsLog(_DbContext.SystemEnentsLog);
        }

        #endregion

        #region Fields And Properties
        private static volatile DbContext _Instance;
        private static object SyncRoot = new object();

        private readonly NgkDbDataSet _DbContext;
        private readonly ISystemEventsRepository _SystemEventsRepository;

        public ISystemEventsRepository SystemEventRepository
        {
            get { return _SystemEventsRepository; }
        }

        #endregion

        #region Methods

        public static IDataRepository Create()
        {
            if (_Instance == null)
            {
                lock (SyncRoot)
                {
                    if (_Instance == null)
                    {
                        _Instance = new DbContext();
                    }
                }
            }

            return _Instance;
        }

        #endregion
    }
}
