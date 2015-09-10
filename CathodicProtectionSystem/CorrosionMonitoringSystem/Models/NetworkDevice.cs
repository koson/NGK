using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using Common.ComponentModel;

namespace NGK.CorrosionMonitoringSystem.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class NetworkDevice
    {
        #region Fields And Propetries
        
        private byte _NodeId;
        /// <summary>
        /// 
        /// </summary>
        public byte NodeId
        {
            get { return _NodeId; }
        }
        private DeviceStatus _Status;
        /// <summary>
        /// 
        /// </summary>
        [TypeConverter(typeof(EnumTypeConverter))]
        public DeviceStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        private UInt32 _NetworkId;
        /// <summary>
        /// 
        /// </summary>
        public UInt32 NetworkId
        {
            get { return _NetworkId; }
            set { _NetworkId = value; }
        }
        private string _NetworkDescription;
        /// <summary>
        /// 
        /// </summary>
        public string NetworkDescription
        {
            get { return _NetworkDescription; }
            set { _NetworkDescription = value; }
        }

        private string _Location;
        /// <summary>
        /// 
        /// </summary>
        public string Location
        {
            get { return _Location; }
            set { _Location = value; }
        }
        private uint _PollingInterval;
        /// <summary>
        /// 
        /// </summary>
        public uint PollingInterval
        {
            get { return _PollingInterval; }
            set { _PollingInterval = value; }
        }
        private List<DataObjectInfo> _ObjectDictionary;
        /// <summary>
        /// Словарь объектов
        /// </summary>
        public List<DataObjectInfo> ObjectDictionary
        {
            get { return _ObjectDictionary; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        private NetworkDevice()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public NetworkDevice(IDevice device)
        {
            _NetworkId = device.Network.NetworkId;
            _NetworkDescription = device.Network.Description;
            _NodeId = device.NodeId;
            _Location = device.LocationName;
            _PollingInterval = device.PollingInterval;
            _Status = device.Status;

            // Создаём оъектный словаря устройства
            _ObjectDictionary = new List<DataObjectInfo>();

            foreach(ObjectInfo info in device.Profile.ObjectInfoList)
            {
                _ObjectDictionary.Add(new DataObjectInfo(info));
            }
        }
        #endregion
    }
}
