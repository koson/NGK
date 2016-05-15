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
        /// ��� ��������� �������� ���������� � ������ �� ������
        /// (��� ������� NodeGuard)
        /// </summary>
        public enum DeviceStatusCode : byte
        {
            /// <summary>
            /// �����: ������� ����������������
            /// </summary>
            OPERATIONAL = 0x05,
            /// <summary>
            /// �����: ���������� �����������
            /// </summary>
            STOPPED = 0x04,
            /// <summary>
            /// �����: Pre-operational. (�� ����, PDO �� ��������, SDO - ��������)
            /// </summary>
            PREOPERATIONAL = 0x7F
        }
        /// <summary>
        /// ����������� ������ ���� DeviceStatusCode � 
        /// ������ ���� DeviceStatus 
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
                            "��������� �������� �������� {0} � ���� DeviceStatus", code);
                        throw new InvalidCastException();
                    }
            }
        }
        /// <summary>
        /// ��������� ��� �������������� ��������� ��� Boot Up �������
        /// �� ��������� ����������
        /// </summary>
        private struct IncomingMessageStuctureNodeGuard
        {
            #region Fields And Pproperties
            /// <summary>
            /// ����� ������ � ������
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
                    // �������� ������� ��������� ����������
                    Byte status = Answer.Value.Data[0];
                    // ���������� ������� ���, ������� ������ �������������� 
                    // ��� ������ ������� � ����������
                    status &= MASK_COBEID;
                    return status;
                }
            }
            internal DeviceStatusCode StatusCode
            {
                get 
                {
                    String msg;
                    // �������� ������� ��������� ����������
                    Byte status = Answer.Value.Data[0];
                    // ���������� ������� ���, ������� ������ �������������� 
                    // ��� ������ ������� � ����������
                    status &= 0x7F;
                    // ��������� ��� ��������� ����������. ���� �� �������, �� true.
                    if (Enum.IsDefined(typeof(DeviceStatusCode), status))
                    {
                        return (DeviceStatusCode)status;
                    }
                    msg = String.Format(
                        "���������� ������������� ��� {0} � DeviceStatusCode", status);
                    throw new InvalidCastException(msg);
                }
            }
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
                    // ������������� 11100000000 �.�. Fct code (1110) + CodId (7 ���)
                    const UInt16 MASK = 0x700;
                    // �������� 7 ��� ���������� CodeId �� ���� Id
                    
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
                    if (DL == 1)
                    {
                        // �� �����, �������� ������� ��������� ����������
                        Byte status = Answer.Value.Data[0];
                        // ���������� ������� ���, ������� ������ �������������� ��� ������ ������� � ����������
                        status &= 0x7F;
                        // ��������� ��� ��������� ����������. ���� �� �������, �� true.
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
            /// ��������� �������� ���������
            /// </summary>
            /// <param name="message">�������� ���������</param>
            /// <returns>��������� ������ ������</returns>
            internal static IncomingMessageStuctureNodeGuard Parse(Frame message)
            {
                const Byte MASK_COBEID = 0x7F; // �������� 7 ��� ���������� CodeId �� ���� Id 
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
        /// ����������� �� ���������
        /// </summary>
        private ServiceNodeGuard()
            : base(null)
        {
            throw new NotImplementedException(
                "������� ������� ����������� ����������� ������ ServiceNodeGuard");
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="controller">���������� ����</param>
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
        /// ���������� ��������� ���������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EventHandlerDevicesCollectionWasChanged(
            object sender, KeyedCollectionWasChangedEventArgs<DeviceBase> e)
        {
            // � ������ ��������� ��������� �������������� ��������
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
                        // ��� ��������� � 0 ������������ ���������� Boot-Up
                        // ������� ������ ��������� �� ������� ���������. ��� ���
                        // ������� Boot-Up
                    }
                    else
                    {
                        // ������ ��������� �������.
                        msg = String.Format(
                            "Network {0}: ������� ��������� � �������� �������� ������ {1}",
                            _NetworkController.NetworkName, message.ToString());
                        //_Logger.Error(msg);
                    }
                    continue;
                }

                if (!_NetworkController.Devices.Contains(msghelper.CobeId))
                {
                    // ���������� �� �������
                    msg = String.Format(
                        "Network {0}: ������ ��������� �� ���������� � NodeId {1}, " +
                        "������ ���������� �� ���������������� � ����. Message - {2}",
                        this.NetworkController.NetworkName, msghelper.CobeId, message.ToString());
                    //Logger.Error(msg);
                    continue;
                }

                // ���������� �������. 
                lock (_SyncRoot)
                {
                    // ��������� ����������
                    Transaction trns = _Context.FindDevice(msghelper.CobeId).CurrentTransaction;

                    if (trns != null)
                    {
                        trns.Stop(message);
                        // ������������� ����� ������ ����������
                        _NetworkController.Devices[msghelper.CobeId].Status =
                            ServiceNodeGuard.ToDeviceStatus(msghelper.StatusCode);
                    }
                    else
                    {
                        // ������ ����� � ���������� �������
                        throw new Exception();
                    }
                }
                // ����� � ������... �� �����������
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

            // ������ �� ��������
            if (_Status != Status.Running)
            {
                return;
            }

            if (_Context.Count == 0)
            {
                return;
            }
            // �������� ��������� ���������� � �����
            _Context.Next();
            
            device = _Context.CurrentDevice;
            
            //if (device.Device.Status == DeviceStatus.CommunicationError)
            //{
                // ���� ���������� �� ��������, ���������� ���
                //return;
            //}

            // ��������� ������ ������� 
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
                            // ���������� �� �������o �� �������� �����. 
                            // ������������� ������ ��������� �������
                            device.CurrentTransaction.Abort(null, "Request timeout"); 
                        }
                        break;
                    }
                case TransactionStatus.Aborted:
                    {
                        // ��������� ���������� ������� ������� � ����������.
                        // ���� ��� �������� �������. ������������� ������ ������
                        if (device.ErrorCount < TotalAttempts)
                        {
                            // ��������� ������
                            device.CurrentTransaction.Repeat();
                            _NetworkController.SendMessageToCanPort(
                                device.CurrentTransaction.Request.Value);
                        }
                        else
                        {
                            // ������������� ����� ������ ����������
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
                        // ���������� ������
                        _NetworkController.SendMessageToCanPort(
                            device.CurrentTransaction.Request.Value);
                        break;
                    }
                case TransactionStatus.Completed:
                    {
                        // �������� ����� ���������� ������ ����������. 
                        // ���������� � ���� ����� �������� ������ ����������
                        // � ���������� � ������� ��������. ���� ���������� ����� ������ ��������, 
                        // �� ��������� ������ � ����������.
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
                            // ���������� ������
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
