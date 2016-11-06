using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Dal.DbEntity
{
    /// <summary>
    /// Соотвествует записям таблицы SystemEventCodes
    /// </summary>
    public enum SystemEventCodes: int
    {
        [Description("Не определено")]
        Undefined = 0,
        [Description("Запуск системы")]
        SystemWasStarted = 1,
        [Description("Остановка системы")]
        SystemWasStopped = 2
    }
}
