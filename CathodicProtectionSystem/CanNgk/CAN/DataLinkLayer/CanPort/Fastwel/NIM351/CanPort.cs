using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataLinkLayer.CanPort;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Convert;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351
{
    /// <summary>
    /// Класс реализует функционал работы с портом CAN устройства Faswel NIM-351
    /// </summary>
    [Serializable]
    [Description("CAN-порт Fastwel NIM-351")]
    public class CanPort: CanPortBase //ICanPort
    {
        #region Fields And Properties
        /// <summary>
        /// Хранит дескриптор устройства CAN
        /// </summary>
        [NonSerialized]
        private SafeFileHandle _DeviceHandle;
        /// <summary>
        /// Наименование порта. Храниться в виде строки в формате [CAN][Номер порта: int]
        /// </summary>
        private String _PortName;
        /// <summary>
        /// Скорость сетевого обмена.
        /// </summary>
        private BaudRate _BitRate = BaudRate.BR10;
        /// <summary>
        /// Режим работы порта
        /// </summary>
        private PortMode _OpMode = PortMode.NORMAL;
        /// <summary>
        /// Формат кадров (поле ID: 11 или 29 бит) с которыми работает порт
        /// могут складыватся по "ИЛИ" для режима MIXED
        /// </summary>
        private FrameFormat _FrameFormat = FrameFormat.MixedFrame;
        /// <summary>
        /// Разрешить приём сообщения Error Frame
        /// </summary>
        private Boolean _ErrorFrameEnable = true;
        /// <summary>
        /// Хранит последний прочитанный из адаптера статус
        /// </summary>
        private F_CAN_STATE _PortStatus = F_CAN_STATE.CAN_STATE_STOPPED;
        /// <summary>
        /// структура таймаутов CAN-адаптера
        /// </summary>
        /// <remarks>Все поля инициализированы нулевыми значениями</remarks>
        private F_CAN_TIMEOUTS _Timeouts;
        /// <summary>
        /// Значение таймаута (мсек) при чтении данных
        /// </summary>
        /// <remarks>
        /// Используется при вызове функции Api.fw_can_recv(...)
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Таймаут чтения")]
        [Description("Значение таймаута в мсек. Определяет значение таймаута, используемого функцией " +
            "fw_can_recv() при чтении данных")]
        [DefaultValue(0)]
        public UInt32 ReadTotalTimeout
        {
            get
            {
                return _Timeouts.ReadTotalTimeout;
            }
            set
            {
                this._Timeouts.ReadTotalTimeout = value;
            }
        }
        /// <summary>
        /// Определяет совместно с WriteTotalTimeoutMultiplier значение таймаута 
        /// при передачи кадров функцией Api.fw_can_send по формуле
        /// Tsend = N * WriteTotalTimeoutMultiplier + WriteTotalTimeoutConstant (мсек)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("WriteTotalTimeoutConstant")]
        [Description("Используется для вычисления тайаута при вызове fw_can_send(). " +
            "Более подробно см. документацию на структуру данных F_CAN_TIMEOUTS")]
        [DefaultValue(0)]        
        public UInt32 WriteTotalTimeoutConstant
        {
            get
            {
                return this._Timeouts.WriteTotalTimeoutConstant;
            }
            set
            {
                this._Timeouts.WriteTotalTimeoutConstant = value;
            }
        }
        /// <summary>
        /// Определяет совместно с WriteTotalTimeoutConstant значение таймаута 
        /// при передачи кадров функцией Api.fw_can_send по формуле
        /// Tsend = N * WriteTotalTimeoutMultiplier + WriteTotalTimeoutConstant (мсек)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("WriteTotalTimeoutMultiplier")]
        [Description("Используется для вычисления тайаута при вызове fw_can_send(). " +
            "Более подробно см. документацию на структуру данных F_CAN_TIMEOUTS")]
        [DefaultValue(0)]  
        public UInt32 WriteTotalTimeoutMultiplier
        {
            get
            {
                return this._Timeouts.WriteTotalTimeoutMultiplier;
            }
            set
            {
                this._Timeouts.WriteTotalTimeoutMultiplier = value;
            }
        }
        /// <summary>
        /// Таймаут (мсек) используется адаптером для автоматического восстановления
        /// из состояния CAN_STATE_BUS_OFF. Если таймаут равен 0, то восстановление
        /// не производится, в противном случае по истечения временного интервала
        /// будет произведён аппаратный сброс адаптера
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("RestartBusoffTimeout")]
        [Description("Таймаут (мсек) используется адаптером для автоматического восстановления " +
            "из состояния CAN_STATE_BUS_OFF. Если таймаут равен 0, то восстановление не производится," +
            "в противном случае по истечению временного интервала будет произведён аппаратный сброс адаптера")]
        public UInt32 RestartBusoffTimeout
        {
            get
            {
                return _Timeouts.RestartBusoffTimeout;
            }
            set
            {
                this._Timeouts.RestartBusoffTimeout = value;
            }
        }
        /// <summary>
        /// Возвращает структуру значений счётчиков адаптера
        /// </summary>
        private F_CAN_STATS Statistics
        {
            get { return this.GetStatistics(); }
        }
        /// <summary>
        /// Поток для обработки очереди входящих сообщений
        /// </summary>
        [NonSerialized]
        private Thread _ThreadForInput;
        /// <summary>
        /// Входной буфер сообщений
        /// </summary>
        [NonSerialized]
        private Queue<Frame> _InputBufferMessages;
        /// <summary>
        /// Quit flag for the receive thread.
        /// </summary>
        [NonSerialized]
        private int _FlagMustQuit = 0;
        /// <summary>
        /// Оъект для сихронизации доступа к ресурсам.
        /// </summary>
        [NonSerialized]
        protected static Object _SyncRoot = new Object();

        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public CanPort()
        {
            // Инициализируем бужер входящих сообщений
            this._InputBufferMessages = new Queue<Frame>(100);

            // Инициализируем дескриптор устройства
            this._DeviceHandle = new SafeFileHandle(IntPtr.Zero, true);
            this._DeviceHandle.Close();

            // Структура таймаутов
            _Timeouts = new F_CAN_TIMEOUTS();
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="portNumber">Номер CAN-порта</param>
        public CanPort(Int32 portNumber)
        {
            // Инициализируем буфер входящих сообщений
            this._InputBufferMessages = new Queue<Frame>(100);

            // Инициализируем дескриптор устройства
            this._DeviceHandle = new SafeFileHandle(IntPtr.Zero, true);
            this._DeviceHandle.Close();

            // Номер порта
            if (portNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "portNumber", 
                    "Недопустимое значение. Допускаются только положительные значения, больше нуля");
            }
            else
            {
                this.PortName = String.Format("CAN{0}", portNumber);
            }

            // Структура таймаутов
            _Timeouts = new F_CAN_TIMEOUTS();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="bitRate"></param>
        /// <param name="frameFormat"></param>
        /// <param name="mode"></param>
        public CanPort(String portName, BaudRate bitRate, FrameFormat frameFormat,
            PortMode mode)
        {
            // Инициализируем буфер входящих сообщений
            this._InputBufferMessages = new Queue<Frame>(100);

            // Инициализируем дескриптор устройства
            this._DeviceHandle = new SafeFileHandle(IntPtr.Zero, true);
            this._DeviceHandle.Close();

            // Номер порта
            PortName = portName;
            _BitRate = bitRate;
            _FrameFormat = frameFormat;
            _OpMode = mode;

            // Структура таймаутов
            _Timeouts = new F_CAN_TIMEOUTS();
        }
        /// <summary>
        /// Конструктор для десериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public CanPort(SerializationInfo info, StreamingContext context)
        {
            this._InputBufferMessages = new Queue<Frame>(100);

            // Инициализируем дескриптор устройства
            this._DeviceHandle = new SafeFileHandle(IntPtr.Zero, true);
            this._DeviceHandle.Close();

            this._PortName = info.GetString("PortName");
            this._BitRate = (BaudRate)info.GetValue("BitRate", typeof(BaudRate));
            this._OpMode = (PortMode)info.GetValue("Mode", typeof(PortMode));
            this._ErrorFrameEnable = info.GetBoolean("ErrorFrameEnable");
            this._FrameFormat = (FrameFormat)info.GetValue("FrameFormat", typeof(FrameFormat));
            this._Timeouts = (F_CAN_TIMEOUTS)info.GetValue("Timeouts", typeof(F_CAN_TIMEOUTS));
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Читает состояние CAN-адаптера
        /// </summary>
        /// <returns>Текущее состояние CAN-адаптера</returns>
        private F_CAN_STATE GetPortStatus()
        {
            F_CAN_RESULT result;
            F_CAN_STATE state;
            String msg;

            if ((this._DeviceHandle.IsClosed) || (_DeviceHandle.IsInvalid))
            {
                return F_CAN_STATE.CAN_STATE_STOPPED;
            }
            else
            {
                result = Api.fw_can_get_controller_state(this._DeviceHandle, out state);

                if (Api.f_can_success(result))
                { return state; }
                else
                {
                    msg = String.Format(
                        "Неудалось прочитать состояние CAN-адаптера. Драйвер вернул: {0}",
                        result.ToString());
                    throw new InvalidOperationException(msg);
                }
            }
        }
        /// <summary>
        /// Читает текущий статус адаптера и сравнивает с предыдущим. Если 
        /// отличается генерирует событие и сохраняет новый статус
        /// </summary>
        /// <returns>true - если статус изменился</returns>
        private Boolean UpdatePortStatus()
        {
            F_CAN_STATE newStatus;
            Boolean result;

            if (this.IsOpen)
            {
                newStatus = this.GetPortStatus();

                if (this._PortStatus != newStatus)
                {
                    // Статус изменился.
                    this._PortStatus = newStatus;
                    this.OnPortChangesStatus(ConvertNim351.ConvertToCanPortStatus(this._PortStatus));
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                // Если порт закрыт, есго статус должен быть F_CAN_STATE.CAN_STATE_STOPPED.
                // Если это не так, то устанавливаем и генерируем событие
                if (this._PortStatus == F_CAN_STATE.CAN_STATE_STOPPED)
                {
                    result = false;
                }
                else
                {
                    // Статус изменился.
                    this._PortStatus = F_CAN_STATE.CAN_STATE_STOPPED;
                    this.OnPortChangesStatus(ConvertNim351.ConvertToCanPortStatus(this._PortStatus));
                    result = true;
                }
            }

            return result;
        }
        /// <summary>
        /// Метод для чтения порта. Запускается во вторичном потоке. Поток на чтение запускается при открытии порта 
        /// и завершается при закрытии.
        /// </summary>
        private void HandleQueueIncomingMessages()
        {
            String msg;
            F_CAN_RESULT result;
            F_CAN_RX buffer;
            F_CAN_WAIT wait;
            UInt32 timeout = 500; // мсек
            Frame message;
            //uint count;

            // Если прот закрыт, чтение его буфера не возможно
            //if (this.IsOpen == false)
            //{
            //    msg = "Невозможно прочитать очередь сообщений, CAN-порт закрыт";
            //    throw new InvalidOperationException(msg);
            //}

            // Читаем порт всё время пока открыт
            while (this._FlagMustQuit > 0)
            {
                wait.waitMask = F_CAN_STATUS.CAN_STATUS_EMPTY | F_CAN_STATUS.CAN_STATUS_TXBUF;
                wait.status = F_CAN_STATUS.CAN_STATUS_EMPTY;

                result = Api.fw_can_wait(this._DeviceHandle, ref wait, timeout);

                if ((result == F_CAN_RESULT.CAN_RES_OK) ||
                    (result == F_CAN_RESULT.CAN_RES_TIMEOUT))
                {
                    if (result == F_CAN_RESULT.CAN_RES_TIMEOUT)
                    {
                        //Console.WriteLine("Сработал таймаут ожидания");
                        // Произошло что-то нехорошее, нужно анализировать и что-то делать
                        // Проверяем текущий статус
                        //if (wait.status == wait.waitMask)
                        //{ }
                        // Останавливаем порт
                        this.Stop();
                        // Генерируем событие 
                        OnErrorReceived(ERROR.Other);
                        // Закрываем порт
                        this.Close();
                        // Завершаем работу потока
                        Interlocked.Exchange(ref this._FlagMustQuit, 0);
                        continue;
                    }

                    // Если  флаг установлен, то есть принятые сообщения. Читаем их
                    // и помещаем в буфер исходящих сообщений
                    if (F_CAN_STATUS.CAN_STATUS_RXBUF == 
                        (wait.status & F_CAN_STATUS.CAN_STATUS_RXBUF))
                    {
                        // Читаем буфер адаптера
                        result = Api.fw_can_peek_message(_DeviceHandle, out buffer);
                        // Разбираем принятое сообщение и формируем событие
                        message = new Frame();
                        message = ConvertNim351.ConvertToFrame(buffer.msg);
                        // Помещает принятое сообщение в буфер входных сообщений и генерируем событие приёма
                        lock (_SyncRoot)
                        {
                            this._InputBufferMessages.Enqueue(message);
                        }
                        this.OnMessageReceived();
                    }

                    // Произошла ошибка читаем её и сбрасываем 
                    if (F_CAN_STATUS.CAN_STATUS_ERR == 
                        (wait.status & F_CAN_STATUS.CAN_STATUS_ERR))
                    {
                        F_CAN_ERRORS errors;
                        result = Api.fw_can_get_clear_errors(this._DeviceHandle, out errors);

                        if (Api.f_can_success(result))
                        {
                            // Анализируем счётчики и генерируем событие
                            this.OnErrorReceived(ERROR.Other);
                        }
                        else
                        {
                            msg = String.Format(
                                "При чтении и сбросе счётчика ошибок CAN-адаптера возникла ошибка: {0}", result);
                            throw new InvalidOperationException(msg);
                        }
                    }
                }
                else
                {
                    msg = String.Format("При выполнении функции fw_can_wait произошла ошибка: {0}", 
                        Enum.GetName(typeof(F_CAN_RESULT), result));
                    throw new InvalidOperationException(msg);
                }
            }
            return;
        }
        /// <summary>
        /// Метод для генерации события приёма сообщения
        /// </summary>
        private void OnMessageReceived()
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
        /// Генерирует событие по изменению статуса порта
        /// </summary>
        /// <param name="status">Новое состояние CAN-порта</param>
        private void OnPortChangesStatus(CanPortStatus status)
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
        /// <summary>
        /// Генерирует событие при возникновении ошибок в CAN адаптере
        /// </summary>
        private void OnErrorReceived(ERROR error)
        {
            EventHandlerErrorRecived handler = this.ErrorReceived;
            EventArgsLineErrorRecived args = new EventArgsLineErrorRecived(error);   
            
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
                    {
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// Очищает очередь сообщений входного буфера CAN-адаптера
        /// </summary>
        public void DiscardInBuffer()
        {
            String message;
            F_CAN_RESULT result;

            if (this.IsOpen)
            {
                result = Api.fw_can_purge(_DeviceHandle,
                    F_CAN_PURGE_MASK.CAN_PURGE_RXABORT | F_CAN_PURGE_MASK.CAN_PURGE_RXCLEAR);

                if (result != F_CAN_RESULT.CAN_RES_OK)
                {
                    message = String.Format("Не удалось очистить входной буфер CAN адаптера. Error: ", result);
                    throw new InvalidOperationException(message);
                }
            }
            return;
        }
        /// <summary>
        /// Возвращает тип миросхемы CAN порта.
        /// </summary>
        /// <param name="controllerType"></param>
        /// <returns>Результат операции чтения</returns>
        public Boolean GetControllerType(out F_CAN_CONTROLLER controllerType)
        {
            String message;
            F_CAN_RESULT result;
            F_CAN_SETTINGS settings;

            if (this.IsOpen)
            {
                // Получаем текущие настройки
                result = Api.fw_can_get_controller_config(_DeviceHandle, out settings);

                if (!Api.f_can_success(result))
                {
                    message = String.Format("Ошибка при чтении настроек CAN адаптера, Error: {0}", result);
                    throw new InvalidOperationException(message);
                }

                controllerType = settings.controller_type;
                return true;
            }
            else
            {
                controllerType = F_CAN_CONTROLLER.UNKNOWN_CAN_DEVICE;
                return false;
            }
        }
        /// <summary>
        /// Считает и сбрасывает счётчики ошибок CAN-адаптера
        /// </summary>
        /// <returns>Счётчики ошибок CAN-адаптера</returns>
        public F_CAN_ERRORS GetAndClearErrorCounters()
        {
            String message;
            F_CAN_RESULT result;
            F_CAN_ERRORS errCounters;

            if (this.IsOpen)
            {
                result = Api.fw_can_get_clear_errors(this._DeviceHandle, out errCounters);

                if (!Api.f_can_success(result))
                {
                    message = String.Format(
                        "Ошибка при попытке получить счётчики ошибок CAN-адаптера, Error: ",
                        result);
                    throw new InvalidOperationException(message);
                }
            }
            else
            {
                message = "Метод не может быть выполнен, порт закрыт";
                throw new InvalidOperationException(message);
            }
            return errCounters;
        }
        /// <summary>
        /// Возаращает статистическую информацию CAN-адаптера
        /// </summary>
        /// <returns>Структура с информацией</returns>
        public F_CAN_STATS GetStatistics()
        {
            String message;
            F_CAN_RESULT result;
            F_CAN_STATS statistics;
            
            if (this.IsOpen)
            {
                result = Api.fw_can_get_stats(_DeviceHandle, out statistics);

                if (!Api.f_can_success(result))
                {
                    message = String.Format("Ошибка при получении статистики CAN-адаптера, Error: ", result);
                    throw new InvalidOperationException(message);
                }
            }
            else
            {
                //message = "Метод не может быть выполнен, порт закрыт";
                //throw new InvalidOperationException(message);
                statistics = new F_CAN_STATS(); // Возвращаем нулевые счётчики
            }
            return statistics;
        }
        /// <summary>
        /// Очищает счётчики
        /// </summary>
        public void ClearStatistics()
        {
            String msg;
            F_CAN_RESULT result;

            if (this.IsOpen)
            {
                result = Api.fw_can_clear_stats(this._DeviceHandle);
                if (!Api.f_can_success(result))
                {
                    msg = String.Format("Ошибка при выполнении сброса статистических счётчиков. " +
                        "Функция вернула: {0}", result);
                    throw new InvalidOperationException(msg);
                }
            }
            return;
        }
        /// <summary>
        /// Создаёт и запускает поток на прослущиваения порта
        /// </summary>
        private void CreateAndStartThreadForListenPort()
        {
            //String msg;

            // Устанавливаем флаг начала опроса порта
            Interlocked.Exchange(ref _FlagMustQuit, Int32.MaxValue);

            if (this._ThreadForInput != null)
            {
                if (this._ThreadForInput.IsAlive)
                {
                    //msg = String.Format(
                    //    "Ошибка. Попытка создать поток на прослушивание CAN-порта." +
                    //    "Поток уже существует и работает");
                    //throw new InvalidOperationException(msg);
                    return;
                }
            }
            
            // Запускаем поток на чтение порта
            this._ThreadForInput = new Thread(new ThreadStart(this.HandleQueueIncomingMessages));
            this._ThreadForInput.Name = String.Format("Thread_{0}_ForPortReading", this.PortName);
            this._ThreadForInput.Priority = ThreadPriority.Normal;
            this._ThreadForInput.IsBackground = true;
            this._ThreadForInput.Start();
            return;
        }
        /// <summary>
        /// Завершает выполнение потка на прослушивание CAN-порта
        /// </summary>
        private void StopThreadForListen()
        {
            Interlocked.Exchange(ref _FlagMustQuit, 0);

            if (this._ThreadForInput != null)
            {
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
                    //msg = String.Format(
                    //    "{0}: class CanPort.Close(): Рабочий поток не завершился за 0,5 секунду и находится в состоянии {1}",
                    //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), 
                    //    this._ThreadForInput.ThreadState.ToString());
                    //Trace.TraceError(msg);
                }
            }
            return;
        }
        #endregion

        #region Events
        public override event EventHandler MessageReceived;
        public override event EventHandlerErrorRecived ErrorReceived;
        public override event EventHandlerPortChangesStatus PortChangedStatus;
        #endregion

        #region ICanPort Members
        /// <summary>
        /// Наименование CAN-порта
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Наименование порта")]
        [Description("Наименование порта в формате CANx, где x - номер CAN-порта в системе 1...9")]
        [DefaultValue("CAN1")]
        public override string PortName
        {
            get { return this._PortName; }
            set 
            {
                if ( Api.CheckPortName(value))
                { this._PortName = value; }
                else
                { 
                    throw new ArgumentException(
                        "Наименование CAN-порта должно быть в формате CANx, где x номер порта 1...9"); 
                }
            }
        }
        /// <summary>
        /// Скорость обмена в сети CAN
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Скорость обмена")]
        [Description("Скорость обмена данными на шине CAN")]
        public override BaudRate BitRate
        {
            get { return this._BitRate; }
            set { this._BitRate = value; }
        }
        /// <summary>
        /// Тип адаптера
        /// </summary>
        public override string HardwareType
        { 
            get 
            {
                F_CAN_CONTROLLER controller;
                this.GetControllerType(out controller);
                return "NIM-351";
                //return String.Format("NIM-351; {0}", 
                //Enum.GetName(typeof(F_CAN_CONTROLLER), controller)); 
            } 
        }
        /// <summary>
        /// Реализация ICanPort
        /// </summary>
        public override string Manufacturer
        { get { return "Fastwel"; } }
        /// <summary>
        /// Версия аппаратного обеспечения
        /// </summary>
        public override Version HardwareVersion
        { get { return new Version(0, 0); } }
        /// <summary>
        /// Версия ПО
        /// </summary>
        public override Version SoftwareVersion
        { get { return new Version(2, 0); } }
        /// <summary>
        /// Возвращает состояние драйвера порта
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Состояние")]
        [DisplayName("Состояние порта")]
        [Description("Количество принятых сообщений во воходном буфере порта")]
        public override bool IsOpen
        {
            get
            {
                if (_DeviceHandle != null)
                {
                    if (_DeviceHandle.IsClosed == true)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    // Если дескриптор порта не определён, считаем что порт закрыт
                    return false;
                }
            }
        }
        /// <summary>
        /// Возвращает состояние порта
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Состояние")]
        [DisplayName("Статус CAN-порта")]
        [Description("Возвращает текущее состояние CAN-порта")]
        public override CanPortStatus PortStatus
        {
            get 
            {
                //F_CAN_STATE status = this.GetPortStatus();
                //return ConvertNim351.ConvertToCanPortStatus(status); 
                return ConvertNim351.ConvertToCanPortStatus(this._PortStatus);
            }
        }
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Режим работы")]
        [Description("Режим работы CAN-порта")]
        public override PortMode Mode
        {
            get
            { return this._OpMode; }
            set
            { this._OpMode = value; }
        }
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
        [DefaultValue("true")]
        public override Boolean ErrorFrameEnable
        {
            get { return this._ErrorFrameEnable; }
            set { this._ErrorFrameEnable = value; }
        }
        /// <summary>
        /// Тип кадров с которыми работает CAN адаптер.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Формат кадра")]
        [Description("Формат кадра сообщения по CAN-шине")]
        //[DefaultValue(typeof(FrameFormat), FrameFormat.StandardFrame.ToString())]
        public override FrameFormat FrameFormat
        {
            get
            { return this._FrameFormat; }
            set
            { this._FrameFormat = value; }
        }
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
            get { return this._InputBufferMessages.Count; }
        }
        /// <summary>
        /// Открывает драйвер порта
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public override void Open()
        {
            String msg;
            F_CAN_RESULT result;
            F_CAN_SETTINGS settings;

            settings = new F_CAN_SETTINGS();
            settings.acceptance_code = 0;
            settings.acceptance_mask = 0;
            settings.baud_rate = ConvertNim351.ConvertToF_CAN_BAUDRATE(this.BitRate);
            //settings.error_mask = CAN_OPMODE_ERRFRAME.CAN_ERR_CRTL | CAN_OPMODE_ERRFRAME.CAN_ERR_BUSOFF ;
            settings.error_mask = 0xFFFF;

            settings.opmode = (UInt16)Api.OpModeBuilder(this.Mode, this.FrameFormat, this.ErrorFrameEnable);

            // Инициализация библиотеки
            result = Api.fw_can_init();
            if (!Api.f_can_success(result))
            {
                msg = String.Format("Ошибка при иницилизации fwcan.dll, Error: {0}", result);
                throw new InvalidOperationException(msg);
            }

            // Открываем адаптер CAN и получаем дескриптор порта
            result = Api.fw_can_open(Api.GetPortNumber(this.PortName), out this._DeviceHandle);
            if (!Api.f_can_success(result))
            {
                msg = String.Format("Ошибка при открытии CAN адаптера, Error: {0}", result);
                throw new InvalidOperationException(msg);
            }

            // Конфигурируем контроллер
            result = Api.fw_can_set_controller_config(_DeviceHandle, ref settings);
            if (!Api.f_can_success(result))
            {
                msg = String.Format("Ошибка при записи настроек в контроллер. Error: {0}", result);
                throw new InvalidOperationException(msg);
            }

            // Конфигурируем таймауты
            result = Api.fw_can_set_timeouts(_DeviceHandle, ref _Timeouts);
            if (!Api.f_can_success(result))
            {
                msg = String.Format("Ошибка при установке таймаутов. Error: {0}", result);
                throw new InvalidOperationException(msg);
            }

            // Обновляем и проверяем статус порта
            if (!this.UpdatePortStatus())
            {
                msg = String.Format("Ошибка. При открытии порта его статус остался неизменным: {0}", 
                    this.PortStatus);
                throw new Exception(msg);
            }

            return;
        }
        /// <summary>
        /// Закрывает порт (освобождает драйвер порта)
        /// </summary>
        public override void Close()
        {
            String msg;
            F_CAN_RESULT result;
            F_CAN_STATE state;

            // Порядок закрытия порта:
            // 1. Проверяем был ли открыт порт. Если не был, то выходим
            // 2. Проверяем состояние адаптера. Если подключен к сети, оключает.
            // 3. Останавливаем поток на прослушивание сети.
            // 4. Закрывает (освобождаем) драйвер порта

            // 1. Проверяем состояние порта
            if (!this.IsOpen)
            {
                // Порт закрыт выходим
                return;
            }

            // 2. Проверяем статус CAN-адаптера, если адаптер запущен, то останавливаем его
            state = GetPortStatus();

            if (state != F_CAN_STATE.CAN_STATE_INIT)
            {
                // Останавливаем адаптер
                this.Stop();
            }

            // 3. Останавливаем поток на чтение CAN-адаптера
            //    Закрываем поток на чтение
            this.StopThreadForListen();

            // 4. Закрываем порт
            result = Api.fw_can_close(_DeviceHandle);

            if (!Api.f_can_success(result))
            {
                msg = String.Format("Не удалось закрыть CAN адаптера. Error: ", result);
                throw new InvalidOperationException(msg);
            }

            // Проверяем если дескриптор CAN адаптера не закрылся, 
            // принудительно освобождаем его.
            if (this._DeviceHandle.IsClosed == false)
            {
                this._DeviceHandle.Close();
            }
            this._DeviceHandle = null;

            // Обновляем и проверяем статус порта
            if (!this.UpdatePortStatus())
            {
                msg = String.Format("Ошибка. При открытии порта его статус остался неизменным: {0}",
                    this.PortStatus);
                throw new Exception(msg);
            }
            return;
        }
        /// <summary>
        /// Производит аппаратный сборос CAN-адаптера
        /// </summary>
        public override void Reset()
        {
            String message;
            F_CAN_RESULT result;

            if (this.IsOpen)
            {
                result = Api.fw_can_purge(_DeviceHandle, F_CAN_PURGE_MASK.CAN_PURGE_HWRESET | F_CAN_PURGE_MASK.CAN_PURGE_RXABORT
                    | F_CAN_PURGE_MASK.CAN_PURGE_RXCLEAR | F_CAN_PURGE_MASK.CAN_PURGE_TXABORT | F_CAN_PURGE_MASK.CAN_PURGE_TXCLEAR);

                if (!Api.f_can_success(result))
                {
                    message = String.Format("Не удалось выполнить сброс CAN адаптера. Error: ", result);
                    throw new Exception(message);
                }
            }

            // Обновляем и проверяем статус порта
            this.UpdatePortStatus();
            return;
        }
        /// <summary>
        /// Переводит адаптер в состояние Active, если порт окрыт. 
        /// Иначе действие игнорируется
        /// </summary>
        public override void Start()
        {
            F_CAN_RESULT result;
            String msg;

            if (this.IsOpen)
            {
                // Запускаем адаптер для работы (переводим в активное состояние)
                result = Api.fw_can_start(this._DeviceHandle);

                if (!Api.f_can_success(result))
                {
                    msg = String.Format("Ошибка при переводе CAN адаптера в активное состояние. Error: {0}", result);
                    throw new InvalidOperationException(msg);
                }

                // Создаём поток на прослушивание сети
                this.CreateAndStartThreadForListenPort();
            }

            // Обновляем и проверяем статус порта
            this.UpdatePortStatus();

            return;
        }
        /// <summary>
        /// Переводит адаптер в состояние CAN_STATE_INIT
        /// </summary>
        public override void Stop()
        {
            F_CAN_RESULT result;
            String msg;

            if (this.IsOpen)
            {
                // Останавливаем поток на прослушивание сети
                this.StopThreadForListen();

                // Переводим контроллер в состояние Init
                result = Api.fw_can_stop(this._DeviceHandle);

                if (!Api.f_can_success(result))
                {
                    msg = String.Format("Ошибка при переводе CAN адаптера в состояние Init. Error: {0}", result);
                    throw new InvalidOperationException(msg);
                }
            }

            // Обновляем и проверяем статус порта
            this.UpdatePortStatus();

            return;
        }
        /// <summary>
        /// Записывает сообщение в порт
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="frameType"></param>
        /// <param name="frameFormat"></param>
        /// <param name="data"></param>
        public override void WriteMessage(uint identifier, FrameType frameType, 
            FrameFormat frameFormat, byte[] data)
        {
            Frame frame = new Frame();
            frame.Identifier = identifier;
            frame.FrameType = frameType;
            frame.FrameFormat = FrameFormat;
            if (data != null)
            { frame.Data = data; }
            else
            { frame.Data = new byte[0]; }
            
            this.Write(frame);
            return;
        }        
        /// <summary>
        /// Записывает сообщение в порт
        /// </summary>
        /// <param name="message">Сообщение для оправки</param>
        public override void Write(Frame message)
        {
            String msg;
            F_CAN_RESULT result;
            F_CAN_TX buffer;
            F_CAN_STATE status;
            uint count = 0;

            // Если прот закрыт, отсылка сообщения не возможна
            if (!this.IsOpen)
            {
                msg = "Невозможно отправить сообщение, CAN-порт закрыт";
                throw new InvalidOperationException(msg);
            }

            status = this.GetPortStatus();
            if (this.PortStatus != CanPortStatus.IsActive)
            {
                msg = "Невозможно отправить сообщение, CAN-порт отключён от физической сети";
                throw new InvalidOperationException(msg);
            }

            // Разбираем сообщение и подготавливаем его для отправки
            buffer.msg = ConvertNim351.ConvertToF_CAN_MSG(message);

            result = Api.fw_can_send(this._DeviceHandle, ref buffer, 1, ref count);
            //result = Api.fw_can_post_message(this._DeviceHandle, ref buffer);

            if (!Api.f_can_success(result))
            {
                // При отправке сообщения возникла ошибка
                msg = String.Format("Не удалось отправить сообщение. Error: ", result);
                throw new InvalidOperationException(msg);
            }
            return;
        }

        public override bool Read(out Frame message)
        {
            lock (_SyncRoot)
            {
                if (this._InputBufferMessages.Count > 0)
                {
                    message = this._InputBufferMessages.Dequeue();
                    return true;
                }
                else
                {
                    message = new Frame();
                    return false;
                }
            }
        }

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

        //public event EventHandler MessageReceived;

        //public event EventHandlerErrorRecived ErrorReceived;

        //public event EventHandlerPortChangesStatus PortChangedStatus;

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            if (this.IsOpen)
            { this.Close(); }
            return;
        }

        #endregion

        #region ISerializable Members

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context)
        {
            // Инициализируем свойства порта
            info.AddValue("PortName", this.PortName);
            info.AddValue("BitRate", this._BitRate);
            info.AddValue("Mode", this._OpMode);
            info.AddValue("ErrorFrameEnable", this._ErrorFrameEnable);
            info.AddValue("FrameFormat", this._FrameFormat);
            info.AddValue("Timeouts", this._Timeouts);
            return;
        }

        #endregion
    }
}
