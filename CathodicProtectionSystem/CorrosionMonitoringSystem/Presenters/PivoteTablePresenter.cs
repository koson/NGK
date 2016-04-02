using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class PivoteTablePresenter : Presenter<IPivotTableView>,
        IViewMode, ISystemButtons, ISystemMenu
    {
        #region Constructors
        
        public PivoteTablePresenter(IApplicationController application,
            IPivotTableView view, IViewRegion region, object model, 
            IManagers managers):
            base(view, region, application)
        {
            _Name = ViewMode.PivoteTable.ToString();
            _Managers = managers;      
        }
        
        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public IPivotTableView ViewConcrete
        {
            get { return (IPivotTableView)base.View; }
        }

        MainWindowPresenter HostWindowPresenter
        {
            get { return (MainWindowPresenter)HostPresenter; }
        }

        public ViewMode ViewMode 
        { 
            get { return ViewMode.PivoteTable; } 
        }

        public ICommand[] MenuItems
        {
            get { return null; }
        }

        public Command[] ButtonCommands
        {
            get { return null; }
        }

        #endregion

        #region Methods

        public override void Show()
        {
            base.Show();
            HostWindowPresenter.Title = @"Таблица параметров";
        }

        #endregion

        #region Event Handlers        
        #endregion

        #region Commands
        #endregion
    }
}
