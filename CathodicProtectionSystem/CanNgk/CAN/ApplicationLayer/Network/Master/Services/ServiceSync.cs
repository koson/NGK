using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.ComponentModel;
using NLog;
using NGK.CAN.ApplicationLayer.Transactions;
using NGK.CAN.DataLinkLayer.Message;
using Common.Controlling;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// Сетевой сервис Sync 
    /// </summary>
    public sealed class ServiceSync: Service
    {
        #region Fields And Properties

        //private static Logger _Logger = LogManager.GetLogger("SyncLogger");

        protected override Logger Logger
        {
            //get { return _Logger; }
            get { return null; }
        }

        public override ServiceType ServiceType
        {
            get
            {
                return ServiceType.Sync;
            }
        }

        private Timer _TimerSyncMessage;
        /// <summary>
        /// Интервал времени между генерацией команды SYNC, мсек
        /// </summary>
        /// <remarks>
        /// Служит для инициализации поля Interval таймера System.Timers.Timer.
        /// MSDN: Время между событиями Elapsed в миллисекундах. 
        /// Значение должно быть больше нуля и меньше или равно Int32.MaxValue. 
        /// Значение по умолчанию — 100 миллисекунд.
        /// Фаронов: int.MaxValue = 2147483647. Максимальное период в миллисекундах
        /// => 214783.367 сек. 
        /// => 214783.367 / 60 = 3579.72745 часов
        /// => 3572.72745 / 24 = 149,155 cуток
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Период сигнала SYNC, мсек")]
        [Description("Период генерации сообщения SYNC в сеть")]
        [DefaultValue(PERIOD_SYNC)]
        public Double PeriodSync
        {
            get
            {
                return this._TimerSyncMessage.Interval;
            }
            set
            {
                if (_Status == Status.Stopped)
                {
                    this._TimerSyncMessage.Interval = value;
                }
            }
        }
        /// <summary>
        /// Период сообщения SYNC по умолчанию
        /// </summary>
        private const Double PERIOD_SYNC = 1000;

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        private ServiceSync()
            : base(null)
        {
            throw new NotImplementedException("Попытка вызвать запрещённый конструктор класса ServiceSync");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller">Контроллер сети</param>
        public ServiceSync(INetworkController controller)
            : base(controller)
        {
            this._TimerSyncMessage = new Timer(PERIOD_SYNC);
            this._TimerSyncMessage.AutoReset = true;
            this._TimerSyncMessage.Elapsed += 
                new ElapsedEventHandler(EventHandler_TimerSyncMessage_Elapsed);

        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controller">Контроллер сети</param>
        /// <param name="periodSync">Период следования запроса Sync, мсек</param>
        public ServiceSync(INetworkController controller, Double periodSync)
            : base(controller)
        {
            this._TimerSyncMessage = new Timer(periodSync);
            this._TimerSyncMessage.AutoReset = true;
            this._TimerSyncMessage.Elapsed += 
                new ElapsedEventHandler(EventHandler_TimerSyncMessage_Elapsed);
        }
        /// <summary>
        /// Конструктор для сериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //public ServiceSync(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //    this._TimerSyncMessage = new Timer(info.GetDouble("PeriodSync"));
        //    this._TimerSyncMessage.AutoReset = true;
        //    this._TimerSyncMessage.Elapsed += 
        //        new ElapsedEventHandler(EventHandler_TimerSyncMessage_Elapsed);
        //}

        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        public override void HandleOutcomingMessages()
        {
            // TODO: Отказаться от таймера 
            // Исходящие сообщения формируются таймером
            //TimeSpan result = DateTime.Now - DateTime.Now;
      
            return;
        }
        public override void HandleIncomingMessages(Frame[] messages)
        {
            //Данный сервис не имеет входящих сообщений
            return;
        }
        /// <summary>
        /// Обработчик события срабатываения таймера.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TimerSyncMessage_Elapsed(object sender,
            ElapsedEventArgs e)
        {
            //String msg;
            Frame message;
            
            // Формируем сообщение SYNC и посылаем его в сеть
            message = new Frame();
            message.Identifier = 0x80;
            message.FrameFormat = FrameFormat.StandardFrame;
            message.FrameType = FrameType.DATAFRAME;
            message.Data = new Byte[0];

            lock (_SyncRoot)
            {
                _NetworkController.SendMessageToCanPort(message);
            }

            // Записываем в лог...
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Start()
        {
            base.Start();
            _TimerSyncMessage.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Stop()
        {
            _TimerSyncMessage.Stop();
            base.Stop();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            _TimerSyncMessage.Stop();
            _TimerSyncMessage.Dispose();
            base.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        //public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info,
        //    System.Runtime.Serialization.StreamingContext context)
        //{
        //    info.AddValue("PeriodSync", this._TimerSyncMessage.Interval);
        //    base.GetObjectData(info, context);
        //    return;
        //}
        #endregion
    }
}
