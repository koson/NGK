using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes.Helper;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// ������ ����������� �� ���� ������: �������� � ��������. ����� ��������� ������:
    /// DDD.DD , ��� ����� ����� - �������� ������, � ������� - �������� ������;
    /// �������� ����� �����������: 100 * ������� �����. �������� ���������� �������� 100�65500;
    /// ��������  ����� �����������: 1 * ������� �����. �������� ���������� �������� 1�99;
    /// </remarks>
    [Serializable]
    public class NgkProductVersionConvertor: ICanDataTypeConvertor
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaler">��. struct Scaler</param>
        /// <param name="precision"></param>
        private NgkProductVersionConvertor() {}
        
        #endregion

        #region Fields And Properties
        
        [NonSerialized]
        private static NgkProductVersionConvertor _Instance;
        [NonSerialized]
        private static object SyncRoot = new object();

        public static NgkProductVersionConvertor Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    lock(SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new NgkProductVersionConvertor();
                    }
                }
                return _Instance;
            }
        }

        public decimal Scaler
        {
            get { return ScalerTypes.x1; }
        }

        public bool Signed
        {
            get { return false; }
        }

        public bool IsBoolean
        {
            get { return false; }
        }

        public Type OutputDataType
        {
            get { return typeof(NgkProductVersion); }
        }

        #endregion

        #region Methods

        public ValueType ConvertToOutputValue(uint basis)
        {
            return new NgkProductVersion(System.Convert.ToUInt16(basis));
        }
 
        public uint ConvertToBasis(ValueType outputValue)
        {
            String msg;

            if (outputValue is NgkProductVersion)
            {
                return Convert.ToUInt32(((NgkProductVersion)outputValue).TotalVersion);
            }
            msg = String.Format("�������������� ����������. ������� ��� {0}, ��������� {1}",
                outputValue.GetType().ToString(), typeof(NgkProductVersion).ToString());
            throw new InvalidCastException(msg);
        }

        public byte[] ToArray(uint basis)
        {
            Byte[] array = new Byte[2];

            UInt16 basis16 = Convert.ToUInt16(basis);

            unchecked
            {
                // ������� ���� �����
                array[0] = (Byte)(basis16 >> 8);
                // ������� ���� �����
                array[1] = (Byte)(basis16);
            }
            return array;
        }

        public UInt32 ConvertFromArray(Byte[] array)
        {
            String msg;
            UInt16 basis;

            if (array.Length == 2)
            {
                unchecked
                {
                    basis = array[0]; // ������� ���� �����
                    basis = (UInt16)(basis << 8);
                    basis |= (UInt16)array[1]; // ������� ���� �����
                }
                return System.Convert.ToUInt32(basis);
            }

            msg = String.Format("���������� ������������� ������ � �������� ���� {0}. " +
                "������ ������� �� ����� {1}, ��������� 2", typeof(NgkProductVersionConvertor), array.Length);
            throw new InvalidCastException(msg);
        }

        public static NgkProductVersion ConvertFromVersion(System.Version version)
        {
            return new NgkProductVersion(version);
        }

        public static uint ConvertToBasis(System.Version version)
        {
            return new NgkProductVersionConvertor().ConvertToBasis(new NgkProductVersion(version));
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
