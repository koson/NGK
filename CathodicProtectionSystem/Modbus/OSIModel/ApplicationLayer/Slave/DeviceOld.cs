using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Modbus.Interfaces.SlaveDevice;
using Modbus.Interfaces.Server.Network.Device.DataModel.Table.Data;
using Modbus.OSIModel.DataLinkLayer.Slave;
using Modbus.OSIModel.DataLinkLayer;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.Tables;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.Tables.Data;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.SerialPort;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    //===============================================================================
    /// <summary>
    /// Класс для создания Slave-устройства.
    /// </summary>
    [Serializable]
    public class DeviceOld: ISlaveDevice, IModbusFunctions, ISerializable
    {
        //---------------------------------------------------------------------------
        #region Delegates
        //---------------------------------------------------------------------------
        /// <summary>
        /// Делегат для запуска метода обработки запроса в асинхронном режиме
        /// </summary>
        /// <param name="message">Запрос</param>
        private delegate void ProcessTheRequest(Message.Message message);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Делегат для запуска метода формирования события 
        /// приёма запроса в асинхронном режиме
        /// </summary>
        /// <param name="args">Аргументы события</param>
        private delegate void EventOnRequstRecived(EventArgsRequestRecived args);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Делегат для запуска метода формирования события 
        /// ответа на запрос в асинхронном режиме
        /// </summary>
        /// <param name="args">Аргументы события</param>
        private delegate void EventOnReplyTransmitted(EventArgsReplyTransmitted args);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Делегат для запуска метода формирования события
        /// ошибки в асинхронном режиме
        /// </summary>
        /// <param name="args">Аргументы события</param>
        private delegate void EventOnError(EventArgsErrorAccured args);
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Fields and Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Разбирает запрос и отвечает на него
        /// </summary>
        private ProcessTheRequest _replyToRequest;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Формирует событие приёма запроса от мастера
        /// </summary>
        private EventOnRequstRecived _onRequstRecived;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Формирует событие ответа на запрос мастеру сети
        /// </summary>
        private EventOnReplyTransmitted _onReplyTransmitted;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Формирует событие при возникновении ошибочной 
        /// ситуации
        /// </summary>
        private EventOnError _onError;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        private Byte _address;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        public Byte Address
        {
            get { return _address; }
            set 
            {
                if ((value == 0) || (value > 247))
                {
                    throw new ArgumentOutOfRangeException("value", String.Format(
                        "Сетвой адрес {0}, должен быть в диапазоне 1...247", value));
                }
                else
                {
                    _address = value;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Структура данных устройства
        /// </summary>
        private DataSet _dataMap;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Данные устройства, таблицы регистров хранения, входных регистров,
        /// дискретных входов, реле
        /// </summary>
        public DataSet DataMap
        {
            get { return _dataMap; }
            set { _dataMap = value; }
        }
        //---------------------------------------------------------------------------
        private IDataLinkLayer _connection;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Интерфейс физического соединения с сетью
        /// </summary>
        public IDataLinkLayer Connection
        {
            get { return _connection; }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors and Destructor
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        private DeviceOld()
        {
            this.Address = 1;
            this.InitDataMap();
            this._connection = null;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Сетевой адрес устройства</param>
        /// <param name="connection">Объект физического порта подключения</param>
        public DeviceOld(Byte address, IDataLinkLayer connection)
        {
            this.Address = address;
            this._replyToRequest = 
                new ProcessTheRequest(this.RequestParse);
            this._onReplyTransmitted = 
                new EventOnReplyTransmitted(this.OnReplyTransmitted);
            this._onError =
                new EventOnError(this.OnError);
            this._onRequstRecived =
                new EventOnRequstRecived(this.OnRequestRecived);

            this.InitDataMap();

            this._connection = connection;

            if (_connection != null)
            {
                _connection.ErrorAccured += 
                    new EventHandlerErrorAccured(EventHandler_DataLinkObject_ErrorOccured);
                _connection.ReplyWasTransmitted += 
                    new EventHandleReplyTransmitted(EventHandler_DataLinkObject_ReplyTransmited);
                _connection.RequestRecived += 
                    new EventHandlerRequestRecived(EventHandler_DataLinkObject_RequestRecived);
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор для десериализатора
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public DeviceOld(SerializationInfo info, StreamingContext context)
        {
            this._replyToRequest =
                new ProcessTheRequest(this.RequestParse);
            this._onReplyTransmitted =
                new EventOnReplyTransmitted(this.OnReplyTransmitted);
            this._onError =
                new EventOnError(this.OnError);
            this._onRequstRecived =
                new EventOnRequstRecived(this.OnRequestRecived);


            this.Address = info.GetByte("Address");
            this._dataMap = (DataSet)info.GetValue("DataMap", typeof(DataSet));
            
            Object[] settings = (Object[])info.GetValue("ConnectionSettings", 
                typeof(Object));

            switch ((InterfaceType)info.GetValue("ConnectionType", 
                typeof(InterfaceType)))
            {
                case InterfaceType.SerialPort:
                    {
                        if ((TransmissionMode)info.GetValue("Mode",
                            typeof(TransmissionMode)) == TransmissionMode.RTU)
                        {
                            // Режим передачи RTU
                            ComPort comport = new ComPort(this.Address,
                                (String)settings[0], (int)settings[1],
                                (System.IO.Ports.Parity)settings[3],
                                (int)settings[2],
                                (System.IO.Ports.StopBits)settings[4]);
                            _connection = (IDataLinkLayer)comport;
                        }
                        else
                        {
                            // Режим передачи ASCII
                            throw new NotImplementedException();
                        }
                        break;
                    }
                case InterfaceType.TCPIP:
                    {
                        throw new NotImplementedException();
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            if (_connection != null)
            {
                _connection.ErrorAccured +=
                    new EventHandlerErrorAccured(EventHandler_DataLinkObject_ErrorOccured);
                _connection.ReplyWasTransmitted +=
                    new EventHandleReplyTransmitted(EventHandler_DataLinkObject_ReplyTransmited);
                _connection.RequestRecived +=
                    new EventHandlerRequestRecived(EventHandler_DataLinkObject_RequestRecived);
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        /// <summary>
        /// Сохраняет модель данных в файл по указаному пути
        /// </summary>
        /// <param name="path">путь к файлу + имя файла</param>
        /// <param name="device">Сохраняемое slave-устройство</param>
        public static void Save(String path, DeviceOld device)
        {
            System.IO.FileStream fs =
                new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            BinaryFormatter frm = new BinaryFormatter();
            frm.Serialize(fs, device);

            fs.Close();

            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод читает bin-файл с сохранённым slave-устройством
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Slave-устройство</returns>
        public static DeviceOld Open(String path)
        {
            System.IO.FileStream fs =
                new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

            BinaryFormatter frm = new BinaryFormatter();

            DeviceOld device = (DeviceOld)frm.Deserialize(fs);

            fs.Close();

            return device;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Инициализирует данные устройства
        /// </summary>
        private void InitDataMap()
        {
            DataTable table;
            
            _dataMap = new DataSet("DataMap");

            table = new DataTable("Coils");
            _dataMap.Tables.Add(table);
            table = new DataTable("DiscretesInputs");
            _dataMap.Tables.Add(table);
            table = new DataTable("HoldingRegisters");
            _dataMap.Tables.Add(table);
            table = new DataTable("InputRegisters");
            _dataMap.Tables.Add(table);

            DataColumn column;

            // Таблица реле
            column = new DataColumn("Address", typeof(UInt16));
            column.AllowDBNull = false;
            column.Caption = "Адрес";
            column.DefaultValue = 0;
            column.Unique = true;
            _dataMap.Tables["Coils"].Columns.Add(column);
            _dataMap.Tables["Coils"].PrimaryKey = new DataColumn[] { column };

            column = new DataColumn("Value", typeof(Boolean));
            column.AllowDBNull = false;
            column.Caption = "Значение";
            column.DefaultValue = false;
            _dataMap.Tables["Coils"].Columns.Add(column);

            column = new DataColumn("Description", typeof(String));
            column.AllowDBNull = true;
            column.Caption = "Описание";
            column.DefaultValue = "Реле";
            _dataMap.Tables["Coils"].Columns.Add(column);

            // Таблица дискретных входов
            column = new DataColumn("Address", typeof(UInt16));
            column.AllowDBNull = false;
            column.Caption = "Адрес";
            column.DefaultValue = 0;
            column.Unique = true;
            _dataMap.Tables["DiscretesInputs"].Columns.Add(column);
            _dataMap.Tables["DiscretesInputs"].PrimaryKey = new DataColumn[] { column };

            column = new DataColumn("Value", typeof(Boolean));
            column.AllowDBNull = false;
            column.Caption = "Значение";
            column.DefaultValue = false;
            _dataMap.Tables["DiscretesInputs"].Columns.Add(column);

            column = new DataColumn("Description", typeof(String));
            column.AllowDBNull = true;
            column.Caption = "Описание";
            column.DefaultValue = "Дискретный вход";
            _dataMap.Tables["DiscretesInputs"].Columns.Add(column);

            // Таблица регистров хранения
            column = new DataColumn("Address", typeof(UInt16));
            column.AllowDBNull = false;
            column.Caption = "Адрес";
            column.DefaultValue = 0;
            column.Unique = true;
            _dataMap.Tables["HoldingRegisters"].Columns.Add(column);
            _dataMap.Tables["HoldingRegisters"].PrimaryKey = new DataColumn[] { column };
            
            column = new DataColumn("Value", typeof(UInt16));
            column.AllowDBNull = false;
            column.Caption = "Значение";
            column.DefaultValue = 0;
            _dataMap.Tables["HoldingRegisters"].Columns.Add(column);

            column = new DataColumn("Description", typeof(String));
            column.AllowDBNull = true;
            column.Caption = "Описание";
            column.DefaultValue = "Регистр хранения";
            _dataMap.Tables["HoldingRegisters"].Columns.Add(column);
            
            // Таблица входных регистров
            column = new DataColumn("Address", typeof(UInt16));
            column.AllowDBNull = false;
            column.Caption = "Адрес";
            column.DefaultValue = 0;
            column.Unique = true;
            _dataMap.Tables["InputRegisters"].Columns.Add(column);
            _dataMap.Tables["InputRegisters"].PrimaryKey = new DataColumn[] { column };

            column = new DataColumn("Value", typeof(UInt16));
            column.AllowDBNull = false;
            column.Caption = "Значение";
            column.DefaultValue = 0;
            _dataMap.Tables["InputRegisters"].Columns.Add(column);

            column = new DataColumn("Description", typeof(String));
            column.AllowDBNull = true;
            column.Caption = "Описание";
            column.DefaultValue = "Входной регистр";
            _dataMap.Tables["InputRegisters"].Columns.Add(column);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Формирует событие при записи мастером 
        /// сети одного или группы реле
        /// </summary>
        private void OnWriteCoils()
        {
            if (this.MasterWritedCoils != null)
            {
                this.MasterWritedCoils(this, new EventArgs());
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Формирует событие при записи мастером 
        /// сети одного или группы регистров хранения
        /// </summary>
        private void OnWriteHoldingRegisters()
        {
            if (this.MasterWritedHoldingRegisters != null)
            {
                this.MasterWritedHoldingRegisters(this, new EventArgs());
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Формирует событие приёма запроса от мастера сети
        /// </summary>
        /// <param name="args">Аргументы события</param>
        private void OnRequestRecived(EventArgsRequestRecived args)
        {
            if (this.RequestRecived != null)
            {
                this.RequestRecived(this, args);
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Формирует событие отправки ответа 
        /// на запрос мастеру сети
        /// </summary>
        /// <param name="args">Аргументы события</param>
        private void OnReplyTransmitted(EventArgsReplyTransmitted args)
        {
            if (this.ReplyTransmited != null)
            {
                this.ReplyTransmited(this, args);
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Формирует событие ошибки при работе.
        /// </summary>
        /// <param name="args">Аргументы события</param>
        private void OnError(EventArgsErrorAccured args)
        {
            if (this.ErrorOccured != null)
            {
                this.ErrorOccured(this, args);
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Зарбирает запрос и вызывает необходимые 
        /// для его выполнения функции
        /// </summary>
        /// <param name="request">Запрос от мастера</param>
        private void RequestParse(Message.Message request)
        {
            Message.Result result;
            // Получаем код команды и анализируем сообщение
            switch (request.PDUFrame.Function)
            {
                case 0x01: // Функция 0x1. Чтение реле (не может быть широковещательной)
                    {
                        result = ((IModbusFunctions)this).ReadCoils(request);
                        break; 
                    }
                case 0x02: // Функция 0x2. Читает дискретные входы (не может быть широковещательной)
                    {
                        result = ((IModbusFunctions)this).ReadDiscreteInputs(request);
                        break;
                    }
                case 0x03: // Функция 0х3. Читает holding-регистры (не может быть широковещательной)
                    {
                        result = ((IModbusFunctions)this).ReadHoldingRegisters(request);
                        break;
                    }
                case 0x04: // Функция 0х4. Читает входные регистры (не может быть широковещательной)
                    {
                        result = ((IModbusFunctions)this).ReadInputRegisters(request);
                        break;
                    }
                case 0x05: // Функция 0х5. Устанавливает реле в состояние вкл./выкл.
                    {
                        result = ((IModbusFunctions)this).WriteSingleCoil(request);
                        break;
                    }
                case 0x06: // Функция 0x6. Записывает значение в одиночный регистр
                    {
                        result = ((IModbusFunctions)this).WriteSingleRegister(request);
                        break;
                    }
                case 0x0F:
                    {
                        result = ((IModbusFunctions)this).WriteMultipleCoils(request);
                        break;
                    }
                case 0x10:
                    {
                        result = ((IModbusFunctions)this).WriteMultipleRegisters(request);
                        break;
                    }
                default:
                    {
                        result = ((IModbusFunctions)this).FunctionNotSupported(request);
                        break;
                    }
            }

        }
        //---------------------------------------------------------------------------
        private void EventHandler_DataLinkObject_RequestRecived(object sender, 
            EventArgsRequestRecived args)
        {
            // Отвечаем на запрос
            this._replyToRequest.BeginInvoke(args.Request, null, null);
            // Формируем событие
            this._onRequstRecived.BeginInvoke(args, null, null);
            //OnRequestRecived(args);
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_DataLinkObject_ReplyTransmited(object sender, 
            EventArgsReplyTransmitted args)
        {
            // Формируем событие
            this._onReplyTransmitted.BeginInvoke(args, null, null);
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_DataLinkObject_ErrorOccured(object sender, 
            EventArgsErrorAccured args)
        {
            // Формируем событие
            this._onError.BeginInvoke(args, null, null);
            return;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Events
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит при записи одного или несколько
        /// регистров хранения мастером сети
        /// </summary>
        public event EventHandler MasterWritedHoldingRegisters;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит при записи оного или более
        /// реле мастером сети
        /// </summary>
        public event EventHandler MasterWritedCoils;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит по принятию запроса от мастера сети
        /// </summary>
        public event EventHandlerRequestRecived RequestRecived;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит после оправки ответа на запрос
        /// </summary>
        public event EventHandleReplyTransmitted ReplyTransmited;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит при возниконовении ошибочной ситуации
        /// </summary>
        public event EventHandlerErrorAccured ErrorOccured;
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Члены IApi
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.ReadCoils(Message.Message request)
        {
            Message.Result result;
            String message;

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x01 невозможен";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
                Debug.WriteLine(message);
            }
            else
            {
                // Проверяем длину PDU (должна равнятся 5 байтам)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // Длина сообщения не верная
                    message = String.Format(
                        "Длина PDU-фрайма в запросе {0}. Должна быть 5 байт", request.PDUFrame.ToArray());
                    result = new Message.Result(Error.DataFormatError, message,
                        request, null);
                }
                else
                {
                    // Разбираем сообщение
                    Byte[] array = new Byte[2];
                    // Получаем адрес первого реле
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // Получаем количесво реле для чтения
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 2000))
                    {
                        message = String.Format(
                            "Количество реле для чтения в запросе {0}, а должно быть 1...2000", quantity);
                        Message.PDU pdu = new Message.PDU(0x81, new Byte[] { 0x03 });
                                                
                        // Отправляем сообщение
                        _connection.SendReply(pdu);

                        Debug.WriteLine(message);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, pdu);

                    }
                    else
                    {
                        // Выполняем запрос. Проверяем доступность в 
                        // системе реле в указанном диапазоне адресов

                        DataRow[] row = new DataRow[quantity];


                        Message.PDU pdu;

                        for (int i = 0; i < quantity; i++)
                        {
                            row[i] = _dataMap.Tables["Coils"].Rows.Find(i + address);
                            // Если реле не существует в системе формируем ответ
                            // c исключение
                            if (row[i] == null)
                            {
                                // Формируем ответ с исключение 2
                                pdu = new Message.PDU();
                                pdu.Function = 0x01;
                                pdu.SetErrorBit();
                                pdu.AddDataByte(0x02);

                                result = new Message.Result(Error.IllegalDataAddress,
                                    String.Format("Реле с адресом {0} не существует", (address + i)),
                                    request, pdu);

                                // Отправляем ответ мастеру
                                this._connection.SendReply(pdu);

                                return result;
                            }
                        }

                        // Все реле найдены формируем ответное сообщение
                        int totalBytes = quantity % 8;

                        if (totalBytes == 0)
                        {
                            totalBytes = quantity / 8;
                        }
                        else
                        {
                            totalBytes = 1 + (quantity / 8);
                        }

                        Byte[] data = new Byte[totalBytes];
                        int number = 0;
                        int index = 0;

                        for (int i = 0; i < quantity; i++)
                        {
                            data[index] = (Byte)(data[index] | (Byte)(Modbus.Convert.BooleanToBit((Boolean)row[i]["Value"]) << number));
                            
                            if (++number > 7)
                            {
                                number = 0;
                                ++index;
                            }
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x01;
                        pdu.AddDataByte((byte)totalBytes);  // Добавляем количество байт с состояниями реле
                        pdu.AddDataBytesRange(data);        // Добавляем байты с состояниями реле

                        result = new Message.Result(Error.NoError, String.Empty, request, pdu);
                        // Отправляем ответ мастеру
                        this._connection.SendReply(pdu);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.ReadDiscreteInputs(Message.Message request)
        {
            Message.Result result;
            String message;

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x02 невозможен";
                Debug.WriteLine(message);
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // Проверяем длину PDU (должна равнятся 5 байтам)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // Длина сообщения не верная
                    String mes = String.Format(
                        "Длина PDU-фрайма равна в запросе 0x2 равна {0} байт. Должна быть 5 байт",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                    Debug.WriteLine(mes);
                }
                else
                {
                    // Разбираем сообщение
                    Byte[] array = new Byte[2];
                    // Получаем адрес первого дискретного входа
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // Получаем количесво дискретных входов для чтения
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 2000))
                    {
                        message = 
                            String.Format("Количество дискретных входов для чтения в запросе {0}, а должно быть 1...2000", 
                            quantity);
                        Message.PDU pdu = new Message.PDU(0x82, new Byte[] { 0x03 });

                        // Отправляем сообщение
                        _connection.SendReply(pdu);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, pdu);
                        
                        Debug.WriteLine(message);
                    }
                    else
                    {
                        // Выполняем запрос. Проверяем доступность в 
                        // системе дискретных входов в указанном диапазоне адресов
                        //DiscreteInput[] inputs = new DiscreteInput[quantity];
                        DataRow[] row = new DataRow[quantity];
                        
                        Message.PDU pdu;

                        for (int i = address; i < address + quantity; i++)
                        {
                            //inputs[i] = _discretesInputs.Find((UInt16)(address + i));
                            row[(i-address)] = _dataMap.Tables["DiscretesInputs"].Rows.Find(i);

                            // Если дискретный вход не существует в системе формируем ответ
                            // c исключение
                            if (row[(i - address)] == null)
                            {
                                // Формируем ответ с исключение 2
                                pdu = new Message.PDU(0x82, new Byte[] { 0x02 });

                                message = 
                                    String.Format("Дискретный вход с адресом {0} не существует", 
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, pdu);

                                // Отправляем ответ мастеру
                                this._connection.SendReply(pdu);

                                Debug.WriteLine(message);
                                return result;
                            }
                        }

                        // Все дискретные входы найдены формируем ответное сообщение
                        int totalBytes = quantity % 8;

                        if (totalBytes == 0)
                        {
                            totalBytes = quantity / 8;
                            
                            if (totalBytes == 0)
                            {
                                totalBytes++;
                            }
                        }
                        else
                        {
                            totalBytes = 1 + (quantity / 8);
                        }

                        Byte[] data = new Byte[totalBytes];
                        int number = 0;
                        int index = 0;

                        for (int i = 0; i < quantity; i++)
                        {
                            data[index] = (Byte)(data[index] | 
                                (Byte)(Modbus.Convert.BooleanToBit((Boolean)row[i]["Value"]) << number));

                            if (++number > 7)
                            {
                                number = 0;
                                ++index;
                            }
                        }
                        pdu = new Message.PDU();
                        pdu.Function = request.PDUFrame.Function;
                        pdu.AddDataByte((Byte)totalBytes);
                        pdu.AddDataBytesRange(data);

                        result = new Message.Result(Error.NoError, String.Empty, request, pdu);
                        // Отправляем ответ мастеру
                        this._connection.SendReply(pdu);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.ReadHoldingRegisters(Message.Message request)
        {
            Message.Result result;
            String message;

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x03 невозможен";
                Debug.WriteLine(message);
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // Проверяем длину PDU (должна равнятся 5 байтам)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // Длина сообщения не верная
                    String mes = String.Format(
                        "Длина PDU-фрайма равна в запросе 0x3 равна {0} байт. Должна быть 5 байт",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                    Debug.WriteLine(mes);
                }
                else
                {
                    // Разбираем сообщение
                    Byte[] array = new Byte[2];
                    // Получаем адрес первого регистра входа
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // Получаем количесво регистров для чтения
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 125))
                    {
                        message =
                            String.Format("Количество регистров для чтения в запросе {0}, а должно быть 1...125",
                            quantity);
                        Message.PDU pdu = new Message.PDU(0x83, new Byte[] { 0x03 });

                        // Отправляем сообщение
                        _connection.SendReply(pdu);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, pdu);

                        Debug.WriteLine(message);
                    }
                    else
                    {
                        // Выполняем запрос. Проверяем доступность в 
                        // системе регистров в указанном диапазоне адресов
                        //HoldingRegister[] registers = new HoldingRegister[quantity];
                        DataRow[] row = new DataRow[quantity];

                        Message.PDU pdu;

                        for (int i = 0; i < quantity; i++)
                        {
                            //registers[i] = _holdingRegisters.Find((UInt16)(address + i));
                            
                            row[i] = _dataMap.Tables["HoldingRegisters"].Rows.Find(address + i);
                            
                            // Если регистр не существует в системе формируем ответ
                            // c исключение
                            if (row[i] == null)
                            {
                                // Формируем ответ с исключение 2
                                pdu = new Message.PDU(0x83, new Byte[] { 0x02 });

                                message =
                                    String.Format("Регистр с адресом {0} не существует",
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, pdu);

                                // Отправляем ответ мастеру
                                this._connection.SendReply(pdu);

                                Debug.WriteLine(message);
                                return result;
                            }
                        }

                        // Все регистры найдены формируем ответное сообщение
                        Byte[] temp;
                        List<Byte> data = new List<byte>();

                        data.Add(System.Convert.ToByte((quantity * 2)));

                        for (int i = 0; i < quantity; i++)
                        {
                            temp = Modbus.Convert.ConvertToBytes((UInt16)row[i]["Value"]);
                            data.AddRange(temp);
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x3;
                        pdu.AddDataBytesRange(data.ToArray());

                        result = new Message.Result(Error.NoError, String.Empty, request, pdu);
                        // Отправляем ответ мастеру
                        this._connection.SendReply(pdu);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.ReadInputRegisters(Message.Message request)
        {
            Message.Result result;
            String message;

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x04 невозможен";
                Debug.WriteLine(message);
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // Проверяем длину PDU (должна равнятся  байтам)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // Длина сообщения не верная
                    String mes = String.Format(
                        "Длина PDU-фрайма равна в запросе 0x4 равна {0} байт. Должна быть 5 байт",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                    Debug.WriteLine(mes);
                }
                else
                {
                    // Разбираем сообщение
                    Byte[] array = new Byte[2];
                    // Получаем адрес первого регистра входа
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // Получаем количесво регистров для чтения
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 125))
                    {
                        message =
                            String.Format("Количество регистров для чтения в запросе {0}, а должно быть 1...125",
                            quantity);
                        Message.PDU pdu = new Message.PDU(0x84, new Byte[] { 0x03 });

                        // Отправляем сообщение
                        _connection.SendReply(pdu);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, pdu);

                        Debug.WriteLine(message);
                    }
                    else
                    {
                        // Выполняем запрос. Проверяем доступность в 
                        // системе регистров в указанном диапазоне адресов
                        //InputRegister[] registers = new InputRegister[quantity];
                        DataRow[] rows = new DataRow[quantity];
 
                        Message.PDU pdu;

                        for (int i = 0; i < quantity; i++)
                        {
                            //registers[i] = _inputRegisters.Find((UInt16)(address + i));
                            rows[i] = _dataMap.Tables["InputRegisters"].Rows.Find(address + i);

                            // Если регистр не существует в системе формируем ответ
                            // c исключение
                            if (rows[i] == null)
                            {
                                // Формируем ответ с исключение 2
                                pdu = new Message.PDU(0x84, new Byte[] { 0x02 });

                                message =
                                    String.Format("Регистр с адресом {0} не существует",
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, pdu);

                                // Отправляем ответ мастеру
                                this._connection.SendReply(pdu);

                                Debug.WriteLine(message);
                                return result;
                            }
                        }

                        // Все регистры найдены формируем ответное сообщение
                        Byte[] temp;
                        List<Byte> data = new List<byte>();

                        for (int i = 0; i < quantity; i++)
                        {
                            temp = Modbus.Convert.ConvertToBytes((UInt16)rows[i]["Value"]);
                            data.AddRange(temp);
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x04;
                        pdu.AddDataByte((Byte)(data.Count));
                        pdu.AddDataBytesRange(data.ToArray());

                        result = new Message.Result(Error.NoError, String.Empty, request, pdu);
                        // Отправляем ответ мастеру
                        this._connection.SendReply(pdu);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.WriteSingleCoil(Message.Message request)
        {
            Message.Result result;
            String message;

            // Проверяем длину PDU (должна быть 5 байт)
            if (request.PDUFrame.ToArray().Length != 5)
            {
                // Длина сообщения не верная
                String mes = String.Format(
                    "Длина PDU-фрайма равна в запросе 0x05 равна {0} байт. Должна быть 5 байт",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
                Debug.WriteLine(mes);
            }
            else
            {
                 Message.PDU pdu;
                // Разбираем сообщение
                Byte[] array = new Byte[2];
                // Получаем адрес реле
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // Получаем значение реле для записи
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 status = Modbus.Convert.ConvertToUInt16(array);
                State statusOutput;
                // Получаем количество байт в пасылке
                switch (status)
                {
                    case 0x0000:
                        {
                            statusOutput = State.Off;
                            break;
                        }
                    case 0xFF00:
                        {
                            statusOutput = State.On;
                            break;
                        }
                    default:
                        {
                            // Ошибка
                            message =
                                String.Format(
                                "Неверный формат данных для кодировки состояния реле {0}, а должно быть 0x0000 или 0xFF00",
                                status.ToString("X4"));
                            pdu = new Message.PDU((Byte)(0x05 | 0x80), new Byte[] { 0x03 });

                            // Отправляем сообщение
                            _connection.SendReply(pdu);

                            result = new Message.Result(Error.IllegalDataValue,
                                message, request, pdu);

                            Debug.WriteLine(message);
                            return result;
                        }
                }
                
                // Выполняем запрос. Проверяем доступность в 
                // системе регистров в указанном диапазоне адресов
                //Coil coil = _coils.Find((UInt16)(address));
                DataRow row = _dataMap.Tables["Coils"].Rows.Find((UInt16)(address));

                // Если регистр не существует в системе формируем ответ
                // c исключение
                if (row == null)
                {
                    // Формируем ответ с исключение 2
                    pdu = new Message.PDU((Byte)(0x80 | 0x05), new Byte[] { 0x02 });
                    
                    message =
                        String.Format("Реле с адресом {0} не существует",
                            address);
                    result = new Message.Result(Error.IllegalDataAddress,
                        message, request, pdu);

                    // Отправляем ответ мастеру
                    this._connection.SendReply(pdu);
                    
                    Debug.WriteLine(message);
                    return result;
                }
                else
                {
                    // Реле найдено, устанавливаем новое значение и
                    // формируем ответное сообщение
                    row["Value"] = Modbus.Convert.ToBoolean(statusOutput);
                    
                    // Формируем ответ.
                    List<Byte> data_ = new List<byte>();
                    data_.AddRange(Modbus.Convert.ConvertToBytes(address));
                    data_.AddRange(Modbus.Convert.StateToArray(statusOutput));

                    pdu = new Message.PDU();
                    pdu.Function = 0x05;
                    pdu.AddDataBytesRange(data_.ToArray());

                    result = new Message.Result(Error.NoError, String.Empty, request, pdu);
                    // Отправляем ответ мастеру
                    this._connection.SendReply(pdu);
                    // Формируем событие
                    this.OnWriteCoils();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.WriteSingleRegister(Message.Message request)
        {
            Message.Result result;
            String message;

            // Проверяем длину PDU (должна быть 5 байт)
            if (request.PDUFrame.ToArray().Length != 5)
            {
                // Длина сообщения не верная
                String mes = String.Format(
                    "Длина PDU-фрайма равна в запросе 0x06 равна {0} байт. Должна быть 5 байт",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
                Debug.WriteLine(mes);
            }
            else
            {
                // Разбираем сообщение
                Byte[] array = new Byte[2];
                // Получаем адрес регистра
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // Получаем значение регистра
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 value = Modbus.Convert.ConvertToUInt16(array);

                // Выполняем запрос. Проверяем доступность в 
                // регистра 
                //HoldingRegister register;
                
                Message.PDU pdu;
                
                //register = _holdingRegisters.Find(address);

                DataRow row = _dataMap.Tables["HoldingRegisters"].Rows.Find(address);

                // Если регистр не существует в системе формируем ответ
                // c исключение
                if (row == null)
                {
                    // Формируем ответ с исключение 2
                    pdu = new Message.PDU((Byte)(0x06 | 0x80), new Byte[] { 0x02 });
                    message =
                        String.Format("Регистр с адресом {0} не существует", address);
                    result = new Message.Result(Error.IllegalDataAddress,
                        message, request, pdu);
                    
                    // Отправляем ответ мастеру
                    this._connection.SendReply(pdu);
                    
                    Debug.WriteLine(message);
                    return result;
                }
                else
                {
                    // Все регистры найдены, устанавливаем новые значения и
                    // формируем ответное сообщение
                    row["Value"] = value;
                    
                    // Формируем ответ.
                    pdu = new Message.PDU();
                    pdu.Function = 0x06;
                    pdu.AddDataBytesRange(Modbus.Convert.ConvertToBytes(address));
                    pdu.AddDataBytesRange(Modbus.Convert.ConvertToBytes(value));

                    result = new Message.Result(Error.NoError, String.Empty, request, pdu);
                    // Отправляем ответ мастеру
                    this._connection.SendReply(pdu);
                    // Формируем событие
                    this.OnWriteHoldingRegisters();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.WriteMultipleCoils(Message.Message request)
        {
            Message.Result result;
            String message;
            Message.PDU pdu;

            // Проверяем длину PDU (должна быть не менее 7 байтам)
            if (request.PDUFrame.ToArray().Length < 7)
            {
                // Длина сообщения не верная
                String mes = String.Format(
                    "Длина PDU-фрайма равна в запросе 0x0F равна {0} байт. Должна быть не менее 7 байт",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
                Debug.WriteLine(mes);
            }
            else
            {
                // Разбираем сообщение
                Byte[] array = new Byte[2];
                // Получаем адрес первого реле
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // Получаем количесво реле для записи
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);
                // Получаем количество байт в пасылке
                Byte count = request.PDUFrame.Data[4];
                
                int totalBytes = quantity % 8;

                if (totalBytes == 0)
                {
                    totalBytes = quantity / 8;
                }
                else
                {
                    totalBytes = 1 + (quantity / 8);
                }

                if ((quantity == 0) || (quantity > 0x7B0) || (totalBytes != count))
                {
                    message =
                        String.Format("Количество реле для записи в запросе равно {0}, а должно быть 1...0x7B0",
                        quantity);
                    pdu = new Message.PDU((Byte)(0xF | 0x80), new Byte[] { 0x03 });

                    // Отправляем сообщение
                    _connection.SendReply(pdu);

                    result = new Message.Result(Error.IllegalDataValue,
                        message, request, pdu);

                    Debug.WriteLine(message);
                }
                else
                {
                    // Выполняем запрос. Проверяем доступность в 
                    // системе регистров в указанном диапазоне адресов
                    //Coil[] coils = new Coil[quantity];
                    DataRow[] rows = new DataRow[quantity];

                    for (int i = 0; i < quantity; i++)
                    {
                        //coils[i] = _coils.Find((UInt16)(address + i));
                        rows[i] = _dataMap.Tables["Coils"].Rows.Find(address + i);
                        // Если регистр не существует в системе формируем ответ
                        // c исключение
                        if (rows[i] == null)
                        {
                            // Формируем ответ с исключение 2
                            pdu = new Message.PDU((Byte)(0x80 | 0x0F), new Byte[] { 0x02 });

                            message =
                                String.Format("Реле с адресом {0} не существует",
                                (address + i));
                            result = new Message.Result(Error.IllegalDataAddress,
                                message, request, pdu);

                            // Отправляем ответ мастеру
                            this._connection.SendReply(pdu);

                            Debug.WriteLine(message);
                            return result;
                        }
                    }

                    // Все реле найдены, устанавливаем новые значения и
                    // формируем ответное сообщение
                    Byte status;
                    // Устанавливаем новые значения в реле
 
                    for (int i = 0; i < count; i++)
                    {
                        status = request.PDUFrame.Data[5 + i];

                        for (int y = 0; y < 8; y++)
                        {
                            totalBytes = i * 8 + y;

                            if (totalBytes < quantity)
                            {
                                if (((status >> y) & 0x01) == 0)
                                {
                                    rows[totalBytes]["Value"] = Modbus.Convert.ToBoolean(State.Off);
                                }
                                else
                                {
                                    rows[totalBytes]["Value"] = Modbus.Convert.ToBoolean(State.On);
                                }
                            }
                        }
                    }
                    // Формируем ответ.
                    List<Byte> data_ = new List<byte>();
                    data_.AddRange(Modbus.Convert.ConvertToBytes(address));
                    data_.AddRange(Modbus.Convert.ConvertToBytes(quantity));

                    pdu = new Message.PDU();
                    pdu.Function = 0x0F;
                    pdu.AddDataBytesRange(data_.ToArray());

                    result = new Message.Result(Error.NoError, String.Empty, request, pdu);
                    // Отправляем ответ мастеру
                    this._connection.SendReply(pdu);
                    // Формируем событие
                    this.OnWriteCoils();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.WriteMultipleRegisters(Message.Message request)
        {
            Message.Result result;
            String message;

            // Проверяем длину PDU (должна быть не менее 8 байтам)
            if (request.PDUFrame.ToArray().Length < 8)
            {
                // Длина сообщения не верная
                String mes = String.Format(
                    "Длина PDU-фрайма равна в запросе 0x10 равна {0} байт. Должна быть не менее 8 байт",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
                Debug.WriteLine(mes);
            }
            else
            {
                // Разбираем сообщение
                Byte[] array = new Byte[2];
                // Получаем адрес первого регистра
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // Получаем количесво регистров для записи
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);
                // Получаем количество байт в пасылке
                Byte count = request.PDUFrame.Data[4];

                if ((quantity == 0) || (quantity > 123) || ((quantity * 2) != count))
                {
                    message =
                        String.Format("Количество регистров для записи в запросе равно {0}, а должно быть 1...123",
                        quantity);
                    Message.PDU pdu = new Message.PDU((Byte)(0x10 | 0x80), new Byte[] { 0x03 });

                    // Отправляем сообщение
                    _connection.SendReply(pdu);

                    result = new Message.Result(Error.IllegalDataValue,
                        message, request, pdu);

                    Debug.WriteLine(message);
                }
                else
                {
                    // Выполняем запрос. Проверяем доступность в 
                    // системе регистров в указанном диапазоне адресов
                    //HoldingRegister[] registers = new HoldingRegister[quantity];
                    DataRow[] rows = new DataRow[quantity];

                    Message.PDU pdu;

                    for (int i = 0; i < quantity; i++)
                    {
                        //registers[i] = _holdingRegisters.Find((UInt16)(address + i));
                        rows[i] = _dataMap.Tables["HoldingRegisters"].Rows.Find((UInt16)(address + i));
                        // Если регистр не существует в системе формируем ответ
                        // c исключение
                        if (rows[i] == null)
                        {
                            // Формируем ответ с исключение 2
                            pdu = new Message.PDU((Byte)(0x10 | 0x80), new Byte[] { 0x02 });

                            message =
                                String.Format("Регистр с адресом {0} не существует",
                                (address + i));
                            result = new Message.Result(Error.IllegalDataAddress,
                                message, request, pdu);

                            // Отправляем ответ мастеру
                            this._connection.SendReply(pdu);

                            Debug.WriteLine(message);
                            return result;
                        }
                    }

                    // Все регистры найдены, устанавливаем новые значения и
                    // формируем ответное сообщение
                    Byte[] temp = new Byte[2];
                    List<Byte> data = new List<byte>();

                    // Устанавливаем новые значения в регистры
                    for (int i = 0; i < quantity; i++)
                    {
                        Array.Copy(request.PDUFrame.Data, (5 + (i * 2)), temp, 0, 2);
                        rows[i]["Value"] = Modbus.Convert.ConvertToUInt16(temp);
                    }

                    // Формируем ответ.
                    temp = Modbus.Convert.ConvertToBytes(address);
                    data.AddRange(temp);
                    temp = Modbus.Convert.ConvertToBytes(quantity);
                    data.AddRange(temp);

                    pdu = new Message.PDU();
                    pdu.Function = 0x10;
                    pdu.AddDataBytesRange(data.ToArray());

                    result = new Message.Result(Error.NoError, String.Empty, request, pdu);
                    // Отправляем ответ мастеру
                    this._connection.SendReply(pdu);
                    // Формируем событие
                    this.OnWriteHoldingRegisters();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        DataLinkLayer.Slave.IDataLinkLayer IModbusFunctions.GetDataLinkObject()
        {
            return this._connection;
        }
        //---------------------------------------------------------------------------
        Message.Result IModbusFunctions.FunctionNotSupported(Message.Message request)
        { 
            Message.PDU pdu = new Message.PDU();
            pdu.Function = (Byte)(request.PDUFrame.Function | 0x80);
            pdu.AddDataByte(0x01);   //Error.IllegalFunction

            Message.Result result =
                new Message.Result(Error.IllegalFunction,
                    "Функция не поддерживается данным устройством",
                    request, pdu);
            
            // Отправляем ответ
            this._connection.SendReply(pdu);
            
            return result;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Члены ISlaveDevice
        //---------------------------------------------------------------------------
        public System.Data.DataTable GetCoils()
        {
            return _dataMap.Tables["Coils"];
        }
        //---------------------------------------------------------------------------
        public System.Data.DataTable GetDiscretesInputs()
        {
            return _dataMap.Tables["DiscretesInputs"];
        }
        //---------------------------------------------------------------------------
        public System.Data.DataTable GetHoldingRegisters()
        {
            return _dataMap.Tables["HoldingRegisters"];
        }
        //---------------------------------------------------------------------------
        public System.Data.DataTable GetInputRegisters()
        {
            return _dataMap.Tables["InputRegisters"];
        }
        //---------------------------------------------------------------------------
        public System.Data.DataSet GetDataMap()
        {
            return _dataMap;
        }
        //---------------------------------------------------------------------------
        public void SerializeToFile(string path)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------------
        public void DeserializeFromFile(string path)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------------
        public void Start()
        {
            if (!_connection.IsOpen())
            {
                _connection.Open();
            }
        }
        //---------------------------------------------------------------------------
        public void Stop()
        {
            if (_connection.IsOpen())
            {
                _connection.Close();
            }
        }
        //---------------------------------------------------------------------------
        public IDataLinkLayer GetConnection()
        {
            return _connection;
        }
        //----------------------------------------------------------------------------
        public void AddCoil(ushort address, State value)
        {
            DataRow row = _dataMap.Tables["Coils"].Rows.Find(address);
            
            if (row == null)
            {
                row = _dataMap.Tables["Coils"].NewRow();
                row["Address"] = address;
                row["Value"] = Modbus.Convert.ToBoolean(value);
                _dataMap.Tables["Coils"].Rows.Add(row);
            }

            return;
        }
        //----------------------------------------------------------------------------
        public void RemoveCoil(ushort address)
        {
            DataRow row = _dataMap.Tables["Coils"].Rows.Find(address);
            
            if (row != null)
            {
                _dataMap.Tables["Coils"].Rows.Remove(row);
            }

            return;
        }
        //----------------------------------------------------------------------------
        public void AddDiscreteInput(ushort address, State value)
        {
            DataRow row = _dataMap.Tables["DiscretesInputs"].Rows.Find(address);
            
            if (row == null)
            {
                row = _dataMap.Tables["DiscretesInputs"].NewRow();
                row["Address"] = address;
                row["Value"] = Modbus.Convert.ToBoolean(value);
                _dataMap.Tables["DiscretesInputs"].Rows.Add(row);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void RemoveDiscreteInput(ushort address)
        {
            DataRow row = _dataMap.Tables["DiscretesInputs"].Rows.Find(address);

            if (row != null)
            {
                _dataMap.Tables["DiscretesInputs"].Rows.Remove(row);
            }

            return;
        }
        //----------------------------------------------------------------------------
        public void AddHoldingRegister(ushort address, ushort value)
        {
            DataRow row = _dataMap.Tables["HoldingRegisters"].Rows.Find(address);

            if (row == null)
            {
                row = _dataMap.Tables["HoldingRegisters"].NewRow();
                row["Address"] = address;
                row["Value"] = value;
                _dataMap.Tables["HoldingRegisters"].Rows.Add(row);
            }
            return;
        }
        //---------------------------------------------------------------------------
        public void RemoveHoldingRegister(ushort address)
        {
            DataRow row = _dataMap.Tables["HoldingRegisters"].Rows.Find(address);

            if (row != null)
            {
                _dataMap.Tables["HoldingRegisters"].Rows.Remove(row);
            }

            return;
        }
        //---------------------------------------------------------------------------
        public void AddInputRegister(ushort address, ushort value)
        {
            DataRow row = _dataMap.Tables["InputRegisters"].Rows.Find(address);

            if (row == null)
            {
                row = _dataMap.Tables["InputRegisters"].NewRow();
                row["Address"] = address;
                row["Value"] = value;
                _dataMap.Tables["InputRegisters"].Rows.Add(row);
            }
            return; 
        }
        //---------------------------------------------------------------------------
        public void RemoveInputRegister(ushort address)
        {
            DataRow row = _dataMap.Tables["InputRegisters"].Rows.Find(address);

            if (row != null)
            {
                _dataMap.Tables["InputRegisters"].Rows.Remove(row);
            }

            return;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Члены ISerializable
        //---------------------------------------------------------------------------
        /// <summary>
        /// Сериализует объект
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, 
            StreamingContext context)
        {
            info.AddValue("Address", this.Address, typeof(Byte));
            info.AddValue("DataMap", this._dataMap, typeof(DataSet));
            info.AddValue("ConnectionType", 
                this._connection.GetTypeInterface(), typeof(InterfaceType));
            info.AddValue("ConnectionMode",
                this._connection.Mode, typeof(TransmissionMode));
            info.AddValue("Mode", _connection.Mode, typeof(TransmissionMode));

            switch (_connection.GetTypeInterface())
            {
                case DataLinkLayer.InterfaceType.SerialPort:
                    {
                        ComPort comport = (ComPort)_connection.GetConnection();

                        Object[] settings = new object[5];
                        settings[0] = (Object)comport.SerialPort.PortName;
                        settings[1] = (Object)comport.SerialPort.BaudRate;
                        settings[2] = (Object)comport.SerialPort.DataBits;
                        settings[3] = (Object)comport.SerialPort.Parity;
                        settings[4] = (Object)comport.SerialPort.StopBits;
                        
                        info.AddValue("ConnectionSettings", (Object)settings);
                        break;
                    }
                case DataLinkLayer.InterfaceType.TCPIP:
                    {
                        throw new NotImplementedException();
                    }
            }
            return;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file