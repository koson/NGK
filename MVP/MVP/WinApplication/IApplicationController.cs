using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.Presenter;

namespace Mvp.WinApplication
{
    public interface IApplicationController
    {
        /// <summary>
        /// Возвращает текущее окно системы
        /// </summary>
        IPresenter CurrentPresenter { get; }
        Form CurrentForm { get; }
        /// <summary>
        /// Отображает новое окно системы 
        /// </summary>
        /// <param name="presenter"></param>
        void ShowWindow(IPresenter presenter);
        /// <summary>
        /// Отображает модальное окно
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        DialogResult ShowDialog(IPresenter presenter);
    }
}
