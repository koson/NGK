using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using Common.ComponentModel;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public abstract class NgkCanDeviceBase
    {
        #region Helper
        
        public struct Indexes
        {
            public static UInt16 NODE_ID = 0x0000;
        }

        #endregion

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
        
        private List<Parameter> _Parameters;

        public Parameter[] Parameters
        {
            get { return _Parameters.ToArray(); }
        }

        #endregion

        #region Constructor

        public NgkCanDeviceBase()
        {
            _Parameters = new List<Parameter>();

            // Добавляем общие для всех устройств параметры 
            
            // Добавляем визитную карточку устройства
            _Parameters.Add(new Parameter(Indexes.NODE_ID, "NodeId", "Сетевой идентификатор устройтсва",
                true, false, true, "Сетевой адрес", string.Empty, Category.System, (byte)1));
        }

        #endregion
    }
}
