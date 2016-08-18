using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    /// <summary>
    /// Базовый класс для реализации диалоговых окон
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DialogFormPresenter<T>: FormPresenter<T>
        where T: IFormView
    {
        #region Constructors

        public DialogFormPresenter(T view)
            : base(view)
        { }

        #endregion
    }
}
