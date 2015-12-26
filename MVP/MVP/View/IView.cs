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
        void Show();
        void Close();
    }
}
