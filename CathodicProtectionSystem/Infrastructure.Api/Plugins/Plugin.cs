using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Plugin;
using Mvp.WinApplication;
using Mvp.WinApplication.Infrastructure;
using Infrastructure.API.Managers;

namespace Infrastructure.Api.Plugins
{
    public abstract class Plugin : PluginBase, IPlugin
    {
        #region Constructors

        public Plugin()
        {
            _ApplicationServices = new List<IApplicationService>();
            _PartialPresenters = new List<IPartialViewPresenter>();
        }

        #endregion

        #region Fields And Properties

        private readonly List<IApplicationService> _ApplicationServices;
        private readonly List<IPartialViewPresenter> _PartialPresenters;
        private NavigationMenuItem _NavigationMenu;
        private IManagers _Managers;

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

        public IManagers Managers
        {
            get { return _Managers; }
            set { _Managers = value; }
        }

        IEnumerable<IApplicationService> IPlugin.ApplicationServices
        {
            get { return _ApplicationServices; }
        }

        IEnumerable<IPartialViewPresenter> IPlugin.PartialPresenters
        {
            get { return _PartialPresenters; }
        }

        #endregion

        #region Methods

        public abstract void Initialize(object state);

        #endregion 
    }
}
