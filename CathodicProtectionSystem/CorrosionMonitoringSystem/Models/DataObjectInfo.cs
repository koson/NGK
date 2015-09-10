using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using Common.ComponentModel;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public class DataObjectInfo
    {
        #region Fields And Propetries
        
        private UInt16 _Index;
        /// <summary>
        /// ����� �������
        /// </summary>
        public UInt16 Index
        {
            get { return _Index; }
            set { _Index = value; }
        }
        /// <summary>
        /// �������� �������
        /// </summary>
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// �������� �������
        /// </summary>
        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /// <summary>
        /// ������ ������ (������ �������� �������� � ������� ����������)
        /// </summary>
        private bool _ReadOnly;

        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }
        /// <summary>
        /// ��� ������ SDO �������
        /// </summary>
        private bool _SdoCanRead;

        public bool SdoCanRead
        {
            get { return _SdoCanRead; }
            set { _SdoCanRead = value; }
        }
        /// <summary>
        /// ���������/��������� ����������� 
        /// ������� ������� � GUI
        /// </summary>
        private bool _Visible;

        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }
        /// <summary>
        /// ������������ ������� � GUI
        /// </summary>
        private string _DisplayedName;

        public string DisplayedName
        {
            get { return _DisplayedName; }
            set { _DisplayedName = value; }
        }
        /// <summary>
        /// ������� ���������
        /// </summary>
        private string _MeasureUnit;

        public string MeasureUnit
        {
            get { return _MeasureUnit; }
            set { _MeasureUnit = value; }
        }
        /// <summary>
        /// ��������� ������� �������
        /// </summary>
        private Category _Category;

        public Category Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        /// <summary>
        /// �������� ������
        /// </summary>
        private string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        /// <summary>
        /// ����� ���������� ��������� �������� �������
        /// </summary>
        private DateTime _Modified;

        public DateTime Modified
        {
            get { return _Modified; }
            set { _Modified = value; }
        }

        private ObjectStatus _Status;
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
        private DataObjectInfo()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public DataObjectInfo(ObjectInfo info)
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
            _Value = String.Empty;
            _Modified = DateTime.Now;
            _Status = ObjectStatus.NoError;
        }
        #endregion
    }
}
