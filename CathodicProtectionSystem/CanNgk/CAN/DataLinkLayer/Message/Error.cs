using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.Message
{
    /// <summary>
    /// Коды ошибок передаваемых во фреймах 
    /// типа ERRORFRAME (см enum FRAMETYPE)
    /// </summary>
    public enum ERROR: byte
    {
        /// <summary>
        /// Bit stuff error
        /// </summary>
        BitStuff,
        /// <summary>
        /// Format error
        /// </summary>
        FormatFrame,
        /// <summary>
        /// Acknowledge error
        /// </summary>
        Acknowledge,
        /// <summary>
        /// Acknowledge error
        /// </summary>
        Bit,
        /// <summary>
        /// CRC error
        /// </summary>
        CRC,
        /// <summary>
        /// Other unspecified error
        /// </summary>
        Other
    }
}
