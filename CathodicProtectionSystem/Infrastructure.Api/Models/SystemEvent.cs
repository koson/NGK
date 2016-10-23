using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Component = System.ComponentModel;

namespace Infrastructure.Api.Models
{
    /// <summary>
    /// ����� ��� ������������� ���������� �������
    /// </summary>
    public class SystemEvent
    {
        public enum Category : int
        {
            Undefined = 0,
            CriticalError = 1,
            Error = 2,
            Warring = 3,
            Information = 4
        }

        public enum SystemEventCodes: int
        {
            Undefined = 0,
            CommunicationError = 1,
            ConfigurationError = 2,
            
        }

        #region Constructors

        public SystemEvent()
        {
            _Uid = null;
            _EventCode = SystemEventCodes.Undefined;
            _Description = String.Empty;
            _EventDateTime = DateTime.Now;
        }

        public SystemEvent(SystemEventCodes eventCode, Category category,
            string description, DateTime dateTime)
        {
            _Uid = null;
            _EventCode = eventCode;
            _Category = category;
            _Description = description;
            _EventDateTime = dateTime;
        }

        #endregion

        #region Fields And Properties

        private int? _Uid;
        private SystemEventCodes _EventCode;
        private Category _Category;
        private string _Description;
        private DateTime _EventDateTime;

        /// <summary>
        /// ���������� ������������� �������
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        //[Component::Category("��������")]
        [DisplayName("UID")]
        [Description("���������� ������������� ���������� �������")]
        public int? Uid
        {
            get { return _Uid; }
            set { _Uid = value; }
        }
        /// <summary>
        /// ��� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        //[Component::Category("��������")]
        [DisplayName("��� �������")]
        [Description("��� ���������� �������")]
        public SystemEventCodes EventCode
        {
            get { return _EventCode; }
            set { _EventCode = value; }
        }
        /// <summary>
        /// ��������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        //[Component::Category("��������")]
        [DisplayName("���������")]
        [Description("��������� ���������� �������")]
        public Category EventCategory
        {
            get { return _Category; }
            set { _Category = value; }
        }
        /// <summary>
        /// �������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        //[Component::Category("��������")]
        [DisplayName("��������")]
        [Description("�������� ���������� �������")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /// <summary>
        /// ���� � ����� ����������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        //[Component::Category("��������")]
        [DisplayName("���� � �����")]
        [Description("���� � ����� ����������� ���������� �������")]
        public DateTime EventDateTime 
        {
            get { return _EventDateTime; }
            set { _EventDateTime = value; }
        }

        #endregion
    }
}
