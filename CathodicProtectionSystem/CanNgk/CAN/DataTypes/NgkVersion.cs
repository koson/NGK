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
    public sealed class NgkVersion: DataConvertor 
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
        public NgkVersion()
        {
            _Signed = false;
            _Scaler = ScalerTypes.x1;
        }
        #endregion

        #region Methods

        public override ValueType ConvertToTotalValue(uint basis)
        {
            return new ProductVersion(System.Convert.ToUInt16(basis));
        }
 
        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            if (totalValue is ProductVersion)
            {
                return Convert.ToUInt32(((ProductVersion)totalValue).TotalVersion);
            }
            msg = String.Format("�������������� ����������. ������� ��� {0}, ��������� {1}",
                totalValue.GetType().ToString(), typeof(ProductVersion).ToString());
            throw new InvalidCastException(msg);
        }

        public override byte[] ToArray(uint basis)
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
                "������ ������� �� ����� {1}, ��������� 2", typeof(NgkVersion), array.Length);
            throw new InvalidCastException(msg);
        }

        #endregion
    }
}
