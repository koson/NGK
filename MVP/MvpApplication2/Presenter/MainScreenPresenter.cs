using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using MvpApplication2.View;

namespace MvpApplication2.Presenter
{
    public class MainScreenPresenter: IPresenter
    {
        #region Constructors
        
        public MainScreenPresenter(IApplicationController application,
            IMainScreenView view, object model)
        {
            _Application = application;
            _View = view;
        }
        
        #endregion

        #region Fields And Properties

        private IApplicationController _Application;
        private IMainScreenView _View; 

        public IView View
        {
            get { return _View; }
        }

        #endregion
    }
}
