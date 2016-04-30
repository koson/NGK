using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NLog;
using NGK.CAN.ApplicationLayer.Transactions;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataTypes.DateTimeConvertor;
using Common.Controlling;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ServicePdoReceive: Service
    {
        #region Fields And Properties

        protected override Logger Logger
        {
            get { return null; }
        }

        public override ServiceType ServiceType
        {
            get
            {
                return ServiceType.PdoReceive;
            }
        }
        /// <summary>
        /// ������ ������������� ������� � �������� ����������� ����, ���
        /// </summary>
        private Int32 _SynchronizationPeriod;
        /// <summary>
        /// ������ ������� ����� ������� ���������� ������������� ������� � ����, ���
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("������ ������������� ������� ���������, ���")]
        [Description("������ ������� ����� ������� ���������� ������������� " +
            "������� � �������� ����������� ���� ")]
        public Int32 Interval
        {
            get
            {
                return _SynchronizationPeriod;
            }
            set
            {
                if (base._Status == Status.Stopped)
                {
                    if (value > 0)
                    {
                        this._SynchronizationPeriod = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Interval", String.Format(
                            "������� ���������� ������������ �������� ������� ������������� �������. " +
                            "������� {0}, ��������� ������ ������ ����",
                            value.ToString()));
                    }
                }
            }
        }
        /// <summary>
        /// ����� � ���� ��������� ������������� ������� � ����
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������")]
        [DisplayName("���� � ����� ������������� �������")]
        [Description("���� � ����� ��������� ������������� ������� � ����")]
        public DateTime LastTimeSynchronisation
        {
            get
            {
                return _LastTransaction == null ? new DateTime() : 
                    _LastTransaction.TimeOfEnd;
            }
        }
        /// <summary>
        /// ������ ���������� ����������� �������� ������. ��� ���������� ��� 
        /// ������������ ���������� ������� ������������ ���� 
        /// </summary>
        private Transaction _LastTransaction;

        #endregion
        
        #region Constructors
        /// <summary>
        /// ����������� �� ���������
        /// </summary>
        private ServicePdoReceive()
            : base(null)
        {
            throw new NotImplementedException(
                "������� ������� ����������� ����������� ������ ServicePdoReceive");
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="controller">���������� ����</param>
        /// <param name="periodOfTimeSync">
        /// ������ ������������� ������� � �������� ����������� ����, ���
        /// </param>
        public ServicePdoReceive(ICanNetworkController controller, 
            Int32 periodOfTimeSync)
            : base(controller)
        {
            if (periodOfTimeSync > 0)
            {
                _SynchronizationPeriod = periodOfTimeSync;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Interval", String.Format(
                    "������� ���������� ������������ �������� ������� ������������� �������. " +
                    "������� {0}, ��������� ������ ������ ����",
                    periodOfTimeSync.ToString()));
            }
        }
        /// <summary>
        /// ����������� ��� ������������
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //public ServicePdoReceive(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //    _SynchronizationPeriod = info.GetInt32("Interval");
        //    _LastTimeSynchronisation = DateTime.Now;
        //}
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        public override void HandleOutcomingMessages()
        {
            //String msg;
            Frame message;

            if (Status != Status.Running)
            {
                return;
            }

            // 1. ���� ��� � ������ ������ (����� ����o��) ���������� ��������� ����������. 
            // �������� ����� ���������� ������ ����������. ���������� � ���� ����� �������� ������������� �������
            // � ���������� � ������� ��������. ���� ���������� ����� ������ ��������, �� ��������� �������������.

            if (_LastTransaction != null)
            {
                TimeSpan interval = DateTime.Now - _LastTransaction.TimeOfEnd;

                if (interval.Seconds < Interval)
                {
                    return;
                }
            }
            
            _LastTransaction = new Transaction();
            
            // ��������� � ���������� ������ � �������� �����
            message = new Frame();
            message.Identifier = 0x200;
            message.FrameFormat = FrameFormat.StandardFrame;
            message.FrameType = FrameType.DATAFRAME;
            message.Data = new Byte[4];
            // �������� ������� ����� � ������� UNIX
            UInt32 unixTime = Unix.ToUnixTime(DateTime.Now);
            unchecked
            {
                message.Data[0] = (Byte)unixTime; // ������� ����
                message.Data[1] = (Byte)(unixTime >> 8);
                message.Data[2] = (Byte)(unixTime >> 16);
                message.Data[3] = (Byte)(unixTime >> 24); // ������� ����
            }
            
            _LastTransaction.Start(TransactionType.BroadcastMode, message);
            _NetworkController.SendMessageToCanPort(_LastTransaction.Request.Value);
            _LastTransaction.Stop(null);

            //Debug.WriteLine(String.Format("����� ��������� ������������� �������: {0}",
            //    now.ToString(new System.Globalization.CultureInfo("ru-Ru", false))));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        //public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info,
        //    System.Runtime.Serialization.StreamingContext context)
        //{
        //    info.AddValue("Interval", this._PeriodOfTimeSynchronization);
        //    base.GetObjectData(info, context);
        //    return;
        //}

        #endregion
    }
}
