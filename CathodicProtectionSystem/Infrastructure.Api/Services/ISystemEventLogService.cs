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
        /// Список системных событий журнала текущего сеанса приложения
        /// </summary>
        BindingList<ISystemEventMessage> SystemEvents { get; }
        /// <summary>
        /// Количество записей журнала событий на одной странице при вызове
        /// метода GetPage()
        /// </summary>
        byte PageSize { get; set; }

        #endregion

        #region Methods

        void AddEvent(SystemEventCodes eventCode, Category category, string message, DateTime created);
        /// <summary>
        /// Возвращает количество страниц журнала
        /// </summary>
        /// <returns></returns>
        int GetTotalPages();
        /// <summary>
        /// Возвращает записи журнала указанной страницы
        /// </summary>
        /// <param name="pageNumber">Страницы нумеруются от 0 и тд...</param>
        /// <returns></returns>
        IEnumerable<ISystemEventMessage> GetPage(int pageNumber);

        #endregion 
    }
}
