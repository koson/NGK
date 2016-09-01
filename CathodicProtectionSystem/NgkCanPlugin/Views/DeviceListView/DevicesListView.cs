using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using NGK.Plugins.Presenters;

namespace NGK.Plugins.Views
{
    public class DevicesListView: PartialView<DevicesList>
    {
        #region Constructors

        public DevicesListView()
        {
            Control.DataBindings.Add(new Binding("SelectedDevice", (DevicesListPresenter)Context.Presenter, "SelectedDevice"));
        }

        #endregion

        #region Fields And Properties

        public DockStyle Dock
        {
            get { return base.Control.Dock; }
            set { base.Control.Dock = value; }
        }

        public override PartialViewContext<PartialView<DevicesList>> Context
        {
            get { return base.Context; }
            set
            {
                base.Context = value;
                Control.Devices = null;
                if (value != null)
                {
                    Control.Devices = ((DevicesListPresenter)value.Presenter).Devices;
                }
            }
        }

        #endregion 
    }
}
