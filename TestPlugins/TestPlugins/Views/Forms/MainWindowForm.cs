using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.View.Collections.ObjectModel;
using PluginsInfrastructure;
using Mvp.WinApplication;

namespace TestPlugins.Views
{
    public partial class MainWindowForm : Form
    {
        #region Constructors

        public MainWindowForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields And Properties

        #region Fields And Properties

        public SplitterPanel WorkingRegionControl { get { return _SplitContainer.Panel1; } }

        #endregion


        #endregion

        #region Event Handlers
        #endregion
    }
}