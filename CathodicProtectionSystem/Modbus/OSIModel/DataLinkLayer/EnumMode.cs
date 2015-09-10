﻿using System;

//===================================================================================
namespace Modbus.OSIModel.DataLinkLayer
{
    //===============================================================================
    /// <summary>
    /// Режим передачи данных протокола Modbus 
    /// </summary>
    public enum TransmissionMode
    {
        //---------------------------------------------------------------------------
        /// <summary>
        /// Данные передаются в режиме ASCII
        /// </summary>
        ASCII,
        //---------------------------------------------------------------------------
        /// <summary>
        /// Данные передаются в режиме RTU
        /// </summary>
        RTU
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file