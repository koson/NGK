using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    public abstract class RegionPresenter<T> : PresenterBase, IRegionPresenter
        where T: IRegionView
    {
        #region Constructors

        public RegionPresenter(T view)
        {
            _View = view;
        }

        #endregion

        #region Fields And Properties

        private T _View;

        public T View { get { return _View; } }

        IRegionView IRegionPresenter.View
        {
            get { return (IRegionView)_View; }
        }

        #endregion

        #region Methods

        public virtual void Dispose()
        {
            View.Dispose();
        }

        #endregion
    }
}
