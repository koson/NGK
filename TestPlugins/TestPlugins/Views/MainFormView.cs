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

            _Regions = new RegionContainersCollection();
        }

        #endregion

        #region Fields And Properties

        private readonly RegionContainersCollection _Regions;

        public ViewType ViewType
        {
            get { return ViewType.Window; }
        }

        public RegionContainersCollection Regions
        {
            get { return _Regions; }
        }

        public MenuStrip Menu { get { return _MenuStrip; } }

        #endregion

        #region Event Handlers

        private void EventHandler_MainFormView_Load(object sender, EventArgs e)
        {

        }

        #endregion
    }
}