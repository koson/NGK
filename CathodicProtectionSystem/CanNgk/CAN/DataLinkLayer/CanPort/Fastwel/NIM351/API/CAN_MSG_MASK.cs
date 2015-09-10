using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    /// <summary>
    /// Битовые маски для значащих бит в кадрах стандартного
    /// и расширенного форматов
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct CAN_MSG_MASK
    {
        /// <summary>
        /// standard frame format (SFF)
        /// </summary>
        public const UInt32 CAN_SFF_MASK = 0x000007FFU;
        /// <summary>
        /// extended frame format (EFF)
        /// </summary>
        public const UInt32 CAN_EFF_MASK = 0x1FFFFFFFU;
    }
}
