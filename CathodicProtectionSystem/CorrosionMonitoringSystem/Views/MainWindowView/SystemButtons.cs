using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NGK.CorrosionMonitoringSystem.Views
{
    /// <summary>
    /// Системные кнопки на панели
    /// </summary>
    public enum SystemButtons
    {
        /// <summary>
        /// Меню
        /// </summary>
        [Description("Кнопка F2")]
        F2,
        [Description("Кнопка F3")]
        F3,
        [Description("Кнопка F4")]
        F4,
        [Description("Кнопка F5")]
        F5,
        /// <summary>
        /// Скрыть/отобразить панель кнопок
        /// </summary>
        [Description("Скрыть/отобразить панель кнопок")]
        F6
    }
}
