using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.View.Collections.ObjectModel;

namespace PluginA.Views
{
    public partial class TestFormView : Form, IFormView
    {
        #region Constructors

        public TestFormView()
        {
            InitializeComponent();

            _Regions = new RegionContainersCollection();
        }

        #endregion

        #region Fields And Properties

        private readonly RegionContainersCollection _Regions;

        public RegionContainersCollection Regions
        {
            get { return _Regions; }
        }

        public ViewType ViewType
        {
            get { return ViewType.Window; }
        }

        #endregion
    }
}