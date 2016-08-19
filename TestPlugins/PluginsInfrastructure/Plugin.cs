using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Plugin;
using Mvp.WinApplication;
using System.Collections.ObjectModel;
using Mvp.WinApplication.Infrastructure;

namespace PluginsInfrastructure
{
    public abstract class Plugin: PluginBase
    {
        #region Constructors

        public Plugin()
        {
            _ApplicationServices = new List<IApplicationService>();
            _Menu = new List<Menu>();
        }

        #endregion

        #region Fields And Properties

        protected readonly List<IApplicationService> _ApplicationServices;
        protected readonly List<Menu> _Menu;

        public ReadOnlyCollection<IApplicationService> ApplicationServices 
        { 
            get { return _ApplicationServices.AsReadOnly(); } 
        }

        public ReadOnlyCollection<Menu> Menu
        {
            get { return _Menu.AsReadOnly(); }
        }

        #endregion
    }
}
