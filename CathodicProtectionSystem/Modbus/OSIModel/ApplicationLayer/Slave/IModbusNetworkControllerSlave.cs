using System;
using Common.Controlling;
using Modbus.OSIModel.DataLinkLayer.Slave;

namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    public interface IModbusNetworkControllerSlave : IManageable
    {
        string NetworkName { get; set; }
        IDataLinkLayer Connection { get; set; }
        ModbusSlaveDevicesCollection Devices { get; }
        WorkMode Mode { get; }
        event NetworkErrorOccurredEventHandler NetworkErrorOccurred;
        event EventHandler DevicesListWasChanged;
    }
}
