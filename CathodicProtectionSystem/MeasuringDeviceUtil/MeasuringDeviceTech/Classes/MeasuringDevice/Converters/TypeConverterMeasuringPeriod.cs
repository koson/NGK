using System;
using System.Collections.Generic;
using System.Text;
using NGK.MeasuringDeviceTech.Classes.MeasuringDevice;

//====================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters
{
    //================================================================================
    public class TypeConverterMeasuringPeriod: System.ComponentModel.UInt32Converter
    {
        //----------------------------------------------------------------------------
        public override object ConvertFrom(
            System.ComponentModel.ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, 
            object value)
        {
            UInt32 var;

            if (context.Instance is MeasuringDeviceMainPower)
            {
                switch ((String)value)
                {
                    case "Ответ по запросу":
                        {
                            var = 0xFFFFFFFF;
                            break;
                        }
                    case "Измерять постоянно":
                        {
                            var = 0;
                            break;
                        }
                    default:
                        {
                            var = UInt32.Parse((String)value);
                            break;
                        }
                }
            }
            else if (context.Instance is MeasuringDeviceBatteryPower)
            {
                switch ((String)value)
                {
                    case "Ответ по запросу":
                        {
                            var = 0xFFFFFFFF;
                            break;
                        }
                    //case "Измерять постоянно":
                    //    {
                    //        var = 0;
                    //        break;
                    //    }
                    default:
                        {
                            var = UInt32.Parse((String)value);
                            break;
                        }
                }
            }
            else
            {
                throw new Exception("Невозможно конверитировать свойсвто, неизвестный объект");
            }
            return var;
            //return base.ConvertFrom(context, culture, value);
        }
        //----------------------------------------------------------------------------
        public override object ConvertTo(
            System.ComponentModel.ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, 
            object value, 
            Type destinationType)
        {
            String str;

            if (context.Instance is MeasuringDeviceBatteryPower)
            { 
                switch ((UInt32)value)
                {
                    case 0xFFFFFFFF:
                        {
                            str = "Ответ по запросу";
                            break;
                        }
                    //case 0:
                    //    {
                    //        str = "Измерять постоянно";
                    //        break;
                    //    }
                    default:
                        {
                            str = value.ToString();
                            break;
                        }
                }
            }
            else if (context.Instance is MeasuringDeviceMainPower)
            {
                switch ((UInt32)value)
                {
                    case 0xFFFFFFFF:
                        {
                            str = "Ответ по запросу";
                            break;
                        }
                    case 0:
                        {
                            str = "Измерять постоянно";
                            break;
                        }
                    default:
                        {
                            str = value.ToString();
                            break;
                        }
                }
            }
            else
            {
                throw new Exception("Невозможно конверитировать свойсвто, неизвестный объект");
            }
            return str;
            //return base.ConvertTo(context, culture, value, destinationType);
        }
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End Of File