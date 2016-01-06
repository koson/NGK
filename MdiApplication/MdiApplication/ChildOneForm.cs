using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MdiApplication
{
    public partial class ChildOneForm : Form
    {
        public ChildOneForm()
        {
            InitializeComponent();
        }

        private void _ButtonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}