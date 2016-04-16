using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Timers;
using System.Diagnostics;
using System.Text;
using Modbus.OSIModel.Transaction;
using Modbus.OSIModel.Message;

namespace Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort
{
    /// <summary>
    /// Класс реализует слой DataLink Layer протокола Modbus
    /// соединение через COM-порт для slave-устройства
    /// </summary>
    public class ComPortSlaveMode: IDataLinkLayer
    {
        #region Fields and Properties
        /// <summary>
        /// Последовательный порт
        /// </summary>
        private SerialPort _SerialPort;
        /// <summary>
        /// Последовательный порт
        /// </summary>
        public SerialPort SerialPort
        {
            get { return _SerialPort; }
        }
        /// <summary>
        /// Наименование порта
        /// </summary>
        public String PortName
        {
            get { return _SerialPort.PortName; }
        }
        /// <summary>
        /// Состояние приёмопередатчика
        /// </summary>
        private Transaction.Transaction _СurrentTransaction;
        /// <summary>
        /// Возвращает копию объекта текущей транзакции 
        /// </summary>
        public Transaction.Transaction СurrentTransaction
        {
            get { return _СurrentTransaction.DeepCopy(); }
        }
        /// <summary>
        /// Таймер определения конца запроса
        /// </summary>
        private Timer _TimerInterFrameDelay;
        /// <summary>
        /// Таймер таймаута текущей транзакции "адресованный запрос - ответ".
        /// Если подчинённое устройство не отвечает период таймера, то транзакция
        /// завершается соответствующей ошибкой.
        /// </summary>
        private Timer _TimerTimeoutCurrentTransaction;
        /// <summary>
        /// Объект для синхронизации операций.
        /// </summary>
        private Object _SyncRoot;

