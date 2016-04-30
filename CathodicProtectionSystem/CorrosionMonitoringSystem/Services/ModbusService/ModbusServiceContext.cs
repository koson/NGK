using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices;

namespace NGK.CorrosionMonitoringSystem.Models.Modbus
{
    public class ModbusServiceContext
    {
        /// <summary>
        /// ������������� CAN ���������� 
        /// </summary>
        public struct CAN
        {
            /// <summary>
            /// ������������ CAN-����
            /// </summary>
            public UInt32 NetworkId;
            /// <summary>
            /// ������� ������������� ���������� � ����
            /// </summary>
            public Byte NodeId;
        }
        /// <summary>
        /// Modbus 
        /// </summary>
        public struct Modbus
        {
            /// <summary>
            /// ����� ����� ���������� ������������� CAN-����������
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
