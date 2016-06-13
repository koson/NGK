using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using Mvp.WinApplication;

namespace NGK.CorrosionMonitoringSystem.Managers.AppConfigManager
{
    public class ConfigManager: IConfigManager
    {
        #region Helper
        /// <summary>
        /// Структура наименование ключей параметров конфигурации
        /// </summary>
        public struct ConfigurationHelper
        {
            /// <summary>
            /// Путь к удалённому файлу настроек приложения
            /// </summary>
            public const string RemoteConfig = "PathToRemoteConfigFile";
            /// <summary>
            /// Разрешает/запрещает отображение курсора
            /// </summary>
            public const string CursorEnabled = "CursorEnabled";
            public const string ShowInTaskbar = "ShowInTaskbar";
        }
        
        #endregion

        #region Constructors

        private ConfigManager()
        {
            Configuration localConfig = 
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
            // Читаем путь к файлу удалённой конфигурации:
            // 1 если путь пустой, то читаем настройки из локального
            // 2 если путь не пустой, то читаем найстройки из удалённого
            string pathToFile = localConfig.AppSettings.Settings[
                ConfigManager.ConfigurationHelper.RemoteConfig].Value;

            if (string.IsNullOrEmpty(pathToFile))
            {
                _Config = localConfig;
            }
            else
            {
                ExeConfigurationFileMap mapconfig =
                    new ExeConfigurationFileMap();

                mapconfig.ExeConfigFilename =
                    _Config.AppSettings
                    .Settings["PathToRemoteConfigFile"].Value;
                _Config = ConfigurationManager.OpenMappedExeConfiguration(
                    mapconfig, ConfigurationUserLevel.None);
            }
        }

        #endregion

        #region Fields And Properties

        static object syncRoot = new object();
        static volatile ConfigManager _Instance;
        Configuration _Config;

