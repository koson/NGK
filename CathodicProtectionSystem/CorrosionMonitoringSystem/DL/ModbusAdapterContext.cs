using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices;

namespace NGK.CorrosionMonitoringSystem.DL
{
    public class ModbusAdapterContext
    {
        /// <summary>
        /// Идентификатор CAN устройства 
        /// </summary>
        public struct CAN
        {
            /// <summary>
            /// Наименование CAN-сети
            /// </summary>
            public UInt32 NetworkId;
            /// <summary>
            /// Сетевой идентификатор устройства в сети
            /// </summary>
            public Byte NodeId;
        }
        /// <summary>
        /// Modbus 
        /// </summary>
        public struct Modbus
        {
            /// <summary>
            /// Номер файла устройства отображающего CAN-устройство
            /// </summary>
            public UInt16 FileNumber;
        }
        public readonly CAN CanDevice;
        public readonly Modbus ModbusDevice;

        public ModbusAdapterContext(Device canDevice, File modbusDevice)
        {
            CanDevice = new CAN();
            CanDevice.NetworkId = canDevice.Network.NetworkId;
            ModbusDevice = new Modbus();
            ModbusDevice.FileNumber = modbusDevice.Number;
        }
    }
}
