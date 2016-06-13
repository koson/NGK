using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    public class ComplexParameter
    {
        #region Constructors

        public ComplexParameter(string parameterName, string displayName,
            string description, bool readOnly, bool visible, string measureUnit,
            ObjectCategory category, IComplexParameterConverter converter, object defaultValue,
            params UInt16[] indexes)
        {
            Name = parameterName;
            DisplayName = displayName;
            Description = description;
            ReadOnly = readOnly;
            Visible = visible;
            MeasureUnit = measureUnit;
            Category = category;
            LinkedIndexes = indexes;
            _Converter = converter;
            if (defaultValue.GetType() == converter.ValueType)
                DefaultValue = defaultValue;
            else
                throw new ArgumentException(String.Format(
                    "Попытка установить значения по умолчанию для комплексного параметра, " +
                    "тип которого не соответствует типу значения комплексного параметра. " + 
                    "Название комплексного параметар: {0}", parameterName), "defaultValue");
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// Наименование параметра
        /// </summary>
        public readonly string Name;
        public readonly string DisplayName;
        public readonly string Description;
        public readonly bool ReadOnly;
        public readonly bool Visible;
        public readonly string MeasureUnit;
        public readonly ObjectCategory Category;
        public readonly object DefaultValue;

        /// <summary>
        /// Индексы объектов словаря устройства, которые участвуют
        /// в образовании комлексного типа параметра
        /// </summary>
        public readonly UInt16[] LinkedIndexes;

        IComplexParameterConverter _Converter;
        /// <summary>
        /// Объект для сборки выходного значения параметра из 
        /// объектов словаря 
        /// </summary>
        public IComplexParameterConverter Converter
        {
            get { return (IComplexParameterConverter)_Converter.Clone(); }
        }

        #endregion
    }
}
