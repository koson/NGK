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
        string Name { get; set; }

        void Show();
        void Close();
    }
}
