using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using NGK.CAN.DataLinkLayer.Message;
using Common.Controlling;
using Infrastructure.LogManager;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// ������� ����� �������� ������� ��������� CAN ���-���. �� ���� ����� �������������
    /// ��� ������� ������� ������� ���������
    /// </summary>
    public abstract class Service : IManageable
    {
        #region Fields And Properties
        /// <summary>
        /// ���������� ��������� ������
        /// </summary>
        protected ILogManager Logger
        {
            get 
            {
                return _LogEnabled ? NLogManager.Instance : null; 
            } 
        }
        private bool _LogEnabled = false;
        /// <summary>
        /// ���������/��������� ����������� 
        /// </summary>
        protected virtual bool LogEnabled
        {
            get { return _LogEnabled; }
            set { _LogEnabled = value; }
        }
        /// <summary>
        /// ���������� ������������ �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("�������")]
        [DisplayName("������������ �������")]
        [Description("������������ �������� ������� CAN ���-���")]
        public abstract ServiceType ServiceType
        {
            get;
        }
        /// <summary>
        /// ������� ��������� �������
        /// </summary>
        protected Status _Status;
        /// <summary>
        /// ���������� ������ �������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("��������� �������")]
        [Description("��������� �������� ������� CAN ���-���")]
        public Status Status
        {
            get { return _Status; }
            set
            {
                String msg;

                switch (value)
                {
                    case Status.Stopped:
                        {
                            this.Stop();
                            break;
                        }
                    case Status.Running:
                        {
                            // ��������� ������
                            this.Start();
                            break;
                        }
                    case Status.Paused:
                        {
                            // ���������������� ������ �������
                            //this.Suspend();
                            //break;
                            msg = String.Format("Network {0}: Service {1}: ������� ���������� �������� " +
                                "Status �������� ���������������� � ������ ������ �� - {1}",
                                this._NetworkController.NetworkName, this.ServiceType.ToString(),
                                value.ToString());
                            throw new NotSupportedException(msg);
                        }
                    default:
                        {
                            msg = String.Format("Network {0}: Service {1}: ������� ���������� �������� " +
                                "Status �������� ���������������� � ������ ������ �� - {2}",
                                this._NetworkController.NetworkName, this.ServiceType.ToString(),
                                value.ToString());
                            throw new Exception(msg);
                        }
                }
            }
        }
        /// <summary>
        /// ������ �� ���������� ��������, �������� ����������� ������ ������
        /// </summary>
        protected ICanNetworkController _NetworkController;
        /// <summary>
        /// ��������������� ��� ��������������� �������
        /// </summary>
        internal ICanNetworkController NetworkController
        {
            get { return _NetworkController; }
            set
            {
                if ((_NetworkController == null) ||
                    (_NetworkController.Equals(value)) ||
                    (value == null))
                {
                    _NetworkController = value;
                }
                else
                {
                    throw new ArgumentException(
                        "������� ���������� ������������ ��������",
                        "NetworkController");
                }
            }
        }
        /// <summary>
        /// ����������� ���������� ������� ������� � ��������� ����������, ������ ���
        /// ������ �������� ���������� � ��������� ������.
        /// </summary>
        protected Int32 _TotalAttempts;
        /// <summary>
        /// ����������� ���������� ������� ������� � ��������� ����������, ������ ���
        /// ������ �������� ���������� � ��������� ������.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("���������� ������� ������� � ����������")]
        [Description("���������� ��������� ������� ������� � ����������, " +
            "����� ���� ���������� ��������� � ��������� �������������.")]
        public Int32 TotalAttempts
        {
            get { return this._TotalAttempts; }
            set
            {
                String msg;

                if (value < 1)
                {
                    msg = String.Format(
                        "Network {0}: Service {1}: ������� ���������� �������� ������������ �������� ������ 1",
                        this._NetworkController.NetworkName, this.ServiceType.ToString());
                    throw new ArgumentOutOfRangeException("TotalAttempts", msg);
                }
                else
                {
                    this._TotalAttempts = value;
                }
            }
        }
        /// <summary>
        /// �������� �������
        /// </summary>
        protected Context _Context;
        /// <summary>
        /// ����� ��� ������������ ������� � ��������.
        /// </summary>
        protected static Object _SyncRoot = new Object();

        #endregion

        #region Constructors
        /// <summary>
        /// ���������� �� ��������� ��������
        /// </summary>
        private Service()
        {
            throw new NotImplementedException(
                "������� ������ ������������ ������������ �� ��������� NetworkService()");
        }
        /// <summary>
        /// ���������� �������� ������
        /// </summary>
        /// <param name="controller">���������� ����</param>
        protected Service(ICanNetworkController controller)
        {
            String msg;

            if (controller == null)
            {
                msg = String.Format("������� ������� ������� ������ {0}, " +
                    "��� ��������������� ����������� ����", ServiceType);
                throw new NullReferenceException(msg);
            }

            _TotalAttempts = 1;
            _Status = Status.Stopped;
            _NetworkController = controller;
        }

        #endregion

        #region Methods
        /// <summary>
        /// ������� ��������� �� ����������� ����
        /// </summary>
        /// <param name="messages"></param>
        public virtual void HandleIncomingMessages(Frame[] messages)
        {
            return;
        }
        /// <summary>
        /// ����� ���������� ������������ ���� � ��������� 
        /// ������� ���������� (������ � ����������)
        /// </summary>
        public virtual void HandleOutcomingMessages()
        {
            return;
        }
        /// <summary>
        /// �������� ������ �������� �������
        /// </summary>
        public virtual void Start()
        {
            String msg;

            if (this._NetworkController != null)
            {
                if (this._NetworkController.Status == Status.Running)
                {
                    switch (this.Status)
                    {
                        case Status.Running:
                            {
                                // ������ �� ������ ������ ��� �������
                                break;
                            }
                        case Status.Stopped:
                            {
                                // ��������� ������
                                lock (_SyncRoot)
                                {
                                    this._Status = Status.Running;
                                }

                                // ���������� �������
                                this.OnStatusChanged();

                                if (Logger != null)
                                {
                                    msg = String.Format("Network {0}: Sevrice {1}: ������ ��� �������",
                                        this._NetworkController.NetworkName, this.ServiceType);
                                    Logger.Info(msg);
                                }

                                break;
                            }
                        case Status.Paused:
                            {
                                msg = String.Format(
                                    "{0}: Network {1}: Service {2}: Start() ��������� ��������� ������� ���������������� � ������ ������ �� - {3}",
                                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")), this._NetworkController.NetworkName,
                                    this.ServiceType.ToString(), this.Status.ToString());
                                throw new NotImplementedException(msg);
                            }
                        default:
                            {
                                msg = String.Format(
                                    "{0}: Network {1}: Service {2}: Start() ��������� ��������� ������� ���������������� � ������ ������ �� - {3}",
                                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")), this._NetworkController.NetworkName,
                                    this.ServiceType.ToString(), this.Status.ToString());
                                throw new NotImplementedException(msg);
                            }
                    }
                }
                else
                {
                    msg = String.Format(
                        "{0}: Network {1}: {2}.Start(): ����������� ��������� ������ �������. ���������� ���� �� �������",
                        DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)),
                        this._NetworkController.NetworkName, this.ServiceType.ToString());
                    throw new InvalidOperationException(msg);
                }
            }
            else
            {
                msg = String.Format(
                    "{0}: Network null: {1}.Start(): ���������� ��������� ������ �������. ����������� (null) ���������� ����",
                    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), this.ServiceType.ToString());
                throw new InvalidOperationException(msg);
            }
            return;
        }
        /// <summary>
        /// ������������� ������ �������
        /// </summary>
        public virtual void Stop()
        {
            String msg;

            if (_Status != Status.Stopped)
            {
                _Status = Status.Stopped;
                // ���������� �������
                this.OnStatusChanged();

                msg = String.Format("Network {0}: Service {1}: ������ ��� ����������",
                    this._NetworkController.NetworkName, this.ServiceType);
                //Logger.Info(msg);
                return;
            }
            
            return;
        }
        /// <summary>
        /// ���������������� ������ �������
        /// </summary>
        public virtual void Suspend()
        {
            string msg;
            msg = String.Format(
                "{0}: Network {1}: Service {2}: ����� ������� ���������������� � ������ ������ ��",
                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru")), 
                _NetworkController.NetworkName, ServiceType);
            throw new NotImplementedException(msg);
        }
        /// <summary>
        /// ����������� ������� ������������ ��������
        /// </summary>
        public virtual void Dispose()
        {
            Stop();
            return;
        }
        /// <summary>
        /// ����� ���������� ������� ��������� ��������� �������� �������
        /// </summary>
        protected void OnStatusChanged()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = this.ServiceChangedStatus;

            if (handler != null)
            {
                foreach (EventHandler SingleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = SingleCast.Target as ISynchronizeInvoke;

                    try
                    {
                        if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        {
                            syncInvoke.Invoke(SingleCast, new Object[] { this, args });
                        }
                        else
                        {
                            SingleCast(this, args);
                        }
                    }
                    catch
                    { throw; }
                }
            }

            //String traceMessage = String.Format(
            //    "���� {0}: ������ {1}: ������ ����� ��������� {2}",
            //    this._NetworkController.NetworkName,
            //    this.ServiceName.ToString(), this._Status.ToString());
            return;
        }

        #endregion

        #region Events
        /// <summary>
        /// ������� ���������� ��� ��������� ��������� �������� �������
        /// </summary>
        public event EventHandler ServiceChangedStatus;
        /// <summary>
        /// 
        /// </summary>
        event EventHandler IManageable.StatusWasChanged
        {
            add
            {
                lock (_SyncRoot)
                {
                    this.ServiceChangedStatus += value;
                }
            }
            remove
            {
                lock (_SyncRoot)
                {
                    this.ServiceChangedStatus -= value;
                }
            }
        }

        #endregion
    }
}
