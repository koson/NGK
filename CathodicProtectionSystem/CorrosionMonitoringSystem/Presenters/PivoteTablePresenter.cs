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
    public class PivoteTablePresenter: Presenter<IPivotTableView>
    {
        #region Constructors
        
        public PivoteTablePresenter(IApplicationController application,
            IPivotTableView view, IViewRegion region, object model, 
            IManagers managers):
            base(view, region, application)
        {
            _Name = NavigationMenuItems.PivoteTable.ToString();
            _Managers = managers;            
        }
        
        #endregion

        #region Fields And Properties

        IManagers _Managers;

        public IPivotTableView ViewConcrete
        {
            get { return (IPivotTableView)base.View; }
        }

        IButtonsPanel _Buttons;

        public IButtonsPanel ButtonsPanel
        {
            get { return _Buttons; }
            set
            {
                _Buttons = value;
                if (_Buttons != null)
                {
                    // настраиваем кнопки
                    //_Buttons = buttons;
                    //_Buttons.ButtonF3IsAccessible = false;
                    //_Buttons.ButtonClick +=
                    //    new EventHandler<ButtonClickEventArgs>(EventHandler_View_ButtonClick);
                }
            }
        }

        #endregion

        #region Event Handlers
        
        void EventHandler_View_ButtonClick(object sender, ButtonClickEventArgs e)
        {
            switch (e.Button)
            {
                case SystemButtons.F2:
                    {
                        break; 
                    }
            }
        }
        
        #endregion

        #region Commands
        #endregion
    }
}
