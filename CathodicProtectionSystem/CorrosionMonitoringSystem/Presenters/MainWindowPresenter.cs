using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;
using Mvp.Presenter;
using Mvp.View;
using Mvp.Input;
using System.Diagnostics;
using System.Windows.Forms;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication.Infrastructure;
using Mvp.WinApplication.ApplicationService;
using System.Drawing;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class MainWindowPresenter : WindowPresenter<MainWindowView>, IHostWindow
    {
        #region Constructors

        public MainWindowPresenter()
            : base()
        {
            Name = "Monitoring System Of Cathodic Protection";
            View.Form.Text = "Система коррозионного мониторинга";
            View.Form.Title = "";
            //ViewConcrete.Title = String.Empty;

            _ShowNavigationMenuCommand = new Command(OnShowNavigationMenu, CanShowNavigationMenu);
            _Commands.Add(_ShowNavigationMenuCommand);
            _HideShowFunctionalButtonsPanelCommand = new Command(OnHideShowFunctionalButtonsPanel);
            _Commands.Add(_HideShowFunctionalButtonsPanelCommand);

            //_ShowDeviceListCommand = new Command("Устройства", OnShowDeviceList, CanShowDeviceList);
            //_Commands.Add(_ShowDeviceListCommand);
            //_ShowPivoteTableCommand = new Command("Параметры ЭХЗ", OnShowPivoteTable, CanShowPivoteTable);
            //_Commands.Add(_ShowPivoteTableCommand);
            //_ShowLogViewerCommand = new Command("Журнал событий", OnShowLogViewer, CanShowLogViewer);
            //_Commands.Add(_ShowLogViewerCommand);
            //_RunCorrosionMonitoringSystemCommand = new Command("Пуск",
            //    OnRunCorrosionMonitoringSystem, CanRunCorrosionMonitoringSystem);
            //_Commands.Add(_RunCorrosionMonitoringSystemCommand);
            //_StopCorrosionMonitoringSystemCommand = new Command("Стоп",
            //    OnStopCorrosionMonitoringSystem, CanStopCorrosionMonitoringSystem);
            //_Commands.Add(_StopCorrosionMonitoringSystemCommand);

            //ViewConcrete.FormBorderEnable = _Managers.ConfigManager.FormBorderEnable;
            //ViewConcrete.ShowInTaskbar = _Managers.ConfigManager.ShowInTaskbar;
            //ViewConcrete.CursorEnabled = _Managers.ConfigManager.CursorEnable;
            //ViewConcrete.FullScreen = _Managers.ConfigManager.FullScreen;

            //ViewConcrete.ShowMenuCommand = _ShowMenuCommand;

            //ViewConcrete.ButtonCommands = null;

            //ViewConcrete.TotalDevices = _Managers.CanNetworkService.Devices.Count;
            //ViewConcrete.FaultyDevices = _Managers.CanNetworkService.FaultyDevices;
            //_Managers.CanNetworkService.StatusWasChanged +=
            //    new EventHandler(EventHandler_CanNetworkService_StatusWasChanged);
            //_Managers.CanNetworkService.FaultyDevicesChanged +=
            //    new EventHandler(EventHandler_CanNetworkService_FaultyDevicesChanged);

            //IPresenter presenter =
            //    _Managers.PresentersFactory.Create(ViewMode.PivoteTable);
            //WorkingRegionPresenter = presenter;

            View.Form.Shown += new EventHandler(EventHandler_Form_Shown);
            View.Form.ContextMenuStripChanged += 
                new EventHandler(EventHandler_Form_ContextMenuStripChanged);
            View.Form.FunctionalButtonClick += 
                new EventHandler<EventArgsFunctionalButtonClick>(EventHandler_Form_FunctionalButtonClick);

            View.Form.ShowMenuCommand = _ShowNavigationMenuCommand;

            base.UpdateStatusCommands();
        }

        #endregion

        #region Fields And Properties

        //public IMainWindowView ViewConcrete
        //{
        //    get { return (IMainWindowView)base.View; }
        //}

        //IPresenter _WorkingRegionPresenter;

        //public IPresenter WorkingRegionPresenter 
        //{
        //    get { return _WorkingRegionPresenter; }
        //    set
        //    {
        //        if (value.View.ViewType != ViewType.Region)
        //        {
        //            throw new ArgumentException(
        //                "Попытка установить значение недопустимого типа", 
        //                "WorkingRegionPresenter");
        //        }
        //        _WorkingRegionPresenter = value;
        //        _WorkingRegionPresenter.ViewRegion = ViewConcrete.WorkingRegion;
        //        _WorkingRegionPresenter.HostPresenter = this;

        //        if (_WorkingRegionPresenter is ISystemButtons)
        //        {
        //            ISystemButtons buttons = _WorkingRegionPresenter as ISystemButtons;

        //            if (buttons.ButtonCommands != null)
        //            {
        //                if (buttons.ButtonCommands.Length > 3)
        //                {
        //                    throw new Exception("Попытка установить недопустимое значение количества " +
        //                        "команд присоединяемых к системным кнопкам");
        //                }
        //                else
        //                {
        //                    ViewConcrete.ButtonCommands = buttons.ButtonCommands;
        //                }
        //            }
        //            else
        //            {
        //                ViewConcrete.ButtonCommands = null; 
        //            }
        //        }

        //        _WorkingRegionPresenter.Show();
        //        OnWorkingRegionChanged();
        //    }
        //}

        //public string Title
        //{
        //    get { return ViewConcrete.Title; }
        //    set { ViewConcrete.Title = value; }
        //}

        //ViewMode CurrentViewMode
        //{
        //    get
        //    {
        //        if (WorkingRegionPresenter != null)
        //        {
        //            if (WorkingRegionPresenter is IViewMode)
        //                return (_WorkingRegionPresenter as IViewMode).ViewMode;
        //        }
        //        return ViewMode.NoSelection;
        //    }
        //}

        private IPartialViewPresenter _SelectedPartivalViewPresenter;
        public IPartialViewPresenter SelectedPartivalViewPresenter
        {
            get { return _SelectedPartivalViewPresenter; }
            private set 
            { 
                _SelectedPartivalViewPresenter = value;
                OnSelectedPartivalViewPresenterChanged();
            }
        }


        #endregion

        #region Methods

        void OnSelectedPartivalViewPresenterChanged()
        {
            if (SelectedPartivalViewPresenterChanged != null)
            {
                SelectedPartivalViewPresenterChanged(this, new EventArgs());
            }
        }

        public override void Show()
        {
            //ViewConcrete.TotalDevices = _Managers.CanNetworkService.Devices.Count;
            //ViewConcrete.FaultyDevices = _Managers.CanNetworkService.FaultyDevices;
            //_Managers.CanNetworkService.StatusWasChanged +=
            //    new EventHandler(EventHandler_CanNetworkService_StatusWasChanged);
            //_Managers.CanNetworkService.FaultyDevicesChanged +=
            //    new EventHandler(EventHandler_CanNetworkService_FaultyDevicesChanged);

            //IPresenter presenter =
            //    _Managers.PresentersFactory.Create(ViewMode.PivoteTable);
            //WorkingRegionPresenter = presenter;
            
            //base.UpdateStatusCommands();
            //base.Show();
        }

        public void Show(IPartialViewPresenter presenter)
        {            
            View.WorkingRegion.Controls.Clear();
            View.WorkingRegion.Controls.Add(presenter.View.Control);
            View.Title = presenter.Title;
            
            View.ResetFunctionalButtons();
            List<Button> buttons = new List<Button>(presenter.FunctionalButtons);

            switch(buttons.Count)
            {
                case 0: break;
                case 1:
                    {
                        View.ButtonF3 = buttons[0];
                        View.ButtonF3.Dock = DockStyle.Fill;
                        View.ButtonF3.Show();
                        break;
                    }
                case 2:
                    {
                        View.ButtonF3 = buttons[0];
                        View.ButtonF4 = buttons[1];
                        break;
                    }
                default: // >=3
                    {
                        View.ButtonF3 = buttons[0];
                        View.ButtonF4 = buttons[1];
                        View.ButtonF5 = buttons[3];
                        break; 
                    }
            }

            SelectedPartivalViewPresenter = presenter;
        }

        #endregion

        #region Event Handlers

        private void EventHandler_Form_Shown(object sender, EventArgs e)
        {
            // Создаём навигационное меню приложения
            View.Form.ContextMenuStrip = new ContextMenuStrip();

            foreach (NavigationMenuItem menu in NavigationService.Menu)
            {
                View.Form.ContextMenuStrip.Items.Add(
                    NavigationMenuItemConverter.ConvertTo(menu));
            }

            // Создаём статусную панель формы
            foreach (IPlugin plugin in Program.AppPluginsService.Plugins)
            {
                foreach (ToolStripItem item in plugin.StatusBarItems)
                    View.StatusBar.Items.Add(item);
            }
        }

        private void EventHandler_Form_ContextMenuStripChanged(object sender, EventArgs e)
        {
            _ShowNavigationMenuCommand.CanExecute();
        }

        private void EventHandler_CanNetworkService_StatusWasChanged(object sender, EventArgs e)
        {
            UpdateStatusCommands();
        }

        private void EventHandler_CanNetworkService_FaultyDevicesChanged(
            object sender, EventArgs e)
        {
            //ViewConcrete.FaultyDevices = 
            //    _Managers.CanNetworkService.FaultyDevices;
        }

        private void EventHandler_Form_FunctionalButtonClick(object sender, 
            EventArgsFunctionalButtonClick e)
        {
            switch (e.Button)
            {
                case Keys.F1:
                    {
                        //#if DEBUG
                        //// Показываем окно управления сетями
                        //FormNetworkControl frm = FormNetworkControl.Instance;
                        //frm.TopMost = true;
                        //frm.Show();
                        //return false;
                        //#else
                        break; 
                    }
                case Keys.F2:
                    { break; }
                case Keys.F3:
                    { break; }
                case Keys.F4:
                    { break; }
                case Keys.F5:
                    { break; }
                case Keys.F6:
                    {
                        _HideShowFunctionalButtonsPanelCommand.Execute();
                        break; 
                    }
            }
        }

        #endregion

        #region Event

        public event EventHandler SelectedPartivalViewPresenterChanged;

        #endregion

        #region Commands

        /// <summary>
        /// Отображат меню приложения
        /// </summary>
        private Command _ShowNavigationMenuCommand;
        private void OnShowNavigationMenu()
        {
            // Отображаем меню в центре формы
            Point point =
                new Point(View.Form.ClientRectangle.Width / 2 - View.Form.ContextMenuStrip.ClientRectangle.Width / 2,
                View.Form.ClientRectangle.Height / 2 - View.Form.ContextMenuStrip.ClientRectangle.Height / 2);

            View.Form.ContextMenuStrip.Show(View.Form, point);
        }
        private bool CanShowNavigationMenu()
        {
            return View.Form.ContextMenuStrip != null;
        }

        private Command _HideShowFunctionalButtonsPanelCommand;
        private void OnHideShowFunctionalButtonsPanel()
        {
            View.FunctionalButtonsPanelVisible = !View.FunctionalButtonsPanelVisible;
        }

        //Command _ShowPivoteTableCommand;
        //void OnShowPivoteTable()
        //{
        //    IPresenter presenter =
        //        _Managers.PresentersFactory.Create(ViewMode.PivoteTable);
        //    WorkingRegionPresenter = presenter;
        //}
        //bool CanShowPivoteTable()
        //{
        //    return CurrentViewMode != ViewMode.PivoteTable;
        //}

        //Command _ShowDeviceListCommand;
        //void OnShowDeviceList()
        //{
        //    IPresenter presenter =
        //        _Managers.PresentersFactory.Create(ViewMode.DeviceList);
        //    WorkingRegionPresenter = presenter;
        //}
        //bool CanShowDeviceList()
        //{
        //    return CurrentViewMode != ViewMode.DeviceList;
        //}

        //Command _ShowLogViewerCommand;
        //void OnShowLogViewer()
        //{
        //    IPresenter presenter =
        //        _Managers.PresentersFactory.Create(ViewMode.LogViewer);
        //    WorkingRegionPresenter = presenter;

        //}
        //bool CanShowLogViewer()
        //{
        //    return CurrentViewMode != ViewMode.LogViewer;
        //}

        //Command _RunCorrosionMonitoringSystemCommand;
        //void OnRunCorrosionMonitoringSystem()
        //{
        //    Debug.WriteLine("Команда: запуск системы");
        //    _Managers.CanNetworkService.Start();
        //}
        //bool CanRunCorrosionMonitoringSystem()
        //{
        //    return _Managers.CanNetworkService != null && _Managers.CanNetworkService.Status == 
        //        Common.Controlling.Status.Stopped;
        //}

        //Command _StopCorrosionMonitoringSystemCommand;
        //void OnStopCorrosionMonitoringSystem()
        //{
        //    Debug.WriteLine("Команда: остановить систему");
        //    _Managers.CanNetworkService.Stop();
        //}
        //bool CanStopCorrosionMonitoringSystem()
        //{
        //    return _Managers.CanNetworkService != null && _Managers.CanNetworkService.Status ==
        //        Common.Controlling.Status.Running;
        //}

        #endregion
    }
}
