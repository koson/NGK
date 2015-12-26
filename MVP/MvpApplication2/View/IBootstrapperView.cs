using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace MvpApplication2.View
{
    public interface IBootstrapperView : IView
    {
        void WriteLine(string text);
        event EventHandler ViewShown;
    }
}
