using System;
using System.Collections.Generic;
using System.Text;
using PluginsInfrastructure;
using PluginA.Views;

namespace PluginA.Presenters
{
    public class TestPartialVIewPresenter: PartialViewPresenter<TestPartialView>
    {
        public override string Title
        {
            get { return "Тестовое частичное представление"; }
        }

        public override void Close()
        {
            View.Close();
        }
    }
}
