using System;
using System.Collections.Generic;
using System.Text;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    //===============================================================================
    /// <summary>
    /// Категории ошибок работы сети
    /// </summary>
    public enum ErrorCategory
    {
        //---------------------------------------------------------------------------
        /// <summary>
        /// Категория ошибки не определена
        /// </summary>
        Unknown,
        //---------------------------------------------------------------------------
        /// <summary>
        /// Ошибка возникла в работе объекта соединения
        /// </summary>
        DataLinkLayerError,
        //---------------------------------------------------------------------------
        /// <summary>
        /// Ошибка возникла в рабоет контроллера сети
        /// </summary>
        ControllerError,
        //---------------------------------------------------------------------------
        /// <summary>
        /// Ошибка возникла в работе устройства
        /// </summary>
        DeviceError
        //---------------------------------------------------------------------------
    }
    //===============================================================================
    /// <summary>
    /// Класс для создания агрументов события NetworkErrorOccurredEventHandler
    /// </summary>
    public class NetworkErrorEventArgs: EventArgs
    {
        //---------------------------------------------------------------------------
        #region Fields And Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Категория ошибки
        /// </summary>
        private ErrorCategory _Category;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Категория ошибки
        /// </summary>
        public ErrorCategory Category
        {
            get { return this._Category; }
            set { this._Category = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Описание ошибки
        /// </summary>
        private String _ErrorDescription;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public String ErrorDescription
        {
            get { return this._ErrorDescription; }
            set { this._ErrorDescription = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Внутреннее исключение 
        /// </summary>
        private Exception _InnerException;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Внутреннее исключение повлекшее возникновение ошибки
        /// </summary>
        public Exception InnerException
        {
            get { return this._InnerException; }
            set { this._InnerException = value; }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public NetworkErrorEventArgs()
        {
            this._Category = ErrorCategory.Unknown;
            this._ErrorDescription = String.Empty;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="category">Категория ошибки</param>
        /// <param name="description">Описание ошибки</param>
        /// <param name="innerException">Исключение при возникновении ошибки</param>
        public NetworkErrorEventArgs(ErrorCategory category, String description, 
            Exception innerException)
        {
            this._Category = Category;
            
            if (description == null)
            {
                this._ErrorDescription = String.Empty;
            }

            this._ErrorDescription = description;
            this._InnerException = innerException;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
    /// <summary>
    /// Делегат для создания события возникновения ошибки в работе сети
    /// </summary>
    /// <param name="sender">Отправитель события</param>
    /// <param name="args">Аргументы события</param>
    public delegate void NetworkErrorOccurredEventHandler(Object sender, 
        NetworkErrorEventArgs args);
    //===============================================================================
}
//===================================================================================
// End of file