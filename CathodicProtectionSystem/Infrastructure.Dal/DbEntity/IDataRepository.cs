using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dal.DbEntity
{
    /// <summary>
    /// ���������� ����������� ������ ���������� � �� ����������
    /// </summary>
    public interface IDataRepository
    {
        #region Properties

        ISystemEventsRepository SystemEventRepository { get; }

        #endregion
    }
}
