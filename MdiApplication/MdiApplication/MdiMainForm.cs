using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MdiApplication
{
    public partial class MdiMainForm : Form
    {
        public MdiMainForm()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ChildOneForm form = new ChildOneForm();
            form.MdiParent = this;
            form.FormBorderStyle = FormBorderStyle.None;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            _Panel.Visible = !_Panel.Visible;
        }
    }
}