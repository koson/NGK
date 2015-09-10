using System;
using System.Collections.Generic;
using System.Text;

//===========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //=======================================================================================
    /// <summary>
    /// Специальные сообщения об ошибках в режиме CAN_OPMODE_ERRFRAME
    /// </summary>
    [Serializable]
    public struct CAN_OPMODE_ERRFRAME
    {
        // Индикатор кадра сообщения об ошибке
        public const UInt32 CAN_ERR_FLAG = 0x20000000U;
        /// <summary>
        /// Длина поля данных сообщения об ошибке
        /// </summary>
        public const UInt32 CAN_ERR_DLC = 8; // dlc for error frames

        // Классы индицируемых ошибок

        ///<summary>
        /// Неизвестная ошибка :)
        ///</summary> 
        public const UInt32 CAN_ERR_UNSPEC = 0x00000000U; /* unspecified */
        /// <summary>
        /// Таймаут передачи
        /// </summary>
        public const UInt32 CAN_ERR_TX_TIMEOUT = 0x00000001U; /* TX timeout / data[0..3] */
        /// <summary>
        ///  Проигрыш арбитража (data[0]) 
        /// </summary>
        public const UInt32 CAN_ERR_LOSTARB = 0x00000002U; /* lost arbitration    / data[0]    */
        /// <summary>
        /// Внутренний сбой адаптера (data[1])
        /// </summary>
        public const UInt32 CAN_ERR_CRTL = 0x00000004U; /* controller problems / data[1]    */
        /// <summary>
        /// Нарушение протокола (data[2..3])
        /// ВНИМАНИЕ! Может возникать очень много и часто!
        /// </summary>
        public const UInt32 CAN_ERR_PROT = 0x00000008U; /* protocol violations (may flood!) / data[2..3] */
        /// <summary>
        /// Статус трансивера (data[4]) ???
        /// </summary>
        public const UInt32 CAN_ERR_TRX = 0x00000010U; /* transceiver status  / data[4]    */
        /// <summary>
        /// Не получен ACK на переданное сообщение
        /// </summary>
        public const UInt32 CAN_ERR_ACK = 0x00000020U; /* received no ACK on transmission */
        /// <summary>
        /// Bus Off
        /// </summary>
        public const UInt32 CAN_ERR_BUSOFF = 0x00000040U; /* bus off */
        /// <summary>
        /// Ошибка на шине 
        /// ВНИМАНИЕ! Может возникать очень много и часто!
        /// </summary>
        public const UInt32 CAN_ERR_BUSERROR = 0x00000080U; /* bus error (may flood!) */
        /// <summary>
        ///  Адаптер перезапущен
        /// </summary>
        public const UInt32 CAN_ERR_RESTARTED = 0x00000100U; /* controller restarted */

        /// <summary>
        /// TX timeout (by netdevice driver) / data[0..3] 
        /// </summary>
        public const UInt32 CAN_ERR_TX_TIMEOUT_UNSPEC = 0x00000000U; /* unspecified */
        /* else can_id */

        /// <summary>
        /// arbitration lost in bit ... / data[0] */
        /// </summary>
        public const Byte CAN_ERR_LOSTARB_UNSPEC = 0x00; /* unspecified */
        /* else bit number in bitstream */

        /* error status of CAN-controller / data[1] */
        /// <summary>
        /// unspecified
        /// </summary>
        public const Byte CAN_ERR_CRTL_UNSPEC = 0x00; /* unspecified */
        /// <summary>
        /// RX buffer overflow
        /// </summary>
        public const Byte CAN_ERR_CRTL_RX_OVERFLOW = 0x01; /* RX buffer overflow */
        /// <summary>
        /// TX buffer overflow
        /// </summary>
        public const Byte CAN_ERR_CRTL_TX_OVERFLOW = 0x02; /* TX buffer overflow */
        /// <summary>
        /// reached warning level for RX errors
        /// </summary>
        public const Byte CAN_ERR_CRTL_RX_WARNING = 0x04; /* reached warning level for RX errors */
        /// <summary>
        /// reached warning level for TX errors
        /// </summary>
        public const Byte CAN_ERR_CRTL_TX_WARNING = 0x08; /* reached warning level for TX errors */
        /// <summary>
        /// reached error passive status RX
        /// </summary>
        public const Byte CAN_ERR_CRTL_RX_PASSIVE = 0x10; /* reached error passive status RX */
        /// <summary>
        /// reached error passive status TX
        /// </summary>
        public const Byte CAN_ERR_CRTL_TX_PASSIVE = 0x20; /* reached error passive status TX */
        /* (at least one error counter exceeds */
        /* the protocol-defined level of 127)  */

        /* error in CAN protocol (type) / data[2] */
        /// <summary>
        /// unspecified
        /// </summary>
        public const Byte CAN_ERR_PROT_UNSPEC = 0x00; /* unspecified */
        /// <summary>
        /// single bit error
        /// </summary>
        public const Byte CAN_ERR_PROT_BIT = 0x01; /* single bit error */
        /// <summary>
        /// frame format error
        /// </summary>
        public const Byte CAN_ERR_PROT_FORM = 0x02; /* frame format error */
        /// <summary>
        /// bit stuffing error
        /// </summary>
        public const Byte CAN_ERR_PROT_STUFF = 0x04; /* bit stuffing error */
        /// <summary>
        /// unable to send dominant bit
        /// </summary>
        public const Byte CAN_ERR_PROT_BIT0 = 0x08; /* unable to send dominant bit */
        /// <summary>
        /// unable to send recessive bit
        /// </summary>
        public const Byte CAN_ERR_PROT_BIT1 = 0x10; /* unable to send recessive bit */
        /// <summary>
        /// bus overload
        /// </summary>
        public const Byte CAN_ERR_PROT_OVERLOAD = 0x20; /* bus overload */
        /// <summary>
        /// active error announcement
        /// </summary>
        public const Byte CAN_ERR_PROT_ACTIVE = 0x40; /* active error announcement */
        /// <summary>
        /// error occured on transmission
        /// </summary>
        public const Byte CAN_ERR_PROT_TX = 0x80; /* error occured on transmission */

        /* error in CAN protocol (location) / data[3] */
        /// <summary>
        /// unspecified
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_UNSPEC = 0x00; /* unspecified */
        /// <summary>
        /// start of frame
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_SOF = 0x03; /* start of frame */
        /// <summary>
        /// ID bits 28 - 21 (SFF: 10 - 3)
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_ID28_21 = 0x02; /* ID bits 28 - 21 (SFF: 10 - 3) */
        /// <summary>
        /// ID bits 20 - 18 (SFF: 2 - 0 )
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_ID20_18 = 0x06; /* ID bits 20 - 18 (SFF: 2 - 0 )*/
        /// <summary>
        /// substitute RTR (SFF: RTR)
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_SRTR = 0x04; /* substitute RTR (SFF: RTR) */
        /// <summary>
        /// identifier extension
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_IDE = 0x05; /* identifier extension */
        /// <summary>
        /// ID bits 17-13
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_ID17_13 = 0x07; /* ID bits 17-13 */
        /// <summary>
        /// ID bits 12-5
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_ID12_05 = 0x0F; /* ID bits 12-5 */
        /// <summary>
        /// ID bits 4-0
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_ID04_00 = 0x0E; /* ID bits 4-0 */
        /// <summary>
        /// RTR 
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_RTR = 0x0C; /* RTR */
        /// <summary>
        /// reserved bit 1
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_RES1 = 0x0D; /* reserved bit 1 */
        /// <summary>
        /// reserved bit 0
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_RES0 = 0x09; /* reserved bit 0 */
        /// <summary>
        /// data length code
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_DLC = 0x0B; /* data length code */
        /// <summary>
        /// data section
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_DATA = 0x0A; /* data section */
        /// <summary>
        /// CRC sequence
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_CRC_SEQ = 0x08; /* CRC sequence */
        /// <summary>
        /// CRC delimiter
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_CRC_DEL = 0x18; /* CRC delimiter */
        /// <summary>
        /// ACK slot
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_ACK = 0x19; /* ACK slot */
        /// <summary>
        /// ACK delimiter
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_ACK_DEL = 0x1B; /* ACK delimiter */
        /// <summary>
        /// end of frame
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_EOF = 0x1A; /* end of frame */
        /// <summary>
        /// intermission
        /// </summary>
        public const Byte CAN_ERR_PROT_LOC_INTERM = 0x12; /* intermission */

        /* error status of CAN-transceiver / data[4] */
        /*                                             CANH CANL */
        public const Byte CAN_ERR_TRX_UNSPEC = 0x00; /* 0000 0000 */
        public const Byte CAN_ERR_TRX_CANH_NO_WIRE = 0x04; /* 0000 0100 */
        public const Byte CAN_ERR_TRX_CANH_SHORT_TO_BAT = 0x05; /* 0000 0101 */
        public const Byte CAN_ERR_TRX_CANH_SHORT_TO_VCC = 0x06; /* 0000 0110 */
        public const Byte CAN_ERR_TRX_CANH_SHORT_TO_GND = 0x07; /* 0000 0111 */
        public const Byte CAN_ERR_TRX_CANL_NO_WIRE = 0x40; /* 0100 0000 */
        public const Byte CAN_ERR_TRX_CANL_SHORT_TO_BAT = 0x50; /* 0101 0000 */
        public const Byte CAN_ERR_TRX_CANL_SHORT_TO_VCC = 0x60; /* 0110 0000 */
        public const Byte CAN_ERR_TRX_CANL_SHORT_TO_GND = 0x70; /* 0111 0000 */
        public const Byte CAN_ERR_TRX_CANL_SHORT_TO_CANH = 0x80; /* 1000 0000 */
    }
    //=======================================================================================
}
//===========================================================================================
