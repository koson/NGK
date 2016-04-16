using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CorrosionMonitoringSystem.Models.Modbus;

namespace NGK.CorrosionMonitoringSystem.DL.Modbus
{    
    public enum ConnectionType : ushort
    {
        /// <summary>
        /// не определено 
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// RS485 (Modbus)
        /// </summary>
        Modbus = 1, 
        /// <summary>
        /// (CAN НГК ЭХЗ)
        /// </summary>
        CAN = 2, 
    }
    /// <summary>
    /// Класс обёртка над Modbus File - передстваляющий сетевое устройство
    /// и предназначен для адаптации параметров данного устройства
    /// </summary>
    public class mKIP9811
    {
        #region Fields And Properties

        private File _Device;
        /// <summary>
        /// Сетевое устройство
        /// </summary>
        public File Device
        {
            get { return _Device; }
        }
        /// <summary>
        /// 0x0001
        /// </summary>
        public UInt16 SoftwareVersion
        {
            get { return _Device.Records[ModbusVisitingCard.VisitingCard.SoftwareVersion].Value; }
            set { _Device.Records[ModbusVisitingCard.VisitingCard.SoftwareVersion].Value = value; }
        }
        /// <summary>
        /// 0x0002
        /// </summary>
        public UInt16 HardwareVersion
        {
            get { return _Device.Records[ModbusVisitingCard.VisitingCard.SoftwareVersion].Value; }
            set { _Device.Records[ModbusVisitingCard.VisitingCard.SoftwareVersion].Value = value; }
        }
        /// <summary>
        /// 0x0003, 0x0004, 0x0005
        /// </summary>
        public UInt64 SerialNumber
        {
            get 
            {
                UInt64 sn = 0;
                sn = (((UInt64)_Device.Records[ModbusVisitingCard.VisitingCard.SerialNumberHigh].Value) << 32);
                sn |= (((UInt64)_Device.Records[ModbusVisitingCard.VisitingCard.SerialNumberMiddle].Value) << 16);
                sn |= (UInt64)_Device.Records[ModbusVisitingCard.VisitingCard.SerialNumberHigh].Value;
                return sn;
            }
            set
            {
                _Device.Records[ModbusVisitingCard.VisitingCard.SerialNumberHigh].Value =
                    (UInt16)(value >> 32);
                _Device.Records[ModbusVisitingCard.VisitingCard.SerialNumberMiddle].Value =
                    (UInt16)(value >> 16);
                _Device.Records[ModbusVisitingCard.VisitingCard.SerialNumberHigh].Value =
                    (UInt16)value;
            }
        }
        /// <summary>
        /// 0x0006
        /// </summary>
        public UInt16 CRC16
        {
            get { return _Device.Records[KIP9811Address.VisitingCard.CRC16].Value; }
            set { _Device.Records[KIP9811Address.VisitingCard.CRC16].Value = value; }
        }
        /// <summary>
        /// 0x0007
        /// </summary>
        public UInt16 VendorCode
        {
            get { return _Device.Records[KIP9811Address.VisitingCard.VendorCode].Value; }
            set { _Device.Records[KIP9811Address.VisitingCard.VendorCode].Value = value; }
        }
        /// <summary>
        /// 0x0008
        /// </summary>
        public ConnectionType NetworkType
        {
            get 
            { 
                if (Enum.IsDefined(typeof(ConnectionType), 
                    _Device.Records[KIP9811Address.ServiceInformation.NetworkType].Value))
                {
                    return (ConnectionType)_Device.Records[KIP9811Address.ServiceInformation.NetworkType].Value;
                }
                throw new InvalidCastException();
            }
            set { _Device.Records[KIP9811Address.VisitingCard.VendorCode].Value = (UInt16)value; } 
        }
        /// <summary>
        /// 0x0009
        /// </summary>
        public UInt16 NetworkNumber
        {
            get { return _Device.Records[KIP9811Address.ServiceInformation.NetworkNumber].Value; }
            set { _Device.Records[KIP9811Address.ServiceInformation.NetworkNumber].Value = value; }
        }
        /// <summary>
        /// 0x000A
        /// </summary>
        public UInt16 NetworkAddress
        {
            get { return _Device.Records[KIP9811Address.ServiceInformation.NetwrokAddress].Value; }
            set { _Device.Records[KIP9811Address.ServiceInformation.NetwrokAddress].Value = value; }
        }
        /// <summary>
        /// 0x000B
        /// </summary>
        public Boolean ConnectionStatus
        {
            get
            {
                return mKIP9811.ToBoolean(_Device.Records[KIP9811Address.ServiceInformation.ConectionStatus].Value);
            }
            set 
            {
                UInt16 boolCode = 0;
                
                if (value)
                {
                    boolCode = 1;
                }
                
                _Device.Records[KIP9811Address.ServiceInformation.ConectionStatus].Value = boolCode;
            }
        }
        /// <summary>
        /// 0x000C
        /// </summary>        
        public UInt16 ErrorsRegister
        {
            get { return _Device.Records[KIP9811Address.Errors].Value; }
            set { _Device.Records[KIP9811Address.Errors].Value = value; }
        }
        /// <summary>
        /// 0x000D
        /// </summary>
        public UInt16 RegistrationErrorsRegister
        {
            get { return _Device.Records[KIP9811Address.ErrorsRegistration].Value; }
            set { _Device.Records[KIP9811Address.ErrorsRegistration].Value = value; }
        }
        /// <summary>
        /// 0x000E
        /// </summary>
        public DeviceStatus DeviceStatus
        {
            get
            {
                if (Enum.IsDefined(typeof(DeviceStatus), _Device.Records[KIP9811Address.DeviceStatus].Value))
                {
                    return (DeviceStatus)_Device.Records[KIP9811Address.DeviceStatus].Value;
                }
                throw new InvalidCastException();
            }
            set
            {
                _Device.Records[KIP9811Address.DeviceStatus].Value = (UInt16)value;
            }
        }
        /// <summary>
        /// 0x0036 0x0037
        /// </summary>
        public UInt32 DateTimeUnix
        {
            get 
            {
                UInt32 time = (UInt32)_Device.Records[KIP9811Address.datetime_low].Value;
                time |= (((UInt32)_Device.Records[KIP9811Address.datetime_high].Value) << 16);
                return time;
            }
            set
            {
                _Device.Records[KIP9811Address.datetime_low].Value = (UInt16)value;
                _Device.Records[KIP9811Address.datetime_high].Value = (UInt16)(value >> 16);
            }
        }
        #endregion

        #region Constructors
        private mKIP9811()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public mKIP9811(File device)
        {
            if (device.Records[KIP9811Address.VisitingCard.DeviceType].Value != 
                (UInt16)NGK.CAN.ApplicationLayer.Network.Devices.DeviceType.KIP_BATTERY_POWER_v1)
            {
                throw new Exception(String.Format("Недопустимый тип устройства: {0}",
                    device.Records[KIP9811Address.VisitingCard.DeviceType].Value));
            }

            _Device = device;
        }
        #endregion

        #region Methods
        public static bool ToBoolean(UInt16 value)
        {
            if (value > 0)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
