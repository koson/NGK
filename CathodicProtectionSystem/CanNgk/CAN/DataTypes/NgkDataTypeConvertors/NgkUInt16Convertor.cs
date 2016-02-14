using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// Реализует знаковый целочисленный двухбайтный тип данных протокола CAN НГК-ЭХЗ
    /// Предстваляет собой целое число UInt32 = основа числа (Basis) * множитель (Scaler) 
    /// </summary>
    [Serializable]
    public sealed class NgkUInt16Convertor : CanDataTypeConvertorBase
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaler">см. struct Scaler</param>
        public NgkUInt16Convertor(decimal scaler)
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
            get { return typeof(UInt32); }
        }

        #endregion
        
        #region Methods

        public override ValueType ConvertToOutputValue(uint basis)
        {
            return Convert.ToUInt32(basis * _Scaler);
        }

        public override uint ConvertToBasis(ValueType totalValue)
        {
            String msg;

            if (totalValue is UInt32)
            {
                return Convert.ToUInt32(((UInt32)totalValue) / _Scaler);
            }
            else if (totalValue is UInt16)
            {
                return Convert.ToUInt32(((UInt16)totalValue) /_Scaler);
            }
            else if (totalValue is Byte)
            {
                return Convert.ToUInt32(((Byte)totalValue) /_Scaler);
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
                "Размер массива равен {1}, ожидается 2", typeof(NgkUInt16Convertor), array.Length);
            throw new InvalidCastException(msg);
        }

        #endregion

        //#region Members of IXmlSerializable

        //public override System.Xml.Schema.XmlSchema GetSchema()
        //{
        //    return null;
        //}

        //public override void ReadXml(System.Xml.XmlReader reader)
        //{
        //    //String str, msg;
        //    //String[] members;
        //    //str = reader.ReadString().ToLower();
        //    //members = str.Split(new char[] { ':' });

        //    //if (members.Length == 2)
        //    //{
        //    //    if (false == Boolean.TryParse(members[0], out _Signed))
        //    //    {
        //    //        msg = String.Format(
        //    //            "Невозможно выполенить десириализацию типа X." + 
        //    //            "Не удалось преобразовать строку {0} в UInt16", members[0]);
        //    //        throw new SerializationException(msg);
        //    //    }
        //    //    try
        //    //    {
        //    //        this._Scaler = (Precision)Enum.Parse(typeof(Precision), members[1]);
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        msg = String.Format(
        //    //            "Невозможно выполенить десириализацию типа NgkUInt16. " +
        //    //            "Не удалось преобразовать строку {0} в enum Precision", members[1]);
        //    //        throw new SerializationException(msg, ex);
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    msg = String.Format(
        //    //        "Невозможно выполенить десириализацию типа NgkUInt16. " +
        //    //        "Ожидается три строки содержащих Signed, Basis и Scaler, " +
        //    //        "а получено при разборе строк - {0}",
        //    //        members.Length);
        //    //    throw new SerializationException(msg);
        //    //}
        //    //return;
        //}

        //public override void WriteXml(System.Xml.XmlWriter writer)
        //{
        //    //String str = String.Format("{0}:{1}",
        //    //    _Signed, _Scaler);
        //    //writer.WriteString(str);
        //}

        //#endregion
    }
}
