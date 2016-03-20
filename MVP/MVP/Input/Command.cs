using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mvp.Input
{
    public delegate void CommandAction();
    public delegate bool Condition();

    public class Command: ICommand
    {
        #region Constructors

        public Command(CommandAction onExecute, Condition canExecute)
        {
            _OnExecute = onExecute;
            _OnCanExecute = canExecute;
        }

        public Command(CommandAction onExecute)
        {
            _OnExecute = onExecute;
            _OnCanExecute = null;
        }

        #endregion

        #region Fields And Properties

        protected CommandAction _OnExecute;
        protected Condition _OnCanExecute;
        protected bool _CommandStatus = true;

        public bool Status { get { return _CommandStatus; } }

        #endregion

        #region Methods

        public void Execute()
        {
            _OnExecute();
        }

        public bool CanExecute()
        {
            if (_OnCanExecute != null)
            {
                if (_CommandStatus != _OnCanExecute())
                {
                    _CommandStatus = !_CommandStatus;
                    OnCanExecuteChanged();
                }
                return _CommandStatus;
            }
            else
            {
                return true;
            }
        }

        //protected virtual void CheckCondition()
        //{
        //    if (_OnCanExecute != null)
        //    {
        //        if (_CanExecute != _OnCanExecute())
        //        {
        //            _CanExecute = !_CanExecute;
        //            OnCanExecuteChanged();
        //        }
        //    }
        //}

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke = 
                        singleCast.Target as ISynchronizeInvoke;

                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                    {
                        syncInvoke.Invoke(singleCast, 
                            new object[] { this, new EventArgs() });
                    }
                    else
                    {
                        singleCast(this, new EventArgs());
                    }
                }
                CanExecuteChanged(this, new EventArgs());
            }
        }

        #endregion

        public event EventHandler CanExecuteChanged;
    }
}