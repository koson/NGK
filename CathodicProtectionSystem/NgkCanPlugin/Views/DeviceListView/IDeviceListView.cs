using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using Mvp.Input;
using Infrastructure.Api.Models.CAN;

namespace NGK.Plugins.Views
{
    public interface IDeviceListView : IPartialView
    {
        DockStyle Dock { get; set; }
        BindingList<IDeviceInfo> Devices { set; }
        NgkCanDevice SelectedDevice { get; }
        event EventHandler SelectedDeviceChanged;
    }
}
