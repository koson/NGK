using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace PluginsInfrastructure
{
    public class RegionContainer<T>: IRegionContainer
        where T: Control
    {
        #region Constructors

        public RegionContainer(string regionName, Control container)
        {
            _Name = regionName;
            _Container = container;
        }

        #endregion

        #region Fields And Properties

        private readonly Control _Container;
        private readonly string _Name;

        public string Name
        {
            get { return _Name; }
        }

        public Control ControlRegionContainer 
        { 
            get { return _Container; } 
        }

        public ReadOnlyCollection<IRegionView> Views
        {
            get 
            {
                List<IRegionView> list = new List<IRegionView>();
                foreach (Control control in _Container.Controls)
                {
                    if (control is IRegionView)
                        list.Add(control as IRegionView);
                }
                return list.AsReadOnly(); 
            }
        }

        #endregion

        #region Methods

        public void Add(IRegionView partialView)
        {
            if (partialView is Control)
                _Container.Controls.Add(partialView as Control);
            else
                throw new ArgumentException("Невозможно добавить частичное предстваление, " +
                    "т.к. не является наследником Control", "partialView");
        }

        #endregion
    }
}
