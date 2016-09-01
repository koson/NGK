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
    /// ������� ������ Emergency
    /// </summary>
    public sealed class ServiceEmcy: Service
    {
        #region Helper
        /// <summary>
        /// ��������� ��� �������������� ��������� ��� Boot Up �������
        /// �� ��������� ����������
        /// </summary>
        private struct IncomingMessageStuctureEmcy
        {
            /// <summary>
            /// ���� ������ � ���� ErrCode
            /// </summary>
            internal enum ErrCode: ushort
            {
                /// <summary>
                /// ������ �����������
                /// </summary>
                NoError = 0x0000,
                /// <summary>
                /// ��������
                /// </summary>
                Tamper = 0x0001,
                /// <summary>
                /// ������ �������� �������
                /// </summary>
                MainSupplyPowerError = 0x0002,
                /// <summary>
                /// ������������� ���������� ������� �������
                /// </summary>
                BatteryError = 0x0003,
                /// <summary>
                /// ������ �����������
                /// </summary>
                RegistrationError = 0x0004,
                /// <summary>
                /// ������ ������������ ������.
                /// </summary>
                DuplicateAddressError = 0x0005,
                /// <summary>
                /// ����������� ���������� �������.
                /// </summary>
                ConnectedServiceConnector   = 0x0006
            }

            /// <summary>
            /// ����� ���� err_reg ��������� EMCY
            /// </summary>
            [Flags]
            internal enum ErrReg: byte
            {
                /// <summary>
                /// ���� �������� 
                /// </summary>
                Tamper = 0x01,
                /// <summary>
                /// O����� �������� (���������) �������
                /// </summary>
                MainSupplyPowerError = 0x02,
                /// <summary>
                /// ������������� ���������� ������� �������
                /// </summary>
                BatteryError = 0x04,
                /// <summary>
                /// ������ ����������� ��(�)-01
                /// </summary>
                RegistrationError = 0x08,
                /// <summary>
                /// ������ ������������ ������ ��(�)-01.
                /// </summary>
                DuplicateAddressError = 0x10,
                /// <summary>
                /// ����������� ���������� �������.
                /// </summary>
                ConnectedServiceConnector   = 0x20
            }

            /// <summary>
            /// ��������� ��������� ���� ���� err_reg � 
            /// ������������� ��� � ���� ������
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
                /// ���� �������� 
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
                /// O����� �������� (���������) �������
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
                /// ������������� ���������� ������� �������
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
                /// ������ ����������� ��(�)-01
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
                /// ������ ������������ ������ ��(�)-01.
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
                /// ����������� ���������� �������.
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
            /// ����� ������ � ������
            /// </summary>
            internal int DL;
            internal Byte CobeId;
            internal Frame? Answer;
            /// <summary>
            /// ���������� true, ���� ��������� �������������
            /// ��� ������� �������
            /// </summary>
            internal bool IsForService
            {
                get
                {
                    // ����� ��� ��������� (Fct code: 4 ������� ����) �� ������ ���������� (7 ������� ���)
                    // ���������� � CobId ���������
                    const UInt16 MASK_FCT_CODE = 0x780; // 11110000000;
                    // ������������� CodId, �� �������� ������������ ��� �������� ���������
                    // ������������� ��� ������� �������
                    const UInt16 MASK = 0x80;

                    // �������� ������ � ������ ����� ��������� ������ ����������.
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
            /// ���������� true, ���� ��������� ����� �������� ������ ���������
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
            /// ���������� ��� ������ (���� ErrCode � ���������)
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
                            "��������� �������� �������� {0} � ���� ErrCode", error));
                    } 
                }
            }
            /// <summary>
            /// ���������� ���� ������ ������ ����������
            /// </summary>
            internal ErrorFlags ErrorRegister
            {
                get
                {
                    return ErrorFlags.Parse(Answer.Value.Data[2]);
                }
            }
            /// <summary>
            /// ���������� ����� ����� 1...4 ��� ������� ErrCode = 
            /// 0x0004 � ������ ����������� ��� 0x0005 � ������ ������������ ������.
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
            /// ��������� �������� ���������
            /// </summary>
            /// <param name="message">�������� ���������</param>
            /// <returns>��������� ������ ������</returns>
            internal static IncomingMessageStuctureEmcy Parse(Frame message)
            {
                const Byte MASK_COBEID = 0x7F; // �������� 7 ��� ���������� CodeId �� ���� Id 
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
        /// �����������
        /// </summary>
        private ServiceEmcy(): base(null)
        {
            throw new NotImplementedException(
                "������� ������� ����������� ����������� ������ ServiceEmcy");
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="controller"></param>
        public ServiceEmcy(ICanNetworkController controller)
            : base(controller)
        { 
        }
        #endregion

        #region Methods
        /// <summary>
        /// ���������� �������� ���������
        /// </summary>
        public override void HandleOutcomingMessages()
        {
            // ������ ������ �� ����� ��������� ���������
            return;
        }
        /// <summary>
        /// ���������� �������� ��������� �� ����
        /// </summary>
        /// <param name="message">�������� ��������� ��� ���������</param>
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

                // ��������� ��� ����� �������.
                //msg = String.Format("Network {0}: ������ {1}: Service Emcy ������ ���������: {2}",
                //    base.NetworkController.NetworkName(), this.ServiceName, message.ToString());
                //_Logger.Trace(msg);

                //���� ���������� ������� �������� ���������
                if (!_NetworkController.Devices.Contains(msghelper.CobeId))
                {
                    // ���������� �� �������
                    msg = String.Format(
                        "Network {0}: ������ ��������� �� ���������� � NodeId {1}, " +
                        "������ ���������� �� ������� ������������ � ����. Message - {2}",
                        this.NetworkController.NetworkName, msghelper.CobeId, message.ToString());
                    //Logger.Error(msg);
                    continue;
                }

                // ���������� �������. 
                // ������������� ����� ������ ����������
                device = _NetworkController.Devices[msghelper.CobeId];

                //����������� ��� ������ (�������� ��� ������� ������������� ������)
                switch (msghelper.ErrorCode)
                {
                    case IncomingMessageStuctureEmcy.ErrCode.NoError:
                        {
                            // ��� ������ ����������
                            if (msghelper.ErrorRegister.FlagsByte != 0)
                            {
                                msg = String.Format(
                                    "�������� ��������� - ������� ErrCode=0 � ��������� err_reg=0, a �������� err_reg = {0}",
                                    msghelper.ErrorRegister);
                                throw new Exception(msg);
                            }

                            // ���������� ������ � ���������� ���� ����
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
                    case IncomingMessageStuctureEmcy.ErrCode.DuplicateAddressError: // TODO: ���������� ���� � - ����� ������ �����
                    case IncomingMessageStuctureEmcy.ErrCode.RegistrationError: // TODO: ���������� ���� � - ����� ������ �����
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

                //TODO: ����� � ������... �� �����������
            }
        }

        #endregion
    }
}
