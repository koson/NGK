using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataLinkLayer.Message
{
    /// <summary>
    /// Типы кадров CAN (см спецификацию от BOSH: can2spec.pdf 3.1 Frame Types)
    /// </summary>
    public enum FrameType
    {
        /// <summary>
        /// A DATA FRAME carries data from a transmitter to the receivers.
        /// </summary>
        DATAFRAME = 0,
        /// <summary>
        /// A REMOTE FRAME is transmitted by a bus unit to request the 
        /// transmission of the DATA FRAME with the same IDENTIFIER.
        /// </summary>
        REMOTEFRAME = 1,
        /// <summary>
        /// An ERROR FRAME is transmitted by any unit on detecting a bus error.
        /// см. коды в enum ERROR
        /// </summary>
        ERRORFRAME = 2,
        /// <summary>
        /// An OVERLOAD FRAME is used to provide for an extra delay between 
        /// the preceding and the succeeding DATA or REMOTE FRAMEs.
        /// </summary>
        OVERLOADFRAME = 3,
        /// <summary>
        /// Кадр со служебной информацией передаваемой от CAN - порта
        /// </summary>
        //INFOFRAME = 4
    }
}
