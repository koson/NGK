using System;
using System.Collections.Generic;
using System.Text;
using PluginsInfrastructure;

namespace PluginA
{
    public class PluginA: Plugin
    {
        #region Constructors

        public PluginA() { }

        #endregion

        #region Fields And Properties

        public override string Name
        {
            get { return "Module A"; }
        }

        #endregion
    }
}
