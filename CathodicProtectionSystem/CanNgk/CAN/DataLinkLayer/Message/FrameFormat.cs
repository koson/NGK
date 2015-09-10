using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.Message
{
    /// <summary>
    /// Список форматов кадров CAN (см спецификацию от BOSH: 
    /// can2spec.pdf PART B 3.1 Frame Formats)
    /// </summary>
    [Serializable]
    public enum FrameFormat
    {
        /// <summary>
        /// The IDENTIFIER are 11 bit 
        /// </summary>        
        StandardFrame,
        /// <summary>
        /// The IDENTIFIER are 29 bit 
        /// </summary>
        ExtendedFrame,
        /// <summary>
        /// The IDENTIFIER are 11 bit and 29 bit
        /// </summary>
        MixedFrame
    }
}
