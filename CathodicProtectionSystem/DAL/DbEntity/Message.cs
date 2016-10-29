using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.DAL.DbModels
{
    public class LogRecord
    {
        #region Constructors

        public LogRecord()
        {
            _MessageId = 0;
            _Message = String.Empty;
            _Created = DateTime.Now;
            _Category = Category.Information;
            _HasRead = false;
        }

        #endregion

        #region Fields And Properties

        int _MessageId;

        public int MessageId
        {
            get { return _MessageId; }
            set { _MessageId = value; }
        }

        string _Message;

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        DateTime _Created;

        public DateTime Created
        {
            get { return _Created; }
            set { _Created = value; }
        }
        
        Category _Category;

        public Category Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        
        Boolean _HasRead;

        public Boolean HasRead
        {
            get { return _HasRead; }
            set { _HasRead = value; }
        }

        #endregion
    }
}
