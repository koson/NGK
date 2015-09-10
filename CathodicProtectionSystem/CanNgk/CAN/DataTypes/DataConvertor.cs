using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// ������� ����� ��� �������� ����� ������
    /// </summary>
    [Serializable]
    public abstract class DataConvertor: IXmlSerializable
    {
        #region Fields And Properties
        protected decimal _Scaler;
        /// <summary>
        /// ��������� �����. ����� ������ �����
        /// (�� struct Scaler)
        /// </summary>
        public decimal Scaler 
        { 
            get { return _Scaler; } 
        }
        protected Boolean _Signed;
        /// <summary>
        /// ����� �������� ��� �����������
        /// </summary>
        public Boolean Signed 
        { 
            get { return _Signed; } 
        }
        /// <summary>
        /// ����� ���
        /// </summary>
        public abstract Boolean IsBoolean
        { get; }

        #endregion

        #region Constructors
  
        #endregion

        #region Methods
        /// <summary>
        /// ����������� ������� ����� (�������� ������������ �� ����) 
        /// � �������� ����� 
        /// </summary>
        /// <param name="value">������������� �������� ���������� � ������  
        /// (��������/�����������, �����/������������)</param>
        /// <returns></returns>
        public abstract ValueType ConvertToTotalValue(UInt32 basis);
        /// <summary>
        /// ����������� �������� � ����� ��� �������� �� ���� 
        /// ��� ��� �������� � ������� ��������
        /// </summary>
        /// <param name="totalValue"></param>
        /// <returns></returns>
        public abstract UInt32 ConvertToBasis(ValueType totalValue);
        /// <summary>
        /// ����������� ����� � ������ ��� �������� � ����
        /// </summary>
        public abstract Byte[] ToArray(UInt32 basis);
        /// <summary>
        /// ����������� ������ ���� � ����� 
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Basis of value</returns>
        public abstract UInt32 ConvertFromArray(Byte[] array);

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
