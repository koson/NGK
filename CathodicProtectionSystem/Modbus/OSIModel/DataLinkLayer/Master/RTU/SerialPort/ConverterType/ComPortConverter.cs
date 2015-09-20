using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort.ConverterType
{
    /// <summary>
    /// Предстваляет конвертер типа для serialport
    /// </summary>
    public class ComPortConverter: TypeConverter
    {
        //private Boolean _Enable;
        public ComPortConverter(): base()
        {
            //_Enable = true;
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            System.IO.Ports.SerialPort serialPort = ((ComPort)context.Instance).SerialPort;
            return true;
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, 
            Type sourceType)
        {
            if(sourceType.Equals(typeof(String)))
            {
                return true;
            }
            else
            {
                // ошибка
                return false;
            }
            
            //return base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, 
            Type destinationType)
        {
            if (destinationType.Equals(typeof(String)))
            {
                return true;
            }
            else
            {
                // Ошибка
                return false;
            }
            //return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, 
            object value)
        {
            String portName;
            int bautRate;
            int dataBits; 
            System.IO.Ports.Parity parity;
            System.IO.Ports.StopBits stopBits;
            System.IO.Ports.SerialPort port;

            System.IO.Ports.SerialPort serialPort = ((ComPort)context.Instance).SerialPort;

            if (value is String)
            {
                String[] properties =
                    ((String)value).Split(new String[] { ", " }, StringSplitOptions.None);
                if (properties.Length != 5)
                {
                    throw new ArgumentException();
                }
                else
                {
                    portName = properties[0];

                    try
                    {
                        bautRate = Int32.Parse(properties[1]);
                    }
                    catch
                    {
                        throw new NotSupportedException(
                            "Невозможно преобразовать парметр BautRate, значение должно быть Int32");
                    }

                    try
                    {
                        dataBits = Int32.Parse(properties[2]);
                    }
                    catch
                    {
                        throw new NotSupportedException(
                            "Невозможно преобразовать парметр DataBits, значение должно быть Int32");
                    }

                    try
                    {
                        parity =
                            (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity),
                            properties[3]);
                    }
                    catch
                    {
                        throw new NotSupportedException(
                            "Невозможно преобразовать парметр Parity, значение должно быть System.IO.Ports.Parity");
                    }

                    try
                    {
                        stopBits =
                            (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits),
                            properties[4]);
                    }
                    catch
                    {
                        throw new NotSupportedException(
                            "Невозможно преобразовать парметр Parity, значение должно быть System.IO.Ports.Parity");
                    }

                    try
                    {
                        port = new System.IO.Ports.SerialPort(portName, bautRate, parity, dataBits, stopBits);
                    }
                    catch
                    {
                        throw new InvalidOperationException(
                            "Невозможно, изменить настройки порта, возможно редактируется открытый порт");
                    }

                    return port;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(
            ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, 
            object value, 
            Type destinationType)
        {
            System.IO.Ports.SerialPort port;
            StringBuilder sb;

            if (destinationType.Equals(typeof(String)))
            {
                port = (System.IO.Ports.SerialPort)value;

                sb = new StringBuilder();
                sb.Append(port.PortName);
                sb.Append(", ");
                sb.Append(port.BaudRate.ToString());
                sb.Append(", ");
                sb.Append(port.DataBits.ToString());
                sb.Append(", ");
                sb.Append(Enum.GetName(typeof(System.IO.Ports.Parity), port.Parity));
                sb.Append(", ");
                sb.Append(Enum.GetName(typeof(System.IO.Ports.StopBits), port.StopBits));

                return sb.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override PropertyDescriptorCollection GetProperties(
            ITypeDescriptorContext context, 
            object value, 
            Attribute[] attributes)
        {
            PropertyDescriptorCollection collection;
            PropertyDescriptorCollection cln;
            PropertyDescriptor[] propesDesc;

            System.IO.Ports.SerialPort port = ((ComPort)context.Instance).SerialPort;
            
            String[] props = new String[] { "PortName", "BaudRate", "DataBits", "Parity", "StopBits" };
            collection = TypeDescriptor.GetProperties(typeof(System.IO.Ports.SerialPort), attributes);

            propesDesc = new PropertyDescriptor[5];
            propesDesc[0] = collection.Find("PortName", true);
            propesDesc[1] = collection.Find("BaudRate", true);
            propesDesc[2] = collection.Find("DataBits", true);
            propesDesc[3] = collection.Find("Parity", true);
            propesDesc[4] = collection.Find("StopBits", true);
            cln = new PropertyDescriptorCollection(propesDesc);
            return cln;            
            //return base.GetProperties(context, value, attributes);
        }
    }
}
