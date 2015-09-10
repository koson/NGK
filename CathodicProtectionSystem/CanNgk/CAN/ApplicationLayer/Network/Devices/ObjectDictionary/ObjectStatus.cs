using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("Норма")]
        NoError = 0,
        /// <summary>
        /// Неопределённая ошибка
        /// </summary>
        [Description("Неиз. ошибка")]
        UncknownError,
        /// <summary>
        /// Ошибка конфигурации, значение в БД не соответствует
        /// значение объекта в удалённом устройстве
        /// </summary>
        [Description("Конф. ошибка")]
        ConfigurationError = 1,
        /// <summary>
        /// Ошибка при чтении из удалённого устройства
        /// </summary>
        [Description("Ошибка соед.")]
        ComunicationError = 2,
        /// <summary>
        /// Значение объекта вышло из допустимого диапазона
        /// (пока триггеры не реализованы)
        /// </summary>
        [Description("Недоп. значение")]
        OutOfRangeError = 3
    }
}
