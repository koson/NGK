using System;
using System.Collections.Generic;
using System.Text;
using Mvp;
using Mvp.View;

namespace MvpApplication2.View
{
    public interface IMainScreenView: IView
    {
        bool CommandIsEnabled { get; }
        bool ButtonEnabled { get; set; }
        ICommand RunCommand { get; set; } 
    }
}
