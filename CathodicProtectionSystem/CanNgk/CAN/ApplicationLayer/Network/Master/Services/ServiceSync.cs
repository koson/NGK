using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.ComponentModel;
using NLog;
using NGK.CAN.ApplicationLayer.Transactions;
using NGK.CAN.DataLinkLayer.Message;
using Common.Controlling;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// ������� ������ Sync 
    /// </summary>
    public sealed class ServiceSync: Service
    {
        #region Fields And Properties

        //private static Logger _Logger = LogManager.GetLogger("SyncLogger");

        protected override Logger Logger
        {
            //get { return _Logger; }
            get { return null; }
        }

        public override ServiceType ServiceType
        {
            get
            {
                return ServiceType.Sync;
            }
        }

        private Timer _TimerSyncMessage;
        /// <summary>
        /// �������� ������� ����� ���������� ������� SYNC, ����
        /// </summary>
        /// <remarks>
        /// ������ ��� ������������� ���� Interval ������� System.Timers.Timer.
        /// MSDN: ����� ����� ��������� Elapsed � �������������. 
        /// �������� ������ ���� ������ ���� � ������ ��� ����� Int32.MaxValue. 
        /// �������� �� ��������� � 100 �����������.
        /// �������: int.MaxValue = 2147483647. ������������ ������ � �������������
        /// => 214783.367 ���. 
        /// => 214783.367 / 60 = 3579.72745 �����
        /// => 3572.72745 / 24 = 149,155 c����
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("������ ������� SYNC, ����")]
        [Description("������ ��������� ��������� SYNC � ����")]
        [DefaultValue(PERIOD_SYNC)]
        public Double PeriodSync
        {
            get
            {
                return this._TimerSyncMessage.Interval;
            }
            set
            {
                if (_Status == Status.Stopped)
                {
                    this._TimerSyncMessage.Interval = value;
                }
            }
        }
        /// <summary>
        /// ������ ��������� SYNC �� ���������
        /// </summary>
        private const Double PERIOD_SYNC = 1000;

        #endregion

        #region Constructors
        /// <summary>
        /// ����������� �� ���������
        /// </summary>
        private ServiceSync()
            : base(null)
        {
            throw new NotImplementedException("������� ������� ����������� ����������� ������ ServiceSync");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controller">���������� ����</param>
        public ServiceSync(INetworkController controller)
            : base(controller)
        {
            this._TimerSyncMessage = new Timer(PERIOD_SYNC);
            this._TimerSyncMessage.AutoReset = true;
            this._TimerSyncMessage.Elapsed += 
                new ElapsedEventHandler(EventHandler_TimerSyncMessage_Elapsed);

        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="controller">���������� ����</param>
        /// <param name="periodSync">������ ���������� ������� Sync, ����</param>
        public ServiceSync(INetworkController controller, Double periodSync)
            : base(controller)
        {
            this._TimerSyncMessage = new Timer(periodSync);
            this._TimerSyncMessage.AutoReset = true;
            this._TimerSyncMessage.Elapsed += 
                new ElapsedEventHandler(EventHandler_TimerSyncMessage_Elapsed);
        }
        /// <summary>
        /// ����������� ��� ������������
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //public ServiceSync(SerializationInfo info, StreamingContext context)
        //    : base(info, context)
        //{
        //    this._TimerSyncMessage = new Timer(info.GetDouble("PeriodSync"));
        //    this._TimerSyncMessage.AutoReset = true;
        //    this._TimerSyncMessage.Elapsed += 
        //        new ElapsedEventHandler(EventHandler_TimerSyncMessage_Elapsed);
        //}

        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        public override void HandleOutcomingMessages()
        {
            // TODO: ���������� �� ������� 
            // ��������� ��������� ����������� ��������
            //TimeSpan result = DateTime.Now - DateTime.Now;
      
            return;
        }
        public override void HandleIncomingMessages(Frame[] messages)
        {
            //������ ������ �� ����� �������� ���������
            return;
        }
        /// <summary>
        /// ���������� ������� ������������� �������.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TimerSyncMessage_Elapsed(object sender,
            ElapsedEventArgs e)
        {
            //String msg;
            Frame message;
            
            // ��������� ��������� SYNC � �������� ��� � ����
            message = new Frame();
            message.Identifier = 0x80;
            message.FrameFormat = FrameFormat.StandardFrame;
            message.FrameType = FrameType.DATAFRAME;
            message.Data = new Byte[0];

            lock (_SyncRoot)
            {
                _NetworkController.SendMessageToCanPort(message);
            }

            // ���������� � ���...
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Start()
        {
            base.Start();
            _TimerSyncMessage.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Stop()
        {
            _TimerSyncMessage.Stop();
            base.Stop();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            _TimerSyncMessage.Stop();
            _TimerSyncMessage.Dispose();
            base.Dispose();
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
        //    info.AddValue("PeriodSync", this._TimerSyncMessage.Interval);
        //    base.GetObjectData(info, context);
        //    return;
        //}
        #endregion
    }
}
