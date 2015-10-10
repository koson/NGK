using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices
{
    /// <summary>
    /// Флаги ошибок для сервиса EMCY
    /// Все устройтсва должны реализовать данный интерфейс
    /// </summary>
    public interface IEmcyErrors
    {
        /// <summary>
        /// Есть Вскрытие 
        /// </summary>
        bool Tamper { get; set; }
        /// <summary>
        /// Oшибка внешнего (основного) питания
        /// </summary>
        bool MainSupplyPowerError { get; set; }
        /// <summary>
        /// Неисправность внутренней батареи питания
        /// </summary>
        bool BatteryError { get; set; }
        /// <summary>
        /// Ошибка регистрации БИ(У)-01
        /// </summary>
        bool RegistrationError { get; set; }
        /// <summary>
        /// Ошибка дублирования адреса БИ(У)-01.
        /// </summary>
        bool DuplicateAddressError { get; set; }
        /// <summary>
        /// Подключение сервисного разъёма.
        /// </summary>
        bool ConnectedServiceConnector { get; set; }
    }
}
