using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Common.ComponentModel;

namespace NGK.CAN.ApplicationLayer.Network.Devices
{
    /// <summary>
    /// Типы slave-устройств работающие в сети CAN НГК-ЭХЗ
    /// </summary>
    [TypeConverter(typeof(EnumTypeConverter))]
    public enum DeviceType: ushort
    {
        /// <summary>
        /// Неопределённый тип устройства
        /// </summary>
        [Description("Неизвестное устройство")]
        UnknownTypeOfDevice = 0,
        // КИП (Проводное питание БИ(У)-00) - эксперементальная 
        // соответствует объектному словарю (протокола редакция 1.0)
        [Description("КИП Осн.пит. v1")]
        KIP_MAIN_POWERED_v1 = 9810,
        // КИП (Автономноге питание БИ(У)-01) - эксперементальная 
        // соответствует объектному словарю (протокола редакция 1.0)
        [Description("КИП Авт.пит. v1")]
        KIP_BATTERY_POWER_v1 = 9811
    }
}
