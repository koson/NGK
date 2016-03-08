using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    public interface IComplexParameterConverter: ICloneable
    {
        /// <summary>
        /// Тип данных значения параметра
        /// </summary>
        Type ValueType { get; }
        /// <summary>
        /// Принимает на вход объекты словаря и возвращает
        /// композитное значение (комплексный параметр)
        /// </summary>
        /// <param name="objectValues">Массив значений объектов
        /// словаря устройтсва</param>
        /// <returns>Значение комплексного параметра</returns>
        Object ConvertTo(object[] objectValues);
        /// <summary>
        /// Принимает занчение комплексного параметра
        /// и возваращает массив занчений объектов словаря устройтсва 
        /// </summary>
        /// <param name="complexParamValue">Значение комплексного параметра</param>
        /// <returns>Значения объектов словаря устройства</returns>
        Object[] ConvertFrom(object complexParamValue);
        /// <summary>
        /// Указывает на возможность выполнения операции ConvertTo
        /// </summary>
        /// <returns></returns>
        bool CanConvertTo();
        /// <summary>
        /// Указывает на возможность выполнения операции ConvertFrom
        /// </summary>
        /// <returns></returns>
        bool CanConvertFrom();
    }
}
