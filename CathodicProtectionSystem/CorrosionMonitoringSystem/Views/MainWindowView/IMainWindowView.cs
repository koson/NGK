using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IMainWindowView : IWindowView
    {
        /// <summary>
        /// Возвращает заголовок окна
        /// </summary>
        String Title { get; set; }

        bool CursorEnabled { get; set; }
        bool ShowInTaskbar { get; set; }
        bool FormBorderEnable { get; set; }
        bool FullScreen { get; set; }
    }
}
