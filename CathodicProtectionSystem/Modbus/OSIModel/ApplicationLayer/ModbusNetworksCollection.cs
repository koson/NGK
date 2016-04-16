using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Modbus.OSIModel.ApplicationLayer
{
    /// <summary>
    /// ����� ��� �������� ��������� ����� Modbus
    /// </summary>
    public class ModbusNetworksCollection: KeyedCollection<string, ModbusNetworkControllerBase>
    {
        #region Constructors and Destructor
        #endregion

        #region Fields and Properties
        #endregion

        #region Methods

        protected override string GetKeyForItem(ModbusNetworkControllerBase item)
        {
            return item.NetworkName;
        }
        /// <summary>
        /// ���������� ��������� � ���� �������
        /// </summary>
        /// <returns>������ ��������� ���������</returns>
        public ModbusNetworkControllerBase[] ToArray()
        {
            ModbusNetworkControllerBase[] array = new ModbusNetworkControllerBase[Count];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this[i];
            }
            return array;
        }
        #endregion
    }
}
