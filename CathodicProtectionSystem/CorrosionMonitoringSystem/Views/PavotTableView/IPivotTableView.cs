using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Data;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IPivotTableView : IView
    {
        DataTable Parameters { set; }
    }
}
