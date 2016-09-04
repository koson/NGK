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

        public StatusStrip StatusBar
        {
            get { return Form._StatusStripMain; }
        }

        public Button ButtonF2
        {
            get { return Form._ButtonF2; }
        }

        public Button ButtonF3
        {
            get { return Form._ButtonF3; }
            set { Form._ButtonF3 = value; }
        }

        public Button ButtonF4
        {
            get { return Form._ButtonF4; }
            set { Form._ButtonF4 = value; }
        }

        public Button ButtonF5
        {
            get { return Form._ButtonF5; }
            set { Form._ButtonF5 = value; }
        }

        public Button ButtonF6
        {
            get { return Form._ButtonF6; }
        }

        public bool FunctionalButtonsPanelVisible
        {
            get { return Form._PanelSystemButtonsRegion.Visible; }
            set { Form._PanelSystemButtonsRegion.Visible = value; }
        }

        #endregion

        #region Methods

        public void ResetFunctionalButtons()
        {
            if (ButtonF3 != null)
            {
                ButtonF3.Dispose();
                ButtonF3 = null;
            }

            if (ButtonF4 != null)
            {
                ButtonF4.Dispose();
                ButtonF4 = null;
            }

            if (ButtonF5 != null)
            {
                ButtonF5.Dispose();
                ButtonF5 = null;
            }
        }

        #endregion
    }
}
