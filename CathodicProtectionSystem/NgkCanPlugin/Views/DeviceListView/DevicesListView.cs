using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using NGK.Plugins.Presenters;
using System.ComponentModel;
using Infrastructure.API.Models.CAN;

namespace NGK.Plugins.Views
{
    public class DevicesListView: PartialView<DevicesList>, IDeviceListView
    {
        #region Constructors

        public DevicesListView()
        {
            //Control.DataBindings.Add(new Binding("SelectedDevice", Context.Presenter, "SelectedDevice"));
        }

        #endregion

        #region Fields And Properties

        public DockStyle Dock
        {
            get { return base.Control.Dock; }
            set { base.Control.Dock = value; }
        }

        #endregion

        #region IDeviceListView Members

        public BindingList<IDeviceInfo> Devices
        {
            set { Control.Devices = value; }
        }

        public NgkCanDevice SelectedDevice
        {
            get { return Control.SelectedDevice; }
        }

        public event EventHandler SelectedDeviceChanged;

        #endregion
    }
}
