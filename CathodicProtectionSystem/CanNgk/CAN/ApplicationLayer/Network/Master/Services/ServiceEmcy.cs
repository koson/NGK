using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Master.Services;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Network.Devices;
using Common.Controlling;
using NGK.Log;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// Сетевой сервис Emergency
    /// </summary>
    public sealed class ServiceEmcy: Service
    {
        #region Helper
        /// <summary>
        /// Структура для предствавления сообщения для Boot Up сервиса
        /// от удалённого устройства
        /// </summary>
        private struct IncomingMessageStuctureEmcy
        {
            /// <summary>
            /// Коды ошибок в поле ErrCode
            /// </summary>
            internal enum ErrCode: ushort
            {
                /// <summary>
                /// Ошибки отсутствуют
                /// </summary>
                NoError = 0x0000,
                /// <summary>
                /// Вскрытие
                /// </summary>
                Tamper = 0x0001,
                /// <summary>
                /// ошибка внешнего питания
                /// </summary>
                MainSupplyPowerError = 0x0002,
                /// <summary>
                /// неисправность внутренней батареи питания
                /// </summary>
                BatteryError = 0x0003,
                /// <summary>
                /// ошибка регистрации
                /// </summary>
                RegistrationError = 0x0004,
                /// <summary>
                /// ошибка дублирования адреса.
                /// </summary>
                DuplicateAddressError = 0x0005,
                /// <summary>
                /// подключение сервисного разъёма.
                /// </summary>
                ConnectedServiceConnector   = 0x0006
            }

            /// <summary>
            /// Флаги поля err_reg сообщения EMCY
            /// </summary>
            [Flags]
            internal enum ErrReg: byte
            {
                /// <summary>
                /// Есть Вскрытие 
                /// </summary>
                Tamper = 0x01,
                /// <summary>
                /// Oшибка внешнего (основного) питания
                /// </summary>
                MainSupplyPowerError = 0x02,
                /// <summary>
                /// Неисправность внутренней батареи питания
                /// </summary>
                BatteryError = 0x04,
                /// <summary>
                /// Ошибка регистрации БИ(У)-01
                /// </summary>
                RegistrationError = 0x08,
                /// <summary>
                /// Ошибка дублирования адреса БИ(У)-01.
                /// </summary>
                DuplicateAddressError = 0x10,
                /// <summary>
                /// Подключение сервисного разъёма.
                /// </summary>
                ConnectedServiceConnector   = 0x20
            }

            /// <summary>
            /// Структура принимает байт поля err_reg и 
            /// предствавляет его в виде флагов
            /// </summary>
            internal struct ErrorFlags
            {
                #region Fields And Properties

                private Byte _FlagsByte;

                public Byte FlagsByte
                {
                    get { return _FlagsByte; }
                    set { _FlagsByte = value; }
                }
                
                internal bool HasErrors
                {
                    get { return _FlagsByte == 0 ? false : true; }
                }

                /// <summary>
                /// Есть Вскрытие 
                /// </summary>
                internal bool Tamper 
                { 
                    get 
                    {
                        return (_FlagsByte & (byte)ErrReg.Tamper) == 
                            (byte)ErrReg.Tamper ? true : false;
                    }
                }
 
                /// <summary>
                /// Oшибка внешнего (основного) питания
                /// </summary>
                internal bool MainSupplyPowerError
                {
                    get
                    {
                        return (_FlagsByte & (byte)ErrReg.MainSupplyPowerError) ==
                            (byte)ErrReg.MainSupplyPowerError ? true : false;
                    }
                }

                /// <summary>
                /// Неисправность внутренней батареи питания
                /// </summary>
                internal bool BatteryError
                {
                    get
                    {
                        return (_FlagsByte & (byte)ErrReg.BatteryError) ==
                            (byte)ErrReg.BatteryError ? true : false;
                    }
                } 

                /// <summary>
                /// Ошибка регистрации БИ(У)-01
                /// </summary>
                internal bool RegistrationError
                {
                    get
                    {
                        return (_FlagsByte & (byte)ErrReg.RegistrationError) ==
                            (byte)ErrReg.RegistrationError ? true : false;
                    }
                } 

                /// <summary>
                /// Ошибка дублирования адреса БИ(У)-01.
                /// </summary>
                internal bool DuplicateAddressError
                {
                    get
                    {
                        return (_FlagsByte & (byte)ErrReg.DuplicateAddressError) ==
                            (byte)ErrReg.DuplicateAddressError ? true : false;
                    }
                } 

                /// <summary>
                /// Подключение сервисного разъёма.
                /// </summary>
                internal bool ConnectedServiceConnector
                {
                    get
                    {
                        return (_FlagsByte & (byte)ErrReg.ConnectedServiceConnector) ==
                            (byte)ErrReg.ConnectedServiceConnector ? true : false;
                    }
                } 

                #endregion

                #region Constructors
                
                internal ErrorFlags(Byte flagsByte)
                {
                    _FlagsByte = flagsByte;
                }

                #endregion

                #region Methods

                internal static ErrorFlags Parse(Byte flagsByte)
                {
                    return new ErrorFlags(flagsByte);
                }

                #endregion 
            }

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
                    const UInt16 MASK = 0x80;

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
                    if (DL != 8)
                    {
                        return true;
                    }
                    return false;
                }
            }
            /// <summary>
            /// Возвращает код ошибки (Поле ErrCode в сообщении)
            /// </summary>
            internal ErrCode ErrorCode
            {
                get 
                {
                    UInt16 error = (UInt16)((((UInt16)Answer.Value.Data[0]) >> 8) | Answer.Value.Data[1]);
                    if (Enum.IsDefined(typeof(ErrCode), error))
                    {
                        return (ErrCode)error;
                    }
                    else
                    {
                        throw new InvalidCastException(String.Format(
                            "Неудалось привести значение {0} к типу ErrCode", error));
                    } 
                }
            }
            /// <summary>
            /// Возвращает байт флагов ошибок устройтсва
            /// </summary>
            internal ErrorFlags ErrorRegister
            {
                get
                {
                    return ErrorFlags.Parse(Answer.Value.Data[2]);
                }
            }
            /// <summary>
            /// Возвращает номер шлюза 1...4 при наличии ErrCode = 
            /// 0x0004 – ошибка регистрации или 0x0005 – ошибка дублирования адреса.
            /// </summary>
            internal Byte ChannelNumber
            {
                get { return Answer.Value.Data[3]; }
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
            internal static IncomingMessageStuctureEmcy Parse(Frame message)
            {
                const Byte MASK_COBEID = 0x7F; // Выделяет 7 бит содержащих CodeId из поля Id 
                IncomingMessageStuctureEmcy frame =
                    new IncomingMessageStuctureEmcy();
                frame.Answer = message;
                frame.CobeId = (Byte)(((Byte)message.Identifier) & MASK_COBEID);   
                frame.DL = message.Data.Length;
                return frame;
            }
            #endregion
        }

        #endregion

        #region Fields And Properties

        public override ServiceType ServiceType
        {
            get { return ServiceType.Emcy; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        private ServiceEmcy(): base(null)
        {
            throw new NotImplementedException(
                "Попытка вызвать запрещённый конструктор класса ServiceEmcy");
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controller"></param>
        public ServiceEmcy(ICanNetworkController controller)
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
            IncomingMessageStuctureEmcy msghelper;
            DeviceBase device;
            UInt16 index;


            if (Status != Status.Running)
            {
                return;
            }

            foreach (Frame message in messages)
            {
                msghelper = IncomingMessageStuctureEmcy.Parse(message);

                if (!msghelper.IsForService)
                {
                    continue;
                }

                if (msghelper.HasIncorrectStructure)
                {
                    continue;
                }

                // Сообщение для этого сервиса.
                //msg = String.Format("Network {0}: Сервис {1}: Service Emcy принял сообщение: {2}",
                //    base.NetworkController.NetworkName(), this.ServiceName, message.ToString());
                //_Logger.Trace(msg);

                //Ищем устройство которое прислало сообщение
                if (!_NetworkController.Devices.Contains(msghelper.CobeId))
                {
                    // Устройство не найдено
                    msg = String.Format(
                        "Network {0}: Пришло сообщение от устройства с NodeId {1}, " +
                        "данное устройство не найдено конфигурации в сети. Message - {2}",
                        this.NetworkController.NetworkName, msghelper.CobeId, message.ToString());
                    //Logger.Error(msg);
                    continue;
                }

                // Устройство найдено. 
                // Устанавливаем новый статус устройству
                device = _NetworkController.Devices[msghelper.CobeId];

                //Анализируем код ошибки (источник или причина возникновения ошибки)
                switch (msghelper.ErrorCode)
                {
                    case IncomingMessageStuctureEmcy.ErrCode.NoError:
                        {
                            // Все ошибки исправлены
                            if (msghelper.ErrorRegister.FlagsByte != 0)
                            {
                                msg = String.Format(
                                    "Неверное состояние - передан ErrCode=0 и ожидается err_reg=0, a получено err_reg = {0}",
                                    msghelper.ErrorRegister);
                                throw new Exception(msg);
                            }

                            // Сбрасываем ошибки в устройстве если были
                            IEmcyErrors err = (IEmcyErrors)device;
                            
                            lock (_SyncRoot)
                            {
                                err.BatteryError = false;
                                err.ConnectedServiceConnector = false;
                                err.DuplicateAddressError = false;
                                err.MainSupplyPowerError = false;
                                err.RegistrationError = false;
                                err.Tamper = false;
                            }                            

                            break;
                        }
                    case IncomingMessageStuctureEmcy.ErrCode.Tamper:
                    case IncomingMessageStuctureEmcy.ErrCode.BatteryError:
                    case IncomingMessageStuctureEmcy.ErrCode.MainSupplyPowerError:
                    case IncomingMessageStuctureEmcy.ErrCode.ConnectedServiceConnector:
                    case IncomingMessageStuctureEmcy.ErrCode.DuplicateAddressError: // TODO: обработать поле Х - номер канала шлюза
                    case IncomingMessageStuctureEmcy.ErrCode.RegistrationError: // TODO: обработать поле Х - номер канала шлюза
                        {
                            IEmcyErrors err = (IEmcyErrors)device;

                            lock (_SyncRoot)
                            {
                                err.BatteryError = msghelper.ErrorRegister.BatteryError;
                                err.ConnectedServiceConnector = 
                                    msghelper.ErrorRegister.ConnectedServiceConnector;
                                err.DuplicateAddressError = 
                                    msghelper.ErrorRegister.DuplicateAddressError;
                                err.MainSupplyPowerError = 
                                    msghelper.ErrorRegister.MainSupplyPowerError;
                                err.RegistrationError = 
                                    msghelper.ErrorRegister.RegistrationError;
                                err.Tamper = msghelper.ErrorRegister.Tamper;
                            } 

                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }

                //TODO: Пишем в журнал... Не реализовано
            }
        }

        #endregion
    }
}
