using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Text;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public class ParametersCollection: KeyedCollection<string, Parameter>
    {
        protected override string GetKeyForItem(Parameter item)
        {
            return item.Name;
        }
    }
}
