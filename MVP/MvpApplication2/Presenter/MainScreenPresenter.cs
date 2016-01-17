using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Mvp.Input;
using Mvp.Presenter;
using Mvp.View;
using Mvp.WinApplication;
using MvpApplication2.View;

namespace MvpApplication2.Presenter
{
    public class MainScreenPresenter: Presenter<IMainScreenView>
    {
        #region Constructors
        
        public MainScreenPresenter(IApplicationController application,
            IMainScreenView view, object model): base(view)
        {
            _Application = application;
            _RunCommand = new Command(OnRunCommand, CanRunCommand);
            _Commands.Add(_RunCommand);
            view.CheckBoxChanged += new EventHandler(view_CheckBoxChanged);
            UpdateStatusCommands();
            //InitializeCommands();
        }
        
        #endregion

        #region Fields And Properties

        IApplicationController _Application;
        IMainScreenView _ConcreteView { get { return (IMainScreenView)View; } }
        #endregion

        #region Event Handlers

        void view_CheckBoxChanged(object sender, EventArgs e)
        {
            UpdateStatusCommands();
        }

        #endregion

        #region Commands

        Command _RunCommand;
        
        void OnRunCommand()
        {
            
        }

        bool CanRunCommand()
        {
            bool val = _ConcreteView.CommandIsEnabled;
            _ConcreteView.ButtonEnabled = val;
            return val;
        }
        #endregion
    }
}
