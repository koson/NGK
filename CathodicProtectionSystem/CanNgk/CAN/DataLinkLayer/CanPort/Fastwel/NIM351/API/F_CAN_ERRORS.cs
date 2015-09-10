using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    //Счетчики ошибок адаптера для fw_can_get_clear_errors()
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_ERRORS
    {
        // кол-во таймаутов передачи
        public UInt32 tx_timeout;
        // кол-во переполнений буфера приема
        public UInt32 data_overrun;
        // кол-во переходов в CAN_STATE_ERROR_PASSIVE
        public UInt32 error_passive;
        // кол-во переходов в CAN_STATE_BUS_OFF
        public UInt32 bus_off;
    }
    //===========================================================================================
}
