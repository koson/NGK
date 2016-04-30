using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NLog;
using NGK.CAN.ApplicationLayer.Transactions;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataTypes.DateTimeConvertor;
using Common.Controlling;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ServicePdoReceive: Service
    {
        #region Fields And Properties

        protected override Logger Logger
        {
            get { return null; }
        }

        public override ServiceType ServiceType
        {
            get
            {
                return ServiceType.PdoReceive;
            }
        }
        /// <summary>
        /// Период синхронизации времени в удалённых устройствах сети, сек
        /// </summary>
        private Int32 _SynchronizationPeriod;
        /// <summary>
        /// Период времени через который происходит синхронизация времени в сети, сек
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Период синхранизации времени устройств, сек")]
        [Description("Период времени через который происходит синхронизация " +
            "времени в удалённых устройствах сети ")]
        public Int32 Interval
        {
            get
            {
                return _SynchronizationPeriod;
            }
            set
            {
                if (base._Status == Status.Stopped)
                {
                    if (value > 0)
                    {
                        this._SynchronizationPeriod = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Interval", String.Format(
                            "Попытка установить недопустимое значение периода синхронизации времени. " +
                            "Знаение {0}, ожидается строго больше нуля",
                            value.ToString()));
                    }
                }
            }
        }
        /// <summary>
        /// Время и дата последней синхронизации времени в сети
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Настройки")]
        [DisplayName("Дата и время синхронизации времени")]
        [Description("Дата и время последней синхронизации времени в сети")]
        public DateTime LastTimeSynchronisation
        {
            get
            {
                return _LastTransaction == null ? new DateTime() : 
                    _LastTransaction.TimeOfEnd;
            }
        }
        /// <summary>
        /// Хранит последений выполненный сервисом запрос. Это необходимо для 
        /// отслеживания выполнения запроса контроллером сети 
        /// </summary>
        private Transaction _LastTransaction;

        #endregion
        
        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        private ServicePdoReceive()
            : base(null)
        {
            throw new NotImplementedException(
                "Попытка вызвать запрещённый конструктор класса ServicePdoReceive");
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controller">Контроллер сети</param>
        /// <param name="periodOfTimeSync">
        /// Период синхронизации времени в удалённых устройствах сети, сек
        /// </param>
        public ServicePdoReceive(ICanNetworkController controller, 
            Int32 periodOfTimeSync)
            : base(controller)
        {
            if (periodOfTimeSync > 0)
            {
                _SynchronizationPeriod = periodOfTimeSync;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Interval", String.Format(
                    "Попытка установить недопустимое значение периода синхронизации времени. " +
                    "Знаение {0}, ожидается строго больше нуля",
                    periodOfTimeSync.ToString()));
            }
        }
        /// <summary>
        /// Конструктор для сериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //public ServicePdoReceive(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //    _SynchronizationPeriod = info.GetInt32("Interval");
        //    _LastTimeSynchronisation = DateTime.Now;
        //}
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        public override void HandleOutcomingMessages()
        {
            //String msg;
            Frame message;

            if (Status != Status.Running)
            {
                return;
            }

            // 1. Один раз в период опроса (равен периoду) опрашиваем состояние устройства. 
            // Получаем время последнего опроса устройства. Прибавляем к нему время прериода синхронизации времени
            // и сравниваем с текущим временем. Если полученное время меньше текущего, то выполняем синхронизацию.

            if (_LastTransaction != null)
            {
                TimeSpan interval = DateTime.Now - _LastTransaction.TimeOfEnd;

                if (interval.Seconds < Interval)
                {
                    return;
                }
            }
            
            _LastTransaction = new Transaction();
            
            // Формируем и записываем запрос в выходной буфер
            message = new Frame();
            message.Identifier = 0x200;
            message.FrameFormat = FrameFormat.StandardFrame;
            message.FrameType = FrameType.DATAFRAME;
            message.Data = new Byte[4];
            // Получаем текущее время в формате UNIX
            UInt32 unixTime = Unix.ToUnixTime(DateTime.Now);
            unchecked
            {
                message.Data[0] = (Byte)unixTime; // Младший байт
                message.Data[1] = (Byte)(unixTime >> 8);
                message.Data[2] = (Byte)(unixTime >> 16);
                message.Data[3] = (Byte)(unixTime >> 24); // Старший байт
            }
            
            _LastTransaction.Start(TransactionType.BroadcastMode, message);
            _NetworkController.SendMessageToCanPort(_LastTransaction.Request.Value);
            _LastTransaction.Stop(null);

            //Debug.WriteLine(String.Format("Время последней синхронизации времени: {0}",
            //    now.ToString(new System.Globalization.CultureInfo("ru-Ru", false))));
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
        //    info.AddValue("Interval", this._PeriodOfTimeSynchronization);
        //    base.GetObjectData(info, context);
        //    return;
        //}

        #endregion
    }
}
