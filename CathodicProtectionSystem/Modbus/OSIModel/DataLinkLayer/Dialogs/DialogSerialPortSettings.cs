using System;
using System.Collections.Generic;
using System.Text;

//===================================================================================
namespace Modbus.OSIModel.DataLinkLayer.Dialogs
{
    //===============================================================================
    public class DialogSerialPortSettings
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
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// Отображает диалог найстройки COM-порта
        /// </summary>
        /// <param name="owner"></param>
        /// <returns>Результат</returns>
        public System.Windows.Forms.DialogResult ShowDialog(System.Windows.Forms.IWin32Window owner)
        {
            FormSerialPortSettings form = new FormSerialPortSettings();
            form.SerialPort = _SerialPort;
            return form.ShowDialog(owner);
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file