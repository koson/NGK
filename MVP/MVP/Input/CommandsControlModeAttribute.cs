using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mvp.Input
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandsControlModeAttribute: Attribute
    {
        /// <summary>
        /// Режим вызова метода СanExecute() 
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// По таймеру
            /// </summary>
            [Description("По таймеру")]
            ByTimer,
            /// <summary>
            /// Ручной режим
            /// </summary>
            [Description("В ручном режиме")]           
            Manual
        }

        #region Constructors

        public CommandsControlModeAttribute()
        {
            _TimersPeriod = 200;
            _UpdatingCanExecuteMode = Mode.Manual;
        }

        #endregion

        #region Fields And Properties

        Double _TimersPeriod;
        Mode _UpdatingCanExecuteMode;

        /// <summary>
        /// Режим вызова метода CanExecute
        /// </summary>
        public Mode UpdatingCanExecuteMode
        {
            get { return _UpdatingCanExecuteMode; }
            set { _UpdatingCanExecuteMode = value; }
        }

        public Double TimersPeriod
        {
            get { return _TimersPeriod; }
            set 
            {
                if (value > 0)
                {
                    _TimersPeriod = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                        "Попытка установить период таймера меньше 1");
                }
            }
        }

        #endregion

    }
}
