using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary
{
    /// <summary>
    /// Состояния объекта словаря
    /// </summary>
    public enum ObjectStatus: int
    {
        /// <summary>
        /// Нет ошибок
        /// </summary>
        NoError = 0,
        /// <summary>
        /// Неопределённая ошибка
        /// </summary>
        UncknownError,
        /// <summary>
        /// Ошибка конфигурации, значение в БД не соответствует
        /// значение объекта в удалённом устройстве
        /// </summary>
        ConfigurationError = 1,
        /// <summary>
        /// Ошибка при чтении из удалённого устройства
        /// </summary>
        ComunicationError = 2,
        /// <summary>
        /// Значение объекта вышло из допустимого диапазона
        /// (пока триггеры не реализованы)
        /// </summary>
        OutOfRangeError = 3
    }
}
