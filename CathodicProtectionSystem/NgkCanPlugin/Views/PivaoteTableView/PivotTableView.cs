using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using NGK.CorrosionMonitoringSystem.Views;
using Infrastructure.Api.Models.CAN;
using System.ComponentModel;
using System.Windows.Forms;

namespace NGK.Plugins.Views
{
    public class PivotTableView: PartialView<PivotTable>, IPivotTableView
    {
        #region Constructors

        public PivotTableView() { }

        #endregion

        #region Fields And Properties

        public DockStyle Dock
        {
            get { return base.Control.Dock; }
            set { base.Control.Dock = value; }
        }

        #endregion

        #region IPivotTableView Members

        public BindingList<IDeviceSummaryParameters> ParametersPivotTable
        {
            set 
            {
                Control.ParametersPivotTable = value;
            }
        }

        public void HideColumn(string columnName)
        {
            Control.HideColumn(columnName);
        }

        #endregion
    }
}
