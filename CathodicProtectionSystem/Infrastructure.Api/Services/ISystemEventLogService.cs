using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Dal.DbEntity;
using System.ComponentModel;
using System.Data;

namespace Infrastructure.Api.Services
{
    public interface ISystemEventLogService
    {
        #region Properties

        /// <summary>
        /// ������ ��������� ������� ������� �������� ������ ����������
        /// </summary>
        BindingList<ISystemEventMessage> SystemEvents { get; }
        /// <summary>
        /// ���������� ������� ������� ������� �� ����� �������� ��� ������
        /// ������ GetPage()
        /// </summary>
        byte PageSize { get; set; }

        #endregion

        #region Methods

        void AddEvent(SystemEventCodes eventCode, Category category, string message, DateTime created);
        /// <summary>
        /// ���������� ���������� ������� �������
        /// </summary>
        /// <returns></returns>
        int GetTotalPages();
        /// <summary>
        /// ���������� ������ ������� ��������� ��������
        /// </summary>
        /// <param name="pageNumber">�������� ���������� �� 0 � ��...</param>
        /// <returns></returns>
        IEnumerable<ISystemEventMessage> GetPage(int pageNumber);

        #endregion 
    }
}
