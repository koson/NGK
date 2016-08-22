using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mvp.View
{
    public class PartialView<T>: IPartialView
        where T: Control
    {
        #region Costructors

        public PartialView()
        {
            _Control = Activator.CreateInstance<T>();
        }

        public PartialView(Control control)
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

        public void Hide()
        {
            _Control.Hide();
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            _Control.Dispose();
        }

        #endregion
    }
}
