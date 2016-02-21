using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.View
{
    /// <summary>
    /// Реализует взаимодействие с кнопками панели приложения
    /// </summary>
    public interface IButtonsPanel
    {
        Boolean ButtonF3IsAccessible { get; set; }
        Boolean ButtonF4IsAccessible { get; set; }
        Boolean ButtonF5IsAccessible { get; set; }

        String ButtonF3Text { set; }
        String ButtonF4Text { set; }
        String ButtonF5Text { set; }

        event EventHandler<ButtonClickEventArgs> ButtonClick;
    }
}
