using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dal.DbEntity
{
    /// <summary>
    /// Глобальный репозиторий данных хранящийся в БД приложения
    /// </summary>
    public interface IDataRepository: IDisposable
    {
        #region Properties

        ISystemEventsRepository SystemEventRepository { get; }

        #endregion
    }
}
