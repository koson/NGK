using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Presenter
{
    public delegate void Action();

    public class Command: ICommand
    {
        #region Constructors

        public Command(Action OnExecute)
        {
            _OnExecute = OnExecute;
        }

        #endregion

        #region Fields And Properties

        private Action _OnExecute;
        private bool _CanExecute;

        public bool CanExecute
        {
            get { return _CanExecute; }
            set
            {
                if (_CanExecute != value)
                {
                    _CanExecute = value;
                }
            }
        }

        #endregion

        #region Methods

        public void Execute()
        {
            _OnExecute.Invoke();
        }

        private void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        #endregion

        public event EventHandler CanExecuteChanged;
    }
}
