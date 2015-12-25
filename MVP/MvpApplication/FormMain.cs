using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MvpApplication
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread action = new Thread(new ThreadStart(SomeAction));
            action.IsBackground = true;
            action.Start();
        }

        private void SomeAction()
        {
            throw new Exception("Test");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            throw new Exception("Test exception");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Program.ApplicationController.Exit();
        }
    }
}