using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;

namespace Mvp.WinApplication
{
    public interface IApplicationController
    {
        /// <summary>
        /// Возвращает текущее окно системы
        /// </summary>
        IPresenter CurrentWindow { get; }
        /// <summary>
        /// Отображает новое окно системы 
        /// </summary>
        /// <param name="presenter"></param>
        void ShowWindow(IPresenter presenter);
    }
}
