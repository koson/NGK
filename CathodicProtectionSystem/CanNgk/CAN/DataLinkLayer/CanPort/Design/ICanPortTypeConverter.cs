using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Design
{
    //====================================================================================
    /// <summary>
    /// Конвертер типа для ICanPort
    /// </summary>
    public class ICanPortTypeConverter: TypeConverter
    {
        //--------------------------------------------------------------------------------
        #region Feilds And Propeties
        //--------------------------------------------------------------------------------
        //private Boolean _Enable;
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Constructors
        //--------------------------------------------------------------------------------
        public ICanPortTypeConverter()
            : base()
        {
            //this._Enable = true;
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------------------------
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // Запрещает созадние из строки объект ICanPort
            return false;
            //return base.CanConvertFrom(context, sourceType);
        }
        //--------------------------------------------------------------------------------
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }
        //--------------------------------------------------------------------------------
        public override object ConvertTo(
            ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, 
            object value, 
            Type destinationType)
        {
            // Преобразуем объект ICanPort в строку
            if (destinationType == typeof(String))
            {
                if (value != null)
                {
                    if (value is ICanPort)
                    {
                        ICanPort port = (ICanPort)value;
                        return String.Format("{0}; {1}; {2}; {3}; {4}; {5}", port.Manufacturer, 
                            port.HardwareType, port.PortName, port.BitRate, port.Mode, port.FrameFormat);
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        //--------------------------------------------------------------------------------
        //public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        //{
        //    //return base.GetPropertiesSupported(context);
        //    return true;
        //}
        //--------------------------------------------------------------------------------
        //public override PropertyDescriptorCollection GetProperties(
        //    ITypeDescriptorContext context, object value, Attribute[] attributes)
        //{
        //    PropertyDescriptorCollection collection;
        //    PropertyDescriptorCollection cln;
        //    PropertyDescriptor[] propesDesc;

        //    String[] props = new String[] { "Manufacturer", "HardwareType", "PortName", "BitRate", 
        //        "Mode", "FrameFormat", "HardwareVersion", "SoftwareVersion", "IsOpen", "PortStatus" };
        //    collection = TypeDescriptor.GetProperties(typeof(ICanPort), attributes);

        //    propesDesc = new PropertyDescriptor[10];
        //    propesDesc[0] = collection.Find("Manufacturer", true);
        //    propesDesc[1] = collection.Find("HardwareType", true);
        //    propesDesc[2] = collection.Find("PortName", true);
        //    propesDesc[3] = collection.Find("BitRate", true);
        //    propesDesc[4] = collection.Find("Mode", true);
        //    propesDesc[0] = collection.Find("FrameFormat", true);
        //    propesDesc[1] = collection.Find("HardwareVersion", true);
        //    propesDesc[2] = collection.Find("SoftwareVersion", true);
        //    propesDesc[3] = collection.Find("IsOpen", true);
        //    propesDesc[4] = collection.Find("PortStatus", true);
        //    cln = new PropertyDescriptorCollection(propesDesc);
        //    return cln;  
        //    //return base.GetProperties(context, value, attributes);
        //}
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File