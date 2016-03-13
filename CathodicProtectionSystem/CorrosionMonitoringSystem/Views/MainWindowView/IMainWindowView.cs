using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IMainWindowView: IView
    {
        /// <summary>
        /// ¬озвращает заголовок окна
        /// </summary>
        String Title { get; set; }
        /// <summary>
        /// ¬озвращает или устанавливает контрол в основной регион окна
        /// </summary>
        UserControl CurrentControl { get; set; }
    }
}
