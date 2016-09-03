using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using NGK.Plugins.Presenters;

namespace NGK.Plugins.Views
{
    public class DeviceDetailView: PartialView<DeviceDetail>
    {
        #region Fields And Properties

        public DockStyle Dock
        {
            get { return base.Control.Dock; }
            set { base.Control.Dock = value; }
        }

        #endregion 
    }
}
