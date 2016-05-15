using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Runtime.Serialization;
using Ixxat.Vci3;
using Ixxat.Vci3.Bal;
using Ixxat.Vci3.Bal.Can;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataLinkLayer.CanPort;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.IXXAT
{
    [Serializable]
    [Description("CAN-порт IXXAT")]
    public class CanPort: CanPortBase //ICanPort
    {
        #region Fields And Properties
        //#if DEBUG
        //private OutputWindow WindowsDebugging;
        //#endif
        /// <summary>
        /// Фильтр для сообщений, применяется только при инициализации устройства. 
        /// Если null - принимаются все сообщния
        /// </summary>
        //private FilterIds _MessagesFilter;
        /// <summary>
        /// Фильтр для сообщений, применяется только при инициализации устройства. 
        /// Если null - принимаются все сообщния
        /// </summary>
        //public FilterIds MessagesFilter
        //{
        //    get { return _MessagesFilter; }
        //    set 
        //    {
        //        // Проверяем корректность фильтра
        //        // Фитльтр работает только со станадратными кадрами или с расширенными,
        //        // смешанный формат не допустим
        //        if (!((value.Format == FRAMEFORMAT.StandardFrame) || (value.Format == FRAMEFORMAT.ExtendedFrame)))
        //        {
        //            _MessagesFilter = null;
        //            throw new ArgumentException();
        //        }
        //        else
        //        {
        //            _MessagesFilter = value;
        //        }
        //    }
        //}
        /// <summary>
        /// Наименование CAN-порта (Серийный номер устройства)
        /// </summary>
        private String _SerialNumber;
        /// <summary>
        /// Наименование CAN-порта (Серийный номер устройства)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Наименование порта")]
        [Description("Обозначение CAN-порта, совпадает с его серийным номером")]
        public override String PortName
        {
            get { return _SerialNumber; }
            set 
            {
                if (this.IsOpen)
                {
                    throw new InvalidOperationException("");
                }
                else
                {
                    IVciDeviceList deviceList;
                    String serialNumber;
                    Object pack;

                    _SerialNumber = value;

                    // Получаем список доступных устройств
                    deviceList = GetDeviceList();
                    // Находим нужное нам устройство и открываем его
                    foreach (IVciDevice item in deviceList)
                    {
                        pack = (Object)item.UniqueHardwareId;
                        serialNumber = GetSerialNumber(ref pack);

                        if (serialNumber == this._SerialNumber)
                        {
                            // Устройство найдено
                            this._CanDevice = item;
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Физическое устройство драйвера CAN
        /// </summary>
        [NonSerialized]
        private IVciDevice _CanDevice;
        /// <summary>
        /// Reference to the CAN message communication channel.
        /// </summary>
        [NonSerialized]
        private ICanChannel _CanChannel;
        /// <summary>
        /// Reference to the message writer of the CAN message channel.
        /// </summary>
        [NonSerialized]
        private ICanMessageWriter _Writer;
        /// <summary>
        /// Reference to the message reader of the CAN message channel.
        /// </summary>
        [NonSerialized]
        private ICanMessageReader _Reader;
        /// <summary>
        /// Thread that handles the message reception.
        /// </summary>
        [NonSerialized]
        private Thread _ThreadForInput;
        /// <summary>
        /// Reference to the CAN controller.
        /// </summary>
        [NonSerialized]
        private ICanControl _CanController;
        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        private IBalObject _CanBusLayer;
        /// <summary>
        /// Event that's set if at least one message was received.
        /// </summary>
        [NonSerialized]
        private AutoResetEvent _RxEvent;
        /// <summary>
        /// Quit flag for the receive thread.
        /// </summary>
        [NonSerialized]
        private int _FlagMustQuit = 0;
        /// <summary>
        /// Скорость передачи данных.
        /// </summary>
        private BaudRate _CanBitRate;
        /// <summary>
        /// Скорость передачи данных.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Скорость обмена")]
        [Description("Скорость обмена данными на шине")]
        public override BaudRate BitRate
        {
            get
            {
                return this._CanBitRate;
            }
            set
            {
                this._CanBitRate = value;
            }
        }
        /// <summary>
        /// Входной буфер сообщений
        /// </summary>
        [NonSerialized]
        private Queue<Frame> _InputBufferMessages;
        /// <summary>
        /// Количество принятых сообщений во входном буфере порта 
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Состояние")]
        [DisplayName("Входящие сообщения")]
        [Description("Количество принятых сообщений во воходном буфере порта")]
        public override int MessagesToRead
        {
            get 
            {
                return _InputBufferMessages.Count;
            }
        }
        /// <summary>
        /// Формат кадров (поле ID: 11 или 29 бит) с которыми работает порт
        /// могут складыватся по "ИЛИ"
        /// </summary>
        private FrameFormat _FrameFormat = FrameFormat.StandardFrame;
        /// <summary>
        /// Формат кадров (поле ID: 11 или 29 бит) с которыми работает порт
        /// могут складыватся по "ИЛИ"
        /// </summary>
        /// <value>
        /// Значение по умолчанию FRAMEFORMAT.STANDARDFRAME
        /// </value>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Формат кадра")]
        [Description("Формат кадра сообщения по CAN-шине")]
        public override FrameFormat FrameFormat
        {
            get { return _FrameFormat; }
            set { _FrameFormat = value; }
        }
        /// <summary>
        /// Разрешить приём сообщения Error Frame
        /// </summary>
        private Boolean _ErrorFrameEnable = true;
        /// <summary>
        /// Разрешить приём сообщения Error Frame
        /// </summary>
        /// <value>
        /// Значение по умолчанию true 
        /// </value>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Сообщения об ошибках")]
        [Description("Включает/отключает передачу сообщений об ошибках от драйвера порта к приложению пользователя")]
        public override Boolean ErrorFrameEnable
        {
            get { return _ErrorFrameEnable; }
            set { _ErrorFrameEnable = value; }
        }
        /// <summary>
        /// Разрешить режим монитора сети: Tx passive
        /// </summary>
        private Boolean _ListenOnlyMode = false;
        /// <summary>
        /// Разрешить режим монитора сети: Tx passive
        /// </summary>
        /// <value>
        /// Значение по умолчанию: отключен (false)
        /// </value>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Только слушать")]
        [Description("Разрешает работу CAN-порта, в режиме прослушиванию шины. Порт не учавствует в работе шины")]
        public Boolean ListenOnlyMode
        {
            get { return _ListenOnlyMode; }
            set { _ListenOnlyMode = value; }
        }
        /// <summary>
        /// Разрешить низкоскоростной режим работы CAN-интерфейса
        /// ????????
        /// </summary>
        private Boolean _LowSpeedModeEnable = false;
        /// <summary>
        /// Разрешить низкоскоростной режим работы CAN-интерфейса
        /// ????????
        /// </summary>
        /// <value>
        /// Значение по умолчанию: отключен (false)</value>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Режим LowSpeed")]
        [Description("Разрешиает или запрещает работу CAN-порта в режиме Low Speed")]
        public Boolean LowSpeedModeEnable
        {
            get { return _LowSpeedModeEnable; }
            set { _LowSpeedModeEnable = value; }
        }
        /// <summary>
        /// Глобальное состояние порта.
        /// </summary>
        [NonSerialized]
        private CanPortStatus _PortStatus = CanPortStatus.IsClosed;
        /// <summary>
        /// Если порт открыт возвращает true. Однако, при этом, порт может находится
        /// в состоянии инициализации.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Состояние")]
        [DisplayName("Открыт")]
        [Description("Состояние CAN-порта: открыт (true) или закрыт (false)")]
        public override Boolean IsOpen
        {
            get 
            {
                if (_PortStatus != CanPortStatus.IsClosed)
                { return true; }
                else
                { return false; }
            }
        }
        /// <summary>
        /// Текущее состояние CAN-порта
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Состояние")]
        [DisplayName("Состояние CAN-порта")]
        [Description("Состояние CAN-порта")]
        public override CanPortStatus PortStatus
        {
            get { return _PortStatus; }
        }
        /// <summary>
        /// Возваращает описание аппаратно-программного коплекса CAN-порта
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Адаптер порта")]
        [DisplayName("Информация")]
        [Description("Информация об адаптере CAN-порта")]
        public DeviceInfo PortInfo
        {
            get 
            {
                if (this._CanDevice != null)
                {
                    return ConvertToDeviceInfo(this._CanDevice);
                }
                else
                {
                    return new DeviceInfo();
                }
            }
        }
        /// <summary>
        /// Для блокировки данных при межпотоковых вызовах (синхронизация данных)
        /// </summary>
        private static Object _SyncRoot = new Object();

        #endregion

        #region Constructors And Destuctor
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public CanPort()
        {
            //#if DEBUG
            //WindowsDebugging = new OutputWindow(String.Format("CAN-Port {0}", this.PortName));
            //WindowsDebugging.Show();
            //#endif
            // Настраиваем поля 
            // Инициализируем входной буфер сообщений
            this._InputBufferMessages = new Queue<Frame>(100);
            // Инициализируем свойства порта
            this._FrameFormat = FrameFormat.StandardFrame;
            this._ErrorFrameEnable = true;
            this._ListenOnlyMode = false;
            this._LowSpeedModeEnable = false;
            // Устанавливаем скорость по умолчанию
            this._CanBitRate = BaudRate.BR10;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="portName">Наименование порта (серийный номер порта)</param>
        public CanPort(String portName)
        {
            IVciDeviceList deviceList;
            String serialNumber;
            Object pack;

            // Настраиваем поля 
            this.PortName = portName;
            // Инициализируем входной буфер сообщений
            this._InputBufferMessages = new Queue<Frame>(100);
            // Инициализируем свойства порта
            this._FrameFormat = FrameFormat.StandardFrame;
            this._ErrorFrameEnable = true;
            this._ListenOnlyMode = false;
            this._LowSpeedModeEnable = false;
            // Устанавливаем скорость по умолчанию
            this._CanBitRate = BaudRate.BR10;

            // Получаем список доступных устройств
            deviceList = GetDeviceList();
            // Находим нужное нам устройство и открываем его
            foreach (IVciDevice item in deviceList)
            {
                pack = (Object)item.UniqueHardwareId;
                serialNumber = GetSerialNumber(ref pack);

                if (serialNumber == this._SerialNumber)
                {
                    // Устройство найдено
                    this._CanDevice = item;
                    break;
                }
            }
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="portName">Наименование порта (серийный номер порта)</param>
        public CanPort(String portName, BaudRate bitRate, FrameFormat frameFormat,
            PortMode mode)
        {
            IVciDeviceList deviceList;
            String serialNumber;
            Object pack;

            // Настраиваем поля 
            this.PortName = portName;
            // Инициализируем входной буфер сообщений
            this._InputBufferMessages = new Queue<Frame>(100);
            // Инициализируем свойства порта
            this._FrameFormat = frameFormat;
            this._ErrorFrameEnable = true;
            ((ICanPort)this).Mode = mode;
            this._LowSpeedModeEnable = false;
            // Устанавливаем скорость по умолчанию
            this._CanBitRate = bitRate;

            // Получаем список доступных устройств
            deviceList = GetDeviceList();
            // Находим нужное нам устройство и открываем его
            foreach (IVciDevice item in deviceList)
            {
                pack = (Object)item.UniqueHardwareId;
                serialNumber = GetSerialNumber(ref pack);

                if (serialNumber == this._SerialNumber)
                {
                    // Устройство найдено
                    this._CanDevice = item;
                    break;
                }
            }
        }
        /// <summary>
        /// Конструктор для десериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public CanPort(SerializationInfo info, StreamingContext context)
        {
            this.PortName = info.GetString("PortName");
            this._InputBufferMessages = new Queue<Frame>(100);

            this._FrameFormat = (FrameFormat)info.GetValue("FrameFormat", typeof(FrameFormat));
            this._ErrorFrameEnable = info.GetBoolean("ErrorFrameEnable");
            this._ListenOnlyMode = info.GetBoolean("ListenOlnyMode");
            this._CanBitRate = (BaudRate)info.GetValue("BitRate", typeof(BaudRate));
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Деструктор порта
        /// </summary>
        ~CanPort()
        {
            this.Dispose();
        }
        //--------------------------------------------------------------------------------
        #endregion
        
        #region Methods
        /// <summary>
        /// Метод вызывается после конструктора десериализации 
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            IVciDeviceList deviceList;
            String serialNumber;
            Object pack;

            // Получаем список доступных устройств
            deviceList = GetDeviceList();
            // Находим нужное нам устройство и открываем его
            foreach (IVciDevice item in deviceList)
            {
                pack = (Object)item.UniqueHardwareId;
                serialNumber = GetSerialNumber(ref pack);

                if (serialNumber == this._SerialNumber)
                {
                    // Устройство найдено
                    this._CanDevice = item;
                    break;
                }
            }
            return;
        }
        /// <summary>
        /// Освобождает ресурсы.
        /// </summary>
        public override void Dispose()
        {
            // Закрываем поток
            Interlocked.Exchange(ref this._FlagMustQuit, 0);
            // Завершаем событие, предварительно установив сигнальное состояние
            // для пробуждения рабочего потока и возможность его завершения
            if (this._RxEvent != null)
            {
                this._RxEvent.Set();
                this._RxEvent.Close();
            }

            // Освобождаем ресурсы
            DisposeVciObject(_CanBusLayer);
            DisposeVciObject(_CanChannel);
            DisposeVciObject(_CanController);
            DisposeVciObject(_CanDevice);

            _CanBusLayer = null;
            _CanChannel = null;
            _CanController = null;
            _CanDevice = null;

            return;
        }
        /// <summary>
        /// Создаёт новую структуру с описание устройства на основе полученной
        /// </summary>
        /// <param name="device">Устройство</param>
        /// <returns>Данные об устройстве</returns>
        internal static DeviceInfo ConvertToDeviceInfo(Ixxat.Vci3.IVciDevice device)
        {
            DeviceInfo devInfo;
            Ixxat.Vci3.VciCtrlInfo[] vciCtrlInfo;
            ControllerInfo[] listCntrInfo;

            if (device != null)
            {
                devInfo = new DeviceInfo();
                devInfo.Description = device.Description;
                devInfo.DeviceClass = device.DeviceClass;
                devInfo.DriverVersion = device.DriverVersion;
                devInfo.HardwareVersion = device.HardwareVersion;
                devInfo.Manufacturer = device.Manufacturer;
                devInfo.UniqueHardwareId = device.UniqueHardwareId;
                //devInfo.VciObjectId = device.VciObjectId;

                // Получаем список поддерживаемых интерфейсов (или микросхем)
                // устройством IXXAT (может поддерживать работу с разными шинами)
                vciCtrlInfo = device.Equipment;
                listCntrInfo = new ControllerInfo[vciCtrlInfo.Length];

                // Получаем данные по оборудованию для каждого поддерживаемого
                // интерфейса
                for (int i = 0; i < vciCtrlInfo.Length; i++)
                {
                    listCntrInfo[i] = new ControllerInfo();
                    // Получаем тип интерфейса
                    listCntrInfo[i].BusType = vciCtrlInfo[i].BusType.ToString();
                    // Получаем тип микросхемы
                    // !!! null - если vciCtrlInfo[i].BusType == unknown
                    if (vciCtrlInfo[i].ControllerType != null)
                    {
                        listCntrInfo[i].ControllerType = 
                            ((Ixxat.Vci3.Bal.Can.CanCtrlType)vciCtrlInfo[i].ControllerType).ToString();
                    }
                    else
                    {
                        // Если тип интерфейса не определён, то и связанный с ним тип микросхемы
                        // возвращаем также неопределённым
                        //listCntrInfo[i].ControllerType = Ixxat.Vci3.Bal.Can.CanCtrlType.Unknown;
                        listCntrInfo[i].ControllerType = "Unknown";
                    }
                }

                devInfo.Equipment = listCntrInfo;
            }
            else
            {
                throw new ArgumentNullException("device", String.Format(
                    "{0}: class CanPort.ConvertToDeviceInfo: Невозможно создать структуру описания устройства, устройство не существует", 
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false))));
            }

            return devInfo;
        }
        /// <summary>
        /// Returns the UniqueHardwareID GUID number as string which
        /// shows the serial number.
        /// Note: This function will be obsolete in later version of the VCI.
        /// Until VCI Version 3.1.4.1784 there is a bug in the .NET API which
        /// returns always the GUID of the interface. In later versions there
        /// the serial number itself will be returned by the UniqueHardwareID property.
        /// </summary>
        /// <param name="serialNumberGuid">Data read from the VCI.</param>
        /// <returns>The GUID as string or if possible the  serial number as string.</returns>
        public static string GetSerialNumber(ref object serialNumberGuid)
        {
            string resultText;

            // check if the object is really a GUID type
            if (serialNumberGuid.GetType() == typeof(System.Guid))
            {
                // convert the object type to a GUID
                System.Guid tempGuid = (System.Guid)serialNumberGuid;

                // copy the data into a byte array
                byte[] byteArray = tempGuid.ToByteArray();

                // serial numbers starts always with "HW"
                if (((char)byteArray[0] == 'H') && ((char)byteArray[1] == 'W'))
                {
                    // run a loop and add the byte data as char to the result string
                    resultText = "";
                    int i = 0;
                    while (true)
                    {
                        // the string stops with a zero
                        if (byteArray[i] != 0)
                            resultText += (char)byteArray[i];
                        else
                            break;
                        i++;

                        // stop also when all bytes are converted to the string
                        // but this should never happen
                        if (i == byteArray.Length)
                            break;
                    }
                }
                else
                {
                    // if the data did not start with "HW" convert only the GUID to a string
                    resultText = serialNumberGuid.ToString();
                }
            }
            else
            {
                // if the data is not a GUID convert it to a string
                string tempString = (string)(string)serialNumberGuid;
                resultText = "";
                for (int i = 0; i < tempString.Length; i++)
                {
                    if (tempString[i] != 0)
                        resultText += tempString[i];
                    else
                        break;
                }
            }

            return resultText;
        }
        /// <summary>
        /// Возвращает список устройств IXXAT (серийные номера)
        /// </summary>
        /// <returns>Список устройств IXXAT в системе</returns>
        public static String[] GetDevices()
        {
            IVciDeviceManager deviceManager = null;
            IVciDeviceList deviceList = null;
            List<String> listOfDevices = new List<string>();

            // Get device manager from VCI server
            deviceManager = VciServer.GetDeviceManager();
            // Get the list of installed VCI devices
            deviceList = deviceManager.GetDeviceList();
            // Убираем за собой мусор
            DisposeVciObject(deviceManager);
            // Получаем серийные номера

            foreach (IVciDevice item in deviceList)
            {
                object serialNumberGuid = item.UniqueHardwareId;
                listOfDevices.Add(GetSerialNumber(ref serialNumberGuid));
            }

            // Убираем за собой мусор
            DisposeVciObject(deviceList);

            return listOfDevices.ToArray();
        }
        /// <summary>
        /// Возвращает список описания всех доступных устройств производства IXXAT
        /// подключенных в систему
        /// </summary>
        /// <returns></returns>
        public static DeviceInfo[] GetDevicesInfo()
        {
            DeviceInfo[] listDeviceInfo; 
            IVciDevice[] listDevices;
            // Получаем список устройств
            listDevices = CAN.DataLinkLayer.CanPort.IXXAT.CanPort.GetIxxatDevices();
            // Создаём список описания устройств
            listDeviceInfo = new DeviceInfo[listDevices.Length];

            try
            {
                for (int i = 0; i < listDeviceInfo.Length; i++)
                {
                    // Получаем описание на устройство и сохраняем 
                    // в выходной список
                    listDeviceInfo[i] = CanPort.ConvertToDeviceInfo(listDevices[i]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    String.Format("{0}: class CanPort.GetDevicesInfo(): Невозмоно получить список IXXAT устройств",
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false))), ex);
            }
            finally
            {
                // Освобождаем ресурсы
                for (int i = 0; i < listDevices.Length; i++)
                {
                    DisposeVciObject(listDevices[i]);
                }
            }
            return listDeviceInfo;
        }
        /// <summary>
        /// Возвращает массив IXXAT-устройств доступных в системе.
        /// Важно!!! Удалить каждое устройство в массиве вызвав
        /// метод Dispose() см. документацию на библиотеку ixxat
        /// </summary>
        /// <returns></returns>
        public static IVciDevice[] GetIxxatDevices()
        {
            List<IVciDevice> array = new List<IVciDevice>();
            // Получаем список
            IVciDeviceList list = GetDeviceList();

            foreach (IVciDevice item in list)
            {
                array.Add(item);
            }
            
            DisposeVciObject(list);

            return array.ToArray();
        }
        /// <summary>
        /// Возвращает список устройств IXXAT доступных в системе
        /// </summary>
        /// <returns></returns>
        private static IVciDeviceList GetDeviceList()
        {
            IVciDeviceManager deviceManager = null;
            IVciDeviceList deviceList = null;

            // Get device manager from VCI server
            deviceManager = VciServer.GetDeviceManager();
            // Get the list of installed VCI devices
            deviceList = deviceManager.GetDeviceList();

            DisposeVciObject(deviceManager);

            return deviceList;
        }
        /// <summary>
        /// Освобождает ресурсы используемые неуправляемыми объектами IXXAT 
        /// </summary>
        /// <param name="obj"></param>
        private static void DisposeVciObject(object obj)
        {
            if (null != obj)
            {
                IDisposable dispose = obj as IDisposable;
                if (null != dispose)
                {
                    dispose.Dispose();
                    obj = null;
                }
            }
            return;
        }
        /// <summary>
        /// Открывает порт
        /// </summary>
        public override void Open()
        {
            String msg;
            IVciDeviceList deviceList;
            String serialNumber;
            Object pack;

            if (this._CanDevice == null)
            {
                // Получаем список доступных устройств
                deviceList = GetDeviceList();
                // Находим нужное нам устройство и открываем его
                foreach (IVciDevice item in deviceList)
                {
                    pack = (Object)item.UniqueHardwareId;
                    serialNumber = GetSerialNumber(ref pack);

                    if (serialNumber == this._SerialNumber)
                    {
                        // Устройство найдено
                        this._CanDevice = item;
                        break;
                    }
                }
            }

            if (this._CanDevice != null)
            {
                // Открываем порт
                Byte numberPort = 0;
                // Устройство найдено, открываем порт
                if (this.InitSocket(numberPort, ref this._CanDevice))
                {
                    // Порт открыт
                    // Устанавливаем флаг начала опроса порта
                    Interlocked.Exchange(ref _FlagMustQuit, Int32.MaxValue);
                    // start the receive thread
                    _ThreadForInput = new Thread(new ThreadStart(HandleQueueIncomingMessages));
                    _ThreadForInput.Start();
                    // Устанавливаем флаг - признак "порт открыт"
                    //_PortStatus = CANPORTSTATUS.IsPassive;
                    
                    // Здесь костыль: Почему-то, инфо-сообщения драйвера о переходе в новый статус при открытии порта не приходят.
                    // Поэтому проверяем в ручную. Если поле _PortStatus не соответсвует текущему состоянию,
                    // то останавливаем принудительно
                    if (this._CanChannel.ChannelStatus.IsActivated)
                    {
                        if (this._PortStatus != CanPortStatus.IsActive)
                        {
                            this._PortStatus = CanPortStatus.IsActive;
                            // Генерируем событие
                            this.OnPortChangesStatus(CanPortStatus.IsActive);
                        }
                    }
                }
                else
                {
                    // Порт не удалось открыть
                    msg = String.Format(
                        "{0}: class CanPort: Невозможно открыть CAN порт. Порт: {1} не удалось открыть.",
                        DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), this._SerialNumber);
                    Trace.TraceError(msg);
                    throw new InvalidOperationException(msg, null);
                }
            }
            else
            {
                // Устройство не найдено
                msg = String.Format("{0}: class CanPort: Невозможно открыть CAN порт. Порт: {1} не найден", 
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), this._SerialNumber);
                Trace.TraceError(msg);
                throw new Exception(msg);
                //throw new InvalidOperationException(msg, null);
            }
            return;
        }
        /// <summary>
        /// Закрывает CAN порт 
        /// </summary>
        public override void Close()
        {
            String msg;

            // Закрываем поток на чтение
            Interlocked.Exchange(ref _FlagMustQuit, 0);
            
            lock (_SyncRoot)
            {
                this._RxEvent.Set();
            }

            for (int i = 0; i < 1; i++)
            {
                if (this._ThreadForInput.IsAlive == true)
                {
                    // Ждём завершение потока
                    Thread.Sleep(500);
                }
                else
                {
                    // Поток завершён
                    break;
                }
            }

            // Если поток не завершился в течнии 10 секунд, значит дело плохо.
            // Выводим об этом трассировочную информацию
            if (this._ThreadForInput.IsAlive == true)
            {
                this._ThreadForInput.Abort();
                msg = String.Format(
                    "{0}: class CanPort.Close(): Рабочий поток не завершился за 0,5 секунду и находится в состоянии {1}",
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), this._ThreadForInput.ThreadState.ToString());
                Trace.TraceError(msg);
            }

            //SetMode(CANPORTSTATUS.IsPassive);
            _CanController.StopLine();
            //_CanChannel.Deactivate();

            _RxEvent.Close();
            _RxEvent = null;

            // Освобождаем ресурсы
            DisposeVciObject(_CanBusLayer);
            DisposeVciObject(_CanChannel);
            DisposeVciObject(_CanController);
            DisposeVciObject(_CanDevice);
            DisposeVciObject(_Reader);
            DisposeVciObject(_Writer);

            _CanBusLayer = null;
            _CanChannel = null;
            _CanController = null;
            _CanDevice = null;
            _Reader = null;
            _Writer = null;
            
            // Устанавливаем флаг признак "порт закрыт"
            this._PortStatus = CanPortStatus.IsClosed;

            // Формирум событие
            OnPortChangesStatus(CanPortStatus.IsClosed);

            Trace.TraceInformation("{0}: class CanPort.Close(): Порт закрыт",
                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")));
            return;
        }
        /// <summary>
        /// This method initialize the CAN line in the specified operating mode and bit 
        /// transfer rate. The method also performs a reset of the CAN controller hardware 
        /// and disables the reception of CAN messages.
        /// </summary>
        public void Init()
        {
            CanOperatingModes mode = CanOperatingModes.Undefined;
            String msg;

            if (this.ListenOnlyMode)
            {
                mode |= CanOperatingModes.ListOnly;
            }

            if (this.LowSpeedModeEnable)
            {
                mode |= CanOperatingModes.LowSpeed;
            }

            if (this.ErrorFrameEnable)
            {
               mode |= CanOperatingModes.ErrFrame;
            }

            switch (this.FrameFormat)
            {
                case FrameFormat.ExtendedFrame:
                    {
                        mode |= CanOperatingModes.Extended;
                        break;
                    }
                case FrameFormat.StandardFrame:
                    {
                        mode |= CanOperatingModes.Standard;
                        break;
                    }
                case FrameFormat.MixedFrame:
                    {
                        mode |= CanOperatingModes.Standard | CanOperatingModes.Extended;
                        break;
                    }
                default:
                    {
                        throw new InvalidEnumArgumentException(
                            "FRAMEFORMAT", (int)this.FrameFormat, typeof(FrameFormat));
                    }
            }

            _CanController.InitLine(mode, ConvertBitrate(this.BitRate));

            msg = String.Format(
                "Контроллер выполнил порерацию InitLine c настройками: \n  LowSpeedModeEnable - {0}\n  ListenOlnyMode - {1}\n  ErrorFrameEnable - {2}\n FrameFormat - {3}\n BitRate - {4}", 
                this.LowSpeedModeEnable, this.ListenOnlyMode, this.ErrorFrameEnable, this.FrameFormat, this.BitRate);
                
            Trace.WriteLine(msg);
            
            return;
        }
        public void SetFilter()
        {
            // Не разобрался с фильрами
            throw new NotImplementedException();

            //CanFilter canFilter;
            
            //if (this.IsOpen)
            //{
            //    if (_CanController != null)
            //    {
            //        // Фильтр сообщений можно применить только в режиме Init 
            //        if (this._CanController.LineStatus.IsInInitMode)
            //        { 
            //            // Применяем фильтр сообщений если он определён
            //            if (this.MessagesFilter != null)
            //            {
            //                switch(this.MessagesFilter.Format)
            //                {
            //                    case FRAMEFORMAT.StandardFrame:
            //                        {
            //                            canFilter = CanFilter.Std;
            //                            break;
            //                        }
            //                    case FRAMEFORMAT.ExtendedFrame:
            //                        {
            //                            canFilter = CanFilter.Ext;
            //                            break;
            //                        }
            //                    default:
            //                        { 
            //                            throw new ArgumentException("Недопустимый формат кадра для фильтра сообщений","Format"); 
            //                        }
            //                }
            //                //this._CanController.RemFilterIds();
            //                this._CanController.AddFilterIds(canFilter, this.MessagesFilter.Code, this.MessagesFilter.Mask);
            //            }
            //        }
            //    }
            //}
            //return;
        }
        /// <summary>
        /// This method starts the CAN line and switch it into running mode. After starting the CAN line, 
        /// CAN messages can be transmitted over the message channel.
        /// </summary>
        public override void Start()
        {
            Boolean result;
            String msg;

            if (this.IsOpen)
            {
                if (this._CanController != null)
                {
                    try
                    {
                        this._CanController.StartLine();
                    }
                    catch//(Exception ex)
                    {
                        this.Init();
                    }

                    // Проверяем состояние контроллера, т.е. выполнение команды
                    result = false;

                    for (int i = 0; i < 10; i++)
                    {
                        if (_CanController.LineStatus.IsBusOff)
                        {
                            // ждём
                            Thread.Sleep(500);
                        }
                        else
                        {
                            result = true;
                            break;
                        }
                    }

                    // Если по истечению 5 секунд контроллер не перевёл линию в активное состояние
                    // генерируем событие
                    if (result == false)
                    {
                        msg = String.Format("{0}: class CanPort.StartLine: Контроллер не смог выполнил StartLine",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                        Trace.TraceError(msg);
                        throw new InvalidOperationException(msg);
                    }
                }
                else
                {
                    msg = String.Format("{0}: class CanPort.StartLine: this._CanController Is Null!!!");
                    Trace.TraceError(msg);
                    throw new NullReferenceException(msg);
                    //this._PortStatus = CANPORTSTATUS.IsClosed;
                }
            }
            return;
        }
        /// <summary>
        /// This method stops the CAN line an switch it into init mode. After stopping the CAN controller 
        /// no further CAN messages are transmitted over the message channel. Other than ResetLine, 
        /// this method does not abort a currently busy transmit message and does not clear the standard 
        /// and extended mode ID filter.
        /// </summary>
        public override void Stop()
        {
            Boolean result;
            String msg;

            if (this.IsOpen)
            {
                this._CanController.StopLine();
               
                // Проверяем состояние контроллера, т.е. выполнение команды
                result = false;

                for (int i = 0; i < 10; i++)
                {
                    if (_CanController.LineStatus.IsInInitMode)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        // ждём
                        Thread.Sleep(500);
                    }
                }

                // Если по истечению 5 секунд контроллер не перевёл линию в активное состояние
                // генерируем событие
                if (result == false)
                {
                    msg = String.Format("{0}: CanPort.StartLine: Контроллер не смог выполнил StopLine",
                        DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                    Trace.TraceError(msg);
                    throw new InvalidOperationException(msg);
                }
            }
            return;
        }
        /// <summary>
        /// This method reset the CAN line to it's initial state. The method aborts a currently 
        /// busy transmit message and switch the CAN controller into init mode. 
        /// The method additionally clears the standard and extended mode ID filter. 
        /// </summary>
        public override void Reset()
        {
            Boolean result;
            String msg;

            if (this.IsOpen)
            {
                _CanController.ResetLine();

                // Проверяем состояние контроллера, т.е. выполнение команды
                result = false;
                for (int i = 0; i < 10; i++)
                {
                    if (_CanController.LineStatus.IsInInitMode)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        // ждём
                        Thread.Sleep(500);
                    }
                }

                // Если по истечению 5 секунд контроллер не перевёл линию в активное состояние
                // генерируем событие
                if (result == false)
                {
                    msg = String.Format("{0}: CanPort.StartLine: Контроллер не смог выполнил ResetLine",
                        DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                    Trace.TraceError(msg);
                    throw new InvalidOperationException(msg);
                }
            }
            return;
        }
        /// <summary>
        /// This method detects the actual bit rate of the CAN line 
        /// to which the controller is connected.
        /// </summary>
        /// <returns>If the method succeeds it returns the index of the detected 
        /// CanBitrate entry within the specified array. 
        /// If the method fails it returns -1. </returns>
        public int DetectBaud()
        {
            int bitRate = 0;
            CanBitrate[] bitrateTable = CanBitrate.CiaBitRates;
            
            if (this.IsOpen)
            {
                try
                {
                    bitRate = _CanController.DetectBaud(10000, bitrateTable);
                    Trace.WriteLine(String.Format("DetectBaud: {0}", BitRate));
                }
                catch (VciTimeoutException Ex) //Time between two successive receive messages exceed the value specified by the timeout parameter. 
                {
                    Trace.WriteLine(String.Format("DetectBaud: {0}", Ex.Message));
                }
            }
            return bitRate;
        }
        /// <summary>
        /// Преобразуте значение перечислимого NGK.CAN.OSIModel.DataLinkLayer.BITRATE
        /// в структуру Ixxat.Vci3.Bal.Can.CanBitrate
        /// </summary>
        /// <returns>Ixxat.Vci3.Bal.Can.CanBitrate</returns>
        private static Ixxat.Vci3.Bal.Can.CanBitrate ConvertBitrate(BaudRate bitRate)
        {
            String msg;
            Ixxat.Vci3.Bal.Can.CanBitrate IxxatBitRate;

            switch (bitRate)
            {
                case BaudRate.BR10:
                    {
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate.Cia10KBit;
                        break; 
                    }
                case BaudRate.BR20:
                    {
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate.Cia20KBit;
                        break;
                    }
                case BaudRate.BR50:
                    {
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate.Cia50KBit;
                        break;
                    }
                case BaudRate.BR100:
                    {
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate._100KBit;
                        break;
                    }
                case BaudRate.BR125:
                    {
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate.Cia125KBit;
                        break;
                    }
                case BaudRate.BR250:
                    {
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate.Cia250KBit;
                        break;
                    }
                case BaudRate.BR500:
                    {
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate.Cia500KBit;
                        break;
                    }
                case BaudRate.BR1000:
                    {
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate.Cia1000KBit;
                        break;
                    }
                default:
                    {
                        msg = String.Format(
                            "{0}: class CanPort: Невозможно преобразовать значение типа BITRATE в значение типа Ixxat.Vci3.Bal.Can.CanBitrate. Не найдено соответствие", 
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                        IxxatBitRate = Ixxat.Vci3.Bal.Can.CanBitrate.Empty;
                        Trace.TraceError(msg);
                        throw new ArgumentException(msg, "bitRate");
                    }
            }
            return IxxatBitRate;
        }
        /// <summary>
        ///   Opens the specified socket, creates a message channel, initializes
        ///   and starts the CAN controller.
        /// </summary>
        /// <param name="canNo">
        ///   Number of the CAN controller to open.
        /// </param>
        /// <returns>
        ///   A value indicating if the socket initialization succeeded or failed.
        /// </returns>
        private bool InitSocket(Byte canNo, 
            ref IVciDevice device)
        {
            this._CanBusLayer = null;
            bool succeeded = false;

            try
            {
                // Open bus access layer
                _CanBusLayer = device.OpenBusAccessLayer();

                // Open a message channel for the CAN controller
                _CanChannel = _CanBusLayer.OpenSocket(canNo, typeof(ICanChannel)) as ICanChannel;

                // Initialize the message channel
                // Используем канал единолично (true) 
                _CanChannel.Initialize(1024, 128, true);

                // Get a message reader object
                _Reader = _CanChannel.GetMessageReader();

                // Initialize message reader
                _Reader.Threshold = 1;

                // Create and assign the event that's set if at least one message
                // was received.
                _RxEvent = new AutoResetEvent(false);
                _Reader.AssignEvent(_RxEvent);

                // Get a message wrtier object
                _Writer = _CanChannel.GetMessageWriter();

                // Initialize message writer
                _Writer.Threshold = 1;

                // Activate the message channel
                _CanChannel.Activate();
                
                // Open the CAN controller
                _CanController = _CanBusLayer.OpenSocket(canNo, typeof(ICanControl)) as ICanControl;

                // Инициализируем контроллер с текущими параметрами
                this.Init();

                // !!!! ВНИМАНИЕ применяется фильтр, с данным куском не разобрался
                // Set the acceptance filter
                //_CanController.SetAccFilter(CanFilter.Std,
                //                     (uint)CanAccCode.All, (uint)CanAccMask.All);

                // Start the CAN controller
                _CanController.StartLine();

                succeeded = true;
            }
            catch
            {
                //MessageBox.Show(this, "Error: Initializing socket failed. Description: " + ex.Message, "Ошибка",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                succeeded = false;
                // Если не удалось открыть порт удаляем всё объекты
                DisposeVciObject(_CanChannel);
                DisposeVciObject(_Reader);
                DisposeVciObject(_Writer);
                DisposeVciObject(_CanBusLayer);
                DisposeVciObject(_CanController);

                throw; // Возобнавляем исключение
            }

            return succeeded;
        }
        /// <summary>
        /// Возвращает тип микросхемы контроллера.
        /// </summary>
        /// <returns>Тип контроллера</returns>
        public CanCtrlType GetControllerType()
        {
            if (_CanController != null)
            {
                return _CanController.ControllerType;
            }
            else
            {
                return CanCtrlType.Unknown;
            }
        }
        /// <summary>
        /// Возвращает состояние контроллера (или сети CAN)
        /// </summary>
        /// <returns>Состояние контроллера</returns>
        /// <remarks>
        ///  InInit - Init mode active  
        ///  BusOff - Bus off status  
        ///  ErrLimit - Error warning limit exceeded  
        ///  Overrun - Data overrun occurred  
        ///  TXPending - Transmission pending  
        ///  </remarks>
        public CanCtrlStatus GetControllerStatus()
        {
            String msg;

            if (this._CanController != null)
            {
                return _CanChannel.LineStatus.ControllerStatus;
            }
            else
            {
                msg = String.Format(
                    "{0}: class CanPort.GetControllerStatus: Невозможно прочитать состояние, контроллер CAN-порта null",
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                Trace.TraceError(msg);
                throw new Exception(msg);
            }
        }
        /// <summary>
        /// Возвращает состояние CAN-порта
        /// </summary>
        /// <returns>Статус порта</returns>
        public CanChannelStatus GetPortStatus()
        {
            String msg;
            CanChannelStatus status;
            
            if (_CanChannel != null)
            {
                status = _CanChannel.ChannelStatus;
            }
            else
            {
                msg = String.Format(
                    "{0}: class CanPort.GetPortStatus: Невозможно прочитать состояние, порт недоступен",
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                Trace.TraceError(msg);
                throw new Exception(msg);
            }
            return status;
        }
        /// <summary>
        /// Функция для приёма данных (запускается в отдельном потоке)
        /// </summary>
        private void HandleQueueIncomingMessages()
        {
            CanMessage canMessage;
            Boolean boolResult;
            Frame message;

            //Trace.TraceInformation("{0}: class CanPort: Поток на чтение данных запущен", 
            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
            //#if DEBUG
            //WindowsDebugging.WriteLine("Поток на чтение порта запущен", Category.Information);
            //#endif

            while(this._FlagMustQuit > 0)
            {
                if (_Reader == null)
                {
                    Trace.WriteLine("");
                    Trace.TraceError("{0} class CanPort.ReceiveMessages: Невозможно прочитать порт объект _Reader не существует, прекращаю поток чтения", 
                        DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                    Interlocked.Exchange(ref this._FlagMustQuit, 0);
                    return;
                }

                try
                {
                    // Ждём пока драйвер can-порта установит сигнальное состоние.
                    // Значит полученны данные в порт
                    boolResult = this._RxEvent.WaitOne(System.Threading.Timeout.Infinite, false);
                    //boolResult = this._RxEvent.WaitOne(100, false);

                    if (boolResult)
                    {
                        // По установке сигнального состояния, проверяем флаг. Возможно
                        // сигнальное состояние было установлено для прекращения работы потока
                        if (this._FlagMustQuit == 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // По таймауту.
                        continue;
                    }

                    // read a CAN message from the receive FIFO
                    //boolResult = true;

                    //while (boolResult)
                    while(this._Reader.ReadMessage(out canMessage))
                    {
                        //lock (SynckRoot)
                        //{
                            //if (this._Reader != null)
                            //{
                            //    boolResult = this._Reader.ReadMessage(out canMessage);
                            //}
                            //else
                            //{
                                // Завершаем поток
                            //    this._FlagMustQuit = 0;
                            //    this._PortStatus = CANPORTSTATUS.IsClosed;
                            //    break;
                            //}
                        //}

                        switch (canMessage.FrameType)
                        {
                            // show data frames
                            case CanMsgFrameType.Data:
                                {
                                    //Trace.WriteLine(String.Format("\nTime: {0,10}  ID: {1,3:X}  DLC: {2,1}  Data:",
                                    //    canMessage.TimeStamp, canMessage.Identifier, canMessage.DataLength));
                                    //Trace.TraceInformation("{0}: class CanPort.ReceiveMessages: TimeStamp: {1,10}  ID: {2,3:X}  DLC: {3,1}  Data:",
                                    //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), canMessage.TimeStamp, canMessage.Identifier, canMessage.DataLength);
                                    
                                    // Ловим это, отправляемых сообщений
                                    if (canMessage.SelfReceptionRequest == true)
                                    {
                                        Debug.WriteLine(String.Format("Message was sent: {0}", canMessage.ToString()));
                                        break;
                                    }
                                    
                                    message = new Frame();
                                    message.TimeStamp = canMessage.TimeStamp;
                                    message.Identifier = canMessage.Identifier;
                                    
                                    if (canMessage.ExtendedFrameFormat)
                                    {
                                        message.FrameFormat = FrameFormat.ExtendedFrame;
                                    }
                                    else
                                    {
                                        message.FrameFormat = FrameFormat.StandardFrame;
                                    }

                                    if (canMessage.RemoteTransmissionRequest)
                                    {
                                        message.FrameType = FrameType.REMOTEFRAME;
                                    }
                                    else
                                    {
                                        message.FrameType = FrameType.DATAFRAME;
                                    }

                                    message.Data = new Byte[canMessage.DataLength];
                                    
                                    for (int index = 0; index < canMessage.DataLength; index++)
                                    {
                                        message.Data[index] = canMessage[index];
                                    }

                                    //#if DEBUG
                                    //WindowsDebugging.WriteLine("IN: " + message.ToString(), Category.Information);
                                    //#endif
                                    // Добавляем сообщение во входной буфер и 
                                    lock (_SyncRoot)
                                    {
                                        this._InputBufferMessages.Enqueue(message);
                                    }
                                    
                                    // Генерируем событие
                                    this.OnDataReceive();
                                    break;
                                }
                                // show informational frames
                            case CanMsgFrameType.Info:
                                {
                                    //message = new Frame();
                                    //message.Data = null;
                                    //message.FrameType = FRAMETYPE.INFOFRAME;

                                    switch ((CanMsgInfoValue)canMessage[0])
                                    {
                                        case CanMsgInfoValue.Start:
                                            //Trace.TraceInformation("{0}: calss CanPort.ReceiveMessages: Принято сообщение от драйвера - CanPort was started", 
                                            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            //#if DEBUG
                                            //WindowsDebugging.WriteLine("Принято сообщение от драйвера - CanPort was started", Category.Information);
                                            //#endif

                                            this._PortStatus = CanPortStatus.IsActive;
                                            // Генерируем событие
                                            this.OnPortChangesStatus(CanPortStatus.IsActive);
                                            break;
                                        case CanMsgInfoValue.Stop:
                                            //Trace.TraceInformation("{0}: calss CanPort.ReceiveMessages: Принято сообщение от драйвера - CanPort was stopped",
                                            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            //#if DEBUG
                                            //WindowsDebugging.WriteLine("Принято сообщение от драйвера - CanPort was stopped", Category.Information);
                                            //#endif
                                            this._PortStatus = CanPortStatus.IsPassive;
                                            this.OnPortChangesStatus(CanPortStatus.IsPassive);
                                            // Генерируем событие
                                            break;
                                        case CanMsgInfoValue.Reset:
                                            //Trace.TraceInformation("{0}: calss CanPort.ReceiveMessages: Принято сообщение от драйвера - CanPort was reseted",
                                            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            //#if DEBUG
                                            //WindowsDebugging.WriteLine("Принято сообщение от драйвера - CanPort was reseted", Category.Information);
                                            //#endif

                                            this._PortStatus = CanPortStatus.IsPassiveAfterReset;
                                            // Генерируем событие
                                            this.OnPortChangesStatus(CanPortStatus.IsPassiveAfterReset);
                                            break;
                                        default:
                                            {
                                                Trace.TraceWarning("{0}: calss CanPort.ReceiveMessages: Получено нейзвестное сообщение от драйвера типа CanMsgFrameType.Info...",
                                                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                                throw new Exception();
                                                //this.OnPortChangesStatus(CANPORTSTATUS.Unknown);
                                                //break;
                                            }
                                    }
                                    break;
                                }
                                // show error frames
                            case CanMsgFrameType.Error:
                                {
                                    // Генерируем событие об ошибке

                                    switch ((CanMsgError)canMessage[0])
                                    {
                                        // Bit stuffing - когда узел передает последовательно в шину 5 бит с одинаковым значением, 
                                        // то он добавляет шестой бит с противоположным значением. Принимающие узлы этот 
                                        // дополнительный бит удаляют. Если узел обнаруживает на шине больше 5 последовательных 
                                        // бит с одинаковым значением, то он генерирует ошибку Stuff Error.
                                        case CanMsgError.Stuff:
                                            //Trace.TraceError("{0}: class CanPort.ReceiveMessages: stuff error...",
                                            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            //#if DEBUG 
                                            //WindowsDebugging.WriteLine("stuff error", Category.Error);
                                            //#endif
                                            OnErrorRecive(ERROR.BitStuff);
                                            break;
                                        // Frame Check - некоторые части CAN-сообщения имеют одинаковое значение во всех 
                                        // типах сообщений. Т.е. протокол CAN точно определяет какие уровни напряжения и 
                                        // когда должны появляться на шине. Если формат сообщений нарушается, то узлы 
                                        // генерируют ошибку Form Error.
                                        case CanMsgError.Form:
                                            //Trace.TraceError("{0}: class CanPort.ReceiveMessages: format of frame error...",
                                            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            //#if DEBUG
                                            //WindowsDebugging.WriteLine("format of frame error", Category.Error);
                                            //#endif

                                            OnErrorRecive(ERROR.FormatFrame);
                                            break;
                                        // ACKnowledgement Check - каждый узел получив правильное сообщение по сети посылает 
                                        // в сеть доминантный (0) бит. Если же этого не происходит, то передающий узел 
                                        // регистрирует ошибку Acknowledgement Error.
                                        case CanMsgError.Acknowledge:
                                            //Trace.TraceError("{0}: class CanPort.ReceiveMessages: acknowledgment error...",
                                            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            //#if DEBUG
                                            //WindowsDebugging.WriteLine("acknowledgment error", Category.Error);
                                            //#endif
                                            
                                            OnErrorRecive(ERROR.Acknowledge);
                                            break;
                                        // Check Bit monitoring - каждый узел во время передачи битов в сеть сравнивает значение 
                                        // передаваемого им бита со значением бита которое появляется на шине. Если эти значения 
                                        // не совпадают, то узел генерирует ошибку Bit Error. Естественно, что во время арбитража 
                                        // на шине (передача поля арбитража в шину) этот механизм проверки ошибок отключается.
                                        case CanMsgError.Bit:
                                            //Trace.TraceError("{0}: class CanPort.ReceiveMessages: bit error...",
                                            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            //#if DEBUG
                                            //WindowsDebugging.WriteLine("bit error", Category.Error);
                                            //#endif

                                            OnErrorRecive(ERROR.Bit);
                                            break;
                                        // CRC Check - каждое сообщение CAN содержит CRC сумму, и каждый принимающий узел 
                                        // подсчитывает значение CRC для каждого полученного сообщения. Если подсчитанное 
                                        // значение CRC суммы, не совпадает со значением CRC в теле сообщения, принимающий 
                                        // узел генерирует ошибку CRC Error.
                                        case CanMsgError.Crc:
                                            Trace.TraceError("{0}: class CanPort.ReceiveMessages: CRC error...",
                                                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            OnErrorRecive(ERROR.CRC);
                                            break;
                                        case CanMsgError.Other:
                                            //Trace.TraceError("{0}: class CanPort.ReceiveMessages: other error...",
                                            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                                            //#if DEBUG
                                            //WindowsDebugging.WriteLine("other error", Category.Error);
                                            //#endif

                                            OnErrorRecive(ERROR.Other);
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        String msg;
                        Interlocked.Exchange(ref _FlagMustQuit, 0);

                        msg = String.Format("{0}: class CanPort.ReceiveMessages: Чтение сообщения из буфера CanPort вызвало исключение {1}, поток завершён, стек: {2}",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), ex.Message, ex.StackTrace);
                        Trace.TraceError(msg);

                        throw;
                    }
                }

            //Trace.TraceInformation("{0}: class CanPort.ReceiveMessages(): Поток на чтение данных завершён", 
            //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")));
            //#if DEBUG
            //WindowsDebugging.WriteLine("Поток на чтение порта зaвершен", Category.Information);
            //#endif

            return;
        }
        /// <summary>
        /// Читает из входного буфера сообщение. 
        /// </summary>
        /// <returns>Если буфер пуст возвращается false</returns>
        public Boolean ReadMessage(out Frame message)
        {
            Boolean result;

            lock (_SyncRoot)
            {
                if (_InputBufferMessages.Count != 0)
                {
                    message = (Frame)_InputBufferMessages.Dequeue();
                    result = true;
                }
                else
                {
                    message = new Frame();
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        /// Читает указанное число сообщений из входного буфера. Если количество
        /// сообщений в буфере меньше указанного числа, то возвращается
        /// фактическое количество.
        /// </summary>
        /// <param name="count">Число сообщений для чтения</param>
        /// <returns>Прочитанные сообщения</returns>
        public override Frame[] ReadMessages(int count)
        {
            List<Frame> messages;
            
            lock (_SyncRoot)
            {
                messages = new List<Frame>();

                while (_InputBufferMessages.Count != 0)
                {
                    if (count != 0)
                    {
                        messages.Add((Frame)_InputBufferMessages.Dequeue());
                        --count;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return messages.ToArray();
        }
        /// <summary>
        /// Читает все доступные сообещния из входного буфера
        /// </summary>
        /// <returns>Прочитанные сообщения</returns>
        public override Frame[] ReadMessages()
        {             
            List<Frame> messages;

            lock (_SyncRoot)
            {
                messages = new List<Frame>();

                while (_InputBufferMessages.Count != 0)
                {
                    messages.Add((Frame)_InputBufferMessages.Dequeue());
                }
            }

            return messages.ToArray();
        }
        /// <summary>
        /// Записывает в порт сообщение (кадр)
        /// </summary>
        /// <param name="identifier">Идентификатор сообщения</param>
        /// <param name="frameType">Тип сообщения</param>
        /// <param name="extendedFrame">Формат сообщения</param>
        /// <param name="remoteTransmissionRequest">Удалённый запрос</param>
        /// <param name="data">Данные сообщения (0...8 байт)</param>
        public void WriteMessage(UInt32 identifier, CanMsgFrameType frameType,
            Boolean extendedFrame, Boolean remoteTransmissionRequest, Byte[] data)
        {
            lock (_SyncRoot)
            {
                CanMessage canMsg = new CanMessage();

                canMsg.TimeStamp = 0;
                canMsg.Identifier = identifier;
                canMsg.FrameType = frameType;
                canMsg.DataLength = System.Convert.ToByte(data.Length);
                canMsg.ExtendedFrameFormat = extendedFrame;
                canMsg.RemoteTransmissionRequest = remoteTransmissionRequest;
                // Ехо!!!
                canMsg.SelfReceptionRequest = true;  // show this message in the console window

                for (Byte i = 0; i < data.Length; i++)
                {
                    canMsg[i] = data[i];
                }

                // Write the CAN message into the transmit FIFO
                this._Writer.SendMessage(canMsg);
            }
            return;
        }
        /// <summary>
        /// Записывает в порт сообщение (кадр)
        /// </summary>
        /// <param name="identifier">Идентификатор сообщения</param>
        /// <param name="frameType">Тип сообщения</param>
        /// <param name="frameFormat">Формат сообщения</param>
        /// <param name="data">Данные сообщения (0...8 байт)</param>
        public override void WriteMessage(UInt32 identifier, FrameType frameType,
            FrameFormat frameFormat, Byte[] data)
        {
            String msg;

            lock (_SyncRoot)
            {
                CanMessage canMsg = new CanMessage();

                canMsg.TimeStamp = 0;
                canMsg.Identifier = identifier;

                switch (frameType)
                {
                    case FrameType.DATAFRAME:
                        {
                            canMsg.FrameType = CanMsgFrameType.Data;
                            canMsg.RemoteTransmissionRequest = false;
                            break;
                        }
                    case FrameType.REMOTEFRAME:
                        {
                            canMsg.FrameType = CanMsgFrameType.Data;
                            canMsg.RemoteTransmissionRequest = true;
                            break;
                        }
                    default:
                        {
                            msg = String.Format("{0}: class CanPort.WriteMessage: Невозможно записать в порт сообщение данного типа: {1}",
                                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), frameFormat.ToString());
                            Trace.TraceError(msg);
                            throw new ArgumentException(msg, FrameType.DATAFRAME.ToString());
                        }
                }

                if (data.Length > 8)
                {
                    msg = String.Format(
                        "{0}: class CanPort: Невозмоно записать в порт сообщение. Поле данных в сообщении не может быть больше 8 байт, передано: {1}",
                        DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), data.Length.ToString());
                    Trace.TraceError(msg);
                    throw new ArgumentException(msg, "data.Length");
                }
                else
                {
                    canMsg.DataLength = System.Convert.ToByte(data.Length);
                }

                switch (frameFormat)
                {
                    case FrameFormat.ExtendedFrame:
                        {
                            canMsg.ExtendedFrameFormat = true;
                            break;
                        }
                    case FrameFormat.StandardFrame:
                        {
                            canMsg.ExtendedFrameFormat = false;
                            break;
                        }
                    default:
                        {
                            msg = String.Format(
                                "{0}: class CanPort: Невозможно записать в порт сообщение с форматом кадра: {1}",
                                frameFormat.ToString());
                            Trace.TraceError(msg);
                            throw new ArgumentException(msg, "frameFormat");
                        }
                }

                // Эхо!!!
                canMsg.SelfReceptionRequest = true;  // show this message in the console window

                for (Byte i = 0; i < data.Length; i++)
                {
                    canMsg[i] = data[i];
                }

                //#if DEBUG
                //StringBuilder sb = new StringBuilder();
                //sb.Append("OUT: ");
                //sb.Append("Type: " + frameType.ToString() + " ");
                //sb.Append("Format: " + frameFormat.ToString() + " ");
                //sb.Append("Id:" + identifier.ToString() + " ");
                
                //foreach (Byte item in data)
                //{
                //    sb.Append("0x" + item.ToString("X2") + " ");
                //}

                //WindowsDebugging.WriteLine(sb.ToString(), Category.Information);
                //#endif

                // Write the CAN message into the transmit FIFO
                Debug.WriteLine(String.Format("Send: {0}", canMsg.ToString()));
                this._Writer.SendMessage(canMsg);
                Debug.WriteLine(String.Format("Buffer: {0}", this._Writer.FreeCount));
            }
            return;
        }
        /// <summary>
        /// Генерирует событие по приёму сообщения CAN линиии
        /// </summary>
        protected virtual void OnDataReceive()
        {
            EventHandler handler = this.MessageReceived;

            if (handler != null)
            {
                foreach (EventHandler SingleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = SingleCast.Target as ISynchronizeInvoke;

                    try
                    {
                        if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        {
                            syncInvoke.Invoke(SingleCast, new Object[] { this, new EventArgs() });
                        }
                        else
                        {
                            SingleCast(this, new EventArgs());
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// Генерирует событие при возникновении ошибки
        /// </summary>
        /// <param name="error">Ошибка</param>
        protected virtual void OnErrorRecive(ERROR error)
        {
            NGK.CAN.DataLinkLayer.CanPort.EventArgsLineErrorRecived args = 
                new EventArgsLineErrorRecived(error);
            EventHandlerErrorRecived handler = this.ErrorReceived;

            //#if DEBUG
            //WindowsDebugging.WriteLine("ERROR: " + error.ToString(), Category.Error);
            //#endif

            if (handler != null)
            {
                foreach (EventHandlerErrorRecived SingleCast in handler.GetInvocationList())
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
        /// Генерирует событие по изменению статуса порта
        /// </summary>
        /// <param name="status">Новое состояние CAN-порта</param>
        protected virtual void OnPortChangesStatus(CanPortStatus status)
        {
            EventArgsPortChangesStatus args = new EventArgsPortChangesStatus(status);
            EventHandlerPortChangesStatus handler = this.PortChangedStatus;

            if (handler != null)
            {
                foreach (EventHandlerPortChangesStatus SingleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = SingleCast.Target as ISynchronizeInvoke;
                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                    {
                        syncInvoke.Invoke(SingleCast, new Object[] { this, args });
                    }
                    else
                    {
                        SingleCast(this, args);
                    }
                }
            }
            return;
        }
        #endregion

        #region Events
        /// <summary>
        /// Событие по приёму сообщения
        /// </summary>
        public override event EventHandler MessageReceived;
        /// <summary>
        /// Событие при приёме сообщения Error Frame
        /// </summary>
        public override event EventHandlerErrorRecived ErrorReceived;
        /// <summary>
        /// Событие происходит когда порт окрыт, преходит в режим
        /// PortIsActive PortIsPassive PortReset
        /// </summary>
        public override event EventHandlerPortChangesStatus PortChangedStatus;
        /// <summary>
        /// Событие происходит при изменении состояния физической линии
        /// </summary>
        //public event EventHandler LineChangesStatus;
        #endregion

        #region Members Of ICanPort

        public override string HardwareType
        {
            get { return this.PortInfo._Description; }
        }
        public override string Manufacturer
        {
            get 
            {
                return this.PortInfo.Manufacturer;
            }
        }

        public override Version HardwareVersion
        {
            get { return this.PortInfo.HardwareVersion; }
        }

        public override Version SoftwareVersion
        {
            get { return this.PortInfo.DriverVersion; }
        }
        public override PortMode Mode
        {
            get 
            {
                if (this.ListenOnlyMode == true)
                {
                    return PortMode.LISTEN_ONLY;
                }
                else
                {
                    return PortMode.NORMAL;
                }
            }
            set 
            {
                switch(value)
                {
                    case PortMode.LISTEN_ONLY:
                        {
                            this.ListenOnlyMode = true;
                            break;
                        }
                    case PortMode.NORMAL:
                        {
                            this.ListenOnlyMode = false;
                            break;
                        }
                    default:
                        {
                            String msg;
                            msg = String.Format(
                                "Недопустимая операция. Режим работы порта {0} не поддерживается данной аппаратурой", value.ToString());
                            throw new InvalidOperationException(msg);
                        }
                }
            }
        }

        public override void Write(Frame message)
        {
            String msg;

            // Проверяем корректность данных. Можно отправить только фреймы двух типов
            // FRAMETYPE.DATAFRAME и FRAMETYPE.REMOTEFRAME остальные не допустимы (служебные)
            // !!! FRAMETYPE.OVERLOADFRAME не разобрался 
            if (!((message.FrameType == FrameType.DATAFRAME) ||
                (message.FrameType == FrameType.REMOTEFRAME)))
            {
                msg = String.Format(
                    "{0}: class CanPort.ICanPort.Write(Frame message): Недопустимый тип фрайма, подустимы только FRAMETYPE.DATAFRAME и FRAMETYPE.REMOTEFRAME",
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                Trace.TraceError(msg);
                throw new ArgumentException(msg, "message.FrameType");
            }
            else if (!((message.FrameFormat == FrameFormat.ExtendedFrame) || 
                (message.FrameFormat == FrameFormat.StandardFrame)))
            {
                msg = String.Format(
                    "{0}: class CanPort.ICanPort.Write(Frame message): Недопустимый формат фрайма, допустимы только FRAMEFORMAT.ExtendedFrame и FRAMEFORMAT.StandardFrame",
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                throw new ArgumentException(msg, "message.FrameFormat"); 
            }
            else
            {
                if (message.Data != null)
                {
                    if (message.Data.Length > 8)
                    {
                        msg = String.Format(
                            "{0}: class CanPort.ICanPort.Write(Frame message): Попытка передать сообещение, содержащее поле данных более 8 байт",
                            DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)));
                        throw new ArgumentException(msg, "message.Data.Length");
                    }
                    else
                    {
                        this.WriteMessage(message.Identifier, message.FrameType, message.FrameFormat, message.Data);
                    }
                }
                else
                {
                    message.Data = new Byte[0];
                    this.WriteMessage(message.Identifier, message.FrameType, message.FrameFormat, message.Data);
                }
            }
            return;
        }

        public override bool Read(out Frame message)
        {
            return this.ReadMessage(out message);
        }

        #endregion

        #region Members Of ISerializable
        /// <summary>
        /// Метод, вызываемый во время сериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("PortName", this.PortName);
            //info.AddValue("InputBufferLength", this._InputBufferMessages.
            // Инициализируем свойства порта
            info.AddValue("FrameFormat", this._FrameFormat);
            info.AddValue("ErrorFrameEnable", this._ErrorFrameEnable);
            info.AddValue("ListenOlnyMode", this._ListenOnlyMode);
            info.AddValue("BitRate", this._CanBitRate);
            return;
        }

        #endregion
    }
}
