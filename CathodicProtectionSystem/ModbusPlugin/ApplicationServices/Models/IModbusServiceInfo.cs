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
    /// Данные для отображения состояния сети
    /// </summary>
    public interface IModbusServiceInfo
    {
        #region Properties

        [ReadOnly(true)]
        [DisplayName("Название сети")]
        [Category("Modbus")]
        [Description("Название modbus сети")]
        public string NetworkName { get; }
        [ReadOnly(true)]
        [DisplayName("Режим работы")]
        [Category("Modbus")]
        [Description("Режим работы контроллера сети Modbus")]
        public WorkMode? Mode { get; }
        [ReadOnly(true)]
        [DisplayName("Состояние сети")]
        [Category("Modbus")]
        [Description("Состояние контроллера сети Modbus")]
        public Status? Status { get; }

        [ReadOnly(true)]
        [DisplayName("СOM-порт")]
        [Category("COM-порт")]
        [Description("Название COM-порта")]
        public string SerialPortName { get; }
        [ReadOnly(true)]
        [DisplayName("Скорость обмена")]
        [Category("COM-порт")]
        [Description("Скорость обмена в сети Modbus")]
        public int? SerialPortBaudRate { get; }
        [ReadOnly(true)]
        [DisplayName("Бит данных")]
        [Category("COM-порт")]
        [Description("Количество бит-данных в кадре")]
        public int? SerialPortDataBits { get; }
        [ReadOnly(true)]
        [DisplayName("Паритет")]
        [Category("COM-порт")]
        [Description("Состояние бита паратета данных кадра")]
        public Parity? SerialPortParity { get; }
        [ReadOnly(true)]
        [DisplayName("Стоп-биты")]
        [Category("COM-порт")]
        [Description("Стоп-биты кадра")]
        public StopBits? SerialPortStopBits { get; }

        #endregion 
    }
}
