using System;
using System.Collections.Generic;
using System.Text;
using Mvp;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Managers;
using NGK.CorrosionMonitoringSystem.View;

namespace NGK.CorrosionMonitoringSystem.Presenter
{
    public class DeviceDetailPresenter: Presenter<IDeviceDetailView>
    {
        #region Constructors

        public DeviceDetailPresenter(IApplicationController application,
            IDeviceDetailView view, object model, IManagers managers)
            :
            base(view)
        {
            _Name = NavigationMenuItems.DeviceDetail.ToString();
            _Application = application;
            _Managers = managers;

            _ShowMenuCommand = new Command(
                new CommandAction(OnShowMenu), new Condition(CanShowMenu));

            view.ButtonClick += 
                new EventHandler<ButtonClickEventArgs>(EventHandler_View_ButtonClick);
        }


        #endregion

        #region Fields And Properties

        IApplicationController _Application;
        IManagers _Managers;

        public IDeviceDetailView ViewConcrete
        {
            get { return (IDeviceDetailView)base.View; }
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
