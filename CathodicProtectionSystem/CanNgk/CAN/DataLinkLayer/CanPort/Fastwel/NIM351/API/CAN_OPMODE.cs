using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    //Параметры адаптера
    ///<sumary>
    /// Флаги режима работы:
    ///</sumary> 
    [Flags]
    [Serializable]
    public enum CAN_OPMODE : ushort
    {
        /// <summary>
        /// Все флаги сброшены. Добавлено мною для удобства (отсутствует в документации на NIM351).
        /// </summary>
        CAN_OPMODE_INIT = 0x0000,
        ///<sumary>
        /// Принимаютя сообщения стандартного формата (11-битовые CAN-ID)
        ///</sumary> 
        CAN_OPMODE_STANDARD = 0x0001,
        ///<summary>
        ///  принимаются сообщения расширенного формата (29-битовые CAN-ID)
        /// </summary>
        CAN_OPMODE_EXTENDED = 0x0002,
        ///<summary>
        /// Индикация ошибок посредством специального CAN-сообщения
        ///</summary>
        CAN_OPMODE_ERRFRAME = 0x0004,
        ///<summary>
        /// Функционирование в режиме "Listen Only" (мониторинг шины), при котором
        /// адаптер не передает никаких подтверждений, даже если сообщение принято успешно.
        ///</summary>
        CAN_OPMODE_LSTNONLY = 0x0008,
        ///<summary>
        /// Адаптер будет выполнять передачу, даже при отсутствии подтверждения на шине
        ///</summary>
        CAN_OPMODE_SELFTEST = 0x0010,
        ///<summary>
        /// Адаптер принимает собственные передаваемые сообщения (при условии, что они
        /// удовлетворяют заданному acceptance-фильтру)
        ///</summary>
        CAN_OPMODE_SELFRECV = 0x0020
    }
    //===========================================================================================
}
