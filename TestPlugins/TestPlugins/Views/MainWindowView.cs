using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using PluginsInfrastructure;
using System.Windows.Forms;

namespace TestPlugins.Views
{
    public class MainWindowView: WindowView<MainWindowForm>
    {
        public MainWindowView()
        {
            base.Regions.Add(new RegionContainer<SplitterPanel>("WorkingRegion", Form.WorkingRegionControl));
            base.Form.Load += new EventHandler(EventHandler_Form_Load);
        }

        public void EventHandler_Form_Load(object sender, EventArgs e)
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
    }
}
