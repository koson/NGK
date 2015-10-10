using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Text;
using System.Configuration;
using System.Timers;
using NGK.CAN.DataTypes;
using NGK.CAN.DataTypes.DateTimeConvertor;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using Modbus.OSIModel.ApplicationLayer;
using Modbus.OSIModel.ApplicationLayer.Slave;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using NGK.CorrosionMonitoringSystem.DL.ModbusAddresses;
using NGK.CorrosionMonitoringSystem.DL.Modbus;
using NGK.CorrosionMonitoringSystem.DL.MatchingAddresses;

namespace NGK.CorrosionMonitoringSystem.DL
{
    /// <summary>
    /// ����� ��� �������� Modbus ����
    /// </summary>
    public class ModbusServiceAdapter
    {
        #region Fields And Properties
        /// <summary>
        /// ���������� modbus ���� � ������ slave
        /// </summary>
        private NetworkController _Network;
        /// <summary>
        /// ComPort
        /// </summary>
        private ComPort _Connection;
        /// <summary>
        /// ���������� modbus ����� �����
        /// </summary>
        private Device _DeviceKCCM;
        /// <summary>
        /// ������������� ����� � ��������
        /// </summary>
        public DateTime SystemTime
        {
            get 
            {
                UInt32 value = 0;
                value = (((UInt32)_DeviceKCCM.HoldingRegisters[0x0000].Value) << 16);
                value |= _DeviceKCCM.HoldingRegisters[0x0001].Value;
                return Unix.ToDateTime(value); 
            }
            set
            {
                UInt32 time = Unix.ToUnixTime(value);
                
                _DeviceKCCM.HoldingRegisters[0x0000].Value = (UInt16)(time >> 16);
                _DeviceKCCM.HoldingRegisters[0x0000].Value = (UInt16)time;
            }
        }
        /// <summary>
        /// ������� ��� �������� ������������
        /// ������������ CAN-����� �������, ��� �������� �� Modbus
        /// </summary>
        private Dictionary<string, int> _CanNetworksTable;
        private List<ModbusAdapterContext> _Context;
        private Timer _Timer;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public ModbusServiceAdapter()
        {
            _Context = new List<ModbusAdapterContext>();
            // ������ ������������ ���� Modbus ��� ������
            // �������� �������
            // �������� ��������� �� ����� ������������ ����������
            NameValueCollection settings = ConfigurationManager.AppSettings;
            string portName = settings["PortName"];
            int baudRate = int.Parse(settings["BaudRate"]);
            Parity parity = (Parity)Enum.Parse(typeof(Parity), 
                settings["Parity"]);
            int dataBits = int.Parse(settings["DataBits"]); 
            StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), 
                settings["StopBits"]);

            _Connection = new ComPort(portName, baudRate, 
                parity, dataBits, stopBits);

