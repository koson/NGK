using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.View
{
    public enum NavigationMenuItems
    {
        /// <summary>
        /// Отмена (переход не выбран)
        /// </summary>
        NoSelection,
        PivoteTable,
        DeviceList,
        DeviceDetail,
        LogViewer,
    }

    public interface INavigationMenuView: IView
    {
        NavigationMenuItems SelectedMenuItem { get; set; }
        bool PivoteTableMenuEnabled { get; set;}
        bool DeviceListEnabled { get; set;}
        bool DeviceDetailEnabled { get; set;}
        bool LogViewerEnabled { get; set;}
        event EventHandler MenuClosed;
    }
}
