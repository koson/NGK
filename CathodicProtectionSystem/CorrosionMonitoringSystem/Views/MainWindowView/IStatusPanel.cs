using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IStatusPanel
    {
        /// <summary>
        /// Всего устройств в системе
        /// </summary>
        Int32 TotalDevices { set; }

        /// <summary>
        /// Неисправных устройств в системе
        /// </summary>
        Int32 FaultyDevices { set; }

        /// <summary>
        /// Непрочитанные сообщения в журнале событий
        /// </summary>
        Int32 UnreadMessages { set; }
    }
}
