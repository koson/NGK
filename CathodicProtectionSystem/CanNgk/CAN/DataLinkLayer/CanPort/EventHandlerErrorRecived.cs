using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;

namespace NGK.CAN.DataLinkLayer.CanPort
{
    /// <summary>
    /// Для создания события возникновения ошибки при рабте CAN-порта
    /// </summary>
    /// <param name="sender">Отправитель события</param>
    /// <param name="error">Ошибка</param>
    public delegate void EventHandlerErrorRecived(Object sender, EventArgsLineErrorRecived error);
    /// <summary>
    /// Класс для передачи аргументов события EventHandlerPortChangesStatus
    /// </summary>
    public class EventArgsLineErrorRecived : EventArgs
    {
        #region Constructors
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public EventArgsLineErrorRecived()
        {
            _Error = Message.ERROR.Other;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="error">Ошибка при работе CAN-порта</param>
        public EventArgsLineErrorRecived(ERROR error)
        {
            _Error = error;
        }

        #endregion

        #region Fields And Properties

        /// <summary>
        /// Ошибка при работе CAN-порта
        /// </summary>
        ERROR _Error;

        /// <summary>
        /// Ошибка при работе CAN-порта
        /// </summary>
        public ERROR Error
        {
            get { return _Error; }
            set { _Error = value; }
        }

        #endregion
    }
}
