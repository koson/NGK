using System;

//===================================================================================
namespace Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort
{
    //===============================================================================
    /// <summary>
    /// Делегат для реализации события приёма запроса от мастера сети.
    /// </summary>
    /// <param name="sender">Отправитель события</param>
    /// <param name="args">Аргументы события</param>
    public delegate void EventHandlerRequestWasRecived(Object sender,
        MessageEventArgs args);
    //===============================================================================
}
//===================================================================================
// End of file