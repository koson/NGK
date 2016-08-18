using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using TestPlugins.Views;
using Mvp.WinApplication;

namespace TestPlugins.Presenters
{
    public class MainFormPresenter: FormPresenter<MainFormView>
    {
        #region Constructors

        public MainFormPresenter(MainFormView view) : base(view) { }
        
        #endregion

        #region Fields And Properties
        #endregion

        #region Methods

        public override void Show()
        {
            View.Show();
        }

        #endregion
    }
}
