using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    /// Коды состояний CAN-адаптера 
    /// </summary>
    [Serializable]
    public enum F_CAN_STATE : short
    {
        /// <summary>
        /// Начальное состояние адаптера; допускается конфигурирование интерфейса
        /// </summary>
        //[MarshalAs(UnmanagedType.U1)]
        CAN_STATE_INIT = 0,
        /// <summary>
        /// Кол-во ошибок приема/передачи не более 96 (состояние определено в спецификации CAN 2.0B).
        /// </summary>
        //[MarshalAs(UnmanagedType.U1)]
        CAN_STATE_ERROR_ACTIVE,
        /// <summary>
        /// Кол-во ошибок приема/передачи от 96 до 127 (CAN 2.0B).
        /// </summary>
        //[MarshalAs(UnmanagedType.U1)]
        CAN_STATE_ERROR_WARNING,
        /// <summary>
        /// кол-во ошибок приема/передачи от 128 до 255 (CAN 2.0B).
        /// </summary>
        //[MarshalAs(UnmanagedType.U1)]
        CAN_STATE_ERROR_PASSIVE,
        /// <summary>
        /// Кол-во ошибок передачи 256 (BUSOFF); адаптер отключен от сети  (CAN 2.0B).
        /// </summary>
        //[MarshalAs(UnmanagedType.U1)]
        CAN_STATE_BUS_OFF,
        /// <summary>
        /// Адаптер остановлен; промежуточное состояние, устанавливаемое при 
        /// конфигурировании, запуске, останове, перезапуске CAN-адаптера.
        /// </summary>
        //[MarshalAs(UnmanagedType.U1)]
        CAN_STATE_STOPPED,
        /// <summary>
        /// состояние сна, не используется в текущей версии
        /// </summary>
        //[MarshalAs(UnmanagedType.U1)]
        CAN_STATE_SLEEPING,
    }
    //===========================================================================================
}
