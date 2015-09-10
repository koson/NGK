using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===============================================================================================
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// arbitration lost in bit ... / data[0] */
    /// </remarks>
    [Flags]
    [Serializable]
    public enum CAN_ERR_LOSTARB : byte
    {
        CAN_ERR_LOSTARB_UNSPEC = 0x00 /* unspecified */
        /* else bit number in bitstream */
    }
    //===============================================================================================
}
