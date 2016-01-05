using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.View
{
    public partial class DeviceListView : TemplateView, IDeviceListView
    {
        public DeviceListView()
        {
            InitializeComponent();
        }
    }
}