using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;

namespace Mvp.WinApplication
{
    public class ApplicationStartingEventArgs:EventArgs
    {
        #region Constructors

        public ApplicationStartingEventArgs()
        {
            _SplashCreen = null;
        }

        public ApplicationStartingEventArgs(IPresenter splashScreen)
        {
            _SplashCreen = splashScreen;
        }

        #endregion

        #region Fields And Properties

        private IPresenter _SplashCreen;

        public IPresenter SplashScreen
        {
            get { return _SplashCreen; }
            set { _SplashCreen = value; }
        }

        #endregion
    }
}
