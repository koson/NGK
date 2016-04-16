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


namespace NGK.CorrosionMonitoringSystem.Services
{
    public class ModbusSystemInformationNetworkService : IModbusSystemInformationNetworkService
    {
        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="pollingInterval">Интервал обновления Modbus-устройства, мсек</param>
        public ModbusSystemInformationNetworkService(IApplicationController application,
            IManagers managers, double pollingInterval)
        {
            _Application = application;
            _Managers = managers;
            _Context = new List<ModbusAdapterContext>();

            _Timer = new Timer(pollingInterval);
            _Timer.AutoReset = true;
            _Timer.Elapsed += new ElapsedEventHandler(EventHandler_Timer_Elapsed);
            _Timer.Stop();
        }

        #endregion

        #region Fields And Properties

        public Status Status
        {
            get
            {
                return _Timer.Enabled ? Status.Running : Status.Stopped;
            }
            set
            {
                switch (value)
                {
                    case Status.Running: { Start(); break; }
                    case Status.Stopped: { Stop(); break; }
                    default: { throw new NotSupportedException(); }
                }
            }
        }

        IApplicationController _Application;
        IManagers _Managers;
        /// <summary>
        /// Контроллер modbus сеть в режиме slave
        /// </summary>
        public ModbusNetworkControllerSlave _Network;
        /// <summary>
        /// Устройство modbus блока КССМУ
        /// </summary>
        private Device _DeviceKCCM;
        /// <summary>
        /// Устанавливаем время в регистре
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
        /// Таблица для хранения соответствия
        /// Наименований CAN-сетей номерам, для передачи по Modbus
        /// </summary>
        private Dictionary<string, int> _CanNetworksTable;
        private List<ModbusAdapterContext> _Context;
        private Timer _Timer;
        
        #endregion

        #region Methods

        public void Start()
        {
            if (_Network == null)
                throw new InvalidOperationException("Попытка запуска неинициализированного сервиса");

            if (Status == Status.Running)
                return;

            _Timer.Start();
            OnStatusWasChanged();
        }

        public void Stop()
        {
            if (Status == Status.Stopped)
                return;

            _Timer.Stop();
            OnStatusWasChanged();
        }

        public void Suspend()
        {
            throw new NotSupportedException();
        }

        public void Dispose() {}

        public void Initialize()
        {
            throw new NotImplementedException();

            _Network = (ModbusNetworkControllerSlave)ModbusNetworksManager.Instance
                .Networks[_Managers.ConfigManager.ModbusSystemInfoNetworkName];

            _Network.Devices.Clear();
            
            //_Managers.CanNetworkService.Devices

            //List<CAN.ApplicationLayer.Network.Devices.DeviceBase> canDevices =
            //    new List<NGK.CAN.ApplicationLayer.Network.Devices.DeviceBase>();
            //CAN.ApplicationLayer.Network.Master.NetworksManager canNetworkManager =
            //    CAN.ApplicationLayer.Network.Master.NetworksManager.Instance;

            //// Создаём таблицу CAN-сетей
            //_CanNetworksTable = new Dictionary<string, int>(canNetworkManager.Networks.Count);

            //foreach (CAN.ApplicationLayer.Network.Master.NetworkController controller
            //    in canNetworkManager.Networks)
            //{
            //    _CanNetworksTable.Add(controller.Description,
            //        canNetworkManager.Networks.IndexOf(controller));
            //}

            //// Получаем список CAN устройств из всех сетей
            //foreach (CAN.ApplicationLayer.Network.Master.NetworkController controller
            //    in canNetworkManager.Networks)
            //{
            //    canDevices.AddRange(controller.Devices);
            //}

            // Создаём slave-устройства и добавляем его в Modbus-сеть
            //_DeviceKCCM = CreateKCCM(1);
            _DeviceKCCM.InputRegisters[KCCM.InputRegister.SoftwareVersion].Value =
                new NgkProductVersion(_Managers.SoftwareVersion).TotalVersion;
            _DeviceKCCM.InputRegisters[KCCM.InputRegister.HardwareVersion].Value =
                new NgkProductVersion(_Managers.HardwareVersion).TotalVersion; //TODO
            //_DeviceKCCM.InputRegisters[KCCM.InputRegister.TotalDevices].Value =
            //    System.Convert.ToUInt16(canDevices.Count);
            //network.Devices.Add(_DeviceKCCM);

            //File mDevice;
            //ushort i = 1;

            //foreach (NGK.CAN.ApplicationLayer.Network.Devices.DeviceBase device in
            //    canDevices)
            //{
            //    switch (device.DeviceType)
            //    {
            //        case NGK.CAN.ApplicationLayer.Network.Devices.DeviceType.KIP_BATTERY_POWER_v1:
            //            {
            //                mDevice = CreateKIP01(); // Создаём пустое устройство нужного типа
            //                // Инициализируем его 
            //                mDevice.Number = i++;
            //                mDevice.Records[KIP9811Address.VisitingCard.HardwareVersion].Value =
            //                    ((ProductVersion)device.GetObject(KIP9811v1.Indexes.hw_version)).TotalVersion;
            //                mDevice.Records[KIP9811Address.VisitingCard.SoftwareVersion].Value =
            //                    ((ProductVersion)device.GetObject(KIP9811v1.Indexes.fw_version)).TotalVersion;
            //                mDevice.Records[KIP9811Address.VisitingCard.SerialNumberHigh].Value =
            //                    System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number1));
            //                mDevice.Records[KIP9811Address.VisitingCard.SerialNumberMiddle].Value =
            //                    System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number2));
            //                mDevice.Records[KIP9811Address.VisitingCard.SerialNumberLow].Value =
            //                    System.Convert.ToUInt16(device.GetObject(KIP9811v1.Indexes.serial_number3));
            //                mDevice.Records[KIP9811Address.VisitingCard.CRC16].Value = 0; //TODO (сделать рассчёт CRC16)
            //                mDevice.Records[KIP9811Address.ServiceInformation.NetworkNumber].Value =
            //                    System.Convert.ToUInt16(_CanNetworksTable[device.Network.Description]);
            //                mDevice.Records[KIP9811Address.ServiceInformation.NetwrokAddress].Value =
            //                    System.Convert.ToUInt16(device.NodeId);
            //                mDevice.Records[KIP9811Address.ServiceInformation.ConectionStatus].Value = 0; // 0-норма 1-ошибка
            //                break;
            //            }
            //        case NGK.CAN.ApplicationLayer.Network.Devices.DeviceType.KIP_MAIN_POWERED_v1:
            //            {
            //                throw new NotImplementedException();
            //            }
            //        default:
            //            {
            //                throw new NotSupportedException();
            //            }
            //    }
            //    // Добавляем устройство
            //    _DeviceKCCM.Files.Add(mDevice);
            //    // Создаём для него контекст для данного устройства
            //    _Context.Add(new ModbusAdapterContext(device, mDevice));
            //}

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
            // Обновляем данные из устройств
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        public event EventHandler StatusWasChanged;

        #endregion
    }
}
