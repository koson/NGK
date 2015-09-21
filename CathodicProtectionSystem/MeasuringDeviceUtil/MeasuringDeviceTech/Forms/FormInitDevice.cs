using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NGK.MeasuringDeviceTech.Classes.MeasuringDevice;

//====================================================================================
namespace NGK.MeasuringDeviceTech.Forms
{
    //================================================================================
    public partial class FormInitDevice : Form
    {
        //----------------------------------------------------------------------------
        private TYPE_NGK_DEVICE _TypeOfDevice;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Тип устройства
        /// </summary>
        public TYPE_NGK_DEVICE TypeOfDevice
        {
            get { return _TypeOfDevice; }
            set { _TypeOfDevice = value; }
        }
        //----------------------------------------------------------------------------
        private UInt64 _SerialNumber;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        public UInt64 SerialNumber
        {
            get { return _SerialNumber; }
        }
        //----------------------------------------------------------------------------
        public FormInitDevice()
        {
            InitializeComponent();
        }
        //----------------------------------------------------------------------------
        public FormInitDevice(TYPE_NGK_DEVICE type)
        {
            InitializeComponent();
            TypeOfDevice = type;
        }
        //----------------------------------------------------------------------------
        private void FormInitDevice_Load(object sender, EventArgs e)
        {
            // Инициализируем элементы формы
            this.textBoxDeviceCode.Text = ((UInt16)this.TypeOfDevice).ToString();

            this.numericUpDownYear.TextAlign = HorizontalAlignment.Right;
            this.numericUpDownYear.Minimum = 0;
            this.numericUpDownYear.Maximum = 99;
            this.numericUpDownYear.Increment = 1;
            this.numericUpDownYear.Value = DateTime.Now.Year % 100;
            this.numericUpDownYear.ValueChanged += new EventHandler(numericUpDownYear_ValueChanged);

            this.numericUpDownSerialNumber.TextAlign = HorizontalAlignment.Right;
            this.numericUpDownSerialNumber.Minimum = 1;
            this.numericUpDownSerialNumber.Maximum = 9999;
            this.numericUpDownSerialNumber.Increment = 1;
            this.numericUpDownSerialNumber.Value = 1;
            this.numericUpDownSerialNumber.ValueChanged += new EventHandler(numericUpDownSerialNumber_ValueChanged);

            // Генерируем серийный номер
            this._SerialNumber = SerialNumberGenerator(this.TypeOfDevice,
                (int)numericUpDownYear.Value,
                (int)numericUpDownSerialNumber.Value);
        }
        //----------------------------------------------------------------------------
        private void numericUpDownSerialNumber_ValueChanged(object sender, EventArgs e)
        {
            // Генерируем серийный номер
            this._SerialNumber = SerialNumberGenerator(this.TypeOfDevice, 
                (int)numericUpDownYear.Value, 
                (int)numericUpDownSerialNumber.Value);
        }
        //----------------------------------------------------------------------------
        void numericUpDownYear_ValueChanged(object sender, EventArgs e)
        {
            // Генерируем серийный номер
            this._SerialNumber = SerialNumberGenerator(this.TypeOfDevice,
                (int)numericUpDownYear.Value,
                (int)numericUpDownSerialNumber.Value);
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Метод генерирует серийный номер устройства на основе вводимых данных
        /// </summary>
        /// <param name="type">Тип устройства NGK</param>
        /// <param name="year">Год выпуска, две последних цыфры</param>
        /// <param name="SerialNumber">Номер по порядку за год</param>
        /// <returns></returns>
        private UInt64 SerialNumberGenerator(
            TYPE_NGK_DEVICE type, 
            int year, 
            int serialNumber)
        {
            // XX.XX.XX.XXXX
            // |  |  |  |_____ Номер по порядку в соответствии с типом и подтипом устройства выпущенных за указанный год
            // |  |  |________ Год выпуска изделия 0..99
            // |  |___________ Под тип устройства NGK
            // |______________ Тип устройства NGK

            if ((year > 99) || (year < 1))
            {
                throw new ArgumentOutOfRangeException("Year", 
                    "Выходит за границы допустимого диапазона 1...99");
            }

            if ((serialNumber > 9999) || (serialNumber < 0))
            {
                throw new ArgumentOutOfRangeException("serialNumber",
                    "Выходит за границы допустимого диапазона 0...9999");
            }

            UInt64 sn = 0;
            sn = sn + (UInt64)serialNumber;
            sn = sn + (((UInt64)year) * 10000);
            sn = sn + (((UInt64)type) * 1000000);

            return sn;
        }
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End Of File