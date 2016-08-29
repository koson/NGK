using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace Infrastructure.Api.Plugins
{
    public class RegionContainer<T> : IRegionContainer
        where T : Control
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

        public IEnumerable<IPartialView> Views
        {
            get
            {
                List<IPartialView> list = new List<IPartialView>();
                foreach (Control control in _Container.Controls)
                {
                    if (control is IPartialView)
                        list.Add(control as IPartialView);
                }
                return list;
            }
        }

        #endregion

        #region Methods

        public void Add(IPartialView partialView)
        {
            _Container.Controls.Add(partialView.Control as Control);
        }

        #endregion
    }
}
