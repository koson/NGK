using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Modbus.OSIModel.DataLinkLayer.Slave;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using Common.Controlling;

namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    /// <summary>
    /// Класс реализует контроллер сети Modubs со стороны slave-устройств 
    /// подключенных в внешней цепи через IDataLinkLayer
    /// </summary>
    public class ModbusNetworkControllerSlave: ModbusNetworkControllerBase
    {
        #region Fields and Properties
        /// <summary>
        /// Возвращает тип контроллера сети (slave или master)
        /// </summary>
        public override WorkMode Mode
        {
            get { return WorkMode.Slave; }
        }
        /// <summary>
        /// Наименование сети Modbus
        /// </summary>
        private String _NetworkName;
        /// <summary>
        /// Наименование сети Modbus
        /// </summary>
        public override String NetworkName
        {
            get { return _NetworkName; }
            set 
            {
                if (ModbusNetworksManager.Instance.Networks.Contains(value))
                {
                    throw new ArgumentException("Невозможно установить новое наименование сети, " +
                        "так как сеть с таким наименованием уже существует", "NetworkName");
                }
                else
                {
                    _NetworkName = value;
                }
            }
        }
        /// <summary>
        /// Порт для физического подключения к сети modbus
        /// </summary>
        IDataLinkLayer _Connection;
        /// <summary>
        /// Порт для физического подключения к сети modbus
        /// </summary>
        public IDataLinkLayer Connection
        {
            get { return _Connection; }
            set 
            {
                lock (SyncRoot)
                {
                    if (Status == Status.Stopped)
                    {
                        if (value != null)
                        {
                            if (value.IsOpen == true)
                            {
                                throw new InvalidOperationException(
                                    "Невозможно установить новое поключение, т.к текущее подключение активно");
                            }
                            else
                            {
                                _Connection = value;

                                _Connection.RequestWasRecived +=
                                    new EventHandlerRequestWasRecived(EventHandler_Connection_RequestWasRecived);
                                _Connection.ErrorOccurred +=
                                    new EventHandlerErrorOccurred(EventHandler_Connection_ErrorAccured);
                            }
                        }
                        else
                        {
                            _Connection = value;                            
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Попытка изменить порт подключения к сети modbus контрллера сети в работающем состоянии");
                    }
                }
            }
        }
        /// <summary>
        /// Коллекция устройств в данной сети
        /// </summary>
        DevicesCollection _Devices;
        /// <summary>
        /// Возвращает коллекцию устройств в сети
        /// </summary>
        public DevicesCollection Devices
        {
            get { return _Devices; }
        }
        /// <summary>
        /// Возвращает/устанавливает 
        /// </summary>
        public override Status Status
        {
            get 
            {
                if (_Connection != null)
                {
                    if (_Connection.IsOpen)
                    {
                        return Status.Running;
                    }
                    else
                    {
                        return Status.Stopped;
                    }
                }
                else
                {
                    return Status.Stopped;
                }
            }
            set 
            {
                switch (value)
                {
                    case Status.Paused:
                        { Suspend(); break; }
                    case Status.Running:
                        { Start(); break; }
                    case Status.Stopped:
                        { Stop(); break; }
                }
            }
        }
        /// <summary>
        /// Поток для прослушки входящих сообщений от мастера сети
        /// </summary>
        //private Thread _ThreadMsgListener;
        /// <summary>
        /// Объект для синхронизации.
        /// </summary>
        public static Object SyncRoot = new Object();
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public ModbusNetworkControllerSlave()
        {
            ModbusNetworksManager manager = ModbusNetworksManager.Instance;
            _NetworkName = GetNewName(); 

            //Stop();
            _Devices = new DevicesCollection(this);
            _Devices.ItemsListWasChanged +=
                new EventHandler(EventHandler_DevicesCollection_ItemsListWasChanged);
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="networkName">Наименование данной сети</param>
        /// <param name="connection">Объект для физического подключения к сети</param>
        public ModbusNetworkControllerSlave(String networkName, IDataLinkLayer connection)
        {
            ModbusNetworksManager manager = ModbusNetworksManager.Instance;
            
            // Проверяем уникальность наименование сети
            if (manager.Networks.Contains(networkName))
            {
                throw new Exception(
                    "Попытка создать сеть Modbus с наименованием, которое имеет другая сеть");
            }
            else
            {
                _NetworkName = networkName;
            }

            _Devices = new DevicesCollection(this);
            _Devices.ItemsListWasChanged +=
                new EventHandler(EventHandler_DevicesCollection_ItemsListWasChanged);

            _Connection = connection;

            if (_Connection != null)
            {
                _Connection.RequestWasRecived +=
                    new EventHandlerRequestWasRecived(EventHandler_Connection_RequestWasRecived);
                _Connection.ErrorOccurred +=
                    new EventHandlerErrorOccurred(EventHandler_Connection_ErrorAccured);

                // Переводим контроллер в состояние "стоп"
                Stop();
            }
        }
        #endregion

        #region EventHandlers For _DataLinkLayerObject
        /// <summary>
        /// Обработчик события ошибок объекта физического подключения к сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandler_Connection_ErrorAccured(
            object sender, ErrorOccurredEventArgs args)
        {
            String msg;
            
            msg = String.Format(
                "Ошибка при работе соединения: {0}; Описание: {1}",
                args.Error, args.Description);

            OnNetworkErrorOccurred(new NetworkErrorEventArgs(
                ErrorCategory.DataLinkLayerError, msg, null));
            return;
        }
        /// <summary>
        /// Обработчик события получение из сети запроса от мастера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandler_Connection_RequestWasRecived(
            object sender, MessageEventArgs args)
        {
            if (args.Message.Address == 0)
            {
                // Рассылаем принятое сообщение по устройствам
                foreach (Device device in _Devices)
                {
                    device.GetIncommingMessage(args.Message);
                }
            }
            else
            {
                if (Devices.Contains(args.Message.Address) == true)
                {
                    // Устройство с указанным адресом в запросе существует.
                    // Направляем ему данный запрос
                    lock (SyncRoot)
                    {
                        Devices[args.Message.Address].GetIncommingMessage(args.Message);
                    }
                }
            }
            return;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Возвращает новое уникальное имя сети
        /// </summary>
        /// <returns></returns>
        private String GetNewName()
        {
            string networkName;
            ModbusNetworksManager manager = ModbusNetworksManager.Instance;

            // Устанавливаем наименование сети по умолчанию
            // Формат имени [Network][index]
            // Индекс - номер по порядку
            // Ищем не занятое имя и устнавливаем

            for (int i = 1; i < Int32.MaxValue; i++)
            {
                // Получаем новое имя 
                networkName = "Network" + i.ToString();
                // Проверяем, существует ли уже такое в списке
                if (!manager.Networks.Contains(networkName))
                {
                    return networkName;
                }
            }
            // Не удалось найти уникальное имя.
            throw new Exception();
        }
        /// <summary>
        /// Обработчик события изменения списка устройств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DevicesCollection_ItemsListWasChanged(
            object sender, EventArgs e)
        {
            // Генерируем событие 
            OnDevicesListWasChanged();
            return;
        }
        /// <summary>
        /// Метод принимает исходящие сообщение (ответ на запрос) от сетевого
        /// устройства
        /// </summary>
        /// <param name="message">Исходящие ответное сообщение</param>
        internal void GetOutcommingMessage(Message.Message message)
        {
            _Connection.SendResponse(message);
        }
        /// <summary>
        /// Запускает работу сети
        /// </summary>
        public override void Start()
        {
            if (_Connection != null)
            {
                if (_Connection.IsOpen)
                {
                    // Ничего не делаем, соединение уже открыто
                }
                else
                {
                    Connection.Open();
                }
            }
            else
            {
                throw new NullReferenceException(
                    "Невозможно запустить контроллер сети, не найден объект IDataLinkLayer");
            }
            return;
        }
        /// <summary>
        /// Останавливает работу сети
        /// </summary>
        public override void Stop()
        {
            if (_Connection != null)
            {
                if (_Connection.IsOpen)
                {
                    foreach (Device device in _Devices)
                    {
                        device.Stop();
                    }
                    Connection.Close();                    
                }
                else
                {
                    // Ничего не делаем, соединение уже закрыто
                }
            }
            else
            {
                //throw new NullReferenceException(
                //    "Невозможно остановить контроллер сети, не найден объект IDataLinkLayer");
            }
            return;
        }
        /// <summary>
        /// Приостанавливает работу сети
        /// </summary>
        public override void Suspend()
        {
            throw new NotImplementedException(
                "Состояние: Paused, контроллера сети modbus не поддерживается");
        }
        /// <summary>
        /// Сериализует и сохраняет содержимое контроллера в файл в виде
        /// XML-схемы
        /// </summary>
        public void SaveToXmlFile(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                path = Environment.CurrentDirectory + @"\config.xml";
            }

            using (XmlTextWriter writer = new XmlTextWriter(path, null))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                
                writer.WriteStartElement("Network");
                writer.WriteAttributeString("Name", NetworkName);

                // Сохраняем список устройств
                writer.WriteStartElement("Devices");
                foreach (Device device in _Devices)
                {
                    writer.WriteStartElement("Device");
                    // Сохраняем свойства элемента "Device"
                    writer.WriteAttributeString("Address", device.Address.ToString());
                    writer.WriteAttributeString("Description", device.Description);
                    writer.WriteAttributeString("Status", device.Status.ToString());
                    
                    // Сохраняем узлы элемента "Device"
                    
                    // Сохраняем регистры хранения
                    writer.WriteStartElement("HoldingRegisters");
                    foreach (HoldingRegister register in device.HoldingRegisters)
                    {
                        writer.WriteStartElement("HoldingRegister");
                        writer.WriteAttributeString("Address", register.Address.ToString());
                        writer.WriteStartElement("Value");
                        writer.WriteValue(register.Value);
                        writer.WriteFullEndElement(); // "Value"
                        writer.WriteStartElement("Description");
                        writer.WriteString(register.Description);
                        writer.WriteFullEndElement(); // "Description"
                        writer.WriteFullEndElement(); // "HoldingRegister"
 
                    }
                    writer.WriteFullEndElement(); // "HoldingRegisters"

                    // Сохраняем входные регистры
                    writer.WriteStartElement("InputRegisters");
                    foreach (InputRegister register in device.InputRegisters)
                    {
                        writer.WriteStartElement("InputRegister");
                        writer.WriteAttributeString("Address", register.Address.ToString());
                        writer.WriteStartElement("Value");
                        writer.WriteValue(register.Value);
                        writer.WriteFullEndElement(); // "Value"
                        writer.WriteStartElement("Description");
                        writer.WriteString(register.Description);
                        writer.WriteFullEndElement(); // "Description"
                        writer.WriteFullEndElement(); // "InputRegister"

                    }
                    writer.WriteFullEndElement(); // "InputRegisters"

                    // Сохраняем дискретные входы/выходы
                    writer.WriteStartElement("Сoils");
                    foreach (Coil coil in device.Coils)
                    {
                        writer.WriteStartElement("Coil");
                        writer.WriteAttributeString("Address", coil.Address.ToString());
                        writer.WriteStartElement("Value");
                        writer.WriteValue(coil.Value);
                        writer.WriteFullEndElement(); // "Value"
                        writer.WriteStartElement("Description");
                        writer.WriteString(coil.Description);
                        writer.WriteFullEndElement(); // "Description"
                        writer.WriteFullEndElement(); // "Coil"

                    }
                    writer.WriteFullEndElement(); // "Сoils"

                    // Сохраняем дискретные входы
                    writer.WriteStartElement("DiscretesInputs");
                    foreach (DiscreteInput dicsreteInput in device.DiscretesInputs)
                    {
                        writer.WriteStartElement("DiscreteInput");
                        writer.WriteAttributeString("Address", dicsreteInput.Address.ToString());
                        writer.WriteStartElement("Value");
                        writer.WriteValue(dicsreteInput.Value);
                        writer.WriteFullEndElement(); // "Value"
                        writer.WriteStartElement("Description");
                        writer.WriteString(dicsreteInput.Description);
                        writer.WriteFullEndElement(); // "Description"
                        writer.WriteFullEndElement(); // "DiscreteInput"

                    }
                    writer.WriteFullEndElement(); // "DiscretesInputs"

                    // Сохраняем файлы устройства
                    writer.WriteStartElement("Files");
                    foreach (File file in device.Files)
                    {
                        writer.WriteStartElement("File");
                        writer.WriteAttributeString("Number", file.Number.ToString());
                        writer.WriteAttributeString("Description", file.Description);
                        
                        writer.WriteStartElement("Records");
                        foreach (Record record in file.Records)
                        {
                            writer.WriteStartElement("Record");
                            writer.WriteAttributeString("Number", record.Address.ToString());
                            writer.WriteAttributeString("Description", record.Description);

                            writer.WriteStartElement("Value");
                            writer.WriteValue(record.Value);
                            writer.WriteFullEndElement(); // "Value"
                            writer.WriteFullEndElement(); // "Record"
                        }
                        writer.WriteFullEndElement(); // "Records"
                        writer.WriteFullEndElement(); // "File"
                    }
                    writer.WriteFullEndElement(); // "Files"

                    writer.WriteFullEndElement(); // "Device"
                }
                writer.WriteFullEndElement(); // "Devices"
                writer.WriteFullEndElement(); // "Network"
            }
            return;
        }
        /// <summary>
        /// Создаёт контроллер сети Modbus из файла конфигурации сети
        /// </summary>
        /// <param name="pathToXmlFile">Путь + название файла конфигурации сети *.xml</param>
        /// <param name="pathToXsdFile">Путь + название файла файла схемы для файла конфигурации *.xsd</param>
        /// <returns>Если возникла ошибка возвращается null, если процесс создания 
        /// успешно завершён возвращается контроллер сети</returns>
        public static ModbusNetworkControllerSlave Create(String pathToXmlFile, 
            String pathToXsdFile)
        {
            XmlReaderSettings xmlrdsettings;
            XmlReader reader;
            ModbusNetworkControllerSlave network;
            Device device;
            Coil coil;
            DiscreteInput dinput;
            HoldingRegister hregister;
            InputRegister iregister;
            File file;
            Record record;
            List<Device> devices;
            String networkName;

            networkName = String.Empty;
            devices = new List<Device>();

            xmlrdsettings = new XmlReaderSettings();
            xmlrdsettings.IgnoreComments = true;
            xmlrdsettings.IgnoreWhitespace = true;
            xmlrdsettings.Schemas.Add("", pathToXsdFile);
            xmlrdsettings.ValidationType = ValidationType.Schema;
            //xmlrdsettings.ValidationEventHandler +=
            //    new ValidationEventHandler(EventHandler_vr_ValidationEventHandler);
            reader = XmlReader.Create(pathToXmlFile, xmlrdsettings);

            try
            {
                while(reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (!reader.IsEmptyElement)
                        {
                            switch (reader.Name)
                            {
                                case "Network":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            // Получаем наименование сети
                                            networkName = reader.GetAttribute("Name");
                                            //network.NetworkName = reader.GetAttribute("Name");
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "Ошибка в строке {0}.Элемент Network не имеет свойства Name",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                                case "Device":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            device = new Device(Byte.Parse(reader.GetAttribute("Address")));
                                            device.Description = reader.GetAttribute("Description");                                          
                                            device.Status = (Status)Enum.Parse(typeof(Status), 
                                                reader.GetAttribute("Status"));
                                            //network.Devices.Add(device);
                                            devices.Add(device);
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "Ошибка в строке {0}.Элемент Device не имеет свойств",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                                case "Coil":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            Boolean value;
                                            UInt16 address = UInt16.Parse(reader.GetAttribute("Address"));

                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Value")
                                                {
                                                    reader.Read();
                                                    value = Boolean.Parse(reader.Value);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "Ошибка в строке {0}. Элемент Coil не содержит элемент Value",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент Coil содержит элемент недопустимого типа",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }

                                            reader.Read(); // EndElement Value
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Description")
                                                {
                                                    reader.Read();
                                                    coil = new Coil(address, value, reader.Value);
                                                    //network.Devices[network.Devices.Count - 1].Coils.Add(coil);
                                                    devices[devices.Count - 1].Coils.Add(coil);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "Ошибка в строке {0}. Элемент Coil не содержит элемент Description",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент Coil содержит элемент недопустимого типа",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "Ошибка в строке {0}.Элемент Coil не имеет свойств",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                                case "DiscreteInput":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            Boolean value;
                                            UInt16 address = UInt16.Parse(reader.GetAttribute("Address"));

                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Value")
                                                {
                                                    reader.Read();
                                                    value = Boolean.Parse(reader.Value);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "Ошибка в строке {0}. Элемент DiscreteInput не содержит элемент Value",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент DiscreteInput содержит элемент недопустимого типа",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }

                                            reader.Read(); // EndElement Value
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Description")
                                                {
                                                    reader.Read();
                                                    dinput = new DiscreteInput(address, value, reader.Value);
                                                    
                                                    //network.Devices[network.Devices.Count - 1].DiscretesInputs.Add(dinput);
                                                    devices[devices.Count - 1].DiscretesInputs.Add(dinput);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "Ошибка в строке {0}. Элемент DiscreteInput не содержит элемент Description",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент DiscreteInput содержит элемент недопустимого типа",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "Ошибка в строке {0}.Элемент DiscreteInput не имеет свойств",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                                case "HoldingRegister":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            UInt16 value;
                                            UInt16 address = UInt16.Parse(reader.GetAttribute("Address"));

                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Value")
                                                {
                                                    reader.Read();
                                                    value = UInt16.Parse(reader.Value);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "Ошибка в строке {0}. Элемент HoldingRegister не содержит элемент Value",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент HoldingRegister содержит элемент недопустимого типа",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }

                                            reader.Read(); // EndElement Value
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Description")
                                                {
                                                    reader.Read();
                                                    hregister = new HoldingRegister(address, value, reader.Value);
                                                    //network.Devices[network.Devices.Count - 1].HoldingRegisters.Add(hregister);
                                                    devices[devices.Count - 1].HoldingRegisters.Add(hregister);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "Ошибка в строке {0}. Элемент HoldingRegister не содержит элемент Description",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент HoldingRegister содержит элемент недопустимого типа",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "Ошибка в строке {0}.Элемент HoldingRegister не имеет свойств",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break; 
                                    }
                                case "InputRegister":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            UInt16 value;
                                            UInt16 address = UInt16.Parse(reader.GetAttribute("Address"));

                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Value")
                                                {
                                                    reader.Read();
                                                    value = UInt16.Parse(reader.Value);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "Ошибка в строке {0}. Элемент InputRegister не содержит элемент Value",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент InputRegister содержит элемент недопустимого типа",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }

                                            reader.Read(); // EndElement Value
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Description")
                                                {
                                                    reader.Read();
                                                    iregister = new InputRegister(address, value, reader.Value);
                                                    //network.Devices[network.Devices.Count - 1].InputRegisters.Add(iregister);
                                                    devices[devices.Count - 1].InputRegisters.Add(iregister);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "Ошибка в строке {0}. Элемент InputRegister не содержит элемент Description",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент InputRegister содержит элемент недопустимого типа",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "Ошибка в строке {0}.Элемент InputRegister не имеет свойств",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break; 
                                    }
                                case "File":
                                    {
                                        file = new File(UInt16.Parse(reader.GetAttribute("Number")),
                                            reader.GetAttribute("Description"));
                                        //network.Devices[network.Devices.Count - 1].Files.Add(file);
                                        devices[devices.Count - 1].Files.Add(file);
                                        break;
                                    }
                                case "Record":
                                    {
                                        UInt16 number = UInt16.Parse(reader.GetAttribute("Number"));
                                        String description = reader.GetAttribute("Description");
                                        // Вычитываем следующий элемент. Это долно быть Value
                                        reader.Read();
                                        if (reader.NodeType == XmlNodeType.Element)
                                        {
                                            if (reader.Name == "Value")
                                            {
                                                reader.Read();
                                                record = new Record(number,
                                                    UInt16.Parse(reader.Value), description);
                                                //device = network.Devices[network.Devices.Count - 1];
                                                //device.Files[device.Files.Count - 1].Records.Add(record);
                                                device = devices[devices.Count - 1];
                                                device.Files[device.Files.Count - 1].Records.Add(record);
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "Ошибка в строке {0}. Элемент Record не содержит элемент Value",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "Ошибка в строке {0}. Элемент Record содержит элемент недопустимого типа",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                            }
                        }
                    } // End of if (reader.NodeType == XmlNodeType.Element)
                } // End of while(reader.Read())
            }
            //catch (XmlException ex)
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }

            //    throw;
            //}
            catch //(Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }

                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            // Создаём сеть из полученных данных
            network = new ModbusNetworkControllerSlave(networkName, null);

            foreach (Device item in devices)
            {
                network.Devices.Add(item);
            }

            return network;
        }
        /// <summary>
        /// Обработчик события ошибки при верификации xml-файла конфигурации и
        /// xsd-схемы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void EventHandler_vr_ValidationEventHandler(
            object sender, ValidationEventArgs e)
        {
            throw new NotImplementedException(
                "Метод не реализован в данной версии ПО");
            //return;
        }
        /// <summary>
        /// Генерирует событие изменения списка устройств в сети
        /// </summary>
        private void OnDevicesListWasChanged()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = DevicesListWasChanged;

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        /// <summary>
        /// Генерирует событие возникновения ошибок при работе сети.
        /// </summary>
        private void OnNetworkErrorOccurred(NetworkErrorEventArgs args)
        {
            NetworkErrorOccurredEventHandler handler = NetworkErrorOccurred;

            if (args == null)
            {
                args = new NetworkErrorEventArgs();
            }

            if (handler != null)
            {
                foreach (NetworkErrorOccurredEventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Возвращаем наименование сети
            return String.Format("Type={0}; NetworkName={1}; PortName={2}",
                Mode, _NetworkName,
                _Connection == null ? "None" : _Connection.PortName);
            //return base.ToString();
        }
        #endregion

        #region Events
        /// <summary>
        /// Событие происходит при изменении списка устройств сети
        /// </summary>
        public event EventHandler DevicesListWasChanged;
        /// <summary>
        /// Событие происходит при возникновении ошибок работы сети
        /// </summary>
        public event NetworkErrorOccurredEventHandler NetworkErrorOccurred;
        
        #endregion
    }
}
