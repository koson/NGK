using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===============================================================================================
    /* error status of CAN-controller / data[1] */
    [Flags]
    [Serializable]
    public enum CAN_ERR_CRTL : byte
    {
        CAN_ERR_CRTL_UNSPEC = 0x00, /* unspecified */
        CAN_ERR_CRTL_RX_OVERFLOW = 0x01, /* RX buffer overflow */
        CAN_ERR_CRTL_TX_OVERFLOW = 0x02, /* TX buffer overflow */
        CAN_ERR_CRTL_RX_WARNING = 0x04, /* reached warning level for RX errors */
        CAN_ERR_CRTL_TX_WARNING = 0x08, /* reached warning level for TX errors */
        CAN_ERR_CRTL_RX_PASSIVE = 0x10, /* reached error passive status RX */
        CAN_ERR_CRTL_TX_PASSIVE = 0x20 /* reached error passive status TX */
        /* (at least one error counter exceeds */
        /* the protocol-defined level of 127)  */
    }
    //===============================================================================================
}
