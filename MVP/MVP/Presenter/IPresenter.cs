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
    }
}