        #endregion

        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public ComPortSlaveMode()
        {
            _SyncRoot = new Object();
            _СurrentTransaction = 
                new Modbus.OSIModel.Transaction.Transaction(TransactionType.Undefined, null);

            _TimerInterFrameDelay = new Timer(20);
            _TimerInterFrameDelay.AutoReset = false;
            _TimerInterFrameDelay.Elapsed +=
                new ElapsedEventHandler(EventHandler_TmrInterFrameDelay_Elapsed);
            _TimerInterFrameDelay.Stop();

            _TimerTimeoutCurrentTransaction = new Timer();
            _TimerTimeoutCurrentTransaction.AutoReset = false;
            _TimerTimeoutCurrentTransaction.Interval = 200;
            _TimerTimeoutCurrentTransaction.Elapsed +=
                new ElapsedEventHandler(EventHandler_TimerTimeoutCurrentTransaction_Elapsed);
            _TimerTimeoutCurrentTransaction.Stop();

            // Настраиваем последовательный порт
            _SerialPort =
                new System.IO.Ports.SerialPort("COM1", 19200, Parity.Even, 8, StopBits.One);
            
            _SerialPort.ErrorReceived += new
                SerialErrorReceivedEventHandler(EventHandler_SerialPort_ErrorReceived);
            _SerialPort.DataReceived +=
                new SerialDataReceivedEventHandler(EventHandler_SerialPort_DataReceived);
            _SerialPort.ReceivedBytesThreshold = 1;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Сетевой адрес устройства</param>
        /// <param name="baudRate">Скорость соединения</param>
        /// <param name="parity">Наличие паритета данных</param>
        /// <param name="dataBits">Количество бит в символе</param>
        /// <param name="stopBits">Количество стоп-бит</param>
        public ComPortSlaveMode(String portName, 
            int baudRate, Parity parity,
            int dataBits, StopBits stopBits)
        {
            _SyncRoot = new Object();
            _СurrentTransaction =
                new Modbus.OSIModel.Transaction.Transaction(TransactionType.Undefined, null);

            _TimerInterFrameDelay = new Timer(20);
            _TimerInterFrameDelay.AutoReset = false;
            _TimerInterFrameDelay.Elapsed +=
                new ElapsedEventHandler(EventHandler_TmrInterFrameDelay_Elapsed);
            _TimerInterFrameDelay.Stop();

            _TimerTimeoutCurrentTransaction = new Timer();
            _TimerTimeoutCurrentTransaction.AutoReset = false;
            _TimerTimeoutCurrentTransaction.Interval = 200;
            _TimerTimeoutCurrentTransaction.Elapsed += 
                new ElapsedEventHandler(EventHandler_TimerTimeoutCurrentTransaction_Elapsed);
            _TimerTimeoutCurrentTransaction.Stop();

            // Настраиваем последовательный порт
            _SerialPort =
                new System.IO.Ports.SerialPort(portName, baudRate,
                    parity, dataBits, stopBits);
            _SerialPort.ErrorReceived += new
                SerialErrorReceivedEventHandler(EventHandler_SerialPort_ErrorReceived);
            _SerialPort.DataReceived +=
                new SerialDataReceivedEventHandler(EventHandler_SerialPort_DataReceived);
            _SerialPort.ReceivedBytesThreshold = 1;
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Methods
        /// <summary>
        /// Обработчик события срабатываения таймера таймаута текущей транзакции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TimerTimeoutCurrentTransaction_Elapsed(
            object sender, ElapsedEventArgs e)
        {
            String msg;
            Timer timer = (Timer)sender;
            Debug.WriteLine("Timer Elapsed: " + _TimerTimeoutCurrentTransaction.Enabled.ToString());
            timer.Stop();
            
            // Прерываем текущую транзакцию.
            if (_СurrentTransaction.TransactionType == TransactionType.UnicastMode)
            {
                if (_СurrentTransaction.IsRunning)
                {
                    _СurrentTransaction.Abort("Подчинённое устройство не ответило за заданное время");
                }
                else
                {
                    msg = "Сработал таймер таймаута ответа подчинённого " +
                        "устройства, во время текущей транзакции имеющей статус завершена";
                    //throw new Exception(msg);
                    OnErrorOccurred(new ErrorOccurredEventArgs(PortError.UnknownError, msg));
                }
            }
            else
            {
                msg = "Сработал таймер таймаута ответа подчинённого " +
                    "устройства, во время текущей транзакции имеющей тип отличный от UnicastMode";
                //throw new Exception(msg);
                OnErrorOccurred(new ErrorOccurredEventArgs(PortError.UnknownError, msg));
            }
            return;
        }
        /// <summary>
        /// Обработчик события срабатывания межкадрового таймера 
        /// </summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="e">Аргументы события</param>
        private void EventHandler_TmrInterFrameDelay_Elapsed(
            object sender, ElapsedEventArgs e)
        {
            Byte[] message;
            Message.Message request;
            Timer tmr = (Timer)sender;
            tmr.Stop();

            // Таймер сработал, значит сообщение (запрос) 
            // принят полностью считаем CRC16            
            message = new byte[_SerialPort.BytesToRead];
            _SerialPort.Read(message, 0, _SerialPort.BytesToRead);

            if (true == Modbus.OSIModel.DataLinkLayer.CRC16.VerefyCRC16(message))
            {
                // Запрос принят корректно
                Byte[] array = new byte[(message.Length - 4)]; //Данные в сообщениии
                Array.Copy(message, 2, array, 0, array.Length); // Выделили данные из сообщения
                
                request = new Message.Message(message[0], message[1], array);
                
                if (request.Address == 0)
                {
                    // Формируем событие приёма широковещаетельного запроса
                    _СurrentTransaction = new Transaction.Transaction(
                        TransactionType.BroadcastMode, request);
                    _СurrentTransaction.TransactionWasEnded += 
                        new EventHandler(EventHandler_СurrentTransaction_TransactionWasEnded);
                    _СurrentTransaction.Start(); // Стартуем транзакцию
                    OnRequestWasRecived(new MessageEventArgs(request));
                    // ??? Можно было бы запустить таймер задержки при широковещаетльном запросе
                    // Сейчас не реализовано. По этому, завершам транзакцию сразу.
                    _СurrentTransaction.Stop(null); // Завершает транзакцию 
                }
                else
                {
                    _СurrentTransaction = new Transaction.Transaction(
                        TransactionType.UnicastMode, request);
                    _СurrentTransaction.TransactionWasEnded +=
                        new EventHandler(EventHandler_СurrentTransaction_TransactionWasEnded);
                    _СurrentTransaction.Start();
                    // Запускаем таймер таймаута ответа подчинённого устройства
                    _TimerTimeoutCurrentTransaction.Start();
                    // Формируем событие приёма адресованного запроса. 
                    // Транзакция здесь продолжается до тех пор, покак не будет
                    // отправлен ответ.
                    OnRequestWasRecived(new MessageEventArgs(request));
                }
            }
            else
            {
                // Принят запрос с неверным CRC16
                OnErrorOccurred(new ErrorOccurredEventArgs
                    (PortError.CheckSumError, "Принят запрос с неверным CRC16"));
            }
            return;
        }
        /// <summary>
        /// Обработчик события окончания текущей транзакции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EventHandler_СurrentTransaction_TransactionWasEnded(
            object sender, EventArgs e)
        {
            String msg;
            Transaction.Transaction transaction = 
                (Transaction.Transaction)sender;
            
