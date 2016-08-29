using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class SplashScreenPresenter: FormPresenter<SplashScreenView>
    {
        #region Constructors

        public SplashScreenPresenter() { }

        public SplashScreenPresenter(SplashScreenPresenter view): base(view)
        {
            _Name = "SplashScreen";
        }

        #endregion

        #region Fields And Properties
        #endregion

        #region EventHandler
        #endregion

        #region Methods

        /// <summary>
        /// ¬ыводит текстовую строку в представлении 
        /// </summary>
        /// <param name="text"></param>
        public void WtriteText(string text)
        {
            View.WriteLine(text);
        }

        public override void Show()
        {
            View.Show();
        }

        #endregion

        #region Events
        #endregion
    }
}
