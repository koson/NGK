using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.DataLinkLayer.Message;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;

namespace NGK.CAN.DataLinkLayer.CanPort.VirtualPort
{
    [Serializable]
    [Description("Виртуальный CAN-порт")]
    public class VirtualCanPort: ICanPort
    {
        #region Constructors

        public VirtualCanPort()
        {
        }
        /// <summary>
        /// Конструктор для десериализации
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public VirtualCanPort(SerializationInfo info, StreamingContext context)
        {
            _PortName = info.GetString("PortName");
            _InputBufferMessages = new Queue<Frame>(100);

            _FrameFormat = (FrameFormat)info.GetValue("FrameFormat", typeof(FrameFormat));
            _Mode = (PortMode)info.GetValue("Mode", typeof(PortMode));
            _BitRate = (BaudRate)info.GetValue("BitRate", typeof(BaudRate));
        }

        #endregion

        #region Fields And Properties
        
        private readonly Version HARDWARE_VERSION = new Version(1, 0);
        private readonly Version SOFTWARE_VERSION = new Version(1, 0);

        private string _PortName = "CAN1";
        private BaudRate _BitRate = BaudRate.BR10;
        private CanPortStatus _PortStatus = CanPortStatus.IsClosed;
        private PortMode _Mode = PortMode.NORMAL;
        [NonSerialized]
        private FrameFormat _FrameFormat = FrameFormat.MixedFrame;
        [NonSerialized]
        private Queue<Frame> _InputBufferMessages = new Queue<Frame>(100);

        public string PortName
        {
            get { return _PortName; }
            set 
            {
                _PortName = value;
            }
        }

        public BaudRate BitRate
        {
            get { return _BitRate; }
            set { _BitRate = value; }
        }

        public string HardwareType
        {
            get { return "Virtual CAN Adapter"; }
        }

        public string Manufacturer
        {
            get { return "NGK"; }
        }

        [XmlIgnore]
        public Version HardwareVersion
        {
            get { return HARDWARE_VERSION; }
        }

        [XmlIgnore]
        public Version SoftwareVersion
        {
            get { return SOFTWARE_VERSION; }
        }

        public bool IsOpen
        {
            get { return _PortStatus != CanPortStatus.IsClosed; }
        }

        public CanPortStatus PortStatus
        {
            get 
            {
                return _PortStatus; 
            }
            private set
            {
                Debug.WriteLine(String.Format("CAN-порт принял новое состояние: {0}", value), "Info");
                _PortStatus = value;
            }
        }

        public PortMode Mode
        {
            get { return _Mode; }
            set 
            { 
                switch (value)
                {
                    //case PortMode.LISTEN_ONLY:
                    case PortMode.NORMAL:
                        {
                            _Mode = value;
                            break;
                        }
                    default:
                        {
                            String msg;
                            msg = String.Format(
                                "Недопустимая операция. Режим работы порта {0} не поддерживается данной аппаратурой", value.ToString());
                            throw new ArgumentException(msg, "Mode");
                        }
                }
            }
        }

        public FrameFormat FrameFormat
        {
            get { return _FrameFormat; }
            set { _FrameFormat = value; }
        }

        [XmlIgnore]
        public bool ErrorFrameEnable
        {
            get
            {
                throw new NotSupportedException("Данный параметр не поддерживается устройством");
            }
            set
            {
                throw new NotSupportedException("Данный параметр не поддерживается устройством");
            }
        }

        public int MessagesToRead
        {
            get { return _InputBufferMessages.Count; }
        }

        #endregion

        #region Methods

        public void Open()
        {
            if (PortStatus != CanPortStatus.IsClosed)
                return;

            PortStatus = CanPortStatus.IsPassive;
            Start();
        }

        public void Close()
        {
            PortStatus = CanPortStatus.IsClosed;
        }

        public void Reset()
        {
            if (PortStatus != CanPortStatus.IsClosed)
                throw new InvalidOperationException("Невозможно выполнить операцию. Порт закрыт");

            PortStatus = CanPortStatus.IsPassiveAfterReset;
            Start();
        }

        public void Start()
        {
            if (PortStatus == CanPortStatus.IsClosed)
                throw new InvalidOperationException("Невозможно выполнить операцию. Порт закрыт");

            if (PortStatus == CanPortStatus.IsActive)
                return;

            PortStatus = CanPortStatus.IsActive;
        }

        public void Stop()
        {
            if (PortStatus == CanPortStatus.IsClosed)
                throw new InvalidOperationException("Невозможно выполнить операцию. Порт закрыт");

            PortStatus = CanPortStatus.IsPassive;
        }

        public void WriteMessage(uint identifier, FrameType frameType, 
            FrameFormat frameFormat, byte[] data)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Write(Frame message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Read(out Frame message)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Frame[] ReadMessages(int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Frame[] ReadMessages()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Dispose() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PortName", PortName);
            // Инициализируем свойства порта
            info.AddValue("FrameFormat", _FrameFormat);
            info.AddValue("Mode", _Mode);
            info.AddValue("BitRate", _BitRate);
        }

        #endregion

        #region Events

        public event EventHandler MessageReceived;

        public event EventHandlerErrorRecived ErrorReceived;

        public event EventHandlerPortChangesStatus PortChangedStatus;

        #endregion
    }
}
