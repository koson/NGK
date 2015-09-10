using System;
using Modbus.OSIModel.DataLinkLayer;

//===================================================================================
namespace Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort
{
    //===============================================================================
    /// <summary>
    /// Класс для созданя аргументов события ошибок при приёме и отправке сообщений
    /// </summary>
    public class ErrorOccurredEventArgs: EventArgs
    {
        //---------------------------------------------------------------------------
        private PortError _error;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Ошибка
        /// </summary>
        public PortError Error
        {
            get { return _error; }
            set { _error = value; }
        }
        //---------------------------------------------------------------------------
        private String _description;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        //---------------------------------------------------------------------------
        public ErrorOccurredEventArgs()
        {
            this.Error = PortError.NoError;
            this.Description = String.Empty;
        }
        //---------------------------------------------------------------------------
        public ErrorOccurredEventArgs(PortError error, String description)
        {
            this.Description = description;
            this.Error = error;
        }
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file