using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Drawing.Design;
using NLog;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataLinkLayer.CanPort;
using NGK.CAN.ApplicationLayer.Network.Master.Services;
using NGK.CAN.ApplicationLayer.Network.Master.Services.Collections;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Collections;
using Common.Controlling;
using Common.Collections.ObjectModel;
//using NGK.CAN.OSIModel.ApplicationLayer.NetWork.Master.Design;

namespace NGK.CAN.ApplicationLayer.Network.Master
{
    /// <summary>
    /// Контроллер управления сетью CAN НГК-ЭХЗ
    /// </summary>
    [Serializable]
    public sealed class NetworkController : INetworkController, ISerializable
    {
        #region Fields And Properties
        //private static Logger _Logger = NLog.LogManager.GetLogger("NetworkLogger");
        /// <summary>
        /// Список имён существующих сетей. Необходим при создании новой сети, 
        /// для присвоения ей уникального имени по умолчанию, 
        /// или при проверке имени устанавливаемого пользователем. 
        /// </summary>
        [NonSerialized]
        private static List<UInt32> _RegisteredNetworks = new List<UInt32>();
        /// <summary>
        /// Возвращает список id существующих сетей.
        /// </summary>
        public static UInt32[] RegisteredNetworks
        {
            get { return _RegisteredNetworks.ToArray(); }
        }
        /// <summary>
        /// Уникальныей идентификатор сети
        /// </summary>
        [NonSerialized]
        private UInt32 _NetworkId;
        /// <summary>
        /// Уникальныей идентификатор сети
        /// </summary>
        public UInt32 NetworkId
        {
            get { return _NetworkId; }
            set 
            {
                // Проверяем имя сети. Если пристваивается не неуникальное имя, то
                // генерируется исключение
                if (_NetworkId != value)
                {
                    // Ищем совпадение имени
                    if (_RegisteredNetworks.Contains(value))
                    {
                        throw new ArgumentException(
                           "Попытка присвоить имя сети, которое уже существует",
                           "NetworkId");
                    }
                    else
                    {
                        // Удаляем старое имя из списка
                        _RegisteredNetworks.Remove(_NetworkId);
                        // Устанавливаем новое имя и помещаем его в список
                        _NetworkId = value;
                        _RegisteredNetworks.Add(value);
                    }
                }
            }
        }
        /// <summary>
        /// Описание сети
        /// </summary>
        [NonSerialized]
        private String _Description;
        /// <summary>
        /// Описание сети
        /// </summary>
        public String Description
        {
            get { return _Description; }
            set 
            {
                _Description = String.IsNullOrEmpty(value) ?
                    String.Empty : value;
            }
        }
        /// <summary>
        /// Состояние сетевого контроллера
        /// </summary>
        [NonSerialized]
        private Status _Status;
        /// <summary>
        /// Состояние сетевого контроллера
        /// </summary>
        public Status Status
        {
            get 
            { 
                return this._Status; 
            }
            set 
            { 
                //this._Status = value;
                switch (value)
                {
                    case Status.Running:
                        {
                            this.Start();
                            break; 
                        }
                    case Status.Stopped:
                        {
                            this.Stop();
                            break; 
                        }
                    case Status.Paused:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        { 
                            throw new NotImplementedException(); 
                        }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private Int32 _TotalAttempts = 1;
        /// <summary>
        /// Возваращает количесвто неудачных попыток к удалённому устройству, прежде чем
        /// сетевой сервис переведёт устройство в состояние аварии. Данный параметр применяется
        /// ко всем сетевым сервисам.
        /// </summary>
        /// <remarks>Должно настраивается при старте сервиса Start(). Сервис берёт значение из 
        /// конфигурационного файла</remarks>
        public Int32 TotalAttempts
        {
            get { return _TotalAttempts; }
            set 
            {
                if (value > 0)
                {
                    _TotalAttempts = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("TotalAttempts", "Кол-во попыток должно быть больше 0");
                }
            }
        }
        /// <summary>
        /// Период генерации сообщения SYNC в сеть, мсек
        /// </summary>
        public Double PeriodSync
        {
            get 
            {
                if (_NetworkServices.Contains(ServiceType.Sync))
                {
                    return ((ServiceSync)_NetworkServices[ServiceType.Sync]).PeriodSync;
                }
                else
                {
                    throw new Exception(
                        "Невозможно получить значение. В контроллере не найден сервис SYNC");
                }
            }
            set 
            {
                if (_NetworkServices.Contains(ServiceType.Sync))
                {
                    ((ServiceSync)_NetworkServices[ServiceType.Sync]).PeriodSync = value;
                }
                else
                {
                    throw new Exception(
                        "Невозможно установить значение. В контроллере не найден сервис SYNC");
                }
            }
        }
        /// <summary>
        /// Время последенй синхронизации сетевого времени
        /// </summary>
        public DateTime SynchronisationLastTime
        {
            get { return ((ServicePdoReceive)_NetworkServices[ServiceType.PdoReceive]).LastTimeSynchronisation; }
        }
        /// <summary>
        /// Период синхронизации сетевого времени (сек)
        /// </summary>
        public int SynchronisationInterval
        {
            get
            {
                return ((ServicePdoReceive)_NetworkServices[ServiceType.PdoReceive]).Interval;
            }
            set
            {
                ((ServicePdoReceive)_NetworkServices[ServiceType.PdoReceive]).Interval = value;
            }
        }
        /// <summary>
        /// CAN-порт для работы сервисов протокола
        /// </summary>
        [NonSerialized]
        private ICanPort _CanPort;
        /// <summary>
        /// CAN-порт для работы данной сети
        /// </summary>
        public ICanPort CanPort
        {
            set 
            {
                if (this.Status == Status.Stopped)
                {
                    this._CanPort = value;
                }
            }
            get { return _CanPort; }
        }
        /// <summary>
        /// Коллекция сетевых сервисов 
        /// </summary>
        [NonSerialized]
        private NetworkServicesCollection _NetworkServices;
       /// <summary>
        /// Список сетевых сервисов конроллера
        /// </summary>
        public NetworkServicesCollection Services
        {
            get { return _NetworkServices; }
        }
        /// <summary>
        /// Список устройств в сети.
        /// </summary>
        [NonSerialized]
        private DevicesCollection _DevicesList;
        /// <summary>
        /// Список сетевых устройств
        /// </summary>
        public DevicesCollection Devices
        {
            get { return _DevicesList; }
        }
        /// <summary>
        /// Поток для обработки исходящих от сервисов сообщений
        /// </summary>
        [NonSerialized]
        private Thread _ThreadMsgSender;
        /// <summary>
        /// Объект для синхронизации межпотоковых операций
        /// </summary>
        [NonSerialized]
        private static Object _SyncRoot = new Object();
        [NonSerialized]
        private EventHandler DeviceChangedData;
        [NonSerialized]
        private EventHandler DeviceChangedStatus; 
        #endregion

        #region Constructors And Destructor
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public NetworkController()
        {
            _Status = Status.Stopped;
            // По умолчанию физического уровня нет.
            _CanPort = null;
            
            // Ищем уникальное имя и когда находим его, присваиваем сети
            lock (_SyncRoot)
            {
                _NetworkId = CreateNetwrokId();
            }

            _Description = String.Format("CanNetworkController{0}", _NetworkId);

            DeviceChangedData = 
                new EventHandler(EventHandlerDeviceChangedValue);
            DeviceChangedStatus = 
                new EventHandler(EventHandlerDeviceChangedStatus);

            // Инициализируем спусок устройств в сети
            _DevicesList = new DevicesCollection(this);
            _DevicesList.CollectionWasChanged +=
                new EventHandler<KeyedCollectionWasChangedEventArgs<Device>>
                (EventHandler_DevicesListWasChanged);

            // Инициализируем список сетевых сервисов
            this.InitNetworkServices();
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="port">CAN-порт</param>
        /// <param name="NetworkId">Наименование сети</param>
        public NetworkController(ICanPort port, UInt32 networkId)
        {
            _Status = Status.Stopped;
            NetworkId = networkId;
            _Description = String.Format("CanNetworkController{0}", NetworkId);
            
            this._CanPort = port;

            if (this._CanPort != null)
            {
                if (this._CanPort.IsOpen)
                {
                    this._CanPort.Close();
                }
            }
            
            DeviceChangedData =
                new EventHandler(EventHandlerDeviceChangedValue);
            DeviceChangedStatus =
                new EventHandler(EventHandlerDeviceChangedStatus);

            // Инициализируем спусок устройств в сети
            _DevicesList = new DevicesCollection(this);
            _DevicesList.CollectionWasChanged +=
                new EventHandler<KeyedCollectionWasChangedEventArgs<Device>>
                (EventHandler_DevicesListWasChanged);

            // Инициализируем список сетевых сервисов
            InitNetworkServices();

            _CanPort.MessageReceived += 
                new EventHandler(EventHandler_CanPort_MessageReceived);
            //_CanPort.PortChangedStatus +=
            //    new EventHandlerPortChangesStatus(EventHandler_CanPort_PortChangesStatus);
            //_CanPort.ErrorReceived += 
            //    new EventHandlerErrorRecived(EventHandler_CanPort_ErrorReceived);
        }

        /// <summary>
        /// Конструктор для десериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private NetworkController(SerializationInfo info, StreamingContext context)
        {
            Device device;

            // Восстанавливаем сохранённые параметры
            _NetworkId = info.GetUInt32("NetworkId");
            _Description = info.GetString("Description");
            _CanPort = (ICanPort)info.GetValue("CanPort", typeof(ICanPort));
            if (_CanPort != null)
            {
                _CanPort.Stop();
                _CanPort.MessageReceived +=
                    new EventHandler(EventHandler_CanPort_MessageReceived);
                //_CanPort.PortChangedStatus +=
                //    new EventHandlerPortChangesStatus(EventHandler_CanPort_PortChangesStatus);
                //_CanPort.ErrorReceived +=
                //    new EventHandlerErrorRecived(EventHandler_CanPort_ErrorReceived);
            }
            _TotalAttempts = info.GetInt32("TotalAttempts");
            
            // Восстанавливаем список устройств
            _DevicesList = new DevicesCollection(this);

            List<string> list =
                (List<string>)info.GetValue("Devices", typeof(List<string>));

            foreach (string str in list)
            {
                device = Device.Create(str);
                _DevicesList.Add(device);
            }
            // Запускаем сетевые сервисы
            InitNetworkServices();

            // Настройки сетевых сервисов
            ((ServicePdoReceive)_NetworkServices[ServiceType.PdoReceive]).Interval =
                info.GetInt32("PdoReceiveInterval");
            ((ServiceSync)_NetworkServices[ServiceType.Sync]).PeriodSync =
                info.GetDouble("SyncPeriodSync");
            foreach (Service service in _NetworkServices)
            {
                service.TotalAttempts = _TotalAttempts;
            }

            DeviceChangedData =
                new EventHandler(EventHandlerDeviceChangedValue);
            DeviceChangedStatus =
                new EventHandler(EventHandlerDeviceChangedStatus);            
        }
        /// <summary>
        /// Деструктор класса.
        /// </summary>
        ~NetworkController()
        {
            Dispose();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Генерирует уникальный идетификатор для сети
        /// в диапазоане значений 1...255
        /// </summary>
        /// <returns></returns>
        private static UInt32 CreateNetwrokId()
        {
            for (UInt32 i = 1; i < 0xFF; i++)
            {
                // Ищем уникальное имя и когда находим его, присваиваем сети
                if (!_RegisteredNetworks.Contains(i))
                {
                    return i;
                }
            }
            throw new Exception(
                "Не удалось содать уникальный идентификатор сети.");
        }
        /// <summary>
        /// Обработчик события изменения списка устройств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DevicesListWasChanged(
            object sender, KeyedCollectionWasChangedEventArgs<Device> e)
        {
            switch (e.Action)
            {
                case Action.Adding:
                    {
                        // Если добавляется устройство подключем его события
                        //e.Item.DataWasChanged += DeviceChangedData;
                        //e.Item.DeviceChangedStatus += DeviceChangedData;
                        break;
                    }
                case Action.Removing:
                    {
                        // Если удаляется устройство отключаем его события
                        //e.Item.DataWasChanged -= DeviceChangedData;
                        //e.Item.DeviceChangedStatus -= DeviceChangedStatus;
                        break;
                    }
            }
        }
        /// <summary>
        /// Обработчик события изменения статуса устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandlerDeviceChangedStatus(object sender, EventArgs e)
        {
            return;
        }
        /// <summary>
        /// Обработчик события изменения данных устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandlerDeviceChangedValue(object sender, EventArgs e)
        {
            return;
        }
        /// <summary>
        /// Вызывается перед десериализацией (перед вызовом конструктора 
        /// NetworkController(SerializationInfo info, StreamingContext context))
        /// </summary>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            // По умолчанию
            this._Status = Status.Stopped;
            return;
        }
        /// <summary>
        /// Вызывается после десериализации (после вызова конструктора 
        /// NetworkController(SerializationInfo info, StreamingContext context))
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {   
            return;
        }
        /// <summary>
        /// Инициализация сетевых сервисов контроллера
        /// </summary>
        private void InitNetworkServices()
        {
            Service service;

            // Инициализируем список сетевых сервисов
            this._NetworkServices = new NetworkServicesCollection(this);

            service = new ServiceBootUp(this);
            service.TotalAttempts = this.TotalAttempts;
            this._NetworkServices.Add(service);

            service = new ServiceNodeGuard(this);
            service.TotalAttempts = this.TotalAttempts;
            this._NetworkServices.Add(service);

            service = new ServiceNmt(this);
            service.TotalAttempts = this.TotalAttempts;
            this._NetworkServices.Add(service);

            service = new ServiceSdoUpload(this);
            service.TotalAttempts = this.TotalAttempts;
            this._NetworkServices.Add(service);

            service = new ServicePdoTransmit(this);
            service.TotalAttempts = this.TotalAttempts;
            this._NetworkServices.Add(service);

            service = new ServiceSync(this, 5000);
            service.TotalAttempts = this.TotalAttempts;
            this._NetworkServices.Add(service);

            service = new ServicePdoReceive(this, 5);
            service.TotalAttempts = this.TotalAttempts;
            this._NetworkServices.Add(service);

            return;
        }
        /// <summary>
        /// Возвращает количество устройств в сети с указанным статусом 
        /// </summary>
        /// <param name="status">Статус устройства</param>
        /// <returns>Количество устройств с указанным статусом</returns>
        public int GetCountOfDevicesByStatus(DeviceStatus status)
        {
            int count = 0;

            foreach (IDevice device in this._DevicesList)
            {
                if (device.Status == status)
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// Запускает работу сети CAN НГК-ЭХЗ (запускает 
        /// работу сетевых сервисов)
        /// </summary>
        public void Start()
        {
            if (_CanPort != null)
            {
                if (_Status != Status.Running)
                {
                    // Открываем порт.
                    if (!_CanPort.IsOpen)
                    {
                        _CanPort.Open();

                        switch (_CanPort.PortStatus)
                        {
                            case CanPortStatus.IsActive:
                                { break; }
                            case CanPortStatus.IsClosed:
                                { throw new Exception("NetworkController.StartNetwork(): Невозможно выполнить. Порт закрыт"); }
                            case CanPortStatus.IsPassive:
                                { 
                                    _CanPort.Start(); 
                                    break; 
                                }
                            case CanPortStatus.IsPassiveAfterReset:
                                { 
                                    _CanPort.Start(); 
                                    break; }
                            case CanPortStatus.Unknown:
                                {
                                    throw new Exception(
                                        "NetworkController.StartNetwork(): Невозможно выполнить. Порт имеет неопределённый статус");
                                }
                            default:
                                {
                                    throw new Exception(
                                        "NetworkController.StartNetwork(): Невозможно выполнить. Статус порта не поддерживается в данной верисии ПО");
                                }
                        }
                    }
                    else
                    {
                        if (_CanPort.PortStatus != CanPortStatus.IsActive)
                        {
                            _CanPort.Start();
                        }
                    }

                    // Создаём рабочий поток сервиса на приём сообщений из сети

                    if (_ThreadMsgSender == null)
                    {
                        _ThreadMsgSender = new Thread(SendOutcomingMessagesToCanPort);
                        _ThreadMsgSender.IsBackground = true;
                        _ThreadMsgSender.Name = String.Format(
                            "Controller_{0}_MessageSender", _Description);
                    }

                    // Устанавливаем статус
                    _Status = Status.Running;

                    // Запускаем доступные сервисы
                    foreach (Service service in _NetworkServices)
                    {
                        // !!! Для отладки будем управляеть работой сервисо в ручную
                        //service.Start();
                    }
                    
                    //_NetworkServices[ServiceType.NodeGuard].Start();
                    //_NetworkServices[ServiceType.Nmt].Start();
                    //_NetworkServices[ServiceType.BootUp].Start();
                    //_NetworkServices[ServiceType.Sync].Start();
                    //_NetworkServices[ServiceType.PdoReceive].Start();
                    //_NetworkServices[ServiceType.PdoTransmit].Start();
                    //_NetworkServices[ServiceType.SdoUpload].Start();
                    
                    // Запускаем поток (он работает в цикле)
                    _ThreadMsgSender.Start();
                    
                    

                    // Генерируем событие
                    OnControllerChangedStatus();
                }
                else
                {
                    // Ничего не далаем, контроллер уже запущен
                }
            }
            else
            {
                // Отсутстует CAN-порт (физический уровень)
                throw new InvalidOperationException(
                    "Невозможно выполнить метод StartNetwork, отсутствует объект ICanPort");
            }
            return;
        }
        /// <summary>
        /// Останавливает работу сети CAN НГК-ЭХЗ (останавливает 
        /// работу сервисов сети) 
        /// </summary>
        public void Stop()
        {
            string msg;

            if (this._Status != Status.Stopped)
            {
                // Останавливаем работу порта.
                if (_CanPort != null)
                {
                    _CanPort.Close();
                    //this._CanPort.Stop();
                }

                // Останавливаем поток на обработку исходящих сообщений
                //Interlocked.Exchange(ref _FlgSendingProcessStop, 0);
                lock (_SyncRoot)
                {
                    _Status = Status.Stopped;
                }

                // Останавливаем доступные сервисы
                foreach (Service service in _NetworkServices)
                {
                    service.Stop();
                }

                // Устанавливаем статус всем устройствам в сети
                foreach (Device device in _DevicesList)
                {
                    device.Status = DeviceStatus.Stopped;
                }
                // Генерируем событие
                OnControllerChangedStatus();

                // Ждём завершения потока. Если поток не завершится за заданное
                // время, то убиваем его                                
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(50);

                    if (_ThreadMsgSender.IsAlive == false)
                    {
                        // Поток завершился :) ура!
                        break;
                    }
                }

                // Поток не завершился за заданное время. Убиваем его
                if (_ThreadMsgSender.IsAlive == true)
                {
                    _ThreadMsgSender.Abort();
                    _ThreadMsgSender = null;
                    msg = String.Format(
                        "Network {0}: Stop() - Поток для обработки очереди исходящих " +
                        "сообщений не смог завершиться за заданное время", _Description);
                    //Logger.Warn(msg);
                }

                _ThreadMsgSender = null;
            }
        }
        /// <summary>
        /// Приостанавливает работу сервиса
        /// </summary>
        public void Suspend()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Обработчик события изменения состояния CAN-порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandler_CanPort_PortChangesStatus(object sender,
            EventArgsPortChangesStatus args)
        {
            String traceMessage;

            switch (args.Status)
            {
                case CanPortStatus.IsActive:
                    {
                        traceMessage = String.Format(
                            "{0}: Сеть {1}: CAN-порт изменил свое состояние на {2}",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")),
                            this.Description, CanPortStatus.IsClosed);
                        Trace.TraceInformation(traceMessage);

                        // СAN-порт в активном состоянии можно работать
                        //this.Start();
                        break;
                    }
                case CanPortStatus.IsClosed:
                    {
                        traceMessage = String.Format(
                            "{0}: Сеть {1}: CAN-порт изменил свое состояние на {2}",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")),
                            this.Description, CanPortStatus.IsClosed);
                        Trace.TraceInformation(traceMessage);

                        // CAN-порт закрыт, останавливаем службы
                        this.Stop();
                        break;
                    }
                case CanPortStatus.IsPassive:
                    {
                        traceMessage = String.Format(
                            "{0}: Сеть {1}: CAN-порт изменил свое состояние на {2}",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")),
                            this.Description, CanPortStatus.IsClosed);
                        Trace.TraceInformation(traceMessage);

                        // CAN-порт закрыт, останавливаем службы
                        this.Stop();
                        break;
                    }
                case CanPortStatus.IsPassiveAfterReset:
                    {
                        traceMessage = String.Format(
                            "{0}: Сеть {1}: CAN-порт изменил свое состояние на {2}",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")),
                            this.Description, CanPortStatus.IsClosed);
                        Trace.TraceInformation(traceMessage);

                        // CAN-порт закрыт, останавливаем службы
                        this.Stop();
                        break;
                    }
                case CanPortStatus.Unknown:
                    {
                        traceMessage = String.Format(
                            "{0}: Сеть {1}: CAN-порт изменил свое состояние на {2}",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")),
                            this.Description, CanPortStatus.IsClosed);
                        Trace.TraceInformation(traceMessage);

                        // CAN-порт закрыт, останавливаем службы
                        this.Stop();
                        break;
                    }
                default:
                    {
                        traceMessage = String.Format(
                            "{0}: Сеть {1}: CAN-порт изменил свое состояние на {2}",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")),
                            this.Description, CanPortStatus.IsClosed);
                        Trace.TraceError(traceMessage);

                        // CAN-порт закрыт, останавливаем службы
                        this.Stop();

                        throw new InvalidOperationException(traceMessage);
                    }
            }
            return;
        }
        /// <summary>
        /// Обработчик события аварийной ситуации от CAN-порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        private void EventHandler_CanPort_ErrorReceived(object sender,
            EventArgsLineErrorRecived error)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Обработчик события приёма сообщения от CAN-порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_CanPort_MessageReceived(object sender, EventArgs e)
        {
            // Запускаем поток на обработку принятых сообщений и рассылки
            // их сетевым сервисам
            //this._ThreadMsgListener.Start();
            SendIncomingMessagesToServices();
            return;
        }
        /// <summary>
        /// Метод запускается в фоновом потоке и выполняет основную работу контроллера
        /// сервисов протокола НГК-ЭХЗ. Принимает сообщения от CAN - порта и рассылает
        /// их по сервисам. А также управляет работой (состоянием) сетевых сервисов
        /// </summary>
        private void SendIncomingMessagesToServices()
        {
            String msg;
            Frame message;
            List<Frame> messages, copies;


            if ((_CanPort == null) || 
                (_CanPort.PortStatus != CanPortStatus.IsActive))
            {
                msg = String.Format(
                    "Network {0}: NetworkController не запущен - CAN-порт статус {1}", this._Description, 
                    _CanPort == null ? "nullreference" : _CanPort.PortStatus.ToString());
                throw new InvalidOperationException(msg);  
            }
            
            lock(_SyncRoot)
            {
                messages = new List<Frame>();

                while(_CanPort.MessagesToRead != 0)
                {
                    if (_CanPort.Read(out message))
                    {
                        messages.Add(message);
                    }
                }
            }
            
            foreach (Service service in _NetworkServices)
            {
                // Клонируем сообщения 
                copies = new List<Frame>();

                foreach(Frame frame in messages)
                {
                    copies.Add((Frame)frame.Clone());
                }
                
                // Записываем его во входной буфер сервиса
                lock (_SyncRoot)
                {
                    if (service.Status == Status.Running)
                    {
                        service.HandleIncomingMessages(copies.ToArray());
                    }    
                }
            }
        }
        /// <summary>
        /// Метод запускается в фоновом потоке и вызываем метод DoWork у
        /// сетевых сервисов, которые при этом формируют запросы в сеть
        /// </summary>
        private void SendOutcomingMessagesToCanPort()
        {
            while (_Status == Status.Running)
            {
                foreach (Service service in _NetworkServices)
                {
                    service.HandleOutcomingMessages();
                }
            }
        }
        /// <summary>
        /// Отправить сообщение в сеть
        /// </summary>
        public void SendMessageToCanPort(Frame outcomingMessage)
        {
            string msg;

            if ((_CanPort == null) || (_CanPort.PortStatus != CanPortStatus.IsActive))
            {
                msg = String.Format(
                    "Network {0}: NetworkController не запущен - CAN-порт статус {1}", this._Description,
                    _CanPort == null ? "nullreference" : _CanPort.PortStatus.ToString());
                throw new InvalidOperationException(msg);
            }
            
            _CanPort.Write(outcomingMessage);                    
        }
        /// <summary>
        /// 
        /// </summary>
        private void OnControllerChangedStatus()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = this.ControllerChangedStatus;

            if (handler != null)
            {
                foreach (EventHandler SingleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke = 
                        SingleCast.Target as System.ComponentModel.ISynchronizeInvoke;

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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Возвращаем наименование сети
            return String.Format("Type={0}; NetworkName={1}; PortName={2}", 
                "Master", _Description, 
                _CanPort == null ? "None": _CanPort.PortName);
            //return base.ToString();
        }
        #endregion
        
        #region Events

        /// <summary>
        /// Событие происходит при изменении состояния контроллера
        /// </summary>
        public event EventHandler ControllerChangedStatus;
        
        #endregion

        #region Members of IManageable

        event EventHandler IManageable.StatusWasChanged
        {
            add 
            {
                lock (_SyncRoot)
                {
                    this.ControllerChangedStatus += value;
                }
            }
            remove 
            {
                lock (_SyncRoot)
                {
                    this.ControllerChangedStatus -= value;
                }
            }
        }

        #endregion

        #region Члены ISerializable
        /// <summary>
        /// Метод, вызываемый во время сериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NetworkId", _NetworkId);
            info.AddValue("Description", _Description);
            info.AddValue("TotalAttempts", _TotalAttempts);
            info.AddValue("CanPort", _CanPort);
            //info.AddValue("Devices", _DevicesList);
            info.AddValue("PdoReceiveInterval", 
                ((ServicePdoReceive)_NetworkServices[ServiceType.PdoReceive]).Interval);
            info.AddValue("SyncPeriodSync", PeriodSync);
            
            // Сериализуем устройства
            List<string> list = new List<string>();
            
            foreach (Device device in Devices)
            {
                list.Add(device.ToString());
            }
            
            info.AddValue("Devices", list);

            return;
        }
        #endregion

        #region Members of IDisposable
        /// <summary>
        /// Вызывается пользователь для освобождения 
        /// ресурсов перед удалением объекта
        /// </summary>
        public void Dispose()
        {
            Stop();
            // Удаляем имя сети из списка имён сущестующих сетей.
            _RegisteredNetworks.Remove(NetworkId);
            return;
        }
        #endregion
    }
}
