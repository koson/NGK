using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using TestPlugins.Views;
using Mvp.WinApplication;

namespace TestPlugins.Presenters
{
    public class SplashScreenPresenter: FormPresenter<SplashScreenView>
    {
        #region Constructors

        public SplashScreenPresenter(SplashScreenView view) : base(view) {}

        #endregion

        #region Methods
        
        public void WriteLine(string output)
        {
            ((ISplashScreenView)View).Output(output);
        }

        public override void Show()
        {
            View.Show();
        }

        #endregion
    }
}
