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
    /// ����� ��������� ������� ������ NMT ��������� CAN ���-���
    /// </summary>
    public sealed class ServiceNmt: Service
    {
        /// <summary>
        /// ���� ������ ��� ���������� ������ ����
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
        /// ����������� �� ��������� ��������
        /// </summary>
        private ServiceNmt()
            : base(null)
        {
            throw new NotImplementedException(
                "������� ������� ����������� ����������� ������ ServiceNmt");
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="controller">���������� ����</param>
        public ServiceNmt(INetworkController controller)
            : base(controller)
        {
            //�������������� ��������� ���������
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
        public override void HandleOutcomingMessages()
        {
            DeviceContext device;
            String msg;
            DateTime time;
            Frame request;
            Transaction transaction;

            // 1. ��������� ������ ����������: 
            //  1.1 ���� ���������� ��������� � ��������� Operational, ������ �� ������
            //  1.2 ���� ���������� ��������� � ��������� CommunicationError, ������ �� ������
            //  1.3 ���� ���������� ��������� � ��������� ConfigurationError, ������ �� ������
            //  1.4 ���� ���������� ��������� � ��������� Stopped, ������ �� ������
            //  1.5 ���� ���������� ��������� � ��������� Pre-operational, ���������� ������� 
            //      �������� ���������� � ��������� Operational
            //     1.5.1 ��������� 

            // ������ �� ��������
            if (_Status != Status.Running)
            {
                return;
            }

            // �������� ����������
            _Context.Next();
            device = _Context.CurrentDevice;

            // ��������� ������
            if (device.Device.Status == DeviceStatus.Preoperational)
            {
                if (device.CurrentTransaction != null)
                {
                    switch (device.CurrentTransaction.Status)
                    {
                        case TransactionStatus.Running:
                            {
                                // ������ ��� ��� ���������. ��������� �������
                                time = device.CurrentTransaction.TimeOfStart;
                                time = time.AddSeconds(device.Device.PollingInterval);

                                if (time < DateTime.Now)
                                {
                                    msg = String.Format("Service NMT: Timeout - ����������" +
                                        "�� ������� � ����� Operational �� �������� �����");
                                    // ���������� �� ������� �� ��������� Pre-Operational 
                                    // �� �������� �����. ������������� ������ ��������� �������
                                    device.CurrentTransaction.Abort(null, msg);
                                }
                                break;
                            }
                        case TransactionStatus.NotInitialized:
                        case TransactionStatus.Completed:
                            {
                                // ���������� ������� � ��������� Operational - ���������� ���������
                                // ������, �� �����-�� ������� ���������� ������� � ��������� Pre-Operational
                                // ��������� ���������� � ��������� Operational
                                request = new Frame();
                                request.Identifier = 0;
                                request.FrameFormat = FrameFormat.StandardFrame;
                                request.FrameType = FrameType.DATAFRAME;
                                request.Data = new Byte[] { 
                                    (Byte)StatusCode.Operational, // ���� �������
                                    device.Device.NodeId // ���� Node ID
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
                                // ��������� ���������� ��������� ������� � ���� ���
                                // �������� ������� �� �������� ���������� ������ ��������
                                if (device.ErrorCount < TotalAttempts)
                                {
                                    // ��������� ������
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
                                    "Network {0}: ServiceNmt - ��������� ������ ������� {1} �� �������������� " +
                                    "� ������ ������ ��", _NetworkController.NetworkName,
                                    device.CurrentTransaction.Status);
                                throw new Exception(msg);
                            }
                    }
                }
                else
                {
                    // ���������� ������� ��� �������� ���������� � ��������� Operational
                    request = new Frame();
                    request.Identifier = 0;
                    request.FrameFormat = FrameFormat.StandardFrame;
                    request.FrameType = FrameType.DATAFRAME;
                    request.Data = new Byte[] { 
                        (Byte)StatusCode.Operational, // ���� �������
                        device.Device.NodeId // ���� Node ID
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
                // O������� ����� ��������� ����� ������ NodeGuard, 
                // ������� ����������� ��������� ����������. � ������ �� ����� ����������, ���
                // ���������� ������� ����� Operational � ������������� ������ ����������.
                // �� �����, ��������� ��������� ���������� � ���� ���������� �������
                // � ���������� �������� ������, �� ������� ��� ������� ����� �� ����������
                // � ��������� ����������.
                if ((device.CurrentTransaction != null) && 
                    (device.CurrentTransaction.IsRunning))
                {
                    // �.� ������ �� �������� ������, ������������� ����������
                    // ��� ������ ������� ������ (������������)
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
            // ������ ������ �� ������������ �������� ���������
            return;
        }
        
        #endregion
    }
}
