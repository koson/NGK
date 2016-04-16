using System;
using System.Drawing;
using System.IO.Ports;

namespace NGK.CorrosionMonitoringSystem.Managers.AppConfigManager
{
    public interface IConfigManager
    {
        Color CommunicationErrorRowColor { get; }
        Color ConfigurationErrorRowColor { get; }
        bool CursorEnable { get; }
        bool FormBorderEnable { get; }
        ushort[] HiddenIndexesKip9810 { get; }
        ushort[] HiddenIndexesKip9811 { get; }
        bool IsDebug { get; }
        int LocationNameColumnWidth { get; set; }
        int NetworkNameColumnWidth { get; set; }
        int ObjectDictionaryDisplayedNameColumnWidth { get; set; }
        int ObjectDictionaryIndexValueColumnWidth { get; set; }
        int ObjectDictionaryLastUpdateTimeColumnWidth { get; set; }
        Color OperationalModeRowColor { get; }
        int PivotTableCorrosion_depth_200FColumnWidth { get; set; }
        int PivotTableCorrosion_speed_2010ColumnWidth { get; set; }
        int PivotTableLocationColumnWidth { get; set; }
        int PivotTablePolarisationCurrent_200СColumnWidth { get; set; }
        int PivotTablePolarisationPotential_2008ColumnWidth { get; set; }
        int PivotTableProtectionCurrent_200BColumnWidth { get; set; }
        int PivotTableProtectionPotential_2009ColumnWidth { get; set; }
        Color PreoperationalModeRowColor { get; }
        bool ShowInTaskbar { get; }
        int StatusColumnWidth { get; set; }
        Color StoppedModeRowColor { get; }
        string ModbusSystemInfoNetworkName { get; } 
        string SerialPortName { get; set; }
        int SerialPortBaudRate { get; set; }
        Parity SerialPortParity { get; set; }
        int SerialPortDataBits { get; set; }
        StopBits SerialPortStopBits { get; set; }
    }
}
