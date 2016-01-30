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
        #region Constructors

        public DeviceListView()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void EventHandler_DeviceListView_Load(object sender, EventArgs e)
        {
        }

        #endregion
    }
}