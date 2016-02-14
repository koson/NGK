using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NLog;
using NGK.CAN.ApplicationLayer.Network.Master.Services;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Network.Devices;
using Common.Controlling;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// Сетевой сервис Boot Up
    /// </summary>
    public sealed class ServiceBootUp: Service
    {
        #region Helper
        /// <summary>
        /// Структура для предствавления сообщения для Boot Up сервиса
        /// от удалённого устройства
        /// </summary>
        private struct IncomingMessageStuctureBootUp
        {
            #region Fields And Pproperties
            /// <summary>
            /// Длина данных в ответе
            /// </summary>
            internal int DL;
            internal Byte CobeId;
            internal Frame? Answer;
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
                    const UInt16 MASK = 0x700;

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
                        if (Answer.Value.Data[0] == 0)
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
            internal static IncomingMessageStuctureBootUp Parse(Frame message)
            {
                const Byte MASK_COBEID = 0x7F; // Выделяет 7 бит содержащих CodeId из поля Id 
                IncomingMessageStuctureBootUp frame = 
                    new IncomingMessageStuctureBootUp();
                frame.Answer = message;
                frame.CobeId = (Byte)(((Byte)message.Identifier) & MASK_COBEID);
                frame.DL = message.Data.Length;
                return frame;
            }
            #endregion
        }
        #endregion

        #region Fields And Properties
        //private static Logger _Logger = LogManager.GetLogger("BootUpLogger");

        protected override Logger Logger
        {
            get 
            {
                return null;
                //return _Logger; 
            }
        }

        public override ServiceType ServiceType
        {
            get { return ServiceType.BootUp; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        private ServiceBootUp(): base(null)
        {
            throw new NotImplementedException(
                "Попытка вызвать запрещённый конструктор класса ServiceBootUp");
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controller"></param>
        public ServiceBootUp(INetworkController controller)
            : base(controller)
        { 
        }
        #endregion

        #region Methods
        /// <summary>
        /// Обработчик исхдящих сообщений
        /// </summary>
        public override void HandleOutcomingMessages()
        {
            // Данный сервис не имеет исходящих сообщений
            return;
        }
        /// <summary>
        /// Обработчик входящих сообщений из сети
        /// </summary>
        /// <param name="message">Входящее сообщение для обработки</param>
        //private void HandleIncomingMessages(Frame message)
        public override void HandleIncomingMessages(Frame[] messages)
        {
            String msg;
            IncomingMessageStuctureBootUp msghelper;
            DeviceBase device;
       
            if (Status != Status.Running)
            {
                return;
            }

            foreach (Frame message in messages)
            {
                msghelper = IncomingMessageStuctureBootUp.Parse(message);

                if (!msghelper.IsForService)
                {
                    continue;
                }

                if (msghelper.HasIncorrectStructure)
                {
                    //!!!Warning: Длина данных в поле данных корректна, но содержание неверно
                    // (message.Data[0] == 0x84) || (message.Data[0] == 0x04) ||
                    // (message.Data[0] == 0x85) || (message.Data[0] == 0x05) ||
                    // (message.Data[0] == 0xFF) || (message.Data[0] == 0x7F))
                    // Эти коды используются в протоколе NodeGuard.
                    continue;
                }

                // Сообщение для этого сервиса.
                //msg = String.Format("Network {0}: Сервис {1}: Service BootUp принял сообщение: {2}",
                //    base.NetworkController.NetworkName(), this.ServiceName, message.ToString());
                //_Logger.Trace(msg);

                //Ищем устройство которое прислало сообщение
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
                // Устанавливаем новый статус устройству
                device = _NetworkController.Devices[msghelper.CobeId];

                lock (_SyncRoot)
                {
                    device.Status = DeviceStatus.Preoperational;
                }
                // Пишем в журнал... Не реализовано
            }
        }
        
        #endregion

        #region Events
        #endregion
    }
}
