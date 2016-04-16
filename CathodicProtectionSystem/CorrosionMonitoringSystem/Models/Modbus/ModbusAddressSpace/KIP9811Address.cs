using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.Models.Modbus
{
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
}
