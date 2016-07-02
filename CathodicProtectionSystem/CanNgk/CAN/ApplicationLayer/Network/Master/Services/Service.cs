using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using NGK.CAN.DataLinkLayer.Message;
using Common.Controlling;
using Infrastructure.LogManager;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// Базовый класс сетевого сервиса протокола CAN НГК-ЭХЗ. От него долны наследоваться
    /// все сетевые сервисы данного протокола
    /// </summary>
    public abstract class Service : IManageable
    {
        #region Fields And Properties
        /// <summary>
        /// Возвращает экземпляр логера
        /// </summary>
        protected ILogManager Logger
        {
            get 
            {
                return _LogEnabled ? NLogManager.Instance : null; 
            } 
        }
        private bool _LogEnabled = false;
        /// <summary>
        /// Разрешает/запрещает логирование 
        /// </summary>
        protected virtual bool LogEnabled
        {
            get { return _LogEnabled; }
            set { _LogEnabled = value; }
        }
        /// <summary>
        /// Возвращает наименование сервиса
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Система")]
        [DisplayName("Наименование сервиса")]
        [Description("Наименование сетевого сервиса CAN НГК-ЭХЗ")]
        public abstract ServiceType ServiceType
        {
            get;
        }
        /// <summary>
        /// Текущее состояние сервиса
        /// </summary>
        protected Status _Status;
        /// <summary>
        /// Возвращает статус сетевого сервиса
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Состояние сервиса")]
        [Description("Состояние сетевого сервиса CAN НГК-ЭХЗ")]
        public Status Status
        {
            get { return _Status; }
            set
            {
                String msg;

                switch (value)
                {
                    case Status.Stopped:
                        {
                            this.Stop();
                            break;
                        }
                    case Status.Running:
                        {
                            // Запускаем сервис
                            this.Start();
                            break;
                        }
                    case Status.Paused:
                        {
                            // Приостанавливаем работу сервиса
                            //this.Suspend();
                            //break;
                            msg = String.Format("Network {0}: Service {1}: Попытка установить свойству " +
                                "Status значение неподдерживаемое в данной версии ПО - {1}",
                                this._NetworkController.NetworkName, this.ServiceType.ToString(),
                                value.ToString());
                            throw new NotSupportedException(msg);
                        }
                    default:
                        {
                            msg = String.Format("Network {0}: Service {1}: Попытка установить свойству " +
                                "Status значение неподдерживаемое в данной версии ПО - {2}",
                                this._NetworkController.NetworkName, this.ServiceType.ToString(),
                                value.ToString());
                            throw new Exception(msg);
                        }
                }
            }
        }
        /// <summary>
        /// Ссылка на контроллер сервисов, которому принадлежит данный сервис
        /// </summary>
        protected ICanNetworkController _NetworkController;
        /// <summary>
        /// Устанавливается при дессериализации сервиса
        /// </summary>
        internal ICanNetworkController NetworkController
        {
            get { return _NetworkController; }
            set
            {
                if ((_NetworkController == null) ||
                    (_NetworkController.Equals(value)) ||
                    (value == null))
                {
                    _NetworkController = value;
                }
                else
                {
                    throw new ArgumentException(
                        "Попытка установить недопустимое значение",
                        "NetworkController");
                }
            }
        }
        /// <summary>
        /// Возваращает количесвто попыток доступа к удалённому устройству, прежде чем
        /// сервис переведёт устройство в состояние аварии.
        /// </summary>
        protected Int32 _TotalAttempts;
        /// <summary>
        /// Возваращает количесвто попыток доступа к удалённому устройству, прежде чем
        /// сервис переведёт устройство в состояние аварии.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Количество попыток доступа к устройству")]
        [Description("Количество неудачных попыток доступа к устройству, " +
            "после чего устройство переходит в состояние неисправности.")]
        public Int32 TotalAttempts
        {
            get { return this._TotalAttempts; }
            set
            {
                String msg;

                if (value < 1)
                {
                    msg = String.Format(
                        "Network {0}: Service {1}: Попытка установить свойству недопустимое значение меньше 1",
                        this._NetworkController.NetworkName, this.ServiceType.ToString());
                    throw new ArgumentOutOfRangeException("TotalAttempts", msg);
                }
                else
                {
                    this._TotalAttempts = value;
                }
            }
        }
        /// <summary>
        /// Контекст сервиса
        /// </summary>
        protected Context _Context;
        /// <summary>
        /// Оъект для сихронизации доступа к ресурсам.
        /// </summary>
        protected static Object _SyncRoot = new Object();

        #endregion

        #region Constructors
        /// <summary>
        /// Констуктор по умолчанию запрещён
        /// </summary>
        private Service()
        {
            throw new NotImplementedException(
                "Попытка вызова запрещённого конструктора по умлочанию NetworkService()");
        }
        /// <summary>
        /// Констуктор базового класса
        /// </summary>
        /// <param name="controller">Контроллер сети</param>
        protected Service(ICanNetworkController controller)
        {
            String msg;

            if (controller == null)
            {
                msg = String.Format("Попытка создать сетевой сервис {0}, " +
                    "для несуществующего контроллера сети", ServiceType);
                throw new NullReferenceException(msg);
            }

            _TotalAttempts = 1;
            _Status = Status.Stopped;
            _NetworkController = controller;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Принять сообщения от контроллера сети
        /// </summary>
        /// <param name="messages"></param>
        public virtual void HandleIncomingMessages(Frame[] messages)
        {
            return;
        }
        /// <summary>
        /// Метод вызывается контроллером сети и выполняет 
        /// сетевую транзакцию (запрос к устройству)
        /// </summary>
        public virtual void HandleOutcomingMessages()
        {
            return;
        }
        /// <summary>
        /// Стартует работу сетевого сервиса
        /// </summary>
        public virtual void Start()
        {
            String msg;

            if (this._NetworkController != null)
            {
                if (this._NetworkController.Status == Status.Running)
                {
                    switch (this.Status)
                    {
                        case Status.Running:
                            {
                                // Ничего не делаем сервис уже запущен
                                break;
                            }
                        case Status.Stopped:
                            {
                                // Запускаем сервис
                                lock (_SyncRoot)
                                {
                                    this._Status = Status.Running;
                                }

                                // Генерируем событие
                                this.OnStatusChanged();

                                if (Logger != null)
                                {
                                    msg = String.Format("Network {0}: Sevrice {1}: Сервис был запущен",
                                        this._NetworkController.NetworkName, this.ServiceType);
                                    Logger.Info(msg);
                                }

                                break;
                            }
                        case Status.Paused:
                            {
                                msg = String.Format(
                                    "{0}: Network {1}: Service {2}: Start() обнаружил состояние севриса неподдерживаемое в данной версии ПО - {3}",
                                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")), this._NetworkController.NetworkName,
                                    this.ServiceType.ToString(), this.Status.ToString());
                                throw new NotImplementedException(msg);
                            }
                        default:
                            {
                                msg = String.Format(
                                    "{0}: Network {1}: Service {2}: Start() обнаружил состояние севриса неподдерживаемое в данной версии ПО - {3}",
                                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")), this._NetworkController.NetworkName,
                                    this.ServiceType.ToString(), this.Status.ToString());
                                throw new NotImplementedException(msg);
                            }
                    }
                }
                else
                {
                    msg = String.Format(
                        "{0}: Network {1}: {2}.Start(): Невозмножно выполнить запуск сервиса. Контроллер сети не запущен",
                        DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)),
                        this._NetworkController.NetworkName, this.ServiceType.ToString());
                    throw new InvalidOperationException(msg);
                }
            }
            else
            {
                msg = String.Format(
                    "{0}: Network null: {1}.Start(): Невозможно выполнить запуск сервиса. Отсутствует (null) контроллер сети",
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), this.ServiceType.ToString());
                throw new InvalidOperationException(msg);
            }
            return;
        }
        /// <summary>
        /// Останавливает работу сервиса
        /// </summary>
        public virtual void Stop()
        {
            String msg;

            if (_Status != Status.Stopped)
            {
                _Status = Status.Stopped;
                // Генерируем событие
                this.OnStatusChanged();

                msg = String.Format("Network {0}: Service {1}: Сервис был остановлен",
                    this._NetworkController.NetworkName, this.ServiceType);
                //Logger.Info(msg);
                return;
            }
            
            return;
        }
        /// <summary>
        /// Приостанавливает работу сервиса
        /// </summary>
        public virtual void Suspend()
        {
            string msg;
            msg = String.Format(
                "{0}: Network {1}: Service {2}: Метод севриса неподдерживается в данной версии ПО",
                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")), 
                _NetworkController.NetworkName, ServiceType);
            throw new NotImplementedException(msg);
        }
        /// <summary>
        /// Освобождает ресурсы используемые объектом
        /// </summary>
        public virtual void Dispose()
        {
            Stop();
            return;
        }
        /// <summary>
        /// Метод генерирует событие изменения состояния сетевого сервиса
        /// </summary>
        protected void OnStatusChanged()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = this.ServiceChangedStatus;

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

            //String traceMessage = String.Format(
            //    "Сеть {0}: Сервис {1}: Принял новое состояние {2}",
            //    this._NetworkController.NetworkName,
            //    this.ServiceName.ToString(), this._Status.ToString());
            return;
        }

        #endregion

        #region Events
        /// <summary>
        /// Событие происходит при изменении состояния сетевого сервиса
        /// </summary>
        public event EventHandler ServiceChangedStatus;
        /// <summary>
        /// 
        /// </summary>
        event EventHandler IManageable.StatusWasChanged
        {
            add
            {
                lock (_SyncRoot)
                {
                    this.ServiceChangedStatus += value;
                }
            }
            remove
            {
                lock (_SyncRoot)
                {
                    this.ServiceChangedStatus -= value;
                }
            }
        }

        #endregion
    }
}
