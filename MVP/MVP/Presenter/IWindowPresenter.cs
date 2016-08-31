using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    public interface IWindowPresenter
    {
        IWindowView View { get; }
    }
}
