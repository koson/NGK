using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using Common.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;

namespace NGK.CorrosionMonitoringSystem.Models
{
    [Serializable]
    public sealed class NgkCanDevice
    {
        #region Helper
        
        public struct ParameterNames
        {
            public const UInt16 ID_ADR = 0x0000;
            public const string ID = "Id";
            public const UInt16 DEVICE_TYPE_ADR = 0x0001;
            public const string DEVICE_TYPE = "Device type";
            public const UInt16 NODE_ID_ADR = 0x0002;
            public const string NODE_ID = "Node Id";
            public const UInt16 LOCATION_ADR = 0x0003;
            public const string LOCATION = "Location";
            public const UInt16 POLLING_INTERVAL_ADR = 0x0004;
            public const string POLLING_INTERVAL = "Polling interval";
            public const UInt16 DEVICE_STATUS_ADR = 0x0005;
            public const string DEVICE_STATUS = "Device status";
            public const UInt16 NETWORK_ID_ADR = 0x0006;
            public const string NETWORK_ID = "Network Id";
            public const UInt16 NETWORK_NAME_ADR = 0x0007;
            public const string NETWORK_NAME = "Network Name";
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
            get { return (Guid)_Parameters[ParameterNames.ID].Value; }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Система")]
        [DisplayName("Тип устройства")]
        [Description("Тип сетевого устройства")]
        public DeviceType DeviceType
        {
            get { return (DeviceType)_Parameters[ParameterNames.DEVICE_TYPE].Value; }
            //set
            //{
            //    _Parameters[ParameterNames.DEVICE_TYPE].Value = value;
            //}
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
            get { return Convert.ToByte(_Parameters[ParameterNames.NODE_ID].Value); }
            //set
            //{
            //    _Parameters[ParameterNames.NODE_ID].Value = value;
            //}
        }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Система")]
        [DisplayName("Статус")]
        [Description("Текущее состояние устройства")]
        public DeviceStatus Status
        {
            get { return (DeviceStatus)_Parameters[ParameterNames.DEVICE_STATUS].Value; }
            set 
            {
                _Parameters[ParameterNames.DEVICE_STATUS].Value = value;
            }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Сетевые настройки")]
        [DisplayName("Id сети")]
        [Description("Уникальный идентификатор сети")]
        public UInt32 NetworkId
        {
            get { return Convert.ToUInt32(_Parameters[ParameterNames.NETWORK_ID].Value); }
            //set
            //{
            //    _Parameters[ParameterNames.NETWORK_ID].Value = value;
            //}
        }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Сетевые настройки")]
        [DisplayName("Наименование сети")]
        [Description("Пользовательское название сети")]
        public string NetworkName
        {
            get { return Convert.ToString(_Parameters[ParameterNames.NETWORK_NAME].Value); }
            //private set 
            //{ 
            //    _Parameters[ParameterNames.NETWORK_NAME]
            //        .SetObjectValue(ParameterNames.NETWORK_NAME_ADR, value); 
            //}
        }

        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Сетевые настройки")]
        [DisplayName("Месторасположение")]
        [Description("Наименование географического места расположения КИП")]
        public string Location
        {
            get { return Convert.ToString(_Parameters[ParameterNames.LOCATION].Value); }
            //private set 
            //{ 
            //    _Parameters[ParameterNames.LOCATION]
            //        .SetObjectValue(ParameterNames.LOCATION_ADR, value); 
            //}
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Настройки")]
        [DisplayName("Интервал опроса, сек")]
        [Description("Период опроса устройства")]
        public uint PollingInterval
        {
            get { return Convert.ToUInt32(_Parameters[ParameterNames.POLLING_INTERVAL].Value); }
            //set 
            //{
            //    _Parameters[ParameterNames.POLLING_INTERVAL]
            //        .SetObjectValue(ParameterNames.POLLING_INTERVAL_ADR, value);
            //}
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
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.ID, 
                "GUID", "Уникальный идентификатор устройства", string.Empty, 
                true, false, ObjectCategory.System, device.DeviceType, device.Id));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.DEVICE_TYPE,
                "Тип устройства", "Тип устройства", string.Empty,
                true, false, ObjectCategory.System, device.DeviceType, device.DeviceType));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.NODE_ID, 
                "Сетевой адрес", "Сетевой идентификатор устройтсва", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType, device.NodeId));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.LOCATION,
                "Расположение", "Наименование места установки оборудования", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType, device.LocationName));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.POLLING_INTERVAL,
                "Периуд опроса", "Период опроса устройства, мсек", "мсек",
                true, true, ObjectCategory.System, device.DeviceType, device.PollingInterval));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.DEVICE_STATUS,
                "Состояние устройства", "Состояние устройства", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType, device.Status));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.NETWORK_ID,
                "ID сети", "ID CAN сети", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType,
                device.Network == null ? 0 : device.Network.NetworkId));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.NETWORK_NAME, 
                "Сеть CAN", "Наименование сети", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType,
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

            device.Status = canDevice.Status;

            if (!((device.Status == DeviceStatus.ConfigurationError) || 
                (device.Status == DeviceStatus.Operational)))
            {
                return;
            }

            foreach (Parameter parameter in device.Parameters)
            {
                if (parameter.IsSpecialParameter)
                {
                    continue;
                }
                else
                {
                    if (parameter.IsComplexParameter)
                    {
                        List<object> values = new List<object>();
                        DateTime modified = new DateTime();
                        ObjectStatus status = ObjectStatus.NoError;

                        foreach (UInt16 index in parameter.Indexes)
                        {
                            DataObject dataObject = canDevice.ObjectDictionary[index];
                            values.Add(canDevice.GetObject(index));

                            if (dataObject.Modified > modified)
                                modified = dataObject.Modified;

                            if (dataObject.Status != ObjectStatus.NoError)
                                status = dataObject.Status;
                        }

                        parameter.Modified = modified;
                        parameter.Value = Prototype.GetProfile(device.DeviceType)
                            .ComplexParameters[parameter.Name].Converter.ConvertTo(values.ToArray());
                    }
                    else
                    {
                        DataObject dataObject = canDevice.ObjectDictionary[parameter.Indexes[0]];
                        parameter.Modified = dataObject.Modified;
                        parameter.Status = dataObject.Status;
                        parameter.Value = dataObject.TotalValue;
                    }
                }
            }
        }
    }
}
