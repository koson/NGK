using System;
using System.Text;
using NGK.CAN.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.Collections;
using NGK.CAN.DataTypes.Helper;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles
{
    /// <summary>
    /// ��������� ������� ���������� ��� 9810 ������ 1
    /// </summary>
    public sealed class KIP9810v1: Prototype  
    {
        #region Fields And Properties
        
        private static KIP9810v1 _Instance;

        public static IProfile Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    _Instance = new KIP9810v1();
                }
                return _Instance; 
            }
        }

        private Version _SoftwareVersion;
        private Version _HardwareVersion;
        private ObjectInfoCollection _ObjectList;
        
        #endregion

        #region Constructors
        /// <summary>
        /// �����������
        /// </summary>
        private KIP9810v1()
        {
            _SoftwareVersion = new Version(1, 0);
            _HardwareVersion = new Version(1, 0);
            CreateObjectDictionary();
        }
        #endregion

        #region Methods
        /// <summary>
        /// ������ �������� �������� ������� �������� ����������
        /// </summary>
        private void CreateObjectDictionary()
        {
            _ObjectList = new ObjectInfoCollection();

            _ObjectList.Add(new ObjectInfo(0x2000, "device_type", "��� ����������",
                true, true, true, "��� ����������", String.Empty, Category.System,
                new NgkUInt16(ScalerTypes.x1), 9810));

            _ObjectList.Add(new ObjectInfo(0x2001, "fw_version", "������ ��", 
                true, true, true, "������ ��", String.Empty, Category.System,
                new NgkVersion(), 
                (new NgkVersion()).ConvertToBasis(new ProductVersion(_SoftwareVersion))));

            _ObjectList.Add(new ObjectInfo(0x2002, "hw_version", "������ ����������", 
                true, true, true, "������ ����������", String.Empty, Category.System,
                new NgkVersion(), 
                (new NgkVersion()).ConvertToBasis(new ProductVersion(_HardwareVersion))));

            _ObjectList.Add(new ObjectInfo(0x2003, "serial_number1", "�������� ����� ����������",
                true, true, true, "�������� ����� 1", String.Empty, Category.System,
                new NgkUInt16(ScalerTypes.x1), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2004, "serial_number2", "�������� ����� ����������",
                true, true, true, "�������� ����� 2", String.Empty, Category.System, 
                new NgkUInt16(ScalerTypes.x1), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2005, "serial_number3", "�������� ����� ����������", 
                true, true, true, "�������� ����� 3", String.Empty, Category.System,
                new NgkUInt16(ScalerTypes.x1), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2006, "vcard_chksum", "����������� ����� CRC16 �������� ����� ����������", 
                true, true, true, "����������� �����", String.Empty, Category.System,
                new NgkUInt16(ScalerTypes.x1), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2007, "vendor_id", "��� �������������", 
                true, true, true, "��� �������������", String.Empty, Category.System, 
                new NgkUInt16(ScalerTypes.x1), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2008, "polarization_pot", 
                "��������������� ���������, �", 
                false, true, true, "��������������� ���������", "B", Category.Measured,
                new NgkFloat(ScalerTypes.x001), (UInt32)0)); 

            _ObjectList.Add(new ObjectInfo(0x2009, "protection_pot", "�������� ���������, �",
                false, true, true, "U ��������","B", Category.Measured,
                new NgkFloat(ScalerTypes.x001), (UInt32)0)); 

            _ObjectList.Add(new ObjectInfo(0x200A, "induced_ac", "���������� ���������� ����������, �", 
                false, true, true, "U ���������� �������.", "B", Category.Measured, 
                new NgkUInt16(ScalerTypes.x1), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x200B, "protection_cur", "��� �������� ������, �",
                false, true, true, "I �������� ������", "A", Category.Measured,
                new NgkUFloat(ScalerTypes.x005), (UInt32)0)); 

            _ObjectList.Add(new ObjectInfo(0x200C, "polarization_cur", 
                "��� �����������, mA",
                false, true, true, "I �����������", "A", Category.Measured,
                new NgkFloat(ScalerTypes.x01), (UInt32)0)); 

            //uFloatNgk = new NgkUFloat(Precision.x001, 0);
            _ObjectList.Add(new ObjectInfo(0x200D, "aux_cur1", "��� ������ 1, mA",
                false, true, true, "I ������ 1", "mA", Category.Measured, 
                new NgkUFloat(ScalerTypes.x001), (UInt32)0));

            //uFloatNgk = new NgkUFloat(Precision.x001, 0);
            _ObjectList.Add(new ObjectInfo(0x200E, "aux_cur2", "��� ������ 2, mA",
                false, true, true, "I ������ 2", "mA", Category.Measured,
                new NgkUFloat(ScalerTypes.x001), (UInt32)0)); 

            _ObjectList.Add(new ObjectInfo(0x200F, "corrosion_depth", "������� ��������, ���",
                false, true, true, "������� ��������", "���", Category.Measured, 
                new NgkUInt16(ScalerTypes.x1), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2010, "corrosion_speed", "�������� ��������, ���/���", 
                false, true, true, "�������� ��������", "���/���", Category.Measured,
                new NgkUInt16(ScalerTypes.x1), (UInt32)0)); 

            _ObjectList.Add(new ObjectInfo(0x2011, "usipk_state", "��������� �������",
                false, true, true, "��������� �������", String.Empty, Category.Measured,
                new NgkByte(), (UInt32)0)); 

            _ObjectList.Add(new ObjectInfo(0x2012, "supply_voltage", 
                "�������� ����������, B", 
                false, true, true, "�������� ����������", "B", Category.Measured,
                new NgkUFloat(ScalerTypes.x005), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2013, "battery_voltage", 
                "���������� �������, B", 
                false, true, true, "U ���.", "B", Category.Measured,
                new NgkUFloat(ScalerTypes.x001), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2014, "int_temp", 
                "����������� � ����������� �������, \u00B0C",
                false, true, true, "����������� � ����������� �������", "C", Category.Measured,
                new NgkInt16(), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2015, "tamper", "��������", 
                false, false, true, "��������", String.Empty, Category.System, 
                new NgkBoolean(),  (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x2016, "supply_voltage_low","������� ����������",
                false, true, true, "���������� ������� ���� �����", String.Empty, Category.Measured,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x2017, "battery_voltage_low", "���������� ������� ���� �����",
                false, true, true, "������� ����������", String.Empty, Category.Measured,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x2018, "corrosion_sense1", "��������� ������� �������� 1",
                false, true, true, "������ �������� 1 ��������", String.Empty, Category.Measured,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x2019, "corrosion_sense2", "��������� ������� �������� 2",
                false, true, true, "������ �������� 2 ��������", String.Empty, Category.Measured,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x201A, "corrosion_sense3", "��������� ������� �������� 3",
                false, true, true, "������ �������� 3 ��������", String.Empty, Category.Measured,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x201B, "polarization_cur_dc", 
                "��� ��������� ���������� (��� ����������� ����� ������ � ���������� ���������)", 
                false, true, true, "��� ��������� ����������, mA", "mA", Category.Measured,
                new NgkFloat(ScalerTypes.x01), (UInt32)0)); 

            _ObjectList.Add(new ObjectInfo(0x201C, "polarization_cur_ac", 
                "��� ��������� ���������� (��� ����������� ����� ������ � ���������� ���������)",
                false, true, true, "I ��������� �����., mA", "mA", Category.Measured,
                new NgkFloat(ScalerTypes.x01), (UInt32)0)); 

            _ObjectList.Add(new ObjectInfo(0x201E, "meas_period", 
                "������ ��������� � �������� ������, ���", 
                false, true, true, "������ ��������� � �������� ������", "���.", Category.Configuration,
                new NgkUInt32(), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2020, "meas_supply_period", "������ ��������� ��������� ����������, ���", 
                false, true, true, "������ ��������� U�", "���.", Category.Configuration,
                new NgkUInt16(ScalerTypes.x1), (UInt32)10));

            _ObjectList.Add(new ObjectInfo(0x2021, "usipk_period", "������ ������ �������, ���",
                false, true, true, "������ ������ �������", "���.", Category.Configuration, 
                new NgkUInt16(ScalerTypes.x10), (UInt32)10));

            _ObjectList.Add(new ObjectInfo(0x2022, "corr_sense_period", "������ ������ �������� ��������, ���",
                false, true, true, "������ ������ �������� ��������", "���.", Category.Configuration,
                new NgkUInt16(ScalerTypes.x10), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2023, "aux1_period", "������ ������ ������ 1, ���",
                false, true, true, "������ ������ ������ 1", "���.", Category.Configuration, 
                new NgkUInt16(ScalerTypes.x10), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2024, "aux2_period", "������ ������ ������ 2, ���",
                false, true, true, "������ ������ ������ 2", "���.", Category.Configuration,
                new NgkUInt16(ScalerTypes.x10), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2026, "shunt_nom", "������� �����, �", 
                false, true, true, "������� �����", "A", Category.Configuration,
                new NgkUInt16(ScalerTypes.x1), (UInt32)50));

            _ObjectList.Add(new ObjectInfo(0x2027, "polarisation_pot_en", "���������� ��������� �����. ����������",
                false, true, true, "���������� ��������� �����. ����������", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x2028, "protection_pot_en", "���������� ��������� ��������� ����������",
                false, true, true, "���������� ��������� ��������� ����������", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x2029, "protection_cur_en", 
                "���������� ��������� ��������� ����",
                false, true, true, "���������� ��������� ��������� ����", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x202A, "polarisation_cur_en", 
                "���������� ��������� ���������������� ����",
                false, true, true, "���������� ��������� ���������������� ����", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x202B, "induced_ac_en", 
                "���������� ��������� ����������� ����������",
                false, true, true, "���������� ��������� ����������� ����������", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x202C, "prot_pot_ext_range", 
                "����������� �������� ��������� ����������",
                false, true, true, "����������� �������� ��������� ����������", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x202D, "polarization_cur_dc_en", 
                "���������� ��������� ����������� ���� ���������", 
                false, true, true, "���������� ��������� ����������� ���� ���������", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x202E, "polarization_cur_ac_en", 
                "���������� ��������� ����������� ���� ���������",
                false, true, true, "���������� ��������� ����������� ���� ���������", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x202F, "status_flags_en", 
                "���������� �������� ����� ���������", 
                false, true, true, "���������� �������� ����� ���������", String.Empty, Category.Configuration,
                new NgkBoolean(), (new NgkBoolean()).ConvertToBasis(false)));

            _ObjectList.Add(new ObjectInfo(0x2030, "pdo_flags", 
                "���������� ��� ���������� �������� PDO",
                false, true, true, "���������� ��� ���������� �������� PDO", String.Empty, Category.Configuration,
                new NgkUInt16(ScalerTypes.x1), (UInt32)0));

            _ObjectList.Add(new ObjectInfo(0x2031, "datetime", "������� �����",
                true, true, true, "������� �����", String.Empty, Category.System,
                new NgkDateTime(), (new NgkDateTime()).ConvertToBasis(DateTime.Now)));

            //// ���������� ������� �� ���������� �������
            //base.CreateObjectDictionary();

            return;
        }
        #endregion

        #region IDeviceProfile Members

        public override DeviceType DeviceType
        {
            get { return DeviceType.KIP_MAIN_POWERED_v1; }
        }

        public override string Description
        {
            get { return @"���������� ��� � �������� �� �������� ����"; }
        }

        public override Version SoftwareVersion
        {
            get { return _SoftwareVersion; }
        }

        public override Version HardwareVersion
        {
            get { return _HardwareVersion; }
        }

        public override ObjectInfoCollection ObjectInfoList
        {
            get 
            {
                return _ObjectList;
            }
        }

        #endregion
    }
}
