using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using System.ComponentModel;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IDeviceDetailView : IView
    {
        BindingList<Parameter> Parameters { set; }
    }
}
