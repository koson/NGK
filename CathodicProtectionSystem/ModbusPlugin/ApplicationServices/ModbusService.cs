using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Mvp.WinApplication;
using Infrastructure.Api.Services;
using Infrastructure.Api.Managers;
using Modbus.OSIModel.ApplicationLayer;
using Modbus.OSIModel.ApplicationLayer.Slave;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using ModbusPlugin.ApplicationServices.Models;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Master;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using NGK.CAN.DataTypes.DateTimeConvertor;
using NGK.CAN.DataTypes;

namespace NGK.Plugins.ApplicationServices
{
    public class ModbusService: ApplicationServiceBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="networkManager"></param>
        /// <param name="pollingInterval">�������� ���������� CAN-����������, ����</param>
        public ModbusService(IManagers managers)
            :
            base("ModbusService")
        {
            _Managers = managers;
        }

        #endregion

        #region Fields And Properties

        private readonly IManagers _Managers;
        /// <summary>
        /// ���������� modbus ���� � ������ slave
        /// </summary>
        private readonly ModbusNetworkControllerSlave _Network;
        /// <summary>
        /// ComPort
        /// </summary>
        private ComPortSlaveMode _Connection;
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
        private Dictionary<string, int> _CanNetworksTable;
        private List<ModbusServiceContext> _Context;
        private Timer _Timer;

        #endregion

        #region Methods

        public override void Initialize(object context)
        {
            _Context = new List<ModbusServiceContext>();
            // ������ ������������ ���� Modbus ��� ������
            // �������� �������
            _Connection = new ComPortSlaveMode(
                _Managers.ConfigManager.SerialPortBaudRate,
                _Managers.ConfigManager.SerialPortParity,
                _Managers.ConfigManager.SerialPortDataBits,
                _Managers.ConfigManager.SerialPortStopBits);

            _Network = new ModbusNetworkControllerSlave("ModbusNetwork", _Connection);
            ModbusNetworksManager.Instance.Networks.Add(_Network);

            // ������������ �� ������ 
            Init(_Network);
            // �������������� ������
            _Timer = new Timer();
            _Timer.AutoReset = true;
            _Timer.Interval = 1000;
            _Timer.Elapsed += new ElapsedEventHandler(EventHandler_Timer_Elapsed);

            base.Initialize(context);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        /// <summary>
        /// ������ ������������ ���� �� ������ CAN-����
        /// </summary>
        private void Init(ModbusNetworkControllerSlave network)
        {
            network.Devices.Clear();

            List<DeviceBase> canDevices =
                new List<DeviceBase>();
            NgkCanNetworksManager canNetworkManager =
                CAN.ApplicationLayer.Network.Master.NgkCanNetworksManager.Instance;

            // ������ ������� CAN-�����
            _CanNetworksTable = new Dictionary<string, int>(canNetworkManager.Networks.Count);

            foreach (CanNetworkController controller in canNetworkManager.Networks)
            {
                _CanNetworksTable.Add(controller.NetworkName,
                    canNetworkManager.Networks.IndexOf(controller));
            }

            // �������� ������ CAN ��������� �� ���� �����
            foreach (CanNetworkController controller in canNetworkManager.Networks)
            {
                canDevices.AddRange(controller.Devices);
            }

            // ������ slave-���������� � ��������� ��� � Modbus-����
            _DeviceKCCM = CreateKCCM(_Managers.ConfigManager.ModbusAddress);
            _DeviceKCCM.InputRegisters[AdderssHelper.KCCM.InputRegister.SoftwareVersion].Value =
                new NgkProductVersion(new Version(1, 0)).TotalVersion; //TODO
            _DeviceKCCM.InputRegisters[AdderssHelper.KCCM.InputRegister.HardwareVersion].Value =
                new NgkProductVersion(new Version(1, 0)).TotalVersion; //TODO
            _DeviceKCCM.InputRegisters[AdderssHelper.KCCM.InputRegister.TotalDevices].Value =
                System.Convert.ToUInt16(canDevices.Count);
            network.Devices.Add(_DeviceKCCM);

            File mDevice;
            ushort i = 1;

            foreach (NGK.CAN.ApplicationLayer.Network.Devices.DeviceBase device in
                canDevices)
            {
                switch (device.DeviceType)
                {
                    case NGK.CAN.ApplicationLayer.Network.Devices.DeviceType.KIP_BATTERY_POWER_v1:
                        {
                            mDevice = CreateKIP01(); // ������ ������ ���������� ������� ����
                            // �������������� ��� 
                            mDevice.Number = i++;
                            mDevice.Records[KIP9811Address.VisitingCard.HardwareVersion].Value =
                                ((NgkProductVersion)device.GetObject(KIP9811v1.Indexes.hw_version)).TotalVersion;
                            mDevice.Records[KIP9811Address.VisitingCard.SoftwareVersion].Value =
                                ((NgkProductVersion)device.GetObject(KIP9811v1.Indexes.fw_version)).TotalVersion;
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
        private ModbusSlaveDevice CreateKCCM(Byte address)
        {
            ModbusSlaveDevice device = new ModbusSlaveDevice(address);

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

        #endregion
    }
}
