using System;
using System.ComponentModel;

namespace Infrastructure.Dal.DbEntity
{
    public interface ISystemEventMessage
    {
        [Description("Id")]
        [Browsable(false)]
        [ReadOnly(false)]
        [DisplayName("ID")]
        int MessageId { get; set; }
        
        [Description("Код события")]
        [Browsable(true)]
        [ReadOnly(false)]
        [DisplayName("Код события")]
        SystemEventCodes SystemEventCode { get; set; }
        
        [Description("Категория события")]
        [Browsable(true)]
        [ReadOnly(false)]
        [DisplayName("Категория события")]
        Category Category { get; set; }
        
        [Description("Данные события")]
        [Browsable(true)]
        [ReadOnly(false)]
        [DisplayName("Описание")]
        string Message { get; set; }
        
        [Description("Дата и время события")]
        [Browsable(true)]
        [ReadOnly(false)]
        [DisplayName("Дата и время события")]
        DateTime Created { get; set; }
        
        [Description("Прочитано")]
        [Browsable(true)]
        [ReadOnly(false)]
        [DisplayName("Прочитано")]
        bool HasRead { get; set; }
    }
}
