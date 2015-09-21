using System;
using System.Collections.Generic;
using System.Text;

//====================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters
{
    //================================================================================
    public class TypeConverterChannel4_20: System.ComponentModel.UInt32Converter
    {
        //----------------------------------------------------------------------------
        public override object ConvertFrom(
            System.ComponentModel.ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, object value)
        {
            if ((String)value == "OFF")
            {
                return 655350;
            }
            else
            {
                return UInt32.Parse((String)value);
            }
            //return base.ConvertFrom(context, culture, value);
        }
        //----------------------------------------------------------------------------
        public override object ConvertTo(
            System.ComponentModel.ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, 
            object value, 
            Type destinationType)
        {
            if ((UInt32)value == 655350)
            {
                return "OFF";
            }
            else
            {
                return value.ToString();
            }
            //return base.ConvertTo(context, culture, value, destinationType);
        }
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End Of File