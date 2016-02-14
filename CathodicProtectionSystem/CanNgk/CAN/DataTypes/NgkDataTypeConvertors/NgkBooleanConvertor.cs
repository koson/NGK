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

        #endregion

        #region Methods
        
        public override ValueType ConvertToOutputValue(uint basis)
        {
            return basis == 0 ? false : true;
        }

        public override uint ConvertToBasis(ValueType outputValue)
        {
            string msg;

            if (outputValue is Boolean)
            {
                return ((bool)outputValue) ? (UInt32)0 : (UInt32)255;
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
