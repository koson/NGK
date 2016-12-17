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

        /// <summary>
        /// ������ ��������� ������� ������� �������� ������ ����������
        /// </summary>
        BindingList<ISystemEventMessage> SystemEnentsLog { get; }
        /// <summary>
        /// ���������� ������� ������� ������� �� ����� �������� ��� ������
        /// ������ GetPage()
        /// </summary>
        byte PageSize { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// ���������� ��������� ������� � ������ 
        /// </summary>
        /// <param name="eventMessage"></param>
        void AddEvent(ISystemEventMessage eventMessage);
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