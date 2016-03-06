using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    public interface IObjectsCombiner
    {
        Object Combine(Dictionary<UInt16, Object> objects);
        Dictionary<UInt16, Object> Decombine(object value);
    }
}
