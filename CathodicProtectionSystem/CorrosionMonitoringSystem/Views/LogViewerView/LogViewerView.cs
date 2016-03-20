using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views.LogViewerView
{
    public partial class LogViewerView : UserControl, ILogViewerView
    {
        #region Constructors

        public LogViewerView()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        #endregion

        #region IView Members

        public ViewType ViewType
        {
            get { return ViewType.Region; }
        }

        IViewRegion[] _ViewRegions = new IViewRegion[0];

        public IViewRegion[] ViewRegions
        {
            get { return _ViewRegions; }
        }

        public void Close()
        {
            Dispose();
        }

        #endregion
    }
}
