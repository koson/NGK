using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Plugin
{
    public abstract class PluginBase//: MarshalByRefObject
    {
        #region Constructors

        public PluginBase()
        {
            _Uid = Guid.NewGuid();
            _Name = "Plugin_" + _Uid.ToString();
        }

        #endregion

        #region Fields And Properties

        protected Guid _Uid;
        protected String _Name;

        public Guid Uid 
        { 
            get { return _Uid; }
            protected set { _Uid = value; }
        }
        /// <summary>
        /// Название плагина
        /// </summary>
        public string Name 
        { 
            get { return _Name; }
            protected set { _Name = value; }
        }

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
