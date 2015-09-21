using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel; // для TypeConverter
using System.Globalization; // для TypeConverter
using System.Drawing; // для TypeConverter
using System.Text;

//====================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters
{
    //================================================================================
    /// <summary>
    /// Класс реализующий пользовательский TypeConverter, для преобразования
    /// типа TYPE_NGK_DEVICE в строку и обратно
    /// </summary>
    public class TypeConverterTypeOfDeviceNGK : System.ComponentModel.EnumConverter
    {
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public override object ConvertTo(ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destType)
        {
            String str;
            switch ((TYPE_NGK_DEVICE)value)
            {
                case TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                    {
                        str = "БИ(У)-01";
                        break; 
                    }
                case TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                    {
                        str = "БИ(У)-00";
                        break; 
                    }
                case TYPE_NGK_DEVICE.UNKNOWN_DEVICE:
                    {
                        str = "Неизвестное устройство";
                        break; 
                    }
                default:
                    {
                        str = value.ToString();
                        break;
                    }
            }
            return str;
        }
        //----------------------------------------------------------------------------
        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            TYPE_NGK_DEVICE type;
            switch ((String)value)
            {
                case "БИ(У)-00":
                    {
                        type = TYPE_NGK_DEVICE.BI_MAIN_POWERED;
                        break; 
                    }
                case "БИ(У)-01":
                    {
                        type = TYPE_NGK_DEVICE.BI_BATTERY_POWER;
                        break; 
                    }
                case "Неизвестное устройство":
                    {
                        type = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                        break;
                    }
                default:
                    {
                        type = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                        break;
                    }
            }

            return type;
        }
        //----------------------------------------------------------------------------
        public TypeConverterTypeOfDeviceNGK(Type enumType): base(enumType) 
        { }
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End Of File
