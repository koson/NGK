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
    /// Класс реализует устройство БИ (блок измерений) на постоянном питании БИ(У)-00 (9810)
    /// </summary>
    [Serializable]
    public class MeasuringDeviceMainPower : System.ComponentModel.INotifyPropertyChanged, IMeasuringDevice,
        System.Runtime.Serialization.IDeserializationCallback
    {
        //------------------------------------------------------------------------------------------------------
        #region Fields and Properties
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Адрес устройства БИ
        /// </summary>
        private Byte _AddressSlave = 1;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Адрес устройства БИ (при работе по технологическому кабелю)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Настройки сети")]
        [Description("Адрес устройства НГК-БИ (при работе по технологическому кабелю)")]
        [DisplayName(@"Адрес устройства НГК-БИ сервис")]
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
        /// Возвращает тип исполнения устройства БИ (на батарейном или основном питании)
        /// </summary>
        /// <remarks>
        /// Input registr 0x0000
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Вариант исполнения устройства НГК-БИ в составе НГК-КИП")]
        [DisplayName(@"Тип устройства")]
        [TypeConverter(typeof(TypeConverterTypeOfDeviceNGK))]
        public TYPE_NGK_DEVICE TypeOfDevice
        {
            get { return TYPE_NGK_DEVICE.BI_MAIN_POWERED; }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Версия ПО
        /// </summary>
        /// <remarks>
        /// Input Register 0x0001
        /// </remarks>
        private float _SofwareVersion = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Версия ПО
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Версия ПО")]
        [DisplayName("Версия ПО")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float SofwareVersion
        {
            get { return _SofwareVersion; }
            set
            {
                _SofwareVersion = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("SofwareVersion"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Версия аппаратуры
        /// </summary>
        /// <remarks>
        /// Input Register 0x0002
        /// </remarks>
        private float _HardwareVersion = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Версия аппаратуры
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Версия аппаратуры")]
        [DisplayName("Версия аппаратуры")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float HardwareVersion
        {
            get { return _HardwareVersion; }
            set
            {
                _HardwareVersion = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("HardwareVersion"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        /// <remarks>
        /// Input register 0x000C и 0x000D
        /// </remarks>
        private UInt64 _SerialNumber = UInt64.MaxValue;
        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        //------------------------------------------------------------------------------------------------------
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Серийный номер устройства")]
        [DisplayName("Серийный номер")]
        [DefaultValue(typeof(UInt64), "0xFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt64 SerialNumber
        {
            get { return _SerialNumber; }
            set
            {
                _SerialNumber = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("SerialNumber"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Контрольная сумма визитной карточки
        /// </summary>
        private UInt16 _CRC16 = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Контрольная сумма визитной карточки
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Контрольная сумма визитной карточки устройства")]
        [DisplayName("Контрольная сумма")]
        //[DefaultValue(typeof(UInt64), "0xFFFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 CRC16
        {
            get { return _CRC16; }
            set { _CRC16 = value; }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Код производителя 
        /// </summary>
        private UInt16 _CodeManufacturer = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Код производителя
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Код производителя")]
        [DisplayName("Код производителя")]
        //[DefaultValue(typeof(UInt64), "0xFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 CodeManufacturer
        {
            get { return _CodeManufacturer; }
            set 
            { 
                _CodeManufacturer = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("CodeManufacturer"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Поляризационный потенциал
        /// </summary>
        /// <remarks>
        /// Преробразовать из UInt16 в формат -2.00...+2.00 В
        /// Input Register
        /// </remarks>
        private float _polarization_potential;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Поляризационный потенциал
        /// </summary>
        /// <remarks>
        /// Input Register 0x0008
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Поляризационный потенциал подземного трубопровода по методу вспомогательного")]
        [DisplayName("Поляризационный потенциал, В")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float PolarizationPotential
        {
            get { return _polarization_potential; }
            set 
            { 
                _polarization_potential = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("PolarizationPotential"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Защитный потенциал
        /// </summary>
        /// <remarks>
        /// Преробразовать из UInt16 в формат -10.00...+10.00 В
        /// Input Register
        /// </remarks>
        private float _protective_potential;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Защитный потенциал
        /// </summary>
        /// <remarks>
        /// Input Register 0x0009
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Защитный потенциал, В")]
        [DisplayName("Защитный потенциал, В")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float ProtectivePotential
        {
            get { return _protective_potential; }
            set
            {
                _protective_potential = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("ProtectivePotential"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Наведённое переменное напряжение на трубопровод от 0 до 100,0 В частотой 50Гц
        /// </summary>
        /// <remarks>
        /// Input Register 0x000A
        /// </remarks>
        private float _InducedVoltage;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Наведённое переменное напряжение на трубопровод от 0 до 100,0 В частотой 50Гц
        /// </summary>
        /// <remarks>
        /// Input Register 0x000A
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Наведённое переменное напряжение на трубопровод от 0 до 100,0 В частотой 50Гц")]
        [DisplayName("Наведённое напряжение, В")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float InducedVoltage
        {
            get { return _InducedVoltage; }
            set
            {
                _InducedVoltage = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("InducedVoltage"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Защитный ток
        /// </summary>
        /// <remarks>
        /// Преробразовать из UInt16 в формат 0.0...+150.0 A
        /// </remarks>
        /// <remarks>
        /// Input Register 0x000B
        /// </remarks>
        private float _protective_current;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Защитный ток
        /// </summary>
        /// <remarks>
        /// Input Register 0x000B
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Ток катодной защиты в точке дренажа методом измерения напряжения на внешнем шунте")]
        [DisplayName("Ток катодной защиты, А")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float ProtectiveСurrent
        {
            get { return _protective_current; }
            set
            {
                _protective_current = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("ProtectiveСurrent"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Поляризационный ток
        /// </summary>
        /// <remarks>
        /// Преробразовать из UInt16 в формат -5.00...+5.00 mA
        /// </remarks>
        /// <remarks>
        /// Input Register 0x000C
        /// </remarks>
        private float _polarization_current;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Поляризационный ток
        /// </summary>
        /// <remarks>
        /// Input Register 0x000C
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Ток поляризации вспомогательного электрода")]
        [DisplayName("Ток поляризации, mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float PolarizationСurrent
        {
            get { return _polarization_current; }
            set
            {
                _polarization_current = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("PolarizationСurrent"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ток измерительного канала 1
        /// </summary>
        /// <remarks>
        /// Input Register 0x0005
        /// 0х02FF- 4mA-0x0EF9- 20 mA; 1bit-0,01mA
        /// Передаются значения АЦП, соответствующие диапазону 4-20 мА.
        /// Значения, соответствующие меньше 4 мА считаются обрывом измерительного канала, более 20 мА - КЗ
        /// </remarks>
        private float _CurrentChannel1;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ток измерительного канала 1
        /// </summary>
        /// <remarks>
        /// Input Register 0x0005
        /// 0х02FF- 4mA-0x0EF9- 20 mA; 1bit-0,01mA
        /// Передаются значения АЦП, соответствующие диапазону 4-20 мА.
        /// Значения, соответствующие меньше 4 мА считаются обрывом измерительного канала, более 20 мА - КЗ
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Ток измерительного канала 1")]
        [DisplayName("Ток измерительного канала 1 (4-20), mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float CurrentChannel1
        {
            get { return _CurrentChannel1; }
            set
            {
                _CurrentChannel1 = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentChannel1"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ток измерительного канала 2
        /// </summary>
        /// <remarks>
        /// Input Register 0x0006
        /// 0х02FF- 4mA-0x0EF9- 20 mA; 1bit-0,01mA
        /// Передаются значения АЦП, соответствующие диапазону 4-20 мА.
        /// Значения, соответствующие меньше 4 мА считаются обрывом измерительного канала, более 20 мА - КЗ
        /// </remarks>
        private float _CurrentChannel2;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ток измерительного канала 2
        /// </summary>
        /// <remarks>
        /// Input Register 0x0006
        /// 0х02FF- 4mA-0x0EF9- 20 mA; 1bit-0,01mA
        /// Передаются значения АЦП, соответствующие диапазону 4-20 мА.
        /// Значения, соответствующие меньше 4 мА считаются обрывом измерительного канала, более 20 мА - КЗ
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Ток измерительного канала 2")]
        [DisplayName("Ток измерительного канала 2 (4-20), mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float CurrentChannel2
        {
            get { return _CurrentChannel2; }
            set
            {
                _CurrentChannel2 = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentChannel1"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Глубина коррозии датчика ИКП с устройства УСИКПСТ
        /// </summary>
        /// <remarks>
        /// Преробразовать из UInt16 в формат 0...1200 мкм
        /// Input Register 0x0007
        /// </remarks>
        private UInt16 _DepthOfCorrosion;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Глубина коррозии датчика ИКП с устройства УСИКПСТ
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Глубина коррозии датчика ИКП с устройства УСИКПСТ")]
        [DisplayName("Глубина коррозии УСИКПСТ, мкм")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 DepthOfCorrosion
        {
            get { return _DepthOfCorrosion; }
            set
            {
                if (value > 0xFFFE)
                {
                    throw new ArgumentOutOfRangeException("DepthOfCorrosion",
                        "Попытка присвоить параметру недопустимое значение. Диапазон допустимых значений 0x0000...0xFFFE");
                }
                else
                {
                    _DepthOfCorrosion = value;
                    // Генерируем событие
                    OnPropertyChanged(new PropertyChangedEventArgs("DepthOfCorrosion"));
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Cкорость коррозии датчика ИКП с устройства УСИЛПСТ
        /// </summary>
        /// <remarks>
        /// Преробразовать из UInt16 в формат 0...65.534 мкМ/Год
        /// Input Register 0x0008
        /// </remarks>
        private float _SpeedOfCorrosion;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Cкорость коррозии датчика ИКП с устройства УСИКПСТ
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Скорость коррозии датчика ИКП с устройства УСИКПСТ")]
        [DisplayName("Скорость коррозии УСИКПСТ, мкм")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float SpeedOfCorrosion
        {
            get { return _SpeedOfCorrosion; }
            set
            {
                _SpeedOfCorrosion = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("SpeedOfCorrosion"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние УСИКПСТ
        /// </summary>
        private UInt16 _StatusUSIKPST;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние УСИКПСТ
        /// </summary>
        /// <remarks>
        /// Input Register 0x0009
        /// 0x0000 - норма 0xFFFF – нет связи Либо код 
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Состояние устройства УСИКПСТ Коды аварий приведены ниже:\n 0 - устройство в норме;\n 65535 - нет связи;\n 1 - некорректная функция (не поддерживается Устройством);\n 2 – зарезервировано;\n 3 - не подключен индикатор коррозионных процессов;\n 4 - верификация микросхемы ПЗУ Устройства выявила ошибки (режим конфигурирования);\n 5 - заданная скорость обмена не поддерживается Устройством (режим конфигурирования);\n 6 - данный тип индикатора не обслуживается;\n 7 – индикатор коррозионных процессов не инициализирован;\n 8 – текущая дата некорректна;\n 9 - невозможно определить состояние ИЭ ИКП.")]
        [DisplayName("Код состояния устройства УСИКПСТ")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 StatusUSIKPST
        {
            get { return _StatusUSIKPST; }
            set
            {
                _StatusUSIKPST = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("StatusUSIKPST"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Питающее напряжение (реализуется позднее)
        /// </summary>
        /// <remarks>
        /// Input Register 0x000A
        /// Диапазон 9-15 В для БИ(У) на батарейном питании, 18-55 В для проводной БИ(У
        /// 1 bit == 0,05 В; 9-55В (0х00B4-0x044С hex)
        /// </remarks>
        private float _SupplyVoltage;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Питающее напряжение
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Напряжение питания НГК-БИ, В")]
        [DisplayName("Напряжение питания НГК-БИ, В")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float SupplyVoltage
        {
            get { return _SupplyVoltage; }
            set
            {
                _SupplyVoltage = (float)(value * 0.05);
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("SupplyVoltage"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Напряжение встроенного элемента питания
        /// </summary>
        /// <remarks>
        /// Input Register 0x000B
        /// 1 bit == 0,01 В 
        /// 1,8-3,6В (0х00В4-0x0168 hex) 
        /// </remarks>
        private float _BattaryVoltage;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Напряжение встроенного элемента питания
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Напряжение батареи, В")]
        [DisplayName("Напряжение батареи, В")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float BattaryVoltage
        {
            get { return _BattaryVoltage; }
            set
            {
                _BattaryVoltage = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("BattaryVoltage"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Температура встроенного датчика БИ(У) Только для БИ-У-01
        /// </summary>
        /// <remarks>
        /// Input Register 0x000C 
        /// 1 bit == 1 °С
        /// -40/+85°С (Пределы измерения будут приведены после Юстировки)
        /// В дополнительном коде
        /// </remarks>
        //private Int16 _InternalTemperatureSensor;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Температура встроенного датчика БИ(У) Только для БИ-У-01
        /// </summary>
        //[Browsable(true)]
        //[ReadOnly(true)]
        //[Category("Измеряемые параметры")]
        //[Description("Температура встроенного датчика, гр.С. Только для БИ-У-01")]
        //[DisplayName("Температура встроенного датчика, гр.С")]
        //[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        //public Int16 InternalTemperatureSensor
        //{
        //    get { return _InternalTemperatureSensor; }
        //    set
        //    {
        //        _InternalTemperatureSensor = value;
        //        // Генерируем событие
        //        OnPropertyChanged(new PropertyChangedEventArgs("InternalTemperatureSensor"));
        //    }
        //}
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ток натекания постоянный
        /// </summary>
        /// <remarks>
        /// Input Register 0x0015
        /// 1 bit == 0,01 mA 
        /// -5...+5 mA (0хFE0C-0x01F4 hex) (дополнительный код) 
        /// </remarks>
        private float _ReferenceElectrodeDcСurrent;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ток натекания постоянный
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Ток натекания постоянный, mA")]
        [DisplayName("Ток натекания постоянный, mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float ReferenceElectrodDcСurrent
        {
            get { return _ReferenceElectrodeDcСurrent; }
            set
            {
                _ReferenceElectrodeDcСurrent = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("LeakageDcСurrent"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ток натекания переменный
        /// </summary>
        /// <remarks>
        /// Input Register 0x0016
        /// 1 bit == 0,01 mA 
        /// 0...+5 mA (0х0000-0x01F4 hex) 
        /// </remarks>
        private float _ReferenceElectrodeAcСurrent;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Ток натекания переменный
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Ток натекания переменный, mA")]
        [DisplayName("Ток натекания переменный, mA")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float ReferenceElectrodeAcСurrent
        {
            get { return _ReferenceElectrodeAcСurrent; }
            set
            {
                _ReferenceElectrodeAcСurrent = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("LeakageAcСurrent"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Discrete Input
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Датчик встрытия НГК-КИП СМ(У)
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0000
        /// </remarks>
        private Boolean _caseOpen;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Датчик встрытия НГК-КИП СМ(У)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Состояние блока измерений")]
        [Description("Состояние датчика вскрытия НГК-КИП СМ(У)")]
        [DisplayName("Датчик встрытия НГК-КИП СМ(У)")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean CaseOpen
        {
            get { return _caseOpen; }
            set
            {
                _caseOpen = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("CaseOpen"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние напряжения притания устройства БИ (норм./ненорм.)
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0001
        /// Напряжение питания ниже нормы
        /// Порог срабатывания канала для БИ(У) на автономном питании Uнорм < 13,6В, где
        /// Uнорм=Uизм±K1*|t|.
        /// Для проводного питания при U изм. < 20В
        /// Дискрет измерения Uизм 0,05В
        /// </remarks>
        private Boolean _SupplyVoltageStatus;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние напряжения притания устройства БИ (норм./ненорм.)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Состояние блока измерений")]
        [Description("Состояние напряжения притания устройства НГК-БИ,  Discretes Input	0x0001")]
        [DisplayName("Состояние напряжения притания устройства НГК-БИ")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean SupplyVoltageStatus
        {
            get { return _SupplyVoltageStatus; }
            set
            {
                _SupplyVoltageStatus = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("SupplyVoltageStatus"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние элемента питания
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0002
        /// Норма/Ниже нормы
        /// Порог срабатывания канала Uнорм < 3,4В, где
        /// Uнорм=Uизм±K1*|t|.
        /// Дискрет измерения Uизм 0,01В
        /// </remarks>
        private Boolean _BattaryStatus;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние элемента питания
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Состояние блока измерений")]
        [Description("Состояние элемента питания")]
        [DisplayName("Элемент питания, норма")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean BattaryStatus
        {
            get { return _BattaryStatus; }
            set
            {
                _BattaryStatus = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("BattaryStatus"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние пластины датчика "1" скорости коррозии
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0003
        /// </remarks>
        private Boolean _CorrosionSensor_1;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние пластины датчика "1" скорости коррозии
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Состояние пластины датчика №1 скорости коррозии")]
        [DisplayName("Датчик коррозии №1")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean CorrosionSensor1
        {
            get { return _CorrosionSensor_1; }
            set
            {
                _CorrosionSensor_1 = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("CorrosionSensor1"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние пластины датчика "2" скорости коррозии
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0004
        /// </remarks>
        private Boolean _CorrosionSensor_2;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние пластины датчика "2" скорости коррозии
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Состояние пластины датчика №2 скорости коррозии")]
        [DisplayName("Датчик коррозии №2")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean CorrosionSensor2
        {
            get { return _CorrosionSensor_2; }
            set
            {
                _CorrosionSensor_2 = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("CorrosionSensor2"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние пластины датчика "3" скорости коррозии
        /// </summary>
        /// <remarks>
        /// Discretes Input	0x0005
        /// </remarks>
        private Boolean _CorrosionSensor_3;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Состояние пластины датчика "3" скорости коррозии
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Измеряемые параметры")]
        [Description("Состояние пластины датчика №3 скорости коррозии")]
        [DisplayName("Датчик коррозии №3")]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean CorrosionSensor3
        {
            get { return _CorrosionSensor_3; }
            set
            {
                _CorrosionSensor_3 = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("CorrosionSensor3"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Holding Registers
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Номер устройства в стети CAN и адрес устройства в сети Modbus
        /// </summary>
        /// <remarks>
        /// Holding register 0x0000
        /// 1...127 (По умолчанию 127)
        /// </remarks>
        private Byte _NetAddress = 127;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Номер устройства в стети CAN и адрес устройства в сети Modbus
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки сети")]
        [Description("Номер устройства в стети CAN и адрес устройства в сети Modbus (1...127, по умолчанию 127)")]
        [DisplayName(@"Адрес/Номер устройства")]
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
                    // Генерируем событие
                    OnPropertyChanged(new PropertyChangedEventArgs("NetAddress"));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("NetAddress",
                        "Попытка присвоить недопустимое значение параметру. Значения параметра должны находиться в диапазоне 1...127");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период измерений и передачи информации
        /// </summary>
        /// <remarks>
        /// Holding register 0x0001 и 0x0002
        /// 1 bit == 1 сек.
        /// 0сек.…7сут. (0х0000-0x93A80 hex)
        /// 0 – измерять постоянно. 0xFFFFFFFF -передача данных только по запросу. 
        /// Для БИ(У) на автономном питании значение параметра 0x00 – 0x0A недопустимо. Возвращать исключение 0x03.
        /// </remarks>
        private UInt32 _MeasuringPeriod = 0xFFFFFFFF;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период измерений и передачи информации
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки измерений")]
        [Description("Период измерений и передачи информации (по умолчанию 1 мин.). 0 – измерять постоянно. Бесконечный период с включенным режимом энергопотребления – передача данных только по запросу")]
        [DisplayName("Период измерений, сек.")]
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
                    // Генерируем событие
                    OnPropertyChanged(new PropertyChangedEventArgs("MeasuringPeriod"));
                }
                else if (value == 0xFFFFFFFF)
                {
                    _MeasuringPeriod = value;
                    // Генерируем событие
                    OnPropertyChanged(new PropertyChangedEventArgs("MeasuringPeriod"));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("MeasuringPeriod", 
                        "Значение вне допустимого диапазона значений");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период измерения питающего напряжения для версии БИ(У)-00
        /// </summary>
        /// <remarks>
        /// Holding register 0x0003
        /// 1 bit == 1 сек. 
        /// 1сек.…100сек. (0х0001-0x0064 hex)
        /// Версии БИ(У)-01 измеряет питающее напряжение и напряжение встроенного элемента 
        /// питания каждый раз после истечения периода измерения и перехода из режима «сна» в активный режим
        /// </remarks>
        private UInt16 _MeasuringVoltagePeriod = 0x000A;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период измерения питающего напряжения для версии БИ(У)-00
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки измерений")]
        [Description("Версии НГК-БИ(У)-01 измеряет питающее напряжение и напряжение встроенного элемента питания каждый раз после истечения периода измерения и перехода из режима «сна» в активный режим. 1...100 сек.")]
        [DisplayName("Период измерения питающего напряжения, сек.")]
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
                        "Значение не входит в диапазон допустимых значений 1...100");
                }
                else
                {
                    _MeasuringVoltagePeriod = value;
                    // Генерируем событие
                    OnPropertyChanged(new PropertyChangedEventArgs("MeasuringVoltagePeriod"));
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период опроса УСИКПСТ (1...65535 мин)
        /// </summary>
        /// <remarks>
        /// Holding register 0x0004
        /// 1 bit == 10 сек.
        /// 10сек.…7сут.(0х0001-0xFFFF hex)
        /// 0xFFFF – неактивный канал Возможен выбор только одного из двух каналов измерения, либо оба не активные
        /// </remarks>
        private UInt32 _PollingPeriodUSIKPST = 0xFFFF * 10;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период опроса УСИКПСТ (1...65535 мин)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки измерений")]
        [Description("Период опроса УСИКПСТ от 10 сек до 7 сут.")]
        [DisplayName("Период опроса УСИКПСТ, сек")]
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
                        // Канал выключен
                        _PollingPeriodUSIKPST = value;
                        // Генерируем событие
                        OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodUSIKPST"));

                    }
                    else
                    {
                        // значение должно быть кратно 10
                        uint x = value % 10;
                        if (x != 0)
                        {
                            throw new ArgumentException(
                                "Попытка присвоить свойству значение не кратное 10",
                                "PollingPeriodUSIKPST");
                        }
                        else
                        {
                            // Канал включен
                            _PollingPeriodUSIKPST = value;
                            // Если канал опроса УСИКПСТ включён, то необходимо выключить
                            // канал опроса датчиков БПИ
                            this.PollingPeriodBPI = 0xFFFF * 10;
                            // Генерируем событие
                            OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodUSIKPST"));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Попытка присвоить свойству недопустимое значение 0",
                        "PollingPeriodUSIKPST");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период опроса датчиков БПИ
        /// </summary>
        /// <remarks>
        /// Holding register 0x0005
        /// 1 bit == 10 сек.
        /// 10сек.…7сут. (0х0001-0xFFFF hex)
        /// 0xFFFF – неактивный канал Возможен выбор только одного из двух каналов измерения, либо оба не активные
        /// </remarks>
        private UInt32 _PollingPeriodBPI = 0xFFFF * 10;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период опроса датчиков БПИ
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки измерений")]
        [Description("Период опроса датчика БПИ от 10 сек до 7 сут.")]
        [DisplayName("Период опроса БПИ, сек")]
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
                        // Канал выключен
                        _PollingPeriodBPI = value;
                        // Генерируем событие
                        OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodBPI"));

                    }
                    else
                    {
                        // значение должно быть кратно 10
                        uint x = value % 10;
                        if (x != 0)
                        {
                            throw new ArgumentException(
                                "Введённое значение не кратно 10",
                                "PollingPeriodBPI");
                        }
                        else
                        {
                            // Канал включен
                            _PollingPeriodBPI = value;
                            // Если канал опроса УСИКПСТ включён, то необходимо выключить
                            // канал опроса устройсва УСИКПСТ
                            this.PollingPeriodUSIKPST = 0xFFFF * 10;
                            // Генерируем событие
                            OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodBPI"));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Попытка присвоить свойству недопустимое значение 0",
                        "PollingPeriodBPI");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период опроса измерительного канала 1: 4-20 мА
        /// </summary>
        /// <remarks>
        /// Holding register 0x0006
        /// 1 bit == 10 сек.
        /// 10сек.…7сут. (0х0001-0xFFFF hex)
        /// 0xFFFF – неактивный канал
        /// </remarks>
        private UInt32 _PollingPeriodChannel1 = 0xFFFF * 10; // Канал по умолчанию выключен
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        ///  Период опроса измерительного канала 1: 4-20 мА
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки измерений")]
        [Description("Период опроса канала 1 от 10 сек до 7 сут.")]
        [DisplayName("Период опроса канала 1, сек")]
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
                        // Канал выключен
                        _PollingPeriodChannel1 = value;
                        // Генерируем событие
                        OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodChannel1"));

                    }
                    else
                    {
                        // значение должно быть кратно 10
                        uint x = value % 10;
                        if (x != 0)
                        {
                            throw new ArgumentException(
                                "Введённое значение не кратно 10",
                                "PollingPeriodChannel1");
                        }
                        else
                        {
                            // Канал включен
                            _PollingPeriodChannel1 = value;
                            // Генерируем событие
                            OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodChannel1"));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Попытка присвоить свойству недопустимое значение 0",
                        "PollingPeriodChannel1");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Период опроса измерительного канала 2: 4-20 мА
        /// </summary>
        /// <remarks>
        /// Holding register 0x0007
        /// 1 bit == 10 сек.
        /// 10сек.…7сут. (0х0001-0xFFFF hex)
        /// 0xFFFF – неактивный канал
        /// </remarks>
        private UInt32 _PollingPeriodChannel2 = 655350; // Канал по умолчанию выключен
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        ///  Период опроса измерительного канала 2: 4-20 мА
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки измерений")]
        [Description("Период опроса канала 2 от 10 сек до 7 сут.")]
        [DisplayName("Период опроса канала 2, сек")]
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
                        // Канал выключен
                        _PollingPeriodChannel2 = value;
                        // Генерируем событие
                        OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodChannel2"));

                    }
                    else
                    {
                        // значение должно быть кратно 10
                        uint x = value % 10;
                        if (x != 0)
                        {
                            throw new ArgumentException(
                                "Введённое значение не кратно 10",
                                "PollingPeriodChannel2");
                        }
                        else
                        {
                            // Канал включен
                            _PollingPeriodChannel2 = value;
                            // Генерируем событие
                            OnPropertyChanged(new PropertyChangedEventArgs("PollingPeriodChannel2"));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Попытка присвоить свойству недопустимое значение 0",
                        "PollingPeriodChannel2");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Скорость обмена в сети CAN кБ/сек
        /// </summary>
        /// <remarks>
        /// Holding register 0x0008
        /// 1 bit == 1 кБит/с
        /// 100 кБит/с; 50 кБит/с; 20 кБит/с; 10 кБит/с (0х000A-0x03E8 hex)
        /// Значения не из списка – отдавать исключение 0х03
        /// </remarks>
        private CANBaudRate _BaudRateCAN = CANBaudRate.BR20K;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Скорость обмена в сети CAN кБ/сек 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки сети")]
        [Description("Скорость обмена кБ/сек в сети CAN")]
        [DisplayName(@"Скорость обмена СAN, кБ/сек")]
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
                    // Генерируем событие
                    OnPropertyChanged(new PropertyChangedEventArgs("BaudRateCAN"));
                }
                else
                {
                    throw new ArgumentException("Попытка присвоить некорректное значение свойству", 
                        "BaudRateCAN");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Значение номинального тока внешнего шунта (0...100А)
        /// </summary>
        /// <remarks>
        /// Holding register 0x0009
        /// 1 bit == 1 А
        /// 10, 20, 30, 50, 75, 100, 150 (0х000A-0x0096 hex)
        /// По умолчанию 50А
        /// </remarks>
        private CurrentShuntValues _CurrentShuntValue = CurrentShuntValues.A50;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Значение номинального тока внешнего шунта (0...100А)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки измерений")]
        [Description("Значение номинального тока внешнего шунта (0...150А)")]
        [DisplayName("Токовый шунт, А")]
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
                    // Генерируем событие
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentShuntValue"));
                }
                else
                {
                    throw new ArgumentException(
                        "Попытка присвоить свойству некорректное значение", 
                        "CurrentShuntValue");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Текущее время
        /// </summary>
        /// <remarks>
        /// Holding register 0x000A и 0x000B
        /// </remarks>
        private DateTime _DateTime;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Текущее время
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Установить системное время и дату в устройстве НГК-БИ")]
        [DisplayName("Дата и время")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public DateTime DateTime
        {
            get { return _DateTime; }
            set
            {
                _DateTime = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("DateTime"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Coils
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала измерения поляризационного 
        /// потенциала подземного трубопровода по методу вспомогательного
        /// </summary>
        /// <remarks>
        /// Coil 0x0000
        /// </remarks>
        private Boolean _PolarizationPotentialEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Разрешение работы канала измерения поляризационного 
        /// потенциала подземного трубопровода по методу вспомогательного
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение работы канала измерения поляризационного потенциала подземного трубопровода по методу вспомогательного")]
        [DisplayName("Измерение поляризационного потенциала")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean PolarizationPotentialEn
        {
            get { return _PolarizationPotentialEn; }
            set
            {
                _PolarizationPotentialEn = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("PolarizationPotentialEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала измерения защитного потенциала
        /// </summary>
        /// <remarks>
        /// Coil 0x0001
        /// </remarks>
        private Boolean _ProtectivePotentialEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Разрешение работы канала измерения защитного потенциала
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение работы канала измерения защитного потенциала")]
        [DisplayName("Измерение защитного потенциала")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean ProtectivePotentialEn
        {
            get { return _ProtectivePotentialEn; }
            set
            {
                _ProtectivePotentialEn = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("ProtectivePotentialEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала измерения тока катодной защиты 
        /// в точке дренажа методом измерения напряжения на внешнем шунте
        /// </summary>
        /// <remarks>
        ///  Coil 0x0002
        ///  </remarks>
        private Boolean _ProtectiveСurrentEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Разрешение работы канала измерения тока катодной защиты 
        /// в точке дренажа методом измерения напряжения на внешнем шунте
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение работы канала измерения тока катодной защиты в точке дренажа методом измерения напряжения на внешнем шунте")]
        [DisplayName("Измерение защитного тока")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean ProtectiveСurrentEn
        {
            get { return _ProtectiveСurrentEn; }
            set
            {
                _ProtectiveСurrentEn = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("ProtectiveСurrentEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала тока поляризации вспомогательного 
        /// электрода
        /// </summary>
        /// <remarks>
        ///  Coil 0x0003
        ///  </remarks>
        private Boolean _PolarizationСurrentEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Разрешение работы канала тока поляризации вспомогательного 
        /// электрода
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение работы канала тока поляризации вспомогательного электрода")]
        [DisplayName("Измерение тока поляризации")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean PolarizationСurrentEn
        {
            get { return _PolarizationСurrentEn; }
            set
            {
                _PolarizationСurrentEn = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("PolarizationСurrentEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала измерения наведённого переменного напряжения на трубопровод
        /// </summary>
        /// <remarks>
        ///  Coil 0x0004
        /// </remarks>
        private Boolean _InducedVoltageEn = true;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Разрешение работы канала измерения наведённого переменного напряжения на трубопровод
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение работы канала измерения наведённого переменного напряжения на трубопровод")]
        [DisplayName("Измерение наведённого напряжения")]
        [DefaultValue(true)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean InducedVoltageEn
        {
            get { return _InducedVoltageEn; }
            set
            {
                _InducedVoltageEn = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("InducedVoltageEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг включения расширенного диапазона Х10 для канала измерения суммарного потенциала
        /// </summary>
        /// <remarks>
        /// Coil 0x0005
        /// Не Установл. Для БИ(У) исполнения Uсум=±5В
        /// </remarks>
        private Boolean _ExtendedSumPotentialEn = false;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг включения расширенного диапазона Х10 для канала измерения суммарного потенциала
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение работы расширенного диапазона X10 для канала измерения суммарного потенциала")]
        [DisplayName("Разрешение расширенного диапазона Х10")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean ExtendedSumPotentialEn
        {
            get { return _ExtendedSumPotentialEn; }
            set
            {
                _ExtendedSumPotentialEn = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("ExtendedSumPotentialEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения передачи слова состояния
        /// </summary>
        /// <remarks>
        /// Coil 0x0006
        /// Не Установл.
        /// </remarks>
        private Boolean _SendStatusWordEn = false;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения передачи слова состояния
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение передачи слова состояния")]
        [DisplayName("Разрешение передачи слова состояния")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean SendStatusWordEn
        {
            get { return _SendStatusWordEn; }
            set
            {
                _SendStatusWordEn = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("SendStatusWordEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала измерения постоянного тока натекания
        /// (ток между трубой и электродом сравнения)
        /// </summary>
        /// <remarks>
        /// Coil 0x0007
        /// Не Установл.
        /// </remarks>
        private Boolean _DcCurrentRefereceElectrodeEn = false;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала измерения постоянного тока натекания
        /// (ток между трубой и электродом сравнения)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение работы канала измерения постоянного тока натекания (ток между трубой и электродом сравнения)")]
        [DisplayName("Разрешение измерения DC тока натекания")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean DcCurrentRefereceElectrodeEn
        {
            get { return _DcCurrentRefereceElectrodeEn; }
            set
            {
                _DcCurrentRefereceElectrodeEn = value;
                // Генерируем событие
                OnPropertyChanged(new PropertyChangedEventArgs("DcCurrentRefereceElectrodeEn"));
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала измерения переменного тока натекания
        /// (ток между трубой и электродом сравнения)
        /// </summary>
        /// <remarks>
        /// Coil 0x0008
        /// Не Установл.
        /// </remarks>
        private Boolean _AcCurrentRefereceElectrodeEn = false;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Флаг разрешения работы канала измерения переменного тока натекания
        /// (ток между трубой и электродом сравнения)
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки управления НГК-КИП")]
        [Description("Разрешение работы канала измерения переменного тока натекания (ток между трубой и электродом сравнения)")]
        [DisplayName("Разрешение измерения AC тока натекания")]
        [DefaultValue(false)]
        [TypeConverter(typeof(BooleanTypeConverter))]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Boolean AcCurrentRefereceElectrodeEn
        {
            get { return _AcCurrentRefereceElectrodeEn; }
            set
            {
                _AcCurrentRefereceElectrodeEn = value;
                // Генерируем событие
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
        /// Конструктор по умолчанию
        /// </summary>
        public MeasuringDeviceMainPower()
        {}
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Methods
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Возвращает строку с описанием текущих параметров устройства
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return base.ToString();

            StringBuilder sb = new StringBuilder();

            sb.Append(String.Format("Дата: {0} Время: {1}",
                DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("СИСТЕМНЫЕ ДАННЫЕ:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Тип НГК-БИ в составе КИП: НГК-БИ(У)-00");
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Серийный номер НГК-БИ в соствае КИП: {0}", this.SerialNumber.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Код производителя: {0}", this.CodeManufacturer.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Версия ПО: {0}", this.SofwareVersion.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Версия аппаратуры: {0}", this.HardwareVersion.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Системное время: {0} дата {1}",
                this.DateTime.ToLongTimeString(), this.DateTime.ToShortDateString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("Настройки управления НГК-КИП:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Измерение защитного потенциала: {0}", this.ProtectivePotentialEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Измерение защитного тока: {0}", this.ProtectiveСurrentEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Измерение наведённого напряжения: {0}", this.InducedVoltageEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Измерение поляризационного потенциала: {0}", this.PolarizationPotentialEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Измерение тока поляризации: {0}", this.PolarizationСurrentEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Разрешение расширенного диапазона х10: {0}", this.ExtendedSumPotentialEn.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("НАСТРОЙКИ СЕТИ:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Адрес устройства НГК-БИ сервис: {0}", this.AddressSlave.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Адрес/номер устройства: {0}", this.NetAddress.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Скорость обмена CAN, кБ/сек: {0}", this.BaudRateCAN.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("НАСТРОЙКИ ИЗМЕРЕНИЙ:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Период измерений, сек: {0}", this.MeasuringPeriod.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Период опроса БПИ, сек: {0}", this.PollingPeriodBPI.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Период измерения питающего напряжения, сек: {0}", 
                this.MeasuringVoltagePeriod.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Период опроса канала 1, сек: {0}", this.PollingPeriodChannel1.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Период опроса канала 2, сек: {0}", this.PollingPeriodChannel2.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Период опроса УС ИКП СТ, сек: {0}", this.PollingPeriodUSIKPST.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Токовый шунт, А: {0}", this.CurrentShuntValue.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("СОСТОЯНИЕ НГК-БИ В СОСТАВЕ НГК-КИП СМ(У):");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Датчик вскрытия: {0}", this.CaseOpen.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Состояние напряжения питания: {0}", this.SupplyVoltageStatus.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Состояние элемента питания: {0}", this.BattaryStatus.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("ИЗМЕРЯЕМЫЕ ПАРАМЕТРЫ:");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Глубина коррозии УС ИКП СТ, мкм: {0}", this.DepthOfCorrosion.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Скорость коррозии УС ИКП СТ, мкм: {0}", this.SpeedOfCorrosion.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Код состояния УС ИКП СТ: {0}", this.StatusUSIKPST.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Датчик коррозии №1: {0}", this.CorrosionSensor1.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Датчик коррозии №2: {0}", this.CorrosionSensor2.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Датчик коррозии №3: {0}", this.CorrosionSensor3.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Защитный потенциал, В: {0}", this.ProtectivePotential.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Ток катодной защиты, А: {0}", this.ProtectiveСurrent.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Наведённое напряжение, В: {0}", this.InducedVoltage.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Напряжение батареи, В: {0}", this.BattaryVoltage.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Напряжение питания НГК-БИ, В: {0}", this.SupplyVoltage.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Поляризационный потенциал, В: {0}", this.PolarizationPotential.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Ток поляризации, мА: {0}", this.PolarizationСurrent.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Ток измерительного канала №1 (4-20), мА: {0}", this.CurrentChannel1.ToString()));
            sb.Append(Environment.NewLine);
            sb.Append(String.Format("Ток измерительного канала №2 (4-20), мА: {0}", this.CurrentChannel2.ToString()));
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Генерирует событие после изменения свойства 
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
        /// Генерирует событие перед изменением свойства 
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
        /// Сериализует объект БИ в файл
        /// </summary>
        /// <param name="path">Путь + имя файла, 
        /// в который необходимо сохранить объект БИ</param>
        /// <param name="device">Объект БИ</param>
        /// <returns>Результат выполнения операции, 
        /// если false - не удалость сериализовать</returns>
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
                // Не удалось сериализовать объект
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
        /// Сериализует объект БИ в файл
        /// </summary>
        /// <param name="stream">Поток в котором открыт файл для сериализации, должен быть открыт для записи</param>
        /// <param name="device">Объект БИ</param>
        /// <returns>Результат выполнения операции, 
        /// если false - не удалость сериализовать</returns>
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
                        // Не удалось сериализовать объект
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
        /// Дессериализует файл в объект устройства БИ
        /// </summary>
        /// <param name="path">Путь содержажий имя файла для дессериализации</param>
        /// <returns>Дессериализованный объект, или null - если имела место ошибка</returns>
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
        /// Дессериализует файл в объект устройства БИ
        /// </summary>
        /// <param name="stream">Поток в котором открыт 
        /// файл из которого нужно десериализовать</param>
        /// <returns>Дессериализованный объект, 
        /// или null - если имела место ошибка</returns>
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
                // Поработали с потоком, установили его внутренний указатель на начало
                // иначе, потом будет плохо.
                stream.Seek(0, SeekOrigin.Begin);
            }
            return device;
        }
        //------------------------------------------------------------------------------------------------------
        #region NetworksCommands
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Проверяет инициализировано устройство или нет. 
        /// </summary>
        /// <param name="host">Master-устройство</param>
        /// <param name="init">TRUE-устройство иницилизировано</param>
        /// <param name="error">Результат выполнения операции</param>
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
                        // Регистры доступны и значит устройство не инициализировано
                        init = false;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // Регистры не доступны значит устройство инициализировано
                        init = true;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                default:
                    {
                        init = false;
                        String msg = String.Format(
                            "Ошибка выполнения чтения устройства. Устройство венуло ошибку: {0}", 
                            result.Description);
                        error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                        break;
                    }
            }

        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Читаем серийный номер устройства. Если данный регистр не доступен, 
        /// то устройство инициализировано.
        /// В противном случае, необходимо записать серийный номер.
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
                                "Устройство вернуло неверные данные. Количество регистров {0}, а должно быть 3", 
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
            // Записываем новое значение в устройство
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
                // При операции записи произошла ошибка
                error = new OperationResult(OPERATION_RESULT.FAILURE, 
                    result.Description);
            }
            else
            {
                // После записи данный регистр становиться не доступен
                // Проверяем записанные данные
                this.Read_HR_SerialNumber(ref host,
                    out sn, out error);
                
                switch (error.Result)
                {
                    case OPERATION_RESULT.IllegalDataAddress:
                        {
                        //    // Чтение выполено успешно, проверяем данные
                        //    if (sn == serialNumber)
                        //    {
                        //        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        //    }
                        //    else
                        //    {
                        //        msg = String.Format(
                        //            "Ошибка записи серийного номера устройства. Не совпал записанное {0} и прочитанное значение {1}",
                        //            serialNumber, sn);
                        //        error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                        //    }

                            error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                            break;
                        }
                    case OPERATION_RESULT.OK:
                        {
                            // При выполнении операции чтения произошла ошибка
                            error = new OperationResult(OPERATION_RESULT.FAILURE, 
                                "Ошибка: После инициализации устройсвта регистр хранения остался доступен");
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
            // Записываем новое значение в устройство
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
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
                        "Ответ НГК-БИ содержит количесво прочитанных регистров {0}, должно быть 2",
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
                        "Ответ НГК-БИ содержит количесво прочитанных регистров {0}, должно быть 2",
                        registers.Length);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    // Проверяем записанное и прочитанное
                    result = host.ReadHoldingRegisters(_AddressSlave,
                        BI_ADDRESSES_OF_HOLDINGREGISTERS.MeasuringPeriod,
                        2, out registers);

                    if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
                    {
                        if (registers.Length != 2)
                        {
                            msg = String.Format(
                                "Ответ НГК-БИ содержит количесво прочитанных регистров {0}, должно быть 2",
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
                                    "Значение записанного прараметра {0} не совподает с прочитанным {1}",
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
                        "Ответ НГК-БИ содержит количесво прочитанных регистров {0}, должен быть 1",
                        registers.Length);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    // Проверяем записанное и прочитанное
                    result = host.ReadHoldingRegisters(_AddressSlave,
                        BI_ADDRESSES_OF_HOLDINGREGISTERS.MeasuringSupplyVoltagePeriod,
                        1, out registers);

                    if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
                    {
                        if (registers.Length != 1)
                        {
                            msg = String.Format(
                                "Ответ НГК-БИ содержит количесво прочитанных регистров {0}, должен быть 1",
                                registers.Length);
                            error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                        }
                        else
                        {
                            period = registers[0];

                            if (this.MeasuringVoltagePeriod != period)
                            {
                                msg = String.Format(
                                    "Значение записанного прараметра {0} не совподает с прочитанным {1}",
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
            // Записываем новое значение в устройство
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
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
            // Записываем новое значение в устройство
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
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
            // Записываем новое значение в устройство
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
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
            // Записываем новое значение в устройство
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
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
            // Записываем новое значение в устройство
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
                // Читаем, то что записали и сверяем
                if ((UInt16)this.BaudRateCAN != value)
                {
                    msg = String.Format(
                        "Значение записанного прараметра {0} не совподает с прочитанным {1}",
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
            // Записываем новое значение в устройство
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
                // Читаем записаное и сверяем результаты
                if ((UInt16)this.CurrentShuntValue != value)
                {
                    msg = String.Format(
                        "Значение записанного прараметра {0} не совподает с прочитанным {1}",
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
            // Записываем новое значение в устройство
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
            // Проверку записанное осуществить невозможно, по причине хода часов!!!
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
                        "Ответ НГК-БИ содержит количесво прочитанных регистров {0}, должно быть 2",
                        registers.Length);
                    error = new OperationResult(OPERATION_RESULT.INCORRECT_ANSWER, msg);
                }
                else
                {
                    // Читать время необходимо из входного регистра. Через регистр
                    // хранения время только записывается в устройство БИ

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
                // Проверяем записанные данные
                if (this.PolarizationPotentialEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
                // Проверяем записанные данные
                if (this.ProtectivePotentialEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
        public void Read_CL_ProtectiveСurrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.ProtectiveСurrentEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.ProtectiveСurrentEn =
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
        public void Write_CL_ProtectiveСurrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.ProtectiveСurrentEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.ProtectiveСurrentEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // Проверяем записанные данные
                if (this.ProtectiveСurrentEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
                        this.ProtectiveСurrentEn, coil);
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
        public void Read_CL_PolarizationСurrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            Modbus.State[] coils;

            result = host.ReadCoils(_AddressSlave,
                BI_ADDRESSES_OF_COILS.PolarizationСurrentEn, 1, out coils);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                this.PolarizationСurrentEn =
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
        public void Write_CL_PolarizationСurrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error)
        {
            Modbus.OSIModel.Message.Result result;
            String msg;
            Modbus.State coil;

            coil = Modbus.Convert.ToState(this.PolarizationСurrentEn);

            result = host.WriteSingleCoil(_AddressSlave,
                BI_ADDRESSES_OF_COILS.PolarizationСurrentEn, ref coil);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                // Проверяем записанные данные
                if (this.PolarizationСurrentEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
                        this.PolarizationСurrentEn, coil);
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
                // Проверяем записанные данные
                if (this.InducedVoltageEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
                // Проверяем записанные данные
                if (this.ExtendedSumPotentialEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
                // Проверяем записанные данные
                if (this.SendStatusWordEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
                // Проверяем записанные данные
                if (this.DcCurrentRefereceElectrodeEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
                // Проверяем записанные данные
                if (this.AcCurrentRefereceElectrodeEn !=
                    Modbus.Convert.ToBoolean(coil))
                {
                    msg = String.Format(
                        "Параметр записан не верно: должно быть {0}, устройство вернуло {1}",
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
                        "Устройство вернуло код неизвестного типа устройства: {0}", registers[0].ToString()));
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
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
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
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
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
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
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
            //    this.ProtectiveСurrent =
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
                        this.ProtectiveСurrent = (float)(registers[0] * 0.05);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
                        this.ProtectiveСurrent = 0;
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
            //    this.PolarizationСurrent =
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
                        this.PolarizationСurrent = (float)(ToValue(registers[0]) * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
                        this.PolarizationСurrent = 0;
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
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
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
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
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
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
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
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
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
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
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
            //throw new InvalidOperationException("Операция неподдреживается в БИ(У)-00");
            error = new OperationResult(OPERATION_RESULT.INVALID_OPERATION,
                "Операция неподдреживается в НГК-БИ(У)-00");

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
            //    this._ReferenceElectrodeDcСurrent =
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
                        this._ReferenceElectrodeDcСurrent =
                            (float)(ToValue(registers[0]) * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
                        this._ReferenceElectrodeDcСurrent = 0;
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
            //    this._ReferenceElectrodeAcСurrent =
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
                        this._ReferenceElectrodeAcСurrent = (float)((registers[0]) * 0.01);
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        break;
                    }
                case Modbus.OSIModel.ApplicationLayer.Error.IllegalDataAddress:
                    {
                        // Если возвращается исключение 0x2 (регистр не доступен для чтения)
                        // это является нормальной ситуацией.
                        // См. протокол: При чтении Input Register, определённого в Coil как 
                        // неактивный канал измерения возвращать исключение 0х02.
                        this._ReferenceElectrodeAcСurrent = 0;
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
        /// Переобразовывает число представленное в дополнительном коде
        /// в знаковое целое число.
        /// </summary>
        /// <param name="valueTwosComplement">
        /// Число представленное в дополнительном коде</param>
        /// <returns>Знаковое целое число</returns>
        private int ToValue(UInt16 valueTwosComplement)
        {
            int value = 0;

            if ((valueTwosComplement & 0x8000) == 0x8000)
            {
                // Число отрицательное (старший разрея установлен в 1)
                unchecked
                {
                    // Преобразовываем число в прямой код
                    valueTwosComplement = (UInt16)(~valueTwosComplement);
                    valueTwosComplement++;
                    value = Convert.ToInt32(valueTwosComplement) * -1;
                    return value;
                }
            }
            else
            {
                // Число положетельное
                unchecked
                {
                    value = (Int16)valueTwosComplement;
                }
            }
            return value;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Преобразует знаковое целое число в дополнительный код
        /// </summary>
        /// <param name="value">Знаковое целое число</param>
        /// <returns>Дополнительный код</returns>
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
        #region Члены INotifyPropertyChanged
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит после изменения свойства
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Events
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит перед изменением свойства
        /// </summary>
        //public event NGK.Devices.PropertyChangingEventHandler PropertyChanging;
        //------------------------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Члены IMeasuringDevice
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
        #region Члены IDeserializationCallback
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Вызывается каждый раз после дессериализации объекта, и необходим в будующем для расширения
        /// полей и свойств класса, для поддержки совместимости. Здесь производиться инициализация
        /// расширенных полей недостающих в файлах старых версий
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