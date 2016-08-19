using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using TestPlugins.Views;
using Mvp.WinApplication;
using Mvp.WinApplication.Infrastructure;
using System.ComponentModel;
using PluginsInfrastructure;
using Mvp.Plugin;

namespace TestPlugins.Presenters
{
    public class MainFormPresenter: FormPresenter<MainFormView>
    {
        #region Constructors

        public MainFormPresenter(MainFormView view) : base(view) 
        {
            Menu = new BindingList<Menu>();
            Menu.ListChanged += new ListChangedEventHandler(EventHandler_Menu_ListChanged);
            View.Shown += new EventHandler(EventHandler_View_Shown);
            View.Load += new EventHandler(EventHadler_View_Load);
        }
        
        #endregion

        #region Fields And Properties

        public readonly BindingList<Menu> Menu;

        #endregion

        #region Methods

        public override void Show()
        {
            View.Show();
        }

        private void EventHandler_Menu_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        Menu menu = Menu[e.NewIndex];
                        View.Menu.Items.Add(MenuConverter.ConvertTo(menu));
                        break; 
                    }
                case ListChangedType.ItemDeleted:
                    { break; }
            }
        }

        private void EventHandler_View_Shown(object sender, EventArgs e)
        {
        }

        public void EventHadler_View_Load(object sender, EventArgs e)
        {
            foreach (IApplicationService service in Program.Application.AppServices)
            {
                if (service is PluginsService<Plugin>)
                {
                    PluginsService<Plugin> plgService = (PluginsService<Plugin>)service;
                    
                    foreach (Plugin plugin in plgService.Plugins)
                    {
                        foreach (Menu menu in plugin.Menu)
                            //Menu.Add(menu);
                            View.Menu.Items.Add(MenuConverter.ConvertTo(menu));
                    }
                }
            }
        }

        #endregion
    }
}
