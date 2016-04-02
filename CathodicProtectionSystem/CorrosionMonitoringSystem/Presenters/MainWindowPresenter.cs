using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;
using Mvp.Presenter;
using Mvp.View;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class MainWindowPresenter : Presenter<IMainWindowView>
    {
        #region Constructors

        public MainWindowPresenter(IApplicationController application,
            IMainWindowView view, object model, IManagers managers)
            : 
            base(view, application)
        {
            _Name = String.Empty;
            _Managers = managers;
            ViewConcrete.Title = String.Empty;

            _ShowMenuCommand = new Command(OnShowMenu, CanShowMenu);
            _Commands.Add(_ShowMenuCommand);
            _ShowDeviceList = new Command("Устройства", OnShowDeviceList, CanShowDeviceList);
            _ShowPivoteTable = new Command("Параметры ЭХЗ", OnShowPivoteTable, CanShowPivoteTable);
            _ShowLogViewer = new Command("Журнал событий", OnShowLogViewer, CanShowLogViewer);

            ViewConcrete.ShowMenuCommand = _ShowMenuCommand;

            ViewConcrete.ButtonCommands = null;

            IPresenter presenter =
                _Managers.PresentersFactory.Create(ViewMode.PivoteTable);
            WorkingRegionPresenter = presenter;

            base.UpdateStatusCommands();
        }

        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public IMainWindowView ViewConcrete
        {
            get { return (IMainWindowView)base.View; }
        }

        IPresenter _WorkingRegionPresenter;

        public IPresenter WorkingRegionPresenter 
        {
            get { return _WorkingRegionPresenter; }
            set
            {
                if (value.View.ViewType != ViewType.Region)
                {
                    throw new ArgumentException(
                        "Попытка установить значение недопустимого типа", 
                        "WorkingRegionPresenter");
                }
                _WorkingRegionPresenter = value;
                _WorkingRegionPresenter.ViewRegion = ViewConcrete.WorkingRegion;
                _WorkingRegionPresenter.HostPresenter = this;

                if (_WorkingRegionPresenter is ISystemButtons)
                {
                    ISystemButtons buttons = _WorkingRegionPresenter as ISystemButtons;

                    if (buttons.ButtonCommands != null)
                    {
                        if (buttons.ButtonCommands.Length > 3)
                        {
                            throw new Exception("Попытка установить недопустимое значение количества " +
                                "команд присоединяемых к системным кнопкам");
                        }
                        else
                        {
                            ViewConcrete.ButtonCommands = buttons.ButtonCommands;
                        }
                    }
                    else
                    {
                        ViewConcrete.ButtonCommands = null; 
                    }
                }

                _WorkingRegionPresenter.Show();
                OnWorkingRegionChanged();
            }
        }

        public string Title
        {
            get { return ViewConcrete.Title; }
            set { ViewConcrete.Title = value; }
        }

        ViewMode? CurrentViewMode
        {
            get
            {
                if (WorkingRegionPresenter != null)
                {
                    if (Enum.IsDefined(typeof(ViewMode), WorkingRegionPresenter.Name))
                    {
                        return (ViewMode)Enum.Parse(typeof(ViewMode), WorkingRegionPresenter.Name);
                    }
                    else
                    {
                        return ViewMode.NoSelection;
                    }
                }
                return null;
            }
        }

        #endregion

        #region Methods

        void OnWorkingRegionChanged()
        {
            if (WorkingRegionChanged != null)
            {
                WorkingRegionChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Event

        public event EventHandler WorkingRegionChanged;
        
        #endregion

        #region Commands

        Command _ShowMenuCommand;
        /// <summary>
        /// Отображаем меню приложения
        /// </summary>
        void OnShowMenu()
        {
            ViewMode mode;
            IPresenter presenter;
            INavigationMenuPresenter navigationMenuPresenter;

            if (_WorkingRegionPresenter == null)
                mode = ViewMode.NoSelection;
            else
            {
                IViewMode vm = _WorkingRegionPresenter as IViewMode;
                mode = vm == null ? ViewMode.NoSelection : vm.ViewMode;
                //mode = _Managers.NavigationService.ShowNavigationMenu(mode);

                navigationMenuPresenter =
                    _Managers.PresentersFactory.CreateNavigationMenu();

                //presenter.CurrentViewMode = currentViewMode; // ???? не нужно

                List<ICommand> menuItems = new List<ICommand>();
                menuItems.Add(_ShowPivoteTable);
                menuItems.Add(_ShowDeviceList);
                menuItems.Add(_ShowLogViewer);

                if (_WorkingRegionPresenter is ISysemMenu)
                {
                    ISysemMenu mnu = _WorkingRegionPresenter as ISysemMenu;
                    menuItems.AddRange(mnu.MenuItems);
                }

                navigationMenuPresenter.MenuItems = menuItems.ToArray();

                navigationMenuPresenter.Show();

                mode = navigationMenuPresenter.CurrentViewMode;

                if (mode != vm.ViewMode)
                {
                    if (mode == ViewMode.NoSelection)
                    {
                        WorkingRegionPresenter = null;
                    }
                    else
                    {
                        presenter = _Managers.PresentersFactory.Create(mode);
                        switch (presenter.View.ViewType)
                        {
                            case ViewType.Window:
                            case ViewType.Dialog:
                                {
                                    presenter.Show();
                                    break;
                                }
                            case ViewType.Region:
                                {
                                    WorkingRegionPresenter = presenter;
                                    break;
                                }
                        }
                    }
                }
            }
        }
        bool CanShowMenu()
        {
            return WorkingRegionPresenter != null;
        }

        Command _ShowPivoteTable;
        void OnShowPivoteTable()
        {
            IPresenter presenter =
                _Managers.PresentersFactory.Create(ViewMode.PivoteTable);
            WorkingRegionPresenter = presenter;
        }
        bool CanShowPivoteTable()
        {
            if (CurrentViewMode.HasValue)
            {
                return CurrentViewMode.Value != ViewMode.PivoteTable;
            }
            else
                return false;
        }

        Command _ShowDeviceList;
        void OnShowDeviceList()
        {
            IPresenter presenter =
                _Managers.PresentersFactory.Create(ViewMode.DeviceList);
            WorkingRegionPresenter = presenter;
        }
        bool CanShowDeviceList()
        {
            if (CurrentViewMode.HasValue)
            {
                return CurrentViewMode.Value != ViewMode.DeviceList;
            }
            else
                return false;
        }

        Command _ShowLogViewer;
        void OnShowLogViewer()
        {
            IPresenter presenter =
                _Managers.PresentersFactory.Create(ViewMode.LogViewer);
            WorkingRegionPresenter = presenter;

        }
        bool CanShowLogViewer()
        {
            if (CurrentViewMode.HasValue)
            {
                return CurrentViewMode.Value != ViewMode.LogViewer;
            }
            else
                return false;
        }

        #endregion
    }
}
