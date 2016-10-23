using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Common.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using WinForms = System.Windows.Forms;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;

namespace Infrastructure.Api.Models.CAN
{
    [Serializable]
    public sealed class NgkCanDevice : INotifyPropertyChanged, IDeviceSummaryParameters, IDeviceInfo
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

            public const string POLARISATION_POTENTIAL_ENABLED = "polarisation_pot_en";
            public const string POLARISATION_POTENTIAL = "polarization_pot";
            public const string POLARISATION_CURRENT_ENABLED = "polarisation_cur_en";
            public const string POLARISATION_CURRENT = "polarization_cur";
            public const string PROTECTION_POTENTIAL_ENABLED = "protection_pot_en";
            public const string PROTECTION_POTENTIAL = "protection_pot";
            public const string PROTECTION_CURRENT_ENABLED = "protection_cur_en";
            public const string PROTECTION_CURRENT = "protection_cur";
            public const string CORROSION_DEPTH = "corrosion_depth";
            public const string CORROSION_SPEED = "corrosion_speed";
            public const string TAMPER = "tamper";
        }

        #endregion

        #region Constructor

        public NgkCanDevice(IDevice device)
        {
            _Parameters = new ParametersCollection();

            // Добавляем общие для всех устройств параметры
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.ID,
                "GUID", "Уникальный идентификатор устройства", string.Empty,
                true, false, ObjectCategory.System, device.DeviceType, device.Id, Guid.Empty));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.DEVICE_TYPE,
                "Тип устройства", "Тип устройства", string.Empty,
                true, false, ObjectCategory.System, device.DeviceType, device.DeviceType, DeviceType.UnknownTypeOfDevice));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.NODE_ID,
                "Сетевой адрес", "Сетевой идентификатор устройтсва", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType, device.NodeId, (byte)1));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.LOCATION,
                "Расположение", "Наименование места установки оборудования", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType, device.LocationName, String.Empty));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.POLLING_INTERVAL,
                "Периуд опроса", "Период опроса устройства, мсек", "мсек",
                true, true, ObjectCategory.System, device.DeviceType, device.PollingInterval, (uint)1000));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.DEVICE_STATUS,
                "Состояние устройства", "Состояние устройства", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType, device.Status, DeviceStatus.Stopped));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.NETWORK_ID,
                "ID сети", "ID CAN сети", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType,
                device.Network == null ? 0 : device.Network.NetworkId, (uint)0));
            _Parameters.Add(Parameter.CreateSpecialParameter(ParameterNames.NETWORK_NAME,
                "Сеть CAN", "Наименование сети", string.Empty,
                true, true, ObjectCategory.System, device.DeviceType,
                device.Network == null ? "Не установлена" : device.Network.NetworkName, "Не установлена"));

            foreach (ObjectInfo info in device.Profile.ObjectInfoList)
            {
                Parameter prm = new Parameter(info);
                if (!_Parameters.Contains(prm.Name))
                    _Parameters.Add(prm);
            }

            foreach (Parameter parameter in _Parameters)
            {
                if ((parameter.Category == ObjectCategory.Configuration) ||
                    (parameter.Category == ObjectCategory.System) ||
                    (parameter.Category == ObjectCategory.Measured))
                {
                    DateTime modified = DateTime.Now;

                    if (parameter.IsComplexParameter)
                    {
                        List<object> values = new List<object>();
                        ObjectStatus status = ObjectStatus.NoError;

                        foreach (UInt16 index in parameter.Indexes)
                        {
                            DataObject dataObject = device.ObjectDictionary[index];
                            values.Add(device.GetObject(index));

                            if (dataObject.Modified > modified)
                                modified = dataObject.Modified;

                            if (dataObject.Status != ObjectStatus.NoError)
                                status = dataObject.Status;
                        }

                        parameter.Modified = null; // modified;
                        parameter.Value = CanDevicePrototype.GetProfile(device.DeviceType)
                            .ComplexParameters[parameter.Name].Converter.ConvertTo(values.ToArray());
                    }
                    else
                    {
                        if (!parameter.IsSpecialParameter)
                        {
                            DataObject dataObject = device.ObjectDictionary[parameter.Indexes[0]];
                            parameter.Modified = null; //modified;
                            parameter.Status = dataObject.Status;
                            parameter.Value = dataObject.TotalValue;
                            parameter.DefaultValue =
                                dataObject.Info.DataTypeConvertor.ConvertToOutputValue(dataObject.Info.DefaultValue);
                        }
                    }
                }
            }

            Parameters[ParameterNames.POLARISATION_CURRENT].PropertyChanged +=
                new PropertyChangedEventHandler(EventHandler_NgkCanDevice_PropertyChanged);
            Parameters[ParameterNames.POLARISATION_POTENTIAL].PropertyChanged +=
                new PropertyChangedEventHandler(EventHandler_NgkCanDevice_PropertyChanged);
            Parameters[ParameterNames.PROTECTION_POTENTIAL].PropertyChanged +=
                new PropertyChangedEventHandler(EventHandler_NgkCanDevice_PropertyChanged);
            Parameters[ParameterNames.PROTECTION_CURRENT].PropertyChanged +=
                new PropertyChangedEventHandler(EventHandler_NgkCanDevice_PropertyChanged);
            Parameters[ParameterNames.CORROSION_DEPTH].PropertyChanged +=
                new PropertyChangedEventHandler(EventHandler_NgkCanDevice_PropertyChanged);
            Parameters[ParameterNames.CORROSION_SPEED].PropertyChanged +=
                new PropertyChangedEventHandler(EventHandler_NgkCanDevice_PropertyChanged);
            Parameters[ParameterNames.TAMPER].PropertyChanged +=
                new PropertyChangedEventHandler(EventHandler_NgkCanDevice_PropertyChanged);
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
                if ((DeviceStatus)_Parameters[ParameterNames.DEVICE_STATUS].Value != value)
                {
                    _Parameters[ParameterNames.DEVICE_STATUS].Value = value;
                    OnPropertyChanged("Status");
                    OnDeviceChangedStatus();
                }
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

        public float? PolarisationPotential
        {
            get
            {
                return (bool)Parameters[ParameterNames.POLARISATION_POTENTIAL_ENABLED].Value ?
                    (float?)Parameters[ParameterNames.POLARISATION_POTENTIAL].Value : null;
            }
        }

        public float? PolarisationCurrent
        {
            get
            {
                if ((bool)Parameters[ParameterNames.POLARISATION_CURRENT_ENABLED].Value)
                {
                    float? v = (float?)Parameters[ParameterNames.POLARISATION_CURRENT].Value;
                    if (v.HasValue)
                        return v.Value == 327.67f ? null : v;
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        public float? ProtectionPotential
        {
            get
            {
                if ((bool)Parameters[ParameterNames.PROTECTION_POTENTIAL_ENABLED].Value)
                    return (float?)Parameters[ParameterNames.PROTECTION_POTENTIAL].Value;
                else
                    return null;
            }
        }

        public float? ProtectionCurrent
        {
            get
            {
                if ((bool)Parameters[ParameterNames.PROTECTION_CURRENT_ENABLED].Value)
                {
                    float? v = (float?)Parameters[ParameterNames.PROTECTION_CURRENT].Value;

                    if (v.HasValue)
                        return v.Value == 0xFFFF * 0.05 ? null : v;
                    else
                        return null;
                }
                else
                    return null;
            }
        }

        public UInt32? CorrosionDepth
        {
            get
            {
                UInt32 value =
                    (UInt32)Parameters[ParameterNames.CORROSION_DEPTH].Value;
                return value == 0xFFFF ? null : (UInt32?)value;
            }
        }

        public UInt32? CorrosionSpeed
        {
            get
            {
                UInt32 value =
                    (UInt32)Parameters[ParameterNames.CORROSION_SPEED].Value;
                return value == 0xFFFF ? null : (UInt32?)value;
            }
        }

        public Boolean Tamper
        {
            get
            {
                return (bool)Parameters[ParameterNames.TAMPER].Value;
            }
        }

        #endregion

        #region Method

        public void Update(IDevice canDevice)
        {
            string msg;
            ObjectInfo objectInfo;

            if (Id != canDevice.Id)
            {
                msg = String.Format(
                    "Не удалось обновить параметры устройства. Не совпадает Id устройств");
                throw new InvalidOperationException(msg);
            }

            if (NetworkId == 0)
            {
                // Не обновляем устройство, если оно непривязано к сети
                return;
            }

            Status = canDevice.Status;

            if (!((Status == DeviceStatus.ConfigurationError) ||
                (Status == DeviceStatus.Operational)))
            {
                // Сбрасываем измеренные значения, если устройство 
                // находиться в состоянии ошибки соединения
                if (Status == DeviceStatus.CommunicationError)
                {
                    foreach (Parameter parameter in Parameters)
                    {
                        if (parameter.IsSpecialParameter)
                        {
                            continue;
                        }

                        if (parameter.Category == ObjectCategory.Measured)
                        {
                            if (parameter.IsComplexParameter)
                            {
                                ComplexParameter cmplx = canDevice.Profile.ComplexParameters[parameter.Name];
                                if ((ValueType)parameter.Value != (ValueType)cmplx.DefaultValue)
                                    parameter.Value = cmplx.DefaultValue;
                            }
                            else
                            {
                                objectInfo =
                                    canDevice.Profile.ObjectInfoList[parameter.Indexes[0]];
                                object newValue =
                                    objectInfo.DataTypeConvertor.ConvertToOutputValue(objectInfo.DefaultValue);
                                if (objectInfo.DataTypeConvertor.ConvertToBasis((ValueType)parameter.Value) !=
                                    objectInfo.DefaultValue)
                                {
                                    parameter.Value = newValue;
                                }
                            }
                        }
                    }
                }
                return;
            }

            foreach (Parameter parameter in Parameters)
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
                        parameter.Value = CanDevicePrototype.GetProfile(DeviceType)
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

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        void OnDeviceChangedStatus()
        {
            if (DeviceChangedStatus != null)
                DeviceChangedStatus(this, new EventArgs());
        }

        #endregion

        #region Events Handlers

        private void EventHandler_NgkCanDevice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Parameter parameter = (Parameter)sender;

            if (e.PropertyName == "Value")
            {
                switch (parameter.Name)
                {
                    case ParameterNames.POLARISATION_CURRENT:
                        {
                            OnPropertyChanged("PolarisationCurrent");
                            break;
                        }
                    case ParameterNames.POLARISATION_POTENTIAL:
                        {
                            OnPropertyChanged("PolarisationPotential");
                            break;
                        }
                    case ParameterNames.PROTECTION_CURRENT:
                        {
                            OnPropertyChanged("ProtectionCurrent");
                            break;
                        }
                    case ParameterNames.PROTECTION_POTENTIAL:
                        {
                            OnPropertyChanged("ProtectionPotential");
                            break;
                        }
                    case ParameterNames.CORROSION_DEPTH:
                        {
                            OnPropertyChanged("CorrosionDepth");
                            break;
                        }
                    case ParameterNames.CORROSION_SPEED:
                        {
                            OnPropertyChanged("CorrosionSpeed");
                            break;
                        }
                    case ParameterNames.TAMPER:
                        {
                            OnPropertyChanged("Tamper");
                            break;
                        }
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler DeviceChangedStatus;

        #endregion
    }
}
