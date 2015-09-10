using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//===================================================================================
namespace Modbus.OSIModel.DataLinkLayer.Dialogs
{
    //===============================================================================
    public partial class FormSerialPortSettings : Form
    {
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public FormSerialPortSettings()
        {
            InitializeComponent();
            this.Load += new EventHandler(EventHandler_FormSerialPortSettings_Load);
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
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
            this._ComboBoxSerialPort.DataSource = System.IO.Ports.SerialPort.GetPortNames();
            this._ComboBoxSerialPort.DropDownStyle = ComboBoxStyle.DropDownList;

            this._ComboBoxBaudRate.DataSource = new Int32[] { 9600, 19200, 115200 };
            this._ComboBoxBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;

            this._ComboBoxDataBits.DataSource = new Int32[] { 7, 8 };
            this._ComboBoxDataBits.DropDownStyle = ComboBoxStyle.DropDownList;

            this._ComboBoxParity.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            this._ComboBoxParity.DropDownStyle = ComboBoxStyle.DropDownList;

            this._ComboBoxStopBits.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));
            this._ComboBoxStopBits.DropDownStyle = ComboBoxStyle.DropDownList;

            this._BindingSourceSerialPort = new BindingSource(); 


            if (this._SerialPort != null)
            {
                if (this._SerialPort.IsOpen)
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
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file