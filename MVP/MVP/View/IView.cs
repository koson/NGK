using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.View
{
    /// <summary>
    /// Интерферйс View паттерна MVP
    /// </summary>
    public interface IView
    {
        #region Properties

        string Name { get; set; }
        ViewType ViewType { get;}

        #endregion

        #region Methods

        void Show();
        void Close();

        #endregion
    }
}
