using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;

namespace Mvp.View
{
    /// <summary>
    /// Класс для связи View и Presenter
    /// </summary>
    public class PartialViewContext<T> where T : IPartialView
    {
        #region Fields And Properties
        
        private RegionPresenter<T> _Presenter;

        public RegionPresenter<T> Presenter
        {
            get { return _Presenter; }
            set { _Presenter = value; }
        }

        #endregion
    }
}
