using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes.Helper;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkUInt32: DataConvertor
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
        /// <param name="precision"></param>
        public NgkUInt32()
        {
            _Signed = false;
            _Scaler = ScalerTypes.x1;
        }
        
        #endregion
        
        #region Methods

        public override ValueType ConvertToTotalValue(uint basis)
        {
            return basis;
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            if (totalValue is UInt32)
            {
                return (UInt32)totalValue;
            }

            msg = String.Format("�������������� ����������. ������� ��� {0}, ��������� {1}",
                totalValue.GetType().ToString(), typeof(UInt32).ToString());
            throw new InvalidCastException(msg);

        }

        /// <summary>
        /// ���������� ����� � ���� ������� �� ���� ����, 
        /// �������������� � �������: [High]...[Low] 
        /// </summary>
        public override Byte[] ToArray(UInt32 basis)
        {
            Byte[] array = new Byte[4];
            
            unchecked
            {
                // ������� ���� �����
                array[0] = (Byte)(basis >> 32);
                array[1] = (Byte)(basis >> 16);
                array[2] = (Byte)(basis >> 8);
                // ������� ���� �����
                array[3] = (Byte)(basis);
            }
            return array;
        }

        public override UInt32 ConvertFromArray(Byte[] array)
        {
            String msg;
            UInt32 basis;

            if (array.Length == 4)
            {
                unchecked
                {
                    basis = 0;
                    basis |= (UInt32)(array[0] << 32); // ������� ���� �����
                    basis |= (UInt32)(array[1] << 16);
                    basis |= (UInt32)(array[2] << 8); 
                    basis |= (UInt16)(array[3]); // ������� ���� �����
                }
                return basis;
            }
            
            msg = String.Format("���������� ������������� ������ � �������� ���� {0}. " + 
                "������ ������� �� ����� {1}, ��������� 4", typeof(NgkUInt32), array.Length);
            throw new InvalidCastException(msg);
        }

        #endregion

    }
}
