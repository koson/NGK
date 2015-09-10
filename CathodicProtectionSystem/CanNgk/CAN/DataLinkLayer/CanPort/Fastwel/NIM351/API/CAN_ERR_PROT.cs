using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===============================================================================================
    /* error in CAN protocol (type) / data[2] */
    [Serializable]
    public enum CAN_ERR_PROT : byte
    {
        CAN_ERR_PROT_UNSPEC = 0x00, /* unspecified */
        CAN_ERR_PROT_BIT = 0x01, /* single bit error */
        CAN_ERR_PROT_FORM = 0x02, /* frame format error */
        CAN_ERR_PROT_STUFF = 0x04, /* bit stuffing error */
        CAN_ERR_PROT_BIT0 = 0x08, /* unable to send dominant bit */
        CAN_ERR_PROT_BIT1 = 0x10, /* unable to send recessive bit */
        CAN_ERR_PROT_OVERLOAD = 0x20, /* bus overload */
        CAN_ERR_PROT_ACTIVE = 0x40, /* active error announcement */
        CAN_ERR_PROT_TX = 0x80 /* error occured on transmission */
    }
}
