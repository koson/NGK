using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Master;

namespace NGK.CAN.Design
{
    /// <summary>
    /// Преобразует объект с интерфейсом INetworkController в строку. Обратного
    /// преобразования нет
    /// </summary>
    public sealed class INetworkControllerTypeConverter: TypeConverter
    {
        #region Fields And Properties
        #endregion

        #region Constructors
        #endregion

        #region Methods

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // Запрещает преобразование из строки в объект INetworkController, потом что
            // это невозможно
            return false;
            //return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                // Разрешаем преобразование объекта INetworkController в строку
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, object value, 
            Type destinationType)
        {
            // Преобразуем объект INetworkController в строку
            if (destinationType == typeof(String))
            {
                if (value != null)
                {
                    if (value is INetworkController)
                    {
                        return ((INetworkController)value).NetworkName;
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion
    }
}
