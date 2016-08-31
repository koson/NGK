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
using Mvp.WinApplication.ApplicationService;
using Mvp.Input;
using System.Windows.Forms;
using System.Drawing;

namespace TestPlugins.Presenters
{
    public class MainFormPresenter: WindowPresenter<MainWindowView>
    {
        #region Constructors

        public MainFormPresenter() 
        {
            _ShowNavigationMenuCommand = new Command(OnShowNavigationMenu, CanShowNavigationMenu);

            //Menu = new BindingList<NavigationMenuItem>();
            //Menu.ListChanged += new ListChangedEventHandler(EventHandler_Menu_ListChanged);
            View.Form.Shown += new EventHandler(EventHandler_View_Shown);
            View.Form.Load += new EventHandler(EventHadler_View_Load);
            View.Form.ContextMenuStripChanged += new EventHandler(EventHandler_View_ContextMenuStripChanged);
            View.Form._ButtonMenu.Click += 
                delegate(object sender, EventArgs args) { _ShowNavigationMenuCommand.Execute(); };

            View.Form._ButtonMenu.DataBindings.Add(new Binding("Enabled", _ShowNavigationMenuCommand, "Status"));

            base._Commands.Add(_ShowNavigationMenuCommand);
            base.UpdateStatusCommands();
        }
        
        #endregion

        #region Fields And Properties

        //public readonly BindingList<NavigationMenuItem> Menu;

        #endregion

        #region Methods

        public override void Show()
        {
            View.Show();
        }

        //private void EventHandler_Menu_ListChanged(object sender, ListChangedEventArgs e)
        //{
        //    switch (e.ListChangedType)
        //    {
        //        case ListChangedType.ItemAdded:
        //            {
        //                NavigationMenuItem menu = Menu[e.NewIndex];
        //                View.Menu.Items.Add(NavigationMenuItemConverter.ConvertTo(menu));
        //                break; 
        //            }
        //        case ListChangedType.ItemDeleted:
        //            { break; }
        //    }
        //}

        private void EventHandler_View_Shown(object sender, EventArgs e)
        {
        }

        public void EventHadler_View_Load(object sender, EventArgs e)
        {
            View.Form.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();

            foreach (NavigationMenuItem menu in NavigationService.Menu)
            {
                View.Form.ContextMenuStrip.Items.Add(NavigationMenuItemConverter.ConvertTo(menu));
                //View.Menu.Items.Add(NavigationMenuItemConverter.ConvertTo(menu));
            }
        }

        public void EventHandler_View_ContextMenuStripChanged(object sender, EventArgs e)
        {
            _ShowNavigationMenuCommand.CanExecute();
        }

        #endregion

        #region Commands

        private Command _ShowNavigationMenuCommand;
        private void OnShowNavigationMenu()
        {
            // Отображаем меню в центре формы
            Point point =
                new Point(View.Form.ClientRectangle.Width / 2 - View.Form.ContextMenuStrip.ClientRectangle.Width / 2,
                View.Form.ClientRectangle.Height / 2 - View.Form.ContextMenuStrip.ClientRectangle.Height / 2);

            View.Form.ContextMenuStrip.Show(View.Form, point);
        }
        private bool CanShowNavigationMenu()
        {
            return View.Form.ContextMenuStrip != null;
        }

        #endregion
    }
}
