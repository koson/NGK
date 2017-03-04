using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
using System.Drawing.Design;
using System.Text;
using NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters;
using NGK.MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors;

//==============================================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice
{
    //==========================================================================================================
    /// <summary>
    /// ����� ��������� ���������� �� (���� ���������) �� ���������� ������� ��(�)-00 (9810)
    /// </summary>
    [Serializable]
    public class MeasuringDeviceMainPower : System.ComponentModel.INotifyPropertyChanged, IMeasuringDevice,
        System.Runtime.Serialization.IDeserializationCallback
    {
        //------------------------------------------------------------------------------------------------------
        #region Fields and Properties
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����� ���������� ��
        /// </summary>
        private Byte _AddressSlave = 1;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����� ���������� �� (��� ������ �� ���������������� ������)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ����")]
        [Description("����� ���������� ���-�� (��� ������ �� ���������������� ������)")]
        [DisplayName(@"����� ���������� ���-�� ������")]
        [DefaultValue(typeof(Byte), "1")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Byte AddressSlave
        {
            get { return _AddressSlave; }
            set { _AddressSlave = value; }
        }
        //------------------------------------------------------------------------------------------------------
        #region Input Register
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ��� ���������� ���������� �� (�� ���������� ��� �������� �������)
        /// </summary>
        /// <remarks>
        /// Input registr 0x0000
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ������")]
        [Description("������� ���������� ���������� ���-�� � ������� ���-���")]
        [DisplayName(@"��� ����������")]
        [TypeConverter(typeof(TypeConverterTypeOfDeviceNGK))]
        public TYPE_NGK_DEVICE TypeOfDevice
        {
            get { return TYPE_NGK_DEVICE.BI_MAIN_POWERED; }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ��
        /// </summary>
        /// <remarks>
        /// Input Register 0x0001
        /// </remarks>
        private float _SofwareVersion = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ��
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ������")]
        [Description("������ ��")]
        [DisplayName("������ ��")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float SofwareVersion
        {
            get { return _SofwareVersion; }
            set
            {
                _SofwareVersion = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("SofwareVersion"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ����������
        /// </summary>
        /// <remarks>
        /// Input Register 0x0002
        /// </remarks>
        private float _HardwareVersion = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ����������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ������")]
        [Description("������ ����������")]
        [DisplayName("������ ����������")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float HardwareVersion
        {
            get { return _HardwareVersion; }
            set
            {
                _HardwareVersion = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("HardwareVersion"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ����� ����������
        /// </summary>
        /// <remarks>
        /// Input register 0x000C � 0x000D
        /// </remarks>
        private UInt64 _SerialNumber = UInt64.MaxValue;
        /// <summary>
        /// �������� ����� ����������
        /// </summary>
        //------------------------------------------------------------------------------------------------------
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ������")]
        [Description("�������� ����� ����������")]
        [DisplayName("�������� �����")]
        [DefaultValue(typeof(UInt64), "0xFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt64 SerialNumber
        {
            get { return _SerialNumber; }
            set
            {
                _SerialNumber = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("SerialNumber"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����������� ����� �������� ��������
        /// </summary>
        private UInt16 _CRC16 = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����������� ����� �������� ��������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ������")]
        [Description("����������� ����� �������� �������� ����������")]
        [DisplayName("����������� �����")]
        //[DefaultValue(typeof(UInt64), "0xFFFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 CRC16
        {
            get { return _CRC16; }
            set { _CRC16 = value; }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� ������������� 
        /// </summary>
        private UInt16 _CodeManufacturer = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� �������������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ������")]
        [Description("��� �������������")]
        [DisplayName("��� �������������")]
        //[DefaultValue(typeof(UInt64), "0xFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 CodeManufacturer
        {
            get { return _CodeManufacturer; }
            set 
            { 
                _CodeManufacturer = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("CodeManufacturer"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������������� ���������
        /// </summary>
        /// <remarks>
        /// �������������� �� UInt16 � ������ -2.00...+2.00 �
        /// Input Register
        /// </remarks>
        private float _polarization_potential;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������������� ���������
        /// </summary>
        /// <remarks>
        /// Input Register 0x0008
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��������������� ��������� ���������� ������������ �� ������ ����������������")]
        [DisplayName("��������������� ���������, �")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float PolarizationPotential
        {
            get { return _polarization_potential; }
            set 
            { 
                _polarization_potential = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("PolarizationPotential"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <remarks>
        /// �������������� �� UInt16 � ������ -10.00...+10.00 �
        /// Input Register
        /// </remarks>
        private float _protective_potential;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ���������
        /// </summary>
        /// <remarks>
        /// Input Register 0x0009
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("�������� ���������, �")]
        [DisplayName("�������� ���������, �")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float ProtectivePotential
        {
            get { return _protective_potential; }
            set
            {
                _protective_potential = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("ProtectivePotential"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� ���������� ���������� �� ����������� �� 0 �� 100,0 � �������� 50��
        /// </summary>
        /// <remarks>
        /// Input Register 0x000A
        /// </remarks>
        private float _InducedVoltage;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� ���������� ���������� �� ����������� �� 0 �� 100,0 � �������� 50��
        /// </summary>
        /// <remarks>
        /// Input Register 0x000A
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��������� ���������� ���������� �� ����������� �� 0 �� 100,0 � �������� 50��")]
        [DisplayName("��������� ����������, �")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float InducedVoltage
        {
            get { return _InducedVoltage; }
            set
            {
                _InducedVoltage = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("InducedVoltage"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ���
        /// </summary>
        /// <remarks>
        /// �������������� �� UInt16 � ������ 0.0...+150.0 A
        /// </remarks>
        /// <remarks>
        /// Input Register 0x000B
        /// </remarks>
        private float _protective_current;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ���
        /// </summary>
        /// <remarks>
        /// Input Register 0x000B
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��� �������� ������ � ����� ������� ������� ��������� ���������� �� ������� �����")]
        [DisplayName("��� �������� ������, �")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float Protective�urrent
        {
            get { return _protective_current; }
            set
            {
                _protective_current = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("Protective�urrent"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������������� ���
        /// </summary>
        /// <remarks>
        /// �������������� �� UInt16 � ������ -5.00...+5.00 mA
        /// </remarks>
        /// <remarks>
        /// Input Register 0x000C
        /// </remarks>
        private float _polarization_current;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������������� ���
        /// </summary>
        /// <remarks>
        /// Input Register 0x000C
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��� ����������� ���������������� ���������")]
        [DisplayName("��� �����������, mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float Polarization�urrent
        {
            get { return _polarization_current; }
            set
            {
                _polarization_current = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("Polarization�urrent"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� �������������� ������ 1
        /// </summary>
        /// <remarks>
        /// Input Register 0x0005
        /// 0�02FF- 4mA-0x0EF9- 20 mA; 1bit-0,01mA
        /// ���������� �������� ���, ��������������� ��������� 4-20 ��.
        /// ��������, ��������������� ������ 4 �� ��������� ������� �������������� ������, ����� 20 �� - ��
        /// </remarks>
        private float _CurrentChannel1;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� �������������� ������ 1
        /// </summary>
        /// <remarks>
        /// Input Register 0x0005
        /// 0�02FF- 4mA-0x0EF9- 20 mA; 1bit-0,01mA
        /// ���������� �������� ���, ��������������� ��������� 4-20 ��.
        /// ��������, ��������������� ������ 4 �� ��������� ������� �������������� ������, ����� 20 �� - ��
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��� �������������� ������ 1")]
        [DisplayName("��� �������������� ������ 1 (4-20), mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float CurrentChannel1
        {
            get { return _CurrentChannel1; }
            set
            {
                _CurrentChannel1 = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentChannel1"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� �������������� ������ 2
        /// </summary>
        /// <remarks>
        /// Input Register 0x0006
        /// 0�02FF- 4mA-0x0EF9- 20 mA; 1bit-0,01mA
        /// ���������� �������� ���, ��������������� ��������� 4-20 ��.
        /// ��������, ��������������� ������ 4 �� ��������� ������� �������������� ������, ����� 20 �� - ��
        /// </remarks>
        private float _CurrentChannel2;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� �������������� ������ 2
        /// </summary>
        /// <remarks>
        /// Input Register 0x0006
        /// 0�02FF- 4mA-0x0EF9- 20 mA; 1bit-0,01mA
        /// ���������� �������� ���, ��������������� ��������� 4-20 ��.
        /// ��������, ��������������� ������ 4 �� ��������� ������� �������������� ������, ����� 20 �� - ��
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��� �������������� ������ 2")]
        [DisplayName("��� �������������� ������ 2 (4-20), mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float CurrentChannel2
        {
            get { return _CurrentChannel2; }
            set
            {
                _CurrentChannel2 = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentChannel1"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������� �������� ������� ��� � ���������� �������
        /// </summary>
        /// <remarks>
        /// �������������� �� UInt16 � ������ 0...1200 ���
        /// Input Register 0x0007
        /// </remarks>
        private UInt16 _DepthOfCorrosion;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������� �������� ������� ��� � ���������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("������� �������� ������� ��� � ���������� �������")]
        [DisplayName("������� �������� �������, ���")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 DepthOfCorrosion
        {
            get { return _DepthOfCorrosion; }
            set
            {
                if (value > 0xFFFE)
                {
                    throw new ArgumentOutOfRangeException("DepthOfCorrosion",
                        "������� ��������� ��������� ������������ ��������. �������� ���������� �������� 0x0000...0xFFFE");
                }
                else
                {
                    _DepthOfCorrosion = value;
                    // ���������� �������
                    OnPropertyChanged(new PropertyChangedEventArgs("DepthOfCorrosion"));
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// C������� �������� ������� ��� � ���������� �������
        /// </summary>
        /// <remarks>
        /// �������������� �� UInt16 � ������ 0...65.534 ���/���
        /// Input Register 0x0008
        /// </remarks>
        private float _SpeedOfCorrosion;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// C������� �������� ������� ��� � ���������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("�������� �������� ������� ��� � ���������� �������")]
        [DisplayName("�������� �������� �������, ���")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float SpeedOfCorrosion
        {
            get { return _SpeedOfCorrosion; }
            set
            {
                _SpeedOfCorrosion = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("SpeedOfCorrosion"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������
        /// </summary>
        private UInt16 _StatusUSIKPST;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������
        /// </summary>
        /// <remarks>
        /// Input Register 0x0009
        /// 0x0000 - ����� 0xFFFF � ��� ����� ���� ��� 
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��������� ���������� ������� ���� ������ ��������� ����:\n 0 - ���������� � �����;\n 65535 - ��� �����;\n 1 - ������������ ������� (�� �������������� �����������);\n 2 � ���������������;\n 3 - �� ��������� ��������� ������������ ���������;\n 4 - ����������� ���������� ��� ���������� ������� ������ (����� ����������������);\n 5 - �������� �������� ������ �� �������������� ����������� (����� ����������������);\n 6 - ������ ��� ���������� �� �������������;\n 7 � ��������� ������������ ��������� �� ���������������;\n 8 � ������� ���� �����������;\n 9 - ���������� ���������� ��������� �� ���.")]
        [DisplayName("��� ��������� ���������� �������")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 StatusUSIKPST
        {
            get { return _StatusUSIKPST; }
            set
            {
                _StatusUSIKPST = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("StatusUSIKPST"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ���������� (����������� �������)
        /// </summary>
        /// <remarks>
        /// Input Register 0x000A
        /// �������� 9-15 � ��� ��(�) �� ���������� �������, 18-55 � ��� ��������� ��(�
        /// 1 bit == 0,05 �; 9-55� (0�00B4-0x044� hex)
        /// </remarks>
        private float _SupplyVoltage;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ����������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("���������� ������� ���-��, �")]
        [DisplayName("���������� ������� ���-��, �")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float SupplyVoltage
        {
            get { return _SupplyVoltage; }
            set
            {
                _SupplyVoltage = (float)(value * 0.05);
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("SupplyVoltage"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����������� �������� �������
        /// </summary>
        /// <remarks>
        /// Input Register 0x000B
        /// 1 bit == 0,01 � 
        /// 1,8-3,6� (0�00�4-0x0168 hex) 
        /// </remarks>
        private float _BattaryVoltage;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����������� �������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("���������� �������, �")]
        [DisplayName("���������� �������, �")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float BattaryVoltage
        {
            get { return _BattaryVoltage; }
            set
            {
                _BattaryVoltage = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("BattaryVoltage"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����������� ����������� ������� ��(�) ������ ��� ��-�-01
        /// </summary>
        /// <remarks>
        /// Input Register 0x000C 
        /// 1 bit == 1 ��
        /// -40/+85�� (������� ��������� ����� ��������� ����� ���������)
        /// � �������������� ����
        /// </remarks>
        //private Int16 _InternalTemperatureSensor;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����������� ����������� ������� ��(�) ������ ��� ��-�-01
        /// </summary>
        //[Browsable(true)]
        //[ReadOnly(true)]
        //[Category("���������� ���������")]
        //[Description("����������� ����������� �������, ��.�. ������ ��� ��-�-01")]
        //[DisplayName("����������� ����������� �������, ��.�")]
        //[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        //public Int16 InternalTemperatureSensor
        //{
        //    get { return _InternalTemperatureSensor; }
        //    set
        //    {
        //        _InternalTemperatureSensor = value;
        //        // ���������� �������
        //        OnPropertyChanged(new PropertyChangedEventArgs("InternalTemperatureSensor"));
        //    }
        //}
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� ��������� ����������
        /// </summary>
        /// <remarks>
        /// Input Register 0x0015
        /// 1 bit == 0,01 mA 
        /// -5...+5 mA (0�FE0C-0x01F4 hex) (�������������� ���) 
        /// </remarks>
        private float _ReferenceElectrodeDc�urrent;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� ��������� ����������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��� ��������� ����������, mA")]
        [DisplayName("��� ��������� ����������, mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float ReferenceElectrodDc�urrent
        {
            get { return _ReferenceElectrodeDc�urrent; }
            set
            {
                _ReferenceElectrodeDc�urrent = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("LeakageDc�urrent"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� ��������� ����������
        /// </summary>
        /// <remarks>
        /// Input Register 0x0016
        /// 1 bit == 0,01 mA 
        /// 0...+5 mA (0�0000-0x01F4 hex) 
        /// </remarks>
        private float _ReferenceElectrodeAc�urrent;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��� ��������� ����������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��� ��������� ����������, mA")]
        [DisplayName("��� ��������� ����������, mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float ReferenceElectrodeAc�urrent
        {
            get { return _ReferenceElectrodeAc�urrent; }
            set
            {
                _ReferenceElectrodeAc�urrent = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("LeakageAc�urrent"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Discrete Input
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ �������� ���-��� ��(�)
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0000
        /// </remarks>
        private Boolean _caseOpen;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ �������� ���-��� ��(�)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ����� ���������")]
        [Description("��������� ������� �������� ���-��� ��(�)")]
        [DisplayName("������ �������� ���-��� ��(�)")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean CaseOpen
        {
            get { return _caseOpen; }
            set
            {
                _caseOpen = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("CaseOpen"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� ���������� �������� ���������� �� (����./������.)
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0001
        /// ���������� ������� ���� �����
        /// ����� ������������ ������ ��� ��(�) �� ���������� ������� U���� < 13,6�, ���
        /// U����=U���K1*|t|.
        /// ��� ���������� ������� ��� U ���. < 20�
        /// ������� ��������� U��� 0,05�
        /// </remarks>
        private Boolean _SupplyVoltageStatus;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� ���������� �������� ���������� �� (����./������.)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ����� ���������")]
        [Description("��������� ���������� �������� ���������� ���-��,  Discretes Input	0x0001")]
        [DisplayName("��������� ���������� �������� ���������� ���-��")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean SupplyVoltageStatus
        {
            get { return _SupplyVoltageStatus; }
            set
            {
                _SupplyVoltageStatus = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("SupplyVoltageStatus"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������� �������
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0002
        /// �����/���� �����
        /// ����� ������������ ������ U���� < 3,4�, ���
        /// U����=U���K1*|t|.
        /// ������� ��������� U��� 0,01�
        /// </remarks>
        private Boolean _BattaryStatus;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������� �������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ����� ���������")]
        [Description("��������� �������� �������")]
        [DisplayName("������� �������, �����")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean BattaryStatus
        {
            get { return _BattaryStatus; }
            set
            {
                _BattaryStatus = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("BattaryStatus"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������� ������� "1" �������� ��������
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0003
        /// </remarks>
        private Boolean _CorrosionSensor_1;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������� ������� "1" �������� ��������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��������� �������� ������� �1 �������� ��������")]
        [DisplayName("������ �������� �1")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean CorrosionSensor1
        {
            get { return _CorrosionSensor_1; }
            set
            {
                _CorrosionSensor_1 = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("CorrosionSensor1"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������� ������� "2" �������� ��������
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0004
        /// </remarks>
        private Boolean _CorrosionSensor_2;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������� ������� "2" �������� ��������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��������� �������� ������� �2 �������� ��������")]
        [DisplayName("������ �������� �2")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean CorrosionSensor2
        {
            get { return _CorrosionSensor_2; }
            set
            {
                _CorrosionSensor_2 = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("CorrosionSensor2"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������� ������� "3" �������� ��������
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0005
        /// </remarks>
        private Boolean _CorrosionSensor_3;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� �������� ������� "3" �������� ��������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("���������� ���������")]
        [Description("��������� �������� ������� �3 �������� ��������")]
        [DisplayName("������ �������� �3")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean CorrosionSensor3
        {
            get { return _CorrosionSensor_3; }
            set
            {
                _CorrosionSensor_3 = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("CorrosionSensor3"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Holding Registers
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����� ���������� � ����� CAN � ����� ���������� � ���� Modbus
        /// </summary>
        /// <remarks>
        /// Holding register 0x0000
        /// 1...127 (�� ��������� 127)
        /// </remarks>
        private Byte _NetAddress = 127;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����� ���������� � ����� CAN � ����� ���������� � ���� Modbus
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ����")]
        [Description("����� ���������� � ����� CAN � ����� ���������� � ���� Modbus (1...127, �� ��������� 127)")]
        [DisplayName(@"�����/����� ����������")]
        [DefaultValue(typeof(Byte), "127")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Byte NetAddress
        {
            get { return _NetAddress; }
            set
            {
                if ((value > 0) && (value < 128))
                {
                    _NetAddress = value;
                    // ���������� �������
                    OnPropertyChanged(new PropertyChangedEventArgs("NetAddress"));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("NetAddress",
                        "������� ��������� ������������ �������� ���������. �������� ��������� ������ ���������� � ��������� 1...127");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ��������� � �������� ����������
        /// </summary>
        /// <remarks>
        /// Holding register 0x0001 � 0x0002
        /// 1 bit == 1 ���.
        /// 0���.�7���. (0�0000-0x93A80 hex)
        /// 0 � �������� ���������. 0xFFFFFFFF -�������� ������ ������ �� �������. 
        /// ��� ��(�) �� ���������� ������� �������� ��������� 0x00 � 0x0A �����������. ���������� ���������� 0x03.
        /// </remarks>
        private UInt32 _MeasuringPeriod = 0xFFFFFFFF;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ��������� � �������� ����������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������")]
        [Description("������ ��������� � �������� ���������� (�� ��������� 1 ���.). 0 � �������� ���������. ����������� ������ � ���������� ������� ����������������� � �������� ������ ������ �� �������")]
        [DisplayName("������ ���������, ���.")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [DefaultValue(typeof(UInt32), "0xFFFFFFFF")]
        [TypeConverter(typeof(NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters.TypeConverterMeasuringPeriod))]
        [Editor(typeof(MeasuringPeriodUITypeEditor), 
            typeof(System.Drawing.Design.UITypeEditor))]
        public UInt32 MeasuringPeriod
        {
            get { return _MeasuringPeriod; }
            set
            {
                if (value <= 0x93A80)
                {
                    _MeasuringPeriod = value;
                    // ���������� �������
                    OnPropertyChanged(new PropertyChangedEventArgs("MeasuringPeriod"));
                }
                else if (value == 0xFFFFFFFF)
                {
                    _MeasuringPeriod = value;
                    // ���������� �������
                    OnPropertyChanged(new PropertyChangedEventArgs("MeasuringPeriod"));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MeasuringPeriod", 
                        "�������� ��� ����������� ��������� ��������");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ��������� ��������� ���������� ��� ������ ��(�)-00
        /// </summary>
        /// <remarks>
        /// Holding register 0x0003
        /// 1 bit == 1 ���. 
        /// 1���.�100���. (0�0001-0x0064 hex)
        /// ������ ��(�)-01 �������� �������� ���������� � ���������� ����������� �������� 
        /// ������� ������ ��� ����� ��������� ������� ��������� � �������� �� ������ ���� � �������� �����
        /// </remarks>
        private UInt16 _MeasuringVoltagePeriod = 0x000A;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ��������� ��������� ���������� ��� ������ ��(�)-00
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������")]
        [Description("������ ���-��(�)-01 �������� �������� ���������� � ���������� ����������� �������� ������� ������ ��� ����� ��������� ������� ��������� � �������� �� ������ ���� � �������� �����. 1...100 ���.")]
        [DisplayName("������ ��������� ��������� ����������, ���.")]
        [DefaultValue(typeof(UInt16),"0x000A")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 MeasuringVoltagePeriod
        {
            get { return _MeasuringVoltagePeriod; }
            set
            {
                if ((value > 100) || (value == 0))
                {
                    throw new ArgumentOutOfRangeException("MeasuringVoltagePeriod", 
                        "�������� �� ������ � �������� ���������� �������� 1...100");
                }
                else
                {
                    _MeasuringVoltagePeriod = value;
                    // ���������� �������
                    OnPropertyChanged(new PropertyChangedEventArgs("MeasuringVoltagePeriod"));
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ������ ������� (1...65535 ���)
        /// </summary>
        /// <remarks>
        /// Holding register 0x0004
        /// 1 bit == 10 ���.
        /// 10���.�7���.(0�0001-0xFFFF hex)
        /// 0xFFFF � ���������� ����� �������� ����� ������ ������ �� ���� ������� ���������, ���� ��� �� ��������
        /// </remarks>
        private UInt32 _PollingPeriodUSIKPST = 0xFFFF * 10;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ������ ������� (1...65535 ���)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������")]
        [Description("������ ������ ������� �� 10 ��� �� 7 ���.")]
        [DisplayName("������ ������ �������, ���")]
        [DefaultValue(typeof(UInt32), "655350")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [Editor(typeof(Channel4_20TypeEditor),
            typeof(UITypeEditor))]
        [TypeConverter(typeof(NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters.TypeConverterChannel4_20))]
        public UInt32 PollingPeriodUSIKPST
        {
            get { return _PollingPeriodUSIKPST; }
            set
            {
                if (value != 0)
                {
                    if (value == 0xFFFF * 10)
                    {
                        // ����� ��������
                        _PollingPeriodUSIKPST = value;
                        // ���������� �������
                        OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodUSIKPST"));

                    }
                    else
                    {
                        // �������� ������ ���� ������ 10
                        uint x = value % 10;
                        if (x != 0)
                        {
                            throw new ArgumentException(
                                "������� ��������� �������� �������� �� ������� 10",
                                "PollingPeriodUSIKPST");
                        }
                        else
                        {
                            // ����� �������
                            _PollingPeriodUSIKPST = value;
                            // ���� ����� ������ ������� �������, �� ���������� ���������
                            // ����� ������ �������� ���
                            this.PollingPeriodBPI = 0xFFFF * 10;
                            // ���������� �������
                            OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodUSIKPST"));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("������� ��������� �������� ������������ �������� 0",
                        "PollingPeriodUSIKPST");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ������ �������� ���
        /// </summary>
        /// <remarks>
        /// Holding register 0x0005
        /// 1 bit == 10 ���.
        /// 10���.�7���. (0�0001-0xFFFF hex)
        /// 0xFFFF � ���������� ����� �������� ����� ������ ������ �� ���� ������� ���������, ���� ��� �� ��������
        /// </remarks>
        private UInt32 _PollingPeriodBPI = 0xFFFF * 10;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ������ �������� ���
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������")]
        [Description("������ ������ ������� ��� �� 10 ��� �� 7 ���.")]
        [DisplayName("������ ������ ���, ���")]
        [DefaultValue(typeof(UInt32), "655350")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [Editor(typeof(Channel4_20TypeEditor),
            typeof(UITypeEditor))]
        [TypeConverter(typeof(NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters.TypeConverterChannel4_20))]
        public UInt32 PollingPeriodBPI
        {
            get { return _PollingPeriodBPI; }
            set
            {
                if (value != 0)
                {
                    if (value == 0xFFFF * 10)
                    {
                        // ����� ��������
                        _PollingPeriodBPI = value;
                        // ���������� �������
                        OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodBPI"));

                    }
                    else
                    {
                        // �������� ������ ���� ������ 10
                        uint x = value % 10;
                        if (x != 0)
                        {
                            throw new ArgumentException(
                                "�������� �������� �� ������ 10",
                                "PollingPeriodBPI");
                        }
                        else
                        {
                            // ����� �������
                            _PollingPeriodBPI = value;
                            // ���� ����� ������ ������� �������, �� ���������� ���������
                            // ����� ������ ��������� �������
                            this.PollingPeriodUSIKPST = 0xFFFF * 10;
                            // ���������� �������
                            OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodBPI"));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("������� ��������� �������� ������������ �������� 0",
                        "PollingPeriodBPI");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ������ �������������� ������ 1: 4-20 ��
        /// </summary>
        /// <remarks>
        /// Holding register 0x0006
        /// 1 bit == 10 ���.
        /// 10���.�7���. (0�0001-0xFFFF hex)
        /// 0xFFFF � ���������� �����
        /// </remarks>
        private UInt32 _PollingPeriodChannel1 = 0xFFFF * 10; // ����� �� ��������� ��������
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        ///  ������ ������ �������������� ������ 1: 4-20 ��
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������")]
        [Description("������ ������ ������ 1 �� 10 ��� �� 7 ���.")]
        [DisplayName("������ ������ ������ 1, ���")]
        [DefaultValue(typeof(UInt32), "655350")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [Editor(typeof(Channel4_20TypeEditor),
            typeof(UITypeEditor))]
        [TypeConverter(typeof(NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters.TypeConverterChannel4_20))]
        public UInt32 PollingPeriodChannel1
        {
            get { return _PollingPeriodChannel1; }
            set
            {
                if (value != 0)
                {
                    if (value == 0xFFFF * 10)
                    {
                        // ����� ��������
                        _PollingPeriodChannel1 = value;
                        // ���������� �������
                        OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodChannel1"));

                    }
                    else
                    {
                        // �������� ������ ���� ������ 10
                        uint x = value % 10;
                        if (x != 0)
                        {
                            throw new ArgumentException(
                                "�������� �������� �� ������ 10",
                                "PollingPeriodChannel1");
                        }
                        else
                        {
                            // ����� �������
                            _PollingPeriodChannel1 = value;
                            // ���������� �������
                            OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodChannel1"));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("������� ��������� �������� ������������ �������� 0",
                        "PollingPeriodChannel1");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ ������ �������������� ������ 2: 4-20 ��
        /// </summary>
        /// <remarks>
        /// Holding register 0x0007
        /// 1 bit == 10 ���.
        /// 10���.�7���. (0�0001-0xFFFF hex)
        /// 0xFFFF � ���������� �����
        /// </remarks>
        private UInt32 _PollingPeriodChannel2 = 655350; // ����� �� ��������� ��������
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        ///  ������ ������ �������������� ������ 2: 4-20 ��
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������")]
        [Description("������ ������ ������ 2 �� 10 ��� �� 7 ���.")]
        [DisplayName("������ ������ ������ 2, ���")]
        [DefaultValue(typeof(UInt32), "655350")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        [Editor(typeof(Channel4_20TypeEditor), 
            typeof(UITypeEditor))]
        [TypeConverter(typeof(NGK.MeasuringDeviceTech.Classes.MeasuringDevice.Converters.TypeConverterChannel4_20))]
        public UInt32 PollingPeriodChannel2
        {
            get { return _PollingPeriodChannel2; }
            set
            {
                if (value != 0)
                {
                    if (value == 0xFFFF * 10)
                    {
                        // ����� ��������
                        _PollingPeriodChannel2 = value;
                        // ���������� �������
                        OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodChannel2"));

                    }
                    else
                    {
                        // �������� ������ ���� ������ 10
                        uint x = value % 10;
                        if (x != 0)
                        {
                            throw new ArgumentException(
                                "�������� �������� �� ������ 10",
                                "PollingPeriodChannel2");
                        }
                        else
                        {
                            // ����� �������
                            _PollingPeriodChannel2 = value;
                            // ���������� �������
                            OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodChannel2"));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("������� ��������� �������� ������������ �������� 0",
                        "PollingPeriodChannel2");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ������ � ���� CAN ��/���
        /// </summary>
        /// <remarks>
        /// Holding register 0x0008
        /// 1 bit == 1 ����/�
        /// 100 ����/�; 50 ����/�; 20 ����/�; 10 ����/� (0�000A-0x03E8 hex)
        /// �������� �� �� ������ � �������� ���������� 0�03
        /// </remarks>
        private CANBaudRate _BaudRateCAN = CANBaudRate.BR20K;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ������ � ���� CAN ��/��� 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ����")]
        [Description("�������� ������ ��/��� � ���� CAN")]
        [DisplayName(@"�������� ������ �AN, ��/���")]
        [DefaultValue(typeof(CANBaudRate), "BR20K")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public CANBaudRate BaudRateCAN
        {
            get { return _BaudRateCAN; }
            set
            {
                if (Enum.IsDefined(typeof(CANBaudRate), value))
                {
                    _BaudRateCAN = value;
                    // ���������� �������
                    OnPropertyChanged(new PropertyChangedEventArgs("BaudRateCAN"));
                }
                else
                {
                    throw new ArgumentException("������� ��������� ������������ �������� ��������", 
                        "BaudRateCAN");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ������������ ���� �������� ����� (0...100�)
        /// </summary>
        /// <remarks>
        /// Holding register 0x0009
        /// 1 bit == 1 �
        /// 10, 20, 30, 50, 75, 100, 150 (0�000A-0x0096 hex)
        /// �� ��������� 50�
        /// </remarks>
        private CurrentShuntValues _CurrentShuntValue = CurrentShuntValues.A50;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������� ������������ ���� �������� ����� (0...100�)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������")]
        [Description("�������� ������������ ���� �������� ����� (0...150�)")]
        [DisplayName("������� ����, �")]
        [DefaultValue(typeof(CurrentShuntValues), "A50")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public CurrentShuntValues CurrentShuntValue
        {
            get { return _CurrentShuntValue; }
            set
            {
                if (Enum.IsDefined(typeof(CurrentShuntValues), value))
                {
                    _CurrentShuntValue = value;
                    // ���������� �������
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentShuntValue"));
                }
                else
                {
                    throw new ArgumentException(
                        "������� ��������� �������� ������������ ��������", 
                        "CurrentShuntValue");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������� �����
        /// </summary>
        /// <remarks>
        /// Holding register 0x000A � 0x000B
        /// </remarks>
        private DateTime _DateTime;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������� �����
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("��������� ������")]
        [Description("���������� ��������� ����� � ���� � ���������� ���-��")]
        [DisplayName("���� � �����")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public DateTime DateTime
        {
            get { return _DateTime; }
            set
            {
                _DateTime = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("DateTime"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Coils
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ���������������� 
        /// ���������� ���������� ������������ �� ������ ����������������
        /// </summary>
        /// <remarks>
        /// Coil 0x0000
        /// </remarks>
        private Boolean _PolarizationPotentialEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������ ������ ��������� ���������������� 
        /// ���������� ���������� ������������ �� ������ ����������������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� ������ ������ ��������� ���������������� ���������� ���������� ������������ �� ������ ����������������")]
        [DisplayName("��������� ���������������� ����������")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean PolarizationPotentialEn
        {
            get { return _PolarizationPotentialEn; }
            set
            {
                _PolarizationPotentialEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("PolarizationPotentialEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ��������� ����������
        /// </summary>
        /// <remarks>
        /// Coil 0x0001
        /// </remarks>
        private Boolean _ProtectivePotentialEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������ ������ ��������� ��������� ����������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� ������ ������ ��������� ��������� ����������")]
        [DisplayName("��������� ��������� ����������")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean ProtectivePotentialEn
        {
            get { return _ProtectivePotentialEn; }
            set
            {
                _ProtectivePotentialEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("ProtectivePotentialEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ���� �������� ������ 
        /// � ����� ������� ������� ��������� ���������� �� ������� �����
        /// </summary>
        /// <remarks>
        ///  Coil 0x0002
        ///  </remarks>
        private Boolean _Protective�urrentEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������ ������ ��������� ���� �������� ������ 
        /// � ����� ������� ������� ��������� ���������� �� ������� �����
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� ������ ������ ��������� ���� �������� ������ � ����� ������� ������� ��������� ���������� �� ������� �����")]
        [DisplayName("��������� ��������� ����")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean Protective�urrentEn
        {
            get { return _Protective�urrentEn; }
            set
            {
                _Protective�urrentEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("Protective�urrentEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ���� ����������� ���������������� 
        /// ���������
        /// </summary>
        /// <remarks>
        ///  Coil 0x0003
        ///  </remarks>
        private Boolean _Polarization�urrentEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������ ������ ���� ����������� ���������������� 
        /// ���������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� ������ ������ ���� ����������� ���������������� ���������")]
        [DisplayName("��������� ���� �����������")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean Polarization�urrentEn
        {
            get { return _Polarization�urrentEn; }
            set
            {
                _Polarization�urrentEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("Polarization�urrentEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ���������� ����������� ���������� �� �����������
        /// </summary>
        /// <remarks>
        ///  Coil 0x0004
        /// </remarks>
        private Boolean _InducedVoltageEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������ ������ ��������� ���������� ����������� ���������� �� �����������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� ������ ������ ��������� ���������� ����������� ���������� �� �����������")]
        [DisplayName("��������� ���������� ����������")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean InducedVoltageEn
        {
            get { return _InducedVoltageEn; }
            set
            {
                _InducedVoltageEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("InducedVoltageEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ��������� ������������ ��������� �10 ��� ������ ��������� ���������� ����������
        /// </summary>
        /// <remarks>
        /// Coil 0x0005
        /// �� ��������. ��� ��(�) ���������� U���=�5�
        /// </remarks>
        private Boolean _ExtendedSumPotentialEn = false;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ��������� ������������ ��������� �10 ��� ������ ��������� ���������� ����������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� ������ ������������ ��������� X10 ��� ������ ��������� ���������� ����������")]
        [DisplayName("���������� ������������ ��������� �10")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean ExtendedSumPotentialEn
        {
            get { return _ExtendedSumPotentialEn; }
            set
            {
                _ExtendedSumPotentialEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("ExtendedSumPotentialEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� �������� ����� ���������
        /// </summary>
        /// <remarks>
        /// Coil 0x0006
        /// �� ��������.
        /// </remarks>
        private Boolean _SendStatusWordEn = false;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� �������� ����� ���������
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� �������� ����� ���������")]
        [DisplayName("���������� �������� ����� ���������")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean SendStatusWordEn
        {
            get { return _SendStatusWordEn; }
            set
            {
                _SendStatusWordEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("SendStatusWordEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ����������� ���� ���������
        /// (��� ����� ������ � ���������� ���������)
        /// </summary>
        /// <remarks>
        /// Coil 0x0007
        /// �� ��������.
        /// </remarks>
        private Boolean _DcCurrentRefereceElectrodeEn = false;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ����������� ���� ���������
        /// (��� ����� ������ � ���������� ���������)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� ������ ������ ��������� ����������� ���� ��������� (��� ����� ������ � ���������� ���������)")]
        [DisplayName("���������� ��������� DC ���� ���������")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean DcCurrentRefereceElectrodeEn
        {
            get { return _DcCurrentRefereceElectrodeEn; }
            set
            {
                _DcCurrentRefereceElectrodeEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("DcCurrentRefereceElectrodeEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ����������� ���� ���������
        /// (��� ����� ������ � ���������� ���������)
        /// </summary>
        /// <remarks>
        /// Coil 0x0008
        /// �� ��������.
        /// </remarks>
        private Boolean _AcCurrentRefereceElectrodeEn = false;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ����������� ���� ���������
        /// (��� ����� ������ � ���������� ���������)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("��������� ���������� ���-���")]
        [Description("���������� ������ ������ ��������� ����������� ���� ��������� (��� ����� ������ � ���������� ���������)")]
        [DisplayName("���������� ��������� AC ���� ���������")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean AcCurrentRefereceElectrodeEn
        {
            get { return _AcCurrentRefereceElectrodeEn; }
            set
            {
                _AcCurrentRefereceElectrodeEn = value;
                // ���������� �������
                OnPropertyChanged(new PropertyChangedEventArgs("AcCurrentRefereceElectrodeEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------     
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Constructors
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����������� �� ���������
        /// </summary>
        public MeasuringDeviceMainPower()
        {}
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Methods
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������ � ��������� ������� ���������� ����������
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return base.ToString();

            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("����: {0} �����: {1}",
                DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("��������� ������:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("��� ���-�� � ������� ���: ���-��(�)-00");
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("�������� ����� ���-�� � ������� ���: {0}", this.SerialNumber.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��� �������������: {0}", this.CodeManufacturer.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ��: {0}", this.SofwareVersion.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ����������: {0}", this.HardwareVersion.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� �����: {0} ���� {1}",
                this.DateTime.ToLongTimeString(), this.DateTime.ToShortDateString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("��������� ���������� ���-���:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� ��������� ����������: {0}", this.ProtectivePotentialEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� ��������� ����: {0}", this.Protective�urrentEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� ���������� ����������: {0}", this.InducedVoltageEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� ���������������� ����������: {0}", this.PolarizationPotentialEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� ���� �����������: {0}", this.Polarization�urrentEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("���������� ������������ ��������� �10: {0}", this.ExtendedSumPotentialEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("��������� ����:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("����� ���������� ���-�� ������: {0}", this.AddressSlave.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("�����/����� ����������: {0}", this.NetAddress.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("�������� ������ CAN, ��/���: {0}", this.BaudRateCAN.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("��������� ���������:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ���������, ���: {0}", this.MeasuringPeriod.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ������ ���, ���: {0}", this.PollingPeriodBPI.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ��������� ��������� ����������, ���: {0}", 
                this.MeasuringVoltagePeriod.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ������ ������ 1, ���: {0}", this.PollingPeriodChannel1.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ������ ������ 2, ���: {0}", this.PollingPeriodChannel2.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ������ �� ��� ��, ���: {0}", this.PollingPeriodUSIKPST.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������� ����, �: {0}", this.CurrentShuntValue.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("��������� ���-�� � ������� ���-��� ��(�):");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ ��������: {0}", this.CaseOpen.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� ���������� �������: {0}", this.SupplyVoltageStatus.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� �������� �������: {0}", this.BattaryStatus.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("���������� ���������:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������� �������� �� ��� ��, ���: {0}", this.DepthOfCorrosion.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("�������� �������� �� ��� ��, ���: {0}", this.SpeedOfCorrosion.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��� ��������� �� ��� ��: {0}", this.StatusUSIKPST.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ �������� �1: {0}", this.CorrosionSensor1.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ �������� �2: {0}", this.CorrosionSensor2.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("������ �������� �3: {0}", this.CorrosionSensor3.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("�������� ���������, �: {0}", this.ProtectivePotential.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��� �������� ������, �: {0}", this.Protective�urrent.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������� ����������, �: {0}", this.InducedVoltage.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("���������� �������, �: {0}", this.BattaryVoltage.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("���������� ������� ���-��, �: {0}", this.SupplyVoltage.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��������������� ���������, �: {0}", this.PolarizationPotential.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��� �����������, ��: {0}", this.Polarization�urrent.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��� �������������� ������ �1 (4-20), ��: {0}", this.CurrentChannel1.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("��� �������������� ������ �2 (4-20), ��: {0}", this.CurrentChannel2.ToString()));
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ����� ��������� �������� 
        /// </summary>
        /// <param name="args"></param>
        private void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
            return;
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ����� ���������� �������� 
        /// </summary>
        /// <param name="args"></param>
        //private void OnPropertyChanging(PropertyChangingEventArgs args)
        //{
        //    //if (this.PropertyChanging != null)
        //    //{
        //    //    this.PropertyChanging(this, args);
        //    //}
        //    return;
        //}
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����������� ������ �� � ����
        /// </summary>
        /// <param name="path">���� + ��� �����, 
        /// � ������� ���������� ��������� ������ ��</param>
        /// <param name="device">������ ��</param>
        /// <returns>��������� ���������� ��������, 
        /// ���� false - �� �������� �������������</returns>
        public static Boolean Serialize(String path, MeasuringDeviceMainPower device)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryFormatter bf = new BinaryFormatter();
            Boolean result;
            try
            {
                bf.Serialize(fs, device);
                result = true;
            }
            catch
            {
                // �� ������� ������������� ������
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return result;
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ����������� ������ �� � ����
        /// </summary>
        /// <param name="stream">����� � ������� ������ ���� ��� ������������, ������ ���� ������ ��� ������</param>
        /// <param name="device">������ ��</param>
        /// <returns>��������� ���������� ��������, 
        /// ���� false - �� �������� �������������</returns>
        public static Boolean Serialize(FileStream stream, MeasuringDeviceMainPower device)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Boolean result;
            if (stream != null)
            {
                if (stream.CanWrite)
                {            
                    try
                    {
                        bf.Serialize(stream, device);
                        result = true;
                    }
                    catch
                    {
                        // �� ������� ������������� ������
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������������� ���� � ������ ���������� ��
        /// </summary>
        /// <param name="path">���� ���������� ��� ����� ��� ���������������</param>
        /// <returns>������������������ ������, ��� null - ���� ����� ����� ������</returns>
        public static MeasuringDeviceMainPower Deserialize(String path)
        {
            MeasuringDeviceMainPower device;
            FileStream fs;
            fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                device = (MeasuringDeviceMainPower)bf.Deserialize(fs);
            }
            catch
            {
                device = null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            
            return device;
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// �������������� ���� � ������ ���������� ��
        /// </summary>
        /// <param name="stream">����� � ������� ������ 
        /// ���� �� �������� ����� ���������������</param>
        /// <returns>������������������ ������, 
        /// ��� null - ���� ����� ����� ������</returns>
        public static MeasuringDeviceMainPower Deserialize(FileStream stream)
        {
            MeasuringDeviceMainPower device;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                device = (MeasuringDeviceMainPower)bf.Deserialize(stream);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                device = null;
            }
            finally
            {
                // ���������� � �������, ���������� ��� ���������� ��������� �� ������
                // �����, ����� ����� �����.
                stream.Seek(0, SeekOrigin.Begin);
            }
            return device;
        }
        //------------------------------------------------------------------------------------------------------
        #region NetworksCommands
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� ���������������� ���������� ��� ���. 
        /// </summary>
        /// <param name="host">Master-����������</param>
        /// <param name="init">TRUE-���������� ���������������</param>
        /// <param name="error">��������� ���������� ��������</param>
        public void Read_HR_VerifyInitDevice(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out Boolean init,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            UInt16[] registers;

            result = host.ReadHoldingRegisters(
                _AddressSlave, BI_ADDRESSES_OF_HOLDINGREGISTERS.SerialNumber, 
                3, out registers);

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        // �������� �������� � ������ ���������� �� ����������������
                        init = false;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // �������� �� �������� ������ ���������� ����������������
                        init = true;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        init = false;
                        String msg = String.Format(
                            "������ ���������� ������ ����������. ���������� ������ ������: {0}", 
                            result.Description);
                        error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                        break;
                    }
            }

        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������ �������� ����� ����������. ���� ������ ������� �� ��������, 
        /// �� ���������� ����������������.
        /// � ��������� ������, ���������� �������� �������� �����.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="serialNumber"></param>
        /// <param name="error"></param>
        public void Read_HR_SerialNumber(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out UInt64 serialNumber,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;


            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.SerialNumber,
                3, out registers);

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        if (registers.Length == 3)
                        {
                            serialNumber = 0;
                            serialNumber = registers[0];
                            serialNumber = (serialNumber << 16);
                            serialNumber |= registers[1];
                            serialNumber = (serialNumber << 16);
                            serialNumber |= registers[2];
                            error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        }
                        else
                        {
                            serialNumber = 0;
                            error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, 
                                String.Format(
                                "���������� ������� �������� ������. ���������� ��������� {0}, � ������ ���� 3", 
                                registers.Length));
                        }
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:  
                    {
                        serialNumber = 0;
                        error = new OperationResult(OPERATION_RESULT.IllegalDataAddress, result.Description);
                        break;
                    }
                default:
                    {
                        serialNumber = 0;
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //------------------------------------------------------------------------------------------------------
        public void Write_HR_SerialNumber(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            UInt64 serialNumber,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt64 sn;
            UInt16[] value;
            //String msg;

            value = new ushort[3];
            value[2] = (UInt16)serialNumber; // Low
            value[1] = (UInt16)(serialNumber >> 16); // High
            value[0] = (UInt16)(serialNumber >> 32); // Upper

            result = host.WriteMultipleRegisters(
                _AddressSlave, BI_ADDRESSES_OF_HOLDINGREGISTERS.SerialNumber,
                value);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��� �������� ������ ��������� ������
                error = new OperationResult(OPERATION_RESULT.FAILURE, 
                    result.Description);
            }
            else
            {
                // ����� ������ ������ ������� ����������� �� ��������
                // ��������� ���������� ������
                this.Read_HR_SerialNumber(ref host,
                    out sn, out error);
                
                switch (error.Result)
                {
                    case OPERATION_RESULT.IllegalDataAddress:
                        {
                        //    // ������ �������� �������, ��������� ������
                        //    if (sn == serialNumber)
                        //    {
                        //        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        //    }
                        //    else
                        //    {
                        //        msg = String.Format(
                        //            "������ ������ ��������� ������ ����������. �� ������ ���������� {0} � ����������� �������� {1}",
                        //            serialNumber, sn);
                        //        error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                        //    }

                            error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                            break;
                        }
                    case OPERATION_RESULT.OK:
                        {
                            // ��� ���������� �������� ������ ��������� ������
                            error = new OperationResult(OPERATION_RESULT.FAILURE, 
                                "������: ����� ������������� ���������� ������� �������� ������� ��������");
                            break;
                        }
                    default:
                        {
                            error = new OperationResult(OPERATION_RESULT.FAILURE, error.Message);
                            break;
                        }
                }                
            }
            return;
        }
        //------------------------------------------------------------------------------------------------------
        public void Read_HR_NetAddress(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.NetAddress,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    this.NetAddress = Convert.ToByte(registers[0]);
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch (Exception ex)
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE, ex.Message);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }

            return;
        }
        //------------------------------------------------------------------------------------------------------
        public void Write_HR_NetAddress(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt16 value = Convert.ToUInt16(this.NetAddress);
            String msg;

            result = host.WriteSingleRegister(
                _AddressSlave, BI_ADDRESSES_OF_HOLDINGREGISTERS.NetAddress,
                ref value);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                if (Convert.ToByte(value) != this.NetAddress)
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.NetAddress, value);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_MeasuringPeriod(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;
            UInt32 period = 0;
            String msg;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.MeasuringPeriod,
                2, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                if (registers.Length != 2)
                {
                    msg = String.Format(
                        "����� ���-�� �������� ��������� ����������� ��������� {0}, ������ ���� 2",
                        registers.Length);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    period |= registers[0]; // high
                    period = period << 16;
                    period |= registers[1]; // low
                    try
                    {
                        this.MeasuringPeriod = period;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                    }
                    catch (Exception ex)
                    {
                        error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, ex.Message);
                    }
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_MeasuringPeriod(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;
            String msg;
            UInt32 period = this._MeasuringPeriod;

            registers = new ushort[2];
            registers[1] |= (UInt16)period; // low
            registers[0] |= (UInt16)(period >> 16); //high

            result = host.WriteMultipleRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.MeasuringPeriod,
                registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                if (registers.Length != 2)
                {
                    msg = String.Format(
                        "����� ���-�� �������� ��������� ����������� ��������� {0}, ������ ���� 2",
                        registers.Length);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    // ��������� ���������� � �����������
                    result = host.ReadHoldingRegisters(_AddressSlave,
                        BI_ADDRESSES_OF_HOLDINGREGISTERS.MeasuringPeriod,
                        2, out registers);

                    if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
                    {
                        if (registers.Length != 2)
                        {
                            msg = String.Format(
                                "����� ���-�� �������� ��������� ����������� ��������� {0}, ������ ���� 2",
                                registers.Length);
                            error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                        }
                        else
                        {
                            period = 0;
                            period |= registers[0]; //high 
                            period = period << 16;
                            period |= registers[1]; // low

                            if (this.MeasuringPeriod != period)
                            {
                                msg = String.Format(
                                    "�������� ����������� ���������� {0} �� ��������� � ����������� {1}",
                                    this.MeasuringPeriod, period);
                                error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                            }
                            else
                            {
                                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                            }
                        }
                    }
                    else
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                    }
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_MeasuringVoltagePeriod(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.MeasuringSupplyVoltagePeriod,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    this.MeasuringVoltagePeriod = registers[0];
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch (Exception ex)
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE,
                        ex.Message);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_MeasuringVoltagePeriod(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;
            String msg;
            UInt16 period = this._MeasuringVoltagePeriod;

            registers = new ushort[] { period };

            result = host.WriteMultipleRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.MeasuringSupplyVoltagePeriod,
                registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                if (registers.Length != 1)
                {
                    msg = String.Format(
                        "����� ���-�� �������� ��������� ����������� ��������� {0}, ������ ���� 1",
                        registers.Length);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    // ��������� ���������� � �����������
                    result = host.ReadHoldingRegisters(_AddressSlave,
                        BI_ADDRESSES_OF_HOLDINGREGISTERS.MeasuringSupplyVoltagePeriod,
                        1, out registers);

                    if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
                    {
                        if (registers.Length != 1)
                        {
                            msg = String.Format(
                                "����� ���-�� �������� ��������� ����������� ��������� {0}, ������ ���� 1",
                                registers.Length);
                            error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                        }
                        else
                        {
                            period = registers[0];

                            if (this.MeasuringVoltagePeriod != period)
                            {
                                msg = String.Format(
                                    "�������� ����������� ���������� {0} �� ��������� � ����������� {1}",
                                    this.MeasuringPeriod, period);
                                error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                            }
                            else
                            {
                                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                            }
                        }
                    }
                    else
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                    }
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_PollingPeriodUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt16 value = (UInt16)(this.PollingPeriodUSIKPST / 10);
            String msg;

            result = host.WriteSingleRegister(
                _AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.PollingPeriodUSIKPST,
                ref value);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                if ((value * 10) != this.PollingPeriodUSIKPST)
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.PollingPeriodUSIKPST / 10, value);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_PollingPeriodUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.PollingPeriodUSIKPST,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    this.PollingPeriodUSIKPST = (UInt32)(registers[0] * 10);
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch (Exception ex)
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE, ex.Message);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_PollingPeriodBPI(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt16 value = (UInt16)(this.PollingPeriodBPI / 10);
            String msg;

            result = host.WriteSingleRegister(
                _AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.PollingPeriodBPI,
                ref value);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                if ((value * 10) != this.PollingPeriodBPI)
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.PollingPeriodBPI / 10, value);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_PollingPeriodBPI(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.PollingPeriodBPI,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    this.PollingPeriodBPI = (UInt32)(registers[0] * 10);
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch (Exception e)
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE, e.Message);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_PollingPeriodChannel1(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt16 value = (UInt16)(this.PollingPeriodChannel1 / 10);
            String msg;

            result = host.WriteSingleRegister(
                _AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.PollingPeriodChannel1_4_20,
                ref value);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                if ((value * 10) != this.PollingPeriodChannel1)
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.PollingPeriodChannel1, value);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_PollingPeriodChannel1(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.PollingPeriodChannel1_4_20,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    this.PollingPeriodChannel1 = (UInt32)(registers[0] * 10);
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch (Exception ex)
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE, ex.Message);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_PollingPeriodChannel2(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt16 value = (UInt16)(this.PollingPeriodChannel2 / 10);
            String msg;

            result = host.WriteSingleRegister(
                _AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.PollingPeriodChannel2_4_20,
                ref value);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                if ((value * 10) != this.PollingPeriodChannel2)
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.PollingPeriodChannel2, value);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_PollingPeriodChannel2(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.PollingPeriodChannel2_4_20,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    this.PollingPeriodChannel2 = (UInt32)(registers[0] * 10);
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch (Exception ex)
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE, ex.Message);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_BaudRateCAN(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt16 value = (UInt16)this.BaudRateCAN;
            String msg;

            result = host.WriteSingleRegister(
                _AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.BaudRateCAN,
                ref value);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                // ������, �� ��� �������� � �������
                if ((UInt16)this.BaudRateCAN != value)
                {
                    msg = String.Format(
                        "�������� ����������� ���������� {0} �� ��������� � ����������� {1}",
                        this.BaudRateCAN, value);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_BaudRateCAN(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.BaudRateCAN,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    this.BaudRateCAN =
                        (CANBaudRate)Enum.ToObject(typeof(CANBaudRate), registers[0]);
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch (Exception ex)
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE, ex.Message);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_CurrentShuntValue(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt16 value = (UInt16)this.CurrentShuntValue;
            String msg;

            result = host.WriteSingleRegister(
                _AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.CurrentShuntValue,
                ref value);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                // ������ ��������� � ������� ����������
                if ((UInt16)this.CurrentShuntValue != value)
                {
                    msg = String.Format(
                        "�������� ����������� ���������� {0} �� ��������� � ����������� {1}",
                        this.CurrentShuntValue, value);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_CurrentShuntValue(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;
            CurrentShuntValues shunt;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.CurrentShuntValue,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    shunt =
                        (CurrentShuntValues)Enum.ToObject(typeof(CurrentShuntValues), registers[0]);
                    this.CurrentShuntValue = shunt;
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch (Exception ex)
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE, ex.Message);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_HR_DateTime(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            // ���������� ����� �������� � ����������
            Modbus.OSIModel.Message.Result result;
            UInt16[] registers = new ushort[2] { 0, 0 };
            DateTime unixStartTime;
            DateTime dt = DateTime.Now;
            UInt32 totalSeconds;
            //String msg;
            TimeSpan ts;

            //unixStartTime = DateTime.Parse("01/01/1970 00:00:00",
            //    new System.Globalization.CultureInfo("ru-Ru", false));

            unixStartTime = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            ts = (DateTime.Now.ToUniversalTime()).Subtract(unixStartTime);

            totalSeconds = Convert.ToUInt32(ts.TotalSeconds);

            unchecked
            {
                registers[0] = (UInt16)(totalSeconds >> 16); // Hi_byte
                registers[1] = (UInt16)(totalSeconds); // Lo byte
            }

            result = host.WriteMultipleRegisters(
                _AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.DateTime,
                registers);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            // �������� ���������� ����������� ����������, �� ������� ���� �����!!!
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_HR_DateTime(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, out DateTime dataTime)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;
            DateTime unixStartTime;
            UInt32 totalSeconds;
            String msg;

            dataTime = DateTime.Now;

            result = host.ReadHoldingRegisters(_AddressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.DateTime,
                2, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                if (registers.Length != 2)
                {
                    msg = String.Format(
                        "����� ���-�� �������� ��������� ����������� ��������� {0}, ������ ���� 2",
                        registers.Length);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    // ������ ����� ���������� �� �������� ��������. ����� �������
                    // �������� ����� ������ ������������ � ���������� ��

                    //unixStartTime = DateTime.Parse("01/01/1970 00:00:00",
                    //    new System.Globalization.CultureInfo("ru-Ru", false));
                    unixStartTime = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                    totalSeconds = 0;
                    totalSeconds = registers[0]; // Hi_byte
                    totalSeconds = (totalSeconds << 16);
                    totalSeconds |= registers[1]; // Lo_byte
                    dataTime = unixStartTime.AddSeconds(totalSeconds);
                    this._DateTime = dataTime;
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_PolarizationPotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.PolarizationPotentialEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.PolarizationPotentialEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_CL_PolarizationPotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.PolarizationPotentialEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.PolarizationPotentialEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.PolarizationPotentialEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.PolarizationPotentialEn, coil);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_ProtectivePotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.ProtectivePotentialEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.ProtectivePotentialEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_CL_ProtectivePotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.ProtectivePotentialEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.ProtectivePotentialEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.ProtectivePotentialEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.ProtectivePotentialEn, coil);
                    error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_Protective�urrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.Protective�urrentEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.Protective�urrentEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_CL_Protective�urrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.Protective�urrentEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.Protective�urrentEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.Protective�urrentEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.Protective�urrentEn, coil);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_Polarization�urrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.Polarization�urrentEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.Polarization�urrentEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_CL_Polarization�urrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.Polarization�urrentEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.Polarization�urrentEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.Polarization�urrentEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.Polarization�urrentEn, coil);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_InducedVoltageEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.InducedVoltageEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.InducedVoltageEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_CL_InducedVoltageEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.InducedVoltageEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.InducedVoltageEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.InducedVoltageEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.InducedVoltageEn, coil);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_ExtendedModeX10SumPotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.ExtendedModeX10SumPotentialEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.ExtendedSumPotentialEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_CL_ExtendedModeX10SumPotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.ExtendedSumPotentialEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.ExtendedModeX10SumPotentialEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.ExtendedSumPotentialEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.ExtendedSumPotentialEn, coil);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_SendStatusWordEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.SendStatusWordEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.SendStatusWordEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_CL_SendStatusWordEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.SendStatusWordEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.SendStatusWordEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.SendStatusWordEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.SendStatusWordEn, coil);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_DcCurrentRefereceElectrodeEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.DcCurrentRefereceElectrodeEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.DcCurrentRefereceElectrodeEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Write_CL_DcCurrentRefereceElectrodeEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.DcCurrentRefereceElectrodeEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.DcCurrentRefereceElectrodeEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.DcCurrentRefereceElectrodeEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.DcCurrentRefereceElectrodeEn, coil);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_CL_AcCurrentRefereceElectrodeEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.AcCurrentRefereceElectrodeEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.AcCurrentRefereceElectrodeEn =
                    Modbus.Convert.ToBoolean(coils[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
        }
        //----------------------------------------------------------------------------
        public void Write_CL_AcCurrentRefereceElectrodeEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.AcCurrentRefereceElectrodeEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.AcCurrentRefereceElectrodeEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // ��������� ���������� ������
                if (this.AcCurrentRefereceElectrodeEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "�������� ������� �� �����: ������ ���� {0}, ���������� ������� {1}",
                        this.AcCurrentRefereceElectrodeEn, coil);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_DI_CaseOpen(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] discreteInputs;

            result = host.ReadDiscreteInputs(_AddressSlave,
                BI_ADDRESSES_OF_DISCRETESINPUTS.CaseOpen, 1, out discreteInputs);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.CaseOpen =
                    Modbus.Convert.ToBoolean(discreteInputs[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_DI_SupplyVoltageStatus(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] discreteInputs;

            result = host.ReadDiscreteInputs(_AddressSlave,
                BI_ADDRESSES_OF_DISCRETESINPUTS.SupplyVoltageStatus,
                1, out discreteInputs);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.SupplyVoltageStatus =
                    Modbus.Convert.ToBoolean(discreteInputs[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_DI_BattaryVoltageStatus(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] discreteInputs;

            result = host.ReadDiscreteInputs(_AddressSlave,
                BI_ADDRESSES_OF_DISCRETESINPUTS.BattaryVoltageStatus,
                1, out discreteInputs);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.BattaryStatus =
                    Modbus.Convert.ToBoolean(discreteInputs[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_DI_CorrosionSensor1(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] discreteInputs;

            result = host.ReadDiscreteInputs(_AddressSlave,
                BI_ADDRESSES_OF_DISCRETESINPUTS.CorrosionSensor1,
                1, out discreteInputs);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.CorrosionSensor1 =
                    Modbus.Convert.ToBoolean(discreteInputs[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_DI_CorrosionSensor2(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] discreteInputs;

            result = host.ReadDiscreteInputs(_AddressSlave,
                BI_ADDRESSES_OF_DISCRETESINPUTS.CorrosionSensor2,
                1, out discreteInputs);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.CorrosionSensor2 =
                    Modbus.Convert.ToBoolean(discreteInputs[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_DI_CorrosionSensor3(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] discreteInputs;

            result = host.ReadDiscreteInputs(_AddressSlave,
                BI_ADDRESSES_OF_DISCRETESINPUTS.CorrosionSensor3,
                1, out discreteInputs);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.CorrosionSensor3 =
                    Modbus.Convert.ToBoolean(discreteInputs[0]);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_TypeOfDevice(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, out TYPE_NGK_DEVICE typeOfDevice)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.TypeOfDevice,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                try
                {
                    typeOfDevice =
                        (TYPE_NGK_DEVICE)Enum.Parse(typeof(TYPE_NGK_DEVICE),
                        registers[0].ToString());
                    
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
                catch
                {
                    typeOfDevice = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                    error = new OperationResult(OPERATION_RESULT.UNKNOWN_DEVICE, String.Format(
                        "���������� ������� ��� ������������ ���� ����������: {0}", registers[0].ToString()));
                }
            }
            else
            {
                typeOfDevice = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_SoftWareVersion(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.VersionSoftware,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.SofwareVersion = ((float)registers[0]) / 100;
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_HardWareVersion(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.VersionHardware,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.HardwareVersion = ((float)registers[0]) / 100;
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_SerialNumber(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.SerialNumber,
                3, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                UInt64 serialNumber;
                serialNumber = 0;
                serialNumber = registers[0]; // Upper
                serialNumber |= (serialNumber << 16);
                serialNumber |= registers[1]; // High
                serialNumber |= (serialNumber << 16);
                serialNumber |= registers[2]; // Low

                this.SerialNumber = serialNumber;

                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_CRC16(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.CRC16,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.CRC16 = registers[0];
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_ManufacturerCode(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.ManufacturerCode,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.CodeManufacturer = registers[0];
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_Polarization_potential(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.Polarization_potential,
                1, out registers);

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.PolarizationPotential = (float)(ToValue(registers[0]) * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;}
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.PolarizationPotential = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_Protective_potential(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.Protective_potential,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.ProtectivePotential =
            //        (float)(ToValue(registers[0]) * 0.01);
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.ProtectivePotential = (float)(ToValue(registers[0]) * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.ProtectivePotential = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_InducedVoltage(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.InducedVoltage,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.InducedVoltage =
            //        (float)(registers[0]);
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.InducedVoltage = (float)(registers[0]);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.InducedVoltage = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_Protective_current(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.Protective_current,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.Protective�urrent =
            //        (float)(registers[0] * 0.05);
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.Protective�urrent = (float)(registers[0] * 0.05);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.Protective�urrent = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_Polarization_current(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.Polarization_current,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.Polarization�urrent =
            //        (float)(ToValue(registers[0]) * 0.01);
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.Polarization�urrent = (float)(ToValue(registers[0]) * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.Polarization�urrent = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_Current_Channel_1(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.Current_Channel_1,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.CurrentChannel1 =
            //        (float)(registers[0] * 0.01);
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.CurrentChannel1 = (float)(registers[0] * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.CurrentChannel1 = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_Current_Channel_2(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.Current_Channel_2,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.CurrentChannel2 =
            //        (float)(registers[0] * 0.01);
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.CurrentChannel2 = (float)(registers[0] * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.CurrentChannel2 = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_DepthOfCorrosionUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.DepthOfCorrosionUSIKPST,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    try
            //    {
            //        this.DepthOfCorrosion =
            //            registers[0];
            //        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //    }
            //    catch (Exception ex)
            //    {
            //        error = new OperationResult(OPERATION_RESULT.FAILURE, ex.Message);
            //    }
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        try
                        {
                            this.DepthOfCorrosion =
                                registers[0];
                            error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        }
                        catch (Exception ex)
                        {
                            error = new OperationResult(OPERATION_RESULT.FAILURE, ex.Message);
                        }
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.DepthOfCorrosion = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_SpeedOfCorrosionUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.SpeedOfCorrosionUSIKPST,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.SpeedOfCorrosion =
            //        registers[0];
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.SpeedOfCorrosion = registers[0];
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.DepthOfCorrosion = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_StatusUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.StatusUSIKPST,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.StatusUSIKPST =
            //        registers[0];
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this.StatusUSIKPST = registers[0];
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this.StatusUSIKPST = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_SupplyVoltage(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.SupplyVoltage,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.SupplyVoltage =
                    (float)(registers[0] * 0.05);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_BattaryVoltage(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.BattaryVoltage,
                1, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.BattaryVoltage =
                    (float)(ToValue(registers[0]) * 0.01);
                error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_InternalTemperatureSensor(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            //throw new InvalidOperationException("�������� ���������������� � ��(�)-00");
            error = new OperationResult(OPERATION_RESULT.INVALID_OPERATION,
                "�������� ���������������� � ���-��(�)-00");

            //Modbus.OSIModel.Message.Result result;
            ////String msg;
            //UInt16[] registers;

            //result = host.ReadInputRegisters(_AddressSlave,
            //    BI_ADDRESSES_OF_INPUTREGISTERS.InternalTemperatureSensor,
            //    1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this.InternalTemperatureSensor =
            //        (Int16)(ToValue(registers[0]));
            //    error = new OperationResult(true, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(false, result.Description);
            //}
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_ReferenceElectrodeDCCurrent(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.ReferenceElectrodeDcCurrent,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this._ReferenceElectrodeDc�urrent =
            //        (float)(ToValue(registers[0]) * 0.01);
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}

            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this._ReferenceElectrodeDc�urrent =
                            (float)(ToValue(registers[0]) * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this._ReferenceElectrodeDc�urrent = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_ReferenceElectrodeACCurrent(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            result = host.ReadInputRegisters(_AddressSlave,
                BI_ADDRESSES_OF_INPUTREGISTERS.ReferenceElectrodeAcCurrent,
                1, out registers);

            //if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            //{
            //    this._ReferenceElectrodeAc�urrent =
            //        (float)((registers[0]) * 0.01);
            //    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
            //}
            //else
            //{
            //    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            //}
            switch (result.Error)
            {
                case Modbus.OSIModel.ApplicationLayer.Error.NoError:
                    {
                        this._ReferenceElectrodeAc�urrent = (float)((registers[0]) * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // ���� ������������ ���������� 0x2 (������� �� �������� ��� ������)
                        // ��� �������� ���������� ���������.
                        // ��. ��������: ��� ������ Input Register, ������������ � Coil ��� 
                        // ���������� ����� ��������� ���������� ���������� 0�02.
                        this._ReferenceElectrodeAc�urrent = 0;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        public TYPE_NGK_DEVICE GetDeviceType()
        {
            return this.TypeOfDevice;
        }
        //----------------------------------------------------------------------------
        public Object GetDevice()
        {
            return this;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// ���������������� ����� �������������� � �������������� ����
        /// � �������� ����� �����.
        /// </summary>
        /// <param name="valueTwosComplement">
        /// ����� �������������� � �������������� ����</param>
        /// <returns>�������� ����� �����</returns>
        private int ToValue(UInt16 valueTwosComplement)
        {
            int value = 0;

            if ((valueTwosComplement & 0x8000) == 0x8000)
            {
                // ����� ������������� (������� ������ ���������� � 1)
                unchecked
                {
                    // ��������������� ����� � ������ ���
                    valueTwosComplement = (UInt16)(~valueTwosComplement);
                    valueTwosComplement++;
                    value = Convert.ToInt32(valueTwosComplement) * -1;
                    return value;
                }
            }
            else
            {
                // ����� �������������
                unchecked
                {
                    value = (Int16)valueTwosComplement;
                }
            }
            return value;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// ����������� �������� ����� ����� � �������������� ���
        /// </summary>
        /// <param name="value">�������� ����� �����</param>
        /// <returns>�������������� ���</returns>
        private UInt16 ToTwosComplementValue(Int16 value)
        {
            UInt16 res = 0;

            res = (UInt16)value;

            unchecked
            {
                if (value < 0)
                {
                    res = (UInt16)(((~res) + 1) | (0x8000));
                }
            }

            return res;
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region ����� INotifyPropertyChanged
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ����� ��������� ��������
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Events
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ����� ���������� ��������
        /// </summary>
        //public event NGK.Devices.PropertyChangingEventHandler PropertyChanging;
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region ����� IMeasuringDevice
        //------------------------------------------------------------------------------------------------------
        object IMeasuringDevice.Deserialize(string path)
        {
            return MeasuringDeviceMainPower.Deserialize(path);
        }
        //------------------------------------------------------------------------------------------------------
        object IMeasuringDevice.Deserialize(FileStream stream)
        {
            return MeasuringDeviceMainPower.Deserialize(stream);
        }
        //------------------------------------------------------------------------------------------------------
        public object DeserializeXml(string path)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------
        public object DeserializeXml(FileStream stream)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------
        public Boolean Serialize(string path)
        {
            return MeasuringDeviceMainPower.Serialize(path, this);
        }
        //------------------------------------------------------------------------------------------------------
        public Boolean Serialize(FileStream stream)
        {
            return MeasuringDeviceMainPower.Serialize(stream, this);
        }
        //------------------------------------------------------------------------------------------------------
        public Boolean SerializeXml(string path)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------
        public Boolean SerializeXml(FileStream stream)
        {
            throw new NotImplementedException();
        }
        //------------------------------------------------------------------------------------------------------
        public UInt64 GetSerialNumber()
        {
            return this._SerialNumber;
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region ����� IDeserializationCallback
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������ ��� ����� ��������������� �������, � ��������� � �������� ��� ����������
        /// ����� � ������� ������, ��� ��������� �������������. ����� ������������� �������������
        /// ����������� ����� ����������� � ������ ������ ������
        /// </summary>
        /// <param name="sender"></param>
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            return;
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
    } // End of class
    //==========================================================================================================
} // End of namespace
//==============================================================================================================
// End of file