using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    /// <summary>
    /// Категория объекта словаря объектов устройства CAN НГК-ЭХЗ
    /// </summary>
    [Serializable]
    public enum Category: int
    {
        /// <summary>
        /// Категория объекту словаря не присвоина.
        /// </summary>
        None = 0,
        /// <summary>
        /// Объект словаря содержащий системныйе данные (серийный номер и т.п.)
        /// </summary>
        System = 1,
        /// <summary>
        /// Объект словаря содержащий конфигурационные данные (разрешения того или
        /// иного измерения параметра и т.п. )
        /// </summary>
        Configuration = 2,
        /// <summary>
        /// Объект словаря для хранения значения измеренного параметра.
        /// </summary>
        Measured = 3,
    }
}
