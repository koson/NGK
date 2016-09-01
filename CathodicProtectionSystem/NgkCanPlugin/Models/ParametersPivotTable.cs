using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using System.ComponentModel;

namespace NGK.Plugins.Models
{
    /// <summary>
    /// Класс формирует сводную таблицу значений объектов словаря каждого устройства сети
    /// (только для КИП). Следит за изменениями этих параметров.
    /// </summary>
    public class ParametersPivotTable
    {
        #region Constructors

        public ParametersPivotTable(BindingList<NgkCanDevice> devices)
        {
            _Devices = devices;
            
            Devices = new BindingList<IDeviceSummaryParameters>();
            
            foreach (NgkCanDevice device in devices)
            {
                Devices.Add(device);
            }
        }

        #endregion

        #region Fields And Properties

        /// <summary>
        /// Список устройств в сети относящихся к КИП 
        /// </summary>
        private BindingList<NgkCanDevice> _Devices;

        /// <summary>
        /// Сводная таблица
        /// </summary>
        public readonly BindingList<IDeviceSummaryParameters> Devices;

        #endregion

        #region Methods
        #endregion

        #region Events        
        #endregion
    }
}