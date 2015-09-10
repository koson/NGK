using System;
using System.Collections.Generic;
using System.Text;

//===========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //=======================================================================================
    /// <summary>
    /// Скорости обмена
    /// </summary>
    [Serializable]
    public enum F_CAN_BAUDRATE
    {
        /// <summary>
        /// 1 MBit/sec
        /// </summary>
        CANBR_1MBaud = 0,
        /// <summary>
        /// 800 kBit/sec
        /// </summary>
        CANBR_800kBaud = 1,
        /// <summary>
        /// 500 kBit/sec
        /// </summary>
        CANBR_500kBaud = 2,
        /// <summary>
        /// 250 kBit/sec
        /// </summary>
        CANBR_250kBaud = 3,
        /// <summary>
        /// 125 kBit/sec
        /// </summary>
        CANBR_125kBaud = 4,
        /// <summary>
        /// 100 kBit/sec
        /// </summary>
        CANBR_100kBaud = 5,
        /// <summary>
        /// 50 kBit/sec
        /// </summary>
        CANBR_50kBaud = 6,
        /// <summary>
        /// 20 kBit/sec
        /// </summary>
        CANBR_20kBaud = 7,
        /// <summary>
        /// 10 kBit/sec
        /// </summary>
        CANBR_10kBaud = 8
    }
    //=======================================================================================
}
//===========================================================================================
// End Of File