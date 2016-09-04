using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Plugin;
using Mvp.WinApplication;
using Mvp.WinApplication.Infrastructure;
using Infrastructure.API.Managers;
using System.Windows.Forms;

namespace Infrastructure.Api.Plugins
{
    public abstract class Plugin : PluginBase, IPlugin
    {
        #region Constructors

        public Plugin()
        {
            _ApplicationServices = new List<IApplicationService>();
            _PartialPresenters = new List<IPartialViewPresenter>();
            _StatusBarItems = new List<ToolStripItem>();
        }

        #endregion

        #region Fields And Properties

        private readonly List<IApplicationService> _ApplicationServices;
        private readonly List<IPartialViewPresenter> _PartialPresenters;
        private readonly List<ToolStripItem> _StatusBarItems;
        private NavigationMenuItem _NavigationMenu;
        private IManagers _Managers;
        private IHostWindow _HostWindow;

        /// <summary>
        /// Сервисы приложения предоставляемые плагином
        /// </summary>
        protected IList<IApplicationService> ApplicationServices
        {
            get { return _ApplicationServices; }
        }
        /// <summary>
        /// Корневой элемент навигационного меню предоставляемого плагином
        /// </summary>
        public NavigationMenuItem NavigationMenu
        {
            get { return _NavigationMenu; }
            protected set { _NavigationMenu = value; }
        }
        /// <summary>
        /// Коллекция частиных видов для главной формы приложения
        /// </summary>
        protected IList<IPartialViewPresenter> PartialPresenters
        {
            get { return _PartialPresenters; }
        }

        protected IList<ToolStripItem> StatusBarItems
        {
            get { return _StatusBarItems; }
        }

        public IManagers Managers
        {
            get { return _Managers; }
            protected set { _Managers = value; }
        }

        IEnumerable<IApplicationService> IPlugin.ApplicationServices
        {
            get { return _ApplicationServices; }
        }

        IEnumerable<IPartialViewPresenter> IPlugin.PartialPresenters
        {
            get { return _PartialPresenters; }
        }

        IEnumerable<ToolStripItem> IPlugin.StatusBarItems
        {
            get { return _StatusBarItems; }
        }

        public IHostWindow HostWindow
        {
            get { return _HostWindow; }
            protected set 
            { 
                _HostWindow = value;
                if (value != null)
                    _HostWindow.SelectedPartivalViewPresenterChanged += 
                        new EventHandler(EventHandler_HostWindow_SelectedPartivalViewPresenterChanged);
            }
        }

        #endregion

        #region Methods

        public abstract void Initialize(IHostWindow host, IManagers managers, object state);

        private void EventHandler_HostWindow_SelectedPartivalViewPresenterChanged(object sender, EventArgs e)
        {
            OnHostWindowSelectedPartivalViewPresenterChanged();
        }

        public virtual void OnHostWindowSelectedPartivalViewPresenterChanged(){}

        #endregion         
    }
}
