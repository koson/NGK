using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.LogManager;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkFloatConverter : CanDataTypeConvertorBase
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaler">��. struct Scaler</param>
        public NgkFloatConverter(decimal scaler)
        {
            _Signed = true;
            _Scaler = scaler;

            MaxTotalValue = Convert.ToSingle(Int16.MaxValue * _Scaler);
            MinTotalValue = Convert.ToSingle(Int16.MinValue * _Scaler);
        }

        #endregion

        #region Fields And Properties

        public override bool IsBoolean
        {
            get { return false; }
        }

        public override Type OutputDataType
        {
            get { return typeof(Single); }
        }

        public readonly Single MaxTotalValue;

        public readonly Single MinTotalValue;

        #endregion
        
        #region Methods

        public override ValueType ConvertToOutputValue(uint basis)
        {
            string msg;
            try
            {
                if (basis <= UInt16.MaxValue)
                {
                    Int16 value;
                    
                    unchecked
                    {
                        value = (Int16)basis;
                    }

                    return Convert.ToSingle(value * _Scaler);
                }
                else
                {
                    msg = String.Format(
                        "�������������� ����������. �������� ����� ������������ �������� {0}",
                        basis);
                    throw new ArgumentOutOfRangeException("basis", msg);
                }
            }
            catch (Exception ex)
            {
                msg = String.Format(
                    "Exception in NgkFloatConverter: message - {0}; stack - {1}; Basis - {2}; Scaler - {3}; Signed - {4}",
                    ex.Message, ex.StackTrace, basis, _Scaler, Signed);
                NLogManager.Instance.Error(msg);
                return 0;
            }
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            try
            {
                if (totalValue is Single)
                {
                    Single single = (Single)totalValue;

                    if (single >= MinTotalValue && single <= MaxTotalValue)
                    {
                        unchecked
                        {
                            return (UInt32)((Single)totalValue / Convert.ToSingle(_Scaler));
                        }
                    }
                    else
                    {
                        msg = String.Format(
                            "�������������� ����������. �������� ����� ������������ �������� {0}",
                            totalValue);
                        throw new ArgumentOutOfRangeException("totalValue", msg);
                    }
                }
                else
                {
                    msg = String.Format("�������������� ����������. ������� ��� {0}, ��������� {1}",
                        totalValue.GetType(), typeof(Single));
                    throw new InvalidCastException(msg);
                }
            }
            catch (Exception ex)
            {
                msg = String.Format(
                    "Exception in NgkFloatConverter: message - {0}; stack - {1}; TotalValue - {2}; Scaler - {3}; Signed - {4}", 
                    ex.Message, ex.StackTrace, totalValue, _Scaler, Signed);
                NLogManager.Instance.Error(msg);
                return 0;
            }
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
                "������ ������� ����� {1}, ��������� 2", typeof(NgkUInt16Convertor), array.Length);
            throw new InvalidCastException(msg);
        }

        #endregion

    }
}
