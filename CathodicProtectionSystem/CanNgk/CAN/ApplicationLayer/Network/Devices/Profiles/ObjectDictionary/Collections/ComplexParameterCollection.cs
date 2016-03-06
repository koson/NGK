using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.Collections
{
    public class ComplexParameterCollection : KeyedCollection<string, ComplexParameter>
    {
        protected override string GetKeyForItem(ComplexParameter item)
        {
            return item.Name;
        }
    }
}
