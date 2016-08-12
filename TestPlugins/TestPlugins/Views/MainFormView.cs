using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace TestPlugins.Views
{
    public partial class MainFormView : Form, IView
    {
        #region Constructors

        public MainFormView()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields And Properties

        public ViewType ViewType
        {
            get { return ViewType.Window; }
        }

        public IViewRegion[] ViewRegions
        {
            get { return new IViewRegion[0]; }
        }

        #endregion
    }
}