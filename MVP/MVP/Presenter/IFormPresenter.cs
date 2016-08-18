using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    public interface IFormPresenter
    {
        IFormView View { get; }
    }
}
