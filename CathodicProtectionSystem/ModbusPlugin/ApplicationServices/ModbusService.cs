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
        /// <param name="pollingInterval">Интервал обновления CAN-устройства, мсек</param>
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
        /// Контроллер modbus сеть в режиме slave
        /// </summary>
        private readonly ModbusNetworkControllerSlave _Network;
        /// <summary>
        /// ComPort
        /// </summary>
        private ComPortSlaveMode _Connection;
        /// <summary>
        /// Устройство modbus блока КССМУ
        /// </summary>
        private ModbusSlaveDevice _DeviceKCCM;
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
        private List<ModbusServiceContext> _Context;
        private Timer _Timer;

        #endregion

        #region Methods

        public override void Initialize(object context)
        {
            _Context = new List<ModbusServiceContext>();
            // Создаём единственную сеть Modbus для работы
            // сверхним уровнем
            _Connection = new ComPortSlaveMode(
                _Managers.ConfigManager.SerialPortBaudRate,
                _Managers.ConfigManager.SerialPortParity,
                _Managers.ConfigManager.SerialPortDataBits,
                _Managers.ConfigManager.SerialPortStopBits);

            _Network = new ModbusNetworkControllerSlave("ModbusNetwork", _Connection);
            ModbusNetworksManager.Instance.Networks.Add(_Network);

            // Конфигурацию на основе 
            Init(_Network);
            // Инициализируем таймер
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
        /// Создаёт конфигурацию сети на основе CAN-сети
        /// </summary>
        private void Init(ModbusNetworkControllerSlave network)
        {
            network.Devices.Clear();

            List<DeviceBase> canDevices =
                new List<DeviceBase>();
            NgkCanNetworksManager canNetworkManager =
                CAN.ApplicationLayer.Network.Master.NgkCanNetworksManager.Instance;

            // Создаём таблицу CAN-сетей
            _CanNetworksTable = new Dictionary<string, int>(canNetworkManager.Networks.Count);

            foreach (CanNetworkController controller in canNetworkManager.Networks)
            {
                _CanNetworksTable.Add(controller.NetworkName,
                    canNetworkManager.Networks.IndexOf(controller));
            }

            // Получаем список CAN устройств из всех сетей
            foreach (CanNetworkController controller in canNetworkManager.Networks)
            {
                canDevices.AddRange(controller.Devices);
            }

            // Создаём slave-устройства и добавляем его в Modbus-сеть
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
                            mDevice = CreateKIP01(); // Создаём пустое устройство нужного типа
                            // Инициализируем его 
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
                            mDevice.Records[KIP9811Address.VisitingCard.CRC16].Value = 0; //TODO (сделать рассчёт CRC16)
                            mDevice.Records[KIP9811Address.ServiceInformation.NetworkNumber].Value =
                                System.Convert.ToUInt16(_CanNetworksTable[device.Network.Description]);
                            mDevice.Records[KIP9811Address.ServiceInformation.NetwrokAddress].Value =
                                System.Convert.ToUInt16(device.NodeId);
                            mDevice.Records[KIP9811Address.ServiceInformation.ConectionStatus].Value = 0; // 0-норма 1-ошибка
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
                // Добавляем устройство
                _DeviceKCCM.Files.Add(mDevice);
                // Создаём для него контекст для данного устройства
                _Context.Add(new ModbusAdapterContext(device, mDevice));
            }
        }

        /// <summary>
        /// Создаёт modbus slave-устройство КССМУ 
        /// </summary>
        /// <param name="address">Сетевой адрес устройства</param>
        /// <returns></returns>
        private ModbusSlaveDevice CreateKCCM(Byte address)
        {
            ModbusSlaveDevice device = new ModbusSlaveDevice(address);

            device.Description = "Блок КССМУ";

            // Инициализируем данные устройства
            device.InputRegisters.Add(new InputRegister(0x0000, 0x2620, "Тип устройства"));
            device.InputRegisters.Add(new InputRegister(0x0001,
                (new ProductVersion(new Version(1, 0))).TotalVersion,
                "Версия ПО"));
            device.InputRegisters.Add(new InputRegister(0x0002,
                (new ProductVersion(new Version(1, 0))).TotalVersion,
                "Версия аппаратной части"));
            device.InputRegisters.Add(new InputRegister(0x0003, 0, "Серийный номер: High"));
            device.InputRegisters.Add(new InputRegister(0x0004, 0, "Серийный номер: Middle"));
            device.InputRegisters.Add(new InputRegister(0x0005, 0, "Серийный номер: Low"));
            device.InputRegisters.Add(new InputRegister(0x0006, 0, "СRC16")); // TODO
            device.InputRegisters.Add(new InputRegister(0x0007, 0, "Код производителя")); // TODO
            device.InputRegisters.Add(new InputRegister(0x0008, 0, "Количество устройств в системе"));
            device.InputRegisters.Add(new InputRegister(0x0009, 0, "Cчётчик электрической энергии: High")); // TODO
            device.InputRegisters.Add(new InputRegister(0x000A, 0, "Cчётчик электрической энергии: Low")); // TODO
            device.InputRegisters.Add(new InputRegister(0x000B, 0, "Напряжение питания, В")); // TODO
            device.HoldingRegisters.Add(new HoldingRegister(0x0000, 0, "Системное время: High"));
            device.HoldingRegisters.Add(new HoldingRegister(0x0001, 0, "Системное время: Low"));
            return device;
        }
        /// <summary>
        /// Создаёт modbus-файл устройства КИП БИ(У)-00 
        /// </summary>
        /// <returns>Modbus файл</returns>
        private File CreateKIP00()
        {
            File file = new File();

            // Создаём визитную карту устройства
            file.Records.Add(new Record(0x0000, 0x2652, "Тип устройства"));
            file.Records.Add(new Record(0x0001, 0, "Версия ПО"));
            file.Records.Add(new Record(0x0002, 0, "Версия Аппаратуры"));
            file.Records.Add(new Record(0x0003, 0, "Серийный номер: High"));
            file.Records.Add(new Record(0x0004, 0, "Серийный номер: Middle"));
            file.Records.Add(new Record(0x0005, 0, "Серийный номер: Low"));
            file.Records.Add(new Record(0x0006, 0, "CRC16"));
            file.Records.Add(new Record(0x0007, 0, "Код производителя"));

            // Добавляем служебную информацию 

            //(2 - Устройство Сети CAN)
            file.Records.Add(new Record(0x0008, 2, "Тип сети"));
            // Не реализованно - всегда 0
            file.Records.Add(new Record(0x0009, 0, "Номер сети"));
            file.Records.Add(new Record(0x000A, 1, "Сетевой адрес"));
            file.Records.Add(new Record(0x000A, 1, "Наличие связи с устройством"));

            // Добавляем данные специфичные для объектов словаря CAN устройства
            file.Records.Add(new Record(0x0001, 0, "Версия ПО"));
            file.Records.Add(new Record(0x0002, 0, "Версия Аппаратуры"));
            file.Records.Add(new Record(0x0003, 0, "Серийный номер: High"));
            file.Records.Add(new Record(0x0004, 0, "Серийный номер: Middle"));
            file.Records.Add(new Record(0x0005, 0, "Серийный номер: Low"));
            file.Records.Add(new Record(0x0006, 0, "CRC16"));
            file.Records.Add(new Record(0x0007, 0, "Код производителя"));

            // Добавляем данные специфичные для объектов словаря CAN устройства
            file.Records.Add(new Record(0x000C, 0, "Регистр ошибок"));
            file.Records.Add(new Record(0x000D, 0, "Регистр ошибок регистрации"));
            file.Records.Add(new Record(0x000E, 0, "Состояние устройства"));
            file.Records.Add(new Record(0x000F, 0, "Защитный потенциал"));
            file.Records.Add(new Record(0x0010, 0, "Поляризационный потенциал подземного трубопровода"));
            file.Records.Add(new Record(0x0011, 0, "Ток катодной защиты в точ-ке дренажа методом измере-ния напряжения на внешнем шунте"));
            file.Records.Add(new Record(0x0012, 0, "Наведённое переменное напряжение на трубопровод"));
            file.Records.Add(new Record(0x0013, 0, "Ток поляризации вспомога-тельного электрода"));
            file.Records.Add(new Record(0x0014, 0, "Плотность тока поляризации вспомогательного электрода"));
            file.Records.Add(new Record(0x0015, 0, "Ток измерительного канала 1"));
            file.Records.Add(new Record(0x0016, 0, "Ток измерительного канала 2"));
            file.Records.Add(new Record(0x0017, 0, "Глубина коррозии датчика ИКП с устройства УСИКПСТ"));
            file.Records.Add(new Record(0x0018, 0, "Скорость коррозии датчика ИКП с устройства УСИКПСТ"));
            file.Records.Add(new Record(0x0019, 0, "Состояние УСИКПСТ"));
            file.Records.Add(new Record(0x001A, 0, "Состояние пластины датчика «1» скорости коррозии 30,0-100,0 Ом"));
            file.Records.Add(new Record(0x001B, 0, "Состояние пластины датчика «2» скорости коррозии 30,0-100,0 Ом"));
            file.Records.Add(new Record(0x001C, 0, "Состояние пластины датчика «3» скорости коррозии 30,0-100,0 Ом"));
            file.Records.Add(new Record(0x001D, 0, "Ток натекания ВЭ постоянный"));
            file.Records.Add(new Record(0x001E, 0, "Ток натекания ВЭ переменный"));
            file.Records.Add(new Record(0x001F, 0, "Плотность тока натекания ВЭ постоянного"));
            file.Records.Add(new Record(0x0020, 0, "Плотность тока натекания ВЭ переменного"));
            file.Records.Add(new Record(0x0021, 0xFFFF, "Зарезервировано"));
            file.Records.Add(new Record(0x0022, 0, "Напряжение встроенного элемента питания"));
            file.Records.Add(new Record(0x0023, 0, "Зарезервировано"));
            file.Records.Add(new Record(0x0024, 0, "Площадь вспомогательного электрода (ВЭ)"));
            file.Records.Add(new Record(0x0025, 0, "Период измерений и передачи информации: High"));
            file.Records.Add(new Record(0x0026, 0, "Период измерений и передачи информации: Low"));
            file.Records.Add(new Record(0x0027, 0xFFFF, "Зарезервировано"));
            file.Records.Add(new Record(0x0028, 0, "Период опроса УСИКПСТ"));
            file.Records.Add(new Record(0x0029, 0, "Период опроса датчиков коррозии"));
            file.Records.Add(new Record(0x002A, 0, "Период опроса измерительно-го канала 1 4-20 мА"));
            file.Records.Add(new Record(0x002B, 0, "Период опроса измерительно-го канала 2 4-20 мА"));
            file.Records.Add(new Record(0x002C, 0, "Номинальный ток внешнего шунта (А)"));
            file.Records.Add(new Record(0x002D, 0, "Флаг разрешения работы канала измерения поляризационного потенциала подземного трубопровода."));
            file.Records.Add(new Record(0x002E, 0, "Флаг разрешения работы канала измерения защитного потенциала."));
            file.Records.Add(new Record(0x002F, 0, "Флаг разрешения работы канала измерения тока катодной защиты в точке дренажа методом измерения напряжения на внешнем шунте."));
            file.Records.Add(new Record(0x0030, 0, "Флаг разрешения работы канала тока поляризации вспомогательного электрода"));
            file.Records.Add(new Record(0x0031, 0, "Флаг разрешения работы канала измерения наведённого переменного напряжения на трубопровод"));
            file.Records.Add(new Record(0x0032, 0, "Флаг разрешения передачи слова состояния"));
            file.Records.Add(new Record(0x0033, 0, "Флаг разрешения работы канала измерения тока натекания ВЭ постоянного"));
            file.Records.Add(new Record(0x0034, 0, "Флаг разрешения работы канала измерения тока натекания ВЭ переменного"));
            file.Records.Add(new Record(0x0035, 0, "Разрешение или запрещение передачи PDO"));
            file.Records.Add(new Record(0x0036, 0, "Текущее время устройства: High"));
            file.Records.Add(new Record(0x0037, 0, "Текущее время устройства: Low"));
            return file;
        }
        /// <summary>
        /// Создаёт modbus-файл устройства КИП БИ(У)-01 
        /// </summary>
        /// <returns>Modbus файл</returns>
        private File CreateKIP01()
        {
            File file = new File();

            // Создаём визитную карту устройства
            file.Records.Add(new Record(0x0000, 0x2653, "Тип устройства"));
            file.Records.Add(new Record(0x0001, 0, "Версия ПО"));
            file.Records.Add(new Record(0x0002, 0, "Версия Аппаратуры"));
            file.Records.Add(new Record(0x0003, 0, "Серийный номер: High"));
            file.Records.Add(new Record(0x0004, 0, "Серийный номер: Middle"));
            file.Records.Add(new Record(0x0005, 0, "Серийный номер: Low"));
            file.Records.Add(new Record(0x0006, 0, "CRC16"));
            file.Records.Add(new Record(0x0007, 0, "Код производителя"));

            // Добавляем служебную информацию 

            //(2 - Устройство Сети CAN)
            file.Records.Add(new Record(0x0008, 2, "Тип сети"));
            // Не реализованно - всегда 0
            file.Records.Add(new Record(0x0009, 0, "Номер сети"));
            file.Records.Add(new Record(0x000A, 1, "Сетевой адрес"));
            file.Records.Add(new Record(0x000B, 1, "Наличие связи с устройством"));

            // Добавляем данные специфичные для объектов словаря CAN устройства

            file.Records.Add(new Record(0x000C, 0, "Регистр ошибок"));
            file.Records.Add(new Record(0x000D, 0, "Регистр ошибок регистрации"));
            file.Records.Add(new Record(0x000E, 0, "Состояние устройства"));
            file.Records.Add(new Record(0x000F, 0, "Защитный потенциал"));
            file.Records.Add(new Record(0x0010, 0, "Поляризационный потенциал подземного трубопровода"));
            file.Records.Add(new Record(0x0011, 0, "Ток катодной защиты в точ-ке дренажа методом измере-ния напряжения на внешнем шунте"));
            file.Records.Add(new Record(0x0012, 0, "Наведённое переменное напряжение на трубопровод"));
            file.Records.Add(new Record(0x0013, 0, "Ток поляризации вспомогательного электрода"));
            file.Records.Add(new Record(0x0014, 0, "Плотность тока поляризации вспомогательного электрода"));
            file.Records.Add(new Record(0x0015, 0, "Ток измерительного канала 1"));
            file.Records.Add(new Record(0x0016, 0, "Ток измерительного канала 2"));
            file.Records.Add(new Record(0x0017, 0, "Глубина коррозии датчика ИКП с устройства УСИКПСТ"));
            file.Records.Add(new Record(0x0018, 0, "Скорость коррозии датчика ИКП с устройства УСИКПСТ"));
            file.Records.Add(new Record(0x0019, 0, "Состояние УСИКПСТ"));
            file.Records.Add(new Record(0x001A, 0, "Состояние пластины датчика «1» скорости коррозии 30,0-100,0 Ом"));
            file.Records.Add(new Record(0x001B, 0, "Состояние пластины датчика «2» скорости коррозии 30,0-100,0 Ом"));
            file.Records.Add(new Record(0x001C, 0, "Состояние пластины датчика «3» скорости коррозии 30,0-100,0 Ом"));
            file.Records.Add(new Record(0x001D, 0, "Ток натекания ВЭ постоянный"));
            file.Records.Add(new Record(0x001E, 0, "Ток натекания ВЭ переменный"));
            file.Records.Add(new Record(0x001F, 0, "Плотность тока натекания ВЭ постоянного"));
            file.Records.Add(new Record(0x0020, 0, "Плотность тока натекания ВЭ переменного"));
            file.Records.Add(new Record(0x0021, 0xFFFF, "Зарезервировано"));
            file.Records.Add(new Record(0x0022, 0, "Напряжение встроенного элемента питания"));
            file.Records.Add(new Record(0x0023, 0, "Температура встроенного датчика БИ(У)"));
            file.Records.Add(new Record(0x0024, 0, "Площадь вспомогательного электрода (ВЭ)"));
            file.Records.Add(new Record(0x0025, 0, "Период измерений и передачи информации: High"));
            file.Records.Add(new Record(0x0026, 0, "Период измерений и передачи информации: Low"));
            file.Records.Add(new Record(0x0027, 0xFFFF, "Зарезервировано"));
            file.Records.Add(new Record(0x0028, 0, "Период опроса УСИКПСТ"));
            file.Records.Add(new Record(0x0029, 0, "Период опроса датчиков коррозии"));
            file.Records.Add(new Record(0x002A, 0, "Период опроса измерительного канала 1 4-20 мА"));
            file.Records.Add(new Record(0x002B, 0, "Период опроса измерительного канала 2 4-20 мА"));
            file.Records.Add(new Record(0x002C, 0, "Номинальный ток внешнего шунта (А)"));
            file.Records.Add(new Record(0x002D, 0, "Флаг разрешения работы канала измерения поляризационного потенциала подземного трубопровода."));
            file.Records.Add(new Record(0x002E, 0, "Флаг разрешения работы канала измерения защитного потенциала."));
            file.Records.Add(new Record(0x002F, 0, "Флаг разрешения работы канала измерения тока катодной защиты в точке дренажа методом измерения напряжения на внешнем шунте."));
            file.Records.Add(new Record(0x0030, 0, "Флаг разрешения работы канала тока поляризации вспомогательного электрода"));
            file.Records.Add(new Record(0x0031, 0, "Флаг разрешения работы канала измерения наведённого переменного напряжения на трубопровод"));
            file.Records.Add(new Record(0x0032, 0, "Флаг разрешения передачи слова состояния"));
            file.Records.Add(new Record(0x0033, 0, "Флаг разрешения работы канала измерения тока натекания ВЭ постоянного"));
            file.Records.Add(new Record(0x0034, 0, "Флаг разрешения работы канала измерения тока натекания ВЭ переменного"));
            file.Records.Add(new Record(0x0035, 0, "Разрешение или запрещение передачи PDO"));
            file.Records.Add(new Record(0x0036, 0, "Текущее время устройства: High"));
            file.Records.Add(new Record(0x0037, 0, "Текущее время устройства: Low"));

            return file;
        }

        #endregion
    }
}
