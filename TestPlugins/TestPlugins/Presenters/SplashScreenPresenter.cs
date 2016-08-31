using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using TestPlugins.Views;
using Mvp.WinApplication;

namespace TestPlugins.Presenters
{
    public class SplashScreenPresenter: WindowPresenter<SplashScreenView>
    {
        #region Constructors

        public SplashScreenPresenter() : base() {}

        #endregion

        #region Methods
        
        public void WriteLine(string output)
        {
            View.Form.Output(output);
        }

        public override void Show()
        {
            View.Show();
        }

        #endregion
    }
}
