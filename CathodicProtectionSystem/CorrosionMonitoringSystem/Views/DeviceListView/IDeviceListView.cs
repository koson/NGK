using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using Mvp.Input;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IDeviceListView : IView
    {
        BindingSource Devices { set; }
    }
}
