using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mvp.Input
{
    public delegate void CommandAction();
    public delegate bool Condition();

    public class Command: ICommand, INotifyPropertyChanged
    {
        #region Constructors

        public Command(CommandAction onExecute, Condition canExecute)
        {
            _OnExecute = onExecute;
            _OnCanExecute = canExecute;
            _Name = String.Empty;
        }

        public Command(string name, CommandAction onExecute, Condition canExecute)
        {
            _OnExecute = onExecute;
            _OnCanExecute = canExecute;
            _Name = name == null ? String.Empty : name;
        }

        public Command(CommandAction onExecute)
        {
            _OnExecute = onExecute;
            _OnCanExecute = null;
            _Name = String.Empty;
        }

        public Command(string name, CommandAction onExecute)
        {
            _OnExecute = onExecute;
            _OnCanExecute = null;
            _Name = name == null ? String.Empty : name;
        }

        #endregion

        #region Fields And Properties

        protected CommandAction _OnExecute;
        protected Condition _OnCanExecute;
        protected bool _CommandStatus = true;
        string _Name;

        public string Name
        {
            get { return _Name; }
        }

        public bool Status 
        { 
            get { return _CommandStatus; }
            private set 
            {
                if (_CommandStatus != value)
                {
                    _CommandStatus = value;
                    OnPropertyChanged("Status");
                    OnCanExecuteChanged();
                }
            }
        }

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
                Status = _OnCanExecute();
                return _CommandStatus;
            }
            else
            {
                return true;
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
                //CanExecuteChanged(this, new EventArgs());
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                foreach (PropertyChangedEventHandler singleCast in handler.GetInvocationList())
                {
                    ISynchronizeInvoke syncInvoke =
                        singleCast.Target as ISynchronizeInvoke;

                    if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                    {
                        syncInvoke.Invoke(singleCast,
                            new object[] { this, new PropertyChangedEventArgs(propertyName) });
                    }
                    else
                    {
                        singleCast(this, new PropertyChangedEventArgs(propertyName));
                    }
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
