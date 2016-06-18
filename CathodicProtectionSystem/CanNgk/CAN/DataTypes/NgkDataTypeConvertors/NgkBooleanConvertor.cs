using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes.Helper;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkBooleanConverter : CanDataTypeConvertorBase
    {        
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaler">см. struct Scaler</param>
        /// <param name="precision"></param>
        public NgkBooleanConverter()
        {
            _Signed = false;
            _Scaler = ScalerTypes.x1;
        }

        #endregion

        #region Fields And Properties

        public override bool IsBoolean
        {
            get { return true; }
        }

        public override Type OutputDataType
        {
            get { return typeof(Boolean); }
        }

        public const UInt32 BasisTrue = Byte.MaxValue; // 255
        public const UInt32 BasisFalse = Byte.MinValue; // 0

        #endregion

        #region Methods
        
        public override ValueType ConvertToOutputValue(uint basis)
        {
            return basis == BasisFalse ? false : true;
        }

        public override uint ConvertToBasis(ValueType outputValue)
        {
            string msg;

            if (outputValue is Boolean)
            {
                return ((bool)outputValue) ? BasisFalse : BasisTrue;
            }

            msg = String.Format("Преобразование невозможно. Передан тип {0}, ожидается {1}",
                outputValue.GetType().ToString(), typeof(Boolean).ToString());
            throw new InvalidCastException(msg);
        }

        public override byte[] ToArray(uint basis)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override uint ConvertFromArray(byte[] array)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
