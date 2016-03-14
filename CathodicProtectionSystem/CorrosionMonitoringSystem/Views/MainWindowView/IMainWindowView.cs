using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IMainWindowView : IView,
        IButtonsPanel, IStatusPanel
    {
        /// <summary>
        /// Возвращает заголовок окна
        /// </summary>
        String Title { get; set; }
        IViewRegion WorkingRegion { get; }
        IViewRegion TitleRegion { get; }
    }
}
