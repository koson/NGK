using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes.Helper;
using NGK.CAN.DataTypes.DateTimeConvertor;

namespace NGK.CAN.DataTypes
{
    public sealed class NgkDateTimeConverter : NgkDataTypeConvertorBase
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public NgkDateTimeConverter()
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
            get { return typeof(DateTime); }
        }

        #endregion

        #region Methods

        public override ValueType ConvertToOutputValue(uint basis)
        {
            return Unix.ToDateTime(basis);
        }
        
        public override uint ConvertToBasis(ValueType totalValue)
        {
            string msg;

            if (totalValue is DateTime)
            {
                return Unix.ToUnixTime((DateTime)totalValue);
            }

            msg = String.Format("Преобразование невозможно. Передан тип {0}, ожидается {1}",
                totalValue.GetType(), typeof(DateTime));
            throw new InvalidCastException(msg);
        }
        
        public override uint ConvertFromArray(byte[] array)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        
        public override byte[] ToArray(uint basis)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
