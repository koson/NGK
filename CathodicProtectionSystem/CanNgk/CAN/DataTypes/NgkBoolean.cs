using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes.Helper;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkBoolean: DataConvertor
    {        
        #region Fields And Properties

        public override bool IsBoolean
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaler">см. struct Scaler</param>
        /// <param name="precision"></param>
        public NgkBoolean()
        {
            _Signed = false;
            _Scaler = ScalerTypes.x1;
        }
        #endregion

        #region Methods
        
        public override ValueType ConvertToTotalValue(uint basis)
        {
            return basis == 0 ? false : true;
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            string msg;

            if (totalValue is Boolean)
            {
                return ((bool)totalValue) ? (UInt32)0 : (UInt32)255;
            }

            msg = String.Format("Преобразование невозможно. Передан тип {0}, ожидается {1}",
                totalValue.GetType().ToString(), typeof(Boolean).ToString());
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
