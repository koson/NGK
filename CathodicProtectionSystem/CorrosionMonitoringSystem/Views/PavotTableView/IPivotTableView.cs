using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Data;
using System.ComponentModel;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IPivotTableView : IView
    {
        BindingList<IDeviceSummaryParameters> ParametersPivotTable { set; }
        void HideColumn(string columnName);
    }
}
