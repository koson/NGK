using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using NGK.CAN.DataTypes.DateTimeConvertor;

namespace ModbusPlugin.ApplicationServices.Models
{
    public static class AdderssHelper
    {
        /// <summary>
        /// Адреса регистров блока КССМУ
        /// </summary>
        public class KCCM
        {
            public class InputRegister
            {
                public const UInt16 DeviceType = 0x0000;
                public const UInt16 SoftwareVersion = 0x0001;
                public const UInt16 HardwareVersion = 0x0002;
                public const UInt16 SerialNumberHigh = 0x0003;
                public const UInt16 SerialNumberMiddle = 0x0004;
                public const UInt16 SerialNumberLow = 0x0005;
                public const UInt16 CRC16 = 0x0006;
                public const UInt16 VendorCode = 0x0007;
                public const UInt16 TotalDevices = 0x0008;
            }
            public class HoldingRegisters
            {
                public const UInt16 DateTimeUnixHigh = 0x0000;
                public const UInt16 DateTimeUnixLow = 0x0001;
            }
        }
        /// <summary>
        /// Адреса "визитной карточки" записей файла устройства 
        /// </summary>
        public class ModbusVisitingCard
        {
            /// <summary>
            /// Адреса визитной карты устройства
            /// </summary>
            public class VisitingCard
            {
                public const UInt16 DeviceType = 0x0000;
                public const UInt16 SoftwareVersion = 0x0001;
                public const UInt16 HardwareVersion = 0x0002;
                public const UInt16 SerialNumberHigh = 0x0003;
                public const UInt16 SerialNumberMiddle = 0x0004;
                public const UInt16 SerialNumberLow = 0x0005;
                public const UInt16 CRC16 = 0x0006;
                public const UInt16 VendorCode = 0x0007;
            }
            /// <summary>
            /// Служебная информация об устройства
            /// </summary>
            public class ServiceInformation
            {
                /// <summary>
                /// Код физического уровня сети, в которой работает данное устройство
                /// </summary>
                public const UInt16 NetworkType = 0x0008;
                /// <summary>
                /// Номер сети (порта) подклю-чённой к блоку КССМ(У)
                /// </summary>
                public const UInt16 NetworkNumber = 0x0009;
                /// <summary>
                /// Сетевой адрес устройства или сетевой идентификатор (CAN Node Id)
                /// </summary>
                public const UInt16 NetwrokAddress = 0x000A;
                /// <summary>
                /// Наличие связи с устройством.
                /// </summary>
                public const UInt16 ConectionStatus = 0x000B;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class KIP9811Address : ModbusVisitingCard
        {
            /// <summary>
            /// Регистр ошибок
            /// </summary>
            public const UInt16 Errors = 0x000C;
            /// <summary>
            /// Регистр ошибок регистрации
            /// </summary>
            public const UInt16 ErrorsRegistration = 0x000D;
            /// <summary>
            /// Состояние устройства
            /// </summary>
            public const UInt16 DeviceStatus = 0x000E;
            /// <summary>
            /// Защитный потенциал
            /// </summary>
            public const UInt16 protection_pot = 0x000F;
            /// <summary>
            /// Поляризационный потенциал подземного трубопровода
            /// </summary>	
            public const UInt16 polarisation_pot = 0x0010;
            /// <summary>
            /// Ток катодной защиты в точке дренажа методом измерения напряжения на внешнем шунте
            /// </summary>
            public const UInt16 protection_cur = 0x0011;
            /// <summary>
            /// Наведённое переменное напряжение на трубопровод
            /// </summary>
            public const UInt16 induced_ac = 0x0012;
            /// <summary>
            /// Ток поляризации вспомогательного электрода
            /// </summary>
            public const UInt16 polarisation_cur = 0x0013;
            /// <summary>
            /// Плотность тока поляризации вспомогательного электрода.
            /// </summary>
            public const UInt16 density_cur = 0x0014;
            /// <summary>
            /// Ток измерительного канала 1 
            /// </summary>
            public const UInt16 aux_cur1 = 0x0015;
            /// <summary>
            /// Ток измерительного канала 2
            /// </summary>
            public const UInt16 aux_cur2 = 0x0016;
            /// <summary>
            /// Глубина коррозии датчика ИКП с устройства УСИКПСТ 
            /// </summary>
            public const UInt16 corrosion_depth = 0x0017;
            /// <summary>
            /// Скорость коррозии датчика ИКП с устройства УСИКПСТ
            /// </summary>
            public const UInt16 corrosion_speed = 0x0018;
            /// <summary>
            /// Состояние УСИКПСТ
            /// </summary>
            public const UInt16 usikp_state = 0x0019;
            /// <summary>
            /// Состояние пластины датчика «1» скорости коррозии 30,0-100,0 Ом
            /// </summary>
            public const UInt16 corrosion_sense1 = 0x001A;
            /// <summary>
            /// Состояние пластины датчика «2» скорости коррозии 30,0-100,0 Ом	
            /// </summary>
            public const UInt16 corrosion_sense2 = 0x001B;
            /// <summary>
            /// Состояние пластины датчика «3» скорости коррозии 30,0-100,0 Ом	
            /// </summary>
            public const UInt16 corrosion_sense3 = 0x001C;
            /// <summary>
            /// Ток натекания ВЭ постоянный	
            /// </summary>
            public const UInt16 polarisation_cur_dc = 0x001D;
            /// <summary>
            /// Ток натекания ВЭ переменный	
            /// </summary>
            public const UInt16 polarisation_cur_ac = 0x001E;
            /// <summary>
            /// Плотность тока натекания ВЭ постоянного 
            /// </summary>
            public const UInt16 density_pol_cur_dc = 0x001F;
            /// <summary>
            /// Плотность тока натекания ВЭ переменного
            /// </summary>
            public const UInt16 density_pol_cur_ac = 0x0020;
            //Зарезервировано 
            //reserved3 =	0x0021
            /// <summary>
            /// Напряжение встроенного элемента питания	
            /// </summary>
            public const UInt16 battery_voltage = 0x0022;
            /// <summary>
            /// Температура встроенного датчика БИ(У) 	
            /// </summary>
            public const UInt16 int_temp = 0x0023;
            /// <summary>
            /// Площадь вспомогательного электрода (ВЭ)
            /// </summary>
            public const UInt16 electrod_area = 0x0024;
            /// <summary>
            /// Период измерений и передачи информации (старший байт)
            /// </summary>
            public const UInt16 meas_period_high = 0x0025;
            /// <summary>
            /// Период измерений и передачи информации (младший байт)
            /// </summary>
            public const UInt16 meas_period_low = 0x0026;
            //Зарезервировано	0x0027
            /// <summary>
            /// Период опроса УСИКПСТ
            /// </summary>
            public const UInt16 usikp_period = 0x0028;
            /// <summary>
            /// Период опроса датчиков коррозии
            /// </summary>
            public const UInt16 corr_sense_period = 0x0029;
            /// <summary>
            /// Период опроса измерительного канала 1 4-20 мА	
            /// </summary>
            public const UInt16 aux1_period = 0x002A;
            /// <summary>
            /// Период опроса измерительного канала 2 4-20 мА
            /// </summary>
            public const UInt16 aux2_period = 0x002B;
            /// <summary>
            /// Номинальный ток внешнего шунта (А)
            /// </summary>
            public const UInt16 shunt_nom = 0x002C;
            /// <summary>
            /// Флаг разрешения работы канала измерения поляризационного потенциала подземного трубопровода.
            /// </summary>
            public const UInt16 polarisation_pot_en = 0x002D;
            /// <summary>
            /// Флаг разрешения работы канала измерения защитного потенциала.
            /// </summary>
            public const UInt16 protection_pot_en = 0x002E;
            /// <summary>
            /// Флаг разрешения работы канала измерения тока катодной защиты в точке дренажа методом измерения напряжения на внешнем шунте.	
            /// </summary>
            public const UInt16 protection_cur_en = 0x002F;
            /// <summary>
            /// Флаг разрешения работы канала тока поляризации вспомогательного электрода	
            /// </summary>
            public const UInt16 polarisation_cur_en = 0x0030;
            /// <summary>
            /// Флаг разрешения работы канала измерения наведённого переменного напряжения на трубопровод
            /// </summary>
            public const UInt16 induced_ac_en = 0x0031;
            /// <summary>
            /// Флаг разрешения передачи слова состояния
            /// </summary>
            public const UInt16 status_flags_en = 0x0032;
            /// <summary>
            /// Флаг разрешения работы канала измерения тока натекания ВЭ постоянного	
            /// </summary>
            public const UInt16 polarisation_cur_dc_en = 0x0033;
            /// <summary>
            /// Флаг разрешения работы канала измерения тока натекания ВЭ переменного
            /// </summary>
            public const UInt16 polarisation_cur_ac_en = 0x0034;
            /// <summary>
            /// Разрешение или запрещение передачи PDO
            /// </summary>
            public const UInt16 pdo_flags = 0x0035;
            /// <summary>
            /// Текущее время устройства 
            /// </summary>
            public const UInt16 datetime_high = 0x0036;
            /// <summary>
            /// Текущее время устройства 
            /// </summary>
            public const UInt16 datetime_low = 0x0037;
        }
        /// <summary>
        /// 
        /// </summary>
        public class RemappingTableKip9811
        {
            /// <summary>
            /// Таблица соовтвествия адресов modbus и can устройства
            /// ключ - Адрес записи файла modbus устройства
            /// значение - индекс объекта словаря can устройства
            /// </summary>
            private static readonly Dictionary<UInt16, UInt16> _TableAddress;

