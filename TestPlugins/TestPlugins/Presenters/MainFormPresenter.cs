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
    public class MainFormPresenter: FormPresenter<MainFormView>
    {
        #region Constructors

        public MainFormPresenter(MainFormView view) : base(view) 
        {
            _ShowNavigationMenuCommand = new Command(OnShowNavigationMenu, CanShowNavigationMenu);

            Menu = new BindingList<NavigationMenuItem>();
            Menu.ListChanged += new ListChangedEventHandler(EventHandler_Menu_ListChanged);
            View.Shown += new EventHandler(EventHandler_View_Shown);
            View.Load += new EventHandler(EventHadler_View_Load);
            View.ContextMenuStripChanged += new EventHandler(EventHandler_View_ContextMenuStripChanged);
            View._ButtonMenu.Click += 
                delegate(object sender, EventArgs args) { _ShowNavigationMenuCommand.Execute(); };

            View._ButtonMenu.DataBindings.Add(new Binding("Enabled", _ShowNavigationMenuCommand, "Status"));

            base._Commands.Add(_ShowNavigationMenuCommand);
            base.UpdateStatusCommands();
        }
        
        #endregion

        #region Fields And Properties

        public readonly BindingList<NavigationMenuItem> Menu;

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
                        NavigationMenuItem menu = Menu[e.NewIndex];
                        View.Menu.Items.Add(NavigationMenuItemConverter.ConvertTo(menu));
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
            View.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();

            foreach (NavigationMenuItem menu in NavigationService.Menu)
            {
                View.ContextMenuStrip.Items.Add(NavigationMenuItemConverter.ConvertTo(menu));
                View.Menu.Items.Add(NavigationMenuItemConverter.ConvertTo(menu));
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
                new Point(View.ClientRectangle.Width / 2 - View.ContextMenuStrip.ClientRectangle.Width / 2,
                View.ClientRectangle.Height / 2 - View.ContextMenuStrip.ClientRectangle.Height / 2);

            View.ContextMenuStrip.Show(View, point);
        }
        private bool CanShowNavigationMenu()
        {
            return View.ContextMenuStrip != null;
        }

        #endregion
    }
}
