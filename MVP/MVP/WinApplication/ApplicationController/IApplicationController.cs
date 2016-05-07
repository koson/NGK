using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.Presenter;
using System.Threading;

namespace Mvp.WinApplication
{
    public interface IApplicationController
    {
        #region Properties

        SynchronizationContext SyncContext { get; }
        
        ApplicationContext AppContext { get; }
        /// <summary>
        /// Возвращает текущее окно системы
        /// </summary>
        IPresenter CurrentPresenter { get; }
        /// <summary>
        /// Текущая форма
        /// </summary>
        Form CurrentForm { get; }
        /// <summary>
        /// Версия ПО
        /// </summary>
        Version Version { get; }
        /// <summary>
        /// Сервисы приложения
        /// </summary>
        IApplicationService[] AppServices { get; }

        #endregion

        #region Methods
        
        /// <summary>
        /// Отображает новое окно системы 
        /// </summary>
        /// <param name="presenter"></param>
        void ShowWindow(IPresenter presenter);
        /// <summary>
        /// Отображает модальное окно
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        DialogResult ShowDialog(IPresenter presenter);
        /// <summary>
        /// Регистрирует сервис приложения
        /// </summary>
        /// <param name="service"></param>
        void RegisterApplicationService(IApplicationService service);

        #endregion
    }
}
