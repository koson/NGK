﻿using System;

//===================================================================================
namespace Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort
{
    //===============================================================================
    /// <summary>
    /// Делегат для создания события отправки ответного сообщения мастеру сети.
    /// </summary>
    /// <param name="sender">Отправитель, подчинённое устройство</param>
    /// <param name="args">Аргументы события</param>
    public delegate void EventHandleResponseWasSent(Object sender, 
        MessageEventArgs args);
    //===============================================================================
}
//===================================================================================
// End of file