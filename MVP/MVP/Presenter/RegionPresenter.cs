using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    public abstract class RegionPresenter<T> : PresenterBase, IRegionPresenter
        where T: IPartialView
    {
        #region Constructors

        public RegionPresenter()
        {
            _View = Activator.CreateInstance<T>();
        }

        public RegionPresenter(T view)
        {
            _View = view;
        }

        #endregion

        #region Fields And Properties

        private T _View;

        public T View { get { return _View; } }

        IPartialView IRegionPresenter.View
        {
            get { return (IPartialView)_View; }
        }

        #endregion

        #region Methods

        public virtual void Dispose()
        {
            View.Dispose();
        }

        public void Hide()
        {
            View.Hide();
        }

        public override void Show()
        {
            View.Show();
            OnShown();
        }

        private void OnShown()
        {
            if (Shown != null)
                Shown(this, new EventArgs());
        }

        #endregion

        #region Events

        public event EventHandler Shown;

        #endregion
    }
}
