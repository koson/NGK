using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace PluginA.Views
{
    public partial class TestPartialView : UserControl, IPartialView
    {
        public TestPartialView()
        {
            InitializeComponent();
        }

        #region IView Members


        public ViewType ViewType
        {
            get { return ViewType.Region; }
        }

        public void Close()
        {
            Dispose();
        }

        #endregion
    }
}
