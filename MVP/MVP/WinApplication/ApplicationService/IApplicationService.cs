using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;

namespace Mvp.WinApplication
{
    /// <summary>
    /// Интерфейс для создания сервиса приложения
    /// </summary>
    public interface IApplicationService: IManageable, IDisposable
    {
        /// <summary>
        /// Наименование сервиса
        /// </summary>
        string ServiceName { get; }
        /// <summary>
        /// Приложение которому принадлежит данный сервис
        /// </summary>
        IApplicationController Application { get; }
        /// <summary>
        /// Возвращает статус сервиса: инициализирован или нет
        /// </summary>
        bool IsInitialized { get; }
        /// <summary>
        /// Инициализирует сервис
        /// </summary>
        /// <param name="context"></param>
        void Initialize(Object context);
    }
}
