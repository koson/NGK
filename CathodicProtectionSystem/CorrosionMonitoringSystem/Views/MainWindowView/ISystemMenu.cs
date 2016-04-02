using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Views
{
    /// <summary>
    /// Интерфейс реализуется презентром типа Region и служит
    /// для выдачи комманд сисменого меню презентеру главного окна приложения
    /// </summary>
    public interface ISystemMenu
    {
        ICommand[] MenuItems { get; }
    }
}
