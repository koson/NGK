using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using Mvp.WinApplication.Infrastructure;
using Infrastructure.API.Managers;

namespace Infrastructure.Api.Plugins
{
    public interface IPlugin
    {
        #region Properties
        
        /// <summary>
        /// Сервисы приложения предоставляемые плагином
        /// </summary>
        IEnumerable<IApplicationService> ApplicationServices { get; }
        /// <summary>
        /// Корневой элемент навигационного меню предоставляемого плагином
        /// </summary>
        NavigationMenuItem NavigationMenu { get; }
        /// <summary>
        /// Коллекция частиных видов для главной формы приложения
        /// </summary>
        IEnumerable<IPartialViewPresenter> PartialPresenters { get; }
        IManagers Managers { get; set; }
        
        #endregion

        #region Methods

        /// <summary>
        /// Инициализирует сервисы и презентеры плагина.
        /// Вызывается после создания плагина
        /// </summary>
        void Initialize(IHostWindow host, object state);

        #endregion
    }
}
