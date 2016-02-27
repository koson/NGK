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
        /// ����� �������
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("������ �������")]
        [Description("������ �������")]
        public UInt16 Index
        {
            get { return _Index; }
            set { _Index = value; }
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
            set { _Name = value; }
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
            set { _Description = value; }
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
            set { _ReadOnly = value; }
        }

        private bool _SdoCanRead;
        /// <summary>
        /// ��� ������ SDO �������
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("������ ��� SDO")]
        [Description("���������/��������� ������ ��������� (������� �������) ������� �������� SDO")]
        public bool SdoCanRead
        {
            get { return _SdoCanRead; }
            set { _SdoCanRead = value; }
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
            set { _Visible = value; }
        }

        private string _DisplayedName;
        /// <summary>
        /// ������������ ������� � GUI
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("��������")]
        [Description("������������ ��������� (������� �������) � GUI")]
        public string DisplayedName
        {
            get { return _DisplayedName; }
            set { _DisplayedName = value; }
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
            set { _MeasureUnit = value; }
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
            set { _Category = value; }
        }

        private object _Value;
        /// <summary>
        /// �������� ������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������")]
        [DisplayName("��������")]
        [Description("�������� ��������� (������� �������)")]
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
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
            set { _Modified = value; }
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
            set { _Status = value; }
        }

        #endregion

        #region Constructors

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
