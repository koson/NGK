using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace Infrastructure.Dal.DbEntity
{
    public interface ISystemEventsRepository: IDisposable
    {
        #region Properties

        BindingList<ISystemEventMessage> SystemEnentsLog { get; }

        #endregion

        #region Methods

        void AddEvent(ISystemEventMessage eventMessage);

        #endregion
    }
}