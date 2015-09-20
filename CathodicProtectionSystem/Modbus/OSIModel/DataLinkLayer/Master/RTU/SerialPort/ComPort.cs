using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using Modbus.OSIModel.DataLinkLayer.Diagnostics;
using Modbus.OSIModel.DataLinkLayer.TypeConverters;
using Modbus.OSIModel.Transaction;

namespace Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort
{
    /// <summary>
    /// Класс реализует Data Link Layer протокола Modbus для физического
    /// уровня RS-485/RS-232 на основе COM-порта компьютера
    /// </summary>
    public class ComPort : Modbus.OSIModel.DataLinkLayer.Master.IDataLinkLayer
    {
        #region Fields and Properties
        //----------------------------------------------------------------------------
        /// <summary>
        /// Объект трассировки, для ведения журнала событий
        /// </summary>
        private TextWriterTraceListener _Logger;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Включает/выключает ведение журнала событий
        /// </summary>
        private Boolean _EventLogEnable;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Включает/выключает ведение журнала событий 
        /// </summary>
        [Category("Журнал")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Включает/выключает ведение журнала событий")]
        [DisplayName("Включить ведение журнала")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        public Boolean EventLogEnable
        {
            get { return _EventLogEnable; }
            set 
            {
                _EventLogEnable = value;
                if (_EventLogEnable)
                {
                    CreateLog();
                }
                else
                {
                    DeleteLog();
                }
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Маска типов сообщений которые будут записываться в журнал
        /// </summary>
        private TypeOfMessageLog _MaskOfMessageLog;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Маска типов сообщений которые будут записываться в журнал
        /// </summary>
        [Category("Журнал")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Типы сообщений доступные для записи в журнал")]
        [DisplayName("Фильтр сообщений")]
        public TypeOfMessageLog MaskOfMessageLog
        {
            get { return _MaskOfMessageLog; }
            set { _MaskOfMessageLog = value; }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Путь к файлу журнала. Если пустая строка, 
        /// то пишется в папку приложения
        /// </summary>
        private String _PathToLogFile;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Путь к файлу журнала. Если пустая строка, 
        /// то пишется в папку приложения
        /// </summary>
        [Category("Журнал")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Путь к файлу журнала")]
        [DisplayName("Путь к файлу журнала")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), 
            typeof(System.Drawing.Design.UITypeEditor))]
        public String PathToLogFile
        {
            get { return _PathToLogFile; }
            set { _PathToLogFile = value; }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Хранит код ошибки при выполнении запроса
        /// </summary>
        private RequestError _error;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Таймер события неответа на запрос
        /// </summary>
        private EventWaitHandle _timeOut;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Буфер входных данных
        /// </summary>
        private List<Byte> _incomingBuffer;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Определяет состояние устройства: свободно или 
        /// идёт транзация "запрос-ответ"
        /// </summary>
        private Modbus.OSIModel.Transaction.TransactionType _transaction;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Значение времени таймаута в мсек.
        /// </summary>
        private int _ValueTimeOut;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Значение времени таймаута в мсек.
        /// </summary>
        [Category("Протокол Modbus")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Таймаут ответа на запрос, мсек")]
        [DisplayName("Таймаут ответа, мсек")]
        public int TimeOut
        {
            get { return _ValueTimeOut; }
            set { _ValueTimeOut = value; }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Значение временной задержки при выполнении 
        /// широковещаетельного запроса
        /// </summary>
        private int _ValueTurnAroundDelay;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Значение временной задержки при выполнении 
        /// широковещаетельного запроса
        /// </summary>
        [Category("Протокол Modbus")]
        [Browsable(true)]
        [ReadOnly(false)]
        [Description("Временная задержка при широковещательной команде, мсек")]
        [DisplayName("Временная задержка, мсек")]
        public int ValueTurnAroundDelay
        {
            get { return _ValueTurnAroundDelay; }
            set { _ValueTurnAroundDelay = value; }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Таймер определяющий межкадровый интервал времени. По истечению
        /// межкадрового интервала считаме сообщение принято
        /// </summary>
        private System.Timers.Timer _tmrInterFrameDelay;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Последовательный порт системы, 
        /// через который выедётся обмен данными
        /// </summary>
        private System.IO.Ports.SerialPort _serialPort;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Последовательный порт
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки физического подключения")]
        [Description("Последоватлеьный порт")]
        [DisplayName("Последоватлеьный порт")]
        [TypeConverter(typeof(Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort.ConverterType.ComPortConverter))]
        [Editor(typeof(UITypeEditor.SerialPortUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public System.IO.Ports.SerialPort SerialPort
        {
            get { return _serialPort; }
            set { _serialPort = value; }
        }
        //----------------------------------------------------------------------------
        #endregion

        #region Constructors
        //----------------------------------------------------------------------------
        /// <summary>
        /// Конструктор по умолчанию запрещён
        /// </summary>
        private ComPort()
        {
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="portName">Имя COM-порта</param>
        /// <param name="baudRate">Скорость соединения</param>
        /// <param name="parity">Наличие паритета данных</param>
        /// <param name="dataBits">Количество бит в символе</param>
        /// <param name="stopBits">Количество стоп-бит</param>
        /// <param name="timeOut">Время таймаута ответа, в мсек</param>
        /// <param name="aroundDelay">Интервал временной задержки при широковещаетльном запросе, мсек</param>
        /// <param name="eventLogEnable">Разрешение вести журнал</param>
        /// <param name="filterLogMessage">Фильт сообщений записываемых в журнал</param>
        /// <param name="PathToLogFile">Путь к файлу журнала, если пустая строка, то по умолчанию в директории приложения</param>
        public ComPort(string portName, int baudRate, Parity parity,
            int dataBits, StopBits stopBits, int timeOut, int aroundDelay, 
            Boolean eventLogEnable, TypeOfMessageLog filterLogMessage, String pathToLogFile)
        {
            _EventLogEnable = eventLogEnable;
            _MaskOfMessageLog = filterLogMessage;
            if (pathToLogFile == String.Empty)
            {
                _PathToLogFile = Environment.CurrentDirectory + @"\SerialPort.log";
            }
            else
            {
                _PathToLogFile = pathToLogFile;
            }

            if (_EventLogEnable)
            {
                // Создаём журнал
                // По умолчанию ищем файл в папке приложения
                if (_PathToLogFile == String.Empty)
                {
                    // По умолчанию сохраняем в папку приложения
                    _PathToLogFile = Environment.CurrentDirectory + @"\datalink.log";
                }

                CreateLog();
            }
            else
            {
                Trace.Listeners.Clear();
            }

            // Таймер определения интервала между сообщениями
            _tmrInterFrameDelay = new System.Timers.Timer();
            // Если скорость передачи выше 19200 бит/сек, то устанавливаем
            // интервал 1.750mc, иначе 3,5*Tсимвола
            // !!! К сожалению следовать протоколу в винде не получается
            // Если ставить задержку меньше 20, пакет принимается не полностью
            // и выскакивает ошибка CRC.
            _tmrInterFrameDelay.Interval = 150; //20
            //if (Settings.BaudRate >= 19200)
            //{
            //    tmrInterFrameDelay.Interval = 1.750;
            //}
            //else
            //{
            //    tmrInterFrameDelay.Interval = Convert.ToDouble(1000 * 3.5 * 11 / Settings.BaudRate);
            //}
            _tmrInterFrameDelay.AutoReset = false;
            _tmrInterFrameDelay.Elapsed += new ElapsedEventHandler(_tmrInterFrameDelay_Elapsed);
            _tmrInterFrameDelay.Stop();

            _serialPort = 
                new System.IO.Ports.SerialPort(portName, baudRate, parity,
                dataBits, stopBits);

            // Настраиваем события от COM-порта
            _serialPort.ErrorReceived += 
                new SerialErrorReceivedEventHandler(ComPort_ErrorReceived);
            _serialPort.DataReceived += 
                new SerialDataReceivedEventHandler(ComPort_DataReceived);
            _serialPort.ReceivedBytesThreshold = 1;

            //_serialPort.ReadBufferSize = 512;
            
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _ValueTimeOut = timeOut;
            _ValueTurnAroundDelay = aroundDelay;
                    
            _timeOut = new EventWaitHandle(false, 
                EventResetMode.AutoReset);

            _incomingBuffer = new List<byte>();
            _incomingBuffer.Clear();

            _transaction = TransactionType.Undefined;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Деструктор класса
        /// </summary>
        ~ComPort()
        {
            // Удаляем ресурсы
            Trace.TraceInformation("{0}: {1} Окончание сессии",
                DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());

            DeleteLog();
            CloseConnect();
            SerialPort = null;

        }
        //----------------------------------------------------------------------------
        #endregion

        #region Methods
        //----------------------------------------------------------------------------
        /// <summary>
        /// Создаёт объетк журнала событий на основе свойств
        /// </summary>
        private void CreateLog()
        {
            if (_Logger != null)
            {
                _Logger.Close();
                _Logger.Filter = null;
            }

            _Logger = new TextWriterTraceListener(_PathToLogFile);
            
            Trace.Listeners.Clear();
            Trace.Listeners.Add(_Logger);
            Trace.AutoFlush = true;
            Trace.TraceInformation(String.Empty); // Для форматирования текста, отделяет сессию от сесии
            Trace.TraceInformation("{0}: {1} Старт сессии",
                DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
            Process prs = Process.GetCurrentProcess();
            Trace.TraceInformation("{0}: ID процесса: {1}",
                DateTime.Now.ToLongTimeString(), prs.Id);
            Trace.TraceInformation("{0}: Наименование процесса: {1}",
                DateTime.Now.ToLongTimeString(), prs.ProcessName);
            Trace.TraceInformation("{0}: Приоритет потка: {1}",
                DateTime.Now.ToLongTimeString(), prs.PriorityClass);
            Thread thread = Thread.CurrentThread;
            Trace.TraceInformation("{0}: ID потка: {1}",
                DateTime.Now.ToLongTimeString(), thread.ManagedThreadId);
            Trace.TraceInformation("{0}: Наименование потока: {1}",
                DateTime.Now.ToLongTimeString(), thread.Name);
            Trace.TraceInformation("{0}: Приоритет потока: {1}",
                DateTime.Now.ToLongTimeString(), thread.Priority);

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Удаляет объект для ведения журнала событий
        /// </summary>
        private void DeleteLog()
        {
            Trace.TraceInformation("{0}: {1} : Жунал отключен",
                DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());

            Trace.Listeners.Remove(_Logger);

            if (_Logger != null)
            {
                _Logger.Close();
                _Logger = null;
            }
            //Trace.Listeners.Clear();
            return;
        } 
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик срабатывания межкадрового таймера таймер.
        /// Служит для определения конца пакета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tmrInterFrameDelay_Elapsed(object sender, 
            ElapsedEventArgs e)
        {
            // Межкадровый таймер сработал. Пакет принят.
            // Выключаем межкадровый таймер
            _tmrInterFrameDelay.Stop();
            // Устанавливаем флаг
            _timeOut.Set(); 
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события приёма данных из COM-порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int data;
            
            // Сбрасываем межкадровый таймер
            _tmrInterFrameDelay.Stop();

            if (_serialPort.BytesToRead == 0)
            {
                if ((_MaskOfMessageLog & TypeOfMessageLog.Warning) == TypeOfMessageLog.Warning)
                {
                    Trace.TraceWarning("{0}: Поток ID: {1} : Принята нулевая посылка",
                        DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                }
                // После принятия очердной порции байт ответа
                // Запускаем межкадровый таймер
                _tmrInterFrameDelay.Start();
                return;
            }
            else
            {
                StringBuilder sb;

                // Идёт транзакция запрос-ответ?
                if (_transaction == TransactionType.UnicastMode)
                {                    
                    // Да - Принимаем байты
                    sb = new StringBuilder();
                    
                    while (_serialPort.BytesToRead != 0)
                    {
                        data = _serialPort.ReadByte();

                        if (data != -1)
                        {
                            lock (_incomingBuffer)
                            {
                                _incomingBuffer.Add(System.Convert.ToByte(data));
                                sb.Append(data.ToString("X2"));
                                sb.Append(" ");
                            }
                        }
                    }

                    if ((_MaskOfMessageLog & TypeOfMessageLog.Information) == TypeOfMessageLog.Information)
                    {
                        Trace.TraceInformation("{0}: Поток ID: {1} : Принято байт: {2}",
                            DateTime.Now.ToLongTimeString(),
                            Thread.CurrentThread.ManagedThreadId, sb.ToString());
                    }

                    // После принятия очердной порции байт ответа
                    // Запускаем межкадровый таймер
                    _tmrInterFrameDelay.Start();                            
                }
                else
                {
                    // Нет - принят мусор с линии
                    sb = new StringBuilder();
                    // Вычитываем этот мусор
                    while (_serialPort.BytesToRead != 0)
                    {
                        data = _serialPort.ReadByte();

                        if (data != -1)
                        {
                            lock (_incomingBuffer)
                            {
                                sb.Append(data.ToString("X2"));
                                sb.Append(" ");
                            }
                        }
                    }

                    if ((_MaskOfMessageLog & TypeOfMessageLog.Warning) == TypeOfMessageLog.Warning)
                    {
                        Trace.TraceWarning("{0}: Поток ID: {1} : Приняты данные, в отсутствии запроса : {2}",
                            DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, sb.ToString());
                    }
                    _serialPort.DiscardInBuffer();
                }
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик событий ошибочной работы COM-порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComPort_ErrorReceived(object sender, 
            SerialErrorReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialError.Frame:
                    {
                        _error = RequestError.Frame;
                        break;
                    }
                case SerialError.Overrun:
                    {
                        _error = RequestError.Overrun;
                        break;
                    }
                case SerialError.RXOver:
                    {
                        _error = RequestError.RXOver;
                        break;
                    }
                case SerialError.RXParity:
                    {
                        _error = RequestError.RXParity;
                        break;
                    }
                case SerialError.TXFull:
                    {
                        _error = RequestError.TXFull;
                        break;
                    }
            }

            StopTransaction();

            _timeOut.Set();
            
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Устанавливает режим транзакции запрос-ответ
        /// </summary>
        private void StartTransaction(
            Modbus.OSIModel.Transaction.TransactionType type)
        {
            if (_transaction == TransactionType.Undefined)
            {
                if ((_MaskOfMessageLog & TypeOfMessageLog.Information) == TypeOfMessageLog.Information)
                {
                    Trace.TraceInformation("{0}: Поток ID: {1} : Старт транзакции",
                        DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                }
                //_incomingBuffer.Clear();
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                _transaction = type;
                _error = RequestError.NoError;
            }
            else
            {
                if ((_MaskOfMessageLog & TypeOfMessageLog.Information) == TypeOfMessageLog.Information)
                {
                    Trace.TraceError("{0}: Поток ID: {1} : Попытка начать новую транзакцию во время текущей транзакции",
                        DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                }
                throw new Exception("Попытка начать новую транзакцию во время текущей транзакции");
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Устанавливает режим ожидания.
        /// </summary>
        private void StopTransaction()
        {
            if ((_MaskOfMessageLog & TypeOfMessageLog.Information) == TypeOfMessageLog.Information)
            {
                Trace.TraceInformation("{0}: Поток ID: {1} : Окончание транзакции",
                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
            }
            _serialPort.DiscardOutBuffer();
            _serialPort.DiscardInBuffer();
            _transaction = TransactionType.Undefined;
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Рассчитывает контрольную сумму и сверяет с переданной
        /// </summary>
        /// <param name="array">сообщение</param>
        /// <returns>Результат проверки: true - суммы совпали</returns>
        private Boolean VerifyCheckSum(Byte[] array)
        {
            Byte _checkSum = 0;

            // Иногда, данные приняты и лежат в буфере порта, 
            // а в программном буфере пусто. Поэтому приходится проверять
            // array на пустоту. Так и не разобрался почему происходит
            //if (array.Length == 0)
            //{
            //    return false;
            //}

            for (int i = 0; i < (array.Length - 1); i++)
            {
                _checkSum += array[i];
            }
            if (_checkSum == array[(array.Length - 1)])
            {
                // суммы сошлись
                return true;
            }
            else
            {
                return false;
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Возвращает название COM-порта 
        /// </summary>
        //public string Name
        //{
        //    get { return _serialPort.PortName; }
        //}
        //----------------------------------------------------------------------------
        /// <summary>
        /// Установить таймаут
        /// </summary>
        /// <param name="value">Таймаут, мсек</param>
        public void SetTimeOut(int value)
        {
            _ValueTimeOut = value;
        }
        //----------------------------------------------------------------------------
        #endregion

        #region Члены IDataLinkLayer
        //---------------------------------------------------------------------------
        /// <summary>
        /// Открыть порт
        /// </summary>
        public void OpenConnect()
        {
            _serialPort.Open();
            
            if ((_MaskOfMessageLog & TypeOfMessageLog.Information) == TypeOfMessageLog.Information)
            {
                Trace.TraceInformation("{0}: Поток ID: {1} : Последовательный порт открыт",
                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
            }
            
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Закрыть порт
        /// </summary>
        public void CloseConnect()
        {
            _serialPort.Close();

            if ((_MaskOfMessageLog & TypeOfMessageLog.Information) == TypeOfMessageLog.Information)
            {
                Trace.TraceInformation(String.Format("{0}: Поток ID: {1}: Последовательный порт закрыт",
                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId));
            }
            
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Отправляет сообщение с запросом
        /// </summary>
        /// <param name="requst">Запрос</param>
        /// <param name="answer">Ответ</param>
        /// <returns>Результат выполнения операции</returns>
        public RequestError SendMessage(
            Modbus.OSIModel.Message.Message requst,
            out Modbus.OSIModel.Message.Message answer)
        {
            RequestError error;
            Byte[] reply;
            Byte[] tempArray;

            error = SendMessage(requst.Address, requst.PDUFrame.Function, 
                requst.PDUFrame.Data, out reply);

            if (error == RequestError.NoError)
            {
                tempArray = new Byte[reply.Length - 1];
                Array.Copy(reply, 1, tempArray, 0, (reply.Length - 1));

                answer = new Modbus.OSIModel.Message.Message(requst.Address, reply[0], tempArray);
            }
            else
            {
                answer = null;
            }

            return error;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Отправляет сообщение
        /// </summary>
        /// <param name="address">Адрес Slave-устройства</param>
        /// <param name="function">Код функции запроса</param>
        /// <param name="request">Данные запроса</param>
        /// <param name="answer">Ответ: Код функции + данные (PDU-фрайм)</param>
        /// <returns>результат выполнения запроса</returns>
        public Modbus.OSIModel.DataLinkLayer.RequestError SendMessage(
            Byte address, 
            Byte function, 
            Byte[] request, 
            out Byte[] answer)
        {
            List<Byte> message = new List<byte>();
            Byte[] package;
            int interval = 0;
            StringBuilder sb;

            if (_transaction != TransactionType.Undefined)
            {
                if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                {
                    sb = new StringBuilder();
                    sb.Append(address.ToString("X2"));
                    sb.Append(" ");
                    sb.Append(function.ToString("X2"));
                    
                    for (int i = 0; i < request.Length; i++)
                    {
                        sb.Append(request[i].ToString("X2"));
                        sb.Append(" ");
                    }

                    Trace.TraceError("{0}: Поток ID: {1} : Попытка отправки сообщения при активной транзакции! : {2}",
                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, sb.ToString());
                }
                answer = null;
                return RequestError.TransatcionError;
            }

            // Формируем запрос
            message.Add(address);
            message.Add(function);
            message.AddRange(request);
            // Контрольная сумма
            message.AddRange(Modbus.OSIModel.DataLinkLayer.CRC16.CalcCRC16(message.ToArray()));

            lock (_incomingBuffer)
            {
                _incomingBuffer.Clear();
            }

            // Если адрес нулевой, значит широковещательный запрос
            if (address == 0)
            {
                StartTransaction(TransactionType.BroadcastMode);
            }
            else
            {
                StartTransaction(TransactionType.UnicastMode);
            }

            // При отключении USB работающего через виртуальный COM-порт,
            // COM-порт удаляется. Программа об этом ничего не знает и считает что порт открыт.
            // Свойство IsOpen вернят труе, а перичесление портов системы будет 
            // содержать уже удалённый порт. Но при обращении к ресурсам порта, будет возникать
            // исключение.

            // Отправляем запрос
            try
            {
                _serialPort.Write(message.ToArray(), 0, message.Count);
            }
            catch (Exception ex)
            {
                // При записи в порт возникло исключение
                if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                {
                    Trace.TraceError("{0}: Поток ID: {1} : При отправке запроса, SerialPort вызвал исключение: {2}",
                        DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, ex.Message);
                }
                
                StopTransaction();
                answer = null;
                _serialPort = null;
                return RequestError.PortError;
            }

            // В зависимости от типа запроса устанавливаем значение
            // таймаута. В случае широковещаетльного запроса таймаут
            // является временной задержкой
            switch (_transaction)
            {
                case TransactionType.UnicastMode:
                    {
                        interval = _ValueTimeOut;
                        break;
                    }
                case TransactionType.BroadcastMode:
                    {
                        interval = _ValueTurnAroundDelay;
                        break;
                    }
            }

            // Ожидаем ответ
            if ((_MaskOfMessageLog & TypeOfMessageLog.Information) == TypeOfMessageLog.Information)
            {
                sb = new StringBuilder();
                for (int i = 0; i < message.Count; i++)
                {
                    sb.Append(message[i].ToString("X2"));
                    sb.Append(" ");
                }

                Trace.TraceInformation("{0}: Поток ID: {1} : Запрос отправлен. Засыпаем, ждём ответа : {2}",
                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, sb.ToString());
            }
            if (true == _timeOut.WaitOne(interval)) //
            {
                //StopTransaction();

                if (_error == RequestError.NoError)
                {
                    // Ошибок нет
                    // Проверяем Контрольную сумму
                    // Последнего принятого пакета

                    lock (_incomingBuffer)
                    {
                        package = _incomingBuffer.ToArray();
                    }

                    if (true == Modbus.OSIModel.DataLinkLayer.CRC16.VerefyCRC16(package))
                    {
                        // Контрольная сумма сошлась, проверяем адрес устройства
                        if (address == package[0])
                        {
                            // Адрес верен, проверяем код функции
                            if (function == ((Byte)(package[1] & 0x7F)))
                            {
                                // Код функции верен, возвращаем ответ
                                message.Clear();
                                message.AddRange(package);
                                // Удаляем два последних байта CRC16
                                message.RemoveRange(message.Count - 2, 2);
                                // Удаляем байт адреса Slave устройства
                                message.RemoveAt(0);
                                // Получаем: Код функции + данные ответного сообщения, 
                                // возвращаем их
                                answer = message.ToArray();
                                StopTransaction();
                                return RequestError.NoError;
                            }
                            else
                            {
                                // Код функции не соотвествует коду в запросе
                                if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                                {
                                    Trace.TraceError("{0}: Поток ID: {1} : Код функции не соотвествует коду в запросе",
                                        DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                                }
                                answer = null;
                                StopTransaction();
                                return RequestError.WrongFunction;
                            }
                        }
                        else
                        {
                            // Адрес не верен.
                            if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                            {
                                Trace.TraceError("{0}: Поток ID: {1} : Адрес подчинённого устройства в ответе несоответствует адресу в запросе",
                                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                            }
                            answer = null;
                            StopTransaction();
                            return RequestError.WrongAddress;
                        }
                    }
                    else
                    {
                        // Контрольная сумма не сошлась
                        if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                        {
                            Trace.TraceError("{0}: Поток ID: {1} : Неверная контрольная сумма CRC16",
                                DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                        }
                        answer = null;
                        StopTransaction();
                        return RequestError.CheckSumError;
                    }
                }
                else
                {
                    // При приёме возникла ошибка
                    if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                    {
                        Trace.TraceError("{0}: Поток ID: {1} : Ошибка при приёме: {2}",
                            DateTime.Now.ToLongTimeString(),
                            Thread.CurrentThread.ManagedThreadId, _error);
                    }
                    answer = null;
                    StopTransaction();
                    return _error;
                }
            }
            else
            {
                // Сработал таймер таймаута. В зависимости от 
                // типа запроса анализируем ситуацию
                switch (_transaction)
                {
                    case TransactionType.UnicastMode:
                        {
                            // Истёк таймаут
                            if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                            {
                                Trace.TraceError("{0}: Поток ID: {1} : Таймаут ответа на запрос",
                                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                                Trace.TraceError("{0}: Поток ID: {1} : Байт в приёмном буфере: {2}",
                                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId,
                                    _serialPort.BytesToRead.ToString());
                            }
                            StopTransaction();
                            answer = null;
                            return RequestError.TimeOut;
                        }
                    case TransactionType.BroadcastMode:
                        {
                            if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                            {
                                Trace.TraceError("{0}: Поток ID: {1} : Истек таймер широковещательной команды",
                                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                            }
                            StopTransaction();
                            answer = null;
                            return RequestError.TurnAroundDelay;
                        }
                    default:
                        {
                            if ((_MaskOfMessageLog & TypeOfMessageLog.Error) == TypeOfMessageLog.Error)
                            {
                                Trace.TraceError("{0}: Поток ID: {1} : Ошибка не определена",
                                    DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId);
                            }
                            StopTransaction();
                            answer = null;
                            return RequestError.UnknownError;
                        }
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает таймаут ответа на запрос, мсек
        /// </summary>
        /// <returns></returns>
        public int GetTimeOut()
        {
            return _ValueTimeOut;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает вемя задержки при широковещаетльной команде, мсек
        /// </summary>
        /// <returns></returns>
        public int GetTurnAroundDelay()
        {
            return _ValueTurnAroundDelay;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает тип физической линии
        /// </summary>
        /// <returns></returns>
        public InterfaceType GetTypeInterface()
        {
            return InterfaceType.SerialPort;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает состояние порта (открыт/закрыт)
        /// </summary>
        /// <returns></returns>
        public Boolean IsOpen()
        {
            return _serialPort.IsOpen;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает режим Modbus: RTU или ASCII
        /// </summary>
        /// <returns></returns>
        Modbus.OSIModel.DataLinkLayer.TransmissionMode IDataLinkLayer.GetMode()
        {
            return TransmissionMode.RTU;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает объект соединения
        /// </summary>
        /// <returns>Объект реализующий физический интерфейс соединения</returns>
        public object GetConnection()
        {
        //    SerialPortSettings settings;
        //    settings.PortName = _serialPort.PortName;
        //    settings.BaudRate = _serialPort.BaudRate;
        //    settings.DataBits = _serialPort.DataBits;
        //    settings.StopBits = _serialPort.StopBits;
        //    settings.Parity = _serialPort.Parity;
        //    settings.TimeOut = _ValueTimeOut;
        //    settings.AroundDelay = _ValueTurnAroundDelay;
        //    settings.Mode = TransmissionMode.RTU;
        //    return (Object)settings;
            return (object)this;
        }
        //---------------------------------------------------------------------------
        #endregion
    }
}