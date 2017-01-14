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
using Infrastructure.Api.Controls;
using Mvp.Controls;

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

            _ShowNavigationMenuCommand = 
                new Command(OnShowNavigationMenu, CanShowNavigationMenu);
            _Commands.Add(_ShowNavigationMenuCommand);
            _HideShowFunctionalButtonsPanelCommand = 
                new Command(OnHideShowFunctionalButtonsPanel);
            _Commands.Add(_HideShowFunctionalButtonsPanelCommand);

            _FunctionalButtonPanelVisibility = 
                new FunctionalButton(_HideShowFunctionalButtonsPanelCommand, Keys.F2);
            _FunctionalButtonPanelVisibility.Text = "Скрыть";
            View.AddFunctionalButton(_FunctionalButtonPanelVisibility);

            _FunctionalButtonNavigationMenu = 
                new FunctionalButton(_ShowNavigationMenuCommand, Keys.F3);
            _FunctionalButtonNavigationMenu.Text = "Меню";
            View.AddFunctionalButton(_FunctionalButtonNavigationMenu);


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

            //View.Form.ShowMenuCommand = _ShowNavigationMenuCommand;

            base.UpdateStatusCommands();
        }

        #endregion

        #region Fields And Properties

        private FunctionalButton _FunctionalButtonPanelVisibility;
        private FunctionalButton _FunctionalButtonNavigationMenu;
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
            if (SelectedPartivalViewPresenter != null)
                SelectedPartivalViewPresenter.Close();

            //View.WorkingRegion.Controls.Clear();
            View.WorkingRegion.Controls.Add(presenter.View.Control);
            View.Title = presenter.Title;
            View.AddRangeFunctionalButtons(presenter.FunctionalButtons);

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
                if (menu != null)
                    View.Form.ContextMenuStrip.Items.Add(
                        NavigationMenuItemConverter.ConvertTo(menu));
            }

            // Создаём статусную панель формы
            foreach (IPlugin plugin in Program.AppPluginsService.Plugins)
            {
                foreach (ToolStripItem item in plugin.StatusBarItems)
                    View.StatusBar.Items.Add(item);
            }

            // Устанавливаем вид по умолчанию
            foreach(BindableToolStripMenuItem item in View.Form.ContextMenuStrip.Items)
            {
                foreach(BindableToolStripMenuItem dropDownItem in item.DropDownItems)
                {
                    if (dropDownItem.Action != null && dropDownItem.Action.Status)
                        dropDownItem.Action.Execute();
                }   
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
                case Keys.F2: // Скрыть/отобразить пнель
                    {
                        _HideShowFunctionalButtonsPanelCommand.Execute();
                        break; 
                    }
                case Keys.F3: break; // Отобарзить навигационное меню.
                case Keys.F4: break;
                case Keys.F5: break;
                case Keys.F6: break; 
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
