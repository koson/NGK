using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using Common.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;

namespace NGK.CorrosionMonitoringSystem.Models
{
    [Serializable]
    public sealed class NgkCanDevice
    {
        #region Helper
        
        public struct Indexes
        {
            public const UInt16 ID = 0x0000;
            public const UInt16 DEVICE_TYPE = 0x0001;
            public const UInt16 NODE_ID = 0x0002;
            public const UInt16 LOCATION = 0x0003;
            public const UInt16 POLLING_INTERVAL = 0x0004;
            public const UInt16 DEVICE_STATUS = 0x0005;
            public const UInt16 NETWORK_ID = 0x0006;
            public const UInt16 NETWORK_NAME = 0x0007;
        }

        #endregion

        #region Fields And Propetries

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Система")]
        [DisplayName("ID")]
        [Description("Уникальный идентификатор устройства")]
        public Guid Id
        {
            get { return (Guid)_Parameters[Indexes.ID].Value; }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Система")]
        [DisplayName("Тип устройства")]
        [Description("Тип сетевого устройства")]
        public DeviceType DeviceType
        {
            get { return (DeviceType)_Parameters[Indexes.DEVICE_TYPE].Value; }
            set { _Parameters[Indexes.DEVICE_TYPE].Value = value; }
        }
        /// <summary>
        /// Сетевой идентификатор устройства 1...127
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Сетевые настройки")]
        [DisplayName("Адрес")]
        [Description("Сетевой идентификатор устройства")]
        public byte NodeId
        {
            get { return Convert.ToByte(_Parameters[Indexes.NODE_ID].Value); }
            set { _Parameters[Indexes.NODE_ID].Value = value; }
        }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Система")]
        [DisplayName("Статус")]
        [Description("Текущее состояние устройства")]
        public DeviceStatus Status
        {
            get { return (DeviceStatus)_Parameters[Indexes.DEVICE_STATUS].Value; }
            set { _Parameters[Indexes.DEVICE_STATUS].Value = value; }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Сетевые настройки")]
        [DisplayName("Id сети")]
        [Description("Уникальный идентификатор сети")]
        public UInt32 NetworkId
        {
            get { return Convert.ToUInt32(_Parameters[Indexes.NETWORK_ID].Value); }
            set { _Parameters[Indexes.DEVICE_STATUS].Value = value; }
        }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Сетевые настройки")]
        [DisplayName("Наименование сети")]
        [Description("Пользовательское название сети")]
        public string NetworkName
        {
            get { return Convert.ToString(_Parameters[Indexes.NETWORK_NAME].Value); }
            private set { _Parameters[Indexes.NETWORK_NAME].Value = value; }
        }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Сетевые настройки")]
        [DisplayName("Месторасположение")]
        [Description("Наименование географического места расположения КИП")]
        public string Location
        {
            get { return Convert.ToString(_Parameters[Indexes.LOCATION].Value); }
            private set { _Parameters[Indexes.LOCATION].Value = value; }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Настройки")]
        [DisplayName("Интервал опроса, сек")]
        [Description("Период опроса устройства")]
        public uint PollingInterval
        {
            get { return Convert.ToUInt32(_Parameters[Indexes.POLLING_INTERVAL].Value); }
            set { _Parameters[Indexes.POLLING_INTERVAL].Value = value; }
        }
        
        private ParametersCollection _Parameters;
        
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Данные")]
        [DisplayName("Параметры")]
        [Description("Список параметров устройства")]
        public ParametersCollection Parameters
        {
            get { return _Parameters; }
        }

        #endregion

        #region Constructor

        public NgkCanDevice(IDevice device)
        {
            _Parameters = new ParametersCollection();

            // Добавляем общие для всех устройств параметры 
            _Parameters.Add(new Parameter(Indexes.ID, "GUID", "Уникальный идентификатор устройства", 
                true, false, false, "ID", string.Empty, ObjectCategory.System, device.Id));
            _Parameters.Add(new Parameter(Indexes.DEVICE_TYPE, "DeviceType", "Тип устройства",
                true, false, true, "Тип устройства", string.Empty, ObjectCategory.System, device.DeviceType));
            _Parameters.Add(new Parameter(Indexes.NODE_ID, "NodeId", "Сетевой идентификатор устройтсва",
                true, false, true, "Сетевой адрес", string.Empty, ObjectCategory.System, device.NodeId));
            _Parameters.Add(new Parameter(Indexes.LOCATION, "Location", "Наименование места установки оборудования",
                true, false, true, "Расположение", string.Empty, ObjectCategory.System, device.LocationName));
            _Parameters.Add(new Parameter(Indexes.POLLING_INTERVAL, "PollingInterval", "Период опроса устройства, мсек",
                true, false, true, "Периуд опроса", "мсек", ObjectCategory.System, device.PollingInterval));
            _Parameters.Add(new Parameter(Indexes.DEVICE_STATUS, "DeviceStatus", "Состояние устройства", 
                true, false, true, "Состояние устройства", string.Empty, ObjectCategory.System, device.Status));
            _Parameters.Add(new Parameter(Indexes.NETWORK_ID, "NetworkId", "ID CAN сети", 
                true, false, false, "ID сети", string.Empty, ObjectCategory.System,
                device.Network == null ? 0 : device.Network.NetworkId));
            _Parameters.Add(new Parameter(Indexes.NETWORK_NAME, "NetworkName", "Наименование сети",
                true, false, true, "Сеть CAN", string.Empty, ObjectCategory.System, 
                device.Network == null ? "Не установлена" : device.Network.NetworkName));

            foreach (ObjectInfo info in device.Profile.ObjectInfoList)
            {
                _Parameters.Add(new Parameter(info));
            }
        }

        #endregion

        public static void Update(NgkCanDevice device, IDevice canDevice)
        {
            string msg;

            if (device.Id != canDevice.Id)
            {
                msg = String.Format(
                    "Не удалось обновить параметры устройтсва. Не совпадает Id устройств");
                throw new InvalidOperationException(msg);
            }

            if (device.NetworkId == 0)
            {
                // Не обновляем устройство, если оно непривязано к сети
                return;
            }

            //device.Location = canDevice.LocationName;
            //device.PollingInterval = canDevice.PollingInterval;
            //device.Status = canDevice.Status;
            //device.NetworkId = canDevice.Network == null ? 0 : canDevice.Network.NetworkId;
            //device.NetworkName = canDevice.Network == null ? 0 : canDevice.Network.NetworkName;

            foreach (Parameter parameter in device.Parameters)
            {
                DataObject dataObject = canDevice.ObjectDictionary[parameter.Index];
                parameter.Modified = dataObject.Modified;
                parameter.Status = dataObject.Status;
                parameter.Value = dataObject.TotalValue;
            }
        }
    }
}
