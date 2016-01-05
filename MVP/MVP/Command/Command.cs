using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mvp
{
    public delegate void CommandAction();
    public delegate bool Condition();

    [Command]
    public class Command: ICommand
    {
        #region Constructors

        public Command(CommandAction onExecute, Condition canExecute)
        {
            _OnExecute = onExecute;
            _OnCanExecute = canExecute;
            _CanExecute = false;
        }

        public Command(CommandAction onExecute)
        {
            _OnExecute = onExecute;
            _OnCanExecute = null;
            _CanExecute = true;
        }

        #endregion

        #region Fields And Properties

        protected CommandAction _OnExecute;
        protected Condition _OnCanExecute;
        protected bool _CanExecute;
        
        public bool CanExecute
        {
            get 
            {
                return _CanExecute;
            }
        }

        #endregion

        #region Methods

        public void Execute()
        {
            _OnExecute();
        }

        protected virtual void CheckCondition()
        {
            if (_OnCanExecute != null)
            {
                if (_CanExecute != _OnCanExecute())
                {
                    _CanExecute = !_CanExecute;
                    OnCanExecuteChanged();
                }
            }
        }

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
