using System;
using System.Drawing;
using System.IO.Ports;

namespace Infrastructure.Api.Managers
{
    public interface IConfigManager
    {
        string PathToAppDirectory { get; }

        bool CursorEnable { get; }
        bool FormBorderEnable { get; }
        bool ShowInTaskbar { get; }
        bool FullScreen { get; }
        bool IsDebug { get; }

        //ushort[] HiddenIndexesKip9810 { get; }
        //ushort[] HiddenIndexesKip9811 { get; }

        //int LocationNameColumnWidth { get; set; }
        //int NetworkNameColumnWidth { get; set; }
        //int ObjectDictionaryDisplayedNameColumnWidth { get; set; }
        //int ObjectDictionaryIndexValueColumnWidth { get; set; }
        //int ObjectDictionaryLastUpdateTimeColumnWidth { get; set; }
        //int PivotTableCorrosion_depth_200FColumnWidth { get; set; }
        //int PivotTableCorrosion_speed_2010ColumnWidth { get; set; }
        //int PivotTableLocationColumnWidth { get; set; }
        //int PivotTablePolarisationCurrent_200СColumnWidth { get; set; }
        //int PivotTablePolarisationPotential_2008ColumnWidth { get; set; }
        //int PivotTableProtectionCurrent_200BColumnWidth { get; set; }
        //int PivotTableProtectionPotential_2009ColumnWidth { get; set; }

        bool PivotTableNodeIdColumnVisble { get; }
        bool PivotTableLocationColumnVisble { get; }
        bool PivotTablePolarisationPotentialColumnVisble { get; }
        bool PivotTableProtectionPotentialColumnVisble { get; }
        bool PivotTableProtectionCurrentColumnVisble { get; }
        bool PivotTablePolarisationCurrentColumnVisble { get; }
        bool PivotTableCorrosionDepthColumnVisble { get; }
        bool PivotTableCorrosionSpeedColumnVisble { get; }
        bool PivotTableTamperColumnVisble { get; }

        //Color CommunicationErrorRowColor { get; }
        //Color ConfigurationErrorRowColor { get; }
        //Color StoppedModeRowColor { get; }
        //Color OperationalModeRowColor { get; }
        //Color PreoperationalModeRowColor { get; }

        //int StatusColumnWidth { get; set; }
        //string ModbusSystemInfoNetworkName { get; }

        /// <summary>
        /// Адрес Modbus для Modbus-сервиса
        /// </summary>
        byte ModbusAddress { get; set; }
        /// <summary>
        /// Наименование COM-порта для Modbus-сервиса
        /// </summary>
        string SerialPortName { get; set; }
        /// <summary>
        /// Скорость передачи данных для Modbus-сервиса
        /// </summary>
        int SerialPortBaudRate { get; set; }
        /// <summary>
        /// Паритет для Modbus-сервиса
        /// </summary>
        Parity SerialPortParity { get; set; }
        /// <summary>
        /// Кол-во бит данных фрейма для Modbus-сервиса
        /// </summary>
        int SerialPortDataBits { get; set; }
        /// <summary>
        /// Кол-во стоп-бит фрейма для Modbus-сервиса
        /// </summary>
        StopBits SerialPortStopBits { get; set; }
    }
}
