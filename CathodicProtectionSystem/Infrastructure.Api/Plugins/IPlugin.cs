using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using Mvp.WinApplication.Infrastructure;
using Infrastructure.API.Managers;
using System.Windows.Forms;

namespace Infrastructure.Api.Plugins
{
    public interface IPlugin
    {
        #region Properties
        
        /// <summary>
        /// —ервисы приложени€ предоставл€емые плагином
        /// </summary>
        IEnumerable<IApplicationService> ApplicationServices { get; }
        /// <summary>
        ///  орневой элемент навигационного меню предоставл€емого плагином
        /// </summary>
        NavigationMenuItem NavigationMenu { get; }
        /// <summary>
        ///  оллекци€ частиных видов дл€ главной формы приложени€
        /// </summary>
        IEnumerable<IPartialViewPresenter> PartialPresenters { get; }
        /// <summary>
        /// Ёлементы статусной строки предоставл€емые плагином
        /// </summary>
        IEnumerable<ToolStripItem> StatusBarItems { get; }
        /// <summary>
        ///  онтейнер дл€ менеджеров различных ресурсов приложени€
        /// </summary>
        IManagers Managers { get; }
        
        #endregion

        #region Methods

        /// <summary>
        /// »нициализирует сервисы и презентеры плагина.
        /// ¬ызываетс€ после создани€ плагина
        /// </summary>
        void Initialize(IHostWindow host, IManagers managers, object state);

        #endregion
    }
}
