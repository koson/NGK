using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel; // для TypeConverter
using System.Globalization; // для TypeConverter
using System.Drawing; // для TypeConverter
using System.Text;

//====================================================================================
namespace Modbus.OSIModel.DataLinkLayer.TypeConverters
{
    //================================================================================
    /// <summary>
    /// Класс реализующий пользовательский TypeConverter, для преобразования
    /// типа boolean в виде ON и OFF
    /// Пример взят из http://www.rsdn.ru/article/dotnet/PropertyGridFAQ.xml#E6F
    /// </summary>
    public class BooleanTypeConverter: BooleanConverter
    {
        //----------------------------------------------------------------------------
        public override object ConvertTo(ITypeDescriptorContext context,
          CultureInfo culture,
          object value,
          Type destType)
        {
            return (bool)value ?
              "ON" : "OFF";
        }
        //----------------------------------------------------------------------------
        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture,
          object value)
        {
            return (string)value == "ON";
        }
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End Of File