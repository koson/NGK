using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.View
{
    public interface ILogViewerView: IView
    {
        event EventHandler<ButtonClickEventArgs> ButtonClick;
    }
}
