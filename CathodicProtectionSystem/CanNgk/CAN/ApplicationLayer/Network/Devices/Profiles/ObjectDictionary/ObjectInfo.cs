using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    /// <summary>
    /// Класс для описания объекта словаря устройства
    /// </summary>
    public class ObjectInfo
    {
        #region Constructors

        public ObjectInfo(UInt16 index, string name, string description,
            bool readOnly, bool sdoCanRead, bool visible, string displayName,
            string measureUnit, ObjectCategory category, ICanDataTypeConvertor convertor,
            UInt32 defaultValue)
        {
            Index = index;
            Name = name;
            Description = description;
            ReadOnly = readOnly;
            SdoCanRead = sdoCanRead;
            Visible = visible;
            DisplayName = displayName;
            MeasureUnit = measureUnit;
            Category = category;
            DataTypeConvertor = convertor;
            DefaultValue = defaultValue;
            ComplexParameterId = null;
        }

        public ObjectInfo(UInt16 index, string name, string description,
            bool readOnly, bool sdoCanRead, bool visible, string displayName,
            string measureUnit, ObjectCategory category, ICanDataTypeConvertor convertor,
            UInt32 defaultValue, String complexParameterName)
        {
            Index = index;
            Name = name;
            Description = description;
            ReadOnly = readOnly;
            SdoCanRead = sdoCanRead;
            Visible = visible;
            DisplayName = displayName;
            MeasureUnit = measureUnit;
            Category = category;
            DataTypeConvertor = convertor;
            DefaultValue = defaultValue;
            ComplexParameterName = complexParameterName;
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// Адрес объекта
        /// </summary>
        public UInt16 Index { get { return LinkedIndexes[0]; } }
        /// <summary>
        /// Название объекта
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Описание объекта
        /// </summary>
        public readonly string Description;
        /// <summary>
        /// Только чтение (нельзя записать значение в удалёном устройстве)
        /// </summary>
        public readonly bool ReadOnly;
        /// <summary>
        /// 
        /// </summary>
        public readonly bool SdoCanRead;
        /// <summary>
        /// Разрешить/запретить отображение 
        /// данного индекса в GUI
        /// </summary>
        public readonly bool Visible;
        /// <summary>
        /// Наименование объекта в GUI
        /// </summary>
        public readonly string DisplayName;
        /// <summary>
        /// Единица измерения
        /// </summary>
        public readonly string MeasureUnit;
        /// <summary>
        /// Категория объекта объекта
        /// </summary>
        public readonly ObjectCategory Category;
        /// <summary>
        /// Тип данных значения объекта
        /// </summary>
        public readonly ICanDataTypeConvertor DataTypeConvertor;
        /// <summary>
        /// Значение по умолчнию (Значение в формате передаваемого посети)
        /// </summary>
        public readonly UInt32 DefaultValue;
        /// <summary>
        /// 
        /// </summary>
        public readonly string ComplexParameterName;

        /// <summary>
        /// Указывает является ли данный объект участником составного типа данных
        /// </summary>
        public bool IsComplexParameter
        {
            get { return ComplexParameterName != null; }
        }

        #endregion
    }
}
