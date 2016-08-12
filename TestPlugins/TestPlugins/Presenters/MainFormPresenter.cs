using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using TestPlugins.Views;
using Mvp.WinApplication;

namespace TestPlugins.Presenters
{
    public class MainFormPresenter: Presenter<MainFormView>
    {
        #region Constructors

        public MainFormPresenter(MainFormView view, IApplicationController application)
            : base(view, application)
        { 
        }

        #endregion
    }
}
