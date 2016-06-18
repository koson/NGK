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
using NGK.CorrosionMonitoringSystem.Models;

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
     
            //view.Parameters = _Managers.CanNetworkService.ParametersPivotTable;
            view.DeviceParameters = _Managers.CanNetworkService.DeviceParameters;

            if (!_Managers.ConfigManager.PivotTableCorrosionDepthColumnVisble)
                view.HideColumn("Corrosion_depth_200F");

            if (!_Managers.ConfigManager.PivotTableCorrosionSpeedColumnVisble)
                view.HideColumn("Corrosion_speed_2010");

            if (!_Managers.ConfigManager.PivotTableLocationColumnVisble)
                view.HideColumn("Location");

            if (!_Managers.ConfigManager.PivotTableNodeIdColumnVisble)
                view.HideColumn("NodeId");

            if (!_Managers.ConfigManager.PivotTablePolarisationCurrentColumnVisble)
                view.HideColumn("PolarisationCurrent_200С");

            if (!_Managers.ConfigManager.PivotTablePolarisationPotentialColumnVisble)
                view.HideColumn("PolarisationPotential_2008");

            if (!_Managers.ConfigManager.PivotTableProtectionCurrentColumnVisble)
                view.HideColumn("ProtectionCurrent_200B");

            if (!_Managers.ConfigManager.PivotTableProtectionPotentialColumnVisble)
                view.HideColumn("ProtectionPotential_2009");

            if (!_Managers.ConfigManager.PivotTableTamperColumnVisble)
                view.HideColumn("Tamper_2015");
        }
        
        #endregion

        #region Fields And Properties

        IManagers _Managers;
        ParametersPivotTable _ParametersTable;

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
