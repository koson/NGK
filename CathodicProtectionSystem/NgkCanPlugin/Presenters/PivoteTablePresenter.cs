using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using Infrastructure.Api.Plugins;
using NGK.CorrosionMonitoringSystem.Views;
using Infrastructure.Api.Controls;
using Infrastructure.Api.Models.CAN;
using System.ComponentModel;
using NGK.Plugins.Views;


namespace NGK.Plugins.Presenters
{
    public class PivoteTablePresenter : PartialViewPresenter<PivotTableView>
    {
        #region Constructors

        public PivoteTablePresenter(NgkCanPlugin plugin)
        {
            _Plugin = plugin;
                 
            //view.Parameters = _Managers.CanNetworkService.ParametersPivotTable;
            View.ParametersPivotTable = _Plugin.CanNetworkService.ParametersPivotTable; ;

            if (!_Plugin.Managers.ConfigManager.PivotTableCorrosionDepthColumnVisble)
                View.HideColumn("Corrosion_depth_200F");

            if (!_Plugin.Managers.ConfigManager.PivotTableCorrosionSpeedColumnVisble)
                View.HideColumn("Corrosion_speed_2010");

            if (!_Plugin.Managers.ConfigManager.PivotTableLocationColumnVisble)
                View.HideColumn("Location");

            if (!_Plugin.Managers.ConfigManager.PivotTableNodeIdColumnVisble)
                View.HideColumn("NodeId");

            if (!_Plugin.Managers.ConfigManager.PivotTablePolarisationCurrentColumnVisble)
                View.HideColumn("PolarisationCurrent_200С");

            if (!_Plugin.Managers.ConfigManager.PivotTablePolarisationPotentialColumnVisble)
                View.HideColumn("PolarisationPotential_2008");

            if (!_Plugin.Managers.ConfigManager.PivotTableProtectionCurrentColumnVisble)
                View.HideColumn("ProtectionCurrent_200B");

            if (!_Plugin.Managers.ConfigManager.PivotTableProtectionPotentialColumnVisble)
                View.HideColumn("ProtectionPotential_2009");

            if (!_Plugin.Managers.ConfigManager.PivotTableTamperColumnVisble)
                View.HideColumn("Tamper_2015");
        }
        
        #endregion

        #region Fields And Properties


        private readonly NgkCanPlugin _Plugin;
        private readonly FunctionalButton _ButtonDevicesList;
        ParametersPivotTable _ParametersTable;

        public override string Title
        {
            get { return "Таблица параметров"; }
        }

        #endregion

        #region Methods

        public override void Show()
        {
            _Plugin.Managers.PartialViewService.Host.Show(this);
            base.Show();
        }

        public override void Close()
        {
            View.Close();
            base.Close();
        }
        #endregion

        #region Event Handlers        
        #endregion

        #region Commands
        #endregion
    }
}
