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
            bool readOnly, bool sdoCanRead, bool visible, string displayedName,
            string measureUnit, ObjectCategory category, ICanDataTypeConvertor convertor,
            UInt32 defaultValue)
        {
            Index = index;
            Name = name;
            Description = description;
            ReadOnly = readOnly;
            SdoCanRead = sdoCanRead;
            Visible = visible;
            DisplayedName = displayedName;
            MeasureUnit = measureUnit;
            Category = category;
            DataTypeConvertor = convertor;
            DefaultValue = defaultValue;
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// Адрес объекта
        /// </summary>
        public UInt16 Index;
        /// <summary>
        /// Название объекта
        /// </summary>
        public string Name;
        /// <summary>
        /// Описание объекта
        /// </summary>
        public string Description;
        /// <summary>
        /// Только чтение (нельзя записать значение в удалёном устройстве)
        /// </summary>
        public bool ReadOnly;
        /// <summary>
        /// 
        /// </summary>
        public bool SdoCanRead;
        /// <summary>
        /// Разрешить/запретить отображение 
        /// данного индекса в GUI
        /// </summary>
        public bool Visible;
        /// <summary>
        /// Наименование объекта в GUI
        /// </summary>
        public string DisplayedName;
        /// <summary>
        /// Единица измерения
        /// </summary>
        public string MeasureUnit;
        /// <summary>
        /// Категория объекта объекта
        /// </summary>
        public ObjectCategory Category;
        /// <summary>
        /// Тип данных значения объекта
        /// </summary>
        public ICanDataTypeConvertor DataTypeConvertor;
        /// <summary>
        /// Значение по умолчнию (Значение в формате передаваемого посети)
        /// </summary>
        public UInt32 DefaultValue;

        #endregion
    }
}
