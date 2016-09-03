using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using Infrastructure.Api.Plugins;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public class MainWindowView: WindowView<MainWindowForm>
    {
        #region Constructors

        public MainWindowView()
        {
            //Regions.Add(new RegionContainer<Panel>("WorkingRegion", Form.WorkingRegionControl));
        }

        #endregion

        #region Fields And Properties

        public string Title
        {
            get { return Form.Title; }
            set { Form.Title = value; }
        }

        public Panel WorkingRegion
        {
            get { return Form.WorkingRegionControl; }
        }

        #endregion
    }
}
