using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataLinkLayer.CanPort;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Convert;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351
{
    /// <summary>
    /// ����� ��������� ���������� ������ � ������ CAN ���������� Faswel NIM-351
    /// </summary>
    [Serializable]
    [Description("CAN-���� Fastwel NIM-351")]
    public class CanPort: CanPortBase //ICanPort
    {
        #region Fields And Properties
        /// <summary>
        /// ������ ���������� ���������� CAN
        /// </summary>
        [NonSerialized]
        private SafeFileHandle _DeviceHandle;
        /// <summary>
        /// ������������ �����. ��������� � ���� ������ � ������� [CAN][����� �����: int]
        /// </summary>
        private String _PortName;
        /// <summary>
        /// �������� �������� ������.
        /// </summary>
        private BaudRate _BitRate = BaudRate.BR10;
        /// <summary>
        /// ����� ������ �����
        /// </summary>
        private PortMode _OpMode = PortMode.NORMAL;
        /// <summary>
        /// ������ ������ (���� ID: 11 ��� 29 ���) � �������� �������� ����
        /// ����� ����������� �� "���" ��� ������ MIXED
        /// </summary>
        private FrameFormat _FrameFormat = FrameFormat.MixedFrame;
        /// <summary>
        /// ��������� ���� ��������� Error Frame
        /// </summary>
        private Boolean _ErrorFrameEnable = true;
        /// <summary>
        /// ������ ��������� ����������� �� �������� ������
        /// </summary>
        private F_CAN_STATE _PortStatus = F_CAN_STATE.CAN_STATE_STOPPED;
        /// <summary>
        /// ��������� ��������� CAN-��������
        /// </summary>
        /// <remarks>��� ���� ���������������� �������� ����������</remarks>
        private F_CAN_TIMEOUTS _Timeouts;
        /// <summary>
        /// �������� �������� (����) ��� ������ ������
        /// </summary>
        /// <remarks>
        /// ������������ ��� ������ ������� Api.fw_can_recv(...)
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("������� ������")]
        [Description("�������� �������� � ����. ���������� �������� ��������, ������������� �������� " +
            "fw_can_recv() ��� ������ ������")]
        [DefaultValue(0)]
        public UInt32 ReadTotalTimeout
        {
            get
            {
                return _Timeouts.ReadTotalTimeout;
            }
            set
            {
                this._Timeouts.ReadTotalTimeout = value;
            }
        }
        /// <summary>
        /// ���������� ��������� � WriteTotalTimeoutMultiplier �������� �������� 
        /// ��� �������� ������ �������� Api.fw_can_send �� �������
        /// Tsend = N * WriteTotalTimeoutMultiplier + WriteTotalTimeoutConstant (����)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("WriteTotalTimeoutConstant")]
        [Description("������������ ��� ���������� ������� ��� ������ fw_can_send(). " +
            "����� �������� ��. ������������ �� ��������� ������ F_CAN_TIMEOUTS")]
        [DefaultValue(0)]        
        public UInt32 WriteTotalTimeoutConstant
        {
            get
            {
                return this._Timeouts.WriteTotalTimeoutConstant;
            }
            set
            {
                this._Timeouts.WriteTotalTimeoutConstant = value;
            }
        }
        /// <summary>
        /// ���������� ��������� � WriteTotalTimeoutConstant �������� �������� 
        /// ��� �������� ������ �������� Api.fw_can_send �� �������
        /// Tsend = N * WriteTotalTimeoutMultiplier + WriteTotalTimeoutConstant (����)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("WriteTotalTimeoutMultiplier")]
        [Description("������������ ��� ���������� ������� ��� ������ fw_can_send(). " +
            "����� �������� ��. ������������ �� ��������� ������ F_CAN_TIMEOUTS")]
        [DefaultValue(0)]  
        public UInt32 WriteTotalTimeoutMultiplier
        {
            get
            {
                return this._Timeouts.WriteTotalTimeoutMultiplier;
            }
            set
            {
                this._Timeouts.WriteTotalTimeoutMultiplier = value;
            }
        }
        /// <summary>
        /// ������� (����) ������������ ��������� ��� ��������������� ��������������
        /// �� ��������� CAN_STATE_BUS_OFF. ���� ������� ����� 0, �� ��������������
        /// �� ������������, � ��������� ������ �� ��������� ���������� ���������
        /// ����� ��������� ���������� ����� ��������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("RestartBusoffTimeout")]
        [Description("������� (����) ������������ ��������� ��� ��������������� �������������� " +
            "�� ��������� CAN_STATE_BUS_OFF. ���� ������� ����� 0, �� �������������� �� ������������," +
            "� ��������� ������ �� ��������� ���������� ��������� ����� ��������� ���������� ����� ��������")]
        public UInt32 RestartBusoffTimeout
        {
            get
            {
                return _Timeouts.RestartBusoffTimeout;
            }
            set
            {
                this._Timeouts.RestartBusoffTimeout = value;
            }
        }
        /// <summary>
        /// ���������� ��������� �������� ��������� ��������
        /// </summary>
        private F_CAN_STATS Statistics
        {
            get { return this.GetStatistics(); }
        }
        /// <summary>
        /// ����� ��� ��������� ������� �������� ���������
        /// </summary>
        [NonSerialized]
        private Thread _ThreadForInput;
        /// <summary>
        /// ������� ����� ���������
        /// </summary>
        [NonSerialized]
        private Queue<Frame> _InputBufferMessages;
        /// <summary>
        /// Quit flag for the receive thread.
        /// </summary>
        [NonSerialized]
        private int _FlagMustQuit = 0;
        /// <summary>
        /// ����� ��� ������������ ������� � ��������.
        /// </summary>
        [NonSerialized]
        protected static Object _SyncRoot = new Object();

        #endregion
        
        #region Constructors
        
        /// <summary>
        /// �����������
        /// </summary>
        public CanPort()
        {
            // �������������� ����� �������� ���������
            this._InputBufferMessages = new Queue<Frame>(100);

            // �������������� ���������� ����������
            this._DeviceHandle = new SafeFileHandle(IntPtr.Zero, true);
            this._DeviceHandle.Close();

            // ��������� ���������
            _Timeouts = new F_CAN_TIMEOUTS();
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="portNumber">����� CAN-�����</param>
        public CanPort(Int32 portNumber)
        {
            // �������������� ����� �������� ���������
            this._InputBufferMessages = new Queue<Frame>(100);

            // �������������� ���������� ����������
            this._DeviceHandle = new SafeFileHandle(IntPtr.Zero, true);
            this._DeviceHandle.Close();

            // ����� �����
            if (portNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "portNumber", 
                    "������������ ��������. ����������� ������ ������������� ��������, ������ ����");
            }
            else
            {
                this.PortName = String.Format("CAN{0}", portNumber);
            }

            // ��������� ���������
            _Timeouts = new F_CAN_TIMEOUTS();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="bitRate"></param>
        /// <param name="frameFormat"></param>
        /// <param name="mode"></param>
        public CanPort(String portName, BaudRate bitRate, FrameFormat frameFormat,
            PortMode mode)
        {
            // �������������� ����� �������� ���������
            this._InputBufferMessages = new Queue<Frame>(100);

            // �������������� ���������� ����������
            this._DeviceHandle = new SafeFileHandle(IntPtr.Zero, true);
            this._DeviceHandle.Close();

            // ����� �����
            PortName = portName;
            _BitRate = bitRate;
            _FrameFormat = frameFormat;
            _OpMode = mode;

            // ��������� ���������
            _Timeouts = new F_CAN_TIMEOUTS();
        }
        /// <summary>
        /// ����������� ��� ��������������
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public CanPort(SerializationInfo info, StreamingContext context)
        {
            this._InputBufferMessages = new Queue<Frame>(100);

            // �������������� ���������� ����������
            this._DeviceHandle = new SafeFileHandle(IntPtr.Zero, true);
            this._DeviceHandle.Close();

            this._PortName = info.GetString("PortName");
            this._BitRate = (BaudRate)info.GetValue("BitRate", typeof(BaudRate));
            this._OpMode = (PortMode)info.GetValue("Mode", typeof(PortMode));
            this._ErrorFrameEnable = info.GetBoolean("ErrorFrameEnable");
            this._FrameFormat = (FrameFormat)info.GetValue("FrameFormat", typeof(FrameFormat));
            this._Timeouts = (F_CAN_TIMEOUTS)info.GetValue("Timeouts", typeof(F_CAN_TIMEOUTS));
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// ������ ��������� CAN-��������
        /// </summary>
        /// <returns>������� ��������� CAN-��������</returns>
        private F_CAN_STATE GetPortStatus()
        {
            F_CAN_RESULT result;
            F_CAN_STATE state;
            String msg;

            if ((this._DeviceHandle.IsClosed) || (_DeviceHandle.IsInvalid))
            {
                return F_CAN_STATE.CAN_STATE_STOPPED;
            }
            else
            {
                result = Api.fw_can_get_controller_state(this._DeviceHandle, out state);

                if (Api.f_can_success(result))
                { return state; }
                else
                {
                    msg = String.Format(
                        "��������� ��������� ��������� CAN-��������. ������� ������: {0}",
                        result.ToString());
                    throw new InvalidOperationException(msg);
                }
            }
        }
        /// <summary>
        /// ������ ������� ������ �������� � ���������� � ����������. ���� 
        /// ���������� ���������� ������� � ��������� ����� ������
        /// </summary>
        /// <returns>true - ���� ������ ���������</returns>
        private Boolean UpdatePortStatus()
        {
            F_CAN_STATE newStatus;
            Boolean result;

            if (this.IsOpen)
            {
                newStatus = this.GetPortStatus();

                if (this._PortStatus != newStatus)
                {
                    // ������ ���������.
                    this._PortStatus = newStatus;
                    this.OnPortChangesStatus(ConvertNim351.ConvertToCanPortStatus(this._PortStatus));
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                // ���� ���� ������, ���� ������ ������ ���� F_CAN_STATE.CAN_STATE_STOPPED.
                // ���� ��� �� ���, �� ������������� � ���������� �������
                if (this._PortStatus == F_CAN_STATE.CAN_STATE_STOPPED)
                {
                    result = false;
                }
                else
                {
                    // ������ ���������.
                    this._PortStatus = F_CAN_STATE.CAN_STATE_STOPPED;
                    this.OnPortChangesStatus(ConvertNim351.ConvertToCanPortStatus(this._PortStatus));
                    result = true;
                }
            }

            return result;
        }
        /// <summary>
        /// ����� ��� ������ �����. ����������� �� ��������� ������. ����� �� ������ ����������� ��� �������� ����� 
        /// � ����������� ��� ��������.
        /// </summary>
        private void HandleQueueIncomingMessages()
        {
            String msg;
            F_CAN_RESULT result;
            F_CAN_RX buffer;
            F_CAN_WAIT wait;
            UInt32 timeout = 500; // ����
            Frame message;
            //uint count;

            // ���� ���� ������, ������ ��� ������ �� ��������
            //if (this.IsOpen == false)
            //{
            //    msg = "���������� ��������� ������� ���������, CAN-���� ������";
            //    throw new InvalidOperationException(msg);
            //}

            // ������ ���� �� ����� ���� ������
            while (this._FlagMustQuit > 0)
            {
                wait.waitMask = F_CAN_STATUS.CAN_STATUS_EMPTY | F_CAN_STATUS.CAN_STATUS_TXBUF;
                wait.status = F_CAN_STATUS.CAN_STATUS_EMPTY;

                result = Api.fw_can_wait(this._DeviceHandle, ref wait, timeout);

                if ((result == F_CAN_RESULT.CAN_RES_OK) ||
                    (result == F_CAN_RESULT.CAN_RES_TIMEOUT))
                {
                    if (result == F_CAN_RESULT.CAN_RES_TIMEOUT)
                    {
                        //Console.WriteLine("�������� ������� ��������");
                        // ��������� ���-�� ���������, ����� ������������� � ���-�� ������
                        // ��������� ������� ������
                        //if (wait.status == wait.waitMask)
                        //{ }
                        // ������������� ����
                        this.Stop();
                        // ���������� ������� 
                        OnErrorReceived(ERROR.Other);
                        // ��������� ����
                        this.Close();
                        // ��������� ������ ������
                        Interlocked.Exchange(ref this._FlagMustQuit, 0);
                        continue;
                    }

                    // ����  ���� ����������, �� ���� �������� ���������. ������ ��
                    // � �������� � ����� ��������� ���������
                    if (F_CAN_STATUS.CAN_STATUS_RXBUF == 
                        (wait.status & F_CAN_STATUS.CAN_STATUS_RXBUF))
                    {
                        // ������ ����� ��������
                        result = Api.fw_can_peek_message(_DeviceHandle, out buffer);
                        // ��������� �������� ��������� � ��������� �������
                        message = new Frame();
                        message = ConvertNim351.ConvertToFrame(buffer.msg);
                        // �������� �������� ��������� � ����� ������� ��������� � ���������� ������� �����
                        lock (_SyncRoot)
                        {
                            this._InputBufferMessages.Enqueue(message);
                        }
                        this.OnMessageReceived();
                    }

                    // ��������� ������ ������ � � ���������� 
                    if (F_CAN_STATUS.CAN_STATUS_ERR == 
                        (wait.status & F_CAN_STATUS.CAN_STATUS_ERR))
                    {
                        F_CAN_ERRORS errors;
                        result = Api.fw_can_get_clear_errors(this._DeviceHandle, out errors);

                        if (Api.f_can_success(result))
                        {
                            // ����������� �������� � ���������� �������
                            this.OnErrorReceived(ERROR.Other);
                        }
                        else
                        {
                            msg = String.Format(
                                "��� ������ � ������ �������� ������ CAN-�������� �������� ������: {0}", result);
                            throw new InvalidOperationException(msg);
                        }
                    }
                }
                else
                {
                    msg = String.Format("��� ���������� ������� fw_can_wait ��������� ������: {0}", 
                        Enum.GetName(typeof(F_CAN_RESULT), result));
                    throw new InvalidOperationException(msg);
                }
            }
            return;
        }
        /// <summary>
        /// ����� ��� ��������� ������� ����� ���������
        /// </summary>
        private void OnMessageReceived()
        {
            EventHandler handler = this.MessageReceived;

            if (handler != null)
            {
                foreach (EventHandler SingleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = SingleCast.Target as ISynchronizeInvoke;

                    try
                    {
                        if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        {
                            syncInvoke.Invoke(SingleCast, new Object[] { this, new EventArgs() });
                        }
                        else
                        {
                            SingleCast(this, new EventArgs());
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// ���������� ������� �� ��������� ������� �����
        /// </summary>
        /// <param name="status">����� ��������� CAN-�����</param>
        private void OnPortChangesStatus(CanPortStatus status)
        {
            EventArgsPortChangesStatus args = new EventArgsPortChangesStatus(status);
            EventHandlerPortChangesStatus handler = this.PortChangedStatus;

            if (handler != null)
            {
                foreach (EventHandlerPortChangesStatus SingleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = SingleCast.Target as ISynchronizeInvoke;
                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                    {
                        syncInvoke.Invoke(SingleCast, new Object[] { this, args });
                    }
                    else
                    {
                        SingleCast(this, args);
                    }
                }
            }
            return;
        }
        /// <summary>
        /// ���������� ������� ��� ������������� ������ � CAN ��������
        /// </summary>
        private void OnErrorReceived(ERROR error)
        {
            EventHandlerErrorRecived handler = this.ErrorReceived;
            EventArgsLineErrorRecived args = new EventArgsLineErrorRecived(error);   
            
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
                    {
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// ������� ������� ��������� �������� ������ CAN-��������
        /// </summary>
        public void DiscardInBuffer()
        {
            String message;
            F_CAN_RESULT result;

            if (this.IsOpen)
            {
                result = Api.fw_can_purge(_DeviceHandle,
                    F_CAN_PURGE_MASK.CAN_PURGE_RXABORT | F_CAN_PURGE_MASK.CAN_PURGE_RXCLEAR);

                if (result != F_CAN_RESULT.CAN_RES_OK)
                {
                    message = String.Format("�� ������� �������� ������� ����� CAN ��������. Error: ", result);
                    throw new InvalidOperationException(message);
                }
            }
            return;
        }
        /// <summary>
        /// ���������� ��� ��������� CAN �����.
        /// </summary>
        /// <param name="controllerType"></param>
        /// <returns>��������� �������� ������</returns>
        public Boolean GetControllerType(out F_CAN_CONTROLLER controllerType)
        {
            String message;
            F_CAN_RESULT result;
            F_CAN_SETTINGS settings;

            if (this.IsOpen)
            {
                // �������� ������� ���������
                result = Api.fw_can_get_controller_config(_DeviceHandle, out settings);

                if (!Api.f_can_success(result))
                {
                    message = String.Format("������ ��� ������ �������� CAN ��������, Error: {0}", result);
                    throw new InvalidOperationException(message);
                }

                controllerType = settings.controller_type;
                return true;
            }
            else
            {
                controllerType = F_CAN_CONTROLLER.UNKNOWN_CAN_DEVICE;
                return false;
            }
        }
        /// <summary>
        /// ������� � ���������� �������� ������ CAN-��������
        /// </summary>
        /// <returns>�������� ������ CAN-��������</returns>
        public F_CAN_ERRORS GetAndClearErrorCounters()
        {
            String message;
            F_CAN_RESULT result;
            F_CAN_ERRORS errCounters;

            if (this.IsOpen)
            {
                result = Api.fw_can_get_clear_errors(this._DeviceHandle, out errCounters);

                if (!Api.f_can_success(result))
                {
                    message = String.Format(
                        "������ ��� ������� �������� �������� ������ CAN-��������, Error: ",
                        result);
                    throw new InvalidOperationException(message);
                }
            }
            else
            {
                message = "����� �� ����� ���� ��������, ���� ������";
                throw new InvalidOperationException(message);
            }
            return errCounters;
        }
        /// <summary>
        /// ���������� �������������� ���������� CAN-��������
        /// </summary>
        /// <returns>��������� � �����������</returns>
        public F_CAN_STATS GetStatistics()
        {
            String message;
            F_CAN_RESULT result;
            F_CAN_STATS statistics;
            
            if (this.IsOpen)
            {
                result = Api.fw_can_get_stats(_DeviceHandle, out statistics);

                if (!Api.f_can_success(result))
                {
                    message = String.Format("������ ��� ��������� ���������� CAN-��������, Error: ", result);
                    throw new InvalidOperationException(message);
                }
            }
            else
            {
                //message = "����� �� ����� ���� ��������, ���� ������";
                //throw new InvalidOperationException(message);
                statistics = new F_CAN_STATS(); // ���������� ������� ��������
            }
            return statistics;
        }
        /// <summary>
        /// ������� ��������
        /// </summary>
        public void ClearStatistics()
        {
            String msg;
            F_CAN_RESULT result;

            if (this.IsOpen)
            {
                result = Api.fw_can_clear_stats(this._DeviceHandle);
                if (!Api.f_can_success(result))
                {
                    msg = String.Format("������ ��� ���������� ������ �������������� ���������. " +
                        "������� �������: {0}", result);
                    throw new InvalidOperationException(msg);
                }
            }
            return;
        }
        /// <summary>
        /// ������ � ��������� ����� �� �������������� �����
        /// </summary>
        private void CreateAndStartThreadForListenPort()
        {
            //String msg;

            // ������������� ���� ������ ������ �����
            Interlocked.Exchange(ref _FlagMustQuit, Int32.MaxValue);

            if (this._ThreadForInput != null)
            {
                if (this._ThreadForInput.IsAlive)
                {
                    //msg = String.Format(
                    //    "������. ������� ������� ����� �� ������������� CAN-�����." +
                    //    "����� ��� ���������� � ��������");
                    //throw new InvalidOperationException(msg);
                    return;
                }
            }
            
            // ��������� ����� �� ������ �����
            this._ThreadForInput = new Thread(new ThreadStart(this.HandleQueueIncomingMessages));
            this._ThreadForInput.Name = String.Format("Thread_{0}_ForPortReading", this.PortName);
            this._ThreadForInput.Priority = ThreadPriority.Normal;
            this._ThreadForInput.IsBackground = true;
            this._ThreadForInput.Start();
            return;
        }
        /// <summary>
        /// ��������� ���������� ����� �� ������������� CAN-�����
        /// </summary>
        private void StopThreadForListen()
        {
            Interlocked.Exchange(ref _FlagMustQuit, 0);

            if (this._ThreadForInput != null)
            {
                for (int i = 0; i < 1; i++)
                {
                    if (this._ThreadForInput.IsAlive == true)
                    {
                        // ��� ���������� ������
                        Thread.Sleep(500);
                    }
                    else
                    {
                        // ����� ��������
                        break;
                    }
                }
                // ���� ����� �� ���������� � ������ 10 ������, ������ ���� �����.
                // ������� �� ���� �������������� ����������
                if (this._ThreadForInput.IsAlive == true)
                {
                    this._ThreadForInput.Abort();
                    //msg = String.Format(
                    //    "{0}: class CanPort.Close(): ������� ����� �� ���������� �� 0,5 ������� � ��������� � ��������� {1}",
                    //    DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-Ru", false)), 
                    //    this._ThreadForInput.ThreadState.ToString());
                    //Trace.TraceError(msg);
                }
            }
            return;
        }
        #endregion

        #region Events
        public override event EventHandler MessageReceived;
        public override event EventHandlerErrorRecived ErrorReceived;
        public override event EventHandlerPortChangesStatus PortChangedStatus;
        #endregion

        #region ICanPort Members
        /// <summary>
        /// ������������ CAN-�����
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("������������ �����")]
        [Description("������������ ����� � ������� CANx, ��� x - ����� CAN-����� � ������� 1...9")]
        [DefaultValue("CAN1")]
        public override string PortName
        {
            get { return this._PortName; }
            set 
            {
                if ( Api.CheckPortName(value))
                { this._PortName = value; }
                else
                { 
                    throw new ArgumentException(
                        "������������ CAN-����� ������ ���� � ������� CANx, ��� x ����� ����� 1...9"); 
                }
            }
        }
        /// <summary>
        /// �������� ������ � ���� CAN
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("�������� ������")]
        [Description("�������� ������ ������� �� ���� CAN")]
        public override BaudRate BitRate
        {
            get { return this._BitRate; }
            set { this._BitRate = value; }
        }
        /// <summary>
        /// ��� ��������
        /// </summary>
        public override string HardwareType
        { 
            get 
            {
                F_CAN_CONTROLLER controller;
                this.GetControllerType(out controller);
                return "NIM-351";
                //return String.Format("NIM-351; {0}", 
                //Enum.GetName(typeof(F_CAN_CONTROLLER), controller)); 
            } 
        }
        /// <summary>
        /// ���������� ICanPort
        /// </summary>
        public override string Manufacturer
        { get { return "Fastwel"; } }
        /// <summary>
        /// ������ ����������� �����������
        /// </summary>
        public override Version HardwareVersion
        { get { return new Version(0, 0); } }
        /// <summary>
        /// ������ ��
        /// </summary>
        public override Version SoftwareVersion
        { get { return new Version(2, 0); } }
        /// <summary>
        /// ���������� ��������� �������� �����
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("���������")]
        [DisplayName("��������� �����")]
        [Description("���������� �������� ��������� �� �������� ������ �����")]
        public override bool IsOpen
        {
            get
            {
                if (_DeviceHandle != null)
                {
                    if (_DeviceHandle.IsClosed == true)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    // ���� ���������� ����� �� ��������, ������� ��� ���� ������
                    return false;
                }
            }
        }
        /// <summary>
        /// ���������� ��������� �����
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("������ CAN-�����")]
        [Description("���������� ������� ��������� CAN-�����")]
        public override CanPortStatus PortStatus
        {
            get 
            {
                //F_CAN_STATE status = this.GetPortStatus();
                //return ConvertNim351.ConvertToCanPortStatus(status); 
                return ConvertNim351.ConvertToCanPortStatus(this._PortStatus);
            }
        }
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("����� ������")]
        [Description("����� ������ CAN-�����")]
        public override PortMode Mode
        {
            get
            { return this._OpMode; }
            set
            { this._OpMode = value; }
        }
        /// <summary>
        /// ��������� ���� ��������� Error Frame
        /// </summary>
        /// <value>
        /// �������� �� ��������� true 
        /// </value>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("��������� �� �������")]
        [Description("��������/��������� �������� ��������� �� ������� �� �������� ����� � ���������� ������������")]
        [DefaultValue("true")]
        public override Boolean ErrorFrameEnable
        {
            get { return this._ErrorFrameEnable; }
            set { this._ErrorFrameEnable = value; }
        }
        /// <summary>
        /// ��� ������ � �������� �������� CAN �������.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("���������")]
        [DisplayName("������ �����")]
        [Description("������ ����� ��������� �� CAN-����")]
        //[DefaultValue(typeof(FrameFormat), FrameFormat.StandardFrame.ToString())]
        public override FrameFormat FrameFormat
        {
            get
            { return this._FrameFormat; }
            set
            { this._FrameFormat = value; }
        }
        /// <summary>
        /// ���������� �������� ��������� �� ������� ������ ����� 
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("���������")]
        [DisplayName("�������� ���������")]
        [Description("���������� �������� ��������� �� �������� ������ �����")]
        public override int MessagesToRead
        {
            get { return this._InputBufferMessages.Count; }
        }
        /// <summary>
        /// ��������� ������� �����
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public override void Open()
        {
            String msg;
            F_CAN_RESULT result;
            F_CAN_SETTINGS settings;

            settings = new F_CAN_SETTINGS();
            settings.acceptance_code = 0;
            settings.acceptance_mask = 0;
            settings.baud_rate = ConvertNim351.ConvertToF_CAN_BAUDRATE(this.BitRate);
            //settings.error_mask = CAN_OPMODE_ERRFRAME.CAN_ERR_CRTL | CAN_OPMODE_ERRFRAME.CAN_ERR_BUSOFF ;
            settings.error_mask = 0xFFFF;

            settings.opmode = (UInt16)Api.OpModeBuilder(this.Mode, this.FrameFormat, this.ErrorFrameEnable);

            // ������������� ����������
            result = Api.fw_can_init();
            if (!Api.f_can_success(result))
            {
                msg = String.Format("������ ��� ������������ fwcan.dll, Error: {0}", result);
                throw new InvalidOperationException(msg);
            }

            // ��������� ������� CAN � �������� ���������� �����
            result = Api.fw_can_open(Api.GetPortNumber(this.PortName), out this._DeviceHandle);
            if (!Api.f_can_success(result))
            {
                msg = String.Format("������ ��� �������� CAN ��������, Error: {0}", result);
                throw new InvalidOperationException(msg);
            }

            // ������������� ����������
            result = Api.fw_can_set_controller_config(_DeviceHandle, ref settings);
            if (!Api.f_can_success(result))
            {
                msg = String.Format("������ ��� ������ �������� � ����������. Error: {0}", result);
                throw new InvalidOperationException(msg);
            }

            // ������������� ��������
            result = Api.fw_can_set_timeouts(_DeviceHandle, ref _Timeouts);
            if (!Api.f_can_success(result))
            {
                msg = String.Format("������ ��� ��������� ���������. Error: {0}", result);
                throw new InvalidOperationException(msg);
            }

            // ��������� � ��������� ������ �����
            if (!this.UpdatePortStatus())
            {
                msg = String.Format("������. ��� �������� ����� ��� ������ ������� ����������: {0}", 
                    this.PortStatus);
                throw new Exception(msg);
            }

            return;
        }
        /// <summary>
        /// ��������� ���� (����������� ������� �����)
        /// </summary>
        public override void Close()
        {
            String msg;
            F_CAN_RESULT result;
            F_CAN_STATE state;

            // ������� �������� �����:
            // 1. ��������� ��� �� ������ ����. ���� �� ���, �� �������
            // 2. ��������� ��������� ��������. ���� ��������� � ����, ��������.
            // 3. ������������� ����� �� ������������� ����.
            // 4. ��������� (�����������) ������� �����

            // 1. ��������� ��������� �����
            if (!this.IsOpen)
            {
                // ���� ������ �������
                return;
            }

            // 2. ��������� ������ CAN-��������, ���� ������� �������, �� ������������� ���
            state = GetPortStatus();

            if (state != F_CAN_STATE.CAN_STATE_INIT)
            {
                // ������������� �������
                this.Stop();
            }

            // 3. ������������� ����� �� ������ CAN-��������
            //    ��������� ����� �� ������
            this.StopThreadForListen();

            // 4. ��������� ����
            result = Api.fw_can_close(_DeviceHandle);

            if (!Api.f_can_success(result))
            {
                msg = String.Format("�� ������� ������� CAN ��������. Error: ", result);
                throw new InvalidOperationException(msg);
            }

            // ��������� ���� ���������� CAN �������� �� ��������, 
            // ������������� ����������� ���.
            if (this._DeviceHandle.IsClosed == false)
            {
                this._DeviceHandle.Close();
            }
            this._DeviceHandle = null;

            // ��������� � ��������� ������ �����
            if (!this.UpdatePortStatus())
            {
                msg = String.Format("������. ��� �������� ����� ��� ������ ������� ����������: {0}",
                    this.PortStatus);
                throw new Exception(msg);
            }
            return;
        }
        /// <summary>
        /// ���������� ���������� ������ CAN-��������
        /// </summary>
        public override void Reset()
        {
            String message;
            F_CAN_RESULT result;

            if (this.IsOpen)
            {
                result = Api.fw_can_purge(_DeviceHandle, F_CAN_PURGE_MASK.CAN_PURGE_HWRESET | F_CAN_PURGE_MASK.CAN_PURGE_RXABORT
                    | F_CAN_PURGE_MASK.CAN_PURGE_RXCLEAR | F_CAN_PURGE_MASK.CAN_PURGE_TXABORT | F_CAN_PURGE_MASK.CAN_PURGE_TXCLEAR);

                if (!Api.f_can_success(result))
                {
                    message = String.Format("�� ������� ��������� ����� CAN ��������. Error: ", result);
                    throw new Exception(message);
                }
            }

            // ��������� � ��������� ������ �����
            this.UpdatePortStatus();
            return;
        }
        /// <summary>
        /// ��������� ������� � ��������� Active, ���� ���� �����. 
        /// ����� �������� ������������
        /// </summary>
        public override void Start()
        {
            F_CAN_RESULT result;
            String msg;

            if (this.IsOpen)
            {
                // ��������� ������� ��� ������ (��������� � �������� ���������)
                result = Api.fw_can_start(this._DeviceHandle);

                if (!Api.f_can_success(result))
                {
                    msg = String.Format("������ ��� �������� CAN �������� � �������� ���������. Error: {0}", result);
                    throw new InvalidOperationException(msg);
                }

                // ������ ����� �� ������������� ����
                this.CreateAndStartThreadForListenPort();
            }

            // ��������� � ��������� ������ �����
            this.UpdatePortStatus();

            return;
        }
        /// <summary>
        /// ��������� ������� � ��������� CAN_STATE_INIT
        /// </summary>
        public override void Stop()
        {
            F_CAN_RESULT result;
            String msg;

            if (this.IsOpen)
            {
                // ������������� ����� �� ������������� ����
                this.StopThreadForListen();

                // ��������� ���������� � ��������� Init
                result = Api.fw_can_stop(this._DeviceHandle);

                if (!Api.f_can_success(result))
                {
                    msg = String.Format("������ ��� �������� CAN �������� � ��������� Init. Error: {0}", result);
                    throw new InvalidOperationException(msg);
                }
            }

            // ��������� � ��������� ������ �����
            this.UpdatePortStatus();

            return;
        }
        /// <summary>
        /// ���������� ��������� � ����
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="frameType"></param>
        /// <param name="frameFormat"></param>
        /// <param name="data"></param>
        public override void WriteMessage(uint identifier, FrameType frameType, 
            FrameFormat frameFormat, byte[] data)
        {
            Frame frame = new Frame();
            frame.Identifier = identifier;
            frame.FrameType = frameType;
            frame.FrameFormat = FrameFormat;
            if (data != null)
            { frame.Data = data; }
            else
            { frame.Data = new byte[0]; }
            
            this.Write(frame);
            return;
        }        
        /// <summary>
        /// ���������� ��������� � ����
        /// </summary>
        /// <param name="message">��������� ��� �������</param>
        public override void Write(Frame message)
        {
            String msg;
            F_CAN_RESULT result;
            F_CAN_TX buffer;
            F_CAN_STATE status;
            uint count = 0;

            // ���� ���� ������, ������� ��������� �� ��������
            if (!this.IsOpen)
            {
                msg = "���������� ��������� ���������, CAN-���� ������";
                throw new InvalidOperationException(msg);
            }

            status = this.GetPortStatus();
            if (this.PortStatus != CanPortStatus.IsActive)
            {
                msg = "���������� ��������� ���������, CAN-���� �������� �� ���������� ����";
                throw new InvalidOperationException(msg);
            }

            // ��������� ��������� � �������������� ��� ��� ��������
            buffer.msg = ConvertNim351.ConvertToF_CAN_MSG(message);

            result = Api.fw_can_send(this._DeviceHandle, ref buffer, 1, ref count);
            //result = Api.fw_can_post_message(this._DeviceHandle, ref buffer);

            if (!Api.f_can_success(result))
            {
                // ��� �������� ��������� �������� ������
                msg = String.Format("�� ������� ��������� ���������. Error: ", result);
                throw new InvalidOperationException(msg);
            }
            return;
        }

        public override bool Read(out Frame message)
        {
            lock (_SyncRoot)
            {
                if (this._InputBufferMessages.Count > 0)
                {
                    message = this._InputBufferMessages.Dequeue();
                    return true;
                }
                else
                {
                    message = new Frame();
                    return false;
                }
            }
        }

        public override Frame[] ReadMessages(int count)
        {
            List<Frame> messages;

            lock (_SyncRoot)
            {
                messages = new List<Frame>();

                while (_InputBufferMessages.Count != 0)
                {
                    if (count != 0)
                    {
                        messages.Add((Frame)_InputBufferMessages.Dequeue());
                        --count;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return messages.ToArray();
        }

        public override Frame[] ReadMessages()
        {
            List<Frame> messages;

            lock (_SyncRoot)
            {
                messages = new List<Frame>();

                while (_InputBufferMessages.Count != 0)
                {
                    messages.Add((Frame)_InputBufferMessages.Dequeue());
                }
            }

            return messages.ToArray();
        }

        //public event EventHandler MessageReceived;

        //public event EventHandlerErrorRecived ErrorReceived;

        //public event EventHandlerPortChangesStatus PortChangedStatus;

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            if (this.IsOpen)
            { this.Close(); }
            return;
        }

        #endregion

        #region ISerializable Members

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context)
        {
            // �������������� �������� �����
            info.AddValue("PortName", this.PortName);
            info.AddValue("BitRate", this._BitRate);
            info.AddValue("Mode", this._OpMode);
            info.AddValue("ErrorFrameEnable", this._ErrorFrameEnable);
            info.AddValue("FrameFormat", this._FrameFormat);
            info.AddValue("Timeouts", this._Timeouts);
            return;
        }

        #endregion
    }
}
