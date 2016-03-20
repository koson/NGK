using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IMainWindowView : IView,
        IButtonsPanel, IStatusPanel
    {
        /// <summary>
        /// ���������� ��������� ����
        /// </summary>
        String Title { get; set; }
        IViewRegion WorkingRegion { get; }
        IViewRegion TitleRegion { get; }
        ICommand ShowMenuCommand { set; }
    }
}