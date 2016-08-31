using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mvp.View
{
    public abstract class PartialView<T>: IPartialView
        where T: Control
    {
        #region Costructors

        public PartialView()
        {
            _Control = Activator.CreateInstance<T>();
        }

        public PartialView(T control)
        {
            _Control = control;
        }

        #endregion

        #region Fields And Properties

        private readonly T _Control;
        private PartialViewContext<PartialView<T>> _Context;

        protected T Control { get { return _Control; } }

        Control IPartialView.Control { get { return _Control; } }

        public string Name
        {
            get { return _Control.Name; }
            set { _Control.Name = value; }
        }

        public virtual PartialViewContext<PartialView<T>> Context
        {
            get { return _Context; }
            set { _Context = value; }
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
            OnPartialViewIsShown();
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

        private void OnPartialViewIsShown()
        {
            if (PartialViewIsShown != null)
                PartialViewIsShown(this, new EventArgs());
        }

        #endregion

        #region Events

        public event EventHandler PartialViewIsShown;

        #endregion
    }
}
