using System;
using System.Collections.Generic;
using System.Text;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort
{
    //====================================================================================
    /// <summary>
    /// Определяет событие по изменению состояния CAN-порта
    /// </summary>
    /// <param name="sender">CAN-порт, отправитель данного события</param>
    /// <param name="args">Аргументы события</param>
    public delegate void EventHandlerPortChangesStatus(
        Object sender, 
        EventArgsPortChangesStatus args);
    //====================================================================================
    /// <summary>
    /// Класс для передачи аргументов события EventHandlerPortChangesStatus
    /// </summary>
    public class EventArgsPortChangesStatus: EventArgs
    {
        //--------------------------------------------------------------------------------
        #region Fields And Properties 
        //--------------------------------------------------------------------------------
        private CanPortStatus _status;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Текущий статус CAN-порта
        /// </summary>
        public CanPortStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Constructors
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public EventArgsPortChangesStatus()
        {
            this.Status = CanPortStatus.Unknown;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="newStatus">Новое состояние CAN-порта</param>
        public EventArgsPortChangesStatus(CanPortStatus newStatus)
        {
            this.Status = newStatus;
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File