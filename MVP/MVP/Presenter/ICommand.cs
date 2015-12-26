using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Presenter
{
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
        bool CanExecute { get; }
        /// <summary>
        /// Событие возваникает при изменении свойства CanExecute
        /// </summary>
        event EventHandler CanExecuteChanged;
    }
}
