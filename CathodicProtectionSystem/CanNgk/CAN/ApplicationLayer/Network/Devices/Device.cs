using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary.Collections;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.Collections;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.DataTypes;
using Common.ComponentModel;

namespace NGK.CAN.ApplicationLayer.Network.Devices
{
    /// <summary>
    /// Класс Slave устройства от которого должны наследоваться 
    /// все классы Slave-устройств
    /// </summary>
    //[Serializable]
    public class DeviceBase : IDevice, IEmcyErrors//, ISerializable
    {
        #region Fields And Properties

        [NonSerialized]
        protected ObjectCollection _ObjectDictionary;
        [NonSerialized]
        private DeviceType _DeviceType;
        [NonSerialized]
        protected Byte _NodeId;
        [NonSerialized]
        protected DeviceStatus _Status;
        [NonSerialized]
        protected INetworkController _Network;
        [NonSerialized]
        protected String _LocationName;
        [NonSerialized]
        private UInt16 _ElectrodeArea = 65486;
        [NonSerialized]
        private UInt32 _PollingInterval;
        [NonSerialized]
        private bool _ConnectedServiceConnector;
        [NonSerialized]
        private bool _DuplicateAddressError; // Only for БИУ-01
        [NonSerialized]
        private bool _RegistrationError; // Only for БИУ-01
        [NonSerialized]
        private IProfile _Profile;
        /// <summary>
        /// Объект для синхронизации доступа разделяемым между потоками ресурсам
        /// </summary>
        [NonSerialized]
        protected static volatile Object _SyncRoot = new object();

        /// <summary>
        /// Словарь объектов устройства
        /// </summary>
        public ObjectCollection ObjectDictionary
        {
            get { return _ObjectDictionary; }
        }

        /// <summary>
        /// Тип устройства
        /// </summary>
        public DeviceType DeviceType
        {
            get { return _DeviceType; }
        }

        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        public UInt64 SerialNumber
        {
            get
            {
                //String msg;
                UInt64 serialNumber;

                serialNumber = (UInt64)(System.Convert.ToUInt64(
                    GetObject(0x2003)) << 32);
                serialNumber |= (UInt64)(System.Convert.ToUInt64(
                    GetObject(0x2004)) << 16);
                serialNumber |= System.Convert.ToUInt64(
                    GetObject(0x2005));
                return serialNumber;
            }
            set
            {
                String msg;
                Double max = Math.Pow(2, 48);

                if (value >= max)
                {
                    msg = String.Format(
                        "Попытка установить значение больше максимально допустимого {0}", max);
                    throw new ArgumentOutOfRangeException(msg);
                }
                unchecked
                {
                    SetObject(0x2005, (UInt16)(value));
                    SetObject(0x2004, (UInt16)(value >> 16));
                    SetObject(0x2003, (UInt16)(value >> 32));
                }

                // Пересчитвываем CRC16
                VisitingCard v = VisitingCard;
                ushort crc = v.GetCRC16();
                v.CRC16 = crc;
            }
        }
        
        /// <summary>
        /// Возвращает визитную карточку устройства НГК
        /// </summary>
        public VisitingCard VisitingCard
        {
            get { return new VisitingCard(this); }
        }

        /// <summary>
        /// Сетевой идентификатор устройства
        /// </summary>
        public Byte NodeId
        {
            get { return _NodeId; }
            set 
            {
                if ((value < 128) && (value > 0))
                {
                    _NodeId = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                        "NodeId", value, "Значение NodeId должно быть от 1 до 127");
                }
            }
        }

        /// <summary>
        /// Статус устройства
        /// </summary>
        public DeviceStatus Status
        {
            get 
            { 
                return _Status; 
            }
            set 
            {
                if (_Status != value)
                {
                    lock (this)
                    {
                        _Status = value;
                        // Генерирует событие
                        OnDeviceChangedStatus();
                    }
                }
            }
        }

        /// <summary>
        /// Сеть
        /// </summary>
        public INetworkController Network
        {
            get { return _Network; }
            set { _Network = value; }
        }

        /// <summary>
        /// Описание расположения устройства
        /// </summary>
        public String LocationName
        {
            get { return _LocationName; }
            set { _LocationName = value; }
        }

        /// <summary>
        /// Интервал опроса устройства
        /// </summary>
        public UInt32 PollingInterval
        {
            get { return _PollingInterval; }
            set { _PollingInterval = value; }
        }

