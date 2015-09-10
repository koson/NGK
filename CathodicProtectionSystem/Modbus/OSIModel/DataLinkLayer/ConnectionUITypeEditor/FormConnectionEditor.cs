using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort;

//===================================================================================
namespace Modbus.OSIModel.DataLinkLayer.Master.ConnectionUITypeEditor
{
    //===============================================================================
    public partial class FormConnectionEditor : Form
    {
        //---------------------------------------------------------------------------
        private IDataLinkLayer _connection;
        //---------------------------------------------------------------------------
        public IDataLinkLayer Connection
        {
            get { return _connection; }
            set 
            { 
                _connection = value;
                
                ComPort comport = _connection as ComPort;
                
                if (comport != null)
                {
                    this.propertyGridProperties.SelectedObject = comport;
                }


            }
        }
        //---------------------------------------------------------------------------
        public FormConnectionEditor()
        {
            InitializeComponent();

            Init();
        }
        //---------------------------------------------------------------------------
        private void Init()
        {
            if (_connection == null)
            {
                // Блокируем окно свойств
                this.propertyGridProperties.SelectedObject = null;
                this.propertyGridProperties.Enabled = true;

                String[] namesType = Enum.GetNames(typeof(InterfaceType));
                this.comboBoxTypeConnection.DataSource = namesType;
                this.comboBoxTypeConnection.SelectedIndexChanged += 
                    new EventHandler(EventHandler_ComboBoxTypeConnection_SelectedIndexChanged);

                this.comboBoxMode.SelectedIndexChanged += 
                    new EventHandler(comboBoxMode_SelectedIndexChanged);
                namesType = Enum.GetNames(typeof(TransmissionMode));
                this.comboBoxMode.DataSource = namesType;
                int index = this.comboBoxMode.FindString(TransmissionMode.RTU.ToString());
                this.comboBoxMode.SelectedIndex = index;
            }
        }
        //---------------------------------------------------------------------------
        void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            TransmissionMode mode = (TransmissionMode)Enum.Parse(typeof(TransmissionMode),
                cb.SelectedItem.ToString());
            
            switch (mode)
            {
                case TransmissionMode.RTU:
                    {
                        break; 
                    }
                case TransmissionMode.ASCII:
                    { 
                        break; 
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ComboBoxTypeConnection_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            
            InterfaceType type = (InterfaceType)Enum.Parse(typeof(InterfaceType), 
                cb.SelectedItem.ToString());

            this.Connection = CreateConnection(type, 
                (TransmissionMode)Enum.Parse(typeof(TransmissionMode), 
                comboBoxMode.SelectedItem.ToString()));
            
            return;
        }
        //---------------------------------------------------------------------------
        private IDataLinkLayer CreateConnection(InterfaceType type, 
            TransmissionMode mode)
        {
            IDataLinkLayer result;

            switch (type)
            {
                case InterfaceType.SerialPort:
                    {
                        switch (mode)
                        {
                            case TransmissionMode.RTU:
                                {
                                    String[] ports = System.IO.Ports.SerialPort.GetPortNames();

                                    if (ports.Length != 0)
                                    {
                                        RTU.SerialPort.ComPort comport =
                                            new RTU.SerialPort.ComPort(ports[0], 19200,
                                                System.IO.Ports.Parity.Even, 8,
                                                System.IO.Ports.StopBits.One, 1000, 200, false, 
                                                Diagnostics.TypeOfMessageLog.Warning | Diagnostics.TypeOfMessageLog.Information | 
                                                Diagnostics.TypeOfMessageLog.Error, String.Empty);
                                        result = (IDataLinkLayer)comport;
                                    }
                                    else
                                    {
                                        RTU.SerialPort.ComPort comport =
                                            new RTU.SerialPort.ComPort(String.Empty, 19200,
                                                System.IO.Ports.Parity.Even, 8,
                                                System.IO.Ports.StopBits.One, 1000, 200, false,
                                                Diagnostics.TypeOfMessageLog.Warning | Diagnostics.TypeOfMessageLog.Information | 
                                                Diagnostics.TypeOfMessageLog.Error, String.Empty);
                                        result = (IDataLinkLayer)comport; 
                                    }
                                    break;
                                }
                            case TransmissionMode.ASCII:
                                {
                                    result = null;
                                    break;
                                }
                            default:
                                {
                                    throw new NotImplementedException();
                                }
                        }
                        break;
                    }
                case InterfaceType.TCPIP:
                    {
                        result = null;
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
            return result;
        }
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file