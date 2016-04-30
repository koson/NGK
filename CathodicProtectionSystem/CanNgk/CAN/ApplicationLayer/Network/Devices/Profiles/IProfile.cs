using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.Collections;
using NGK.CAN.DataTypes;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles
{
    /// <summary>
    /// Профиль устройства
    /// </summary>
    public interface ICanDeviceProfile
    {
        #region Fields And Properties
        /// <summary>
        /// Тип устройства
        /// </summary>
        DeviceType DeviceType { get; }
        /// <summary>
        /// Информация об устройстве
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Версия ПО
        /// </summary>
        Version SoftwareVersion { get; }
        /// <summary>
        /// Версия аппаратуры
        /// </summary>
        Version HardwareVersion { get; }
        /// <summary>
        /// Описание объектов словаря устройства
        /// </summary>
        ObjectInfoCollection ObjectInfoList { get; }
        /// <summary>
        /// Описание сложных праметров устройтсва 
        /// (т.е состоящих из нескольких объектов словаря)
        /// </summary>
        ComplexParameterCollection ComplexParameters { get; }

        #endregion
    }
}
