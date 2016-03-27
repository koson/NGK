using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Presenters;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public class NavigationService: INavigationService
    {
        #region Constructors

        private NavigationService( )
        {
            _ShowDeviceList = new Command("Устройства", OnShowDeviceList, CanShowDeviceList);
            _ShowPivoteTable = new Command("Параметры ЭХЗ", OnShowPivoteTable, CanShowPivoteTable);
            _ShowDeviceDetail = new Command("Подробно об устройстве", OnShowDeviceDetail, CanShowDeviceDetail);
            _ShowLogViewer = new Command("Журнал событий", OnShowLogViewer, CanShowLogViewer);
        }

        #endregion

        #region Fields And Properties

        MainWindowPresenter _MainWindowPresenter;
        static object SyncRoot = new object();
        static volatile NavigationService _Instance;

        IManagers _Managers;

        public IManagers Managers
        {
            get { return _Managers; }
            set
            {
                _Managers = value;
            }
        }

        public static NavigationService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_Instance == null)
                            _Instance = new NavigationService();
                    }
                }
                return _Instance;
            }
        }

        public MainWindowPresenter MainWindowPresenter
        {
            get { return _MainWindowPresenter; }
            set 
            {
                if (value != null)
                {
                    if (_MainWindowPresenter != null)
                    {
                        _MainWindowPresenter.WorkingRegionChanged -=
                            EventHandler_MainWindowPresenter_WorkingRegionChanged;
                    }
                    _MainWindowPresenter = value;
                    _MainWindowPresenter.WorkingRegionChanged +=
                        new EventHandler(EventHandler_MainWindowPresenter_WorkingRegionChanged);
                }
                else
                {
                    if (_MainWindowPresenter != null)
                    {
                        _MainWindowPresenter.WorkingRegionChanged -=
                            EventHandler_MainWindowPresenter_WorkingRegionChanged;
                    }
                    _MainWindowPresenter = value;
                }
            }
        }

        public

        ViewMode? CurrentViewMode
        {
            get
            {
                if (MainWindowPresenter != null)
                {
                    if (MainWindowPresenter.WorkingRegionPresenter != null)
                    {
                        if (Enum.IsDefined(typeof(ViewMode), MainWindowPresenter.WorkingRegionPresenter.Name))
                        {
                            return (ViewMode)Enum.Parse(typeof(ViewMode), MainWindowPresenter.WorkingRegionPresenter.Name);
                        }
                        else
                        {
                            return ViewMode.NoSelection;
                        }
                    }                    
                }
                return null;
            }
        }

        #endregion

        #region Methods

        void UpdateCommandStatus()
        {
            _ShowDeviceDetail.CanExecute();
            _ShowDeviceList.CanExecute();
            _ShowLogViewer.CanExecute();
            _ShowPivoteTable.CanExecute();
        }

        #endregion

        #region Event Handlers

        void EventHandler_MainWindowPresenter_WorkingRegionChanged(object sender, EventArgs e)
        {
            UpdateCommandStatus();
        }

        #endregion

        #region Commands

        Command _ShowPivoteTable;
        void OnShowPivoteTable() 
        {
            Debug.WriteLine("Команда _ShowPivoteTable запущена");
            IPresenter presenter = 
                _Managers.PresentersFactory.Create(ViewMode.PivoteTable);
            MainWindowPresenter.WorkingRegionPresenter = presenter;
        }
        bool CanShowPivoteTable() 
        {
            if (MainWindowPresenter == null)
                return false;
            
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
            Debug.WriteLine("Команда _ShowDeviceList запущена");
            IPresenter presenter =
                _Managers.PresentersFactory.Create(ViewMode.DeviceList);
            MainWindowPresenter.WorkingRegionPresenter = presenter;
        }
        bool CanShowDeviceList() 
        {
            if (MainWindowPresenter == null)
                return false;

            if (CurrentViewMode.HasValue)
            {
                return CurrentViewMode.Value != ViewMode.DeviceList;
            }
            else
                return false;
        }

        Command _ShowDeviceDetail;
        void OnShowDeviceDetail() 
        { 
            Debug.WriteLine("Команда _ShowDeviceDetail запущена");
            IPresenter presenter =
                _Managers.PresentersFactory.Create(ViewMode.DeviceDetail);
            MainWindowPresenter.WorkingRegionPresenter = presenter;
        }
        bool CanShowDeviceDetail() 
        {
            if (MainWindowPresenter == null)
                return false;

            if (CurrentViewMode.HasValue)
            {
                if (CurrentViewMode.Value == ViewMode.DeviceList)
                {
                    return ((DeviceListPresenter)MainWindowPresenter
                        .WorkingRegionPresenter).SelectedDevice != null;
                }
                else
                    return false;
            }
            else
                return false; 
        }

        Command _ShowLogViewer;
        void OnShowLogViewer() 
        { 
            Debug.WriteLine("Команда _ShowLogViewer запущена");
            IPresenter presenter =
                _Managers.PresentersFactory.Create(ViewMode.LogViewer);
            MainWindowPresenter.WorkingRegionPresenter = presenter;

        }
        bool CanShowLogViewer() 
        {
            if (MainWindowPresenter == null)
                return false;

            if (CurrentViewMode.HasValue)
            {
                return CurrentViewMode.Value != ViewMode.LogViewer;
            }
            else
                return false;
        }
        
        #endregion

        #region INavigationService Members

        public ViewMode ShowNavigationMenu(ViewMode currentViewMode)
        {
            UpdateCommandStatus();

            INavigationMenuPresenter presenter = 
                _Managers.PresentersFactory.CreateNavigationMenu();
            
            presenter.CurrentViewMode = currentViewMode; // ???? не нужно
            presenter.MenuItems = new ICommand[] { 
                _ShowPivoteTable, _ShowDeviceList, _ShowDeviceDetail, _ShowLogViewer };
            presenter.Show();

            return presenter.CurrentViewMode;
        }

        #endregion
    }
}
