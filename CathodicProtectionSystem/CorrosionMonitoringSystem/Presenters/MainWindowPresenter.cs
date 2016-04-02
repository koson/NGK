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
                        "ѕопытка установить значение недопустимого типа", 
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
                            throw new Exception("ѕопытка установить недопустимое значение количества " +
                                "команд присоедин€емых к системным кнопкам");
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

        public 

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
        /// ќтображаем меню приложени€
        /// </summary>
        void OnShowMenu()
        {
            ViewMode mode;

            if (_WorkingRegionPresenter == null)
                mode = ViewMode.NoSelection;
            else
            {
                IViewMode vm = _WorkingRegionPresenter as IViewMode;
                mode = vm == null ? ViewMode.NoSelection : vm.ViewMode;
                mode = _Managers.NavigationService.ShowNavigationMenu(mode);

                IPresenter presenter;

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

        #endregion
    }
}
