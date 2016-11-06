using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Dal.DbEntity
{
    /// <summary>
    /// Категория сообщения
    /// </summary>
    public enum Category: byte
    {
        [Description("Информация")]
        Information = 0,
        [Description("Ошибка")]
        Error = 1,
        [Description("Критическая ошибка")]
        CriticalError = 2
    }
}
