using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.WinApplication;
using NGK.CorrosionMonitoringSystem.Managers;
using NGK.CorrosionMonitoringSystem.Views;
using NGK.CorrosionMonitoringSystem.Models;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.Presenters
{
    public class DeviceDetailPresenter: Presenter<IDeviceDetailView>
    {
        #region Constructors

        public DeviceDetailPresenter(IApplicationController application,
            IDeviceDetailView view, object model, IManagers managers, NgkCanDevice device)
            :
            base(view, application)
        {
            _Name = NavigationMenuItems.DeviceDetail.ToString();
            _Managers = managers;

            _ShowMenuCommand = new Command(
                new CommandAction(OnShowMenu), new Condition(CanShowMenu));

            _ParametersContext = new BindingSource();
            _Parameters = new BindingList<Parameter>();
            _ParametersContext.DataSource = _Parameters;

            view.ButtonClick += 
                new EventHandler<ButtonClickEventArgs>(EventHandler_View_ButtonClick);
            view.ButtonF3IsAccessible = false;
            view.ButtonF4IsAccessible = false;
            view.ButtonF5IsAccessible = false;

            Device = device;
            ViewConcrete.ParametersContext = _ParametersContext;
        }


        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public IDeviceDetailView ViewConcrete
        {
            get { return (IDeviceDetailView)base.View; }
        }

        NgkCanDevice _Device;
        public NgkCanDevice Device
        {
            get { return _Device; }
            set 
            { 
                _Device = value;
                if (_Device != null)
                {
                    _Parameters.Clear();
                    foreach (Parameter prm in _Device.Parameters)
                    {
                        _Parameters.Add(prm);
                    }
                }
                else
                {
                    _Parameters.Clear();
                }
            }
        }

        BindingSource _ParametersContext;
        public BindingSource ParametersContext 
        { 
            get { return _ParametersContext; } 
        }

        BindingList<Parameter> _Parameters;

        #endregion

        #region Event Handlers

        void EventHandler_View_ButtonClick(object sender, ButtonClickEventArgs e)
        {
            switch (e.Button)
            {
                case SystemButtons.F2:
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
