using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    /// <summary>
    /// Интерфейс Presenter паттерна MVP
    /// </summary>
    public interface IPresenter
    {
        IView View { get; }
        String Name { get; }
        IViewRegion ViewRegion { get; set; }
        /// <summary>
        /// Презентер являющийся хостом для данного 
        /// (устанавливается, если данный является регионом)
        /// </summary>
        IPresenter HostPresenter { get; set; }
        /// <summary>
        /// Показывает окно если View - окно, или регион формы если View - контрол
        /// </summary>
        void Show();
    }
}
