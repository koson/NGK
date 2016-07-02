using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes;
using System.ComponentModel;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    /// <summary>
    /// Класс для описания объекта словаря устройства
    /// </summary>
    public class ObjectInfo
    {
        #region Constructors
        
        /// <summary>
        /// Конструктор простого объекта словаря
        /// </summary>
        /// <param name="deviceProfile"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="readOnly"></param>
        /// <param name="sdoCanRead"></param>
        /// <param name="visible"></param>
        /// <param name="displayName"></param>
        /// <param name="measureUnit"></param>
        /// <param name="category"></param>
        /// <param name="convertor"></param>
        /// <param name="defaultValue"></param>
        /// <param name="typeConverter"></param>
        public ObjectInfo(ICanDeviceProfile deviceProfile, UInt16 index, string name, string description,
            bool readOnly, bool sdoCanRead, bool visible, string displayName,
            string measureUnit, ObjectCategory category, ICanDataTypeConvertor convertor,
            UInt32 defaultValue, TypeConverter typeConverter)
        {
            DeviceProfile = deviceProfile;
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
            ComplexParameterName = null;
            TypeConverter = typeConverter;
        }

        /// <summary>
        /// Конструктор для создания комплексного (составног) объекта (значение которое состоит
        /// из нескольких объектов словаря)
        /// </summary>
        /// <param name="deviceProfile"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="readOnly"></param>
        /// <param name="sdoCanRead"></param>
        /// <param name="visible"></param>
        /// <param name="displayName"></param>
        /// <param name="measureUnit"></param>
        /// <param name="category"></param>
        /// <param name="convertor"></param>
        /// <param name="defaultValue"></param>
        /// <param name="complexParameterName"></param>
        public ObjectInfo(ICanDeviceProfile deviceProfile, UInt16 index, string name, string description,
            bool readOnly, bool sdoCanRead, bool visible, string displayName,
            string measureUnit, ObjectCategory category, ICanDataTypeConvertor convertor,
            UInt32 defaultValue, String complexParameterName)
        {
            DeviceProfile = deviceProfile;
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
        public UInt16 Index;
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
        /// Конвертер для преобразования данных полученных из сети 
        /// в выходное значение и обратно 
        /// </summary>
        public readonly ICanDataTypeConvertor DataTypeConvertor;
        /// <summary>
        /// Значение по умолчнию (Значение в формате передаваемого посети)
        /// </summary>
        public UInt32 DefaultValue;
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

        /// <summary>
        /// Конвертер для преобразования заняения объекта строку и обратно
        /// (Значение объетка TotalValue )
        /// </summary>
        public readonly TypeConverter TypeConverter;

        public readonly ICanDeviceProfile DeviceProfile;

        #endregion
    }
}
