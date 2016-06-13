using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using Common.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public class Parameter : INotifyPropertyChanged
    {
        #region Constructors

        public Parameter(ObjectInfo info)
        {
            _IsSpecialParameter = false;
            _IsComplexParameter = info.IsComplexParameter;
            _DeviceType = info.DeviceProfile.DeviceType;

            if (_IsComplexParameter)
            {
                ComplexParameter complexParam = CanDevicePrototype.GetProfile(_DeviceType)
                    .ComplexParameters[info.ComplexParameterName];

                _Name = complexParam.Name;
                _Description = complexParam.Description;
                _ReadOnly = complexParam.ReadOnly;
                _Visible = complexParam.Visible;
                _DisplayName = complexParam.DisplayName;
                _MeasureUnit = complexParam.MeasureUnit;
                _Category = complexParam.Category;
                _ValueType = complexParam.Converter.ValueType;
                _DefaultValue = complexParam.DefaultValue;
            }
            else
            {
                _Indexes = new ushort[] { info.Index };
                _Name = info.Name;
                _Description = info.Description;
                _ReadOnly = info.ReadOnly;
                _Visible = info.Visible;
                _DisplayName = info.DisplayName;
                _MeasureUnit = info.MeasureUnit;
                _Category = info.Category;
                _ValueType = info.DataTypeConvertor.OutputDataType;
                _DefaultValue = info.DataTypeConvertor.ConvertToOutputValue(info.DefaultValue);
            }

            _Value = Activator.CreateInstance(_ValueType); 
            _Modified = DateTime.Now;
            _Status = ObjectStatus.NoError;
        }

        private Parameter(bool isSpecialParam, bool isComplexParam, string paramName, 
            string displayName, string description, bool readOnly, bool visible, 
            string measureUnit, ObjectCategory category, DeviceType deviceType, 
            UInt16 index, object value, object defaultValue)
        {
            _IsSpecialParameter = isSpecialParam;
            _IsComplexParameter = isComplexParam;
            _Name = paramName;
            _Description = description;
            _DisplayName = displayName;
            _MeasureUnit = measureUnit;
            _Category = category;
            _DeviceType = deviceType;
            _Modified = DateTime.Now;
            _Status = ObjectStatus.NoError;
            if (_IsSpecialParameter)
            {
                _ValueType = value.GetType();
                _Value = value;
            }
            else
            {
                _ValueType = _IsComplexParameter ? CanDevicePrototype.GetProfile(_DeviceType)
                    .ComplexParameters[_Name].Converter.ValueType :
                    CanDevicePrototype.GetProfile(_DeviceType)
                        .ObjectInfoList[index].DataTypeConvertor.OutputDataType;

                _Value = Activator.CreateInstance(_ValueType);
            }
            if ((isComplexParam == true) && (isSpecialParam == true))
            {
                throw new ArgumentException();
            }

            if (index> 0)
            {
                _IsComplexParameter = true;
            }
            else
            {
                _IsComplexParameter = false;
                _Indexes = new ushort[] { index };
            }
        }

        #endregion

        #region Fields And Propetries

        DeviceType _DeviceType;
        
        Type _ValueType;
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("��� ������")]
        [Description("��� ������ �������� ���������")]
        public Type ValueType 
        {
            get { return _ValueType; }
        } 

        bool _IsSpecialParameter;
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("����. ��������")]
        [Description("�������, ��� �������� ��� �������, �.�. �� ����� ����� ������������� � ������� ����������")]
        public bool IsSpecialParameter
        {
            get { return _IsSpecialParameter; }
        }

        bool _IsComplexParameter;
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("����������� ��������")]
        [Description("�������, ��� �������� ������� �� ���������� �������� ������� ����������")]
        public bool IsComplexParameter 
        { 
            get { return _IsComplexParameter; } 
        } 

        UInt16[] _Indexes;
        /// <summary>
        /// ����� �������
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("������� �������")]
        [Description("������� �������")]
        public UInt16[] Indexes
        {
            get
            {
                return _IsComplexParameter ? 
                    CanDevicePrototype.GetProfile(_DeviceType).ComplexParameters[Name].LinkedIndexes : 
                    _Indexes;
            }
        }

        private string _Name;
        /// <summary>
        /// �������� �������
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("������������ �������")]
        [Description("������������ ��������� (������� �������)")]
        public string Name
        {
            get { return _Name; }
            set 
            { 
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _Description;
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("��������")]
        [Description("�������� ��������� (������� �������)")]
        /// <summary>
        /// �������� �������
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set 
            { 
                _Description = value;
                OnPropertyChanged("Description");
            }
        }

        private bool _ReadOnly;
        /// <summary>
        /// ������ ������ (������ �������� �������� � ������� ����������)
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("�����������")]
        [Description("����������� ������� � ��������� (������� �������)")]
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set 
            { 
                _ReadOnly = value;
                OnPropertyChanged("ReadOnly");
            }
        }

        private bool _Visible;
        /// <summary>
        /// ���������/��������� ����������� 
        /// ������� ������� � GUI
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("��������� � GUI")]
        [Description("���������/��������� ����������� ��������� (������� �������) GUI")]
        public bool Visible
        {
            get { return _Visible; }
            set 
            { 
                _Visible = value;
                OnPropertyChanged("Visible");
            }
        }

        private string _DisplayName;
        /// <summary>
        /// ������������ ������� � GUI
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("��������")]
        [Description("������������ ��������� (������� �������) � GUI")]
        public string DisplayName
        {
            get { return _DisplayName; }
            set 
            { 
                _DisplayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        private string _MeasureUnit;
        /// <summary>
        /// ������� ���������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("�����������")]
        [Description("������� ��������� �������� ��������� (������� �������)")]
        public string MeasureUnit
        {
            get { return _MeasureUnit; }
            set 
            { 
                _MeasureUnit = value;
                OnPropertyChanged("MeasureUnit");
            }
        }

        private ObjectCategory _Category;
        /// <summary>
        /// ��������� ������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("���������")]
        [Description("��������� ��������� (������� �������)")]
        public ObjectCategory Category
        {
            get { return _Category; }
            set 
            { 
                _Category = value;
                OnPropertyChanged("Category");
            }
        }

        private object _Value;
        /// <summary>
        /// �������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("��������")]
        [Description("�������� ��������� (������� �������)")]
        public Object Value
        {
            get { return _Value; }
            set 
            { 
                _Value = value;
                OnPropertyChanged("Value");
            }
        }

        private Object _DefaultValue;

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("�������� �� ���������")]
        [Description("�������� ��������� (������� �������) �� ���������")]
        public Object DefaultValue
        {
            get { return _DefaultValue; }
            set { _DefaultValue = value; }
        }

        private DateTime _Modified;
        /// <summary>
        /// ����� ���������� ��������� �������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("����� ����������")]
        [Description("���� � ����� ���������� ������ ������� �� ���������� (������� �������)")]
        public DateTime Modified
        {
            get { return _Modified; }
            set 
            { 
                _Modified = value;
                OnPropertyChanged("Modified");
            }
        }

        private ObjectStatus _Status;
        /// <summary>
        /// ��������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("������")]
        [Description("������ ��������� (������� �������)")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public ObjectStatus Status
        {
            get { return _Status; }
            set 
            { 
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

        #endregion

        #region Methods

        public static Parameter CreateSpecialParameter(string paramName,
            string displayName, string description, string measureUnit, 
            bool readOnly, bool visible, ObjectCategory category, 
            DeviceType deviceType, object value, object defaultValue)
        {
            return new Parameter(true, false, paramName, displayName,
                description, readOnly, visible, measureUnit, category, deviceType, 0, value, defaultValue);
        }

        void OnPropertyChanged(string parameterName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(parameterName));
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
