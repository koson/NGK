using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.View.Collections.ObjectModel;

namespace TestPlugins.Views
{
    public partial class SplashScreenFrom : Form
    {
        #region Constructors

        public SplashScreenFrom()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields And Properties
        #endregion

        #region Methods

        public void Output(string line)
        {

            if (_LabelOutput.InvokeRequired)
                _LabelOutput.Invoke(new MethodInvoker(delegate() { _LabelOutput.Text = line; }));
            else
                _LabelOutput.Text = line;
        }

        #endregion
    }
}