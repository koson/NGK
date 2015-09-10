using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
//
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using Modbus.OSIModel.ApplicationLayer.Slave.NetworkAPI;
using Modbus.OSIModel.Message;
//
using Common.Controlling;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    //===============================================================================
    /// <summary>
    /// ����� ��� ���������� Slave - ���������� ���� modbus
    /// </summary>
    [Serializable]
    public class Device: INetworkFunctions, IManageable
    {
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ���������� ��� ������ �������� ��������� �� ����� � �������
        /// � ����� 0x14
        /// </summary>
        public struct Code0x14SubRequest
        {
            public Byte ReferenceType;
            public UInt16 FileNumber;
            public UInt16 RecordNumber;
            public UInt16 RecordLength;
        }
        //---------------------------------------------------------------------------
        #region Fields and Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ����� ����������
        /// </summary>
        private Byte _Address;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ����� ����������
        /// </summary>
        public Byte Address
        {
            get { return _Address; }
            set 
            {
                if ((value == 0) || (value > 247))
                {
                    throw new ArgumentOutOfRangeException("Address",
                        "������� ���������� ����������� �������� �������� ������ ����������. " +
                        "����� ������ ���� � ��������� 1...247");
                }
                else
                {
                    this._Address = value;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����, ������� �������� ������ ����������
        /// </summary>
        private NetworkController _NetworkController;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ���������� ����, ������� �������� ������ ����������
        /// </summary>
        public NetworkController NetworkController
        {
            get { return _NetworkController; }
            set { _NetworkController = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ������\�������
        /// </summary>
        private CoilsCollection _Coils;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ������\�������
        /// </summary>
        public CoilsCollection Coils
        {
            get { return this._Coils; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ������
        /// </summary>
        private DiscretesInputsCollection _DiscretesInputs;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ������
        /// </summary>
        public DiscretesInputsCollection DiscretesInputs
        {
            get { return _DiscretesInputs; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ��������� ��������
        /// </summary>
        private HoldingRegistersCollection _HoldingRegisters;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ��������� ��������
        /// </summary>
        public HoldingRegistersCollection HoldingRegisters
        {
            get { return _HoldingRegisters; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ������� ���������
        /// </summary>
        private InputRegistersCollection _InputRegisters;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ������� ���������
        /// </summary>
        public InputRegistersCollection InputRegisters
        {
            get { return _InputRegisters; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ ������ ������� ����������
        /// </summary>
        private FilesCollection _Files;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ ������ ����������
        /// </summary>
        public FilesCollection Files
        {
            get { return this._Files; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ����������
        /// </summary>
        private String _Description;
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ����������
        /// </summary>
        public String Description
        {
            get { return this._Description; }
            set 
            {
                if (value == null)
                {
                    this._Description = String.Empty;
                }
                else
                {
                    this._Description = value;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ ����������. 
        /// </summary>
        private Status _Status;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ���������� Modbus
        /// </summary>
        public Status Status
        {
            get { return this._Status; }
            set 
            {
                switch (value)
                {
                    case Status.Running:
                        { this.Start(); break; }
                    case Status.Stopped:
                        { this.Stop(); break; }
                    case Status.Paused:
                        {
                            throw new NotSupportedException(
                                "������ ��������� ����������� �� ��������������");
                        }
                    default:
                        {
                            throw new NotImplementedException(
                                "��������� ������� ��������� ����������� �� �����������");
                        }
                }
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        public Device()
        {
            this._Status = Status.Stopped;
            this.Address = 1;
            this._Description = String.Empty;

            this._Coils = new CoilsCollection();
            this._Coils.SetOwner(this);
            this._Coils.ListWasChanged += 
                new EventHandler(EventHandler_Coils_ListWasChanged);
            this._DiscretesInputs = new DiscretesInputsCollection();
            this._DiscretesInputs.SetOwner(this);
            this._DiscretesInputs.ListWasChanged += 
                new EventHandler(EventHandler_DiscretesInputs_ListWasChanged);
            this._InputRegisters = new InputRegistersCollection();
            this._InputRegisters.SetOwner(this);
            this._InputRegisters.ListWasChanged += 
                new EventHandler(EventHandler_InputRegisters_ListWasChanged);
            this._HoldingRegisters = new HoldingRegistersCollection();
            this._HoldingRegisters.SetOwner(this);
            this._HoldingRegisters.ListWasChanged += 
                new EventHandler(EventHandler_HoldingRegisters_ListWasChanged);
            this._Files = new FilesCollection();
            this._Files.SetOwner(this);
            this._Files.ListWasChanged += 
                new EventHandler(EventHandler_Files_ListWasChanged);
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="address">������� ����� ���������</param>
        public Device(Byte address)
        {
            this._Status = Status.Stopped;
            this.Address = address;
            this._Description = String.Empty;

            this._Coils = new CoilsCollection();
            this._Coils.SetOwner(this);
            this._Coils.ListWasChanged +=
                new EventHandler(EventHandler_Coils_ListWasChanged);
            this._DiscretesInputs = new DiscretesInputsCollection();
            this._DiscretesInputs.SetOwner(this);
            this._DiscretesInputs.ListWasChanged +=
                new EventHandler(EventHandler_DiscretesInputs_ListWasChanged);
            this._InputRegisters = new InputRegistersCollection();
            this._InputRegisters.SetOwner(this);
            this._InputRegisters.ListWasChanged +=
                new EventHandler(EventHandler_InputRegisters_ListWasChanged);
            this._HoldingRegisters = new HoldingRegistersCollection();
            this._HoldingRegisters.SetOwner(this);
            this._HoldingRegisters.ListWasChanged +=
                new EventHandler(EventHandler_HoldingRegisters_ListWasChanged);
            this._Files = new FilesCollection();
            this._Files.SetOwner(this);
            this._Files.ListWasChanged +=
                new EventHandler(EventHandler_Files_ListWasChanged);
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ����������. ������������� �������� ������ ����������. 
        /// ���������� �������� �� ������� �� ���� (�� ������� ����)
        /// </summary>
        public void Start()
        {
            this._Status = Status.Running;
            // ���������� �������
            this.OnStatusWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������������� ����������. ���������� �� ��������� 
        /// �� ������� �� ���� (�� ������� ����)
        /// </summary>
        public void Stop()
        {
            this._Status = Status.Stopped;
            // ���������� �������
            this.OnStatusWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ��������� ������ ���������� ������/�������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Coils_ListWasChanged(object sender, EventArgs e)
        {
            // ���������� �������
            this.OnDeviceChangedConfiguration();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ��������� ������ ���������� ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DiscretesInputs_ListWasChanged(object sender, EventArgs e)
        {
            // ���������� �������
            this.OnDeviceChangedConfiguration();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ��������� ������ ������ ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Files_ListWasChanged(object sender, EventArgs e)
        {
            // ���������� �������
            this.OnDeviceChangedConfiguration();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ��������� ������ ��������� �����/������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_HoldingRegisters_ListWasChanged(
            object sender, EventArgs e)
        {
            // ���������� �������
            this.OnDeviceChangedConfiguration();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ��������� ������ ������� ���������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_InputRegisters_ListWasChanged(
            object sender, EventArgs e)
        {
            // ���������� �������
            this.OnDeviceChangedConfiguration();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� �������� �������� �� ���� ��������� (�� �������) ������������ ���
        /// � ��������� �����, ���� ��� ���������� (��� ������������ �������)
        /// </summary>
        /// <param name="message">�������� ���������</param>
        /// <returns>��������� �������� ���������</returns>
        internal void GetIncommingMessage(Message.Message message)
        {
            // ��������� ������ ����������, ���� ���������� ��������
            // �� ������������ ������ � �� ��������.
            if (this.Status == Status.Running)
            {
                // ���������. ��� ��������� ���������� ������� ���������� ��� ���
                if ((message.Address == this.Address) || (message.Address == 0))
                {
                    // ��������� ������������� ��� ������� ����������
                    // !!! ������ ������� ��������� � ��������� ������
                    this.RequestParse(message);
                }
                else
                {
                    // ���, ��� ��������� �� ������������� ��� ������� ����������
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� ���������� ����� slave-���������� ������� ���� �� ��� ������.
        /// </summary>
        /// <param name="answer">�������� ���������</param>
        private void SendResponse(Message.Message answer)
        {
            if (this.NetworkController != null)
            {
                this.NetworkController.GetOutcommingMessage(answer);
            }
            else
            {
                throw new NullReferenceException(
                    "���������� ��������� ����� ������� �� ������. ���������� ���������� ����");
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ������ � �������� ����������� 
        /// ��� ��� ���������� �������
        /// </summary>
        /// <param name="request">������ �� �������</param>
        protected virtual void RequestParse(Message.Message request)
        {
            Message.Result result;
            
            // �������� ��� ������� � ����������� ���������
            switch (request.PDUFrame.Function)
            {
                case 0x01: // ������� 0x1. ������ ���� (�� ����� ���� �����������������)
                    {
                        result = ((INetworkFunctions)this).ReadCoils(request);
                        break;
                    }
                case 0x02: // ������� 0x2. ������ ���������� ����� (�� ����� ���� �����������������)
                    {
                        result = ((INetworkFunctions)this).ReadDiscreteInputs(request);
                        break;
                    }
                case 0x03: // ������� 0�3. ������ holding-�������� (�� ����� ���� �����������������)
                    {
                        result = ((INetworkFunctions)this).ReadHoldingRegisters(request);
                        break;
                    }
                case 0x04: // ������� 0�4. ������ ������� �������� (�� ����� ���� �����������������)
                    {
                        result = ((INetworkFunctions)this).ReadInputRegisters(request);
                        break;
                    }
                case 0x05: // ������� 0�5. ������������� ���� � ��������� ���./����.
                    {
                        result = ((INetworkFunctions)this).WriteSingleCoil(request);
                        break;
                    }
                case 0x06: // ������� 0x6. ���������� �������� � ��������� �������
                    {
                        result = ((INetworkFunctions)this).WriteSingleRegister(request);
                        break;
                    }
                case 0x0F:
                    {
                        result = ((INetworkFunctions)this).WriteMultipleCoils(request);
                        break;
                    }
                case 0x10:
                    {
                        result = ((INetworkFunctions)this).WriteMultipleRegisters(request);
                        break;
                    }
                case 0x14:
                    {
                        result = ((INetworkFunctions)this).ReadFileRecord(request);
                        break;
                    }
                default:
                    {
                        result = ((INetworkFunctions)this).FunctionNotSupported(request);
                        break;
                    }
            }

        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� ������������� �������� ������� �������. ����� ���������� ����������
        /// ��� ���������� ������� ���������� � ���� ������. � ��� ��, ���������� ��� 
        /// �������� ������� ���������� �� ������ ������. ��� ���� ���� ���������� (null)
        /// </summary>
        /// <param name="owner">�������� �������� ������� ����������</param>
        internal void SetOwner(NetworkController owner)
        {
            if (this._NetworkController == null)
            {
                this._NetworkController = owner;
            }
            else
            {
                if (owner == null)
                {
                    // ����������� ���������, ������ ���������� ��������
                    this._NetworkController = null;
                }
                else
                {
                    // ���� ���������� ����, ������� ����������� ������ ���������� 
                    // ���������������, ����� ������ �� ������. 
                    // ����� ��� ������. � ��������� ������, ���������� ����������
                    if (this.Equals(owner) == false)
                    {
                        throw new InvalidOperationException(
                            "������ modbus-���������� ��� ����������� ������� ����������� ����");
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� MasterChangedCoils
        /// </summary>
        private void OnMasterChangedCoils()
        {
            EventHandler handler = this.MasterChangedCoils;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� MasterChangedHoldingRegisters
        /// </summary>
        private void OnMasterChangedHoldingRegisters()
        {
            EventHandler handler = this.MasterChangedHoldingRegisters;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ��������� ������������ ������ ������ ����������
        /// </summary>
        private void OnDeviceChangedConfiguration()
        {
            EventHandler handler = this.DeviceChangedConfiguration;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;
                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ��������� ������� ����������
        /// </summary>
        private void OnStatusWasChanged()
        {
            EventHandler handler = this.StatusWasChanged;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke synckInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;
                    if (synckInvoke != null)
                    {
                        if (synckInvoke.InvokeRequired)
                        {
                            synckInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������ � ���� ������
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Device; Address: 0x");
            sb.Append(this.Address.ToString("X2"));
            sb.Append("; ");

            if (this._NetworkController != null)
            {
                sb.Append("Network: ");
                sb.Append(this._NetworkController.NetworkName);
                sb.Append("; ");
            }
            else
            {
                sb.Append("Network: null; ");
            }

            sb.Append("Description: ");
            sb.Append(this.Description);
            
            return sb.ToString();
            //return base.ToString();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Events
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ��������� ����� ����, ��� ������-���������� ���������
        /// ������ ������ ��� ����� ���������� ������/�������
        /// </summary>
        public event EventHandler MasterChangedCoils;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ��������� ����� ����, ��� ������-���������� ���������
        /// ������ ������ ��� ����� ��������� ��������
        /// </summary>
        public event EventHandler MasterChangedHoldingRegisters;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ��������� ����� ����, ��� ������-���������� ���������
        /// ������ � ����
        /// </summary>
        //public event EventHandler MasterRecordedFile;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ����� ��������� ������������ ������ ������ ����������
        /// </summary>
        public event EventHandler DeviceChangedConfiguration;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ����� ��������� ������� ����������
        /// </summary>
        public event EventHandler StatusWasChanged;
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        // !!! ����������� ������� ����������� ��������, ��� ���� ��� �� ���������
        // ������ ��������� � ���.
        #region INetworkFunctions Members
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadCoils(Modbus.OSIModel.Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            PDU pdu;
            String message;

            if (request.Address == 0)
            {
                // ������. ������ ������� �� ����� ���� �����������������
                message = "����������������� ������ �� ���������� ������� 0x01 ����������";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // ��������� ����� PDU (������ �������� 5 ������)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // ����� ��������� �� ������
                    message = String.Format(
                        "����� PDU-������ � ������� {0}. ������ ���� 5 ����", request.PDUFrame.ToArray());
                    // �������� ��������� �� ����������
                    result = new Message.Result(Error.DataFormatError, message,
                        request, null);
                }
                else
                {
                    // ��������� ���������
                    Byte[] array = new Byte[2];
                    // �������� ����� ������� ����
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // �������� ��������� ���� ��� ������
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 2000))
                    {
                        message = String.Format(
                            "���������� ���� ��� ������ � ������� {0}, � ������ ���� 1...2000", quantity);
                        pdu = new Message.PDU(0x81, new Byte[] { 0x03 });
                        answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);
                        // ���������� ���������
                        this.SendResponse(answer);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, answer);

                    }
                    else
                    {
                        // ��������� ������. ��������� ����������� � 
                        // ������� ���� � ��������� ��������� �������
                        for (int i = 0; i < quantity; i++)
                        {
                            // ��������� ���������� �� ���� � ��������� �������.
                            // ���� ���� �� ���������� � ������� ��������� �����
                            // c ����������

                            if (this.Coils.Contains(System.Convert.ToUInt16(address + i)) == false)
                            {
                                // ��������� ����� � ���������� 2
                                answer = new Modbus.OSIModel.Message.Message(this.Address,
                                    new PDU(0x81, new Byte[] { 0x2 }));

                                // ���������� ����� �������
                                this.SendResponse(answer);

                                result = new Message.Result(Error.IllegalDataAddress,
                                    String.Format("���� � ������� {0} �� ����������", 
                                    System.Convert.ToUInt16(address + i)),
                                    request, answer);

                                return result;
                            }
                        }

                        // ��� ���� ������� ��������� �������� ���������
                        int totalBytes = quantity % 8;

                        if (totalBytes == 0)
                        {
                            totalBytes = quantity / 8;
                        }
                        else
                        {
                            totalBytes = 1 + (quantity / 8);
                        }

                        Byte[] data = new Byte[totalBytes];
                        int number = 0;
                        int index = 0;

                        for (int i = 0; i < quantity; i++)
                        {
                            data[index] = (Byte)(data[index] | 
                                (Byte)(Modbus.Convert.BooleanToBit(
                                this.Coils[System.Convert.ToUInt16(address + i)].Value) << number));

                            if (++number > 7)
                            {
                                number = 0;
                                ++index;
                            }
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x01;
                        pdu.AddDataByte((byte)totalBytes);  // ��������� ���������� ���� � ����������� ����
                        pdu.AddDataBytesRange(data);        // ��������� ����� � ����������� ����

                        answer = new Modbus.OSIModel.Message.Message(this.Address, pdu);
                        // ���������� ����� �������
                        this.SendResponse(answer);
                        result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    }
                }
            }
            return result;

        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadDiscreteInputs(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            if (request.Address == 0)
            {
                // ������. ������ ������� �� ����� ���� �����������������
                message = "����������������� ������ �� ���������� ������� 0x02 ����������";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // ��������� ����� PDU (������ �������� 5 ������)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // ����� ��������� �� ������
                    String mes = String.Format(
                        "����� PDU-������ ����� � ������� 0x2 ����� {0} ����. ������ ���� 5 ����",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                    Debug.WriteLine(mes);
                }
                else
                {
                    // ��������� ���������
                    Byte[] array = new Byte[2];
                    // �������� ����� ������� ����������� �����
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // �������� ��������� ���������� ������ ��� ������
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 2000))
                    {
                        message =
                            String.Format("���������� ���������� ������ ��� ������ � ������� {0}, � ������ ���� 1...2000",
                            quantity);
                        pdu = new Message.PDU(0x82, new Byte[] { 0x03 });
                        answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);

                        // ���������� ���������
                        this.SendResponse(answer);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, answer);
                    }
                    else
                    {
                        // ��������� ������. ��������� ����������� � 
                        // ������� ���������� ������ � ��������� ��������� �������
                        //DiscreteInput[] inputs = new DiscreteInput[quantity];

                        for (int i = 0; i < quantity; i++)
                        {
                            //inputs[i] = _discretesInputs.Find((UInt16)(address + i));
                            if (this.DiscretesInputs.Contains(System.Convert.ToUInt16(address + i)) == false)
                            {
                            // ���� ���������� ���� �� ���������� � ������� ��������� �����
                            // c ����������� 2
                                pdu = new Message.PDU(0x82, new Byte[] { 0x02 });
                                answer = new Message.Message(this._Address, pdu);

                                message =
                                    String.Format("���������� ���� � ������� {0} �� ����������",
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, answer);

                                // ���������� ����� �������
                                this.SendResponse(answer);

                                return result;
                            }
                        }

                        // ��� ���������� ����� ������� ��������� �������� ���������
                        int totalBytes = quantity % 8;

                        if (totalBytes == 0)
                        {
                            totalBytes = quantity / 8;

                            if (totalBytes == 0)
                            {
                                totalBytes++;
                            }
                        }
                        else
                        {
                            totalBytes = 1 + (quantity / 8);
                        }

                        Byte[] data = new Byte[totalBytes];
                        int number = 0;
                        int index = 0;

                        for (int i = 0; i < quantity; i++)
                        {
                            data[index] = (Byte)(data[index] |
                                (Byte)(Modbus.Convert.BooleanToBit(this.DiscretesInputs[address + i].Value) << number));

                            if (++number > 7)
                            {
                                number = 0;
                                ++index;
                            }
                        }
                        pdu = new Message.PDU();
                        pdu.Function = request.PDUFrame.Function;
                        pdu.AddDataByte((Byte)totalBytes);
                        pdu.AddDataBytesRange(data);
                        answer = new Message.Message(this._Address, pdu);

                        result = new Message.Result(Error.NoError, String.Empty, request, answer);
                        // ���������� ����� �������
                        this.SendResponse(answer);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadHoldingRegisters(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            if (request.Address == 0)
            {
                // ������. ������ ������� �� ����� ���� �����������������
                message = "����������������� ������ �� ���������� ������� 0x03 ����������";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // ��������� ����� PDU (������ �������� 5 ������)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // ����� ��������� �� ������
                    String mes = String.Format(
                        "����� PDU-������ ����� � ������� 0x3 ����� {0} ����. ������ ���� 5 ����",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                }
                else
                {
                    // ��������� ���������
                    Byte[] array = new Byte[2];
                    // �������� ����� ������� �������� �����
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // �������� ��������� ��������� ��� ������
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 125))
                    {
                        message =
                            String.Format("���������� ��������� ��� ������ � ������� {0}, � ������ ���� 1...125",
                            quantity);
                        pdu = new Message.PDU(0x83, new Byte[] { 0x03 });
                        answer = new Message.Message(this._Address, pdu);
                        // ���������� ���������
                        this.SendResponse(answer);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, answer);
                    }
                    else
                    {
                        // ��������� ������. ��������� ����������� � 
                        // ������� ��������� � ��������� ��������� �������
                        for (int i = 0; i < quantity; i++)
                        {
                            // ���� ������� �� ���������� � ������� ��������� �����
                            // c ����������
                            if (this.HoldingRegisters.Contains(System.Convert.ToUInt16(address + i)) == false)
                            {
                                // ��������� ����� � ���������� 2
                                pdu = new Message.PDU(0x83, new Byte[] { 0x02 });
                                answer = new Message.Message(this._Address, pdu);

                                message =
                                    String.Format("������� � ������� {0} �� ����������",
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, answer);

                                // ���������� ����� �������
                                this.SendResponse(answer);

                                return result;
                            }
                        }

                        // ��� �������� ������� ��������� �������� ���������
                        Byte[] temp;
                        List<Byte> data = new List<byte>();

                        data.Add(System.Convert.ToByte((quantity * 2)));

                        for (int i = 0; i < quantity; i++)
                        {
                            temp = Modbus.Convert.ConvertToBytes(this.HoldingRegisters[i].Value);
                            data.AddRange(temp);
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x3;
                        pdu.AddDataBytesRange(data.ToArray());
                        answer = new Message.Message(this._Address, pdu);

                        result = new Message.Result(Error.NoError, String.Empty, request, answer);
                        // ���������� ����� �������
                        this.SendResponse(answer);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadInputRegisters(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            if (request.Address == 0)
            {
                // ������. ������ ������� �� ����� ���� �����������������
                message = "����������������� ������ �� ���������� ������� 0x04 ����������";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // ��������� ����� PDU (������ ��������  ������)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // ����� ��������� �� ������
                    String mes = String.Format(
                        "����� PDU-������ ����� � ������� 0x4 ����� {0} ����. ������ ���� 5 ����",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                }
                else
                {
                    // ��������� ���������
                    Byte[] array = new Byte[2];
                    // �������� ����� ������� �������� �����
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // �������� ��������� ��������� ��� ������
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 125))
                    {
                        message =
                            String.Format("���������� ��������� ��� ������ � ������� {0}, � ������ ���� 1...125",
                            quantity);
                        pdu = new Message.PDU(0x84, new Byte[] { 0x03 });
                        answer = new Message.Message(this._Address, pdu);

                        // ���������� ���������
                        this.SendResponse(answer);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, answer);
                    }
                    else
                    {
                        // ��������� ������. ��������� ����������� � 
                        // ������� ��������� � ��������� ��������� �������
                        for (int i = 0; i < quantity; i++)
                        {
                            // ���� ������� �� ���������� � ������� ��������� �����
                            // c ����������� 2
                            if (this.InputRegisters.Contains(System.Convert.ToUInt16(address + i)) == false)
                            {
                                pdu = new Message.PDU(0x84, new Byte[] { 0x02 });
                                answer = new Message.Message(this.Address, pdu);

                                message =
                                    String.Format("������� � ������� {0} �� ����������",
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, answer);

                                // ���������� ����� �������
                                this.SendResponse(answer);
                                return result;
                            }
                        }

                        // ��� �������� ������� ��������� �������� ���������
                        Byte[] temp;
                        List<Byte> data = new List<byte>();

                        for (int i = 0; i < quantity; i++)
                        {
                            temp = Modbus.Convert.ConvertToBytes(
                                this.InputRegisters[System.Convert.ToUInt16(address + i)].Value);
                            data.AddRange(temp);
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x04;
                        pdu.AddDataByte((Byte)(data.Count));
                        pdu.AddDataBytesRange(data.ToArray());
                        answer = new Message.Message(this._Address, pdu);

                        result = new Result(Error.NoError, String.Empty, request, answer);
                        // ���������� ����� �������
                        this.SendResponse(answer);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.WriteSingleCoil(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            // ��������� ����� PDU (������ ���� 5 ����)
            if (request.PDUFrame.ToArray().Length != 5)
            {
                // ����� ��������� �� ������
                String mes = String.Format(
                    "����� PDU-������ ����� � ������� 0x05 ����� {0} ����. ������ ���� 5 ����",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
            }
            else
            {
                // ��������� ���������
                Byte[] array = new Byte[2];
                // �������� ����� ����
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // �������� �������� ���� ��� ������
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 status = Modbus.Convert.ConvertToUInt16(array);
                State coilValue;
                // �������� ���������� ���� � �������
                switch (status)
                {
                    case 0x0000:
                        {
                            coilValue = State.Off;
                            break;
                        }
                    case 0xFF00:
                        {
                            coilValue = State.On;
                            break;
                        }
                    default:
                        {
                            // ������
                            message =
                                String.Format(
                                "�������� ������ ������ ��� ��������� ��������� ���� {0}, � ������ ���� 0x0000 ��� 0xFF00",
                                status.ToString("X4"));
                            pdu = new Message.PDU((Byte)(0x05 | 0x80), new Byte[] { 0x03 });
                            answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);
                            // ���������� ���������
                            this.SendResponse(answer);

                            result = new Message.Result(Error.IllegalDataValue,
                                message, request, answer);
                            return result;
                        }
                }

                // ��������� ������. ��������� ����������� � 
                // ������� ��������� � ��������� ��������� �������

                // ���� ������� �� ���������� � ������� ��������� �����
                // c ����������
                if (this.Coils.Contains(address) == false)
                {
                    // ��������� ����� � ���������� 2
                    pdu = new Message.PDU((Byte)(0x80 | 0x05), new Byte[] { 0x02 });
                    answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);
                    message =
                        String.Format("���� � ������� {0} �� ����������",
                            address);
                    result = new Message.Result(Error.IllegalDataAddress,
                        message, request, answer);

                    // ���������� ����� �������
                    this.SendResponse(answer);
                    return result;
                }
                else
                {
                    // ���� �������, ������������� ����� �������� �
                    // ��������� �������� ���������
                    this.Coils[address].Value = Modbus.Convert.ToBoolean(coilValue);

                    // ��������� �����.
                    List<Byte> data_ = new List<byte>();
                    data_.AddRange(Modbus.Convert.ConvertToBytes(address));
                    data_.AddRange(Modbus.Convert.StateToArray(coilValue));

                    pdu = new Message.PDU();
                    pdu.Function = 0x05;
                    pdu.AddDataBytesRange(data_.ToArray());
                    answer = new Message.Message(this._Address, pdu);

                    result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    // ���������� ����� �������
                    this.SendResponse(answer);
                    // ��������� �������
                    this.OnMasterChangedCoils();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.WriteSingleRegister(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            // ��������� ����� PDU (������ ���� 5 ����)
            if (request.PDUFrame.ToArray().Length != 5)
            {
                // ����� ��������� �� ������
                String mes = String.Format(
                    "����� PDU-������ ����� � ������� 0x06 ����� {0} ����. ������ ���� 5 ����",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
            }
            else
            {
                // ��������� ���������
                Byte[] array = new Byte[2];
                // �������� ����� ��������
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // �������� �������� ��������
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 value = Modbus.Convert.ConvertToUInt16(array);

                // ��������� ������. ��������� ����������� � 
                // �������� 
                if (this._HoldingRegisters.Contains(address) == false)
                {
                    // ���� ������� �� ���������� � ������� ��������� �����
                    // c ����������
                    // ��������� ����� � ���������� 2
                    pdu = new Message.PDU((Byte)(0x06 | 0x80), new Byte[] { 0x02 });
                    answer = new Message.Message(this._Address, pdu);

                    message =
                        String.Format("������� � ������� {0} �� ����������", address);
                    
                    result = new Message.Result(Error.IllegalDataAddress,
                        message, request, answer);

                    // ���������� ����� �������
                    this.SendResponse(answer);

                    return result;
                }
                else
                {
                    // ��� �������� �������, ������������� ����� �������� �
                    // ��������� �������� ���������
                    this._HoldingRegisters[address].Value = value;

                    // ��������� �����.
                    pdu = new Message.PDU();
                    pdu.Function = 0x06;
                    pdu.AddDataBytesRange(Modbus.Convert.ConvertToBytes(address));
                    pdu.AddDataBytesRange(Modbus.Convert.ConvertToBytes(value));
                    answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);

                    result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    // ���������� ����� �������
                    this.SendResponse(answer);
                    // ��������� �������
                    this.OnMasterChangedHoldingRegisters();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.WriteMultipleCoils(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu; 
            String message;


            // ��������� ����� PDU (������ ���� �� ����� 7 ������)
            if (request.PDUFrame.ToArray().Length < 7)
            {
                // ����� ��������� �� ������
                String mes = String.Format(
                    "����� PDU-������ ����� � ������� 0x0F ����� {0} ����. ������ ���� �� ����� 7 ����",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
            }
            else
            {
                // ��������� ���������
                Byte[] array = new Byte[2];
                // �������� ����� ������� ����
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // �������� ��������� ���� ��� ������
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);
                // �������� ���������� ���� � �������
                Byte count = request.PDUFrame.Data[4];

                int totalBytes = quantity % 8;

                if (totalBytes == 0)
                {
                    totalBytes = quantity / 8;
                }
                else
                {
                    totalBytes = 1 + (quantity / 8);
                }

                if ((quantity == 0) || (quantity > 0x7B0) || (totalBytes != count))
                {
                    message =
                        String.Format("���������� ���� ��� ������ � ������� ����� {0}, � ������ ���� 1...0x7B0",
                        quantity);
                    pdu = new Message.PDU((Byte)(0xF | 0x80), new Byte[] { 0x03 });
                    answer = new Message.Message(this._Address, pdu);

                    // ���������� ���������
                    this.SendResponse(answer);

                    result = new Message.Result(Error.IllegalDataValue,
                        message, request, answer);
                }
                else
                {
                    // ��������� ������. ��������� ����������� � 
                    // ������� ��������� � ��������� ��������� �������
                    for (int i = 0; i < quantity; i++)
                    {
                        // ���� ������� �� ���������� � ������� ��������� �����
                        // c ����������
                        if (this.Coils.Contains(System.Convert.ToUInt16(address + i)) == false)
                        {
                            // ��������� ����� � ���������� 2
                            pdu = new Message.PDU((Byte)(0x80 | 0x0F), new Byte[] { 0x02 });
                            answer = new Message.Message(this.Address, pdu);

                            message =
                                String.Format("���� � ������� {0} �� ����������",
                                (address + i));
                            result = new Message.Result(Error.IllegalDataAddress,
                                message, request, answer);

                            // ���������� ����� �������
                            this.SendResponse(answer);
                            return result;
                        }
                    }

                    // ��� ���� �������, ������������� ����� �������� �
                    // ��������� �������� ���������
                    Byte status;
                    // ������������� ����� �������� � ����

                    for (int i = 0; i < count; i++)
                    {
                        status = request.PDUFrame.Data[5 + i];

                        for (int y = 0; y < 8; y++)
                        {
                            totalBytes = i * 8 + y;

                            if (totalBytes < quantity)
                            {
                                if (((status >> y) & 0x01) == 0)
                                {
                                    this.Coils[System.Convert.ToUInt16(address + totalBytes)].Value = 
                                        Modbus.Convert.ToBoolean(State.Off); 
                                    //rows[totalBytes]["Value"] = Modbus.Convert.ToBoolean(State.Off);
                                }
                                else
                                {
                                    this.Coils[System.Convert.ToUInt16(address + totalBytes)].Value =
                                        Modbus.Convert.ToBoolean(State.On);
                                    //rows[totalBytes]["Value"] = Modbus.Convert.ToBoolean(State.On);
                                }
                            }
                        }
                    }
                    // ��������� �����.
                    List<Byte> data_ = new List<byte>();
                    data_.AddRange(Modbus.Convert.ConvertToBytes(address));
                    data_.AddRange(Modbus.Convert.ConvertToBytes(quantity));

                    pdu = new Message.PDU();
                    pdu.Function = 0x0F;
                    pdu.AddDataBytesRange(data_.ToArray());
                    answer = new Message.Message(this._Address, pdu);

                    result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    // ���������� ����� �������
                    this.SendResponse(answer);
                    // ��������� �������
                    this.OnMasterChangedCoils();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.WriteMultipleRegisters(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            // ��������� ����� PDU (������ ���� �� ����� 8 ������)
            if (request.PDUFrame.ToArray().Length < 8)
            {
                // ����� ��������� �� ������
                String mes = String.Format(
                    "����� PDU-������ ����� � ������� 0x10 ����� {0} ����. ������ ���� �� ����� 8 ����",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
            }
            else
            {
                // ��������� ���������
                Byte[] array = new Byte[2];
                // �������� ����� ������� ��������
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // �������� ��������� ��������� ��� ������
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);
                // �������� ���������� ���� � �������
                Byte count = request.PDUFrame.Data[4];

                if ((quantity == 0) || (quantity > 123) || ((quantity * 2) != count))
                {
                    message =
                        String.Format("���������� ��������� ��� ������ � ������� ����� {0}, � ������ ���� 1...123",
                        quantity);
                    pdu = new Message.PDU((Byte)(0x10 | 0x80), new Byte[] { 0x03 });
                    answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);

                    // ���������� ���������
                    this.SendResponse(answer);

                    result = new Message.Result(Error.IllegalDataValue,
                        message, request, answer);
                }
                else
                {
                    // ��������� ������. ��������� ����������� � 
                    // ������� ��������� � ��������� ��������� �������
                    for (int i = 0; i < quantity; i++)
                    {
                        // ���� ������� �� ���������� � ������� ��������� �����
                        // c ����������
                        if (this.HoldingRegisters.Contains(System.Convert.ToUInt16(address + i)) == false)
                        {
                            // ��������� ����� � ���������� 2
                            pdu = new Message.PDU((Byte)(0x10 | 0x80), new Byte[] { 0x02 });
                            answer = new Message.Message(this._Address, pdu);
                            message =
                                String.Format("������� � ������� {0} �� ����������",
                                (address + i));
                            result = new Message.Result(Error.IllegalDataAddress,
                                message, request, answer);

                            // ���������� ����� �������
                            this.SendResponse(answer);
                            return result;
                        }
                    }

                    // ��� �������� �������, ������������� ����� �������� �
                    // ��������� �������� ���������
                    Byte[] temp = new Byte[2];
                    List<Byte> data = new List<byte>();

                    // ������������� ����� �������� � ��������
                    for (int i = 0; i < quantity; i++)
                    {
                        Array.Copy(request.PDUFrame.Data, (5 + (i * 2)), temp, 0, 2);
                        this._HoldingRegisters[System.Convert.ToUInt16(address + i)].Value = 
                            Modbus.Convert.ConvertToUInt16(temp);
                    }

                    // ��������� �����.
                    temp = Modbus.Convert.ConvertToBytes(address);
                    data.AddRange(temp);
                    temp = Modbus.Convert.ConvertToBytes(quantity);
                    data.AddRange(temp);

                    pdu = new Message.PDU();
                    pdu.Function = 0x10;
                    pdu.AddDataBytesRange(data.ToArray());
                    answer = new Message.Message(this._Address, pdu);
                    result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    // ���������� ����� �������
                    this.SendResponse(answer);
                    // ��������� �������
                    this.OnMasterChangedHoldingRegisters();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadFileRecord(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            //Message.PDU pdu;
            String message;
            UInt16 var;


            Code0x14SubRequest[] subRequestList;

            const int ByteCountIndex = 0; // ������ ��������� Byte Count � ������� ������ �������  

            if (request.Address == 0)
            {
                // ������. ������ ������� �� ����� ���� �����������������
                message = "����������������� ������ �� ���������� ������� 0x14 ����������";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
                return result;
            }
            else
            {
                if (request.PDUFrame.Length < 7)
                {
                    // ������. ����������� ����� pdu ������� 0�14 ��� 7 ����
                    message = "������. ����� pdu ������� 0x14 ����� 7 ����";
                    answer = new Message.Message(this._Address,
                        new PDU((Byte)(0x14 | 0x80), new byte[] { 0x03 }));

                    result = new Message.Result(Error.RequestError,
                        message, request, answer);
                    // ���������� �����
                    this.SendResponse(answer);
                    return result;
                }
                else
                {
                    if ((request.PDUFrame.Data[ByteCountIndex] < 0x07) ||
                        (request.PDUFrame.Data[ByteCountIndex] > 0xF5))
                    {
                        // ������. ��������� ����� Byte Count ��� ����������� ��������� ��������
                        message = String.Format(
                            "������. ��� ������� 0x14. �������� Byte Count = {0} ��� ����������� ��������� 0x07...0xF5",
                            request.PDUFrame.Data[ByteCountIndex], (request.PDUFrame.Length - 2));

                        answer = new Message.Message(this._Address,
                        new PDU((Byte)(0x14 | 0x80), new byte[] { 0x03 }));

                        result = new Message.Result(Error.RequestError,
                            message, request, answer);
                        // ���������� �����
                        this.SendResponse(answer);
                        return result;
                    }
                    else
                    {
                        // ��������� ����������� ����� ������� � �������� ���������� 
                        // � ������� "Byte Count"
                        if ((Int32)request.PDUFrame.Data[ByteCountIndex] != (request.PDUFrame.Length - 2))
                        {
                            // ������. ��������� ����� �� ��������� � �����������
                            message = String.Format(
                                "������. �������� Byte Count = {0} �� ��������� � ����������� ������ {1}",
                                request.PDUFrame.Data[ByteCountIndex], (request.PDUFrame.Length - 2));

                            answer = new Message.Message(this._Address,
                            new PDU((Byte)(0x14 | 0x80), new byte[] { 0x03 }));

                            result = new Message.Result(Error.RequestError,
                                message, request, answer);
                            // ���������� �����
                            this.SendResponse(answer);
                            return result;
                        }
                        else
                        {
                            // ���������� ���������� ����� (�����������) � ������ �������
                            if ((request.PDUFrame.Data.Length - 1) % 7 != 0)
                            {
                                // ����� ���������� ������ ����� 7, ������� ������ ����
                                // ������. ���� ��� ������� �� 7, ������� �� ����� 0, ��
                                // ������ �� ���������
                                // ������. ��������� ����� �� ��������� � �����������
                                message = String.Format("������. �������� ����� ������ � �������");

                                answer = new Message.Message(this._Address,
                                new PDU((Byte)(0x14 | 0x80), new byte[] { 0x03 }));

                                result = new Message.Result(Error.RequestError,
                                    message, request, answer);
                                // ���������� �����
                                this.SendResponse(answer);
                                return result;

                            }
                            else
                            {
                                // ���������� ����� ���������� ������. �������� ��
                                // �������� ���������� �����������
                                subRequestList = new Code0x14SubRequest[((request.PDUFrame.Data.Length - 1) / 7)];
                                // �������� ���� ����������
                                for (int i = 0; i < subRequestList.Length; i++)
                                {
                                    subRequestList[i].ReferenceType = request.PDUFrame.Data[(7 * i + 1)];
                                    var = (UInt16)(((UInt16)request.PDUFrame.Data[(7 * i + 2)]) << 8); // Hi
                                    var = (UInt16)(var | request.PDUFrame.Data[(7 * i + 3)]); // Lo
                                    subRequestList[i].FileNumber = var;
                                    var = (UInt16)(((UInt16)request.PDUFrame.Data[(7 * i + 4)]) << 8); // Hi
                                    var = (UInt16)(var | request.PDUFrame.Data[(7 * i + 5)]); // Lo
                                    subRequestList[i].RecordNumber = var;
                                    var = (UInt16)(((UInt16)request.PDUFrame.Data[(7 * i + 6)]) << 8); // Hi
                                    var = (UInt16)(var | request.PDUFrame.Data[(7 * i + 7)]); // Lo
                                    subRequestList[i].RecordLength = var;
                                }
                                // ��������� ������������ ������ � ����������� 
                                for (int i = 0; i < subRequestList.Length; i++)
                                {
                                    // ��������� �������� ��������� "The reference type". ��� ������
                                    // ������ ��������� 0x6
                                    if (subRequestList[i].ReferenceType != 0x6)
                                    {
                                        // ������
                                        answer = new Message.Message(this._Address,
                                            new PDU((Byte)(0x80 | 0x14), new byte[] { 0x2 }));
                                        message = "������: ��������� ����� ������������ �������� �������� The reference type";
                                        result = new Result(Error.RequestError, message, request, answer);
                                        
                                        this.SendResponse(answer);
                                        
                                        return result;
                                    }
                                    else
                                    {
                                        // ��������� ����� ����������� ������. ������ ���� 
                                        // �� ����� 10000 (0x270F)
                                        if (subRequestList[i].RecordNumber > 0x270F)
                                        {
                                            // ������
                                            answer = new Message.Message(this._Address,
                                                new PDU((Byte)(0x80 | 0x14), new byte[] { 0x2 }));
                                            message = "������: ��������� ����� ������������ �������� ������ ������ �����";
                                            result = new Result(Error.RequestError, message, request, answer);

                                            this.SendResponse(answer);

                                            return result;
                                        }
                                        else
                                        {
                                            // ��������� �������� ����� (��������� ����� + ����� �����)
                                            if (subRequestList[i].RecordNumber + subRequestList[i].RecordLength > 0x270F)
                                            {
                                                // ������
                                                answer = new Message.Message(this._Address,
                                                    new PDU((Byte)(0x80 | 0x14), new byte[] { 0x2 }));
                                                message = "������: ��������� ����� ������������ �������� ����� ����� �������� ������� �� �����";
                                                result = new Result(Error.RequestError, message, request, answer);

                                                this.SendResponse(answer);

                                                return result;
                                            }
                                            else
                                            {
                                                // ��� �������� ��������. ������ ����� �������� �������� 
                                                // ��������
                                            }
                                        }
                                    }
                                }
                                
                                // ���������� ����� ����� ������ � ��������� ��������� �� ��� ���������� ������
                                var = 2; // Function code + Resp. Data length
                                for (int i = 0; i < subRequestList.Length; i++)
                                {
                                    var = System.Convert.ToUInt16(var + 2); // File resp. length + Ref. Type 
                                    var = System.Convert.ToUInt16(var + (subRequestList[i].RecordLength * 2));
                                }

                                if (var > 253)
                                {
                                    // ������. ����� ����������� ������ ��������� ������������
                                    // ����� PDU ��������� ������
                                    answer = new Message.Message(this._Address,
                                        new PDU((Byte)(0x80 | 0x14), new byte[] { 0x2 }));
                                    message = "������: ����� ����������� ������ ��������� ������������ ����� PDU";
                                    result = new Result(Error.RequestError, message, request, answer);

                                    this.SendResponse(answer);

                                    return result;
                                }
                                else
                                {
                                    // ���� ��������� � ���� �����, ������ ������ ���������. �������� ��������
                                    Code0x14SubRequest subRequest;
                                    List<byte> list = new List<byte>(var);
                                    list.Add(System.Convert.ToByte(var - 2)); // ���� Resp. data length

                                    for (int i = 0; i < subRequestList.Length; i++)
                                    {
                                        subRequest = subRequestList[i];

                                        if (this._Files.Contains(subRequest.FileNumber) == true)
                                        {
                                            // ���� ������. �������� ��� ������
                                            list.Add(System.Convert.ToByte(subRequest.RecordLength * 2 + 1)); // File resp. length
                                            list.Add(0x06); // Ref. type

                                            File file = this._Files[subRequest.FileNumber];
                                            
                                            for (int y = 0; y < subRequest.RecordLength; y++)
                                            {
                                                // ��������� ���������� �� ������ � ������ �������
                                                var = System.Convert.ToUInt16(
                                                    subRequest.RecordNumber + System.Convert.ToUInt16(y)); 
                                                if (file.Records.Contains(var) == true)
                                                {
                                                    // ��������� ������ ����������. �������� � ��������
                                                    // ��������� � ������ � ���� ������������������ ����
                                                    list.AddRange(Modbus.Convert.ConvertToBytes(
                                                        file.Records[var].Value));
                                                }
                                                else
                                                {
                                                    // ��������� ������ �� ���������� � ������ �����
                                                    // ������. ����� ����������� ������ ��������� ������������
                                                    // ����� PDU ��������� ������
                                                    answer = new Message.Message(this._Address,
                                                        new PDU((Byte)(0x80 | 0x14), new byte[] { 0x4 }));
                                                    message = String.Format(
                                                        "������: �� ������� ������������� ������ � ������� {0} � ������������� ����� c ������� {0}",
                                                        var, file.Number);
                                                    result = new Result(Error.RequestError, message, request, answer);

                                                    this.SendResponse(answer);

                                                    return result;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // ����� � ����� ������� �� ����������. ���������� ����������
                                            answer = new Message.Message(this._Address,
                                                new PDU((Byte)(0x80 | 0x14), new byte[] { 0x4 }));
                                            message = String.Format(
                                                "������: �� ������ ������������� ���� c ������� {0}",
                                                subRequest.FileNumber);
                                            result = new Result(Error.RequestError, message, request, answer);

                                            this.SendResponse(answer);

                                            return result;
                                        }
                                    }

                                    // ���� ��������� � ������ �����, ������ ���������� ���� ���������� ��������
                                    // ����� ���������� ������
                                    answer = new Message.Message(this._Address,
                                        new PDU(0x14, list.ToArray()));
                                    message = String.Empty;
                                    result = new Result(Error.NoError, message, request, answer);

                                    this.SendResponse(answer);

                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            //return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.FunctionNotSupported(Message.Message request)
        {
            Message.Message answer;
            Message.PDU pdu = new Message.PDU();
            pdu.Function = (Byte)(request.PDUFrame.Function | 0x80);
            pdu.AddDataByte(0x01);   //Error.IllegalFunction
            answer = new Modbus.OSIModel.Message.Message(this.Address, pdu);

            // ���������� �����
            this.SendResponse(answer);

            Message.Result result =
                new Message.Result(Error.IllegalFunction,
                    "������� �� �������������� ������ �����������",
                    request, answer);
            return result;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region IManageable Members
        //---------------------------------------------------------------------------
        void IManageable.Start()
        {
            this.Start();
        }
        //---------------------------------------------------------------------------
        void IManageable.Stop()
        {
            this.Stop();
        }
        //---------------------------------------------------------------------------
        void IManageable.Suspend()
        {
            throw new NotSupportedException("������ ����� �� ��������������");
        }
        //---------------------------------------------------------------------------
        Status IManageable.Status
        {
            get
            {
                return this.Status;
            }
            set
            {
                this.Status = value;
            }
        }
        //---------------------------------------------------------------------------
        event EventHandler IManageable.StatusWasChanged
        {
            add 
            {
                this.StatusWasChanged += value;
            }
            remove 
            {
                this.StatusWasChanged -= value;
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file