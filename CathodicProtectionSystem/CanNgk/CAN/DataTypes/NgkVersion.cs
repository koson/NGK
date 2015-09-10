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
    /// Версия формируется из двух частей: мажорная и минорная. Имеет следующий формат:
    /// DDD.DD , где целая часть - Мажорная версия, а дробная - Минорная версия;
    /// Мажорная часть формируется: 100 * текущий номер. Диапазон допустимых значений 100…65500;
    /// Минорная  часть формируется: 1 * текущий номер. Диапазон допустимых значений 1…99;
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
        /// <param name="scaler">см. struct Scaler</param>
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
            msg = String.Format("Преобразование невозможно. Передан тип {0}, ожидается {1}",
                totalValue.GetType().ToString(), typeof(ProductVersion).ToString());
            throw new InvalidCastException(msg);
        }

        public override byte[] ToArray(uint basis)
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
                "Размер массива не равен {1}, ожидается 2", typeof(NgkVersion), array.Length);
            throw new InvalidCastException(msg);
        }

        #endregion
    }
}
