using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.View;
using NGK.CorrosionMonitoringSystem.Managers;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public class DeviceListPresenter : Presenter<IDeviceListView>
    {
        #region Constructors

        public DeviceListPresenter(IApplicationController application,
            IDeviceListView view, object model, IManagers managers)
            : 
            base(view)
        {
            _Name = NavigationMenuItems.DeviceList.ToString();
            _Managers = managers;

            _ShowMenuCommand = new Command(
                new CommandAction(OnShowMenu), new Condition(CanShowMenu));

            view.ButtonClick +=
                new EventHandler<ButtonClickEventArgs>(EventHandler_View_ButtonClick);
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

        #region Fields And Properties
        
        IManagers _Managers;

        IDeviceListView ViewConcrete
        {
            get { return (IDeviceListView)_View; }
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
