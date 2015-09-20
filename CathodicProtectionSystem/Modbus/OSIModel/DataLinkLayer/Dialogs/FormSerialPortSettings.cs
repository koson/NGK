using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Modbus.OSIModel.DataLinkLayer.Dialogs
{
    public partial class FormSerialPortSettings : Form
    {
        #region Fields And Properties
        //---------------------------------------------------------------------------
        private System.IO.Ports.SerialPort _SerialPort;
        //---------------------------------------------------------------------------
        public System.IO.Ports.SerialPort SerialPort
        {
            get { return _SerialPort; }
            set { _SerialPort = value; }
        }
        //---------------------------------------------------------------------------
        private BindingSource _BindingSourceSerialPort;
        //---------------------------------------------------------------------------
        #endregion

        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public FormSerialPortSettings()
        {
            InitializeComponent();
            Load += new EventHandler(EventHandler_FormSerialPortSettings_Load);
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormSerialPortSettings_Load(
            object sender, EventArgs e)
        {
            _ComboBoxSerialPort.DataSource = System.IO.Ports.SerialPort.GetPortNames();
            _ComboBoxSerialPort.DropDownStyle = ComboBoxStyle.DropDownList;

            _ComboBoxBaudRate.DataSource = new Int32[] { 9600, 19200, 115200 };
            _ComboBoxBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;

            _ComboBoxDataBits.DataSource = new Int32[] { 7, 8 };
            _ComboBoxDataBits.DropDownStyle = ComboBoxStyle.DropDownList;

            _ComboBoxParity.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            _ComboBoxParity.DropDownStyle = ComboBoxStyle.DropDownList;

            _ComboBoxStopBits.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));
            _ComboBoxStopBits.DropDownStyle = ComboBoxStyle.DropDownList;

            _BindingSourceSerialPort = new BindingSource(); 


            if (_SerialPort != null)
            {
                if (_SerialPort.IsOpen)
                {
                }
                else
                { 
                }
            }
            else
            { 
            }

            return;
        }
        //---------------------------------------------------------------------------
        #endregion
    }
}
