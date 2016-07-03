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
    /// ��������� ������� ���������� ��� 9811 ������ 1
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
            get { return @"���������� ��� � ���������� �������� �� ���"; }
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
        /// �����������
        /// </summary>
        private KIP9811v1(): base()
        {
            SetSoftwareVersion(new Version(1, 0));
            SetHardwareVersion(new Version(1, 0));
        }

        #endregion

        #region Methods

        /// <summary>
        /// ������ �������� �������� ������� �������� ����������
        /// </summary>
        protected override void CreateObjectDictionary()
        {
            base.CreateObjectDictionary();

            // ���������� ������� (���������) �������:
            _ComplexParameters.Add(new ComplexParameter("SerialNumber", "�������� �����",
                "�������� ����� ����������", true, true, string.Empty, 
                ObjectCategory.System, new SerialNumberConverter(), (UInt64)0,
                0x2003, 0x2004, 0x2005));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2000, "device_type", "��� ����������",
                true, true, true, "��� ����������", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt16)this.DeviceType));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2001, "fw_version", "������ ��",
                true, true, true, "������ ��", String.Empty, ObjectCategory.System,
                NgkProductVersionConvertor.Instance, 
                NgkProductVersionConvertor.Instance.ConvertToBasis(new NgkProductVersion(new Version(1, 0)))));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2002, "hw_version", "������ ����������",
                true, true, true, "������ ����������", String.Empty, ObjectCategory.System,
                NgkProductVersionConvertor.Instance,
                NgkProductVersionConvertor.Instance.ConvertToBasis(new NgkProductVersion(new Version(1, 0)))));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2003, "serial_number1", "�������� ����� ����������",
                true, true, true, "�������� ����� 1", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0,
                _ComplexParameters["SerialNumber"].Name));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2004, "serial_number2", "�������� ����� ����������",
                true, true, true, "�������� ����� 2", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0,
                _ComplexParameters["SerialNumber"].Name));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2005, "serial_number3", "�������� ����� ����������",
                true, true, true, "�������� ����� 3", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0,
                _ComplexParameters["SerialNumber"].Name));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2006, "vcard_chksum", "����������� ����� CRC16 �������� ����� ����������",
                true, true, true, "����������� �����", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2007, "vendor_id", "��� �������������",
                false, true, true, "��� �������������", String.Empty, ObjectCategory.System,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2008, "polarization_pot",
                "��������������� ���������, �",
                false, true, true, "��������������� ���������", "B", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x001), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2009, "protection_pot", "�������� ���������, �",
                false, true, true, "U ��������", "B", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x001), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x200A, "induced_ac", "���������� ���������� ����������, �",
                false, true, true, "U ���������� �������.", "B", ObjectCategory.Measured,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x200B, "protection_cur", "��� �������� ������, �",
                false, true, true, "I �������� ������", "A", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x005), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x200C, "polarization_cur",
                "��� �����������, mA",
                false, true, true, "I �����������", "A", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x01), (UInt32)0));

            //uFloatNgk = new NgkUFloat(Precision.x001, 0);
            _ObjectInfoList.Add(new ObjectInfo(this, 0x200D, "aux_cur1", "��� ������ 1, mA",
                false, true, true, "I ������ 1", "mA", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x001), (UInt32)0));

            //uFloatNgk = new NgkUFloat(Precision.x001, 0);
            _ObjectInfoList.Add(new ObjectInfo(this, 0x200E, "aux_cur2", "��� ������ 2, mA",
                false, true, true, "I ������ 2", "mA", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x001), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x200F, "corrosion_depth", "������� ��������, ���",
                false, true, true, "������� ��������", "���", ObjectCategory.Measured,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2010, "corrosion_speed", "�������� ��������, ���/���",
                false, true, true, "�������� ��������", "���/���", ObjectCategory.Measured,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2011, "usipk_state", "��������� �������",
                false, true, true, "��������� �������", String.Empty, ObjectCategory.Measured,
                new NgkByteConverter(), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2012, "supply_voltage",
                "�������� ����������, B",
                false, true, true, "�������� ����������", "B", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x005), (UInt32)0, null));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2013, "battery_voltage",
                "���������� �������, B",
                false, true, true, "U ���.", "B", ObjectCategory.Measured,
                new NgkUFloatConvertor(ScalerTypes.x001), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2014, "int_temp",
                "����������� � ����������� �������, \u00B0C",
                false, true, true, "����������� � ����������� �������", "C", ObjectCategory.Measured,
                new NgkInt16Converter(), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2015, "tamper", "��������",
                false, false, true, "��������", String.Empty, ObjectCategory.System,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2016, "supply_voltage_low", "������� ����������",
                false, true, true, "���������� ������� ���� �����", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2017, "battery_voltage_low", "���������� ������� ���� �����",
                false, true, true, "������� ����������", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2018, "corrosion_sense1", "��������� ������� �������� 1",
                false, true, true, "������ �������� 1 ��������", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2019, "corrosion_sense2", "��������� ������� �������� 2",
                false, true, true, "������ �������� 2 ��������", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x201A, "corrosion_sense3", "��������� ������� �������� 3",
                false, true, true, "������ �������� 3 ��������", String.Empty, ObjectCategory.Measured,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x201B, "polarization_cur_dc",
                "��� ��������� ���������� (��� ����������� ����� ������ � ���������� ���������)",
                false, true, true, "��� ��������� ����������, mA", "mA", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x01), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x201C, "polarization_cur_ac",
                "��� ��������� ���������� (��� ����������� ����� ������ � ���������� ���������)",
                false, true, true, "I ��������� �����., mA", "mA", ObjectCategory.Measured,
                new NgkFloatConverter(ScalerTypes.x01), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x201E, "meas_period",
                "������ ��������� � �������� ������, ���",
                false, true, true, "������ ��������� � �������� ������", "���.", ObjectCategory.Configuration,
                new NgkUInt32Convertor(), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2020, "meas_supply_period", "������ ��������� ��������� ����������, ���",
                false, true, true, "������ ��������� U�", "���.", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)10));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2021, "usipk_period", "������ ������ �������, ���",
                false, true, true, "������ ������ �������", "���.", ObjectCategory.Configuration,
                new NgkUInt16WithStatusDisabledConverter(ScalerTypes.x10), 0xFFFF)); //10

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2022, "corr_sense_period", "������ ������ �������� ��������, ���",
                false, true, true, "������ ������ �������� ��������", "���.", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x10), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2023, "aux1_period", "������ ������ ������ 1, ���",
                false, true, true, "������ ������ ������ 1", "���.", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x10), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2024, "aux2_period", "������ ������ ������ 2, ���",
                false, true, true, "������ ������ ������ 2", "���.", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x10), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2026, "shunt_nom", "������� �����, �",
                false, true, true, "������� �����", "A", ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)50));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2027, "polarisation_pot_en", "���������� ��������� �����. ����������",
                false, true, true, "���������� ��������� �����. ����������", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2028, "protection_pot_en", "���������� ��������� ��������� ����������",
                false, true, true, "���������� ��������� ��������� ����������", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2029, "protection_cur_en",
                "���������� ��������� ��������� ����",
                false, true, true, "���������� ��������� ��������� ����", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202A, "polarisation_cur_en",
                "���������� ��������� ���������������� ����",
                false, true, true, "���������� ��������� ���������������� ����", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202B, "induced_ac_en",
                "���������� ��������� ����������� ����������",
                false, true, true, "���������� ��������� ����������� ����������", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202C, "prot_pot_ext_range",
                "����������� �������� ��������� ����������",
                false, true, true, "����������� �������� ��������� ����������", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202D, "polarization_cur_dc_en",
                "���������� ��������� ����������� ���� ���������",
                false, true, true, "���������� ��������� ����������� ���� ���������", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202E, "polarization_cur_ac_en",
                "���������� ��������� ����������� ���� ���������",
                false, true, true, "���������� ��������� ����������� ���� ���������", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x202F, "status_flags_en",
                "���������� �������� ����� ���������",
                false, true, true, "���������� �������� ����� ���������", String.Empty, ObjectCategory.Configuration,
                new NgkBooleanConverter(), (new NgkBooleanConverter()).ConvertToBasis(false)));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2030, "pdo_flags",
                "���������� ��� ���������� �������� PDO",
                false, true, true, "���������� ��� ���������� �������� PDO", String.Empty, ObjectCategory.Configuration,
                new NgkUInt16Convertor(ScalerTypes.x1), (UInt32)0));

            _ObjectInfoList.Add(new ObjectInfo(this, 0x2031, "datetime", "������� �����",
                true, true, true, "������� �����", String.Empty, ObjectCategory.Measured,
                new NgkDateTimeConverter(), (new NgkDateTimeConverter()).ConvertToBasis(DateTime.Now)));

            //// ���������� ������� �� ���������� �������
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
        /// ������� �������� ������� ����������
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

