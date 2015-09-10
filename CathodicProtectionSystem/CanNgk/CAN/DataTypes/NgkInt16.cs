using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes.Helper;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkInt16: DataConvertor
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
        public NgkInt16()
        {
            _Signed = true;
            _Scaler = ScalerTypes.x1;
        }
        
        #endregion
        
        #region Methods

        public override ValueType ConvertToTotalValue(uint basis)
        {
            return System.Convert.ToInt16(basis);
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            if (totalValue is Int16)
            {
                return (UInt32)totalValue;
            }

            msg = String.Format("�������������� ����������. ������� ��� {0}, ��������� {1}",
                totalValue.GetType(), typeof(UInt32));
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
                "������ ������� ����� {1}, ��������� 2", typeof(NgkInt16), array.Length);
            throw new InvalidCastException(msg);
        }
        #endregion
    }
}
