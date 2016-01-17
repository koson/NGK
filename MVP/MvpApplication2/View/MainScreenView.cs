using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.Input;
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

        public event EventHandler CheckBoxChanged;

        #endregion

        #region Methods

        void OnCheckBoxChanged()
        {
            if (CheckBoxChanged != null)
            {
                CheckBoxChanged(this, new EventArgs());
            }
        }

        #endregion

        private void EventHandler_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            OnCheckBoxChanged();
        }
    }
}