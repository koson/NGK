using System;
using System.Collections.ObjectModel;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.Collections
{
    public class ObjectInfoCollection: KeyedCollection<UInt16, ObjectInfo>
    {
        protected override ushort GetKeyForItem(ObjectInfo item)
        {
            return item.Index;
        }
    }
}
