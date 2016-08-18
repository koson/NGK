using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    public interface IRegionPresenter: IDisposable
    {
        IRegionView View { get; }
        void Close();
    }
}
