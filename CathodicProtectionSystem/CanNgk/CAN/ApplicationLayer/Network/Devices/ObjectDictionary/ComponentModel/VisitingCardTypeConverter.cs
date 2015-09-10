using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary.ComponentModel
{
    public class VisitingCardTypeConverter: TypeConverter
    {
        #region Fields And Properties

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // Запрещаем преобразоавние из строки в объект визитной карточки устройства.
            // Это сделано потому, что невозможно восстановить из строки визитную карату
            // устройства. Это из-за того, что визитная карта имеет ссылку на устройство
            // для которого она создана. Сериализовать ссылку в строку невозможно. Поэтому
            // и восстановить визитку в полном объёме также не возможно.
            return false;
            //return base.CanConvertFrom(context, sourceType);
        }

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

        /// <summary>
        /// Метод преобразует объект визитной карточки в строку
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            // Преобразовываем VisitingCard в строку
            if (destinationType == typeof(String))
            {
                if (value != null)
                {
                    if (value is VisitingCard)
                    {
                        return ((VisitingCard)value).ToString();
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            // Возвращаем true, если нужно отображать составные свойства объекта
            return true;
            //return base.GetPropertiesSupported(context);
        }

        public override PropertyDescriptorCollection GetProperties(
            ITypeDescriptorContext context, 
            object value, 
            Attribute[] attributes)
        {
            // Возвращаем свойства для отображения
            return TypeDescriptor.GetProperties(typeof(VisitingCard));
            //return base.GetProperties(context, value, attributes);
        }

        #endregion
    }
}
