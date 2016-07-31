using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Plugin
{
    public interface IPlugin: IDisposable
    {
        #region Properties

        /// <summary>
        /// Название плагина
        /// </summary>
        string Name { get;}

        #endregion

        #region Events
        /// <summary>
        /// Событие происходит после при удалении плагина
        /// </summary>
        event EventHandler PluginRemoving;

        #endregion
    }
}
