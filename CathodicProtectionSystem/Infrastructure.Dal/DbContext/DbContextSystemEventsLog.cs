using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Infrastructure.Dal.DbEntity;
using Infrastructure.Dal.DbContext.NgkDbDataSetTableAdapters;
using System.ComponentModel;
using System.Data.SqlClient;

namespace Infrastructure.Dal.DbContext
{
    public class DbContextSystemEventsLog : ISystemEventsRepository
    {
        #region Constructors

        public DbContextSystemEventsLog()
        {
            _DbContext = new NgkDbDataSet();
            _SystemEnentsLog = new BindingList<ISystemEventMessage>();
            _SystemEnentsLog.AddingNew +=
                new AddingNewEventHandler(EventHandler_SystemEnentsLog_AddingNew);
        }

        #endregion

        #region Fields And Properties

        private NgkDbDataSet _DbContext;
        private readonly BindingList<ISystemEventMessage>  _SystemEnentsLog;
        private byte _PageSize = 20; // По умолчанию
        /// <summary>
        /// Количество записей из бд на странице
        /// </summary>
        public byte PageSize
        {
            get { return _PageSize; }
            set 
            {
                if (value < 5)
                {
                    throw new ArgumentOutOfRangeException("QuantityOnPage",
                        "Попытка установить недопустимое значение количества записей на странице");
                }
                _PageSize = value; 
            }
        }
        /// <summary>
        /// Лог текущего сеанса
        /// </summary>
        public BindingList<ISystemEventMessage> SystemEnentsLog
        {
            get { return _SystemEnentsLog; }
        }

        #endregion

        #region Methods

        public void AddEvent(ISystemEventMessage eventMessage)
        {
            using (QueriesTableAdapter adapter = new QueriesTableAdapter())
            {
                int? id = 0;
                adapter.AddSystemEvent((int?)eventMessage.SystemEventCode,
                    eventMessage.Message, (DateTime?)eventMessage.Created, (byte?)eventMessage.Category,
                    eventMessage.HasRead, ref id);
                if (id.HasValue && id.Value != 0)
                    eventMessage.MessageId = id.Value;
            }
            _SystemEnentsLog.Add(eventMessage);
        }       
        /// <summary>
        /// Возвращает количество страниц записей в БД
        /// </summary>
        /// <returns></returns>
        public int GetTotalPages()
        {
            using (QueriesTableAdapter adapter = new QueriesTableAdapter())
            {
                int? count = 0;
                int result;
                result = adapter.GetSystemEventsCount(ref count);
                return count.Value > PageSize ? 
                    count.Value % PageSize == 0 ? 
                        (count.Value / PageSize) : 
                        (count.Value / PageSize) + 1 : 
                    1;
            }
        }

        public IEnumerable<ISystemEventMessage> GetPage(int pageNumber)
        {
            using (GetSystemEventsPageTableAdapter adapter = new GetSystemEventsPageTableAdapter())
            {
                int result = adapter.Fill(
                    _DbContext.GetSystemEventsPage, pageNumber, PageSize);
                return ConvertTo(_DbContext.GetSystemEventsPage); 

                //switch (result)
                //{
                //    case 0: 
                //        { 
                //            return ConvertTo(_DbContext.GetSystemEventsPage); 
                //        }
                //    case 1: { throw new InvalidOperationException("Недопустимый размер страницы"); }
                //    case 2: { throw new InvalidOperationException("Недопустимый номер страницы"); }
                //    case 3: { throw new InvalidOperationException("Страница с данным номером не существует"); }
                //    default:
                //        {
                //            throw new InvalidOperationException(
                //                String.Format("Ошбика выполнения хранимой процедуры. БД вернула код {0}", result));
                //        }
                //}
            }
        }

        private IEnumerable<ISystemEventMessage> ConvertTo(NgkDbDataSet.GetSystemEventsPageDataTable table)
        {
            SystemEventMessage message;
            List<ISystemEventMessage> list = new List<ISystemEventMessage>();

            foreach (NgkDbDataSet.GetSystemEventsPageRow row in table)
            {
                message = new SystemEventMessage();
                message.MessageId = row.MessageId;
                message.SystemEventCode = SystemEventMessage.ToSystemEventCodes(row.SystemEventCode);
                message.Message = row.Message;
                message.Category = SystemEventMessage.ToCategory(row.Category);
                message.Created = row.Created;
                message.HasRead = row.HasRead;

                list.Add(message);
            }
            return list;
        }

        private void EventHandler_SystemEnentsLog_AddingNew(object sender, AddingNewEventArgs e)
        {
            while (_SystemEnentsLog.Count >= PageSize)
            {
                _SystemEnentsLog.RemoveAt(0); // Удаляем первый элемент
            }
        }
        public void Dispose()
        {
            _DbContext.Dispose();
            _DbContext = null;
        }

        #endregion
    }
}
