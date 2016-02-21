using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.View;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public class PivoteTablePresenter: Presenter<IPivotTableView>
    {
        #region Constructors
        
        public PivoteTablePresenter(IApplicationController application,
            IPivotTableView view, object model, IManagers managers):
            base(view)
        {
            _Name = NavigationMenuItems.PivoteTable.ToString();
            _Managers = managers;
            _Application = application;
            
            // настраиваем кнопки
            _View.ButtonF3IsAccessible = false;

            _ShowMenuCommand = new Command(
                new CommandAction(OnShowMenu), new Condition(CanShowMenu));

            view.ButtonClick += 
                new EventHandler<ButtonClickEventArgs>(EventHandler_View_ButtonClick);

            _View.TotalDevices = _Managers.CanNetworkService.Devices.Count;
        }
        
        #endregion

        #region Fields And Properties

        IApplicationController _Application;
        IManagers _Managers;

        public IPivotTableView ViewConcrete
        {
            get { return (IPivotTableView)base.View; }
        }

        #endregion

        #region Event Handlers
        
        void EventHandler_View_ButtonClick(object sender, ButtonClickEventArgs e)
        {
            switch (e.Button)
            {
                case TemplateView.Buttons.F2:
                    {
                        _ShowMenuCommand.Execute();
                        break; 
                    }
            }
        }
        
        #endregion

        #region Commands

        Command _ShowMenuCommand;
        
        void OnShowMenu()
        {
            _Managers.NavigationService.ShowNavigationMenu();
        }

        bool CanShowMenu()
        {
            return true;
        }

        #endregion
    }
}
