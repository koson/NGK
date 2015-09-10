using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.Core
{
    /// <summary>
    /// Класс управляет настройками приложения
    /// </summary>
    public static class Settings
    {
        #region Fields And Properties
        private static Configuration _LocalConfig;
        private static Configuration _RemoteConfig;
        private static Boolean _RemoteConfigurationEnable;

        public static Boolean CursorEnable
        {
            get { return Boolean.Parse(GetParameter("CursorEnable")); }
        }
        public static Boolean ShowInTaskbar
        {
            get { return Boolean.Parse(GetParameter("ShowInTaskbar")); }
        }
        [DefaultSettingValueAttribute("255, 128, 0")]
        [Description("Цвет строки неисправного устройсва в списке устройств")]
        public static Color CommunicationErrorRowColor
        {
            get { return ConvertTo(GetParameter("CommunicationErrorRowColor")); }
        }    
        public static Color ConfigurationErrorRowColor
        {
            get { return ConvertTo(GetParameter("ConfigurationErrorRowColor")); }
        }
        public static Color PreoperationalModeRowColor
        {
            get { return ConvertTo(GetParameter("PreoperationalModeRowColor")); }
        }
        public static Color OperationalModeRowColor
        {
            get { return ConvertTo(GetParameter("OperationalModeRowColor")); }
        }
        public static Color StoppedModeRowColor
        {
            get { return ConvertTo(GetParameter("StoppedModeRowColor")); }
        }
        public static Boolean ObjectDictionaryIndexVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryIndexVisible")); }
        }
        public static Boolean ObjectDictionaryIndexNameVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryIndexNameVisible")); }
        }
        public static Boolean ObjectDictionaryDisplayedNameVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryDisplayedNameVisible")); }
        }
        public static Boolean ObjectDictionaryIndexValueVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryIndexValueVisible")); }
        }
        public static Boolean ObjectDictionaryDataTypeVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryDataTypeVisible")); }
        }
        public static Boolean ObjectDictionaryLastUpdateTimeVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryLastUpdateTimeVisible")); }
        }
        public static Boolean ObjectDictionaryDescriptionVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryDescriptionVisible")); }
        }
        public static Boolean ObjectDictionaryCategoryVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryCategoryVisible")); }
        }
        public static Boolean ObjectDictionaryReadOnlyVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionaryReadOnlyVisible")); }
        }
        public static Boolean ObjectDictionarySdoReadEnableVisible
        {
            get { return Boolean.Parse(GetParameter("ObjectDictionarySdoReadEnableVisible")); }
        }

        public static Boolean DevicesListNodeIdVisible
        {
            get { return Boolean.Parse(GetParameter("NodeIdVisible")); }
        }
        public static Boolean DevicesListStatusVisible
        {
            get { return Boolean.Parse(GetParameter("StatusVisible")); }
        }
        public static Boolean DevicesListNetworkNameVisible
        {
            get { return Boolean.Parse(GetParameter("NetworkNameVisible")); }
        }
        public static Boolean DevicesListLocationNameVisible
        {
            get { return Boolean.Parse(GetParameter("LocationNameVisible")); }
        }
        public static Boolean DevicesListPollingIntervalVisible
        {
            get { return Boolean.Parse(GetParameter("PollingIntervalVisible")); }
        }
        /// <summary>
        /// Возвращает массив номеров индексов объектного словаря устройства КИП9810, которые должны
        /// быть скрыты от пользователя
        /// </summary>
        public static UInt16[] HiddenIndexesKip9810
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
        /// Возвращает массив номеров индексов объектного словаря устройства КИП9811, которые должны
        /// быть скрыты от пользователя
        /// </summary>
        public static UInt16[] HiddenIndexesKip9811
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
        /// <summary>
        /// Возвращает значение разрешающее/запрещающее основному 
        /// окну программы иметь заголовок и т.п.
        /// </summary>
        public static Boolean FormBorderEnable
        {
            get { return Boolean.Parse(GetParameter("FormBorderEnable")); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static int NodeIdColumnWidth
        {
            get { return Int32.Parse(GetParameter("NodeIdColumnWidth")); }
            set { SetParameter("NodeIdColumnWidth", value); }
        }
        public static int StatusColumnWidth
        {
            get { return Int32.Parse(GetParameter("StatusColumnWidth")); }
            set { SetParameter("StatusColumnWidth", value); }
        }
        public static int NetworkNameColumnWidth
        {
            get { return Int32.Parse(GetParameter("NetworkNameColumnWidth")); }
            set { SetParameter("NetworkNameColumnWidth", value); }            
        }
        public static int LocationNameColumnWidth
        {
            get { return Int32.Parse(GetParameter("LocationNameColumnWidth")); }
            set { SetParameter("LocationNameColumnWidth", value); }
        }
        public static int PollingIntervalColumnWidth
        {
            get { return Int32.Parse(GetParameter("PollingIntervalColumnWidth")); }
            set { SetParameter("PollingIntervalColumnWidth", value); }
        }
        public static int ObjectDictionaryIndexColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryIndexColumnWidth")); }
            set { SetParameter("ObjectDictionaryIndexColumnWidth", value); }
        }
        public static int ObjectDictionaryIndexNameColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryIndexNameColumnWidth")); }
            set { SetParameter("ObjectDictionaryIndexNameColumnWidth", value); }
        }
        public static int ObjectDictionaryDisplayedNameColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryDisplayedNameColumnWidth")); }
            set { SetParameter("ObjectDictionaryDisplayedNameColumnWidth", value); }
        }
        public static int ObjectDictionaryIndexValueColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryIndexValueColumnWidth")); }
            set { SetParameter("ObjectDictionaryIndexValueColumnWidth", value); }
        }
        public static int ObjectDictionaryTypeOfValueColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryTypeOfValueColumnWidth")); }
            set { SetParameter("ObjectDictionaryTypeOfValueColumnWidth", value); }
        }
        public static int ObjectDictionaryLastUpdateTimeColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryLastUpdateTimeColumnWidth")); }
            set { SetParameter("ObjectDictionaryLastUpdateTimeColumnWidth", value); }
        }
        public static int ObjectDictionaryDescriptionColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryDescriptionColumnWidth")); }
            set { SetParameter("ObjectDictionaryDescriptionColumnWidth", value); }
        }
        public static int ObjectDictionaryCategoryColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryCategoryColumnWidth")); }
            set { SetParameter("ObjectDictionaryCategoryColumnWidth", value); }
        }
        public static int ObjectDictionaryReadOnlyColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryReadOnlyColumnWidth")); }
            set { SetParameter("ObjectDictionaryReadOnlyColumnWidth", value); }
        }
        public static int ObjectDictionarySdoReadEnableColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionarySdoReadEnableColumnWidth")); }
            set { SetParameter("ObjectDictionarySdoReadEnableColumnWidth", value); }
        }
        public static int PivotTableNodeIdColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableNodeIdColumnWidth")); }
            set { SetParameter("PivotTableNodeIdColumnWidth", value); }
        }
        public static int PivotTableLocationColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableLocationColumnWidth")); }
            set { SetParameter("PivotTableLocationColumnWidth", value); }
        }
        public static int PivotTablePolarisationPotential_2008ColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTablePolarisationPotential_2008ColumnWidth")); }
            set { SetParameter("PivotTablePolarisationPotential_2008ColumnWidth", value); }
        }
        public static int PivotTableProtectionPotential_2009ColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableProtectionPotential_2009ColumnWidth")); }
            set { SetParameter("PivotTableProtectionPotential_2009ColumnWidth", value); }
        }
        public static int PivotTableProtectionCurrent_200BColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableProtectionCurrent_200BColumnWidth")); }
            set { SetParameter("PivotTableProtectionCurrent_200BColumnWidth", value); }
        }
        public static int PivotTablePolarisationCurrent_200СColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTablePolarisationCurrent_200СColumnWidth")); }
            set { SetParameter("PivotTablePolarisationCurrent_200СColumnWidth", value); }
        }
        public static int PivotTableCorrosion_depth_200FColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableCorrosion_depth_200FColumnWidth")); }
            set { SetParameter("PivotTableCorrosion_depth_200FColumnWidth", value); }
        }
        public static int PivotTableCorrosion_speed_2010ColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableCorrosion_speed_2010ColumnWidth")); }
            set { SetParameter("PivotTableCorrosion_speed_2010ColumnWidth", value); }
        }
        public static int PivotTableTamper_2015ColumnWidth
        {
            get { return Int32.Parse(GetParameter("PivotTableTamper_2015ColumnWidth")); }
            set { SetParameter("PivotTableTamper_2015ColumnWidth", value); }
        }
        public static Boolean PivotTableNodeIdColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTableNodeIdColumnVisible")); }
            set { SetParameter("PivotTableNodeIdColumnVisible", value); }
        }
        public static Boolean PivotTableLocationColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTableLocationColumnVisible")); }
            set { SetParameter("PivotTableLocationColumnVisible", value); }
        }
        public static Boolean PivotTablePolarisationPotential_2008ColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTablePolarisationPotential_2008ColumnVisible")); }
            set { SetParameter("PivotTablePolarisationPotential_2008ColumnVisible", value); }
        }
        public static Boolean PivotTableProtectionPotential_2009ColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTableProtectionPotential_2009ColumnVisible")); }
            set { SetParameter("PivotTableProtectionPotential_2009ColumnVisible", value); }
        }
        public static Boolean PivotTableProtectionCurrent_200BColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTableProtectionCurrent_200BColumnVisible")); }
            set { SetParameter("PivotTableProtectionCurrent_200BColumnVisible", value); }
        }
        public static Boolean PivotTablePolarisationCurrent_200СColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTablePolarisationCurrent_200СColumnVisible")); }
            set { SetParameter("PivotTablePolarisationCurrent_200СColumnVisible", value); }
        }
        public static Boolean PivotTableCorrosion_depth_200FColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTableCorrosion_depth_200FColumnVisible")); }
            set { SetParameter("PivotTableCorrosion_depth_200FColumnVisible", value); }
        }
        public static Boolean PivotTableCorrosion_speed_2010ColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTableCorrosion_speed_2010ColumnVisible")); }
            set { SetParameter("PivotTableCorrosion_speed_2010ColumnVisible", value); }
        }
        public static Boolean PivotTableTamper_2015ColumnVisible
        {
            get { return Boolean.Parse(GetParameter("PivotTableTamper_2015ColumnVisible")); }
            set { SetParameter("PivotTableTamper_2015ColumnVisible", value); }
        }
        #endregion
        #region Constructors
        static Settings()
        {
            _LocalConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _RemoteConfigurationEnable = Boolean.Parse(_LocalConfig.AppSettings.Settings["EnableRemoteConfigFile"].Value);

            if (_RemoteConfigurationEnable)
            {
                ExeConfigurationFileMap mapconfig = new ExeConfigurationFileMap();
                mapconfig.ExeConfigFilename = _LocalConfig.AppSettings.Settings["PathToRemoteConfigFile"].Value;
                _RemoteConfig = ConfigurationManager.OpenMappedExeConfiguration(mapconfig,
                    ConfigurationUserLevel.None);
            }
        }
        #endregion
        #region Methods
        private static String GetParameter(String paramName)
        {
            if (_RemoteConfigurationEnable)
            {
                return _RemoteConfig.AppSettings.Settings[paramName].Value;
            }
            else
            {
                return _LocalConfig.AppSettings.Settings[paramName].Value;
            }
        }
        private static void SetParameter(String paramName, ValueType value)
        {
            if (_RemoteConfigurationEnable)
            {
                _RemoteConfig.AppSettings.Settings[paramName].Value = value.ToString();
                _RemoteConfig.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                _LocalConfig.AppSettings.Settings[paramName].Value = value.ToString();
                _LocalConfig.Save(ConfigurationSaveMode.Modified, false);
                ConfigurationManager.RefreshSection("appSettings");
            }
            return;
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