            // Анализируем причины завершения транзакции
            // !!! Здесь можно реализовать стек отработавший транзакций и поместить транзакцию туда
            switch (transaction.Status)
            {
                case TransactionStatus.Aborted:
                    {
                        msg = String.Format("Transaction ID: {0}; Статус: {1}; Описание ошибки: {2}; " +
                            "Начало транзакции: {3} мсек; Конец транзакции: {4} мсек; Время транзакции: {5} мсек",
                            transaction.Identifier.ToString(), transaction.Status, transaction.DescriptionError,
                            transaction.TimeOfStart, transaction.TimeOfEnd, transaction.TimeOfTransaction);   
                        Trace.TraceInformation(msg);
                        break; 
                    }
                case TransactionStatus.Completed:
                    {
                        msg = String.Format("Transaction ID: {0}; Статус: {1}; Описание ошибки: {2}; " +
                            "Начало транзакции: {3} мсек; Конец транзакции: {4} мсек; Время транзакции: {5} мсек",
                            transaction.Identifier.ToString(), transaction.Status, transaction.DescriptionError,
                            transaction.TimeOfStart, transaction.TimeOfEnd, transaction.TimeOfTransaction);
                        Trace.TraceInformation(msg);
                        break; 
                    }
                case TransactionStatus.NotInitialized:
                    {
                        throw new Exception(String.Format(
                            "Обнаружен неверный статус завершённой транзакции - {0}", transaction.Status));
                    }
                case TransactionStatus.Running:
                    {
                        throw new Exception(String.Format(
                            "Обнаружен неверный статус завершённой транзакции - {0}", transaction.Status));
                    }
                default:
                    {
                        throw new Exception(
                            "Полученное состояние транзакции не поддерживается в текущей версии ПО"); 
                    }
            }

            //_SerialPort.DiscardOutBuffer();
            //_SerialPort.DiscardInBuffer();

            return;
        }
        /// <summary>
        /// Обработчик события приёма байт из COM-порта
        /// </summary>
        /// <param name="sender">Отправитель события</param>
        /// <param name="e">Аргументы события</param>
        private void EventHandler_SerialPort_DataReceived(
            object sender, SerialDataReceivedEventArgs e)
        {
            Byte[] data;
            StringBuilder sb;
            
            // Сбрасываем межкадровый таймер
            _TimerInterFrameDelay.Stop();

            if (!_СurrentTransaction.IsRunning)
            {
                if (_SerialPort.BytesToRead == 0)
                {
                    Debug.WriteLine("Принята нулевая посылка");
                    return;
                }
                else
                {
                    // После принятия очердной порции байт запроса
                    // Запускаем межкадровый таймер
                    _TimerInterFrameDelay.Start();
                }
            }
            else
            {
                // Принято сообщение при активной транзакции обработке ранее принятого
                // сообщения. Прочитали (опустошили буфер) и забыли
                if (_SerialPort.BytesToRead != 0)
                {
                    data = new byte[_SerialPort.BytesToRead];
                    _SerialPort.Read(data, 0, data.Length);
                }
                else
                {
                    data = new byte[0];
                }
                
                // Генерируем события ошибки
                sb = new StringBuilder(50);
                sb.AppendFormat("Modbus ComPort {0} Приняты данные при активной транзакции запрос-ответ: ", 
                    _SerialPort.PortName);

                foreach (Byte var in data)
                {
                    sb.AppendFormat("0x{0:X2} ", var);                    
                }

                OnErrorOccurred(new ErrorOccurredEventArgs(
                    PortError.TransatcionError, sb.ToString().Trim()));
            }

            return;
        }
        /// <summary>
        /// Обработчик события возникновения ошибки при работе COM-порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_SerialPort_ErrorReceived(
            object sender, SerialErrorReceivedEventArgs e)
        {
            String msg;
            PortError _error;
            _error = (PortError)e.EventType;

            msg = String.Format(
                "Modbus ComPort {0}. Ошибка при приёме данных", _SerialPort.PortName);

            // Останавливаем таймер
            _TimerInterFrameDelay.Stop();

            if (_СurrentTransaction != null)
            {
                if (_СurrentTransaction.IsRunning)
                {
                    _СurrentTransaction.Abort(msg);
                }
            }
          
