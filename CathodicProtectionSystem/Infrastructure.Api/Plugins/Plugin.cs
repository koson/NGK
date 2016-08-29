using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Plugin;
using Mvp.WinApplication;
using Mvp.WinApplication.Infrastructure;

namespace Infrastructure.Api.Plugins
{
    public abstract class Plugin: PluginBase
    {
        #region Constructors

        public Plugin()
        {
            _ApplicationServices = new List<IApplicationService>();
            _Menu = new List<NavigationMenuItem>();
            _PartialPresenters = new List<IPartialViewPresenter>();
        }

        #endregion

        #region Fields And Properties

        protected readonly List<IApplicationService> _ApplicationServices;
        protected readonly List<NavigationMenuItem> _Menu;
        protected readonly List<IPartialViewPresenter> _PartialPresenters;

        /// <summary>
        /// —ервисы приложени€ предоставл€емые плагином
        /// </summary>
        public IEnumerable<IApplicationService> ApplicationServices
        {
            get { return _ApplicationServices; }
        }
        /// <summary>
        /// Ќавигационное меню предоставл€емое плагином
        /// </summary>
        public IEnumerable<NavigationMenuItem> Menu
        {
            get { return _Menu; }
        }
        /// <summary>
        ///  оллекци€ частиных видов дл€ главной формы приложени€
        /// </summary>
        public IEnumerable<IPartialViewPresenter> PartialPresenters
        {
            get { return _PartialPresenters; }
        }

        #endregion
    }
}
