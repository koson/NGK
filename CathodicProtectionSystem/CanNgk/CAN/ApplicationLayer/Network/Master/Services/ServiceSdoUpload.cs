using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Transactions;
using NGK.CAN.ApplicationLayer.Network.Master.Services;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using NGK.CAN.DataTypes;
using Common.Controlling;
using Common.Collections.ObjectModel;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// Сетевой сервис SDO
    /// </summary>
    public sealed class ServiceSdoUpload: Service
    {
        #region Helper
        /// <summary>
        /// Индексы 8 байт данных в ответном сообщениии
        /// </summary>
        internal struct ANSWER_STUCTURE
        {
            /// <summary>
            /// Длина данных в ответе
            /// </summary>
            internal const Int32 DL = 0;
            /// <summary>
            /// Младший байт индекса
            /// </summary>
            internal const Int32 INDEXLOW = 1;
            /// <summary>
            /// Старший байт индекса
            /// </summary>
            internal const Int32 INDEXHIGH = 2;
            /// <summary>
            /// Субиндекс
            /// </summary>
            internal const Int32 SUBINDEX = 3;
            /// <summary>
            /// Младший байт данных
            /// </summary>
            internal const Int32 D0 = 4;
            /// <summary>
            /// 
            /// </summary>
            internal const Int32 D1 = 5;
            /// <summary>
            /// 
            /// </summary>
            internal const Int32 D2 = 6;
            /// <summary>
            /// Старший байт данных
            /// </summary>
            internal const Int32 D3 = 7;
        }
        // Константа для определения длины данных в байтах.
        // Передаётся в ответе на запрос в 0-вом байте данных сообщения
        private enum DataLenght: byte
        {
            NotDefined = 0,
            OneByte = 0x4F,
            TwoBytes = 0x4B,
            ThreeBytes = 0x47,
            FourBytes = 0x43,
            Ecxeption = 0x80 // Возвращено исключение
        }
        /// <summary>
        /// Структура для предствавления содержимого ответа на запрос по SDO
        /// </summary>
        private struct IncomingMessageStuctureSdo
        {
            #region Fields And Pproperties
            /// <summary>
            /// Длина данных в ответе
            /// </summary>
            internal DataLenght DL; 
            /// <summary>
            /// Индекс объекта
            /// </summary>
            internal UInt16 Index;
            /// <summary>
            /// Подиндекс объекта
            /// </summary>
            internal Byte SubIndex;
            /// <summary>
            /// Неформатированное значение числа (знаковое/безнаковое, целое/вещественное)
            /// 4 байта или в случае исключения - код исключения 
            /// </summary>
            internal UInt32 Value;
            /// <summary>
            /// Ответное сообщение содержит исключение (устройство вернуло ошибку)
            /// </summary>
            internal Boolean HasExсeption
            {
                get 
                {
                    return (DL == DataLenght.Ecxeption) ? true : false; 
                }
            }
            private bool _HasIncorrectStructure;
            /// <summary>
            /// Возвращает true, если сообщение имеет неверный формат сообщения
            /// </summary>
            internal bool HasIncorrectStructure
            {
                get
                {
                    return _HasIncorrectStructure;
                }
            }
            /// <summary>
            /// Возвращает код исключения (если исключение содержиться в ответе)
            /// </summary>
            internal UInt32? ExceptionCode
            {
                get { return HasExсeption ? (UInt32?)Value : null; }
            }
            internal Byte CobeId;
            internal Frame? Answer;
            /// <summary>
            /// Возвращает true если сообщение пердназначено
            /// для данного сервиса
            /// </summary>
            internal bool IsForService
            {
                get
                {
                    const UInt16 MASK = 0x580; // соответствует 10110000000 т.у. Fct code (1011) + CodId (7 Бит)
                    const UInt16 MASK_FCT_CODE = 0x780; // 11110000000;

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

            #endregion

            #region Constructors
            #endregion

            #region Methods
            /// <summary>
            /// Разбирает ответное сообщение
            /// </summary>
            /// <param name="message">Ответное сообщение</param>
            /// <returns>Структура данных ответа</returns>
            internal static IncomingMessageStuctureSdo Parse(Frame message)
            {
                const Byte MASK_COBEID = 0x7F; // Выделяет 7 бит содержащих CodeId из поля Id 

                IncomingMessageStuctureSdo frame = new IncomingMessageStuctureSdo();
                frame.Answer = message;
                frame.CobeId = (Byte)(((Byte)message.Identifier) & MASK_COBEID);
                // Длина данных ответа в SDO всегда 8
                if (message.Data.Length == 8)
                {
                    frame.DL = (DataLenght)message.Data[ANSWER_STUCTURE.DL];
                    switch (frame.DL)
                    {
                        case DataLenght.OneByte:
                            {
                                frame.Value = message.Data[ANSWER_STUCTURE.D0];
                                break;
                            }
                        case DataLenght.TwoBytes:
                            {
                                frame.Value = System.Convert.ToUInt32(
                                    (message.Data[ANSWER_STUCTURE.D0] |
                                    ((UInt32)message.Data[ANSWER_STUCTURE.D1] << 8)));
                                break;
                            }
                        case DataLenght.ThreeBytes:
                            {
                                frame.Value = System.Convert.ToUInt32(
                                    (message.Data[ANSWER_STUCTURE.D0] |
                                    ((UInt32)message.Data[ANSWER_STUCTURE.D1] << 8) |
                                    ((UInt32)message.Data[ANSWER_STUCTURE.D2] << 16)));
                                break;
                            }
                        case DataLenght.FourBytes:
                        case DataLenght.Ecxeption:
                            {
                                frame.Value = System.Convert.ToUInt32(
                                    (message.Data[ANSWER_STUCTURE.D0] |
                                    ((UInt32)message.Data[ANSWER_STUCTURE.D1] << 8) |
                                    ((UInt32)message.Data[ANSWER_STUCTURE.D2] << 16) |
                                    ((UInt32)message.Data[ANSWER_STUCTURE.D3] << 32)));
                                break;
                            }
                        case DataLenght.NotDefined:
                        default:
                            {
                                frame._HasIncorrectStructure = true;
                                frame.Value = 0; break;
                            }
                    }
                }
                else
                {
                    frame._HasIncorrectStructure = true;
                    frame.Value = 0;
                }

                if (!frame._HasIncorrectStructure)
                {
                    frame.Index = message.Data[ANSWER_STUCTURE.INDEXLOW];
                    frame.Index = System.Convert.ToUInt16((frame.Index | (message.Data[ANSWER_STUCTURE.INDEXHIGH] << 8)));
                    frame.SubIndex = message.Data[ANSWER_STUCTURE.SUBINDEX];
                }
                else
                {
                    frame.Index = 0;
                    frame.SubIndex = 0;
                }
                return frame;
            }
            #endregion
        }
        #endregion

        #region Fields And Properties

        protected override NLog.Logger Logger
        {
            get 
            {
                return null;
                //throw new Exception("The method or operation is not implemented."); 
            }
        }

        public override ServiceType  ServiceType
        {
            get { return ServiceType.SdoUpload; }
        }
        
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        private ServiceSdoUpload(): base(null)
        {
            throw new NotImplementedException(
                "Попытка вызвать запрещённый конструктор класса ServiceSdo");            
        }
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="controller"></param>
        public ServiceSdoUpload(INetworkController controller)
            : base(controller)
        {            
            //Инициализируем контексты устройств
            _Context = new Context(_NetworkController.Devices.ToArray());
            _NetworkController.Devices.CollectionWasChanged +=
                new EventHandler<KeyedCollectionWasChangedEventArgs<Device>>(
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
            object sender, KeyedCollectionWasChangedEventArgs<Device> e)
        {
            // В случае изменения коллекции переопределяем контекст
            lock (_SyncRoot)
            {
                _Context = new Context(_NetworkController.Devices.ToArray());
            }
        }
        /// <summary>
        /// Возвращает индекс объекта в запросе
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private UInt16 GetIndex(Frame request)
        {
            UInt16 index = 0;
            
            unchecked
            {
                index = Convert.ToUInt16(request.Data[ANSWER_STUCTURE.INDEXLOW] |
                    (request.Data[ANSWER_STUCTURE.INDEXHIGH] << 8));
            }
            return index; 
        }
        /// <summary>
        /// Обработать принятые сообщения
        /// </summary>
        /// <param name="messages">Входящие сообщения из сети</param>
        public override void HandleIncomingMessages(Frame[] messages)
        {            
            String msg;
            IncomingMessageStuctureSdo msghelper;
            DeviceContext deviceContex;
            DataObject obj;
            // Индексы объектов которые не обрабатываем!!!
            List<UInt16> notHandledIndexes = new List<ushort>();
            notHandledIndexes.AddRange(new UInt16[] { 0x2001, 0x2002 });

            if (Status != Status.Running)
            {
                return;
            }

            foreach(Frame message in messages)
            {            
                // Сообщение для этого сервиса. Получаем структуру сообщения
                msghelper = IncomingMessageStuctureSdo.Parse(message);

                if (!msghelper.IsForService)
                {
                    // Сообещение не для этого сервиса
                    continue;
                }
            
                // Ищем устройство с данным CobId. Если устройство найдено,
                // то устанавливаем флаг
                deviceContex = _Context.FindDevice(msghelper.CobeId);
            
                if (deviceContex == null)
                { 
                    // Устройство не найдено.
                    msg = String.Format(
                        "Network {0}: SDO Service: Получено сообщение от незарегистрированного устройства. " +
                        "Message - {1}", _NetworkController.Description, message.ToString());
                    //Trace.TraceError(msg);
                    continue;                
                }

                // Устройство найдено. Проверяем его текущий запрос, т.е index
                if ((deviceContex.CurrentTransaction != null) && 
                    (deviceContex.CurrentTransaction.Status == TransactionStatus.Running))
                {
                    if (msghelper.HasExсeption)
                    {
                        if (GetIndex(deviceContex.CurrentTransaction.Request.Value) == msghelper.Index)
                        {
                            msg = String.Format(
                                "Network {0}: SDO Service: Устройсво вернуло исключение {1}",
                                _NetworkController.NetworkId, msghelper.ExceptionCode);
                            deviceContex.CurrentTransaction.Abort(msghelper.Answer, msg);
                        }
                        else
                        {
                            msg = String.Format("Network {0}: SDO Service: " +
                                "Индекс в ответе не соответствует запрошенному. Запрос - {1}, Ответ - {2}",
                                NetworkController.NetworkId,deviceContex.CurrentTransaction.Request.Value.ToString(),
                                message.ToString());
                        }
                        continue;
                    }

                    // Проверяем индекс запроса и ответа
                    if (GetIndex(deviceContex.CurrentTransaction.Request.Value) != msghelper.Index)
                    {
                        // Прищёл ответ на несуществующий запрос
                        msg = String.Format("Network {0}: SDO Service: " +
                            "Индекс в ответе не соответствует запрошенному. Запрос - {1}, Ответ - {2}",
                            NetworkController.NetworkId, deviceContex.CurrentTransaction.Request.Value.ToString(),
                            message.ToString());
                        //Trace.TraceError(msg);
                        continue;
                    }
                    else
                    {                    
                        // Устанавливаем ответ на запрос
                        //deviceContex.CurrentTransaction.Answer = frmSdo.Answer;
                        // Проверяем контекст
                        if (deviceContex.CurrentObject.Index != msghelper.Index)
                        {
                            msg = String.Format(
                                "Network {0}: SDO Service: Индекс в ответе соответствует запрошенному {1}, " +
                                "но не соотвествует индексу в контексте {2}", _NetworkController.NetworkId,
                                msghelper.Index, deviceContex.CurrentObject);
                            deviceContex.CurrentTransaction.Abort(msghelper.Answer, msg);
                            throw new Exception(msg);
                        }
                    }

                    // Всё нормально, subindex-ы не проверяем они всегда равны 0;
                    
                    // Исключений нет, устанавливаем новое полученное значение для объекта 
                    // с указанным индексом 
                    // Получаем тип данных соответствующего идекса для соотвествующего профиля устройства
                   ObjectInfo objInfo;

                   if (!deviceContex.Device.Profile.ObjectInfoList.Contains(msghelper.Index))
                   {
                       msg = String.Format("Network {0}: SDO Service: " +
                           "Ненайдено описание объекта c индексом {1} в профиле устройства {2}",
                           _NetworkController.Description, msghelper.Index, deviceContex.Device.Profile.DeviceType);
                       deviceContex.CurrentTransaction.Abort(msghelper.Answer, msg);
                       throw new InvalidOperationException(msg);
                   }
                   else
                   {
                       objInfo = deviceContex.Device.Profile.ObjectInfoList[msghelper.Index];
                   }
                    
                    if (!deviceContex.Device.ObjectDictionary.Contains(msghelper.Index))
                    {
                        msg = String.Format("Network {0}: SDO Service: " +
                            "Ненайден объект c индексом {1} в словаре устройства {2}",
                           _NetworkController.Description, msghelper.Index, deviceContex.Device.NodeId);
                       deviceContex.CurrentTransaction.Abort(msghelper.Answer, msg);
                       throw new InvalidOperationException(msg);
                    }
                    else
                    {
                        obj = deviceContex.Device.ObjectDictionary[msghelper.Index];
                    }
                    // Проверяем модификатор доступа к значению. Если "только для чтения"
                    // и тип: System или Configuration, то параметр конфигурационный. 
                    // В этом случае сравниваем прочитанное значение со значением объета словаря. 
                    // Если они не совпадают, устанавливаем статус "Ошибка конфигурации"
                    // Если тип Measured, то читаем и устанавливаем данное значение из устройства
                    if ((objInfo.ReadOnly) && (objInfo.Category == Category.Configuration ||
                        objInfo.Category == Category.System))
                    {
                        if (notHandledIndexes.Contains(obj.Index))
                        {
                            // Обновляем время последнего прочтения
                            obj.Value = obj.Value;
                            // Закрываем транзакцию
                            deviceContex.CurrentTransaction.Stop(msghelper.Answer);
                            continue;
                        }

                        if (msghelper.Value != obj.Value)
                        {
                            // Ошбика конфигурации
                            msg = String.Format(
                                "Ошбика конфигурации устройства: Устройство NodeId={0}, Индекс объекта={1}. " +
                                "Значение в словаре={2}, прочитнное значение={3}",
                                deviceContex.Device.NodeId, msghelper.Index, obj.Value, msghelper.Value);
                            // Обновляем дату последнего обновления параметра
                            obj.Value = obj.Value;
                            // Устанавливаем код состояния объекта словаря
                            obj.Status = ObjectStatus.ConfigurationError;
                            // Записываем ошибку в лог
                            // ...
                            deviceContex.CurrentTransaction.Abort(msghelper.Answer, msg);
                        }
                        else
                        {
                            // Ошибки нет.
                            // Обновляем дату последнего обновления параметра
                            obj.Value = obj.Value;
                            // Устанавливаем код состояния объекта словаря
                            obj.Status = ObjectStatus.NoError;
                            deviceContex.CurrentTransaction.Stop(msghelper.Answer);
                        }
                    }
                    else
                    {
                        // Записываем новое значение в БД
                        // TODO: реализовать проверку на допустимые значение объекта
                        obj.Value = msghelper.Value;
                        obj.Status = ObjectStatus.NoError;
                        deviceContex.CurrentTransaction.Stop(msghelper.Answer);
                    }
                }
            }
        }
        /// <summary>
        /// Выполняет запрос к удалённому устройству 
        /// (переберая устройства и объекты словаря)
        /// </summary>
        public override void  HandleOutcomingMessages()
        {
            DeviceContext device;
            //ObjectInfo objInfo;

            // Сервис не работает
            if (_Status != Status.Running)
            {
                return;
            }

            if (_Context.Count == 0)
            {
                return;
            }
            // Устанавливаем следующее по списку устройство для обработки
            _Context.Next();
            // Получаем устройство для обработки
            device = _Context.CurrentDevice;

            // Если устройство активно, выполняем запрос
            lock (_SyncRoot)
            {
                if ((device.Device.Status == DeviceStatus.Operational) ||
                    (device.Device.Status == DeviceStatus.Preoperational) ||
                    (device.Device.Status == DeviceStatus.Stopped))
                {
                    if (device.CurrentTransaction == null)
                    {
                        device.CurrentTransaction = new Transaction();
                    }
                    // Проверяем, что последнй запрос (транзакция) к устройству выполнена
                    // Если это так, начинаем новую, в противном случае ничего неделаем (ждём окончания транзакции)
                    if (device.CurrentTransaction.IsRunning)
                    {
                        // Транзакция активна проверяем длительность транзакции
                        if (device.CurrentTransaction.TimeOfStart.AddSeconds(
                            device.Device.PollingInterval) <= DateTime.Now)
                        {
                            // Таймаут на запрос
                            device.CurrentTransaction.Abort(null, "Request timeout");
                            // Пишем в лог...
                        }
                        return;
                    }
                    else
                    {
                        // Если запрос был успешным или неудачным, выжадаем требуемое время
                        // для нового запроса
                        if ((device.CurrentTransaction.Status == TransactionStatus.Aborted) ||
                            (device.CurrentTransaction.Status == TransactionStatus.Completed))
                        {
                            if (device.CurrentTransaction.TimeOfStart.AddSeconds(
                                device.Device.PollingInterval) > DateTime.Now)
                            {
                                return;
                            }
                        }

                        if (device.CurrentTransaction.Status == TransactionStatus.Aborted)
                        {
                            if (device.ErrorCount < TotalAttempts)
                            {
                                // Повторяем запрос
                                device.CurrentTransaction.Repeat();
                                // Отправляем сообщение к устройству
                                _NetworkController.SendMessageToCanPort(
                                    device.CurrentTransaction.Request.Value);
                                return;
                            }
                            else
                            {
                                device.CurrentTransaction = null;
                            }
                        }

                        //Получаем следующий объект словаря для обработки 
                        device.Next();

                        if (!device.CurrentObject.SdoCanRead)
                        {
                            // Данный объект не обрабатывается сервисом SDO
                            return;
                        }
                        Transaction trans = new Transaction();

                        Frame message = new Frame();
                        message.Identifier = System.Convert.ToUInt32(0x600 +
                            device.Device.NodeId);
                        message.FrameFormat = FrameFormat.StandardFrame;
                        message.FrameType = FrameType.DATAFRAME;
                        message.Data = new Byte[8];
                        message.Data[0] = 0x40;
                        message.Data[1] = (byte)device.CurrentObject.Index;
                        message.Data[2] = (byte)(device.CurrentObject.Index >> 8);
                        device.CurrentTransaction = trans;
                        trans.Start(TransactionType.UnicastMode, message);
                    }
                    // Отправляем сообщение к устройству
                    _NetworkController.SendMessageToCanPort(
                        device.CurrentTransaction.Request.Value);
                }
            }
        }

        #endregion
    }
}
