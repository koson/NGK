using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Text;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public class ParametersCollection: KeyedCollection<UInt16, Parameter>
    {
        protected override ushort GetKeyForItem(Parameter item)
        {
            return item.Index;
        }
    }
}
