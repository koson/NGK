using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using Mvp.Input;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.View
{
    public interface IDeviceListView : IView, IButtonsPanel
    {
        BindingSource Devices { set; }
    }
}
