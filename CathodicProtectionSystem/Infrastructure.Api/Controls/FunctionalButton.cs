using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Input;

namespace Infrastructure.Api.Controls
{
    public class FunctionalButton: Button
    {
        #region Constructors

        public FunctionalButton(ICommand command, Keys key)
        {
            _Command = command;
            Key = key;

            DataBindings.Add(new Binding("Enabled", Command, "Status"));
        }

        #endregion

        #region Fields And Properties

        private ICommand _Command;
        private Keys _Key;
        
        public ICommand Command
        {
            get { return _Command; }
        }

        public Keys Key
        {
            get { return _Key; }
            private set
            {
                switch (value)
                {
                    case Keys.F2:
                    case Keys.F3:
                    case Keys.F4:
                    case Keys.F5:
                    case Keys.F6: { _Key = value; break; }
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        #endregion

        #region Methods

        protected override void OnClick(EventArgs e)
        {
            if (Command != null)
                Command.Execute();

            base.OnClick(e);
        }

        #endregion
    }
}
