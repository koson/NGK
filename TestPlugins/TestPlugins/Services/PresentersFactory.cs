using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using TestPlugins.Presenters;
using TestPlugins.Views;

namespace TestPlugins.Services
{
    public static class PresentersFactory
    {
        private static SplashScreenPresenter _SplashScreenPresenter;
        private static MainFormPresenter _MainFormPresenter;

        public static IFormPresenter CreateForm<T>() where T : IFormPresenter
        {
            if (typeof(T) == typeof(SplashScreenPresenter))
            {
                if (_SplashScreenPresenter == null)
                {
                    SplashScreenView view = new SplashScreenView();
                    _SplashScreenPresenter = new SplashScreenPresenter(view);
                }
                return _SplashScreenPresenter;
            }
            else if (typeof(T) == typeof(MainFormPresenter))
            {
                if (_MainFormPresenter == null)
                {
                    MainFormView view = new MainFormView();
                    _MainFormPresenter = new MainFormPresenter(view);
                }
                return _MainFormPresenter;
            }
            else
            {
                throw new NotSupportedException(String.Format("Невозможно создать объект типа {0}", typeof(T)));
            }
        }
    }
}
