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
        /// ����� ������ ������ �anExecute() 
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// �� �������
            /// </summary>
            [Description("�� �������")]
            ByTimer,
            /// <summary>
            /// ������ �����
            /// </summary>
            [Description("� ������ ������")]           
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
        /// ����� ������ ������ CanExecute
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
                        "������� ���������� ������ ������� ������ 1");
                }
            }
        }

        #endregion

    }
}
