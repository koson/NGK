using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataTypes
{
    [Serializable]
    public abstract class CanDataTypeConvertorBase: ICanDataTypeConvertor
    {
        #region Fields And Properties
        
        protected decimal _Scaler;
        protected Boolean _Signed;
        
        public decimal Scaler
        {
            get { return _Scaler; }
        }

        public bool Signed
        {
            get { return _Signed; }
        }

        public abstract bool IsBoolean { get; }

        public abstract Type OutputDataType { get ; }

        #endregion

        #region Methods

        public abstract ValueType ConvertToOutputValue(uint basis);

        public abstract uint ConvertToBasis(ValueType outputValue);

        public abstract byte[] ToArray(uint basis);

        public abstract uint ConvertFromArray(byte[] array);

        #endregion

        #region IXmlSerializable Members

        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
