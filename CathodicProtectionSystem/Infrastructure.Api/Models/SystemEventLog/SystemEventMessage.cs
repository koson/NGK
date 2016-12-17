using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dal.DbEntity
{
    public class SystemEventMessage : ISystemEventMessage
    {
        #region Constructors

        public SystemEventMessage()
        {
            _MessageId = 0;
            _Message = String.Empty;
            _Created = DateTime.Now;
            _Category = Category.Information;
            _HasRead = false;
        }

        #endregion

        #region Fields And Properties

        private int _MessageId;
        private SystemEventCodes _SystemEventCode;
        private string _Message;
        private DateTime _Created;
        private Category _Category;
        private Boolean _HasRead;

        public int MessageId
        {
            get { return _MessageId; }
            set { _MessageId = value; }
        }

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public SystemEventCodes SystemEventCode
        {
            get { return _SystemEventCode; }
            set { _SystemEventCode = value; }
        }

        public DateTime Created
        {
            get { return _Created; }
            set { _Created = value; }
        }
        
        public Category Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        
        public Boolean HasRead
        {
            get { return _HasRead; }
            set { _HasRead = value; }
        }

        #endregion

        #region Methods

        public static SystemEventCodes ToSystemEventCodes(int value)
        {
            return (SystemEventCodes)value;
        }

        public static Category ToCategory(int value)
        {
            return (Category)value;
        }

        #endregion
    }
}
