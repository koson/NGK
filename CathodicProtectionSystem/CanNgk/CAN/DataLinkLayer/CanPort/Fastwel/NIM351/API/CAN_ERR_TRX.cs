using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===============================================================================================
    /* error status of CAN-transceiver / data[4] */
    /*                                             CANH CANL */
    [Serializable]
    public enum CAN_ERR_TRX : byte
    {
        CAN_ERR_TRX_UNSPEC = 0x00, /* 0000 0000 */
        CAN_ERR_TRX_CANH_NO_WIRE = 0x04, /* 0000 0100 */
        CAN_ERR_TRX_CANH_SHORT_TO_BAT = 0x05, /* 0000 0101 */
        CAN_ERR_TRX_CANH_SHORT_TO_VCC = 0x06, /* 0000 0110 */
        CAN_ERR_TRX_CANH_SHORT_TO_GND = 0x07, /* 0000 0111 */
        CAN_ERR_TRX_CANL_NO_WIRE = 0x40, /* 0100 0000 */
        CAN_ERR_TRX_CANL_SHORT_TO_BAT = 0x50, /* 0101 0000 */
        CAN_ERR_TRX_CANL_SHORT_TO_VCC = 0x60, /* 0110 0000 */
        CAN_ERR_TRX_CANL_SHORT_TO_GND = 0x70, /* 0111 0000 */
        CAN_ERR_TRX_CANL_SHORT_TO_CANH = 0x80, /* 1000 0000 */
    }
    //===============================================================================================
}
