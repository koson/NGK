using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;
using Mvp.Presenter;
using Mvp.View;

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

            IPresenter presenter =
                _Managers.PresentersFactory.Create(NavigationMenuItems.PivoteTable);
            WorkingRegionPresenter = presenter;
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

    }
}
