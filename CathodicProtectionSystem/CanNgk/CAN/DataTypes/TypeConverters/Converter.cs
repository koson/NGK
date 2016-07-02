using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NGK.CAN.DataTypes.TypeConverters
{
    /// <summary>
    /// Конвертер целого типа UInt16, которое имеет значение "Отключено" 
    /// (0xFFFF - означает отключено)
    /// </summary>
    public class IntegerWithStatusDisabledConverter: UInt16Converter
    {
        public const string DisabledString = "Откл.";
        public const UInt16 DisabledCode = 0xFFFF;

        public override object ConvertFrom(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, object value)
        {
            string strValue = (string)value;

            return strValue == DisabledString ? DisabledCode : UInt16.Parse(strValue);
        }

        public override object ConvertTo(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            UInt16 shortValue = (UInt16)value;

            return shortValue == DisabledCode ? DisabledString : shortValue.ToString();
        }
    }
}
