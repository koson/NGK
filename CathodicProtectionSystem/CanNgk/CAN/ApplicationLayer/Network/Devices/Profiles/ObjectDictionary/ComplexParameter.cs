using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    public class ComplexParameter
    {
        #region Constructors

        public ComplexParameter(string parameterName, UInt16[] indexes, IObjectsCombiner combiner)
        {
            Name = parameterName;
            LinkedIndexes = indexes;
            Combiner = combiner;
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// Наименование параметра
        /// </summary>
        public readonly String Name;
        /// <summary>
        /// Индексы объектов словаря устройства, которые участвуют
        /// в образовании комлексного типа параметра
        /// </summary>
        public readonly UInt16[] LinkedIndexes;
        /// <summary>
        /// Объект для сборки выходного значения параметра из 
        /// объектов словаря 
        /// </summary>
        public readonly IObjectsCombiner Combiner;

        #endregion
    }
}
