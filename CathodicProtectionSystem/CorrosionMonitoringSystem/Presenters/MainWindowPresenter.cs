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
            ViewConcrete.ButtonF3IsAccessible = false;
            ViewConcrete.ButtonF3Text = String.Empty;
            ViewConcrete.ButtonF4IsAccessible = false;
            ViewConcrete.ButtonF4Text = String.Empty;
            ViewConcrete.ButtonF5IsAccessible = false;
            ViewConcrete.ButtonF5Text = String.Empty;

            _ShowMenuCommand = new Command(OnShowMenu, CanShowMenu);
            _Commands.Add(_ShowMenuCommand);

            ViewConcrete.ShowMenuCommand = _ShowMenuCommand;

            ViewConcrete.ButtonClick += 
                new EventHandler<ButtonClickEventArgs>(
                EventHandler_ViewConcrete_ButtonClick);

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
                _WorkingRegionPresenter.Show();
            }
        }

        public string Title
        {
            get { return ViewConcrete.Title; }
            set { ViewConcrete.Title = value; }
        }

        #endregion

        #region Event Handlers of View

        void EventHandler_ViewConcrete_ButtonClick(
            object sender, ButtonClickEventArgs e)
        {
            switch (e.Button)
            {
                case SystemButtons.F2:
                    {
                        _ShowMenuCommand.Execute(); break;
                    }
            }
        }

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
