using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mvp.WinApplication;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Mvp.Plugin
{
    public class PluginsService: ApplicationServiceBase
    {
        public PluginsService(string pathToPluginsDirectory, 
            bool usingOnlyAppDomain) :
            base("PluginsService")
        {
            PathToPluginsDirectory = pathToPluginsDirectory;
            UsingOnlyAppDomain = usingOnlyAppDomain;
            Plugins = new List<IPlugin>().AsReadOnly();
        }

        #region Fields And Properties

        /// <summary>
        /// Путь к папке с плагинами
        /// </summary>
        public readonly string PathToPluginsDirectory;

        /// <summary>
        /// Указывает создавать создавать отдельный
        /// домен для кажного плагина или загружать все
        /// плагины в домен приложения.
        /// </summary>
        public readonly bool UsingOnlyAppDomain;

        private ReadOnlyCollection<IPlugin> _Plugins;
        /// <summary>
        /// Доступные плагины
        /// </summary>
        public ReadOnlyCollection<IPlugin> Plugins
        {
            get { return _Plugins; }
            private set { _Plugins = value; }
        }

        #endregion

        #region Methods

        public override void OnStarting()
        {
            LoadPlugins();
        }

        public void LoadPlugins()
        {
            IPlugin plugin;
            List<IPlugin> plugins = new List<IPlugin>();
            
            if (!Directory.Exists(PathToPluginsDirectory))
            {
                throw new InvalidOperationException(String.Format(
                    "Невозможно загрузить расширения. Путь {0} не существует", 
                    PathToPluginsDirectory));
            }
            
            string[] files = Directory.GetFiles(PathToPluginsDirectory, "*.dll");

            foreach (string file in files)
            {
                Assembly assembly = Assembly.LoadFrom(file);
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (type.IsClass && typeof(IPlugin).IsAssignableFrom(type))
                    {
                        if (UsingOnlyAppDomain)
                        {
                            plugin = (IPlugin)AppDomain.CurrentDomain
                                .CreateInstanceAndUnwrap(assembly.FullName, type.FullName);
                            plugins.Add(plugin);
                        }
                        else
                        {
                            AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
                            domain.UnhandledException += 
                                new UnhandledExceptionEventHandler(EventHandler_PluginDomain_UnhandledException);
                            plugin = (IPlugin)domain.CreateInstanceAndUnwrap(assembly.FullName, type.FullName);
                            throw new NotImplementedException();
                        }
                    }
                }
            }

        }

        void EventHandler_PluginDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppDomain domain = (AppDomain)sender;
            AppDomain.Unload(domain);
        }

        #endregion
    }
}
