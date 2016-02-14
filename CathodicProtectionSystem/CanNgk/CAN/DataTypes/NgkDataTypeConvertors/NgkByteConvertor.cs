using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes.Helper;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkByteConverter : CanDataTypeConvertorBase
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public NgkByteConverter()
        {
            _Signed = false;
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
            get { return typeof(Byte); }
        }

        #endregion
        
        #region Methods

        public override ValueType ConvertToOutputValue(uint basis)
        {
            return Convert.ToByte(basis);
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            if (totalValue is Byte)
            {
                return Convert.ToUInt32(totalValue);
            }

            msg = String.Format("ѕреобразование невозможно. ѕередан тип {0}, ожидаетс€ {1}",
                totalValue.GetType(), typeof(Byte));
            throw new InvalidCastException(msg);

        }

        /// <summary>
        /// ¬озвращает базис в виде массива из двух байт, 
        /// представленном в дополнительном коде: [High][Low] 
        /// </summary>
        public override Byte[] ToArray(UInt32 basis)
        {
            Byte[] array = new Byte[] { Convert.ToByte(basis) };

            return array;            
        }

        public override UInt32 ConvertFromArray(Byte[] array)
        {
            String msg;

            if (array.Length == 1)
            {
                return System.Convert.ToUInt32(array[0]);
            }
            
            msg = String.Format("Ќевозможно преобразовать массив в значение типа {0}. " + 
                "–азмер массива равен {1}, ожидаетс€ 2", typeof(NgkByteConverter), array.Length);
            throw new InvalidCastException(msg);
        }

        #endregion
    }
}
