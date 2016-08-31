using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;

namespace Mvp.View
{
    public class WindowViewContext<T> where T: IWindowView
    {
        #region Fields And Properties

        private WindowPresenter<T> _Presenter;

        public WindowPresenter<T> Presenter
        {
            get { return _Presenter; }
            set { _Presenter = value; }
        }

        #endregion
    }
}
