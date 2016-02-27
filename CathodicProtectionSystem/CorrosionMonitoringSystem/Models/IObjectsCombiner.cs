using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public interface IObjectsCombiner
    {
        Object Combine(Dictionary<UInt16, Object> objects);
    }
}
