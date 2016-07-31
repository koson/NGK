using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Plugin
{
    public abstract class PluginBase: MarshalByRefObject, IPlugin
    {
        #region Fields And Properties

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

        public event EventHandler PluginRemoving;

        #endregion
    }
}
