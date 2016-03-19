using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View.Collections.ObjectModel;

namespace Mvp.View
{
    /// <summary>
    /// Интерферйс View паттерна MVP
    /// </summary>
    public interface IView: IDisposable
    {
        #region Properties

        string Name { get; set; }
        ViewType ViewType { get;}
        IViewRegion[] ViewRegions { get; }

        #endregion

        #region Methods

        void Show();
        void Close();

        #endregion
    }
}
