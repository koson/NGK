using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Infrastructure.Dal.DbEntity
{
    public interface ISystemEventsRepository
    {
        #region Properties

        DataTable SystemEventsCashTable { get; }

        #endregion

        #region Methods

        void AddEvent(ISystemEventMessage eventMessage);
        void UdateCash();

        #endregion
    }
}