using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Views;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public interface IViewMode
    {
        ViewMode ViewMode { get; }
    }
}
