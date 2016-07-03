using System;
using System.Text;
using NGK.CAN.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.Collections;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.ComplexParameterConverters;
using NGK.CAN.DataTypes;
using NGK.CAN.DataTypes.Helper;
using NGK.CAN.DataTypes.TypeConverters;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles
{
    /// <summary>
    /// Реализует профиль устройства КИП 9811 версии 1
    /// </summary>
    public sealed class KIP9811v1 : CanDevicePrototype
    {
        #region Fields And Properties

        private static KIP9811v1 _Instance;

        public static ICanDeviceProfile Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new KIP9811v1();
                    }
                }
                return _Instance;
            }
        }

        public override DeviceType DeviceType
        {
            get { return DeviceType.KIP_BATTERY_POWER_v1; }
        }

        public override string Description
        {
            get { return @"Устройство КИП с автономным питанием от АКБ"; }
        }

        public override Version SoftwareVersion
        {
            get
            {
                ObjectInfo objInfo = _ObjectInfoList[KIP9811v1.Indexes.fw_version];
                NgkProductVersion version = (NgkProductVersion)objInfo.DataTypeConvertor
                    .ConvertToOutputValue(objInfo.DefaultValue);
                return version.Version;
            }
        }

        public override Version HardwareVersion
        {
            get
            {
                ObjectInfo objInfo = _ObjectInfoList[KIP9811v1.Indexes.hw_version];
                NgkProductVersion version = (NgkProductVersion)objInfo.DataTypeConvertor
                    .ConvertToOutputValue(objInfo.DefaultValue);
                return version.Version;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        private KIP9811v1(): base()
        {
            SetSoftwareVersion(new Version(1, 0));
            SetHardwareVersion(new Version(1, 0));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Создаёт описание объектов словаря объектов устройства
        /// </summary>
        protected override void CreateObjectDictionary()
        {
            base.CreateObjectDictionary();

            // Определяем сложные (составные) объекты:
            _ComplexParameters.Add(new ComplexParameter("SerialNumber", "Серийный номер",
                "Серийный номер устройства", true, true, string.Empty, 
                ObjectCategory.System, new SerialNumberConverter(), (UInt64)0,
                0x2003, 0x2004, 0x2005));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2000, "device_type", "Тип устройства",
                true, true, true, "Тип устройства", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt16)this.DeviceType));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2001, "fw_version", "Версия ПО",
                true, true, true, "Версия ПО", String.Empty, ObjectCategory.System,
                NgkProductVersionConvertor.Instance, 
                NgkProductVersionConvertor.Instance.ConvertToBasis(new NgkProductVersion(new Version(1, 0)))));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2002, "hw_version", "Версия аппаратуры",
                true, true, true, "Версия аппаратуры", String.Empty, ObjectCategory.System,
                NgkProductVersionConvertor.Instance,
                NgkProductVersionConvertor.Instance.ConvertToBasis(new NgkProductVersion(new Version(1, 0)))));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2003, "serial_number1", "Серийный номер устройства",
                true, true, true, "Серийный номер 1", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0,
                _ComplexParameters["SerialNumber"].Name));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2004, "serial_number2", "Серийный номер устройства",
                true, true, true, "Серийный номер 2", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0,
                _ComplexParameters["SerialNumber"].Name));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2005, "serial_number3", "Серийный номер устройства",
                true, true, true, "Серийный номер 3", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0,
                _ComplexParameters["SerialNumber"].Name));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2006, "vcard_chksum", "Контрольная сумма CRC16 визитной карты устройства",
                true, true, true, "Контрольная сумма", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2007, "vendor_id", "Код производителя",
                false, true, true, "Код производителя", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2008, "polarization_pot",
                "Поляризационный потенциал, В",
                false, true, true, "Поляризационный потенциал", "B", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x001), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2009, "protection_pot", "Защитный потенциал, В",
                false, true, true, "U защитный", "B", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x001), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x200A, "induced_ac", "Наведенное переменное напряжение, В",
                false, true, true, "U наведенное перемен.", "B", ObjectCategory.Measured,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x200B, "protection_cur", "Ток катодной защиты, А",
                false, true, true, "I катодной защиты", "A", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x005), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x200C, "polarization_cur",
                "Ток поляризации, mA",
                false, true, true, "I поляризации", "A", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x01), (UInt32)0));

            //uFloatNgk = new NgkUFloat(Precision.x001, 0);
            _ObjectInfoList.Add(new ObjectInfo(this, 0x200D, "aux_cur1", "Ток канала 1, mA",
                false, true, true, "I канала 1", "mA", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x001), (UInt32)0));

            //uFloatNgk = new NgkUFloat(Precision.x001, 0);
            _ObjectInfoList.Add(new ObjectInfo(this, 0x200E, "aux_cur2", "Ток канала 2, mA",
                false, true, true, "I канала 2", "mA", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x001), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x200F, "corrosion_depth", "Глубина коррозии, мкм",
                false, true, true, "Глубина коррозии", "мкм", ObjectCategory.Measured,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2010, "corrosion_speed", "Скорость коррозии, мкм/год",
                false, true, true, "Скорость коррозии", "мкм/год", ObjectCategory.Measured,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2011, "usipk_state", "Состояние УСИКПСТ",
                false, true, true, "Состояние УСИКПСТ", String.Empty, ObjectCategory.Measured,
                new NgkByteConverter(), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2012, "supply_voltage",
                "Питающее напряжение, B",
                false, true, true, "Питающее напряжение", "B", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x005), (UInt32)0, null));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2013, "battery_voltage",
                "Напряжение батареи, B",
                false, true, true, "U бат.", "B", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x001), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2014, "int_temp",
                "Температура с встроенного датчика, \u00B0C",
                false, true, true, "Температура с встроенного датчика", "C", ObjectCategory.Measured,
                new NgkInt16Converter(), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2015, "tamper", "Вскрытие",
                false, false, true, "Вскрытие", String.Empty, ObjectCategory.System,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2016, "supply_voltage_low", "Питания неисправно",
                false, true, true, "Напряжение питания ниже нормы", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2017, "battery_voltage_low", "Напряжение батареи ниже нормы",
                false, true, true, "Батарея неисправна", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2018, "corrosion_sense1", "Состояние датчика коррозии 1",
                false, true, true, "Датчик коррозии 1 сработал", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2019, "corrosion_sense2", "Состояние датчика коррозии 2",
                false, true, true, "Датчик коррозии 2 сработал", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x201A, "corrosion_sense3", "Состояние датчика коррозии 3",
                false, true, true, "Датчик коррозии 3 сработал", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x201B, "polarization_cur_dc",
                "Ток натекания постоянный (ток протекающий между трубой и электродом сравнения)",
                false, true, true, "Ток натекания постоянный, mA", "mA", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x01), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x201C, "polarization_cur_ac",
                "Ток натекания переменный (ток протекающий между трубой и электродом сравнения)",
                false, true, true, "I натекания перем., mA", "mA", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x01), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x201E, "meas_period",
                "Период измерений и передачи данных, сек",
                false, true, true, "Период измерений и передачи данных", "сек.", ObjectCategory.Configuration,
                new NgkUInt32Convertor(), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2020, "meas_supply_period", "Период измерения питающего напряжения, сек",
                false, true, true, "Период измерения Uп", "сек.", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)10));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2021, "usipk_period", "Период опроса УСИКПСТ, сек",
                false, true, true, "Период опроса УСИКПСТ", "сек.", ObjectCategory.Configuration,
                new NgkUInt16WithStatusDisabledConverter(ScalerTypes.x10), 0xFFFF)); //10

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2022, "corr_sense_period", "Период опроса датчиков коррозии, сек",
                false, true, true, "Период опроса датчиков коррозии", "сек.", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x10), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2023, "aux1_period", "Период опроса канала 1, сек",
                false, true, true, "Период опроса канала 1", "сек.", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x10), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2024, "aux2_period", "Период опроса канала 2, сек",
                false, true, true, "Период опроса канала 2", "сек.", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x10), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2026, "shunt_nom", "Номинал шунта, А",
                false, true, true, "Номинал шунта", "A", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)50));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2027, "polarisation_pot_en", "Разрешение измерения поляр. потенциала",
                false, true, true, "Разрешение измерения поляр. потенциала", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2028, "protection_pot_en", "Разрешение измерения защитного потенциала",
                false, true, true, "Разрешение измерения защитного потенциала", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2029, "protection_cur_en",
                "Разрешение измерения защитного тока",
                false, true, true, "Разрешение измерения защитного тока", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202A, "polarisation_cur_en",
                "Разрешение измерения поляризационного тока",
                false, true, true, "Разрешение измерения поляризационного тока", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202B, "induced_ac_en",
                "Разрешение измерения наведенного напряжения",
                false, true, true, "Разрешение измерения наведенного напряжения", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202C, "prot_pot_ext_range",
                "Расширенный диапазон защитного потенциала",
                false, true, true, "Расширенный диапазон защитного потенциала", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202D, "polarization_cur_dc_en",
                "Разрешение измерения постоянного тока натекания",
                false, true, true, "Разрешение измерения постоянного тока натекания", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202E, "polarization_cur_ac_en",
                "Разрешение измерения переменного тока натекания",
                false, true, true, "Разрешение измерения переменного тока натекания", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202F, "status_flags_en",
                "Разрешение передачи слова состояния",
                false, true, true, "Разрешение передачи слова состояния", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2030, "pdo_flags",
                "Разрешение или запрещение передачи PDO",
                false, true, true, "Разрешение или запрещение передачи PDO", String.Empty, ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2031, "datetime", "Текущее время",
                true, true, true, "Текущее время", String.Empty, ObjectCategory.Measured,
                new NgkDateTimeConverter(), (new NgkDateTimeConverter()).ConvertToBasis(DateTime.Now)));

            //// Подключаем событие от объектного словаря
            //base.CreateObjectDictionary();

            return;
        }

        void SetSoftwareVersion(Version version)
        { 
            ObjectInfo objInfo = _ObjectInfoList[KIP9811v1.Indexes.fw_version];
            objInfo.DefaultValue = 
                NgkProductVersionConvertor.ConvertFromVersion(version).TotalVersion;
        }

        void SetHardwareVersion(Version version)
        {
            ObjectInfo objInfo = _ObjectInfoList[KIP9811v1.Indexes.hw_version];
            objInfo.DefaultValue = 
                NgkProductVersionConvertor.ConvertFromVersion(version).TotalVersion;
        }

        #endregion

        #region Indexes
        
        /// <summary>
        /// Индексы объектов словаря устройства
        /// </summary>
        public static class Indexes
        {
            public const UInt16 device_type = 0x2000;
            public const UInt16 fw_version = 0x2001;
            public const UInt16 hw_version = 0x2002;
            public const UInt16 serial_number1 = 0x2003;
            public const UInt16 serial_number2 = 0x2004;
            public const UInt16 serial_number3 = 0x2005;
            public const UInt16 vcard_chksum = 0x2006;
            public const UInt16 vendor_id = 0x2007;
            public const UInt16 polarisation_pot = 0x2008;
            public const UInt16 protection_pot = 0x2009;
            public const UInt16 induced_ac = 0x200A;
            public const UInt16 protection_cur = 0x200B;
            public const UInt16 polarisation_cur = 0x200C;
            public const UInt16 aux_cur1 = 0x200D;
            public const UInt16 aux_cur2 = 0x200E;
            public const UInt16 corrosion_depth = 0x200F;
            public const UInt16 corrosion_speed = 0x2010;
            public const UInt16 usikp_state = 0x2011;
            public const UInt16 reserved1_supply_voltage = 0x2012;
            public const UInt16 battery_voltage = 0x2013;
            public const UInt16 int_temp = 0x2014;
            public const UInt16 tamper = 0x2015;
            public const UInt16 reserved2_supply_voltage_low = 0x2016;
            public const UInt16 battery_voltage_low = 0x2017;
            public const UInt16 corrosion_sense1 = 0x2018;
            public const UInt16 corrosion_sense2 = 0x2019;
            public const UInt16 corrosion_sense3 = 0x201A;
            public const UInt16 polarisation_cur_dc = 0x201B;
            public const UInt16 polarisation_cur_ac = 0x201C;
            public const UInt16 reserved3 = 0x201D;
            public const UInt16 meas_period = 0x201E;
            public const UInt16 reserved4 = 0x201F;
            public const UInt16 reserved5_meas_supply_period = 0x2020;
            public const UInt16 usikp_period = 0x2021;
            public const UInt16 corr_sense_period = 0x2022;
            public const UInt16 aux1_period = 0x2023;
            public const UInt16 aux2_period = 0x2024;
            public const UInt16 reserved6 = 0x2025;
            public const UInt16 shunt_nom = 0x2026;
            public const UInt16 polarisation_pot_en = 0x2027;
            public const UInt16 protection_pot_en = 0x2028;
            public const UInt16 protection_cur_en = 0x2029;
            public const UInt16 polarisation_cur_en = 0x202A;
            public const UInt16 induced_ac_en = 0x202B;
            public const UInt16 prot_pot_ext_range = 0x202C;
            public const UInt16 polarisation_cur_dc_en = 0x202D;
            public const UInt16 polarisation_cur_ac_en = 0x202E;
            public const UInt16 status_flags_en = 0x202F;
            public const UInt16 pdo_flags = 0x2030;
            public const UInt16 datetime = 0x2031;
        }

        #endregion

    }
}

