using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    ///<summary>
    /// Параметры, используемые в fw_can_get_controller_config() и
    /// fw_can_set_controller_config().
    ///</summary> 
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_SETTINGS
    {
        ///<summary>
        /// Тип адаптера (только чтение)
        ///</summary>
        //[MarshalAs(UnmanagedType.I4)]
        public F_CAN_CONTROLLER controller_type;
        /// <summary>
        ///  Скорость обмена
        /// </summary>
        public F_CAN_BAUDRATE baud_rate;
        /// <summary>
        /// Флаги режима работы (см. CAN_OPMODE)
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 opmode;
        ///<summary>
        /// Фильтр допускаемых CAN-ID-ов входящих (принимаемых) сообщений.
        /// Определяется полями acceptance_code и acceptance_mask.
        /// Битовый паттерн идентификаторов сообщений, подлежащих приему
        ///</summary>
        public UInt32 acceptance_code;
        ///<summary>
        /// Маска, позволяющая указать "значимость" битовых позиций при проверке 
        /// соответствия битовому паттерну
        ///</summary> 
        public UInt32 acceptance_mask;
        ///<summary>
        /// Маска, позволяющая фильтровать некоторые ошибки, передаваемые специальными
        /// сообщениями об ошибках при работе в режиме CAN_OPMODE_ERRFRAME.
        ///</summary> 
        public UInt32 error_mask;
    }
    //===========================================================================================
}