        public static ConfigManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new ConfigManager();
                        }
                    }
                }
                return _Instance;
            }
        }

        #region Настройки основного окна программы

        /// <summary>
        /// Разрешает/запрещает отображение курсора
        /// </summary>
        public Boolean CursorEnable
        {
            get 
            {
#if DEBUG
                return true;
#endif
                return Boolean.Parse(GetParameter(
                    ConfigManager.ConfigurationHelper.CursorEnabled)); 
            }
        }
        /// <summary>
        /// Разрешает/запрещает отображение приложения на панели задач
        /// </summary>
        public Boolean ShowInTaskbar
        {
            get 
            {
#if DEBUG
                return true;
#endif
                return Boolean.Parse(GetParameter(
                    ConfigManager.ConfigurationHelper.ShowInTaskbar)); 
            }
        }
        /// <summary>
        /// Возвращает значение разрешающее/запрещающее основному 
        /// окну программы иметь рамку (строка заголовака и состояния) и т.п.
        /// </summary>
        public Boolean FormBorderEnable
        {
            get
            {
#if DEBUG
                return true;
#endif
                return Boolean.Parse(GetParameter("FormBorderEnable")); 
            }
        }

        #endregion

        #region Настройки грида отображения устройств системы
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Ошибка соединения"
        /// </summary>
        [DefaultSettingValueAttribute("255, 128, 0")]
        [Description("Цвет строки неисправного устройсва в списке устройств")]
        public Color CommunicationErrorRowColor
        {
            get { return ConvertTo(GetParameter("CommunicationErrorRowColor")); }
        }
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Ошибка конфигурации"
        /// </summary>
        //[DefaultSettingValueAttribute("255, 128, 0")]
        [Description("Цвет строки неисправного устройсва в списке устройств")]
        public Color ConfigurationErrorRowColor
        {
            get { return ConvertTo(GetParameter("ConfigurationErrorRowColor")); }
        }
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Preoperational"
        /// </summary>
        //[DefaultSettingValueAttribute("255, 128, 0")]
        [Description("Цвет строки устройсва в состоянии Preoperational")]
        public Color PreoperationalModeRowColor
        {
            get { return ConvertTo(GetParameter("PreoperationalModeRowColor")); }
        }
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Operational"
        /// </summary>
        //[DefaultSettingValueAttribute("255, 128, 0")]       
        [Description("Цвет строки устройсва в состоянии Operational")]
        public Color OperationalModeRowColor
        {
            get { return ConvertTo(GetParameter("OperationalModeRowColor")); }
        }
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Stopped"
        /// </summary>
        //[DefaultSettingValueAttribute("255, 128, 0")]       
        [Description("Цвет строки устройсва в состоянии Stopped")]
        public Color StoppedModeRowColor
        {
            get { return ConvertTo(GetParameter("StoppedModeRowColor")); }
        }
        #endregion

        #region Mode

        /// <summary>
        /// Возвращает режим работы прорграммы. В режиме "Debug" отображатся
        /// вся информация. В противном случае, только часть.
        /// </summary>
        public Boolean IsDebug
        {
            get { return Boolean.Parse(GetParameter("IsDebug")); }
        }
        
        #endregion 

        #region Настроки гридов

        public int StatusColumnWidth
        {
            get { return Int32.Parse(GetParameter("StatusColumnWidth")); }
            set { SetParameter("StatusColumnWidth", value); }
        }
        public int NetworkNameColumnWidth
        {
            get { return Int32.Parse(GetParameter("NetworkNameColumnWidth")); }
            set { SetParameter("NetworkNameColumnWidth", value); }
        }
        public int LocationNameColumnWidth
        {
            get { return Int32.Parse(GetParameter("LocationNameColumnWidth")); }
            set { SetParameter("LocationNameColumnWidth", value); }
        }
        public int ObjectDictionaryDisplayedNameColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryDisplayedNameColumnWidth")); }
            set { SetParameter("ObjectDictionaryDisplayedNameColumnWidth", value); }
        }
        public int ObjectDictionaryIndexValueColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryIndexValueColumnWidth")); }
            set { SetParameter("ObjectDictionaryIndexValueColumnWidth", value); }
        }
        public int ObjectDictionaryLastUpdateTimeColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryLastUpdateTimeColumnWidth")); }
            set { SetParameter("ObjectDictionaryLastUpdateTimeColumnWidth", value); }
        }
        public int PivotTableLocationColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableLocationColumnWidth")); }
            set { SetParameter("PivotTableLocationColumnWidth", value); }
        }
        public int PivotTablePolarisationPotential_2008ColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTablePolarisationPotential_2008ColumnWidth")); }
            set { SetParameter("PivotTablePolarisationPotential_2008ColumnWidth", value); }
        }
        public int PivotTableProtectionPotential_2009ColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableProtectionPotential_2009ColumnWidth")); }
            set { SetParameter("PivotTableProtectionPotential_2009ColumnWidth", value); }
        }
        public int PivotTableProtectionCurrent_200BColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableProtectionCurrent_200BColumnWidth")); }
            set { SetParameter("PivotTableProtectionCurrent_200BColumnWidth", value); }
        }
        public int PivotTablePolarisationCurrent_200СColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTablePolarisationCurrent_200СColumnWidth")); }
            set { SetParameter("PivotTablePolarisationCurrent_200СColumnWidth", value); }
        }
        public int PivotTableCorrosion_depth_200FColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableCorrosion_depth_200FColumnWidth")); }
            set { SetParameter("PivotTableCorrosion_depth_200FColumnWidth", value); }
        }
        public int PivotTableCorrosion_speed_2010ColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableCorrosion_speed_2010ColumnWidth")); }
            set { SetParameter("PivotTableCorrosion_speed_2010ColumnWidth", value); }
        }

        public bool PivotTableNodeIdColumnVisble
        {
            get { return Boolean.Parse(GetParameter("NodeIdColumnVisble")); }
        }
        public bool PivotTableLocationColumnVisble
        {
            get { return Boolean.Parse(GetParameter("LocationColumnVisble")); }
        }
        public bool PivotTablePolarisationPotentialColumnVisble
        {
            get { return Boolean.Parse(GetParameter("PolarisationPotentialColumnVisble")); }
        }
        public bool PivotTableProtectionPotentialColumnVisble 
        {
            get { return Boolean.Parse(GetParameter("ProtectionPotentialColumnVisble")); }
        }
        public bool PivotTableProtectionCurrentColumnVisble
        {
            get { return Boolean.Parse(GetParameter("ProtectionCurrentColumnVisble")); }
        }
        public bool PivotTablePolarisationCurrentColumnVisble 
        {
            get { return Boolean.Parse(GetParameter("PolarisationCurrentColumnVisble")); }
        }
        public bool PivotTableCorrosionDepthColumnVisble
        {
            get { return Boolean.Parse(GetParameter("CorrosionDepthColumnVisble")); }
        }
        public bool PivotTableCorrosionSpeedColumnVisble
        {
            get { return Boolean.Parse(GetParameter("CorrosionSpeedColumnVisble")); }
        }
        public bool PivotTableTamperColumnVisble
        {
            get { return Boolean.Parse(GetParameter("TamperColumnVisble")); }
        }

        #endregion

        #region Настройки COM-порта для работы Modbus (Slave)
        /// <summary>
        /// Название сети Modbus (Slave) для организации обмена данными
        /// КССМУ с системами верхнего уровня.
        /// </summary>
        public string ModbusSystemInfoNetworkName
        {
            get { return "ModbusSystemInfoNetwork"; }
        }

        public byte ModbusAddress
        {
            get { return Byte.Parse(GetParameter("ModbusAddress")); }
            set { SetParameter("ModbusAddress", value); }
        }

        public string SerialPortName
        {
            get { return GetParameter("PortName"); }
            set { SetParameter("PortName", value); }
        }

        public int SerialPortBaudRate
        {
            get { return Int32.Parse(GetParameter("BaudRate")); }
            set { SetParameter("BaudRate", value); }
        }

        public Parity SerialPortParity
        {
            get { return (Parity)Enum.Parse(typeof(Parity), GetParameter("Parity")); }
            set { SetParameter("Parity", value); }
        }

        public int SerialPortDataBits
        {
            get { return Int32.Parse(GetParameter("DataBits")); }
            set { SetParameter("DataBits", value); }
        }

        public StopBits SerialPortStopBits
        {
            get { return (StopBits)Enum.Parse(typeof(StopBits), GetParameter("StopBits")); }
            set { SetParameter("StopBits", value); }
        }

        #endregion

        /// <summary>
        /// Возвращает массив номеров индексов объектного словаря устройства КИП9810,
        /// которые должны быть скрыты от пользователя
        /// </summary>
        public UInt16[] HiddenIndexesKip9810
        {
            get 
            {
                String[] strArr;
                String str = GetParameter("HideObjectIndexForKip9810");
                if (String.IsNullOrEmpty(str))
                {
                    strArr = new string[0];
                }
                else
                {
                    strArr = str.Split(',');
                }
                UInt16[] array = new ushort[strArr.Length];
                for (int i = 0; i < strArr.Length; i++)
                {
                    array[i] = UInt16.Parse(strArr[i].Trim(), 
                        NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                }
                return array;
            }
        }
        /// <summary>
        /// Возвращает массив номеров индексов объектного словаря устройства КИП9811,
        /// которые должны быть скрыты от пользователя
        /// </summary>
        public UInt16[] HiddenIndexesKip9811
        {
            get
            {
                String[] strArr;
                String str = GetParameter("HideObjectIndexForKip9811");
                if (String.IsNullOrEmpty(str))
                {
                    strArr = new string[0];
                }
                else
                {
                    strArr = str.Split(',');
                }
                UInt16[] array = new ushort[strArr.Length];
                for (int i = 0; i < strArr.Length; i++)
                {
                    array[i] = UInt16.Parse(strArr[i].Trim(),
                        NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
                }
                return array; 
            }
        }

        #endregion

        #region Methods

        String GetParameter(String paramName)
        {
            return _Config.AppSettings.Settings[paramName].Value;
        }

        void SetParameter(String paramName, ValueType value)
        {
            _Config.AppSettings.Settings[paramName].Value = value.ToString();
            _Config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        void SetParameter(String paramName, string value)
        {
            _Config.AppSettings.Settings[paramName].Value = value;
            _Config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        private static Color ConvertTo(String paramName)
        {
            String[] arr = paramName.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = arr[i].Trim();
            }
            if (arr.Length == 1)
            {
                return Color.FromName(arr[0]);
            }
            else if (arr.Length == 3)
            {
                int[] rgb = new int[3];
                for (int i = 0; i < arr.Length; i++)
                {
                    rgb[i] = Int32.Parse(arr[i], System.Globalization.NumberStyles.Integer, 
                        NumberFormatInfo.InvariantInfo);
                }
                return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
            }
            else
            {
                throw new Exception("Некорректный формат значения параметра конфигурации");
            }
        }

        #endregion
    }
}
