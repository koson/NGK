using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using TestPlugins.Views;
using Mvp.WinApplication;

namespace TestPlugins.Presenters
{
    public class SplashScreenPresenter: Presenter<SplashScreenView>
    {
        #region Constructors

        public SplashScreenPresenter(SplashScreenView view, IApplicationController application):
            base(view, application)
        {
        }

        #endregion

        #region Methods
        
        public void WriteLine(string output)
        {
            SpecialView.WriteLine(output);
        }

        #endregion
    }
}
