using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Input
{
    [Command]
    public interface ICommand
    {
        /// <summary>
        /// Запускает команду на исполнение
        /// </summary>
        void Execute();
        /// <summary>
        /// Возвращает значение, которое показывает может ли
        /// быть выполнена данная команда 
        /// </summary>
        bool CanExecute();
        /// <summary>
        /// Событие возваникает при изменении свойства CanExecute
        /// </summary>
        event EventHandler CanExecuteChanged;
    }
}
