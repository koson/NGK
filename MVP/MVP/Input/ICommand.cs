using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Input
{
    [Command]
    public interface ICommand
    {
        /// <summary>
        /// —осто€ние команды: можно выполнить или нет
        /// </summary>
        bool Status { get; }
        /// <summary>
        /// «апускает команду на исполнение
        /// </summary>
        void Execute();
        /// <summary>
        /// ¬озвращает значение, которое показывает может ли
        /// быть выполнена данна€ команда 
        /// </summary>
        bool CanExecute();
        /// <summary>
        /// —обытие возваникает при изменении свойства CanExecute
        /// </summary>
        event EventHandler CanExecuteChanged;
    }
}
