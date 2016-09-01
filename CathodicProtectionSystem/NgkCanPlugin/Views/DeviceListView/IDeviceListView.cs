using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using Mvp.Input;
using NGK.Plugins.Models;

namespace NGK.Plugins.Views
{
    public interface IDeviceListView : IView
    {
        BindingList<IDeviceInfo> Devices { set; }
        NgkCanDevice SelectedDevice { get; }
        event EventHandler SelectedDeviceChanged;
    }
}
