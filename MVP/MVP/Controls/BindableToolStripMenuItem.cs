using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Mvp.Input;

namespace Mvp.Controls
{
    public class BindableToolStripMenuItem: ToolStripMenuItem, IBindableComponent
    {
        #region Constructors
        #endregion

        #region Fields And Properties

        private ICommand _Action;

        public ICommand Action 
        { 
            get { return _Action; }
            set 
            {
                if (_Action != null)
                    _Action.CanExecuteChanged -= EventHandler_Action_CanExecuteChanged;

                _Action = value;

                if (_Action != null)
                    _Action.CanExecuteChanged += EventHandler_Action_CanExecuteChanged;
            }
        }

        #endregion

        #region IBindableComponent Members

        private BindingContext _BindingContext;
        private ControlBindingsCollection _DataBindings;

        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                if (_BindingContext == null)
                    _BindingContext = new BindingContext();

                return _BindingContext;
            }
            set
            {
                _BindingContext = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBindingsCollection DataBindings
        {
            get 
            {
                if (_DataBindings == null)
                    _DataBindings = new ControlBindingsCollection(this);

                return DataBindings;
            }
        }

        #endregion

        #region Methods

        protected override void  OnClick(EventArgs e)
        {
            if (_Action != null)
                _Action.Execute();

            base.OnClick(e);
        }

        private void EventHandler_Action_CanExecuteChanged(object sender, EventArgs e)
        {
            Enabled = _Action.Status;
        }

        #endregion
    }
}