            static RemappingTableKip9811()
            {
                if (_TableAddress == null)
                {
                    _TableAddress = new Dictionary<ushort, ushort>();
                    _TableAddress.Add(0x0000, 0x2000);
                    _TableAddress.Add(0x0001, 0x2001);
                    _TableAddress.Add(0x0002, 0x2002);
                    _TableAddress.Add(0x0003, 0x2003);
                    _TableAddress.Add(0x0004, 0x2004);
                    _TableAddress.Add(0x0005, 0x2005);
                    _TableAddress.Add(0x0006, 0x2006);
                    _TableAddress.Add(0x0007, 0x2007);
                    _TableAddress.Add(0x000F, 0x2009);
                    _TableAddress.Add(0x0010, 0x2008);
                    _TableAddress.Add(0x0011, 0x200B);
                    _TableAddress.Add(0x0012, 0x200A);
                    _TableAddress.Add(0x0013, 0x200C);
                    _TableAddress.Add(0x0015, 0x200D);
                    _TableAddress.Add(0x0016, 0x200E);
                    _TableAddress.Add(0x0017, 0x200F);
                    _TableAddress.Add(0x0018, 0x200E);
                    _TableAddress.Add(0x0019, 0x2011);
                    _TableAddress.Add(0x001A, 0x2018);
                    _TableAddress.Add(0x001B, 0x2019);
                    _TableAddress.Add(0x001C, 0x201A);
                    _TableAddress.Add(0x001D, 0x201B);
                    _TableAddress.Add(0x001E, 0x201C);
                    _TableAddress.Add(0x0022, 0x2013);
                    _TableAddress.Add(0x0023, 0x2014);
                    _TableAddress.Add(0x0025, 0x201E);
                    _TableAddress.Add(0x0026, 0x201E);
                    _TableAddress.Add(0x0028, 0x2021);
                    _TableAddress.Add(0x0029, 0x2022);
                    _TableAddress.Add(0x002A, 0x2023);
                    _TableAddress.Add(0x002B, 0x2024);
                    _TableAddress.Add(0x002C, 0x2026);
                    _TableAddress.Add(0x002D, 0x2027);
                    _TableAddress.Add(0x002E, 0x2028);
                    _TableAddress.Add(0x002F, 0x2029);
                    _TableAddress.Add(0x0030, 0x202A);
                    _TableAddress.Add(0x0031, 0x202B);
                    _TableAddress.Add(0x0032, 0x202F);
                    _TableAddress.Add(0x0033, 0x202D);
                    _TableAddress.Add(0x0034, 0x202E);
                    _TableAddress.Add(0x0035, 0x2030);
                    _TableAddress.Add(0x0036, 0x2031);
                    _TableAddress.Add(0x0037, 0x2031);
                }
            }

