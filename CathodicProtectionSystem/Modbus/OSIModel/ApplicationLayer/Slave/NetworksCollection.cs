using System;
using System.Collections.Generic;
using System.Text;

namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    /// <summary>
    /// Класс для хранения коллекции сетей Modbus
    /// </summary>
    public class NetworksCollection: 
        System.Collections.ObjectModel.KeyedCollection<string, NetworkController>
    {
        #region Fields and Properties
        #endregion

        #region Constructors and Destructor
        #endregion

        #region Methods

        protected override string GetKeyForItem(NetworkController item)
        {
            return item.NetworkName;
        }
        /// <summary>
        /// Возвращает коллекцию в виде массива
        /// </summary>
        /// <returns>Массив элементов коллекции</returns>
        public NetworkController[] ToArray()
        {
            NetworkController[] array = new NetworkController[this.Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this[i];
            }
            return array;
        }
        #endregion
    }
}
