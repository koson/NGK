using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
//
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Devices;

namespace NGK.CAN.ApplicationLayer.Network.Master.Collections
{
    /// <summary>
    /// Класс-контейнер для хранения списка сетей CAN НГК-ЭХЗ
    /// </summary>
    [Serializable]
    public sealed class NetworkControllersCollection : 
        KeyedCollection<UInt32, NetworkController>
    {
        #region Methods

        protected override UInt32 GetKeyForItem(NetworkController item)
        {
            return item.NetworkId;
        }
        #endregion
    }
}
