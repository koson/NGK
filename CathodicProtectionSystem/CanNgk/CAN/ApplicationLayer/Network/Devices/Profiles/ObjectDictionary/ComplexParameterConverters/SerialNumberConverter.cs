using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.ComplexParameterConverters
{
    public class SerialNumberConverter : IComplexParameterConverter
    {
        #region IComplexParameterConverter Members

        public Type ValueType
        {
            get { return typeof(UInt64); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectValues">
        /// старший[0x2003], средний[0x2004], млаший[0x2005]</param>
        /// <returns></returns>
        public object ConvertTo(params object[] objectValues)
        {
            UInt64 serialNumber;

            if (objectValues.Length != 3)
            {
                throw new ArgumentException("Неверная длина массива", "objectValues");
            }

            serialNumber = (UInt64)(System.Convert.ToUInt64(objectValues[0]) << 32); //0x2003
            serialNumber |= (UInt64)(System.Convert.ToUInt64(objectValues[1]) << 16); //0x2004
            serialNumber |= System.Convert.ToUInt64(objectValues[2]); //(0x2005))
            return serialNumber;
        }

        public object[] ConvertFrom(object complexParamValue)
        {
            String msg;
            UInt64 max = 281474976710656; // 2^48
            UInt64 value = System.Convert.ToUInt64(complexParamValue);
            object[] result;

            if (value >= max)
            {
                msg = String.Format(
                    "Попытка установить значение больше максимально допустимого {0}", max);
                throw new ArgumentOutOfRangeException(msg);
            }

            result = new object[3];
            unchecked
            {
                result[0] = (UInt16)(value >> 32); //0x2003
                result[1] = (UInt16)(value >> 16); //0x2004
                result[2] = (UInt16)(value); //0x2005
            }
            return result;
        }

        public bool CanConvertTo()
        {
            return true;
        }

        public bool CanConvertFrom()
        {
            return true;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new SerialNumberConverter();
        }

        #endregion
    }
}
