using System;
using System.Collections.Generic;
using System.Text;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort
{
    //====================================================================================
    /// <summary>
    /// Состояние CAN-порта
    /// </summary>
    public enum CanPortStatus
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        Unknown,
        /// <summary>
        /// Активный
        /// </summary>
        IsActive,
        /// <summary>
        /// Инициализация
        /// </summary>
        IsPassive,
        /// <summary>
        /// Инициализация после выполения Reset
        /// </summary>
        IsPassiveAfterReset,
        /// <summary>
        /// CAN-порт закрыт
        /// </summary>
        IsClosed
    }
    //====================================================================================
}
//========================================================================================
// End Of File