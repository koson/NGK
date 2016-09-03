using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;
using System.Timers;
using Mvp.WinApplication;
using Modbus.OSIModel.ApplicationLayer;
using Modbus.OSIModel.ApplicationLayer.Slave;
using Modbus.OSIModel.DataLinkLayer.Slave;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using NGK.CAN.DataTypes;
using NGK.CAN.DataTypes.DateTimeConvertor;
using NGK.CorrosionMonitoringSystem.Managers;
using NGK.CorrosionMonitoringSystem.Models.Modbus;
using NGK.CorrosionMonitoringSystem.DL;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using NGK.CorrosionMonitoringSystem.Models;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Master;
using Infrastructure.API.Managers;


namespace NGK.CorrosionMonitoringSystem.Services
{
    public class SystemInformationModbusNetworkService : ApplicationServiceBase,
        ISystemInformationModbusNetworkService, IDisposable
    {
        #region Constructors      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="pollingInterval">�������� ���������� Modbus-����������, ����</param>
        public SystemInformationModbusNetworkService(string serviceName,
            IModbusNetworkControllerSlave network, 
            byte networkAddress,  double pollingInterval, IManagers managers):
            base(serviceName)
        {
            _Managers = managers;
            _Network = network;
            Address = networkAddress;

            _Timer = new Timer(pollingInterval);
            _Timer.AutoReset = true;
            _Timer.Elapsed += new ElapsedEventHandler(EventHandler_Timer_Elapsed);
            _Timer.Stop();
        }

        #endregion

        #region Fields And Properties

        private IManagers _Managers; 
        /// <summary>
        /// ���������� modbus ���� � ������ slave
        /// </summary>
        private IModbusNetworkControllerSlave _Network;
        /// <summary>
        /// ���������� modbus ����� �����
        /// </summary>
        private ModbusSlaveDevice _DeviceKCCM;
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
        Dictionary<string, int> _CanNetworksTable;
        List<ModbusServiceContext> _Context;
        Timer _Timer;

        Byte _Address;
        /// <summary>
        /// ������� ����� ����������
        /// </summary>
        public byte Address 
        {
            get { return _Address; }
            set 
            {
                if ((value > ModbusSlaveDevice.MaxAddress) ||
                    (value < ModbusSlaveDevice.MinAddress))
                {
                    throw new ArgumentOutOfRangeException("Address", 
                        "������� ���������� ������������ �������� ������ ������");
                }
                
                _Address = value;
            } 
        }

        #endregion

        #region Methods

        public override void OnStarting()
        {
            _Timer.Start();
        }

        public override void OnStopping()
        {
            _Timer.Stop();
        }

        public override void Suspend()
        {
            throw new NotSupportedException();
        }

        public override void Dispose() 
        {
            if (_Timer != null)
                _Timer.Dispose();

            base.Dispose();
        }

        public override void Initialize(object context)
        {
            _Network.Devices.Clear();
            
            // ������ slave-���������� � ��������� ��� � Modbus-����
            _DeviceKCCM = CreateKCCM(Address);
            _DeviceKCCM.InputRegisters[KCCM.InputRegister.SoftwareVersion].Value =
                new NgkProductVersion(_Managers.SoftwareVersion).TotalVersion;
            _DeviceKCCM.InputRegisters[KCCM.InputRegister.HardwareVersion].Value =
                new NgkProductVersion(_Managers.HardwareVersion).TotalVersion;
            _DeviceKCCM.InputRegisters[KCCM.InputRegister.TotalDevices].Value =
                System.Convert.ToUInt16(_Managers.CanNetworkService.Devices.Count);
            _Network.Devices.Add(_DeviceKCCM);

            List<DeviceBase> allDevices = new List<DeviceBase>();

            _CanNetworksTable = new Dictionary<string, int>();

            for(int i=0; i < NgkCanNetworksManager.Instance.Networks.Count; i++)
            {
                allDevices.AddRange(NgkCanNetworksManager.Instance.Networks[i].Devices);
                _CanNetworksTable.Add(NgkCanNetworksManager.Instance.Networks[i].NetworkName, i + 1);
            }

            _Context = new List<ModbusServiceContext>(allDevices.Count);

            ushort number = 0;
            File modbusDevice;

            foreach (DeviceBase device in allDevices)
            {
                modbusDevice = CreateDevice(device, ++number);
                // ��������� ����������
                _DeviceKCCM.Files.Add(modbusDevice);
                // ������ ��� ���� �������� ��� ������� ����������
                _Context.Add(new ModbusServiceContext(device, modbusDevice)); 
            }

            base.Initialize(context);
        }
        /// <summary>
        /// ������ modbus slave-���������� ����� 
        /// </summary>
        /// <param name="address">������� ����� ����������</param>
        /// <returns></returns>
        ModbusSlaveDevice CreateKCCM(Byte address)
        {
            ModbusSlaveDevice device = new ModbusSlaveDevice(address);

            device.Description = "���� �����";

            // �������������� ������ ����������
            device.InputRegisters.Add(new InputRegister(0x0000, 0x2620, "��� ����������"));
            device.InputRegisters.Add(new InputRegister(0x0001,
                (new NgkProductVersion(new Version(1, 0))).TotalVersion,
                "������ ��"));
            device.InputRegisters.Add(new InputRegister(0x0002,
                (new NgkProductVersion(new Version(1, 0))).TotalVersion,
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
        /// ������ modbus ����������
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        File CreateDevice(DeviceBase device, ushort fileNumber)
        {
            File modbusDevice;

            switch (device.DeviceType)
            {
                case NGK.CAN.ApplicationLayer.Network.Devices.DeviceType.KIP_BATTERY_POWER_v1:
                    {
                        modbusDevice = CreateKIP01();
                        ICanDeviceProfile profile = CanDevicePrototype.GetProfile(device.DeviceType);

                        // ��������������
                        modbusDevice.Number = fileNumber;
                        modbusDevice.Records[KIP9811AddressSpaceHelper.VisitingCard.HardwareVersion].Value =
                            (new NgkProductVersion(profile.HardwareVersion)).TotalVersion;
                        modbusDevice.Records[KIP9811AddressSpaceHelper.VisitingCard.SoftwareVersion].Value =
                            (new NgkProductVersion(profile.SoftwareVersion)).TotalVersion;
                        modbusDevice.Records[KIP9811AddressSpaceHelper.VisitingCard.SerialNumberHigh].Value =
                            System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number1));
                        modbusDevice.Records[KIP9811AddressSpaceHelper.VisitingCard.SerialNumberMiddle].Value =
                            System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number2));
                        modbusDevice.Records[KIP9811AddressSpaceHelper.VisitingCard.SerialNumberLow].Value =
                            System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number3));
                        modbusDevice.Records[KIP9811AddressSpaceHelper.VisitingCard.CRC16].Value = 0; //TODO (������� ������� CRC16)
                        modbusDevice.Records[KIP9811AddressSpaceHelper.ServiceInformation.NetworkNumber].Value =
                            System.Convert.ToUInt16(_CanNetworksTable[device.Network.NetworkName]);
                        modbusDevice.Records[KIP9811AddressSpaceHelper.ServiceInformation.NetwrokAddress].Value =
                            System.Convert.ToUInt16(device.NodeId);
                        modbusDevice.Records[KIP9811AddressSpaceHelper.ServiceInformation.ConectionStatus].Value = 0; // 0-����� 1-������
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
            return modbusDevice;
        }
        /// <summary>
        /// ������ modbus-���� ���������� ��� ��(�)-00 
        /// </summary>
        /// <returns>Modbus ����</returns>
        File CreateKIP00()
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
        File CreateKIP01()
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
        private static void UdateDevice(File modbusDevice, DeviceBase canDevice)
        {
            DeviceType type =
                (DeviceType)modbusDevice.Records[
                KIP9811AddressSpaceHelper.VisitingCard.DeviceType].Value;
            switch (type)
            {
                case DeviceType.KIP_BATTERY_POWER_v1:
                    {
                        RemappingTableKip9811.Copy(modbusDevice, canDevice);
                        break;
                    }
                default: { throw new NotSupportedException(); }
            }
        }
        void OnStatusWasChanged()
        {
            if (StatusWasChanged != null)
                StatusWasChanged(this, new EventArgs());
        }

        #endregion

        #region Event Handlers

        void EventHandler_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DeviceBase device;
            File modbusDevice;
            NgkCanNetworksManager manager = NgkCanNetworksManager.Instance;

            // ��������� ��� ����������
            foreach (ModbusServiceContext context in _Context)
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

        #endregion

        #region Events

        public event EventHandler StatusWasChanged;

        #endregion
    }
}
