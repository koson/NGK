using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Master.Services;

namespace NGK.CAN.OSIModel.ApplicationLayer.NetWork.Master.Design
{
    public sealed class INetworkServiceTypeConverter: TypeConverter
    {
        #region Fields And Properties
        #endregion
        #region Constructors
        #endregion
        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // Запрещает преобразование из строки в объект INetworkService, потом что
            // это невозможно
            //return base.CanConvertFrom(context, sourceType);
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                // Разрешаем преобразование объекта INetworkService в строку
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture, object value,
            Type destinationType)
        {
            // Преобразуем объект INetworkService в строку
            if (destinationType == typeof(String))
            {
                if (value != null)
                {
                    if (value is INetworkService)
                    {
                        return ((INetworkService)value).GetTypeOfService().ToString();
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }//End Of Class
}//End Of Namespace
