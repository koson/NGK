using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Views
{
    /// <summary>
    /// Реализует взаимодействие презентера типа Region
    /// с кнопками панели приложения
    /// </summary>
    public interface ISystemButtons
    {
        /// <summary>
        /// Возвращает команды прикрепляемые к кнопкам F3, F4, F5
        /// на панели кнопок 
        /// </summary>
        Command[] ButtonCommands { get; }
    }
}
