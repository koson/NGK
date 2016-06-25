using System;
using System.Drawing;
using System.IO.Ports;

namespace NGK.CorrosionMonitoringSystem.Managers.AppConfigManager
{
    public interface IConfigManager
    {
        bool CursorEnable { get; }
        bool FormBorderEnable { get; }
        bool ShowInTaskbar { get; }
        bool FullScreen { get; }
        bool IsDebug { get; }

        ushort[] HiddenIndexesKip9810 { get; }
        ushort[] HiddenIndexesKip9811 { get; }

        int LocationNameColumnWidth { get; set; }
        int NetworkNameColumnWidth { get; set; }
        int ObjectDictionaryDisplayedNameColumnWidth { get; set; }
        int ObjectDictionaryIndexValueColumnWidth { get; set; }
        int ObjectDictionaryLastUpdateTimeColumnWidth { get; set; }
        int PivotTableCorrosion_depth_200FColumnWidth { get; set; }
        int PivotTableCorrosion_speed_2010ColumnWidth { get; set; }
        int PivotTableLocationColumnWidth { get; set; }
        int PivotTablePolarisationCurrent_200СColumnWidth { get; set; }
        int PivotTablePolarisationPotential_2008ColumnWidth { get; set; }
        int PivotTableProtectionCurrent_200BColumnWidth { get; set; }
        int PivotTableProtectionPotential_2009ColumnWidth { get; set; }

        bool PivotTableNodeIdColumnVisble { get; }
        bool PivotTableLocationColumnVisble { get; }
        bool PivotTablePolarisationPotentialColumnVisble { get; }
        bool PivotTableProtectionPotentialColumnVisble { get; }
        bool PivotTableProtectionCurrentColumnVisble { get; }
        bool PivotTablePolarisationCurrentColumnVisble { get; }
        bool PivotTableCorrosionDepthColumnVisble { get; }
        bool PivotTableCorrosionSpeedColumnVisble { get; }
        bool PivotTableTamperColumnVisble { get; }

        Color CommunicationErrorRowColor { get; }
        Color ConfigurationErrorRowColor { get; }
        Color StoppedModeRowColor { get; }
        Color OperationalModeRowColor { get; }
        Color PreoperationalModeRowColor { get; }

        int StatusColumnWidth { get; set; }
        string ModbusSystemInfoNetworkName { get; } 
        string SerialPortName { get; set; }
        int SerialPortBaudRate { get; set; }
        Parity SerialPortParity { get; set; }
        int SerialPortDataBits { get; set; }
        StopBits SerialPortStopBits { get; set; }
        byte ModbusAddress { get; set; }
    }
}
