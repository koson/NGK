using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using Mvp.View;

namespace MvpApplication.Presenter
{
    public class MainPresenter: IPresenter
    {
        public MainPresenter(IView view) { }

        #region IPresenter Members

        public IView View
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
