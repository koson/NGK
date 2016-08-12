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
    public interface ISplashScreenView : IView
    {
        void WriteLine(string line);
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

        public ViewType ViewType
        {
            get { return ViewType.Window; }
        }

        public IViewRegion[] ViewRegions
        {
            get { return new IViewRegion[0]; }
        }

        #endregion

        #region Methods

        public void WriteLine(string line)
        {

            //if (_LabelOutput.InvokeRequired)
            //{ }
            //_LabelOutput.Invoke(delegate() { _LabelOutput.Text = line; });
            //else
                _LabelOutput.Text = line;
        }

        #endregion
    }
}