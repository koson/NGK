using System;
using System.Collections.Generic;
using System.Text;

namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    /// <summary>
    /// Категории ошибок работы сети
    /// </summary>
    public enum ErrorCategory
    {
        /// <summary>
        /// Категория ошибки не определена
        /// </summary>
        Unknown,
        /// <summary>
        /// Ошибка возникла в работе объекта соединения
        /// </summary>
        DataLinkLayerError,
        /// <summary>
        /// Ошибка возникла в рабоет контроллера сети
        /// </summary>
        ControllerError,
        /// <summary>
        /// Ошибка возникла в работе устройства
        /// </summary>
        DeviceError
    }

    /// <summary>
    /// Класс для создания агрументов события NetworkErrorOccurredEventHandler
    /// </summary>
    public class NetworkErrorEventArgs: EventArgs
    {
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
            get { return _Category; }
            set { _Category = value; }
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
            get { return _ErrorDescription; }
            set { _ErrorDescription = value; }
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
            get { return _InnerException; }
            set { _InnerException = value; }
        }
        //---------------------------------------------------------------------------
        #endregion
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public NetworkErrorEventArgs()
        {
            _Category = ErrorCategory.Unknown;
            _ErrorDescription = String.Empty;
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
            _Category = Category;
            
            if (description == null)
            {
                _ErrorDescription = String.Empty;
            }

            _ErrorDescription = description;
            _InnerException = innerException;
        }
        //---------------------------------------------------------------------------
        #endregion
    }
    /// <summary>
    /// Делегат для создания события возникновения ошибки в работе сети
    /// </summary>
    /// <param name="sender">Отправитель события</param>
    /// <param name="args">Аргументы события</param>
    public delegate void NetworkErrorOccurredEventHandler(Object sender, 
        NetworkErrorEventArgs args);
}
