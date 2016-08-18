using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestApplication
{
    public partial class SplashScreenForm : Form
    {
        public delegate void SetValue();

        public SplashScreenForm()
        {
            InitializeComponent();
        }

        public string Output
        {
            set
            {
                if (_LabelOutput.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate() { _LabelOutput.Text = value; }), null);
                }
                else
                    _LabelOutput.Text = value;
            }
        }
    }
}