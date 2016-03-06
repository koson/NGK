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
        #region Constructors

        public Parameter(ObjectInfo info)
        {
            //_Index = info.Index;
            _Name = info.Name;
            _Description = info.Description;
            _ReadOnly = info.ReadOnly;
            //_SdoCanRead = info.SdoCanRead;
            _Visible = info.Visible;
            _DisplayName = info.DisplayName;
            _MeasureUnit = info.MeasureUnit;
            _Category = info.Category;
            _Value = Activator.CreateInstance(info.DataTypeConvertor.OutputDataType);
            _Modified = DateTime.Now;
            _Status = ObjectStatus.NoError;
        }

        //public Parameter(UInt16 index, string name, string description, bool readOnly,
        //    bool sdoCanRead, bool visible, string displayedName, string measureUnit,
        //    ObjectCategory category, object value)
        //{
        //    //_Index = index;
        //    _Name = name == null ? string.Empty : name;
        //    _Description = description == null ? string.Empty : description;
        //    _ReadOnly = readOnly;
        //    //_SdoCanRead = sdoCanRead;
        //    _Visible = visible;
        //    _DisplayedName = displayedName == null ? string.Empty : displayedName;
        //    _MeasureUnit = measureUnit == null ? string.Empty : measureUnit;
        //    _Category = category;
        //    _Value = value;
        //    _Modified = DateTime.Now;
        //    _Status = ObjectStatus.NoError;
        //}

        public Parameter(String parameterName, String description,
            String displayedName, String measureUnit, Boolean visible,
            ObjectCategory category, ObjectInfo[] objectInfos, 
            IObjectsCombiner combiner)
        {
            _Combiner = combiner;

            if ((objectInfos.Length > 0) && (_Combiner == null))
            {
                throw new ArgumentNullException("combiner", String.Format(
                    "Системый параметр {0}. IObjectsCombiner не может быть null, " +
                    "Эесли параметр состоит более чем одного объекта словаря", parameterName)); 
            }

            _Objects = new Dictionary<ushort, object>();

            foreach (ObjectInfo info in objectInfos)
            {
                _Objects.Add(info.Index, Activator.CreateInstance(info.DataTypeConvertor.OutputDataType));
            }

            _Name = parameterName;
            _Description = description;
            _Visible = visible;
            _DisplayName = displayedName;
            _MeasureUnit = measureUnit == null ? String.Empty : measureUnit;
            _Category = category;
            _Modified = DateTime.Now;
            _Status = ObjectStatus.NoError;
        }

        #endregion

        #region Fields And Propetries

        Dictionary<UInt16, Object> _Objects;
        IObjectsCombiner _Combiner;

        /// <summary>
        /// Адрес объекта
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Индексы объекта")]
        [Description("Индексы объекта")]
        public UInt16[] Indexes
        {
            get
            {
                UInt16[] array = new ushort[_Objects.Keys.Count];
                _Objects.Keys.CopyTo(array, 0);
                return array;
            }
        }

        private string _Name;
        /// <summary>
        /// Название объекта
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Наименование объекта")]
        [Description("Наименование параметра (объекта словаря)")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Description;
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Описание")]
        [Description("Описание параметра (объекта словаря)")]
        /// <summary>
        /// Описание объекта
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private bool _ReadOnly;
        /// <summary>
        /// Только чтение (нельзя записать значение в удалёном устройстве)
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Доступность")]
        [Description("Модификатор доступа к параметру (объекту словаря)")]
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }

        private bool _Visible;
        /// <summary>
        /// Разрешить/запретить отображение 
        /// данного индекса в GUI
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Видимость в GUI")]
        [Description("Разрешает/запрещает отображение параметра (объекта словаря) GUI")]
        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        private string _DisplayName;
        /// <summary>
        /// Наименование объекта в GUI
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Параметр")]
        [Description("Наименование параметра (объекта словаря) в GUI")]
        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        private string _MeasureUnit;
        /// <summary>
        /// Единица измерения
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Размерность")]
        [Description("Единица измерения значения параметра (объекта словаря)")]
        public string MeasureUnit
        {
            get { return _MeasureUnit; }
            set { _MeasureUnit = value; }
        }

        private ObjectCategory _Category;
        /// <summary>
        /// Категория объекта объекта
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Категория")]
        [Description("Категория параметра (объекта словаря)")]
        public ObjectCategory Category
        {
            get { return _Category; }
            set { _Category = value; }
        }

        private object _Value;
        /// <summary>
        /// Значение объкта
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Значение")]
        [Description("Значение параметра (объекта словаря)")]
        public Object Value
        {
            get { return _Value; }
            private set { _Value = value; }
        }

        private DateTime _Modified;
        /// <summary>
        /// Время последнего получения значения объекта
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Время обновления")]
        [Description("Дата и время последнего чтения объекта из устройтсва (объекта словаря)")]
        public DateTime Modified
        {
            get { return _Modified; }
            set { _Modified = value; }
        }

        private ObjectStatus _Status;
        /// <summary>
        /// Состояние объекта
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Параметр")]
        [DisplayName("Статус")]
        [Description("Статус параметра (объекта словаря)")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public ObjectStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        #endregion

        #region Methods

        public void Combine()
        {
            if (_Combiner != null)
                Value = _Combiner.Combine(_Objects);
            else
                Value = _Objects[Indexes[0]];
        }

        public bool SetObjectValue(UInt16 index, Object value)
        {
            if (_Objects.ContainsKey(index))
            {
                if (_Objects[index].GetType() == value.GetType())
                {
                    _Objects[index] = value;
                    return true;
                }
            }
            return false;
        }

        public bool SetParamValue(Object value)
        {
            if (_Combiner != null)
            {
                Dictionary<UInt16, Object> objects = _Combiner.Decombine(value);
                if (objects.Count == _Objects.Count)
                {
                    foreach (KeyValuePair<UInt16, Object> pair in objects)
                    {
                        _Objects[pair.Key] = pair.Value;
                    }
                }
                else
                    throw new InvalidCastException();
            }
            else
            {
                _Objects[Indexes[0]] = value;
            }
        }

        #endregion
    }
}
