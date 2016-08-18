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
    public partial class MainFormView : Form, IFormView
    {
        #region Constructors

        public MainFormView()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields And Properties

        private RegionContainersCollection _Regions = new RegionContainersCollection();

        public ViewType ViewType
        {
            get { return ViewType.Window; }
        }

        public RegionContainersCollection Regions
        {
            get { return _Regions; }
        }

        #endregion
    }
}