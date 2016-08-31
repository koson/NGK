using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using PluginA.Views;

namespace PluginA.Presenters
{
    public class TestFormPresenter: WindowPresenter<TestView>
    {
        #region Constructors
        #endregion

        #region Fields And Properties
        #endregion

        #region Methods

        public override void Show()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
