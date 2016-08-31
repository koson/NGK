using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    /// <summary>
    /// ������� ����� ��� ���������� ���������� ����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DialogWindowPresenter<T>: WindowPresenter<T>
        where T: IWindowView
    {
        #region Constructors

        public DialogWindowPresenter()
            : base()
        { }

        #endregion
    }
}
