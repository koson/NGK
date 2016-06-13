using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes.Helper;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkInt16Converter : CanDataTypeConvertorBase
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public NgkInt16Converter()
        {
            _Signed = true;
            _Scaler = ScalerTypes.x1;
        }

        #endregion

        #region Fields And Properties

        public override bool IsBoolean
        {
            get { return false; }
        }

        public override Type OutputDataType
        {
            get { return typeof(Int16); }
        }

        #endregion
        
        #region Methods

        public override ValueType ConvertToOutputValue(uint basis)
        {
            return System.Convert.ToInt16(basis);
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            if (totalValue is Int16)
            {
                Int32 value = Convert.ToInt32(totalValue); // Учитываем знак
                UInt32 result = (UInt32)value;
                return result;
            }

            msg = String.Format("Преобразование невозможно. Передан тип {0}, ожидается {1}",
                totalValue.GetType(), typeof(UInt32));
            throw new InvalidCastException(msg);

        }

        /// <summary>
        /// Возвращает базис в виде массива из двух байт, 
        /// представленном в дополнительном коде: [High][Low] 
        /// </summary>
        public override Byte[] ToArray(UInt32 basis)
        {
            Byte[] array = new Byte[2];

            UInt16 basis16 = Convert.ToUInt16(basis);
            
            unchecked
            {
                // Старший байт числа
                array[0] = (Byte)(basis16 >> 8);
                // Младший байт числа
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
                    basis = array[0]; // старший байт числа
                    basis = (UInt16)(basis << 8);
                    basis |= (UInt16)array[1]; // младший байт числа
                }
                return System.Convert.ToUInt32(basis);
            }
            
            msg = String.Format("Невозможно преобразовать массив в значение типа {0}. " + 
                "Размер массива равен {1}, ожидается 2", typeof(NgkInt16Converter), array.Length);
            throw new InvalidCastException(msg);
        }
        #endregion
    }
}
