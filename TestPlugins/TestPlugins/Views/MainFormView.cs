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
    public partial class MainFormView : Form, IFormView
    {
        #region Constructors

        public MainFormView()
        {
            InitializeComponent();

            _Regions = new RegionContainersCollection();
            _Regions.Add(new RegionContainer<SplitterPanel>("WorkingRegion", _SplitContainer.Panel1));
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

        #endregion

        #region Event Handlers

        private void EventHandler_MainFormView_Load(object sender, EventArgs e)
        {
            IRegionContainer container = Regions["WorkingRegion"];

            foreach (Plugin plugin in Program.AppPluginsService.Plugins)
            {
                foreach (IPartialViewPresenter presenter in plugin.PartialPresenters)
                {
                    presenter.Hide();
                    container.Add(presenter.View);
                }
            }
        }

        #endregion
    }
}