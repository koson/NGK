using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mvp.WinApplication;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Mvp.Plugin
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="https://habrahabr.ru/post/242209/"/>
    [Serializable]
    public class PluginsService<T>: ApplicationServiceBase
        where T: PluginBase
    {
        public PluginsService(string pathToPluginsDirectory, 
            bool usingOnlyAppDomain) :
            base("PluginsService")
        {
            PathToPluginsDirectory = pathToPluginsDirectory;
            UsingOnlyAppDomain = usingOnlyAppDomain;
            Plugins = new List<T>().AsReadOnly();
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

        private ReadOnlyCollection<T> _Plugins;
        /// <summary>
        /// Доступные плагины
        /// </summary>
        public ReadOnlyCollection<T> Plugins
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
            T plugin;
            List<T> plugins = new List<T>();
            
            if (!Directory.Exists(PathToPluginsDirectory))
            {
                throw new InvalidOperationException(String.Format(
                    "Невозможно загрузить расширения. Путь {0} не существует", 
                    PathToPluginsDirectory));
            }

            AppDomain.CurrentDomain.AppendPrivatePath(PathToPluginsDirectory);

            string[] files = Directory.GetFiles(PathToPluginsDirectory, "*.dll");

            foreach (string file in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    foreach (Type type in assembly.GetExportedTypes())
                    {
                        if (type.IsClass && typeof(PluginBase).IsAssignableFrom(type))
                        {
                            if (UsingOnlyAppDomain)
                            {
                                plugin = (T)AppDomain.CurrentDomain
                                    .CreateInstanceAndUnwrap(assembly.FullName, type.FullName);
                                plugins.Add(plugin);
                            }
                            else
                            {
                                AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
                                domain.UnhandledException +=
                                    new UnhandledExceptionEventHandler(EventHandler_PluginDomain_UnhandledException);
                                plugin = (T)domain.CreateInstanceAndUnwrap(assembly.FullName, type.FullName);

                                throw new NotImplementedException();
                            }
                        }
                    }
                }
                catch (BadImageFormatException ex)
                {
                    // Если найдена нативная dll
                }
            }
            Plugins = plugins.AsReadOnly();
            AppDomain.CurrentDomain.ClearPrivatePath();
        }

        void EventHandler_PluginDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppDomain domain = (AppDomain)sender;
            AppDomain.Unload(domain);
        }

        #endregion
    }
}
