using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    ///<summary>
    /// Таймауты адаптера по приему и передаче, используемые функциями:
    /// fw_can_get_timeouts()
    /// fw_can_set_timeouts()
    /// fw_can_recv()
    /// fw_can_send()
    /// Значение таймаута передачи функцией fw_can_send() определяется формулой:
    /// Tsend = N * WriteTotalTimeoutMultiplier + WriteTotalTimeoutConstant (мс),
    /// где N -- кол-во сообщений, определяемое параметром nTx.
    /// Если WriteTotalTimeoutMultiplier и WriteTotalTimeoutConstant равны 0,
    /// ожидание fw_can_send() будет бесконечным.
    /// Если при вызове fw_can_recv() в буфере приема имеется хотя бы одно сообщение,
    /// fw_can_recv() прочитает их и немедленно завершится.
    /// Если при вызове fw_can_recv() в буфере приема нет ни одного сообщения, то
    /// при приеме первого же сообщения fw_can_recv() прочитает его и немедленно
    /// завершится.
    /// Если при вызове fw_can_recv() в буфере приема нет ни одного сообщения, и ранее
    /// был задан отличный от нуля ReadTotalTimeout, то при отсутствии принятых сообщений
    /// в течение ReadTotalTimeout (мс) функция fw_can_recv() заврешится таймаутом.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_TIMEOUTS
    {
        /// <summary>
        ///  Множитель для вычисления таймаута передачи
        /// </summary>
        public UInt32 WriteTotalTimeoutMultiplier;
        /// <summary>
        /// Константа для вычисления таймаута передачи
        /// </summary>
        public UInt32 WriteTotalTimeoutConstant;
        /// <summary>
        ///  Таймаут приема (в мс) функцией fw_can_recv()
        /// </summary>
        public UInt32 ReadTotalTimeout;
        /// <summary>
        ///  Таймаут автоматического восстановления из состояния bus-off (CAN_STATE_BUS_OFF)
        /// </summary>
        public UInt32 RestartBusoffTimeout;
    }
    //===========================================================================================
}
