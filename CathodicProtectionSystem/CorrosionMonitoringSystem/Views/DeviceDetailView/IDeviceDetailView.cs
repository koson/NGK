using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.View
{
    public interface IDeviceDetailView : IView, IButtonsPanel
    {
        BindingSource ParametersContext { set; }
    }
}
