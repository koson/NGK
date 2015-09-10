using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Common.ComponentModel;

namespace NGK.CAN.ApplicationLayer.Network.Devices
{
    /// <summary>
    /// Определяет возможные состояния сетевого устройства
    /// </summary>
    /// <remarks>Внимание!!! Числовое значение константы необъодимо для механизма
    /// сортировки по статусу устройства. См. Соответствующие классы
    /// реализующие IComparer для устройства CAN НГК-ЭХЗ</remarks>
    [TypeConverter(typeof(EnumTypeConverter))]
    public enum DeviceStatus: byte
    {
        /// <summary>
        /// При старте системы все устройства находятся в этом состоянии.
        /// </summary>
        //Disconnected,
        /// <summary>
        /// Ошибка соединения с устройством
        /// </summary>
        [Description("Ошибка соед.")] // Необходим для EnumTypeConverter
        CommunicationError = 1,
        /// <summary>
        /// Ошибка кофигурации устройства. Один или более конфигурационных параметров
        /// объектного словаря не соотвествует прочитанному значению из реального устройства
        /// </summary>
        /// <remarks>
        /// Если объект словаря объектов имеет модификатор доступа ReadOnly = true, значит это
        /// конфигурационная константа, которая задаётся при построении сети.
        /// </remarks>
        [Description("Ошибка конфиг.")]
        ConfigurationError = 2,
        /// <summary>
        /// Устройство остановлено
        /// </summary>
        [Description("Остановлено")]
        Stopped = 4,
        /// <summary>
        /// Устройство готово к работе
        /// </summary>
        [Description("Готово")]
        Preoperational = 5,
        /// <summary>
        /// Устройство работает в нормальном режиме
        /// </summary>
        [Description("Работает")]
        Operational = 0x7F
    }
    //====================================================================================
}
//========================================================================================
//End of file