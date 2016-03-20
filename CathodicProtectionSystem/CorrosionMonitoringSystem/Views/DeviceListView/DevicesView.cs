using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NGK.CorrosionMonitoringSystem.Views;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class DevicesView : UserControl, IDeviceListView
    {
        #region Constructors

        public DevicesView()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        #endregion

        #region Fields And Properties

        IButtonsPanel _ButtonsPanel;

        public IButtonsPanel ButtonsPanel
        {
            get { return _ButtonsPanel; }
            set
            {
                _ButtonsPanel = value;
            }
        }

        IViewRegion[] _ViewRegions = new IViewRegion[0];

        public IViewRegion[] ViewRegions
        {
            get { return _ViewRegions; }
        }

        #endregion

        #region IDeviceListView Members

        BindingSource _Devices;

        public BindingSource Devices
        {
            set { _Devices = value ; }
        }

        #endregion

        #region IView Members


        public ViewType ViewType
        {
            get { return ViewType.Region; }
        }

        public void Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
