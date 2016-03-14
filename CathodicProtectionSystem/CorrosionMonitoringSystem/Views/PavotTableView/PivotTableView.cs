using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class PivotTableView : UserControl, IPivotTableView
    {
        #region Constructors

        public PivotTableView()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        #endregion

        #region IView Members

        public ViewType ViewType
        {
            get { return ViewType.Control; }
        }

        IViewRegion[] _ViewRegions = new IViewRegion[0];

        public IViewRegion[] ViewRegions
        {
            get { return _ViewRegions; }
        }

        public void Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
