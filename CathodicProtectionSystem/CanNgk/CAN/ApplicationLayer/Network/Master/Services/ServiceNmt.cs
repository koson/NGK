using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Transactions;
using Common.Collections.ObjectModel;
using Common.Controlling;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// Класс реализует сетевую службу NMT протокола CAN НГК-ЭХЗ
    /// </summary>
    public sealed class ServiceNmt: Service
    {
        /// <summary>
        /// Коды команд для управления узлами сети
        /// </summary>
        public enum StatusCode: byte
        {
            Operational = 0x01,
            Preoperational = 0x80,
            Stop = 0x02,
            Initialisation = 0x81
        }

        #region Fields And Properties

        //private static Logger _Logger = LogManager.GetLogger("NmtLogger");

        protected override Logger Logger
        {
            get { return null; } //return _Logger;
        }

        public override ServiceType ServiceType
        {
            get { return ServiceType.Nmt; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию запрещён
        /// </summary>
        private ServiceNmt()
            : base(null)
        {
            throw new NotImplementedException(
                "Попытка вызвать запрещённый конструктор класса ServiceNmt");
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controller">Контроллер сети</param>
        public ServiceNmt(INetworkController controller)
            : base(controller)
        {
            //Инициализируем контексты устройств
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
        public override void HandleOutcomingMessages()
        {
            DeviceContext device;
            String msg;
            DateTime time;
            Frame request;
            Transaction transaction;

            // 1. Проверяем статус устройства: 
            //  1.1 Если устройство находится в состоянии Operational, ничего не делаем
            //  1.2 Если устройство находится в состоянии CommunicationError, ничего не делаем
            //  1.3 Если устройство находится в состоянии ConfigurationError, ничего не делаем
            //  1.4 Если устройства находится в состоянии Stopped, ничего не делаем
            //  1.5 Если устройство находится в состоянии Pre-operational, отправляем команду 
            //      перевода устройства в состояние Operational
            //     1.5.1 Проверяем 

            // Сервис не работает
            if (_Status != Status.Running)
            {
                return;
            }

            // Получаем устройство
            _Context.Next();
            device = _Context.CurrentDevice;

            // Проверяем статус
            if (device.Device.Status == DeviceStatus.Preoperational)
            {
                if (device.CurrentTransaction != null)
                {
                    switch (device.CurrentTransaction.Status)
                    {
                        case TransactionStatus.Running:
                            {
                                // Запрос был уже отправлен. Проверяем таймаут
                                time = device.CurrentTransaction.TimeOfStart;
                                time = time.AddSeconds(device.Device.PollingInterval);

                                if (time < DateTime.Now)
                                {
                                    msg = String.Format("Service NMT: Timeout - устройство" +
                                        "не перешло в режим Operational за заданное время");
                                    // Устройство не перешло из состояние Pre-Operational 
                                    // за заданное время. Устанавливаем статус неудачной попытки
                                    device.CurrentTransaction.Abort(null, msg);
                                }
                                break;
                            }
                        case TransactionStatus.NotInitialized:
                        case TransactionStatus.Completed:
                            {
                                // Устройство перешло в состояние Operational - транзакция выполнена
                                // Однако, по какой-то причине устройство перешло в состояние Pre-Operational
                                // Переводим устройство в состояние Operational
                                request = new Frame();
                                request.Identifier = 0;
                                request.FrameFormat = FrameFormat.StandardFrame;
                                request.FrameType = FrameType.DATAFRAME;
                                request.Data = new Byte[] { 
                                    (Byte)StatusCode.Operational, // Поле комадны
                                    device.Device.NodeId // Поле Node ID
                                    };
                                transaction = new Transaction();
                                transaction.Start(TransactionType.BroadcastMode, request);
                                lock (_SyncRoot)
                                {
                                    _NetworkController.SendMessageToCanPort(
                                        transaction.Request.Value);
                                }
                                break;
                            }
                        case TransactionStatus.Aborted:
                            {
                                // Проверяем количество неудачных попыток и если оно
                                // достигло предела не отсылаем устройству больше запросов
                                if (device.ErrorCount < TotalAttempts)
                                {
                                    // Повторяем запрос
                                    device.CurrentTransaction.Repeat();
                                    lock (_SyncRoot)
                                    {
                                        _NetworkController.SendMessageToCanPort(
                                            device.CurrentTransaction.Request.Value);
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                msg = String.Format(
                                    "Network {0}: ServiceNmt - Обнаружен статус запроса {1} не поддерживаемый " +
                                    "в данной версии ПО", _NetworkController.NetworkName,
                                    device.CurrentTransaction.Status);
                                throw new Exception(msg);
                            }
                    }
                }
                else
                {
                    // Отправляем команду для перевода устройства в состояние Operational
                    request = new Frame();
                    request.Identifier = 0;
                    request.FrameFormat = FrameFormat.StandardFrame;
                    request.FrameType = FrameType.DATAFRAME;
                    request.Data = new Byte[] { 
                        (Byte)StatusCode.Operational, // Поле комадны
                        device.Device.NodeId // Поле Node ID
                    };
                    device.CurrentTransaction = new Transaction();
                    device.CurrentTransaction.Start(TransactionType.UnicastMode, request);
                    lock (_SyncRoot)
                    {
                        _NetworkController.SendMessageToCanPort(
                            device.CurrentTransaction.Request.Value);
                    }
                }
            }
            else
            {
                // Oбратная связь возникает через сервис NodeGuard, 
                // который отслеживает состояние устройства. И только он может установить, что
                // устройство перешло режим Operational и устанавливает статус устройству.
                // По этому, проверяем состояние транзакции и если транзакция активна
                // и устройство изменило статус, то считаем что команда дошла до устройства
                // и завершаем транзакцию.
                if ((device.CurrentTransaction != null) && 
                    (device.CurrentTransaction.IsRunning))
                {
                    // Т.к сервис не получает ответа, останавливаем транзакцию
                    // при помощи пустого фрайма (псевноответа)
                    device.CurrentTransaction.Stop(new Frame());
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public override void HandleIncomingMessages(Frame[] messages)
        {
            // Данный сервис не обрабатывает входящие сообщения
            return;
        }
        
        #endregion
    }
}
