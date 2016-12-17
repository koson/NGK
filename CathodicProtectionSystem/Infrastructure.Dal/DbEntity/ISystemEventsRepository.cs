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
        /// Список системных событий журнала текущего сеанса приложения
        /// </summary>
        BindingList<ISystemEventMessage> SystemEnentsLog { get; }
        /// <summary>
        /// Количество записей журнала событий на одной странице при вызове
        /// метода GetPage()
        /// </summary>
        byte PageSize { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Записывает системное событие в журнал 
        /// </summary>
        /// <param name="eventMessage"></param>
        void AddEvent(ISystemEventMessage eventMessage);
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