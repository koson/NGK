using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IDeviceDetailView : IView
    {
        BindingSource ParametersContext { set; }
    }
}
