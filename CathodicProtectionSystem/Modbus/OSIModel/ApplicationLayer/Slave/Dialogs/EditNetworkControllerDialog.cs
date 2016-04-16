using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Modbus.OSIModel.ApplicationLayer.Slave.Dialogs
{
    /// <summary>
    /// Класс для создания диалогового окна редактирования сети Modbus
    /// </summary>
    public class EditNetworkControllerDialog
    {
        #region Fields And Properties
        //---------------------------------------------------------------------------
        private ModbusNetworkControllerSlave _Network;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Устанавливает/возвращает редактируемую сеть Modbus
        /// </summary>
        public ModbusNetworkControllerSlave Network
        {
            get { return _Network; }
            set 
            { 
                _Network = value;
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        #region Constructors
        //---------------------------------------------------------------------------
        #endregion
        #region Methods
        //---------------------------------------------------------------------------
        public DialogResult ShowDialog()
        {
            FormEditNetworkController form = new FormEditNetworkController();
            form.Network = _Network;
            return form.ShowDialog();
        }
        //---------------------------------------------------------------------------
        #endregion
    }
}
