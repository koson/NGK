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
        #region Fields And Properties

        SynchronizationContext SyncContext { get; }
        
        ApplicationContext AppContext { get; }
        /// <summary>
        /// Возвращает текущее окно системы
        /// </summary>
        IPresenter CurrentPresenter { get; }
        
        Form CurrentForm { get; }
        /// <summary>
        /// Версия ПО
        /// </summary>
        Version Version { get; }
        
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

        #endregion
    }
}
