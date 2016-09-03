using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using System.ComponentModel;
using Infrastructure.API.Models.CAN;

namespace NGK.Plugins.Views
{
    public interface IDeviceDetailView : IView
    {
        BindingList<Parameter> Parameters { set; }
    }
}
