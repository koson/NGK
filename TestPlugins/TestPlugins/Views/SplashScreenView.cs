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
    public interface ISplashScreenView : IFormView
    {
        void Output(string line);
    }

    public partial class SplashScreenView : Form, ISplashScreenView
    {
        #region Constructors

        public SplashScreenView()
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

        #region Methods

        public void Output(string line)
        {

            if (_LabelOutput.InvokeRequired)
                _LabelOutput.Invoke(new MethodInvoker(delegate() { _LabelOutput.Text = line; }));
            else
                _LabelOutput.Text = line;
        }

        #endregion
    }
}