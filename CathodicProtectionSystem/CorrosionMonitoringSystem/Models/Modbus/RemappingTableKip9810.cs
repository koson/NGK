using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.DataTypes.DateTimeConvertor;

namespace NGK.CorrosionMonitoringSystem.Models.Modbus
{
    public static class RemappingTableKip9810
    {
                /// <summary>
        /// Таблица соовтвествия адресов modbus и can устройства
        /// ключ - Адрес записи файла modbus устройства
        /// значение - индекс объекта словаря can устройства
        /// </summary>
        private static readonly Dictionary<UInt16, UInt16> _TableAddress;

        static RemappingTableKip9810()
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
                //_TableAddress.Add(0x0023, 0x2014); // Температурный датчик в БИ-01
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

            if (canDevice.DeviceType != DeviceType.KIP_MAIN_POWERED_v1)
            {
                throw new InvalidCastException("Требуется устройство CAN типа КИП-9810");
            }
            if (modbusDevice.Records[KIP9810AddressSpaceHelper.VisitingCard.DeviceType].Value !=
                (UInt16)DeviceType.KIP_MAIN_POWERED_v1)
            {
                throw new InvalidCastException("Требуется устройство modbus типа КИП-9810");
            }

            // Адреса, которые обрабатываются по особому
            UInt16[] exclusions = new ushort[] { 0x000A, 0x000B, 0x000C, 0x000E, 0x0014,
                0x0019, 0x0020, 0x0024, 0x0025, 0x0026, 0x0036, 0x0037 };
            // Адреса, которые исключаются из обработки
            UInt16[] nothandled = new ushort[] { 0x0000, 0x0006, 0x0007, 0x0008, 0x0009, 
                0x000D, 0x000E, 0x0023 }; // 0x0023
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
            unchecked
            {
                var32 = (UInt32)canDevice.GetObject(_TableAddress[0x0025]);
                modbusDevice.Records[0x0025].Value = (UInt16)(var32 >> 16);
                var32 = (UInt32)canDevice.GetObject(_TableAddress[0x0026]);
                modbusDevice.Records[0x0026].Value = (UInt16)var32;
                //0x0036, 0x0037
                var32 = Unix.ToUnixTime((DateTime)canDevice.GetObject(_TableAddress[0x0036]));
                modbusDevice.Records[0x0036].Value = (UInt16)(var32 >> 16);
                var32 = Unix.ToUnixTime((DateTime)canDevice.GetObject(_TableAddress[0x0037]));
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
