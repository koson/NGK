using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public class CanDevice
    {
        public class Parameter
        {
            public Parameter(ValueType value, DateTime modified, ObjectStatus status)
            {
                _Value = value;
                _Modified = modified;
                _Status = status;
            }

            ValueType _Value;
            DateTime _Modified;
            ObjectStatus _Status;

            public ObjectStatus Status
            {
                get { return _Status; }
                set { _Status = value; }
            }

            public ValueType Value
            {
                get { return _Value; }
                set { _Value = value; }
            }

            public DateTime Modified
            {
                get { return _Modified; }
                set { _Modified = value; }
            }
        }

        public CanDevice(IDevice device)
        {
            Id = device.Id;
            DeviceProfile = device.Profile;

            _Parameters = new Dictionary<ushort, Parameter>();
            foreach (DataObject parameter in device.ObjectDictionary)
            {
                _Parameters.Add(parameter.Index, 
                    new Parameter(parameter.TotalValue, parameter.Modified, parameter.Status));
            }
        }

        #region Device Information

        public readonly IProfile DeviceProfile;
        public readonly Guid Id;
        /// <summary>
        /// Возвращает или устанавливает сеть (контроллер сети), 
        /// которой пренадлежит данное устройство
        /// </summary>
        public readonly UInt32 NetworkId;

        Byte _NodeId;
        /// <summary>
        /// Сетевой идентификатор устройства 1...127
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Сетевые настройки")]
        [DisplayName("Адрес")]
        [Description("Сетевой идентификатор устройства")]
        //[DefaultValue(typeof(UInt16),"0")]
        //[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Byte NodeId { get { return _NodeId; } }

        DeviceType _DeviceType;
        /// <summary>
        /// Тип устройства
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Система")]
        [DisplayName("Тип устройства")]
        [Description("Тип устройства для сети CAN НГК-ЭХЗ")]
        public DeviceType DeviceType { get { return _DeviceType; } }

        UInt64 _SerialNumber;
        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Система")]
        [DisplayName("Серийный номер")]
        [Description("Серийный номер устройства")]
        public UInt64 SerialNumber { get { return _SerialNumber; } }

        DeviceStatus _Status;
        /// <summary>
        /// Статус устройства
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Система")]
        [DisplayName("Статус")]
        [Description("Текущее состояние устройства")]
        public DeviceStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        String _LocationName;
        /// <summary>
        /// Наименование географической точки установки оборудования  
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Сетевые настройки")]
        [DisplayName("Месторасположение")]
        [Description("Наименование географического места расположения КИП")]
        //[DefaultValue(typeof(UInt16),"0")]
        //[RefreshProperties(System.ComponentModel.RefreshProperties.All)] 
        public String LocationName { get { return _LocationName; } }

        UInt32 _PollingInterval;
        /// <summary>
        /// Возвращает время (сек) опроса устройства (равно времени 
        /// измерения и передачи данных у БИ(У))
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Интервал опроса, сек")]
        [Description("Период опроса устройства")]
        public UInt32 PollingInterval { get { return _PollingInterval; } }

        UInt16 _ElectrodeArea;
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Площадь электрода, кв.мм")]
        [Description("Площадь вспомогательного электрода, кв.мм")]
        public UInt16 ElectrodeArea { get { return _ElectrodeArea; } }
        
        Dictionary<UInt16, Parameter> _Parameters;
        /// <summary>
        /// Словарь объектов устройства
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Словарь объектов")]
        [Description("Словарь объектов устройства")]
        public Dictionary<UInt16, Parameter> Parameters
        {
            get { return _Parameters; }
        }

        #endregion
    }
}
