using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mvp.View
{
    public class RegionView<T>: IRegionView
        where T: Control
    {
        #region Costructors

        public RegionView(Control control)
        {
            _Control = control;
        }

        #endregion

        #region Fields And Properties

        private readonly Control _Control;

        public string Name
        {
            get { return _Control.Name; }
            set { _Control.Name = value; }
        }

        /// <summary>
        /// TODO: это рудимент - проверить и удалить
        /// </summary>
        public ViewType ViewType
        {
            get { return ViewType.Region; }
        }

        #endregion

        #region Methods

        public void Show()
        {
            _Control.Show();
        }

        public void Close()
        {
            _Control.Dispose();
        }

        public void Dispose()
        {
            _Control.Dispose();
        }

        #endregion
    }
}
