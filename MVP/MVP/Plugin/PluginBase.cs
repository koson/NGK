using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Plugin
{
    public abstract class PluginBase: MarshalByRefObject
    {
        #region Fields And Properties
        /// <summary>
        /// Название плагина
        /// </summary>
        public abstract string Name { get; }

        #endregion

        #region Methods

        public virtual void Dispose()
        {
            OnPluginRemoving();
        }

        private void OnPluginRemoving()
        {
            if (PluginRemoving != null)
                PluginRemoving(this, new EventArgs());
        }

        #endregion

        #region Events
        /// <summary>
        /// Событие происходит после при удалении плагина
        /// </summary>
        public event EventHandler PluginRemoving;

        #endregion
    }
}
