using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Modbus.OSIModel.DataLinkLayer.Slave;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using Common.Controlling;

namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    /// <summary>
    /// ����� ��������� ���������� ���� Modubs �� ������� slave-��������� 
    /// ������������ � ������� ���� ����� IDataLinkLayer
    /// </summary>
    public class ModbusNetworkControllerSlave: ModbusNetworkControllerBase
    {
        #region Fields and Properties
        /// <summary>
        /// ���������� ��� ����������� ���� (slave ��� master)
        /// </summary>
        public override WorkMode Mode
        {
            get { return WorkMode.Slave; }
        }
        /// <summary>
        /// ������������ ���� Modbus
        /// </summary>
        private String _NetworkName;
        /// <summary>
        /// ������������ ���� Modbus
        /// </summary>
        public override String NetworkName
        {
            get { return _NetworkName; }
            set 
            {
                if (ModbusNetworksManager.Instance.Networks.Contains(value))
                {
                    throw new ArgumentException("���������� ���������� ����� ������������ ����, " +
                        "��� ��� ���� � ����� ������������� ��� ����������", "NetworkName");
                }
                else
                {
                    _NetworkName = value;
                }
            }
        }
        /// <summary>
        /// ���� ��� ����������� ����������� � ���� modbus
        /// </summary>
        IDataLinkLayer _Connection;
        /// <summary>
        /// ���� ��� ����������� ����������� � ���� modbus
        /// </summary>
        public IDataLinkLayer Connection
        {
            get { return _Connection; }
            set 
            {
                lock (SyncRoot)
                {
                    if (Status == Status.Stopped)
                    {
                        if (value != null)
                        {
                            if (value.IsOpen == true)
                            {
                                throw new InvalidOperationException(
                                    "���������� ���������� ����� ����������, �.� ������� ����������� �������");
                            }
                            else
                            {
                                _Connection = value;

                                _Connection.RequestWasRecived +=
                                    new EventHandlerRequestWasRecived(EventHandler_Connection_RequestWasRecived);
                                _Connection.ErrorOccurred +=
                                    new EventHandlerErrorOccurred(EventHandler_Connection_ErrorAccured);
                            }
                        }
                        else
                        {
                            _Connection = value;                            
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "������� �������� ���� ����������� � ���� modbus ���������� ���� � ���������� ���������");
                    }
                }
            }
        }
        /// <summary>
        /// ��������� ��������� � ������ ����
        /// </summary>
        DevicesCollection _Devices;
        /// <summary>
        /// ���������� ��������� ��������� � ����
        /// </summary>
        public DevicesCollection Devices
        {
            get { return _Devices; }
        }
        /// <summary>
        /// ����������/������������� 
        /// </summary>
        public override Status Status
        {
            get 
            {
                if (_Connection != null)
                {
                    if (_Connection.IsOpen)
                    {
                        return Status.Running;
                    }
                    else
                    {
                        return Status.Stopped;
                    }
                }
                else
                {
                    return Status.Stopped;
                }
            }
            set 
            {
                switch (value)
                {
                    case Status.Paused:
                        { Suspend(); break; }
                    case Status.Running:
                        { Start(); break; }
                    case Status.Stopped:
                        { Stop(); break; }
                }
            }
        }
        /// <summary>
        /// ����� ��� ��������� �������� ��������� �� ������� ����
        /// </summary>
        //private Thread _ThreadMsgListener;
        /// <summary>
        /// ������ ��� �������������.
        /// </summary>
        public static Object SyncRoot = new Object();
        #endregion

        #region Constructors
        /// <summary>
        /// �����������
        /// </summary>
        public ModbusNetworkControllerSlave()
        {
            ModbusNetworksManager manager = ModbusNetworksManager.Instance;
            _NetworkName = GetNewName(); 

            //Stop();
            _Devices = new DevicesCollection(this);
            _Devices.ItemsListWasChanged +=
                new EventHandler(EventHandler_DevicesCollection_ItemsListWasChanged);
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="networkName">������������ ������ ����</param>
        /// <param name="connection">������ ��� ����������� ����������� � ����</param>
        public ModbusNetworkControllerSlave(String networkName, IDataLinkLayer connection)
        {
            ModbusNetworksManager manager = ModbusNetworksManager.Instance;
            
            // ��������� ������������ ������������ ����
            if (manager.Networks.Contains(networkName))
            {
                throw new Exception(
                    "������� ������� ���� Modbus � �������������, ������� ����� ������ ����");
            }
            else
            {
                _NetworkName = networkName;
            }

            _Devices = new DevicesCollection(this);
            _Devices.ItemsListWasChanged +=
                new EventHandler(EventHandler_DevicesCollection_ItemsListWasChanged);

            _Connection = connection;

            if (_Connection != null)
            {
                _Connection.RequestWasRecived +=
                    new EventHandlerRequestWasRecived(EventHandler_Connection_RequestWasRecived);
                _Connection.ErrorOccurred +=
                    new EventHandlerErrorOccurred(EventHandler_Connection_ErrorAccured);

                // ��������� ���������� � ��������� "����"
                Stop();
            }
        }
        #endregion

        #region EventHandlers For _DataLinkLayerObject
        /// <summary>
        /// ���������� ������� ������ ������� ����������� ����������� � ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandler_Connection_ErrorAccured(
            object sender, ErrorOccurredEventArgs args)
        {
            String msg;
            
            msg = String.Format(
                "������ ��� ������ ����������: {0}; ��������: {1}",
                args.Error, args.Description);

            OnNetworkErrorOccurred(new NetworkErrorEventArgs(
                ErrorCategory.DataLinkLayerError, msg, null));
            return;
        }
        /// <summary>
        /// ���������� ������� ��������� �� ���� ������� �� �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandler_Connection_RequestWasRecived(
            object sender, MessageEventArgs args)
        {
            if (args.Message.Address == 0)
            {
                // ��������� �������� ��������� �� �����������
                foreach (Device device in _Devices)
                {
                    device.GetIncommingMessage(args.Message);
                }
            }
            else
            {
                if (Devices.Contains(args.Message.Address) == true)
                {
                    // ���������� � ��������� ������� � ������� ����������.
                    // ���������� ��� ������ ������
                    lock (SyncRoot)
                    {
                        Devices[args.Message.Address].GetIncommingMessage(args.Message);
                    }
                }
            }
            return;
        }
        #endregion

        #region Methods
        /// <summary>
        /// ���������� ����� ���������� ��� ����
        /// </summary>
        /// <returns></returns>
        private String GetNewName()
        {
            string networkName;
            ModbusNetworksManager manager = ModbusNetworksManager.Instance;

            // ������������� ������������ ���� �� ���������
            // ������ ����� [Network][index]
            // ������ - ����� �� �������
            // ���� �� ������� ��� � ������������

            for (int i = 1; i < Int32.MaxValue; i++)
            {
                // �������� ����� ��� 
                networkName = "Network" + i.ToString();
                // ���������, ���������� �� ��� ����� � ������
                if (!manager.Networks.Contains(networkName))
                {
                    return networkName;
                }
            }
            // �� ������� ����� ���������� ���.
            throw new Exception();
        }
        /// <summary>
        /// ���������� ������� ��������� ������ ���������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DevicesCollection_ItemsListWasChanged(
            object sender, EventArgs e)
        {
            // ���������� ������� 
            OnDevicesListWasChanged();
            return;
        }
        /// <summary>
        /// ����� ��������� ��������� ��������� (����� �� ������) �� ��������
        /// ����������
        /// </summary>
        /// <param name="message">��������� �������� ���������</param>
        internal void GetOutcommingMessage(Message.Message message)
        {
            _Connection.SendResponse(message);
        }
        /// <summary>
        /// ��������� ������ ����
        /// </summary>
        public override void Start()
        {
            if (_Connection != null)
            {
                if (_Connection.IsOpen)
                {
                    // ������ �� ������, ���������� ��� �������
                }
                else
                {
                    Connection.Open();
                }
            }
            else
            {
                throw new NullReferenceException(
                    "���������� ��������� ���������� ����, �� ������ ������ IDataLinkLayer");
            }
            return;
        }
        /// <summary>
        /// ������������� ������ ����
        /// </summary>
        public override void Stop()
        {
            if (_Connection != null)
            {
                if (_Connection.IsOpen)
                {
                    foreach (Device device in _Devices)
                    {
                        device.Stop();
                    }
                    Connection.Close();                    
                }
                else
                {
                    // ������ �� ������, ���������� ��� �������
                }
            }
            else
            {
                //throw new NullReferenceException(
                //    "���������� ���������� ���������� ����, �� ������ ������ IDataLinkLayer");
            }
            return;
        }
        /// <summary>
        /// ���������������� ������ ����
        /// </summary>
        public override void Suspend()
        {
            throw new NotImplementedException(
                "���������: Paused, ����������� ���� modbus �� ��������������");
        }
        /// <summary>
        /// ����������� � ��������� ���������� ����������� � ���� � ����
        /// XML-�����
        /// </summary>
        public void SaveToXmlFile(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                path = Environment.CurrentDirectory + @"\config.xml";
            }

            using (XmlTextWriter writer = new XmlTextWriter(path, null))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                
                writer.WriteStartElement("Network");
                writer.WriteAttributeString("Name", NetworkName);

                // ��������� ������ ���������
                writer.WriteStartElement("Devices");
                foreach (Device device in _Devices)
                {
                    writer.WriteStartElement("Device");
                    // ��������� �������� �������� "Device"
                    writer.WriteAttributeString("Address", device.Address.ToString());
                    writer.WriteAttributeString("Description", device.Description);
                    writer.WriteAttributeString("Status", device.Status.ToString());
                    
                    // ��������� ���� �������� "Device"
                    
                    // ��������� �������� ��������
                    writer.WriteStartElement("HoldingRegisters");
                    foreach (HoldingRegister register in device.HoldingRegisters)
                    {
                        writer.WriteStartElement("HoldingRegister");
                        writer.WriteAttributeString("Address", register.Address.ToString());
                        writer.WriteStartElement("Value");
                        writer.WriteValue(register.Value);
                        writer.WriteFullEndElement(); // "Value"
                        writer.WriteStartElement("Description");
                        writer.WriteString(register.Description);
                        writer.WriteFullEndElement(); // "Description"
                        writer.WriteFullEndElement(); // "HoldingRegister"
 
                    }
                    writer.WriteFullEndElement(); // "HoldingRegisters"

                    // ��������� ������� ��������
                    writer.WriteStartElement("InputRegisters");
                    foreach (InputRegister register in device.InputRegisters)
                    {
                        writer.WriteStartElement("InputRegister");
                        writer.WriteAttributeString("Address", register.Address.ToString());
                        writer.WriteStartElement("Value");
                        writer.WriteValue(register.Value);
                        writer.WriteFullEndElement(); // "Value"
                        writer.WriteStartElement("Description");
                        writer.WriteString(register.Description);
                        writer.WriteFullEndElement(); // "Description"
                        writer.WriteFullEndElement(); // "InputRegister"

                    }
                    writer.WriteFullEndElement(); // "InputRegisters"

                    // ��������� ���������� �����/������
                    writer.WriteStartElement("�oils");
                    foreach (Coil coil in device.Coils)
                    {
                        writer.WriteStartElement("Coil");
                        writer.WriteAttributeString("Address", coil.Address.ToString());
                        writer.WriteStartElement("Value");
                        writer.WriteValue(coil.Value);
                        writer.WriteFullEndElement(); // "Value"
                        writer.WriteStartElement("Description");
                        writer.WriteString(coil.Description);
                        writer.WriteFullEndElement(); // "Description"
                        writer.WriteFullEndElement(); // "Coil"

                    }
                    writer.WriteFullEndElement(); // "�oils"

                    // ��������� ���������� �����
                    writer.WriteStartElement("DiscretesInputs");
                    foreach (DiscreteInput dicsreteInput in device.DiscretesInputs)
                    {
                        writer.WriteStartElement("DiscreteInput");
                        writer.WriteAttributeString("Address", dicsreteInput.Address.ToString());
                        writer.WriteStartElement("Value");
                        writer.WriteValue(dicsreteInput.Value);
                        writer.WriteFullEndElement(); // "Value"
                        writer.WriteStartElement("Description");
                        writer.WriteString(dicsreteInput.Description);
                        writer.WriteFullEndElement(); // "Description"
                        writer.WriteFullEndElement(); // "DiscreteInput"

                    }
                    writer.WriteFullEndElement(); // "DiscretesInputs"

                    // ��������� ����� ����������
                    writer.WriteStartElement("Files");
                    foreach (File file in device.Files)
                    {
                        writer.WriteStartElement("File");
                        writer.WriteAttributeString("Number", file.Number.ToString());
                        writer.WriteAttributeString("Description", file.Description);
                        
                        writer.WriteStartElement("Records");
                        foreach (Record record in file.Records)
                        {
                            writer.WriteStartElement("Record");
                            writer.WriteAttributeString("Number", record.Address.ToString());
                            writer.WriteAttributeString("Description", record.Description);

                            writer.WriteStartElement("Value");
                            writer.WriteValue(record.Value);
                            writer.WriteFullEndElement(); // "Value"
                            writer.WriteFullEndElement(); // "Record"
                        }
                        writer.WriteFullEndElement(); // "Records"
                        writer.WriteFullEndElement(); // "File"
                    }
                    writer.WriteFullEndElement(); // "Files"

                    writer.WriteFullEndElement(); // "Device"
                }
                writer.WriteFullEndElement(); // "Devices"
                writer.WriteFullEndElement(); // "Network"
            }
            return;
        }
        /// <summary>
        /// ������ ���������� ���� Modbus �� ����� ������������ ����
        /// </summary>
        /// <param name="pathToXmlFile">���� + �������� ����� ������������ ���� *.xml</param>
        /// <param name="pathToXsdFile">���� + �������� ����� ����� ����� ��� ����� ������������ *.xsd</param>
        /// <returns>���� �������� ������ ������������ null, ���� ������� �������� 
        /// ������� �������� ������������ ���������� ����</returns>
        public static ModbusNetworkControllerSlave Create(String pathToXmlFile, 
            String pathToXsdFile)
        {
            XmlReaderSettings xmlrdsettings;
            XmlReader reader;
            ModbusNetworkControllerSlave network;
            Device device;
            Coil coil;
            DiscreteInput dinput;
            HoldingRegister hregister;
            InputRegister iregister;
            File file;
            Record record;
            List<Device> devices;
            String networkName;

            networkName = String.Empty;
            devices = new List<Device>();

            xmlrdsettings = new XmlReaderSettings();
            xmlrdsettings.IgnoreComments = true;
            xmlrdsettings.IgnoreWhitespace = true;
            xmlrdsettings.Schemas.Add("", pathToXsdFile);
            xmlrdsettings.ValidationType = ValidationType.Schema;
            //xmlrdsettings.ValidationEventHandler +=
            //    new ValidationEventHandler(EventHandler_vr_ValidationEventHandler);
            reader = XmlReader.Create(pathToXmlFile, xmlrdsettings);

            try
            {
                while(reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (!reader.IsEmptyElement)
                        {
                            switch (reader.Name)
                            {
                                case "Network":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            // �������� ������������ ����
                                            networkName = reader.GetAttribute("Name");
                                            //network.NetworkName = reader.GetAttribute("Name");
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "������ � ������ {0}.������� Network �� ����� �������� Name",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                                case "Device":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            device = new Device(Byte.Parse(reader.GetAttribute("Address")));
                                            device.Description = reader.GetAttribute("Description");                                          
                                            device.Status = (Status)Enum.Parse(typeof(Status), 
                                                reader.GetAttribute("Status"));
                                            //network.Devices.Add(device);
                                            devices.Add(device);
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "������ � ������ {0}.������� Device �� ����� �������",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                                case "Coil":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            Boolean value;
                                            UInt16 address = UInt16.Parse(reader.GetAttribute("Address"));

                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Value")
                                                {
                                                    reader.Read();
                                                    value = Boolean.Parse(reader.Value);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "������ � ������ {0}. ������� Coil �� �������� ������� Value",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� Coil �������� ������� ������������� ����",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }

                                            reader.Read(); // EndElement Value
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Description")
                                                {
                                                    reader.Read();
                                                    coil = new Coil(address, value, reader.Value);
                                                    //network.Devices[network.Devices.Count - 1].Coils.Add(coil);
                                                    devices[devices.Count - 1].Coils.Add(coil);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "������ � ������ {0}. ������� Coil �� �������� ������� Description",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� Coil �������� ������� ������������� ����",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "������ � ������ {0}.������� Coil �� ����� �������",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                                case "DiscreteInput":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            Boolean value;
                                            UInt16 address = UInt16.Parse(reader.GetAttribute("Address"));

                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Value")
                                                {
                                                    reader.Read();
                                                    value = Boolean.Parse(reader.Value);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "������ � ������ {0}. ������� DiscreteInput �� �������� ������� Value",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� DiscreteInput �������� ������� ������������� ����",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }

                                            reader.Read(); // EndElement Value
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Description")
                                                {
                                                    reader.Read();
                                                    dinput = new DiscreteInput(address, value, reader.Value);
                                                    
                                                    //network.Devices[network.Devices.Count - 1].DiscretesInputs.Add(dinput);
                                                    devices[devices.Count - 1].DiscretesInputs.Add(dinput);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "������ � ������ {0}. ������� DiscreteInput �� �������� ������� Description",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� DiscreteInput �������� ������� ������������� ����",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "������ � ������ {0}.������� DiscreteInput �� ����� �������",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                                case "HoldingRegister":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            UInt16 value;
                                            UInt16 address = UInt16.Parse(reader.GetAttribute("Address"));

                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Value")
                                                {
                                                    reader.Read();
                                                    value = UInt16.Parse(reader.Value);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "������ � ������ {0}. ������� HoldingRegister �� �������� ������� Value",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� HoldingRegister �������� ������� ������������� ����",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }

                                            reader.Read(); // EndElement Value
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Description")
                                                {
                                                    reader.Read();
                                                    hregister = new HoldingRegister(address, value, reader.Value);
                                                    //network.Devices[network.Devices.Count - 1].HoldingRegisters.Add(hregister);
                                                    devices[devices.Count - 1].HoldingRegisters.Add(hregister);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "������ � ������ {0}. ������� HoldingRegister �� �������� ������� Description",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� HoldingRegister �������� ������� ������������� ����",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "������ � ������ {0}.������� HoldingRegister �� ����� �������",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break; 
                                    }
                                case "InputRegister":
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            UInt16 value;
                                            UInt16 address = UInt16.Parse(reader.GetAttribute("Address"));

                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Value")
                                                {
                                                    reader.Read();
                                                    value = UInt16.Parse(reader.Value);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "������ � ������ {0}. ������� InputRegister �� �������� ������� Value",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� InputRegister �������� ������� ������������� ����",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }

                                            reader.Read(); // EndElement Value
                                            reader.Read();
                                            if (reader.NodeType == XmlNodeType.Element)
                                            {
                                                if (reader.Name == "Description")
                                                {
                                                    reader.Read();
                                                    iregister = new InputRegister(address, value, reader.Value);
                                                    //network.Devices[network.Devices.Count - 1].InputRegisters.Add(iregister);
                                                    devices[devices.Count - 1].InputRegisters.Add(iregister);
                                                }
                                                else
                                                {
                                                    throw new Exception(String.Format(
                                                        "������ � ������ {0}. ������� InputRegister �� �������� ������� Description",
                                                        reader.Settings.LineNumberOffset.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� InputRegister �������� ������� ������������� ����",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "������ � ������ {0}.������� InputRegister �� ����� �������",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break; 
                                    }
                                case "File":
                                    {
                                        file = new File(UInt16.Parse(reader.GetAttribute("Number")),
                                            reader.GetAttribute("Description"));
                                        //network.Devices[network.Devices.Count - 1].Files.Add(file);
                                        devices[devices.Count - 1].Files.Add(file);
                                        break;
                                    }
                                case "Record":
                                    {
                                        UInt16 number = UInt16.Parse(reader.GetAttribute("Number"));
                                        String description = reader.GetAttribute("Description");
                                        // ���������� ��������� �������. ��� ����� ���� Value
                                        reader.Read();
                                        if (reader.NodeType == XmlNodeType.Element)
                                        {
                                            if (reader.Name == "Value")
                                            {
                                                reader.Read();
                                                record = new Record(number,
                                                    UInt16.Parse(reader.Value), description);
                                                //device = network.Devices[network.Devices.Count - 1];
                                                //device.Files[device.Files.Count - 1].Records.Add(record);
                                                device = devices[devices.Count - 1];
                                                device.Files[device.Files.Count - 1].Records.Add(record);
                                            }
                                            else
                                            {
                                                throw new Exception(String.Format(
                                                    "������ � ������ {0}. ������� Record �� �������� ������� Value",
                                                    reader.Settings.LineNumberOffset.ToString()));
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception(String.Format(
                                                "������ � ������ {0}. ������� Record �������� ������� ������������� ����",
                                                reader.Settings.LineNumberOffset.ToString()));
                                        }
                                        break;
                                    }
                            }
                        }
                    } // End of if (reader.NodeType == XmlNodeType.Element)
                } // End of while(reader.Read())
            }
            //catch (XmlException ex)
            //{
            //    if (reader != null)
            //    {
            //        reader.Close();
            //    }

            //    throw;
            //}
            catch //(Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }

                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            // ������ ���� �� ���������� ������
            network = new ModbusNetworkControllerSlave(networkName, null);

            foreach (Device item in devices)
            {
                network.Devices.Add(item);
            }

            return network;
        }
        /// <summary>
        /// ���������� ������� ������ ��� ����������� xml-����� ������������ �
        /// xsd-�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void EventHandler_vr_ValidationEventHandler(
            object sender, ValidationEventArgs e)
        {
            throw new NotImplementedException(
                "����� �� ���������� � ������ ������ ��");
            //return;
        }
        /// <summary>
        /// ���������� ������� ��������� ������ ��������� � ����
        /// </summary>
        private void OnDevicesListWasChanged()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = DevicesListWasChanged;

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
        /// <summary>
        /// ���������� ������� ������������� ������ ��� ������ ����.
        /// </summary>
        private void OnNetworkErrorOccurred(NetworkErrorEventArgs args)
        {
            NetworkErrorOccurredEventHandler handler = NetworkErrorOccurred;

            if (args == null)
            {
                args = new NetworkErrorEventArgs();
            }

            if (handler != null)
            {
                foreach (NetworkErrorOccurredEventHandler singleCast in handler.GetInvocationList())
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // ���������� ������������ ����
            return String.Format("Type={0}; NetworkName={1}; PortName={2}",
                Mode, _NetworkName,
                _Connection == null ? "None" : _Connection.PortName);
            //return base.ToString();
        }
        #endregion

        #region Events
        /// <summary>
        /// ������� ���������� ��� ��������� ������ ��������� ����
        /// </summary>
        public event EventHandler DevicesListWasChanged;
        /// <summary>
        /// ������� ���������� ��� ������������� ������ ������ ����
        /// </summary>
        public event NetworkErrorOccurredEventHandler NetworkErrorOccurred;
        
        #endregion
    }
}
