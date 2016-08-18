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
        /// Возвращает главное окно системы
        /// </summary>
        IFormPresenter MainFormPresenter { get; }
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
        //void ShowWindow(FormPresenter presenter);
        /// <summary>
        /// Отображает модальное окно
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        //DialogResult ShowDialog(DialogFormPresenter presenter);
        /// <summary>
        /// Регистрирует сервис приложения
        /// </summary>
        /// <param name="service"></param>
        void RegisterApplicationService(ApplicationServiceBase service);

        #endregion
    }
}
