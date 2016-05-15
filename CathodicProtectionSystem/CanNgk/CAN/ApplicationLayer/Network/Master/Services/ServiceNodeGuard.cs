using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Transactions;
using Common.Controlling;
using Common.Collections.ObjectModel;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ServiceNodeGuard: Service
    {
        #region Helper
        /// <summary>
        /// Код состояния сетевого устройства в ответе на запрос
        /// (Для сервиса NodeGuard)
        /// </summary>
        public enum DeviceStatusCode : byte
        {
            /// <summary>
            /// Режим: Штатное функционирование
            /// </summary>
            OPERATIONAL = 0x05,
            /// <summary>
            /// Режим: Устройство остановлено
            /// </summary>
            STOPPED = 0x04,
            /// <summary>
            /// Режим: Pre-operational. (По идее, PDO не работает, SDO - работает)
            /// </summary>
            PREOPERATIONAL = 0x7F
        }
        /// <summary>
        /// Преобразует данные типа DeviceStatusCode в 
        /// данные типа DeviceStatus 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static DeviceStatus ToDeviceStatus(DeviceStatusCode code)
        {
            String msg;

            switch (code)
            {
                case DeviceStatusCode.OPERATIONAL:
                    { return DeviceStatus.Operational; }
                case DeviceStatusCode.PREOPERATIONAL:
                    { return DeviceStatus.Preoperational; }
                case DeviceStatusCode.STOPPED:
                    { return DeviceStatus.Stopped; }
                default:
                    {
                        msg = String.Format(
                            "Неудалось привести значение {0} к типу DeviceStatus", code);
                        throw new InvalidCastException();
                    }
            }
        }
        /// <summary>
        /// Структура для предствавления сообщения для Boot Up сервиса
        /// от удалённого устройства
        /// </summary>
        private struct IncomingMessageStuctureNodeGuard
        {
            #region Fields And Pproperties
            /// <summary>
            /// Длина данных в ответе
            /// </summary>
            internal int DL;
            internal Byte CobeId;
            internal Frame? Answer;
            internal Byte Code
            {
                get
                {
                    const Byte MASK_COBEID = 0x7F; 
                    //String msg;
                    // получаем текущее состояние устройства
                    Byte status = Answer.Value.Data[0];
                    // Ингорируем старший бит, который должен инверироваться 
                    // при каждом запросе к устройству
                    status &= MASK_COBEID;
                    return status;
                }
            }
            internal DeviceStatusCode StatusCode
            {
                get 
                {
                    String msg;
                    // получаем текущее состояние устройства
                    Byte status = Answer.Value.Data[0];
                    // Ингорируем старший бит, который должен инверироваться 
                    // при каждом запросе к устройству
                    status &= 0x7F;
                    // Проверяем код состояния устройства. Если он валиден, то true.
                    if (Enum.IsDefined(typeof(DeviceStatusCode), status))
                    {
                        return (DeviceStatusCode)status;
                    }
                    msg = String.Format(
                        "Невозможно преобразовать код {0} в DeviceStatusCode", status);
                    throw new InvalidCastException(msg);
                }
            }
            /// <summary>
            /// Возвращает true, если сообщение пердназначено
            /// для данного сервиса
            /// </summary>
            internal bool IsForService
            {
                get
                {
                    // Маска для отделения (Fct code: 4 старших бита) от адреса устройства (7 младших бит)
                    // переданных в CobId сообщения
                    const UInt16 MASK_FCT_CODE = 0x780; // 11110000000;
                    // Идентификатор CodId, по которому определяется что входящее сообщение
                    // предназначено для данного сервиса
                    // соответствует 11100000000 т.у. Fct code (1110) + CodId (7 Бит)
                    const UInt16 MASK = 0x700;
                    // Выделяет 7 бит содержащих CodeId из поля Id
                    
                    // Работаем только с данным типом сообщений другие игнорируем.
                    if ((Answer.Value.FrameType == FrameType.DATAFRAME) &&
                        (Answer.Value.Identifier & MASK_FCT_CODE) == MASK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            /// <summary>
            /// Возвращает true, если сообщение имеет неверный формат сообщения
            /// </summary>
            internal bool HasIncorrectStructure
            {
                get
                {
                    if (DL == 1)
                    {
                        // Всё верно, получаем текущее состояние устройства
                        Byte status = Answer.Value.Data[0];
                        // Ингорируем старший бит, который должен инверироваться при каждом запросе к устройству
                        status &= 0x7F;
                        // Проверяем код состояния устройства. Если он валиден, то true.
                        if (Enum.IsDefined(typeof(DeviceStatusCode), status))
                        {
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
            }
            #endregion

            #region Constructors
            #endregion

            #region Methods
            /// <summary>
            /// Разбирает ответное сообщение
            /// </summary>
            /// <param name="message">Ответное сообщение</param>
            /// <returns>Структура данных ответа</returns>
            internal static IncomingMessageStuctureNodeGuard Parse(Frame message)
            {
                const Byte MASK_COBEID = 0x7F; // Выделяет 7 бит содержащих CodeId из поля Id 
                IncomingMessageStuctureNodeGuard frame =
                    new IncomingMessageStuctureNodeGuard();
                frame.Answer = message;
                frame.CobeId = (Byte)(((Byte)message.Identifier) & MASK_COBEID);
                frame.DL = message.Data.Length;
                return frame;
            }
            #endregion
        }

        #endregion

        #region Fields And Properties

        //private static Logger _Logger = LogManager.GetLogger("NodeGuardLogger");

        protected override Logger Logger
        {
            //get { return _Logger; }
            get { return null; }
        }

        public override ServiceType ServiceType
        {
            get { return ServiceType.NodeGuard; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        private ServiceNodeGuard()
            : base(null)
        {
            throw new NotImplementedException(
                "Попытка вызвать запрещённый конструктор класса ServiceNodeGuard");
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controller">Контроллер сети</param>
        public ServiceNodeGuard(ICanNetworkController controller)
            : base(controller)
        {
            _Context = new Context(_NetworkController.Devices.ToArray());
            
            _NetworkController.Devices.CollectionWasChanged += 
                new EventHandler<KeyedCollectionWasChangedEventArgs<DeviceBase>>(
                EventHandlerDevicesCollectionWasChanged);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Обработчик изменения коллекции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EventHandlerDevicesCollectionWasChanged(
            object sender, KeyedCollectionWasChangedEventArgs<DeviceBase> e)
        {
            // В случае изменения коллекции переопределяем контекст
            lock (_SyncRoot)
            {
                _Context = new Context(_NetworkController.Devices.ToArray());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public override void HandleIncomingMessages(NGK.CAN.DataLinkLayer.Message.Frame[] messages)
        {
            String msg;
            IncomingMessageStuctureNodeGuard msghelper;
            DeviceBase device;

            if (Status != Status.Running)
            {
                return;
            }

            foreach (Frame message in messages)
            {
                msghelper = IncomingMessageStuctureNodeGuard.Parse(message);

                if (!msghelper.IsForService)
                {
                    continue;
                }

                if (msghelper.HasIncorrectStructure)
                {
                    if ((msghelper.DL == 1) && (msghelper.Code == 0))
                    {
                        // Код состояния с 0 используется протоколом Boot-Up
                        // поэтому данное собщенеие не считаем очибочным. Оно для
                        // сервиса Boot-Up
                    }
                    else
                    {
                        // Формат сообщения неверен.
                        msg = String.Format(
                            "Network {0}: Принято сообщение с неверным форматом данных {1}",
                            _NetworkController.NetworkName, message.ToString());
                        //_Logger.Error(msg);
                    }
                    continue;
                }

                if (!_NetworkController.Devices.Contains(msghelper.CobeId))
                {
                    // Устройство не найдено
                    msg = String.Format(
                        "Network {0}: Пришло сообщение от устройства с NodeId {1}, " +
                        "данное устройство не зарегистрировано в сети. Message - {2}",
                        this.NetworkController.NetworkName, msghelper.CobeId, message.ToString());
                    //Logger.Error(msg);
                    continue;
                }

                // Устройство найдено. 
                lock (_SyncRoot)
                {
                    // Завершаем транзакцию
                    Transaction trns = _Context.FindDevice(msghelper.CobeId).CurrentTransaction;

                    if (trns != null)
                    {
                        trns.Stop(message);
                        // Устанавливаем новый статус устройству
                        _NetworkController.Devices[msghelper.CobeId].Status =
                            ServiceNodeGuard.ToDeviceStatus(msghelper.StatusCode);
                    }
                    else
                    {
                        // Принят ответ в отсутствии запроса
                        throw new Exception();
                    }
                }
                // Пишем в журнал... Не реализовано
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void HandleOutcomingMessages()
        {
            DeviceContext device;
            DateTime time;
            Frame message;

            // Сервис не работает
            if (_Status != Status.Running)
            {
                return;
            }

            if (_Context.Count == 0)
            {
                return;
            }
            // Получаем следующее устройство в спике
            _Context.Next();
            
            device = _Context.CurrentDevice;
            
            //if (device.Device.Status == DeviceStatus.CommunicationError)
            //{
                // Если устройство не доступно, пропускаем его
                //return;
            //}

            // Проверяем статус запроса 
            if (device.CurrentTransaction == null)
            {
                device.CurrentTransaction = new Transaction();
            }

            switch (device.CurrentTransaction.Status)
            {
                case TransactionStatus.Running:
                    {
                        time = device.CurrentTransaction.TimeOfStart.AddSeconds(
                            device.Device.PollingInterval);
                        if (time < DateTime.Now)
                        {
                            // Устройство не ответилo за заданное время. 
                            // Устанавливаем статус неудачной попытки
                            device.CurrentTransaction.Abort(null, "Request timeout"); 
                        }
                        break;
                    }
                case TransactionStatus.Aborted:
                    {
                        // Проверяем количество попыток доступа к устройству.
                        // Если оно достигло предела. Устанавливаем статус ошибки
                        if (device.ErrorCount < TotalAttempts)
                        {
                            // Повторяем запрос
                            device.CurrentTransaction.Repeat();
                            _NetworkController.SendMessageToCanPort(
                                device.CurrentTransaction.Request.Value);
                        }
                        else
                        {
                            // Устанавливаем новый статус устройству
                            device.CurrentTransaction = null;
                            device.Device.Status = DeviceStatus.CommunicationError;
                        }
                        break;
                    }
                case TransactionStatus.NotInitialized:
                    {
                        message = new Frame();
                        message.Identifier =
                            System.Convert.ToUInt32(0x700 + device.Device.NodeId);
                        message.FrameFormat = FrameFormat.StandardFrame;
                        message.FrameType = FrameType.REMOTEFRAME;
                        message.Data = new Byte[0];

                        device.CurrentTransaction = new Transaction();
                        device.CurrentTransaction.Start(TransactionType.UnicastMode, message);
                        // Отправляем запрос
                        _NetworkController.SendMessageToCanPort(
                            device.CurrentTransaction.Request.Value);
                        break;
                    }
                case TransactionStatus.Completed:
                    {
                        // Получаем время последнего опроса устройства. 
                        // Прибавляем к нему время прериода опроса устройства
                        // и сравниваем с текущим временем. Если полученное время меньше текущего, 
                        // то выполняем запрос к устройству.
                        time = device.CurrentTransaction.TimeOfStart.AddSeconds(
                            device.Device.PollingInterval);
                        if (time < DateTime.Now)
                        {
                            message = new Frame();
                            message.Identifier =
                                System.Convert.ToUInt32(0x700 + device.Device.NodeId);
                            message.FrameFormat = FrameFormat.StandardFrame;
                            message.FrameType = FrameType.REMOTEFRAME;
                            message.Data = new Byte[0];

                            device.CurrentTransaction = new Transaction();
                            device.CurrentTransaction.Start(TransactionType.UnicastMode, message);
                            // Отправляем запрос
                            _NetworkController.SendMessageToCanPort(
                                device.CurrentTransaction.Request.Value);
                        }
                        break;
                    }
                default: { throw new Exception(); }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Start()
        {
            _Context = new Context(_NetworkController.Devices.ToArray());

            base.Start();
        }
        #endregion
    }
}
