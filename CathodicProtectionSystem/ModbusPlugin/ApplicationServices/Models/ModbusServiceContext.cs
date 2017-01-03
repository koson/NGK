using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;

namespace ModbusPlugin.ApplicationServices.Models
{
    public class ModbusServiceContext
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

        public ModbusServiceContext(DeviceBase canDevice, File modbusDevice)
        {
            CanDevice = new CAN();
            CanDevice.NetworkId = canDevice.Network.NetworkId;
            CanDevice.NodeId = canDevice.NodeId;
            ModbusDevice = new Modbus();
            ModbusDevice.FileNumber = modbusDevice.Number;
        }
    }
}