            public static void Copy(File modbusDevice, DeviceBase canDevice)
            {
                Record rec;
                UInt16 index;
                UInt32 var32;

                if (canDevice.DeviceType != DeviceType.KIP_BATTERY_POWER_v1)
                {
                    throw new InvalidCastException("Требуется устройство CAN типа КИП-9811");
                }
                if (modbusDevice.Records[KIP9811Address.VisitingCard.DeviceType].Value !=
                    (UInt16)DeviceType.KIP_BATTERY_POWER_v1)
                {
                    throw new InvalidCastException("Требуется устройство modbus типа КИП-9811");
                }

                // Адреса, которые обрабатываются по особому
                UInt16[] exclusions = new ushort[] { 0x000A, 0x000B, 0x000C, 0x000E, 0x0014,
                0x0019, 0x0020, 0x0024, 0x0025, 0x0026, 0x0036, 0x0037 };
                // Адреса, которые исключаются из обработки
                UInt16[] nothandled = new ushort[] { 0x0000, 0x0006, 0x0007, 0x0008, 0x0009, 
                0x000D, 0x000E };
                // Копируем данные
                foreach (UInt16 modbusAddress in _TableAddress.Keys)
                {
                    // Исключаем из обработки адреса
                    if (Exist(nothandled, modbusAddress))
                    {
                        continue;
                    }

                    // Адреса для обработки особым способом,
                    // так же пропускаем, их обработаем ниже
                    if (Exist(exclusions, modbusAddress))
                    {
                        continue;
                    }

                    // Получаем индекс объекта CAN-устройства
                    index = _TableAddress[modbusAddress];
                    // Копируем значение объекта в запись modbus 
                    modbusDevice.Records[modbusAddress].Value =
                        (UInt16)canDevice.ObjectDictionary[index].Value;
                }

                // Теперь обрабатываем сложные параметры
                // 0x0007
                //modbusDevice.Records[0x0007].Value = 0; //TODO Код производителя, всега неопределён
                // 0x000A 
                modbusDevice.Records[0x000A].Value = Convert.ToUInt16(canDevice.NodeId);
                // 0x000B
                if (canDevice.Status == DeviceStatus.CommunicationError)
                { modbusDevice.Records[0x000B].Value = 0; }
                else
                { modbusDevice.Records[0x000B].Value = 1; }
                // 0x000C
                //modbusDevice.Records[0x000C].Value = 0; //TODO
                // 0x000E
                switch (canDevice.Status)
                {
                    case DeviceStatus.CommunicationError:
                    case DeviceStatus.ConfigurationError:
                    case DeviceStatus.Stopped:
                        {
                            modbusDevice.Records[0x000E].Value =
                                (UInt16)DeviceStatus.Stopped; // Stopped
                            break;
                        }
                    case DeviceStatus.Operational:
                        {
                            modbusDevice.Records[0x000E].Value =
                                (UInt16)DeviceStatus.Operational;
                            break;
                        }
                    case DeviceStatus.Preoperational:
                        {
                            modbusDevice.Records[0x000E].Value =
                                (UInt16)DeviceStatus.Preoperational;
                            break;
                        }
                    default: { throw new NotSupportedException(); }
                }
                //0x0014 TODO
                //0x0019 TODO
                //0x0020 TODO
                //0x0024
                modbusDevice.Records[0x0024].Value = canDevice.ElectrodeArea;
                //0x0025, 0x0026
                var32 = (UInt32)canDevice.GetObject(_TableAddress[0x0025]);
                modbusDevice.Records[0x0025].Value = (UInt16)(var32 >> 16);
                var32 = (UInt32)canDevice.GetObject(_TableAddress[0x0026]);
                modbusDevice.Records[0x0026].Value = (UInt16)var32;
                //0x0036, 0x0037
                var32 = Unix.ToUnixTime((DateTime)canDevice.GetObject(_TableAddress[0x0036]));
                modbusDevice.Records[0x0036].Value = (UInt16)(var32 >> 16);
                var32 = Unix.ToUnixTime((DateTime)canDevice.GetObject(_TableAddress[0x0037]));
                unchecked
                {
                    modbusDevice.Records[0x0037].Value = (UInt16)var32;
                }
                return;
            }

            private static bool Exist(UInt16[] array, UInt16 value)
            {
                foreach (UInt16 item in array)
                {
                    if (item == value)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