            // Генерируем соыбытие
            OnErrorOccurred(new ErrorOccurredEventArgs(_error, msg));
            return;
        }
        /// <summary>
        /// Генерирует событие приёма сообщения
        /// </summary>
        /// <param name="args"></param>
        private void OnRequestWasRecived(MessageEventArgs args)
        {
            EventHandlerRequestWasRecived handler = RequestWasRecived;

            if (handler != null)
            {
                foreach (EventHandlerRequestWasRecived singleCast in handler.GetInvocationList())
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
        /// Генерирует событие отправки сообщения
        /// </summary>
        /// <param name="args">Аргументы события</param>
        private void OnResponseWasSent(MessageEventArgs args)
        {
            EventHandleResponseWasSent handler = ResponseWasSent;

            if (handler != null)
            {
                foreach (EventHandleResponseWasSent singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke = singleCast.Target as
                        System.ComponentModel.ISynchronizeInvoke;
                    
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
        /// Генерирует событие ошибки при работе com-порта
        /// </summary>
        /// <param name="args">Аргументы события</param>
        private void OnErrorOccurred(ErrorOccurredEventArgs args)
        {
            EventHandlerErrorOccurred handle = ErrorOccurred;

            if (handle != null)
            {
                foreach (EventHandlerErrorOccurred singleCast in handle.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke synckInvoke = 
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (synckInvoke != null)
                    {
                        if (synckInvoke.InvokeRequired == true)
                        {
                            synckInvoke.Invoke(singleCast, new Object[] { this, args });
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
        /// Освобождаем ресурсы используемые портом
        /// </summary>
        public void Dispose()
        {
            if (_TimerInterFrameDelay != null)
            {
                _TimerInterFrameDelay.Stop();
                _TimerInterFrameDelay.Dispose();
            }
            
            if (_TimerTimeoutCurrentTransaction != null)
            {
                _TimerTimeoutCurrentTransaction.Stop();
                _TimerTimeoutCurrentTransaction.Dispose();
            }

            if (_SerialPort != null)
            {
                SerialPort.Dispose();
            }
            return;
        }
        #endregion

        #region Events
        /// <summary>
        /// Событие происходит по принятию запроса от мастера сети
        /// </summary>
        public event EventHandlerRequestWasRecived RequestWasRecived;
        /// <summary>
        /// Событие происходит после оправки ответа на запрос
        /// </summary>
        public event EventHandleResponseWasSent ResponseWasSent;
        /// <summary>
        /// Событие происходит при возниконовении ошибочной ситуации
        /// </summary>
        public event EventHandlerErrorOccurred ErrorOccurred;
        #endregion

        #region Члены IDataLinkLayer

        public void Open()
        {
            _SerialPort.Open();
        }

        public void Close()
        {
            _SerialPort.Close();
        }

        public void SendResponse(Message.Message answer)
        {
            // Останавливаем таймер таймаута
            _TimerTimeoutCurrentTransaction.Stop();
           
            Byte[] array = answer.ToArray();
            // Отсылаем ответ
            _SerialPort.Write(array, 0, array.Length);
            // Останавливаем транзакцию
            _СurrentTransaction.Stop(answer);
            // Формирует событие
            OnResponseWasSent(new MessageEventArgs(answer));

            return;
        }

        public InterfaceType TypeInterface
        {
            get
            {
                return InterfaceType.SerialPort;
            }
        }

        public bool IsOpen
        {
            get
            {
                return _SerialPort.IsOpen;
            }
        }

        public TransmissionMode Mode
        {
            get
            {
                return TransmissionMode.RTU;
            }
        }

        //object IDataLinkLayer.Connection
        //{
        //    get
        //    {
        //        return (Object)this;
        //    }
        //}

        event EventHandlerRequestWasRecived IDataLinkLayer.RequestWasRecived
        {
            add
            {
                lock (_SyncRoot)
                {
                    RequestWasRecived += value;
                }
            }
            remove 
            {
                lock (_SyncRoot)
                {
                    RequestWasRecived -= value;
                }
            }
        }

        event EventHandleResponseWasSent IDataLinkLayer.ResponseWasSent
        {
            add 
            {
                lock (_SyncRoot)
                {
                    ResponseWasSent += value;
                }
            }
            remove 
            {
                lock (_SyncRoot)
                {
                    ResponseWasSent -= value;
                }
            }
        }

        event EventHandlerErrorOccurred IDataLinkLayer.ErrorOccurred
        {
            add 
            {
                lock (_SyncRoot)
                {
                    ErrorOccurred += value;
                }
            }
            remove 
            {
                lock (_SyncRoot)
                {
                    ErrorOccurred -= value;
                }
            }
        }
        #endregion
    }
}