using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using Common.Controlling;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// ������ PDO Transmit
    /// </summary>
    /// <remarks>������ ������ �� ����� �������� ���������</remarks>
    public sealed class ServicePdoTransmit: Service
    {
        #region Helper
        /// <summary>
        /// ���� PDO-���������
        /// </summary>
        private enum PdoType
        {
            Unknown,
            PDO1,
            PDO2,
            PDO3,
            PDO4
        }
        /// <summary>
        /// ��������� ��� �������������� ��������� ��� Boot Up �������
        /// �� ��������� ����������
        /// </summary>
        private struct IncomingMessageStuctureServicePdoTransmit
        {
            #region Fields And Pproperties
            /// <summary>
            /// ����� ������ � ������
            /// </summary>
            internal int DL;
            internal Byte NodeId;
            internal PdoType Pdo;
            internal Frame? Answer;
            public bool _IsForService;
            /// <summary>
            /// ���������� true, ���� ��������� �������������
            /// ��� ������� �������
            /// </summary>
            internal bool IsForService
            {
                get
                {
                    return _IsForService;
                }
            }
            private bool _IsCorrectStructure;
            /// <summary>
            /// ���������� true, ���� ��������� ����� �������� ������ ���������
            /// </summary>
            internal bool HasIncorrectStructure
            {
                get
                {
                    return !_IsCorrectStructure;
                }
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
            internal static IncomingMessageStuctureServicePdoTransmit Parse(Frame message)
            {
                // ����� ��� ��������� (Fct code: 4 ������� ����) �� ������ ���������� (7 ������� ���)
                // ���������� � CobId ���������
                const UInt16 MASK_FCT_CODE = 0x780; // 11110000000;
                // ������������� CodId, �� �������� ������������ ��� �������� ���������
                // ������������� ��� ������� �������
                const UInt16 MASK_PDO1 = 0x180;
                const UInt16 MASK_PDO2 = 0x280;
                const UInt16 MASK_PDO3 = 0x380;
                const UInt16 MASK_PDO4 = 0x480;
                const Byte MASK_COBEID = 0x7F; // �������� 7 ��� ���������� CodeId �� ���� Id 
                
                IncomingMessageStuctureServicePdoTransmit frame =
                    new IncomingMessageStuctureServicePdoTransmit();
                
                frame.Answer = message;
                
                // �������� 7 ��� ���������� CodeId �� ���� Id
                    // �������� ������ � ������ ����� ��������� ������ ����������.
                if (frame.Answer.Value.FrameType == FrameType.DATAFRAME)
                {
                    UInt16 cobId = (UInt16)(frame.Answer.Value.Identifier & MASK_FCT_CODE);

                    switch (cobId)
                    {
                        case MASK_PDO1:
                            {
                                frame.Pdo = PdoType.PDO1;
                                frame._IsForService = true;
                                // ��������� ������������ ������
                                frame._IsCorrectStructure = frame.Answer.Value.Data.Length == 8 ?
                                    true : false;
                                break;
                            }
                        case MASK_PDO2:
                            {
                                frame.Pdo = PdoType.PDO2;
                                frame._IsForService = true;
                                // ��������� ������������ ������
                                frame._IsCorrectStructure = frame.Answer.Value.Data.Length == 7 ?
                                    true : false;
                                break;
                            }
                        case MASK_PDO3:
                            {
                                frame.Pdo = PdoType.PDO3;
                                frame._IsForService = true;
                                // ��������� ������������ ������
                                frame._IsCorrectStructure = frame.Answer.Value.Data.Length == 6 ?
                                    true : false;
                                break;
                            }
                        case MASK_PDO4:
                            {
                                frame.Pdo = PdoType.PDO4;
                                frame._IsForService = true;
                                // ��������� ������������ ������
                                frame._IsCorrectStructure = frame.Answer.Value.Data.Length == 4 ?
                                    true : false;
                                break;
                            }
                        default:
                            {
                                frame.Pdo = PdoType.Unknown;
                                frame._IsForService = false;
                                // ��������� ������������ ������
                                frame._IsCorrectStructure = false;
                                break;
                            }
                    }
                }
                else
                {
                    frame.Pdo = PdoType.Unknown;
                    frame._IsForService = false;
                }

                frame.NodeId = (Byte)(((Byte)message.Identifier) & MASK_COBEID);
                frame.DL = message.Data.Length;
                
                
                return frame;
            }
            #endregion
        }

        #endregion

        #region Fields And Properties

        private static Logger _Logger = null; //LogManager.GetLogger("PdoTransmitLogger");

        protected override Logger Logger
        {
            get { return _Logger; }
        }

        public override ServiceType ServiceType
        {
            get { return ServiceType.PdoTransmit; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// ����������� �� ���������
        /// </summary>
        private ServicePdoTransmit()
            : base(null)
        {
            throw new NotImplementedException(
                "������� ������� ����������� ����������� ������ ServicePdoTransmit");
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="controller">���������� ����</param>
        public ServicePdoTransmit(INetworkController controller)
            : base(controller)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        public override void HandleIncomingMessages(Frame[] messages)
        {
            String msg;
            IncomingMessageStuctureServicePdoTransmit msghelper;
            DeviceBase device;
            //ObjectInfo objInfo;

            if (_Status != Status.Running)
            {
                return;
            }
            
            foreach(Frame message in messages)
            {
                msghelper = IncomingMessageStuctureServicePdoTransmit.Parse(message);

                if (!msghelper.IsForService)
                {
                    return;
                }

                if (msghelper.HasIncorrectStructure)
                {
                    // ������ ��������� �������.
                    msg = String.Format(
                        "Network {0}: ������� ��������� � �������� �������� ������ {1}",
                        _NetworkController.NetworkName, message.ToString());
                    //_Logger.Error(msg);
                    return;
                }

                if (!_NetworkController.Devices.Contains(msghelper.NodeId))
                {
                    // ���������� �� �������
                    msg = String.Format(
                        "Network {0}: ������ ��������� �� ���������� � NodeId {1}, " +
                        "������ ���������� �� ���������������� � ����. Message - {2}",
                        this.NetworkController.NetworkName, msghelper.NodeId, message.ToString());
                    //Logger.Error(msg);
                    continue;
                }

                device = _NetworkController.Devices[msghelper.NodeId];

                switch (msghelper.Pdo)
                {
                    case PdoType.PDO1:
                        {
                            // Data A (polarisation_pot)
                            device.ObjectDictionary[(UInt16)0x2008].Value =
                                ToUInt16(message.Data[0], message.Data[1]);
                            // Data B (protection_pot)
                            device.ObjectDictionary[(UInt16)0x2009].Value =
                                ToUInt16(message.Data[2], message.Data[3]);
                            // Data C (induced_ac)
                            device.ObjectDictionary[(UInt16)0x200A].Value =
                                ToUInt16(message.Data[4], message.Data[5]);
                            // Data D (protection_cur)
                            device.ObjectDictionary[(UInt16)0x200B].Value =
                                ToUInt16(message.Data[6], message.Data[7]);
                            break;
                        }
                    case PdoType.PDO2:
                        {
                            // Data A (polarisation_cur)
                            device.ObjectDictionary[(UInt16)0x200C].Value =
                                ToUInt16(message.Data[0], message.Data[1]);
                            // Data B (aux_cur1)
                            device.ObjectDictionary[(UInt16)0x200D].Value =
                                ToUInt16(message.Data[2], message.Data[3]);
                            // Data C (induced_ac)
                            device.ObjectDictionary[(UInt16)0x200E].Value =
                                ToUInt16(message.Data[4], message.Data[5]);
                            // Data D (���� ��������� ��������)
                            // 0bxxxxxD3D2D1: byte
                            // D1 - ���, ��������� ������� �������� tamper
                            // D2 - ���, ��������� supply_voltage_low
                            // D3 - ���, ��������� battary_volatage_low
                            device.ObjectDictionary[(UInt16)0x2015].Value =
                                (UInt16)(message.Data[6] & 0x01);
                            device.ObjectDictionary[(UInt16)0x2016].Value =
                                (UInt16)(message.Data[6] & 0x02);
                            device.ObjectDictionary[(UInt16)0x2017].Value =
                                (UInt16)(message.Data[6] & 0x04);
                            break;
                        }
                    case PdoType.PDO3:
                        {
                            // Data A (corrosion_depth)
                            device.ObjectDictionary[(UInt16)0x200F].Value =
                                ToUInt16(message.Data[0], message.Data[1]);
                            // Data B (corrosion_speed)
                            device.ObjectDictionary[(UInt16)0x2010].Value =
                                ToUInt16(message.Data[2], message.Data[3]);
                            // Data C (usipk_state)
                            device.ObjectDictionary[(UInt16)0x2011].Value =
                                message.Data[4];
                            // Data D (���� ��������� �������� ��������)
                            // 0bxxxxxD3D2D1: byte
                            // D1 - ���, ��������� ������� �������� 1
                            // D2 - ���, ��������� ������� �������� 2
                            // D3 - ���, ��������� ������� �������� 3
                            device.ObjectDictionary[(UInt16)0x2018].Value =
                                (UInt16)(message.Data[5] & 0x01);
                            device.ObjectDictionary[(UInt16)0x2019].Value =
                                (UInt16)(message.Data[5] & 0x02);
                            device.ObjectDictionary[(UInt16)0x201A].Value =
                                (UInt16)(message.Data[5] & 0x04);
                            break;
                        }
                    case PdoType.PDO4:
                        {
                            // Data A (polarisation_cur_dc)
                            device.ObjectDictionary[(UInt16)0x201B].Value =
                                ToUInt16(message.Data[0], message.Data[1]);
                            // Data B (aux_cur1)
                            device.ObjectDictionary[(UInt16)0x201C].Value =
                                ToUInt16(message.Data[2], message.Data[2]);
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        private static UInt16 ToUInt16(byte low, byte high)
        {
            unchecked
            {
                return (UInt16)((high << 8) | (low));
            }
        }
        #endregion
    }
}
