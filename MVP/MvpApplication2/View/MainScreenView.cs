using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp;
using Mvp.View;

namespace MvpApplication2.View
{
    public partial class MainScreenView : Form, IMainScreenView
    {
        public MainScreenView()
        {
            InitializeComponent();
        }

        #region Fields And Properties
        
        public bool CommandIsEnabled 
        {
            get { return _CheckBox.Checked; } 
        }
        
        public bool ButtonEnabled 
        {
            get { return _Button.Enabled; }
            set 
            {
                if (_Button.InvokeRequired)
                {
                    _Button.Invoke(new Action<bool>(
                        delegate(bool flag)
                        {
                            _Button.Enabled = flag;
                        }), value);
                }
                else
                {
                    _Button.Enabled = value;
                }
            } 
        }

        #endregion



        #region IView Members

        void IView.Show()
        {
            this.Show();
        }

        void IView.Close()
        {
            this.Close();
        }

        #endregion

        #region IMainScreenView Members

        ICommand _RunCommand;

        public ICommand RunCommand
        {
            get { return _RunCommand; }
            set 
            { 
                _RunCommand = value;
                if (_RunCommand != null)
                {
                    _RunCommand.CanExecuteChanged += 
                        new EventHandler(_RunCommand_CanExecuteChanged);
                }
            }
        }

        void _RunCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            ICommand cmd = (ICommand)sender;
            _Button.Enabled = cmd.CanExecute;
        }

        #endregion
    }
}