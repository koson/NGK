using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using Mvp.Presenter.Collections;

namespace Mvp.Presenter
{
    /// <summary>
    /// Базовый класс для реализации форм
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WindowPresenter<T> : PresenterBase, IWindowPresenter
        where T: IWindowView
    {
        #region Constructors

        public WindowPresenter()
        {
            _View = Activator.CreateInstance<T>();
            _Regions = new RegionsCollections();
            foreach (IRegionContainer regionContainer in _View.Regions)
            {
                _Regions.Add(new Region(regionContainer));
            }
        }

        #endregion

        #region Fields And Properties

        private readonly T _View;

        private RegionsCollections _Regions;
        /// <summary>
        /// Регионы формы
        /// </summary>
        public RegionsCollections Regions { get { return _Regions; } }
        
        public T View { get { return _View; } }

        IWindowView IWindowPresenter.View
        {
            get { return (IWindowView)_View; }
        }

        #endregion

        #region Methods

        public override void Show()
        {
            View.Show();
        }

        public override void Close()
        {
            foreach (Region region in Regions)
            {
                if (!region.IsEmpty)
                {
                    foreach (IRegionPresenter presenter in region.RegionPresenters)
                        presenter.Close();
                }
            }

            View.Close();
        }

        #endregion
    }
}
