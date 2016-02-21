using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using Common.ComponentModel;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public class Parameter
    {
        #region Fields And Propetries
        
        private UInt16 _Index;
        /// <summary>
        /// Адрес объекта
        /// </summary>
        public UInt16 Index
        {
            get { return _Index; }
            set { _Index = value; }
        }
        /// <summary>
        /// Название объекта
        /// </summary>
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// Описание объекта
        /// </summary>
        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /// <summary>
        /// Только чтение (нельзя записать значение в удалёном устройстве)
        /// </summary>
        private bool _ReadOnly;

        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }
        /// <summary>
        /// Для работы SDO сервиса
        /// </summary>
        private bool _SdoCanRead;

        public bool SdoCanRead
        {
            get { return _SdoCanRead; }
            set { _SdoCanRead = value; }
        }
        /// <summary>
        /// Разрешить/запретить отображение 
        /// данного индекса в GUI
        /// </summary>
        private bool _Visible;

        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }
        /// <summary>
        /// Наименование объекта в GUI
        /// </summary>
        private string _DisplayedName;

        public string DisplayedName
        {
            get { return _DisplayedName; }
            set { _DisplayedName = value; }
        }
        /// <summary>
        /// Единица измерения
        /// </summary>
        private string _MeasureUnit;

        public string MeasureUnit
        {
            get { return _MeasureUnit; }
            set { _MeasureUnit = value; }
        }
        /// <summary>
        /// Категория объекта объекта
        /// </summary>
        private ObjectCategory _Category;

        public ObjectCategory Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        /// <summary>
        /// Значение объкта
        /// </summary>
        private object _Value;

        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        /// <summary>
        /// Время последнего получения значения объекта
        /// </summary>
        private DateTime _Modified;

        public DateTime Modified
        {
            get { return _Modified; }
            set { _Modified = value; }
        }

        private ObjectStatus _Status;

        /// <summary>
        /// Состояние объекта
        /// </summary>
        [TypeConverter(typeof(EnumTypeConverter))]
        public ObjectStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public Parameter(ObjectInfo info)
        {
            _Index = info.Index;
            _Name = info.Name;
            _Description = info.Description;
            _ReadOnly = info.ReadOnly;
            _SdoCanRead = info.SdoCanRead;
            _Visible = info.Visible;
            _DisplayedName = info.DisplayedName;
            _MeasureUnit = info.MeasureUnit;
            _Category = info.Category;
            _Value = Activator.CreateInstance(info.DataTypeConvertor.OutputDataType);
            _Modified = DateTime.Now;
            _Status = ObjectStatus.NoError;
        }

        public Parameter(UInt16 index, string name, string description, bool readOnly,
            bool sdoCanRead, bool visible, string displayedName, string measureUnit,
            ObjectCategory category, object value)
        {
            _Index = index;
            _Name = name == null ? string.Empty : name;
            _Description = description == null ? string.Empty : description;
            _ReadOnly = readOnly;
            _SdoCanRead = sdoCanRead;
            _Visible = visible;
            _DisplayedName = displayedName == null ? string.Empty : displayedName;
            _MeasureUnit = measureUnit == null ? string.Empty : measureUnit;
            _Category = category;
            _Value = value;
            _Modified = DateTime.Now;
            _Status = ObjectStatus.NoError;
        }

        #endregion
    }
}
