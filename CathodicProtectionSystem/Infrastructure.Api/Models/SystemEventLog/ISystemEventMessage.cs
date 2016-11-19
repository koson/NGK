using System;
using System.ComponentModel;

namespace Infrastructure.Dal.DbEntity
{
    public interface ISystemEventMessage
    {
        [Description("Id")]
        int MessageId { get; set; }
        [Description("Код события")]
        SystemEventCodes SystemEventCode { get; set; }
        [Description("Категория события")]
        Category Category { get; set; }
        [Description("Данные события")]
        string Message { get; set; }
        [Description("Дата и время события")]
        DateTime Created { get; set; }
        [Description("Прочитано")]
        bool HasRead { get; set; }
    }
}
