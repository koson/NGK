using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.View
{
    public interface IDeviceDetailView: IView
    {
        event EventHandler<ButtonClickEventArgs> ButtonClick;
    }
}
