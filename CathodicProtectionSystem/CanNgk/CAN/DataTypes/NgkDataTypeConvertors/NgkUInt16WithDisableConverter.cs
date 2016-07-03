using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataTypes
{
    public class NgkUInt16WithStatusDisabledConverter: NgkDataTypeConvertorBase
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaler">см. struct ScalerTypes</param>
        public NgkUInt16WithStatusDisabledConverter(decimal scaler)
        {
            _Signed = false;
            _Scaler = scaler;
        }

        #endregion

        #region Fields And Properties

        public override bool IsBoolean
        {
            get { return false; }
        }

        public override Type OutputDataType
        {
            get { return typeof(UInt32WithStatusDisabled); }
        }

        #endregion
        
        #region Methods

        public override ValueType ConvertToOutputValue(uint basis)
        {
            return new UInt32WithStatusDisabled(Convert.ToUInt32(basis * _Scaler));
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            if (totalValue is UInt32WithStatusDisabled)
            {
                return Convert.ToUInt32(((UInt32WithStatusDisabled)totalValue).Value / _Scaler);
            }

            msg = String.Format("Преобразование невозможно. Передан тип {0}, ожидается {1}",
                totalValue.GetType(), typeof(UInt32WithStatusDisabled));
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
                "Размер массива равен {1}, ожидается 2", typeof(NgkUInt16Convertor), array.Length);
            throw new InvalidCastException(msg);
        }

        #endregion
    }
}
