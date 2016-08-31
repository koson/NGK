using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public class SplashScreenView: WindowView<SplashScreenForm>, ISplashScreenView
    {
        #region Constructors

        #endregion

        #region Methods

        public void WriteLine(string text)
        {
            base.Form.WriteLine(text);
        }

        #endregion

    }
}
