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

        #region Настройки основного окна программы
        /// <summary>
        /// Разрешает/запрещает отображение курсора
        /// </summary>
        public static Boolean CursorEnable
        {
            get { return Boolean.Parse(GetParameter("CursorEnable")); }
        }
        /// <summary>
        /// Разрешает/запрещает отображение приложения на панели задач
        /// </summary>
        public static Boolean ShowInTaskbar
        {
            get { return Boolean.Parse(GetParameter("ShowInTaskbar")); }
        }
        /// <summary>
        /// Возвращает значение разрешающее/запрещающее основному 
        /// окну программы иметь рамку (строка заголовака и состояния) и т.п.
        /// </summary>
        public static Boolean FormBorderEnable
        {
            get { return Boolean.Parse(GetParameter("FormBorderEnable")); }
        }

        #endregion

        #region Настройки грида отображения устройств системы
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Ошибка соединения"
        /// </summary>
        [DefaultSettingValueAttribute("255, 128, 0")]
        [Description("Цвет строки неисправного устройсва в списке устройств")]
        public static Color CommunicationErrorRowColor
        {
            get { return ConvertTo(GetParameter("CommunicationErrorRowColor")); }
        }
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Ошибка конфигурации"
        /// </summary>
        //[DefaultSettingValueAttribute("255, 128, 0")]
        [Description("Цвет строки неисправного устройсва в списке устройств")]
        public static Color ConfigurationErrorRowColor
        {
            get { return ConvertTo(GetParameter("ConfigurationErrorRowColor")); }
        }
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Preoperational"
        /// </summary>
        //[DefaultSettingValueAttribute("255, 128, 0")]
        [Description("Цвет строки устройсва в состоянии Preoperational")]
        public static Color PreoperationalModeRowColor
        {
            get { return ConvertTo(GetParameter("PreoperationalModeRowColor")); }
        }
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Operational"
        /// </summary>
        //[DefaultSettingValueAttribute("255, 128, 0")]       
        [Description("Цвет строки устройсва в состоянии Operational")]
        public static Color OperationalModeRowColor
        {
            get { return ConvertTo(GetParameter("OperationalModeRowColor")); }
        }
        /// <summary>
        /// Возвращает цвет стрки устройства в состоянии "Stopped"
        /// </summary>
        //[DefaultSettingValueAttribute("255, 128, 0")]       
        [Description("Цвет строки устройсва в состоянии Stopped")]
        public static Color StoppedModeRowColor
        {
            get { return ConvertTo(GetParameter("StoppedModeRowColor")); }
        }
        #endregion

        #region Mode
        /// <summary>
        /// Возвращает режим работы прорграммы. В режиме "Debug" отображатся
        /// вся информация. В противном случае, только часть.
        /// </summary>
        public static Boolean IsDebug
        {
            get { return Boolean.Parse(GetParameter("IsDebug")); }
        }
        #endregion 

        #region Настроки гридов

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
        public static int ObjectDictionaryLastUpdateTimeColumnWidth
        {
            get { return Int32.Parse(GetParameter("ObjectDictionaryLastUpdateTimeColumnWidth")); }
            set { SetParameter("ObjectDictionaryLastUpdateTimeColumnWidth", value); }
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

        #endregion

        /// <summary>
        /// Возвращает массив номеров индексов объектного словаря устройства КИП9810,
        /// которые должны быть скрыты от пользователя
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
        /// Возвращает массив номеров индексов объектного словаря устройства КИП9811,
        /// которые должны быть скрыты от пользователя
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

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
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
