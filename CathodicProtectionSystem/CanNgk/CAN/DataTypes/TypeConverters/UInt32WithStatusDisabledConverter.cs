using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NGK.CAN.DataTypes.TypeConverters
{
    /// <summary>
    /// Конвертер типа UInt32WithStatusDisabled, 
    /// </summary>
    public class UInt32WithStatusDisabledConverter: TypeConverter  //UInt16Converter
    {
        public const string DisabledString = "Откл.";
        public const uint DisabledCode = 10 * 0xFFFF;

        public override object ConvertFrom(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, object value)
        {
            string strValue = (string)value;

            return strValue == DisabledString ? new UInt32WithStatusDisabled(DisabledCode) : 
                        new UInt32WithStatusDisabled(UInt32.Parse(strValue));
        }

        public override object ConvertTo(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            UInt32WithStatusDisabled uintValue = (UInt32WithStatusDisabled)value;

            return uintValue.Value == DisabledCode ? DisabledString : uintValue.ToString();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type t)
        {
            return t == typeof(string) || base.CanConvertTo(context, t);
        }
    }
}
