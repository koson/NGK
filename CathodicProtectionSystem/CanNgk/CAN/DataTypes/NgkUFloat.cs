using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkUFloat: DataConvertor
    {
        #region Fields And Properties

        public override bool IsBoolean
        {
            get { return false; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaler">��. struct Scaler</param>
        public NgkUFloat(decimal scaler)
        {
            _Signed = false;
            _Scaler = scaler;
        }
        
        #endregion
        
        #region Methods

        public override ValueType ConvertToTotalValue(uint basis)
        {
            UInt16 basis16 = System.Convert.ToUInt16(basis);

            return Convert.ToSingle(basis * _Scaler);
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            if (totalValue is Single)
            {
                return Convert.ToUInt32(
                    ((Single)totalValue) / Convert.ToSingle(_Scaler));
            }

            msg = String.Format("�������������� ����������. ������� ��� {0}, ��������� {1}",
                totalValue.GetType(), typeof(Single));
            throw new InvalidCastException(msg);

        }

        /// <summary>
        /// ���������� ����� � ���� ������� �� ���� ����, 
        /// �������������� � �������������� ����: [High][Low] 
        /// </summary>
        public override Byte[] ToArray(UInt32 basis)
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

        public override UInt32 ConvertFromArray(Byte[] array)
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
                "������ ������� ����� {1}, ��������� 2", typeof(NgkUInt16), array.Length);
            throw new InvalidCastException(msg);
        }

        #endregion

    }
}
