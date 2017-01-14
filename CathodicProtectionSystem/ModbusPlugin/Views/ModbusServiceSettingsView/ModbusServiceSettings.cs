using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NGK.Plugins.Views
{
    public partial class ModbusServiceSettings : UserControl
    {
        #region Constructors

        public ModbusServiceSettings()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        #endregion

        #region Fields And Properties

        public Object ServiceSettings 
        {
            set 
            {
                _PropertyGridModbusServiceSettings.Object = null;
                _PropertyGridModbusServiceSettings.Object = value; 
            }
        }

        #endregion

        #region Methods

        public void Close()
        {
            Dispose();
        }

        #endregion
    }
}
