using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public enum ViewMode
    {
        /// <summary>
        /// Отмена (переход не выбран)
        /// </summary>
        NoSelection,
        PivoteTable,
        DeviceList,
        DeviceDetail,
        LogViewer
    }

    public interface INavigationMenuView: IView
    {
        ViewMode SelectedMenuItem { get; set; }
        bool PivoteTableMenuEnabled { get; set;}
        bool DeviceListMenuEnabled { get; set;}
        bool DeviceDetailMenuEnabled { get; set;}
        bool LogViewerMenuEnabled { get; set;}

        ICommand[] MenuItems { set; }

        event EventHandler MenuClosed;
    }
}
