using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Reflection;
using Mvp.View;
using Mvp.Input;
using Mvp.WinApplication;

namespace Mvp.Presenter
{
    /// <summary>
    /// TODO: разделить данный класс на два PresenterWindow и PresenterPartialView
    /// (и возможно PresenterModalWindow - ViewType.ModalWindow)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Presenter<T> : IPresenter, IDisposable 
        where T: IView
    {
        #region Constructor

        public Presenter(T view, IApplicationController application)
        {
            if ((view.ViewType != ViewType.Window) && (view.ViewType != ViewType.Dialog))
            {
                throw new ArgumentException(
                    "Ќевозмоно создать презентер с регионом == null и view не €вл€ющимс€ окном",
                    "view");
            }
            _Region = null;
            _Commands = new List<ICommand>();
            _View = view;
            _Application = application;
        }

        public Presenter(T view, IViewRegion region, 
            IApplicationController application)
        {
            if (((view.ViewType == ViewType.Window) || 
                (view.ViewType == ViewType.Dialog)) && (region != null))
            {
                throw new Exception(
                    "ѕопытка создать презентер имеющий тип View = window и имеющий регион не равный null");
            }
            _Region = region;
            _Commands = new List<ICommand>();
            _View = view;
            _Application = application;
        }

        #endregion

        #region Fields And Properties

        IViewRegion _Region;
        public IViewRegion ViewRegion 
        { 
            get { return _Region; }
            set { _Region = value; }
        }

        IPresenter _HostPresenter;
        public IPresenter HostPresenter
        {
            get { return _HostPresenter; }
            set { _HostPresenter = value; }
        }

        protected List<ICommand> _Commands;
        /// <summary>
        /// ћассив зарегистрированных комманд представител€
        /// </summary>
        public ICommand[] Commands
        {
            get { return _Commands.ToArray(); }
        }

        protected T _View;

        public IView View
        {
            get { return _View; }
        }

        public T SpecialView
        {
            get { return _View; }
        }

        protected string _Name;

        public string Name
        {
            get { return _Name; }
        }

        protected IApplicationController _Application;
        
        public IApplicationController Application
        {
            get { return _Application; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// ќбновл€ет состо€ние команд
        /// </summary>
        protected void UpdateStatusCommands()
        {
            foreach (ICommand cmd in _Commands)
            {
                cmd.CanExecute();
            }
        }

        /// <summary>
        /// ѕровер€ем в классе (в его наследника) наличие полей с
        /// атрибутоа CommandAttribute и если такой найден, то
        /// запускаем в нЄм проверку услови€ выполени€ команды
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FindCommands()
        {
            FieldInfo[] fields = this.GetType().GetFields(
                BindingFlags.Instance | 
                BindingFlags.NonPublic |
                BindingFlags.GetField);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(Command))
                {
                    Command cmd = (Command)field.GetValue(this);
                    MethodInfo method = cmd.GetType().GetMethod("CheckCondition", 
                        BindingFlags.NonPublic |
                        BindingFlags.InvokeMethod |
                        BindingFlags.Instance);
                    method.Invoke(cmd, new object[0]);
                }
                
                //if (field.FieldType.IsDefined(typeof(CommandAttribute), false))
                //{ 
                //}
            }
        }

        public virtual void Dispose() 
        {
            if (View != null)
            {
                View.Dispose();
            }
        }

        protected void InitializeCommands()
        {
            List<ICommand> commands = new List<ICommand>();

            FieldInfo[] fields = this.GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.GetField);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(Command))
                {
                    Command cmd = (Command)field.GetValue(this);
                    commands.Add(cmd);
                }
            }
            _Commands = commands;
        }

        public virtual void Show()
        {
            switch (View.ViewType)
            {
                case ViewType.Window:
                    {
                        Application.ShowWindow(this); break;
                    }
                case ViewType.Dialog:
                    {
                        Application.ShowDialog(this); break;
                    }
                case ViewType.Region:
                    {
                        ViewRegion.Show(View); break;
                    }
                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        #endregion
    }
}
