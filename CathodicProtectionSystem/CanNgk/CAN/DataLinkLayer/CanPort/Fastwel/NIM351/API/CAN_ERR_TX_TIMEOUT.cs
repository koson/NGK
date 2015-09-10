using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===============================================================================================
    /// <summary>
    /// TX timeout (by netdevice driver) / data[0..3] */ 
    /// </summary>
    [Flags]
    [Serializable]
    public enum CAN_ERR_TX_TIMEOUT : uint
    {
        CAN_ERR_TX_TIMEOUT_UNSPEC = 0x00000000U /* unspecified */
        /* else can_id */
    }
    //===============================================================================================
}
