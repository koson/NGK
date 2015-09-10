using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===============================================================================================
    /* error in CAN protocol (location) / data[3] */
    [Serializable]
    enum CAN_ERR_PROT_LOC : byte
    {
        CAN_ERR_PROT_LOC_UNSPEC = 0x00, /* unspecified */
        CAN_ERR_PROT_LOC_SOF = 0x03, /* start of frame */
        CAN_ERR_PROT_LOC_ID28_21 = 0x02, /* ID bits 28 - 21 (SFF: 10 - 3) */
        CAN_ERR_PROT_LOC_ID20_18 = 0x06, /* ID bits 20 - 18 (SFF: 2 - 0 )*/
        CAN_ERR_PROT_LOC_SRTR = 0x04, /* substitute RTR (SFF: RTR) */
        CAN_ERR_PROT_LOC_IDE = 0x05, /* identifier extension */
        CAN_ERR_PROT_LOC_ID17_13 = 0x07, /* ID bits 17-13 */
        CAN_ERR_PROT_LOC_ID12_05 = 0x0F, /* ID bits 12-5 */
        CAN_ERR_PROT_LOC_ID04_00 = 0x0E, /* ID bits 4-0 */
        CAN_ERR_PROT_LOC_RTR = 0x0C, /* RTR */
        CAN_ERR_PROT_LOC_RES1 = 0x0D, /* reserved bit 1 */
        CAN_ERR_PROT_LOC_RES0 = 0x09, /* reserved bit 0 */
        CAN_ERR_PROT_LOC_DLC = 0x0B, /* data length code */
        CAN_ERR_PROT_LOC_DATA = 0x0A, /* data section */
        CAN_ERR_PROT_LOC_CRC_SEQ = 0x08, /* CRC sequence */
        CAN_ERR_PROT_LOC_CRC_DEL = 0x18, /* CRC delimiter */
        CAN_ERR_PROT_LOC_ACK = 0x19, /* ACK slot */
        CAN_ERR_PROT_LOC_ACK_DEL = 0x1B, /* ACK delimiter */
        CAN_ERR_PROT_LOC_EOF = 0x1A, /* end of frame */
        CAN_ERR_PROT_LOC_INTERM = 0x12 /* intermission */
    }
}