            _Network = new NetworkController("ModbusNetwork", _Connection);
            NetworksManager.Instance.Networks.Add(_Network);
            // ������������ �� ������ 
            Init(_Network);
            // ��������� ������
            _Timer = new Timer();
            _Timer.AutoReset = true;
            _Timer.Interval = 1000;
            _Timer.Elapsed += new ElapsedEventHandler(EventHandler_Timer_Elapsed);
            _Timer.Start();
        }
        #endregion

        #region Methods
        /// <summary>
        /// ������ ������������ ���� �� ������ CAN-����
        /// </summary>
        private void Init(NetworkController network)
        {
            network.Devices.Clear();

            List<CAN.ApplicationLayer.Network.Devices.DeviceBase> canDevices =
                new List<NGK.CAN.ApplicationLayer.Network.Devices.DeviceBase>();
            CAN.ApplicationLayer.Network.Master.NetworksManager canNetworkManager =
                CAN.ApplicationLayer.Network.Master.NetworksManager.Instance;

            // ������ ������� CAN-�����
            _CanNetworksTable = new Dictionary<string, int>(canNetworkManager.Networks.Count);

            foreach (CAN.ApplicationLayer.Network.Master.NetworkController controller
                in canNetworkManager.Networks)
            {
                _CanNetworksTable.Add(controller.Description,
                    canNetworkManager.Networks.IndexOf(controller));
            }

            // �������� ������ CAN ��������� �� ���� �����
            foreach (CAN.ApplicationLayer.Network.Master.NetworkController controller
                in canNetworkManager.Networks)
            {
                canDevices.AddRange(controller.Devices);
            }

            // ������ slave-���������� � ��������� ��� � Modbus-����
            _DeviceKCCM = CreateKCCM(1);
            _DeviceKCCM.InputRegisters[KCCM.InputRegister.SoftwareVersion].Value =
                new ProductVersion(new Version(1, 0)).TotalVersion; //TODO
            _DeviceKCCM.InputRegisters[KCCM.InputRegister.HardwareVersion].Value =
                new ProductVersion(new Version(1, 0)).TotalVersion; //TODO
            _DeviceKCCM.InputRegisters[KCCM.InputRegister.TotalDevices].Value =
                System.Convert.ToUInt16(canDevices.Count);
            network.Devices.Add(_DeviceKCCM);

            File mDevice;
            ushort i = 1;

            foreach (NGK.CAN.ApplicationLayer.Network.Devices.DeviceBase device in
                canDevices)
            {
                switch(device.DeviceType) 
                {
                    case NGK.CAN.ApplicationLayer.Network.Devices.DeviceType.KIP_BATTERY_POWER_v1:
                        {
                            mDevice = CreateKIP01(); // ������ ������ ���������� ������� ����
                            // �������������� ��� 
                            mDevice.Number = i++;
                            mDevice.Records[KIP9811Address.VisitingCard.HardwareVersion].Value =
                                ((ProductVersion)device.GetObject(KIP9811v1.Indexes.hw_version)).TotalVersion;
                            mDevice.Records[KIP9811Address.VisitingCard.SoftwareVersion].Value = 
                                ((ProductVersion)device.GetObject(KIP9811v1.Indexes.fw_version)).TotalVersion;
                            mDevice.Records[KIP9811Address.VisitingCard.SerialNumberHigh].Value = 
                                System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number1));
                            mDevice.Records[KIP9811Address.VisitingCard.SerialNumberMiddle].Value =
                                System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number2));
                            mDevice.Records[KIP9811Address.VisitingCard.SerialNumberLow].Value =
                                System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number3));
                            mDevice.Records[KIP9811Address.VisitingCard.CRC16].Value = 0; //TODO (������� ������� CRC16)
                            mDevice.Records[KIP9811Address.ServiceInformation.NetworkNumber].Value = 
                                System.Convert.ToUInt16(_CanNetworksTable[device.Network.Description]);
                            mDevice.Records[KIP9811Address.ServiceInformation.NetwrokAddress].Value =
                                System.Convert.ToUInt16(device.NodeId);
                            mDevice.Records[KIP9811Address.ServiceInformation.ConectionStatus].Value = 0; // 0-����� 1-������
                            break;
                        }
                    case NGK.CAN.ApplicationLayer.Network.Devices.DeviceType.KIP_MAIN_POWERED_v1:
                        {
                            throw new NotImplementedException();
                        }
                    default:
                        {
                            throw new NotSupportedException();
                        }
                }
                // ��������� ����������
                _DeviceKCCM.Files.Add(mDevice);
                // ������ ��� ���� �������� ��� ������� ����������
                _Context.Add(new ModbusAdapterContext(device, mDevice));
            }
        }
        /// <summary>
        /// ������ modbus slave-���������� ����� 
        /// </summary>
        /// <param name="address">������� ����� ����������</param>
        /// <returns></returns>
        private Device CreateKCCM(Byte address)
        {
            Device device = new Device(address);

            device.Description = "���� �����";

            // �������������� ������ ����������
            device.InputRegisters.Add(new InputRegister(0x0000, 0x2620, "��� ����������"));
            device.InputRegisters.Add(new InputRegister(0x0001,
                (new ProductVersion(new Version(1, 0))).TotalVersion,
                "������ ��"));
            device.InputRegisters.Add(new InputRegister(0x0002,
                (new ProductVersion(new Version(1, 0))).TotalVersion,
                "������ ���������� �����"));
            device.InputRegisters.Add(new InputRegister(0x0003, 0, "�������� �����: High"));
            device.InputRegisters.Add(new InputRegister(0x0004, 0, "�������� �����: Middle"));
            device.InputRegisters.Add(new InputRegister(0x0005, 0, "�������� �����: Low"));
            device.InputRegisters.Add(new InputRegister(0x0006, 0, "�RC16")); // TODO
            device.InputRegisters.Add(new InputRegister(0x0007, 0, "��� �������������")); // TODO
            device.InputRegisters.Add(new InputRegister(0x0008, 0, "���������� ��������� � �������"));
            device.InputRegisters.Add(new InputRegister(0x0009, 0, "C������ ������������� �������: High")); // TODO
            device.InputRegisters.Add(new InputRegister(0x000A, 0, "C������ ������������� �������: Low")); // TODO
            device.InputRegisters.Add(new InputRegister(0x000B, 0, "���������� �������, �")); // TODO
            device.HoldingRegisters.Add(new HoldingRegister(0x0000, 0, "��������� �����: High"));
            device.HoldingRegisters.Add(new HoldingRegister(0x0001, 0, "��������� �����: Low"));
            return device;
        }
        /// <summary>
        /// ������ modbus-���� ���������� ��� ��(�)-00 
        /// </summary>
        /// <returns>Modbus ����</returns>
        private File CreateKIP00()
        {
            File file = new File();

            // ������ �������� ����� ����������
            file.Records.Add(new Record(0x0000, 0x2652, "��� ����������"));
            file.Records.Add(new Record(0x0001, 0, "������ ��"));
            file.Records.Add(new Record(0x0002, 0, "������ ����������"));
            file.Records.Add(new Record(0x0003, 0, "�������� �����: High"));
            file.Records.Add(new Record(0x0004, 0, "�������� �����: Middle"));
            file.Records.Add(new Record(0x0005, 0, "�������� �����: Low"));
            file.Records.Add(new Record(0x0006, 0, "CRC16"));
            file.Records.Add(new Record(0x0007, 0, "��� �������������"));

            // ��������� ��������� ���������� 

            //(2 - ���������� ���� CAN)
            file.Records.Add(new Record(0x0008, 2, "��� ����"));
            // �� ������������ - ������ 0
            file.Records.Add(new Record(0x0009, 0, "����� ����"));
            file.Records.Add(new Record(0x000A, 1, "������� �����"));
            file.Records.Add(new Record(0x000A, 1, "������� ����� � �����������"));

            // ��������� ������ ����������� ��� �������� ������� CAN ����������
            file.Records.Add(new Record(0x0001, 0, "������ ��"));
            file.Records.Add(new Record(0x0002, 0, "������ ����������"));
            file.Records.Add(new Record(0x0003, 0, "�������� �����: High"));
            file.Records.Add(new Record(0x0004, 0, "�������� �����: Middle"));
            file.Records.Add(new Record(0x0005, 0, "�������� �����: Low"));
            file.Records.Add(new Record(0x0006, 0, "CRC16"));
            file.Records.Add(new Record(0x0007, 0, "��� �������������"));
            
            // ��������� ������ ����������� ��� �������� ������� CAN ����������
            file.Records.Add(new Record(0x000C, 0, "������� ������"));
            file.Records.Add(new Record(0x000D, 0, "������� ������ �����������"));
            file.Records.Add(new Record(0x000E, 0, "��������� ����������"));
            file.Records.Add(new Record(0x000F, 0, "�������� ���������"));
            file.Records.Add(new Record(0x0010, 0, "��������������� ��������� ���������� ������������"));
            file.Records.Add(new Record(0x0011, 0, "��� �������� ������ � ���-�� ������� ������� ������-��� ���������� �� ������� �����"));
            file.Records.Add(new Record(0x0012, 0, "��������� ���������� ���������� �� �����������"));
            file.Records.Add(new Record(0x0013, 0, "��� ����������� ��������-�������� ���������"));
            file.Records.Add(new Record(0x0014, 0, "��������� ���� ����������� ���������������� ���������"));
            file.Records.Add(new Record(0x0015, 0, "��� �������������� ������ 1"));
            file.Records.Add(new Record(0x0016, 0, "��� �������������� ������ 2"));
            file.Records.Add(new Record(0x0017, 0, "������� �������� ������� ��� � ���������� �������"));
            file.Records.Add(new Record(0x0018, 0, "�������� �������� ������� ��� � ���������� �������"));
            file.Records.Add(new Record(0x0019, 0, "��������� �������"));
            file.Records.Add(new Record(0x001A, 0, "��������� �������� ������� �1� �������� �������� 30,0-100,0 ��"));
            file.Records.Add(new Record(0x001B, 0, "��������� �������� ������� �2� �������� �������� 30,0-100,0 ��"));
            file.Records.Add(new Record(0x001C, 0, "��������� �������� ������� �3� �������� �������� 30,0-100,0 ��"));
            file.Records.Add(new Record(0x001D, 0, "��� ��������� �� ����������"));
            file.Records.Add(new Record(0x001E, 0, "��� ��������� �� ����������"));
            file.Records.Add(new Record(0x001F, 0, "��������� ���� ��������� �� �����������"));
            file.Records.Add(new Record(0x0020, 0, "��������� ���� ��������� �� �����������"));
            file.Records.Add(new Record(0x0021, 0xFFFF, "���������������"));
            file.Records.Add(new Record(0x0022, 0, "���������� ����������� �������� �������"));
            file.Records.Add(new Record(0x0023, 0, "���������������"));
            file.Records.Add(new Record(0x0024, 0, "������� ���������������� ��������� (��)"));
            file.Records.Add(new Record(0x0025, 0, "������ ��������� � �������� ����������: High"));
            file.Records.Add(new Record(0x0026, 0, "������ ��������� � �������� ����������: Low"));
            file.Records.Add(new Record(0x0027, 0xFFFF, "���������������"));
            file.Records.Add(new Record(0x0028, 0, "������ ������ �������"));
            file.Records.Add(new Record(0x0029, 0, "������ ������ �������� ��������"));
            file.Records.Add(new Record(0x002A, 0, "������ ������ ������������-�� ������ 1 4-20 ��"));
            file.Records.Add(new Record(0x002B, 0, "������ ������ ������������-�� ������ 2 4-20 ��"));
            file.Records.Add(new Record(0x002C, 0, "����������� ��� �������� ����� (�)"));
            file.Records.Add(new Record(0x002D, 0, "���� ���������� ������ ������ ��������� ���������������� ���������� ���������� ������������."));
            file.Records.Add(new Record(0x002E, 0, "���� ���������� ������ ������ ��������� ��������� ����������."));
            file.Records.Add(new Record(0x002F, 0, "���� ���������� ������ ������ ��������� ���� �������� ������ � ����� ������� ������� ��������� ���������� �� ������� �����."));
            file.Records.Add(new Record(0x0030, 0, "���� ���������� ������ ������ ���� ����������� ���������������� ���������"));
            file.Records.Add(new Record(0x0031, 0, "���� ���������� ������ ������ ��������� ���������� ����������� ���������� �� �����������"));
            file.Records.Add(new Record(0x0032, 0, "���� ���������� �������� ����� ���������"));
            file.Records.Add(new Record(0x0033, 0, "���� ���������� ������ ������ ��������� ���� ��������� �� �����������"));
            file.Records.Add(new Record(0x0034, 0, "���� ���������� ������ ������ ��������� ���� ��������� �� �����������"));
            file.Records.Add(new Record(0x0035, 0, "���������� ��� ���������� �������� PDO"));
            file.Records.Add(new Record(0x0036, 0, "������� ����� ����������: High"));
            file.Records.Add(new Record(0x0037, 0, "������� ����� ����������: Low"));
            return file;
        }
        /// <summary>
        /// ������ modbus-���� ���������� ��� ��(�)-01 
        /// </summary>
        /// <returns>Modbus ����</returns>
        private File CreateKIP01() 
        {
            File file = new File();

            // ������ �������� ����� ����������
            file.Records.Add(new Record(0x0000, 0x2653, "��� ����������"));
            file.Records.Add(new Record(0x0001, 0, "������ ��"));
            file.Records.Add(new Record(0x0002, 0, "������ ����������"));
            file.Records.Add(new Record(0x0003, 0, "�������� �����: High"));
            file.Records.Add(new Record(0x0004, 0, "�������� �����: Middle"));
            file.Records.Add(new Record(0x0005, 0, "�������� �����: Low"));
            file.Records.Add(new Record(0x0006, 0, "CRC16"));
            file.Records.Add(new Record(0x0007, 0, "��� �������������"));
            
            // ��������� ��������� ���������� 

            //(2 - ���������� ���� CAN)
            file.Records.Add(new Record(0x0008, 2, "��� ����"));
            // �� ������������ - ������ 0
            file.Records.Add(new Record(0x0009, 0, "����� ����"));
            file.Records.Add(new Record(0x000A, 1, "������� �����"));
            file.Records.Add(new Record(0x000B, 1, "������� ����� � �����������"));
            
            // ��������� ������ ����������� ��� �������� ������� CAN ����������

            file.Records.Add(new Record(0x000C, 0, "������� ������"));
            file.Records.Add(new Record(0x000D, 0, "������� ������ �����������"));
            file.Records.Add(new Record(0x000E, 0, "��������� ����������"));
            file.Records.Add(new Record(0x000F, 0, "�������� ���������"));
            file.Records.Add(new Record(0x0010, 0, "��������������� ��������� ���������� ������������"));
            file.Records.Add(new Record(0x0011, 0, "��� �������� ������ � ���-�� ������� ������� ������-��� ���������� �� ������� �����"));
            file.Records.Add(new Record(0x0012, 0, "��������� ���������� ���������� �� �����������"));
            file.Records.Add(new Record(0x0013, 0, "��� ����������� ���������������� ���������"));
            file.Records.Add(new Record(0x0014, 0, "��������� ���� ����������� ���������������� ���������"));
            file.Records.Add(new Record(0x0015, 0, "��� �������������� ������ 1"));
            file.Records.Add(new Record(0x0016, 0, "��� �������������� ������ 2"));
            file.Records.Add(new Record(0x0017, 0, "������� �������� ������� ��� � ���������� �������"));
            file.Records.Add(new Record(0x0018, 0, "�������� �������� ������� ��� � ���������� �������"));
            file.Records.Add(new Record(0x0019, 0, "��������� �������"));
            file.Records.Add(new Record(0x001A, 0, "��������� �������� ������� �1� �������� �������� 30,0-100,0 ��"));
            file.Records.Add(new Record(0x001B, 0, "��������� �������� ������� �2� �������� �������� 30,0-100,0 ��"));
            file.Records.Add(new Record(0x001C, 0, "��������� �������� ������� �3� �������� �������� 30,0-100,0 ��"));
            file.Records.Add(new Record(0x001D, 0, "��� ��������� �� ����������"));
            file.Records.Add(new Record(0x001E, 0, "��� ��������� �� ����������"));
            file.Records.Add(new Record(0x001F, 0, "��������� ���� ��������� �� �����������"));
            file.Records.Add(new Record(0x0020, 0, "��������� ���� ��������� �� �����������"));
            file.Records.Add(new Record(0x0021, 0xFFFF, "���������������"));
            file.Records.Add(new Record(0x0022, 0, "���������� ����������� �������� �������"));
            file.Records.Add(new Record(0x0023, 0, "����������� ����������� ������� ��(�)"));
            file.Records.Add(new Record(0x0024, 0, "������� ���������������� ��������� (��)"));
            file.Records.Add(new Record(0x0025, 0, "������ ��������� � �������� ����������: High"));
            file.Records.Add(new Record(0x0026, 0, "������ ��������� � �������� ����������: Low"));
            file.Records.Add(new Record(0x0027, 0xFFFF, "���������������"));
            file.Records.Add(new Record(0x0028, 0, "������ ������ �������"));
            file.Records.Add(new Record(0x0029, 0, "������ ������ �������� ��������"));
            file.Records.Add(new Record(0x002A, 0, "������ ������ �������������� ������ 1 4-20 ��"));
            file.Records.Add(new Record(0x002B, 0, "������ ������ �������������� ������ 2 4-20 ��"));
            file.Records.Add(new Record(0x002C, 0, "����������� ��� �������� ����� (�)"));
            file.Records.Add(new Record(0x002D, 0, "���� ���������� ������ ������ ��������� ���������������� ���������� ���������� ������������."));
            file.Records.Add(new Record(0x002E, 0, "���� ���������� ������ ������ ��������� ��������� ����������."));
            file.Records.Add(new Record(0x002F, 0, "���� ���������� ������ ������ ��������� ���� �������� ������ � ����� ������� ������� ��������� ���������� �� ������� �����."));
            file.Records.Add(new Record(0x0030, 0, "���� ���������� ������ ������ ���� ����������� ���������������� ���������"));
            file.Records.Add(new Record(0x0031, 0, "���� ���������� ������ ������ ��������� ���������� ����������� ���������� �� �����������"));
            file.Records.Add(new Record(0x0032, 0, "���� ���������� �������� ����� ���������"));
            file.Records.Add(new Record(0x0033, 0, "���� ���������� ������ ������ ��������� ���� ��������� �� �����������"));
            file.Records.Add(new Record(0x0034, 0, "���� ���������� ������ ������ ��������� ���� ��������� �� �����������"));
            file.Records.Add(new Record(0x0035, 0, "���������� ��� ���������� �������� PDO"));
            file.Records.Add(new Record(0x0036, 0, "������� ����� ����������: High"));
            file.Records.Add(new Record(0x0037, 0, "������� ����� ����������: Low"));

            return file;
        }
        /// <summary>
        /// ��������� ������ modbus-����� �� ������ ����������� CAN-����������
        /// </summary>
        /// <param name="modbusDevice"></param>
        /// <param name="canDevice"></param>
        private static void UdateDevice(
            File modbusDevice, NGK.CAN.ApplicationLayer.Network.Devices.DeviceBase canDevice)
        {
            NGK.CAN.ApplicationLayer.Network.Devices.DeviceType type =
                (NGK.CAN.ApplicationLayer.Network.Devices.DeviceType)modbusDevice.Records[ModbusAddresses
                .ModbusVisitingCard.VisitingCard.DeviceType].Value;
            switch (type)
            {
                case NGK.CAN.ApplicationLayer.Network.Devices.DeviceType.KIP_BATTERY_POWER_v1:
                    {
                        RemappingTableKip9811.Copy(modbusDevice, canDevice);
                        break;
                    }
                default: { throw new NotSupportedException(); }
            }
        }
        /// <summary>
        /// ����������� � ��������� ������. ��������� ������-���������� ������� ��
        /// �AN-����������
        /// </summary>
        private void DoWork()
        { 
        }
        /// <summary>
        /// ���������� ������������� �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NGK.CAN.ApplicationLayer.Network.Devices.DeviceBase device;
            File modbusDevice;
            NGK.CAN.ApplicationLayer.Network.Master.NetworksManager manager =
                NGK.CAN.ApplicationLayer.Network.Master.NetworksManager.Instance;

            // ��������� ��� ����������
            foreach (ModbusAdapterContext context in _Context)
            {
                // �������� CAN-����������
                //UInt32 x = context.CanDevice.NetworkId;
                //Byte y = context.CanDevice.NodeId;
                //device = manager.Networks[x].Devices[y];
                device = manager.Networks[context.CanDevice.NetworkId]
                    .Devices[context.CanDevice.NodeId];
                modbusDevice = _DeviceKCCM.Files[context.ModbusDevice.FileNumber];
                UdateDevice(modbusDevice, device);
            }
        }
        /// <summary>
        /// ��������� ������ ���� � ���������� �����
        /// </summary>
        public void Start()
        {
            // ��������� � ������
            _Network.Start();
            _DeviceKCCM.Start();
        }
        public void Stop()
        {
            // ������������� � ������
            _Network.Stop();
        }
        #endregion
    }
}
