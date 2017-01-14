using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer;
using System.ComponentModel;
using Common.Controlling;
using System.IO.Ports;

namespace ModbusPlugin.ApplicationServices.Models
{
    /// <summary>
    /// ������ ��� ����������� ��������� ����
    /// </summary>
    public interface IModbusServiceInfo
    {
        #region Properties

        [ReadOnly(true)]
        [DisplayName("�������� ����")]
        [Category("Modbus")]
        [Description("�������� modbus ����")]
        public string NetworkName { get; }
        [ReadOnly(true)]
        [DisplayName("����� ������")]
        [Category("Modbus")]
        [Description("����� ������ ����������� ���� Modbus")]
        public WorkMode? Mode { get; }
        [ReadOnly(true)]
        [DisplayName("��������� ����")]
        [Category("Modbus")]
        [Description("��������� ����������� ���� Modbus")]
        public Status? Status { get; }

        [ReadOnly(true)]
        [DisplayName("�OM-����")]
        [Category("COM-����")]
        [Description("�������� COM-�����")]
        public string SerialPortName { get; }
        [ReadOnly(true)]
        [DisplayName("�������� ������")]
        [Category("COM-����")]
        [Description("�������� ������ � ���� Modbus")]
        public int? SerialPortBaudRate { get; }
        [ReadOnly(true)]
        [DisplayName("��� ������")]
        [Category("COM-����")]
        [Description("���������� ���-������ � �����")]
        public int? SerialPortDataBits { get; }
        [ReadOnly(true)]
        [DisplayName("�������")]
        [Category("COM-����")]
        [Description("��������� ���� �������� ������ �����")]
        public Parity? SerialPortParity { get; }
        [ReadOnly(true)]
        [DisplayName("����-����")]
        [Category("COM-����")]
        [Description("����-���� �����")]
        public StopBits? SerialPortStopBits { get; }

        #endregion 
    }
}
