using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Presenters;
using Mvp.View;
//using NGK.CorrosionMonitoringSystem.Views.LogViewerView;
//using NGK.CorrosionMonitoringSystem.Views.DeviceDetailView;

namespace NGK.CorrosionMonitoringSystem.Managers.Factory
{
    public static class PresentersFactory
    {
        #region Constructors
        #endregion

        #region Fields And Properties

        private static SplashScreenPresenter _SplashScreenPresenter;
        private static MainWindowPresenter _MainWindowPresenter; 
        
        #endregion

        #region Methods

        public static IWindowPresenter CreateWindowPresenter<T>()
            where T : IWindowPresenter
        {
            //if (typeof(T) == typeof(SplashScreenPresenter))
            //{
            //    if (_SplashScreenPresenter == null)
            //        _SplashScreenPresenter = new SplashScreenPresenter();
            //    return _SplashScreenPresenter;
            //}
            //else if (typeof(T) == typeof(MainWindowPresenter))
            //{
            //    if (_MainWindowPresenter == null)
            //        _MainWindowPresenter = new MainWindowPresenter();
            //    return _MainWindowPresenter;
            //}
            //else
            {
                throw new NotSupportedException(
                    String.Format("Невозможно создать объект типа {0}", typeof(T)));
            }
        }

        #endregion
    }
}