        /// <summary>
        /// Площадь вспомогательного электрода
        /// </summary>
        public UInt16 ElectrodeArea
        {
            get { return _ElectrodeArea; }
            set 
            {
                if (value > 65534)
                {
                    throw new ArgumentOutOfRangeException(
                        String.Format("Попытка установить недопустимое значение параметру"));
                }
                _ElectrodeArea = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IProfile Profile
        {
            get { return _Profile; }
        }

        #region IEmcyErrors Members

        public bool Tamper
        {
            get
            {
                return (bool)GetObject(0x2015);
            }
            set
            {
                SetObject(0x2015, value);
            }
        }
  
        public bool MainSupplyPowerError
        {
            get
            {
                return (bool)GetObject(0x2016);
            }
            set
            {
                SetObject(0x2016, value);
            }
        }

        public bool BatteryError
        {
            get
            {
                return (bool)GetObject(0x2017);
            }
            set
            {
                SetObject(0x2017, value);
            }
        }

        public bool RegistrationError
        {
            get
            {
                return _RegistrationError;
            }
            set
            {
                _RegistrationError = value;
            }
        }

        public bool DuplicateAddressError
        {
            get
            {
                return _DuplicateAddressError;
            }
            set
            {
                _DuplicateAddressError = value;
            }
        }

        public bool ConnectedServiceConnector
        {
            get
            {
                return _ConnectedServiceConnector;
            }
            set
            {
                _ConnectedServiceConnector = value;
            }
        }

        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        private DeviceBase()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        internal DeviceBase(IProfile profile)
        {
            _Profile = profile;
            _LocationName = String.Empty;
            _NodeId = 1;
            _PollingInterval = 1;
            _Status = DeviceStatus.Stopped;
            // Создаём устройство на основе профиля
            _DeviceType = profile.DeviceType;
            // Создаём словарь объектов устройства
            _ObjectDictionary = new ObjectCollection(this);
            foreach (ObjectInfo info in profile.ObjectInfoList)
            {
                _ObjectDictionary.Add(new DataObject(info));
            }
            // Устанавливаем версии ПО и аппаратуры
            ProductVersion version = new ProductVersion();
            version.Version = profile.SoftwareVersion;
            SetObject(0x2001, version);
            version.Version = profile.HardwareVersion;
            SetObject(0x2002, version);

            // Пересчитвываем CRC16
            VisitingCard vc = VisitingCard;
            ushort crc = vc.GetCRC16();
            vc.CRC16 = crc;
        }
        /// <summary>
        /// Конструктор для десериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //protected Device(SerializationInfo info, StreamingContext context)
        //{
        //    _DeviceType = (DeviceType)info.GetValue(
        //        "DeviceType", typeof(DeviceType));
        //    _NodeId = info.GetByte("NodeId");
        //    _LocationName = info.GetString("Location");
        //    _PollingInterval = info.GetUInt32("PollingInterval");
        //    SerialNumber = info.GetUInt64("SerialNumber");

        //    _Status = DeviceStatus.Stopped;
        //    IProfile profile = Prototype.Create(_DeviceType);
        //    // Создаём словарь объектов устройства
        //    _ObjectDictionary = new ObjectCollection(this);
        //    foreach (ObjectInfo objinfo in profile.ObjectInfoList)
        //    {
        //        _ObjectDictionary.Add(new DataObject(objinfo));
        //    }
        //    // Устанавливаем версии ПО и аппаратуры
        //    ProductVersion version = new ProductVersion();
        //    version.Version = profile.SoftwareVersion;
        //    SetObject(0x2001, version);
        //    version.Version = profile.HardwareVersion;
        //    SetObject(0x2002, version);
        //}
        #endregion
        
        #region Methods
        /// <summary>
        /// Создаём устойство на основе строки в формате
        /// Type={0}; Network={1}; Address={2}; Location={3}; PollingInterval={4} 
        /// </summary>
        /// <param name="formatedString"></param>
        /// <returns></returns>
        public static DeviceBase Create(string formattedString)
        {
            DeviceBase device;

            Dictionary<string, string> parameters = 
                DeviceBase.GetParameters(formattedString);

            DeviceType type = (DeviceType)Enum.Parse(typeof(DeviceType), 
                parameters["Type"]);

            device = DeviceBase.Create(type);
            device._LocationName = parameters["Location"];
            device._NodeId = Byte.Parse(parameters["Address"]);
            device._PollingInterval = UInt32.Parse(parameters["PollingInterval"]);
            device.SerialNumber = UInt64.Parse(parameters["SerialNumber"]);

            UInt32 networkId = UInt32.Parse(parameters["NetworkId"]);

            // Ищем сеть с данным именем если она сущестует, у устанавливаем
            // свойство устройста
            if (NetworksManager.Instance.Networks.Contains(networkId))
            {
                device._Network = NetworksManager.Instance.Networks[networkId];            
            }

            return device;
        }
        /// <summary>
        /// Возвращает список прараметров из строки созданой в ToString()
        /// </summary>
        /// <param name="formatedString">Сериализованное в строку устройство</param>
        /// <returns>Таблица параметров устройства</returns>
        private static Dictionary<string,string> GetParameters(
            string formattedString)
        {
            string[] parameters;
            Dictionary<string, string> parametersTable;

            parametersTable = new Dictionary<string, string>();
            parameters = formattedString.Split(';');

            if (parameters.Length != 6)
            {
                throw new InvalidCastException("Неверный формат строки");
            }

            foreach (string parameter in parameters)
            {
                string[] pair = parameter.Split('=');

                if (pair.Length == 2)
                {
                    parametersTable.Add(pair[0].Trim(), pair[1].Trim());
                }
                else
                {
                    throw new InvalidCastException("Неверный формат строки");
                }
            }
            return parametersTable;
        }
        /// <summary>
        /// Создаёт устройство на основе профиля устройства
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static DeviceBase Create(IProfile profile)
        {
            return new DeviceBase(profile);
        }
        /// <summary>
        /// Создаёт устройство на основе типа устройства
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DeviceBase Create(DeviceType type)
        {
            IProfile profile = Prototype.Create(type);
            return new DeviceBase(profile);
        }
        /// <summary>
        /// Метод который вызывается до вызова конструктора 
        /// для десериализации Device(SerializationInfo info, StreamingContext context)
        /// </summary>
        /// <param name="context"></param>
        //[OnDeserializing]
        //private void OnDeserializing(StreamingContext context)
        //{
        //    // Устанавливаем статус устройства
        //    this._Status = DeviceStatus.Stopped;
        //}
        /// <summary>
        /// Метод генерирует событие DeviceChangedStatus
        /// </summary>
        private void OnDeviceChangedStatus()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = this.DeviceChangedStatus;

            if (handler != null)
            {
                foreach (EventHandler SingleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = SingleCast.Target as ISynchronizeInvoke;

                    try
                    {
                        if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        {
                            syncInvoke.Invoke(SingleCast, new Object[] { this, args });
                        }
                        else
                        {
                            SingleCast(this, args);
                        }
                    }
                    catch
                    { throw; }
                }
            }

            String traceMessage = String.Format(
                "{0}: Сеть {1}: Устройство NodeId {2}: Приняло новое состояние - {3}",
                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")),
                this._Network.Description, this._NodeId, this._Status);
            Trace.TraceInformation(traceMessage);

            return;
        }
        /// <summary>
        /// Метод генерирует событие DataWasChanged
        /// </summary>
        protected void OnDataWasChanged()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = this.DataWasChanged;

            if (handler != null)
            {
                foreach (EventHandler SingleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = SingleCast.Target as ISynchronizeInvoke;

                    try
                    {
                        if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        {
                            syncInvoke.Invoke(SingleCast, new Object[] { this, args });
                        }
                        else
                        {
                            SingleCast(this, args);
                        }
                    }
                    catch
                    { throw; }
                }
            }
            return;
        }
        /// <summary>
        /// Обработчик события изменения значения объекта объектного словаря устройства.
        /// </summary>
        /// <param name="sender">Словарь объектов устройства</param>
        /// <param name="e">Аргументы события</param>
        private void EventHandler_ObjectDictionary_ObjectChangedValue(
            object sender, EventArgs e)
        {
            // Генерируем событие
            OnDataWasChanged();
        }
        /// <summary>
        /// Отображает параметры устройства 
        /// КИП в котроле формы. Если данный контрол
        /// не поддерживается в реализации генерируется исключение
        /// </summary>
        /// <param name="widget">Контрол для отображения параметров устройства</param>
        /// <exception cref="InvalidOperationException">Генерируется если метод не поддерживает
        /// работу с контролом</exception>
        public virtual void Show(System.Windows.Forms.Control widget, Boolean readOnly)
        {
            String msg;
            msg = String.Format(
                "Метод Show устройства NodeId: {0} выполнил недопустимую операцию. "+
                "Виджет типа: {1} не поддеживается в текущей реализации",
                this.NodeId, widget);
            Trace.TraceError(msg);
            throw new NotImplementedException(msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //EnumTypeConverter converter =
            //    new EnumTypeConverter(typeof(DeviceType));
            //String str = converter.ConvertToString(DeviceType);

            return String.Format("Type={0}; NetworkId={1}; Address={2}; " +
                "Location={3}; PollingInterval={4}; SerialNumber={5}", 
                _DeviceType, _Network == null ? String.Empty : _Network.NetworkId.ToString(), 
                NodeId, _LocationName == null ? String.Empty : _LocationName, 
                PollingInterval, SerialNumber);
            //return base.ToString();
        }
        /// <summary>
        /// Возвращает значение объекта словаря с указанным индексом
        /// </summary>
        /// <param name="address">Индекс объекта</param>
        /// <returns>Значение объекта</returns>
        public ValueType GetObject(UInt16 index)
        {
            string msg;

            if (! _Profile.ObjectInfoList.Contains(index))
            {
                msg = String.Format("Network {0}: Устройство NodeId {1}: " +
                    "Не найдено описание объекта с индексом {2} в профиле",
                    _Network.Description, _NodeId, index);
                throw new InvalidOperationException(msg);
            }

            if (!_ObjectDictionary.Contains(index))
            {
                msg = String.Format("Network {0}: Устройство NodeId {1}: " +
                    "Не найден объект с индексом {2}",
                    _Network.Description, _NodeId, index);
                throw new InvalidOperationException(msg);
            }

            return _Profile.ObjectInfoList[index]
                .DataType.ConvertToTotalValue(ObjectDictionary[index].Value);
        }
        /// <summary>
        /// Устанавливает значение объекта словаря
        /// </summary>
        /// <param name="index">Индекс объекта</param>
        /// <param name="value">Новое значение</param>
        public void SetObject(UInt16 index, ValueType value)
        {
            string msg;

            if (!_Profile.ObjectInfoList.Contains(index))
            {
                msg = String.Format("Network {0}: Устройство NodeId {1}: " +
                    "Не найдено описание объекта с индексом {2} в профиле",
                    _Network.Description, _NodeId, index);
                throw new InvalidOperationException(msg);
            }

            if (!_ObjectDictionary.Contains(index))
            {
                msg = String.Format("Network {0}: Устройство NodeId {1}: " +
                    "Не найден объект с индексом {2}",
                    _Network.Description, _NodeId, index);
                throw new InvalidOperationException(msg);
            }

            ObjectDictionary[index].Value = _Profile.ObjectInfoList[index]
                .DataType.ConvertToBasis(value);
        }
        /// <summary>
        /// Проверяет содержит ли словарь объектов устройтсва
        /// указанный объект
        /// </summary>
        /// <param name="index">индекс объекта</param>
        /// <returns>true-содержит</returns>
        public Boolean ContainsObject(UInt16 index)
        {
            return _ObjectDictionary.Contains(index); 
        }
        #endregion
        
        #region Events
        /// <summary>
        /// Собтытие происходит при изменении состояния (статуса) устройства
        /// </summary>
        public event EventHandler DeviceChangedStatus;
        /// <summary>
        /// Событие происходит при изменении параметров объектного словаря
        /// или других параметров устройтсва
        /// </summary>
        public event EventHandler DataWasChanged;
        #endregion

        #region Члены IDevice
       
        event EventHandler IDevice.DeviceChangedStatus
        {
            add 
            {
                lock (_SyncRoot)
                {
                    this.DeviceChangedStatus += value;
                }
            }
            remove 
            {
                lock (_SyncRoot)
                {
                    this.DeviceChangedStatus -= value;
                }
            }
        }

        event EventHandler IDevice.DataWasChanged
        {
            add
            {
                lock (_SyncRoot)
                {
                    this.DataWasChanged += value;
                }
            }
            remove
            {
                lock (_SyncRoot)
                {
                    this.DataWasChanged -= value;
                }
            }
        }

        #endregion

        #region ISerializable
        /// <summary>
        /// Метод, вызываемый во время сериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //[SecurityPermissionAttribute(SecurityAction.Demand,
        //    SerializationFormatter = true)]
        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("DeviceType", _DeviceType);
        //    info.AddValue("NodeId", _NodeId);
        //    info.AddValue("Location", _LocationName);
        //    info.AddValue("PollingInterval", _PollingInterval);
        //    info.AddValue("SerialNumber", VisitingCard.SerialNumber);
        //    return;
        //}
        #endregion

    }
}
