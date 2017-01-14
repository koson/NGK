using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;

namespace NGK.Plugins.Views
{
    public class ModbusServiceSettingsView: PartialView<ModbusServiceSettings>
    {
        #region Fields And Properties

        public DockStyle Dock
        {
            get { return base.Control.Dock; }
            set { base.Control.Dock = value; }
        }

        public string NetworkName
        {
            set { throw new NotImplementedException(); }
        }

        public string Mode
        {
            set { throw new NotImplementedException(); }
        }

        public string Status { set { throw new NotImplementedException(); } }

        #region Serial Port Settings

        public string SerialPortName { set {throw new NotImplementedException(); }} 
        public stirng SerialPortBaudRate { set {throw new NotImplementedException(); }}
        public string SerialPortParity { set {throw new NotImplementedException(); }}
        public string SerialPortDataBits { set {throw new NotImplementedException(); }}
        public stirng SerialPortStopBits { set { throw new NotImplementedException(); } }

        #endregion 

        #endregion
    }
}
